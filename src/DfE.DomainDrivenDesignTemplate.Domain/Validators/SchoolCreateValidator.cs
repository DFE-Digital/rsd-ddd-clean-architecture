using DfE.DomainDrivenDesignTemplate.Domain.Common;
using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using FluentValidation;

namespace DfE.DomainDrivenDesignTemplate.Domain.Validators
{
    public class SchoolCreateValidator : BaseEntityValidator<School>
    {
        protected override IEnumerable<IValidator<School>> GetValidationRules()
        {
            yield return new InlineValidator<School>
            {
                v => v.RuleFor(s => s.SchoolName)
                    .NotEmpty().WithMessage("School name is required.")
                    .MaximumLength(100).WithMessage("School name cannot exceed 100 characters."),

                v => v.RuleFor(s => s.PrincipalDetails)
                    .NotNull().WithMessage("Principal details are required.")
                    .SetValidator(new PrincipalDetailsValidator()),

                v => v.RuleFor(nd => nd.NameDetails.NameListAs)
                    .NotEmpty().WithMessage("NameListAs is required.")
                    .MaximumLength(100).WithMessage("NameListAs cannot exceed 100 characters."),

                v => v.RuleFor(nd => nd.NameDetails.NameDisplayAs)
                    .NotEmpty().WithMessage("NameDisplayAs is required.")
                    .MaximumLength(100).WithMessage("NameDisplayAs cannot exceed 100 characters."),

                v => v.RuleFor(nd => nd.NameDetails.NameFullTitle)
                    .NotEmpty().WithMessage("NameFullTitle is required.")
                    .MaximumLength(100).WithMessage("NameFullTitle cannot exceed 100 characters."),
            };
        }
    }
}
