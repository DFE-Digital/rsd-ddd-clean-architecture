using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using FluentValidation;

namespace DfE.DomainDrivenDesignTemplate.Domain.Validators
{
    public class PrincipalDetailsValidator : AbstractValidator<PrincipalDetails>
    {
        public PrincipalDetailsValidator()
        {
            RuleFor(p => p.TypeId)
                .GreaterThan(0).WithMessage("TypeId must be a positive integer.");

            RuleFor(p => p.Email)
                .EmailAddress().WithMessage("A valid email address is required.")
                .When(p => !string.IsNullOrEmpty(p.Email));  // Only validate if email is provided

            RuleFor(p => p.Phone)
                .Matches(@"^\+?\d+$").WithMessage("Phone number must contain only numbers and optional leading +")
                .When(p => !string.IsNullOrEmpty(p.Phone));  // Only validate if phone is provided
        }
    }
}
