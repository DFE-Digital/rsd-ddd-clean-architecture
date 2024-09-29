using AutoFixture;
using DfE.DomainDrivenDesignTemplate.Tests.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations
{
    public class DbContextCustomization<TContext> : ICustomization where TContext : DbContext
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<DbSet<object>>(() => null!);

            fixture.Customize<TContext>(composer => composer.FromFactory(() =>
            {
                var services = new ServiceCollection();
                var dbContext = DbContextHelper<TContext>.CreateDbContext(services);
                fixture.Inject(dbContext);
                return dbContext;
            }).OmitAutoProperties());
        }
    }
}
