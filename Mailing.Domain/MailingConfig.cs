using Mailing.Domain.Abstractions;

namespace Mailing.Domain
{
	public class MailingConfig : IMailingConfig
	{
		public string UserName { get; set; }
		public string ApiKey { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public bool EnableSSL { get; set; }

		public MailingConfig(string username, string apiKey, string host, int port, string email, string password, bool enableSSL)
		{
			UserName = username;
			ApiKey = apiKey;
			Host = host;
			Port = port;
			Email = email;
			Password = password;
			EnableSSL = enableSSL;
		}
	}
}
