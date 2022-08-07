using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace AspNetCore_WebApi_DevIO.Controllers
{
	// Validation of error notifications, validation of modelstate and validation of the business operation
	[ApiController]
	public abstract class MainController : ControllerBase
	{
		private readonly INotifier Notifier;

		public MainController(INotifier notifier)
		{
			Notifier = notifier;
		}

		protected bool ValidOperation() => !Notifier.HasNotification();

		protected ActionResult CustomResponse(object result = null)
		{
			if (ValidOperation())
			{
				return Ok(new
				{
					success = true,
					data = result
				});
			}

			return BadRequest(new
			{
				success = false,
				errors = Notifier.GetNotifications().Select(x => x.Message)
			});
		}

		// Handle received errors
		protected ActionResult CustomResponse(ModelStateDictionary modelState)
		{
			if (!modelState.IsValid)
			{
				NotifyErrorInvalidModel(modelState);
			}

			return CustomResponse();
		}

		protected void NotifyErrorInvalidModel(ModelStateDictionary modelState)
		{
			var errors = modelState.Values.SelectMany(x => x.Errors);
			foreach (var error in errors)
			{
				var errorMessage = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
				NotifyError(errorMessage);
			}
		}

		protected void NotifyError(string message)
		{
			Notifier.Handle(new Notification(message));
		}

	}
}
