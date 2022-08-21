using AspNetCore_WebApi_DevIO.Services;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using Mailing.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/v{version:apiVersion}")]
	public class AuthController : MainController
	{
		private readonly AuthenticationService AuthenticationService;
		private readonly ITransmissionService EmailService;
		private readonly ILogger Logger;

		public AuthController(INotifier notifier,
							  IUser user,
							  AuthenticationService authenticationService,
							  ITransmissionService emailService,
							  ILogger<AuthController> logger) : base(notifier, user)
		{
			AuthenticationService = authenticationService;
			EmailService = emailService;
			Logger = logger;
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
				Logger.LogInformation($"User {registerUserViewModel.Email} has been registered successfully.");
				return CustomResponse(await AuthenticationService.GenerateJwt(registerUserViewModel.Email));
			}
			foreach (var error in result.Errors)
			{
				NotifyError(error.Description);
				Logger.LogInformation($"User could not be registered. Failed with message: {error.Description}.");
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
				Logger.LogInformation($"User {loginUserViewModel.Email} has been logged in successfully.");
				return CustomResponse(await AuthenticationService.GenerateJwt(loginUserViewModel.Email));
			}
			if (result.IsLockedOut)
			{
				NotifyError("User temporarily locked for failed login attempts");
				Logger.LogInformation($"User {loginUserViewModel.Email} has been temporarily locked for failed login attempts.");
				return CustomResponse(loginUserViewModel);
			}

			NotifyError("Incorrect user or password");
			Logger.LogInformation($"User {loginUserViewModel.Email} provided a non-registered email or an incorrect password.");
			return CustomResponse(loginUserViewModel);
		}

		[HttpPost("refresh-token")]
		public async Task<ActionResult> RefreshToken(RefreshTokenViewModel refreshTokenViewModel)
		{
			var refreshToken = refreshTokenViewModel.RefreshToken;
			if (string.IsNullOrEmpty(refreshToken))
			{
				NotifyError("Informed refresh token is invalid");
				Logger.LogInformation($"Refresh token {refreshTokenViewModel.RefreshToken} is invalid.");
				return CustomResponse();
			}

			var token = await AuthenticationService.GetRefreshToken(Guid.Parse(refreshToken));
			if (token is null)
			{
				NotifyError("Refresh token has expired");
				Logger.LogInformation($"Refresh token {refreshTokenViewModel.RefreshToken} has expired.");
				return CustomResponse();
			}

			var result = await AuthenticationService.GenerateJwt(token.Username);
			Logger.LogInformation($"A new access token {result.AccessToken} has been generated successfully for user {token.Username}.");
			return CustomResponse(result);
		}

		[HttpPost("forgot-password")]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
		{
			var email = forgotPasswordViewModel.Email;
			if (string.IsNullOrEmpty(email))
			{
				NotifyError("Informed email is invalid");
				Logger.LogInformation($"Email {forgotPasswordViewModel.Email} is not valid.");
				return CustomResponse();
			}

			var user = await AuthenticationService.UserManager.FindByEmailAsync(email);
			if (user == null)
			{
				NotifyError("Could not find an user with the provided email.");
				Logger.LogInformation($"The user {forgotPasswordViewModel.Email} could not be found.");
				return CustomResponse();
			}

			var token = await AuthenticationService.UserManager.GeneratePasswordResetTokenAsync(user);
			var url = $"{AuthenticationService.AppSettings.FrontEndBaseURL}/forgot-password?token={token}";
			EmailService.Send(email, "Forgot Password", url);
			Logger.LogInformation($"A new token {token} to reset the password has been generated successfully for user {user.Email}.");

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
				Logger.LogInformation($"The user {resetPasswordViewModel.Email} could not be found.");
				return CustomResponse();
			}

			var result = await AuthenticationService.UserManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(error.Code, error.Description);
					Logger.LogInformation($"Password could not be modified. Failed with message: {error.Description}.");
				}
				return CustomResponse(ModelState);
			}

			Logger.LogInformation($"Password for user {user.Email} has been modified successfully.");
			return CustomResponse(result);
		}
	}
}
