using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Database;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Repositories;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Security.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureDependencyGroup(
            this IServiceCollection services, IConfiguration config)
        {
            //Repos
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped(typeof(ISclRepository<>), typeof(SclRepository<>));

            //Cache service
            services.AddServiceCaching(config);

            //Db
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<SclContext>(options =>
                options.UseSqlServer(connectionString));

            // Authentication
            services.AddCustomAuthorization(config);

            return services;
        }
    }
}
