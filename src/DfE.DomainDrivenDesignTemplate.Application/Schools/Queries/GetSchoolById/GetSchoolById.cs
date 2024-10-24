using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;
using MediatR;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetSchoolById
{
    public record GetSchoolByIdQuery(int Key) : IRequest<School>;

    public class GetSchoolByIdQueryHandler(ISclRepository<School> schoolRepository)
        : IRequestHandler<GetSchoolByIdQuery, School>
    {
        public async Task<School> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
        {
            return await schoolRepository.FindAsync(new SchoolId(request.Key));
        }
    }
}
