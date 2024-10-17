using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetSchools
{
    public record GetSchoolsQuery() : IRequest<IQueryable<School>>;

    public class GetSchoolsQueryHandler(ISclRepository<School> schoolRepository)
        : IRequestHandler<GetSchoolsQuery, IQueryable<School>>
    {
        public Task<IQueryable<School>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(schoolRepository.Query().AsNoTracking());
        }
    }
}
