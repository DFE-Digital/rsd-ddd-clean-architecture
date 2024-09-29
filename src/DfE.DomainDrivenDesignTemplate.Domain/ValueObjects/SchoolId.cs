using DfE.DomainDrivenDesignTemplate.Domain.Common;

namespace DfE.DomainDrivenDesignTemplate.Domain.ValueObjects
{
    public record SchoolId(int Value) : IStronglyTypedId;

}
