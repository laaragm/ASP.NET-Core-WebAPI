using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore_WebAPI_DevIO.Business.Notifications
{
	public class Notification
	{
		public string Message { get; }

		public Notification(string message)
		{
			Message = message;
		}
	}
}
