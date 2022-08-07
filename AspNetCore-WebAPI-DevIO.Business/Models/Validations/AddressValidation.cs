using FluentValidation;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations
{
	public class AddressValidation : AbstractValidator<Address>
	{
		public AddressValidation()
		{
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 200).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.Neighbourhood)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 100).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(8).WithMessage("The field {PropertyName} should contain {MaxLength} characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 100).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 50).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(1, 50).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");
        }
	}
}
