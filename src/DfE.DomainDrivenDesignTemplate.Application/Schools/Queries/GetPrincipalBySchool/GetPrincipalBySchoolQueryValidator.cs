using FluentValidation;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalBySchool
{
    public class GetPrincipalBySchoolQueryValidator : AbstractValidator<GetPrincipalBySchoolQuery>
    {
        public GetPrincipalBySchoolQueryValidator()
        {
            RuleFor(x => x.SchoolName)
                .NotNull().WithMessage("School name cannot be null.")
                .NotEmpty().WithMessage("School name cannot be empty.");
        }
    }
}
