using FluentValidation;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalsBySchools
{
    public class GetPrincipalsBySchoolsQueryValidator : AbstractValidator<GetPrincipalsBySchoolsQuery>
    {
        public GetPrincipalsBySchoolsQueryValidator()
        {
            RuleFor(x => x.SchoolNames)
                .NotNull().WithMessage("School names cannot be null.")
                .NotEmpty().WithMessage("School names cannot be empty.")
                .Must(c => c.Count > 0).WithMessage("At least one school must be provided.");
        }
    }
}
