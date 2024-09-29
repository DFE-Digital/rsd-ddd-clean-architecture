using MediatR;

namespace DfE.DomainDrivenDesignTemplate.Domain.Common
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
