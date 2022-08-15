using AspNetCore_WebApi_DevIO.Data;
using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Services
{
	public class AuthenticationService
	{
		public readonly SignInManager<IdentityUser> SignInManager;
		public readonly UserManager<IdentityUser> UserManager;
		public readonly AppSettings AppSettings;
		public readonly ApplicationDbContext Context;

		public AuthenticationService(SignInManager<IdentityUser> signInManager,
									 UserManager<IdentityUser> userManager,
									 IOptions<AppSettings> appSettings,
									 ApplicationDbContext context)
		{
			SignInManager = signInManager;
			UserManager = userManager;
			AppSettings = appSettings.Value;
			Context = context;
		}

		private ClaimsIdentity GetIdentityClaims(IdentityUser user, IEnumerable<string> roles, IList<Claim> claims)
		{
			claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id)); // Attemps to match subject of the token
			claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // Provides a unique identifier for the JWT
			claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString())); // Time before which the JWT must not be accepted
			claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)); // Issued at
			foreach (var role in roles)
			{
				claims.Add(new Claim("role", role));
			}
			var identityClaims = new ClaimsIdentity();
			identityClaims.AddClaims(claims);

			return identityClaims;
		}

		private string CreateToken(ClaimsIdentity identityClaims)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
			var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
			{
				Issuer = AppSettings.Issuer,
				Audience = AppSettings.Audience,
				Subject = identityClaims,
				Expires = DateTime.UtcNow.AddHours(AppSettings.TokenHoursToExpire),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
			});
			var encodedToken = tokenHandler.WriteToken(token); // Serialize token

			return encodedToken;
		}

		private LoginResponseViewModel GenerateResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken)
		{
			var response = new LoginResponseViewModel
			{
				AccessToken = encodedToken,
				RefreshToken = refreshToken.Token,
				ExpiresIn = TimeSpan.FromHours(AppSettings.TokenHoursToExpire).TotalSeconds,
				UserToken = new UserTokenViewModel
				{
					Id = user.Id,
					Email = user.Email,
					Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
				}
			};

			return response;
		}

		public async Task<LoginResponseViewModel> GenerateJwt(string email)
		{
			var user = await UserManager.FindByEmailAsync(email);
			var roles = await UserManager.GetRolesAsync(user);
			var claims = await UserManager.GetClaimsAsync(user);
			var identityClaims = GetIdentityClaims(user, roles, claims);
			var encodedToken = CreateToken(identityClaims);
			var refreshToken = await GenerateRefreshToken(email);

			return GenerateResponse(encodedToken, user, claims, refreshToken);
		}

		private static long ToUnixEpochDate(DateTime date)
			=> (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

		// Every time we generate a token, we will also generate a refresh token
		private async Task<RefreshToken> GenerateRefreshToken(string email)
		{
			var refreshToken = new RefreshToken
			{
				Username = email,
				ExpirationDate = DateTime.UtcNow.AddHours(AppSettings.RefreshTokenHoursToExpire)
			};
			
			Context.RefreshTokens.RemoveRange(Context.RefreshTokens.Where(x => x.Username == email)); // Every time we want to generate a new token for the user, we'll remove the old ones
			await Context.RefreshTokens.AddAsync(refreshToken); // Add the new refresh token
			await Context.SaveChangesAsync();

			return refreshToken;
		}

		public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
		{
			var token = await Context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == refreshToken);

			return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now ? token : null;
		}
	}
}
