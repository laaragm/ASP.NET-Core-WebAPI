using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_WebApi_DevIO.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "The field {0} is mandatory")]
        [EmailAddress(ErrorMessage = "The field {0} does not have a valid format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
		[StringLength(100, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The passwords are not equal")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginUserViewModel
    {
        [Required(ErrorMessage = "The field {0} is mandatory")]
        [EmailAddress(ErrorMessage = "The field {0} does not have a valid format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(100, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class UserTokenViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimViewModel> Claims { get; set; }
    }

    public class LoginResponseViewModel
    {
        public string AccessToken { get; set; }
        public Guid RefreshToken { get; set; }
        public double ExpiresIn { get; set; }
        public UserTokenViewModel UserToken { get; set; }
    }

    public class ClaimViewModel
    {
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class RefreshToken
	{
        public Guid Id { get; set; } // This property simply exists so it can be the primary key of the table
        public string Username { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpirationDate { get; set; }

		public RefreshToken()
		{
            Id = Guid.NewGuid();
            Token = Guid.NewGuid();
		}
	}

    public class RefreshTokenViewModel
	{
        public string RefreshToken { get; set; }
	}

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
	{
        public string Email { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory")]
        [StringLength(100, ErrorMessage = "The field {0} is mandatory and it should contain between {2} and {1} letters", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The passwords are not equal")]
        public string ConfirmPassword { get; set; }
    }
}
