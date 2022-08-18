using System.Threading.Tasks;

namespace Mailing.Services.Abstractions
{
	public interface ITransmissionService
	{
		bool Send(string to, string subject, string link);
	}
}
