using AutoFixture;
using AutoMapper;
using DfE.DomainDrivenDesignTemplate.Application.MappingProfiles;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations
{
    public class AutoMapperCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<IMapper>(composer => composer.FromFactory(() =>
            {
                var profiles = typeof(SchoolProfile).Assembly
                    .GetTypes()
                    .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    foreach (var profileType in profiles)
                    {
                        var profileInstance = (Profile)Activator.CreateInstance(profileType)!;
                        cfg.AddProfile(profileInstance);
                    }
                });

                return config.CreateMapper();
            }));
        }
    }
}
