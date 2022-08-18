using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.Services;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using Mailing.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	[Route("api")]
	public class AuthController : MainController
	{
		private readonly AuthenticationService AuthenticationService;
		private readonly ITransmissionService EmailService;

		public AuthController(INotifier notifier, IUser user, AuthenticationService authenticationService, ITransmissionService emailService) : base(notifier, user)
		{
			AuthenticationService = authenticationService;
			EmailService = emailService;
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
			var result = await AuthenticationService.UserManager.CreateAsync(user, registerUserViewModel.Password);
			if (result.Succeeded)
			{
				await AuthenticationService.SignInManager.SignInAsync(user, isPersistent: false);
				return CustomResponse(await AuthenticationService.GenerateJwt(registerUserViewModel.Email));
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

			var result = await AuthenticationService.SignInManager.PasswordSignInAsync(loginUserViewModel.Email, loginUserViewModel.Password, isPersistent: false, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				return CustomResponse(await AuthenticationService.GenerateJwt(loginUserViewModel.Email));
			}
			if (result.IsLockedOut)
			{
				NotifyError("User temporarily locked for failed login attempts");
				return CustomResponse(loginUserViewModel);
			}

			NotifyError("Incorrect user or password");
			return CustomResponse(loginUserViewModel);
		}

		[HttpPost("refresh-token")]
		public async Task<ActionResult> RefreshToken(RefreshTokenViewModel refreshTokenViewModel)
		{
			var refreshToken = refreshTokenViewModel.RefreshToken;
			if (string.IsNullOrEmpty(refreshToken))
			{
				NotifyError("Informed refresh token is invalid");
				return CustomResponse();
			}

			var token = await AuthenticationService.GetRefreshToken(Guid.Parse(refreshToken));
			if (token is null)
			{
				NotifyError("Refresh token has expired");
				return CustomResponse();
			}

			return CustomResponse(await AuthenticationService.GenerateJwt(token.Username));
		}

		[HttpPost("forgot-password")]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
		{
			var email = forgotPasswordViewModel.Email;
			if (string.IsNullOrEmpty(email))
			{
				NotifyError("Informed email is invalid");
				return CustomResponse();
			}

			var user = await AuthenticationService.UserManager.FindByEmailAsync(email);
			if (user == null)
			{
				NotifyError("Could not find an user with the provided email.");
				return CustomResponse();
			}

			var token = await AuthenticationService.UserManager.GeneratePasswordResetTokenAsync(user);
			var url = $"https://localhost:44358/forgot-password?token={token}";
			EmailService.Send(email, "Forgot Password", url);

			return CustomResponse(token);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
		{
			if (!ModelState.IsValid)
			{
				return CustomResponse(ModelState);
			}

			var user = await AuthenticationService.UserManager.FindByEmailAsync(resetPasswordViewModel.Email);
			if (user == null)
			{
				NotifyError("Could not find an user with the provided email.");
				return CustomResponse();
			}

			var result = await AuthenticationService.UserManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError(error.Code, error.Description);
				return CustomResponse(ModelState);
			}

			return CustomResponse(result);
		}
	}
}
