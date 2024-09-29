using DfE.DomainDrivenDesignTemplate.Domain.Common;

namespace DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories
{
    public interface ISclRepository<TAggregate> : IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}
