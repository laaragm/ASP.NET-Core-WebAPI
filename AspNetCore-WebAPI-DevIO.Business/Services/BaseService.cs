using AspNetCore_WebAPI_DevIO.Business.Interfaces;
using AspNetCore_WebAPI_DevIO.Business.Models;
using AspNetCore_WebAPI_DevIO.Business.Notifications;
using FluentValidation;
using FluentValidation.Results;

namespace AspNetCore_WebAPI_DevIO.Business.Services
{
	public abstract class BaseService
	{
        private readonly INotifier Notifier;

        protected BaseService(INotifier notifier)
        {
            Notifier = notifier;
        }

        protected void Notify(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notify(error.ErrorMessage);
            }
        }

        protected void Notify(string message)
        {
            Notifier.Handle(new Notification(message));
        }

        protected bool ExecuteValidation<TV, TE>(TV validation, TE entity) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validation.Validate(entity);

            if (validator.IsValid)
            {
                return true;
            }

            Notify(validator);

            return false;
        }
    }
}
