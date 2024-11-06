using DfE.DomainDrivenDesignTemplate.Infrastructure.Security.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System.Diagnostics.CodeAnalysis;

namespace DfE.DomainDrivenDesignTemplate.Infrastructure.Security.Authorization
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                var policyConfigs = configuration.GetSection("Authorization:Policies").Get<List<PolicyConfig>>();
                if (policyConfigs != null)
                {
                    foreach (var policyConfig in policyConfigs)
                    {
                        options.AddPolicy(policyConfig.Name, policyBuilder =>
                        {
                            policyBuilder.RequireAuthenticatedUser();

                            if (policyConfig.Roles.Any())
                            {
                                if (string.Equals(policyConfig.Operator, "AND", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Use AND logic: require each role individually
                                    foreach (var role in policyConfig.Roles)
                                    {
                                        policyBuilder.RequireRole(role);
                                    }
                                }
                                else
                                {
                                    // Use OR logic: require any of the roles
                                    policyBuilder.RequireRole(policyConfig.Roles.ToArray());
                                }
                            }

                            if (policyConfig.Claims != null && policyConfig.Claims.Any())
                            {
                                foreach (var claim in policyConfig.Claims)
                                {
                                    policyBuilder.RequireClaim(claim.Type, claim.Values.ToArray());
                                }
                            }
                        });
                    }
                }
            });
            return services;
        }
    }
}