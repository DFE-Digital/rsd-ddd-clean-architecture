using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;

namespace DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories
{
    public interface ISchoolRepository
    {
        Task<School?> GetPrincipalBySchoolAsync(string schoolName, CancellationToken cancellationToken);
        IQueryable<School> GetPrincipalsBySchoolsQueryable(List<string> schoolNames);

    }
}
