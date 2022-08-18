namespace Mailing.Domain.Abstractions
{
	public interface IMailingConfig
	{
		string UserName { get; }
		string ApiKey { get; set; }
		string Host { get; }
		int Port { get; }
		string Email { get; }
		string Password { get; }
		bool EnableSSL { get; }
	}
}
