using FluentValidation;
using SH.Models.Customer;

namespace SH.BLL
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MinimumLength(4).WithMessage("FullName minimum length is 4 characters");

            RuleFor(x => DateOnly.FromDateTime(x.DateOfBirth))
                .NotEmpty().WithMessage("DateOfBirth is required.")
                .LessThan(d => DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("DateOfBirth must not be a future date.");
        }
    }
}