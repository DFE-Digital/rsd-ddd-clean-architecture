using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Database.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.DomainDrivenDesignTemplate.Infrastructure.Database;

public class SclContext : DbContext
{
    private readonly IConfiguration? _configuration;
    const string DefaultSchema = "scl";
    private readonly IServiceProvider _serviceProvider = null!;

    public SclContext()
    {
    }

    public SclContext(DbContextOptions<SclContext> options, IConfiguration configuration, IServiceProvider serviceProvider)
        : base(options)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }


    public DbSet<PrincipalDetails> PrincipalDetails { get; set; } = null!;
    public DbSet<School> Schools { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration!.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        optionsBuilder.AddInterceptors(new DomainEventDispatcherInterceptor(mediator));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrincipalDetails>(ConfigurePrincipalDetails);
        modelBuilder.Entity<School>(ConfigureSchool);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigurePrincipalDetails(EntityTypeBuilder<PrincipalDetails> principalDetailsConfiguration)
    {
        principalDetailsConfiguration.HasKey(e => e.Id);
        principalDetailsConfiguration.ToTable("PrincipalDetails", DefaultSchema);
        principalDetailsConfiguration.Property(e => e.Id).HasColumnName("PrincipalId")
            .ValueGeneratedOnAdd()
            .HasConversion(
                    v => v.Value,
                    v => new PrincipalId(v))
                .IsRequired();

        principalDetailsConfiguration.Property(e => e.Email).HasColumnName("Email");
        principalDetailsConfiguration.Property(e => e.Phone).HasColumnName("Phone");
        principalDetailsConfiguration.Property(e => e.TypeId).HasColumnName("TypeId");
    }

    private static void ConfigureSchool(EntityTypeBuilder<School> schoolConfiguration)
    {
        schoolConfiguration.HasKey(s => s.Id);
        schoolConfiguration.ToTable("Schools", DefaultSchema);
        schoolConfiguration.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v!.Value,
                v => new SchoolId(v));

        schoolConfiguration.Property(e => e.PrincipalId)
            .HasConversion(
                v => v.Value,
                v => new PrincipalId(v));

        schoolConfiguration.Property(e => e.SchoolName).HasColumnName("SchoolName");

        schoolConfiguration.OwnsOne(e => e.NameDetails, nameDetails =>
        {
            nameDetails.Property(nd => nd.NameListAs).HasColumnName("NameListAs");
            nameDetails.Property(nd => nd.NameDisplayAs).HasColumnName("NameDisplayAs");
            nameDetails.Property(nd => nd.NameFullTitle).HasColumnName("NameFullTitle");
        });

        schoolConfiguration.Property(e => e.LastRefresh).HasColumnName("LastRefresh");

        schoolConfiguration
            .HasOne(c => c.PrincipalDetails)
            .WithOne()
            .HasForeignKey<School>(c => c.PrincipalId)
            .HasPrincipalKey<PrincipalDetails>(m => m.Id);
    }

}
