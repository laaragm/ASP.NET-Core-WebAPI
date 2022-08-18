using Mailing.Domain.Abstractions;
using Mailing.Services.Abstractions;
using System;
using System.Net;
using System.Net.Mail;

namespace Mailing.Services
{
	public class TransmissionService : ITransmissionService
	{
		private readonly IMailingConfig Config;

		public TransmissionService(IMailingConfig config)
		{
			Config = config;
		}

		private SmtpClient CreateSmtpClient()
		{
			var client = new SmtpClient
			{
				UseDefaultCredentials = false,
				Port = Config.Port,
				Host = Config.Host,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				EnableSsl = Config.EnableSSL,
				Credentials = new NetworkCredential
				{
					UserName = Config.UserName,
					Password = Config.ApiKey
				}
			};

			return client;
		}

		public bool Send(string to, string subject, string link)
		{
			if(!string.IsNullOrEmpty(to))
				Execute(to, subject, link, false);
			return true;
		}

		private bool Execute(string destination, string subject, string body, bool isBodyHtml)
		{
			try
			{
				MailMessage msg = new MailMessage();
				msg.To.Add(new MailAddress(destination));
				msg.From = new MailAddress(Config.Email);
				msg.Subject = subject;
				msg.Body = body;
				msg.IsBodyHtml = isBodyHtml;
				CreateSmtpClient().Send(msg);
				return true;
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}
