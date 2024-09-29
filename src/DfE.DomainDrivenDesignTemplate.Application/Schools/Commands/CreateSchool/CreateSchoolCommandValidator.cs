using FluentValidation;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Commands.CreateSchool
{
    public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
    {
        public CreateSchoolCommandValidator()
        {
            RuleFor(x => x.SchoolName)
                .NotEmpty().WithMessage("School name is required.");

            RuleFor(x => x.LastRefresh)
                .NotNull().NotEmpty().WithMessage("Last refresh date cannot be null.");

            RuleFor(x => x.EndDate)
                .NotNull().NotEmpty().WithMessage("End date cannot be null.");

            RuleFor(x => x.NameDetails)
                .NotNull().WithMessage("Name details are required.");

            RuleFor(x => x.PrincipalDetails)
                .NotNull().WithMessage("Principal details are required.");
        }
    }
}
