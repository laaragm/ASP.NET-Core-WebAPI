using AspNetCore_WebAPI_DevIO.Business.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore_WebAPI_DevIO.Business.Interfaces
{
	public interface INotifier
	{
		bool HasNotification();
		List<Notification> GetNotifications();
		void Handle(Notification notification);
	}
}
