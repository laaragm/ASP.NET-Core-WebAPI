namespace AspNetCore_WebApi_DevIO.Extensions
{
	public class AppSettings
	{
		public string Secret { get; set; } // Encryption key
		public int HoursToExpire { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; } // Applications that should accept the token
	}
}
