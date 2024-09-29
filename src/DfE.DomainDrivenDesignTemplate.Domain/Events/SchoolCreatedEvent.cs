using DfE.DomainDrivenDesignTemplate.Domain.Common;
using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;

namespace DfE.DomainDrivenDesignTemplate.Domain.Events
{
    public class SchoolCreatedEvent(School school) : IDomainEvent
    {
        public School School { get; } = school;

        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
