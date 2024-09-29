using System.Security.Claims;
using AutoFixture;
using DfE.DomainDrivenDesignTemplate.Api.Client.Extensions;
using DfE.DomainDrivenDesignTemplate.Client.Contracts;
using DfE.DomainDrivenDesignTemplate.Client;
using DfE.DomainDrivenDesignTemplate.Tests.Common.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations
{
    public class CustomWebApplicationFactoryCustomization<TProgram> : ICustomization
        where TProgram : class {

        public void Customize(IFixture fixture)
        {
            fixture.Customize<CustomWebApplicationFactory<TProgram>>(composer => composer.FromFactory(() =>
            {

                var factory = new CustomWebApplicationFactory<TProgram>(); 

                var client = factory.CreateClient();

                var config = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "ApiClient:BaseUrl", client.BaseAddress!.ToString() }
                    })
                    .Build();


                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(config);
                services.AddApiClient<ISchoolsClient, SchoolsClient>(config, client);

                var serviceProvider = services.BuildServiceProvider();

                fixture.Inject(factory);
                fixture.Inject(serviceProvider);
                fixture.Inject(client);
                fixture.Inject(serviceProvider.GetRequiredService<ISchoolsClient>());
                fixture.Inject(new List<Claim>());

                return factory;
            }));

        }
    }
}
