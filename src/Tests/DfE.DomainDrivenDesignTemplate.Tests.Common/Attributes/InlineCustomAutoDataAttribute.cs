using AutoFixture.Xunit2;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Attributes
{
    public class InlineCustomAutoDataAttribute(object[] values, params Type[] customizations)
        : InlineAutoDataAttribute(new CustomAutoDataAttribute(customizations), values);
}
