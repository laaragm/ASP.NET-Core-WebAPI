using FluentValidation;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations
{
	public class ProductValidation : AbstractValidator<Product>
	{
		public ProductValidation()
		{
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 200).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 1000).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("The field {PropertyName} must be greater than {ComparisonValue}");
        }
	}
}
