using DfE.DomainDrivenDesignTemplate.Domain.Common;

namespace DfE.DomainDrivenDesignTemplate.Domain.ValueObjects
{
    public record PrincipalId(int Value) : IStronglyTypedId;
}
