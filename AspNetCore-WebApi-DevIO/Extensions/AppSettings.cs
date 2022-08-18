namespace AspNetCore_WebApi_DevIO.Extensions
{
	public class AppSettings
	{
		public string Secret { get; set; } // Encryption key
		public int TokenHoursToExpire { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; } // Applications that should accept the token
		public int RefreshTokenHoursToExpire { get; set; }
		public string FrontEndBaseURL { get; set; }
	}
}
