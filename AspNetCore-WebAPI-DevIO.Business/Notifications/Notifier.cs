using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore_WebAPI_DevIO.Business.Notifications
{
	public class Notifier : INotifier
	{
		private List<Notification> Notifications;

		public Notifier()
		{
			Notifications = new List<Notification>();
		}

		public void Handle(Notification notification) => Notifications.Add(notification);

		public List<Notification> GetNotifications() => Notifications;

		public bool HasNotification() => Notifications.Any();
	}
}
