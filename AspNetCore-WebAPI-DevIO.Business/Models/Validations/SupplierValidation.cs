using AspNetCore_WebAPI_DevIO.Business.Enums;
using AspNetCore_WebAPI_DevIO.Business.Models.Validations.Documents;
using FluentValidation;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations
{
	public class SupplierValidation : AbstractValidator<Supplier>
	{
		public SupplierValidation()
		{
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("The field {PropertyName} must be filled")
                .Length(2, 200).WithMessage("The field {PropertyName} should contain between {MinLength} and {MaxLength} characters");

            When(f => f.SupplierType == SupplierType.PF, () =>
            {
                RuleFor(x => x.Document.Length).Equal(CPFValidation.CPFSize)
                    .WithMessage("The field Document should contain {ComparisonValue} characters and {PropertyValue} were informed.");
                RuleFor(x => CPFValidation.Validate(x.Document)).Equal(true)
                    .WithMessage("The informed document is invalid.");
            });

            When(f => f.SupplierType == SupplierType.PJ, () =>
            {
                RuleFor(x => x.Document.Length).Equal(CNPJValidation.CNPJSize)
                    .WithMessage("The field Document should contain {ComparisonValue} characters and {PropertyValue} were informed.");
                RuleFor(x => CNPJValidation.Validate(x.Document)).Equal(true)
                    .WithMessage("The informed document is invalid.");
            });
        }
	}
}
