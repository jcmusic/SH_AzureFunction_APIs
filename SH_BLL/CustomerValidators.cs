using FluentValidation;
using SH.Models.Customer;

namespace SH.BLL
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            DateTime dte;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required")
                .MinimumLength(4).WithMessage("FullName minimum length is 4 characters");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("DateOfBirth is required")
                .NotNull().WithMessage("DateOfBirth is required")
                .LessThan(d => DateTime.Today.AddDays(1)).WithMessage("DateOfBirth may not be a future date");
        }
    }
}