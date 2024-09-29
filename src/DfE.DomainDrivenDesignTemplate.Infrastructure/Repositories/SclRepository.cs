using System.Diagnostics.CodeAnalysis;
using DfE.DomainDrivenDesignTemplate.Domain.Common;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Database;

namespace DfE.DomainDrivenDesignTemplate.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class SclRepository<TAggregate>(SclContext dbContext)
        : Repository<TAggregate, SclContext>(dbContext), ISclRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}