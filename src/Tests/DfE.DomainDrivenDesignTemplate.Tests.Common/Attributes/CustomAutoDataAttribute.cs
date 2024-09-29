using AutoFixture.Xunit2;
using DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations;
using DfE.DomainDrivenDesignTemplate.Tests.Common.Helpers;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Attributes
{
    public class CustomAutoDataAttribute(params Type[] customizations)
        : AutoDataAttribute(() => FixtureFactoryHelper.ConfigureFixtureFactory(CombineCustomizations(customizations)))
    {
        private static Type[] CombineCustomizations(Type[] customizations)
        {
            var defaultCustomizations = new[] { typeof(NSubstituteCustomization) };
            return defaultCustomizations.Concat(customizations).ToArray();
        }
    }
}
