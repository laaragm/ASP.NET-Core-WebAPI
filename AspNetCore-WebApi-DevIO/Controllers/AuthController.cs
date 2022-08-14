using AspNetCore_WebApi_DevIO.Extensions;
using AspNetCore_WebApi_DevIO.Services;
using AspNetCore_WebApi_DevIO.ViewModels;
using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	[Route("api")]
	public class AuthController : MainController
	{
		private readonly AuthenticationService AuthenticationService;

		public AuthController(INotifier notifier, IUser user, AuthenticationService authenticationService) : base(notifier, user)
		{
			AuthenticationService = authenticationService;
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
	}
}
