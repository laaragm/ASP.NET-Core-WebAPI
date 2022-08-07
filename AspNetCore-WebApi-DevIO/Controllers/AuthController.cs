using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	[Route("api")]
	public class AuthController : MainController
	{
		private readonly SignInManager<IdentityUser> SignInManager;
		private readonly UserManager<IdentityUser> UserManager;
		private readonly AppSettings AppSettings;

		public AuthController(INotifier notifier, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings) : base(notifier)
		{
			SignInManager = signInManager;
			UserManager = userManager;
			AppSettings = appSettings.Value;
		}

		[HttpPost("register")]
		public async Task<ActionResult> Register(RegisterUserViewModel registerUserViewModel)
		{
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}

			var user = new IdentityUser()
			{
				UserName = registerUserViewModel.Email,
				Email = registerUserViewModel.Email,
				EmailConfirmed = true
			};
			var result = await UserManager.CreateAsync(user, registerUserViewModel.Password);
			if (result.Succeeded)
			{
				await SignInManager.SignInAsync(user, isPersistent: false);
				return CustomResponse(await GenerateJwt(registerUserViewModel.Email));
			}
			foreach (var error in result.Errors)
			{
				NotifyError(error.Description);
			}

			return CustomResponse(registerUserViewModel);
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login(LoginUserViewModel loginUserViewModel)
		{
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}

			var result = await SignInManager.PasswordSignInAsync(loginUserViewModel.Email, loginUserViewModel.Password, isPersistent: false, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				return CustomResponse(await GenerateJwt(loginUserViewModel.Email));
			}
			if (result.IsLockedOut)
			{
				NotifyError("User temporarily locked for failed login attempts");
				return CustomResponse(loginUserViewModel);
			}

			NotifyError("Incorrect user or password");
			return CustomResponse(loginUserViewModel);
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
				Expires = DateTime.UtcNow.AddHours(AppSettings.HoursToExpire),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
			});
			var encodedToken = tokenHandler.WriteToken(token); // Serialize token

			return encodedToken;
		}

		private LoginResponseViewModel GenerateResponse(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
		{
			var response = new LoginResponseViewModel
			{
				AccessToken = encodedToken,
				ExpiresIn = TimeSpan.FromHours(AppSettings.HoursToExpire).TotalSeconds,
				UserToken = new UserTokenViewModel
				{
					Id = user.Id,
					Email = user.Email,
					Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
				}
			};

			return response;
		}

		private async Task<LoginResponseViewModel> GenerateJwt(string email)
		{
			var user = await UserManager.FindByEmailAsync(email);
			var roles = await UserManager.GetRolesAsync(user);
			var claims = await UserManager.GetClaimsAsync(user);
			var identityClaims = GetIdentityClaims(user, roles, claims);
			var encodedToken = CreateToken(identityClaims);

			return GenerateResponse(encodedToken, user, claims);
		}

		private static long ToUnixEpochDate(DateTime date) 
			=> (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
	}
}
