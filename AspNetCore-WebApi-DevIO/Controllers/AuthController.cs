using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
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
				return CustomResponse(GenerateJwt());
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
				return CustomResponse(GenerateJwt());
			}
			if (result.IsLockedOut)
			{
				NotifyError("User temporarily locked for failed login attempts");
				return CustomResponse(loginUserViewModel);
			}

			NotifyError("Incorrect user or password");
			return CustomResponse(loginUserViewModel);
		}

		private string GenerateJwt()
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
			var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
			{
				Issuer = AppSettings.Issuer,
				Audience = AppSettings.Audience,
				Expires = DateTime.UtcNow.AddHours(AppSettings.HoursToExpire),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			});
			var endcodedToken = tokenHandler.WriteToken(token); // Serialize token

			return endcodedToken;
		}
	}
}
