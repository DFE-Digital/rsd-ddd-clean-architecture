using AutoFixture;
using DfE.CoreLibs.Testing.Mocks.Authentication;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using DfE.DomainDrivenDesignTemplate.Api.Client.Extensions;
using DfE.DomainDrivenDesignTemplate.Client;
using DfE.DomainDrivenDesignTemplate.Client.Contracts;
using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Security.Claims;
using SchoolId = DfE.DomainDrivenDesignTemplate.Domain.ValueObjects.SchoolId;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations
{
    public class CustomWebApplicationDbContextFactoryCustomization<TProgram, TDbContext> : ICustomization
        where TProgram : class
        where TDbContext : DbContext
    {
        public Action<TDbContext>? SeedAction { get; set; }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<CustomWebApplicationDbContextFactory<TProgram, TDbContext>>(composer => composer.FromFactory(() =>
            {

                var factory = new CustomWebApplicationDbContextFactory<TProgram, TDbContext>()
                {
                    SeedData = SeedAction ?? DefaultSeedData,
                    ExternalServicesConfiguration = services =>
                    {
                        services.PostConfigure<AuthenticationOptions>(options =>
                        {
                            options.DefaultAuthenticateScheme = "TestScheme";
                            options.DefaultChallengeScheme = "TestScheme";
                        });

                        services.AddAuthentication("TestScheme")
                            .AddScheme<AuthenticationSchemeOptions, MockJwtBearerHandler>("TestScheme", options => { });
                    },
                    ExternalHttpClientConfiguration = client =>
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "external-mock-token");
                    }
                };

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

        private static void DefaultSeedData(TDbContext context)
        {
            if (context is SclContext sclContext && !sclContext.Schools.Any())
            {
                var memberContact1 = new PrincipalDetails(
                    new PrincipalId(1),
                    1,
                    "test1@example.com",
                    null
                );

                var memberContact2 = new PrincipalDetails(
                    new PrincipalId(2),
                    1,
                    "test2@example.com",
                    null
                );

                var school1 = new School(
                    new SchoolId(1),
                    new PrincipalId(1),
                    "Test School 1",
                    new NameDetails(
                        "Wood, John",
                        "John Wood",
                        "Mr. John Wood MP"
                    ),
                    DateTime.UtcNow,
                    null,
                    memberContact1
                );

                var school2 = new School(
                    new SchoolId(2),
                    new PrincipalId(2),
                    "Test School 2",
                    new NameDetails(
                        "Wood, Joe",
                        "Joe Wood",
                        "Mr. Joe Wood MP"
                    ),
                    DateTime.UtcNow,
                    null,
                    memberContact2
                );

                sclContext.Schools.AddRange(school1, school2);
                sclContext.SaveChanges();
            }
        }
    }
}
