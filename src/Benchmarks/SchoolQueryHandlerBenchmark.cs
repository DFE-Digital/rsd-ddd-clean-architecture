using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DfE.DomainDrivenDesignTemplate.Application.Common.Behaviours;
using DfE.DomainDrivenDesignTemplate.Application.MappingProfiles;
using DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalBySchool;
using DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalsBySchools;
using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Database;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using DfE.CoreLibs.Caching.Services;
using DfE.CoreLibs.Caching.Settings;

namespace DfE.DomainDrivenDesignTemplate.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 1, invocationCount: 1)]
    public class SchoolQueryHandlerBenchmark
    {
        [Params("Test School 1")]
        public string? ConstituencyName;

        [Params(true, false)]
        public bool IncludePerformanceBehaviour;

        private IMediator? _mediator;
        private GetPrincipalsBySchoolsQuery? _query;
        private ISchoolRepository? _realRepository;
        private ICacheService? _cacheService;
        private IMapper? _mapper;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SchoolProfile>();
            });
            _mapper = config.CreateMapper();

            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };

            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var cacheSettings = Options.Create(new CacheSettings
            {
                DefaultDurationInSeconds = 600, // 10 minutes
                Durations = new Dictionary<string, int>
                {
                    { nameof(GetPrincipalBySchoolQueryHandler), 300 } // 5 minutes
                }
            });

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MemoryCacheService>();
            _cacheService = new MemoryCacheService(memoryCache, logger, cacheSettings);

            // Register necessary services
            services.AddSingleton(_cacheService);
            services.AddSingleton(_mapper);
            services.AddSingleton<IHttpContextAccessor>(httpContextAccessor);
            services.AddSingleton(loggerFactory);
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            // Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetPrincipalBySchoolQueryHandler).Assembly);

                if (IncludePerformanceBehaviour)
                {
                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
                }
            });


            // Initialize DbContext within the benchmark setup
            var dbContext = InitializeDbContext(services, configuration);

            _realRepository = new SchoolRepository(dbContext);
            services.AddSingleton(_realRepository);

            // Rebuild the provider to include the repository
            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();

            _query = new GetPrincipalsBySchoolsQuery(dbContext.Schools.Select(x => x.SchoolName).Take(100).ToList());
        }

        private static SclContext InitializeDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetConnectionStringFromConfig(configuration);

            if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("DataSource=:memory:"))
            {
                var connection = new SqliteConnection(connectionString ?? "DataSource=:memory:");
                connection.Open();

                services.AddSingleton<DbConnection>(_ => connection);
                services.AddDbContext<SclContext>((sp, options) =>
                {
                    var conn = sp.GetRequiredService<DbConnection>();
                    options.UseSqlite(conn);
                });
            }
            else
            {
                services.AddDbContext<SclContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            }

            services.AddSingleton(configuration);

            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<SclContext>();

            // Ensure the database is created
            dbContext.Database.EnsureCreated();

            SeedTestData(dbContext);

            return dbContext;
        }

        private static void SeedTestData(SclContext context)
        {
            if (!context.Schools.Any())
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

                context.Schools.Add(school1);
                context.Schools.Add(school2);

                context.SaveChanges();
            }
        }

        private static string? GetConnectionStringFromConfig(IConfiguration configuration)
        {
            return configuration.GetConnectionString("DefaultConnection");
        }


        [Benchmark]
        public async Task RunHandlerWithCacheAsync()
        {
            await _mediator?.Send(_query!)!;
        }

        [Benchmark]
        public async Task RunHandlerWithoutCacheAsync()
        {
            var cacheKey = $"Principal_{CacheKeyHelper.GenerateHashedCacheKey(_query?.SchoolNames!)}";
            _cacheService?.Remove(cacheKey);
            await _mediator?.Send(_query!)!;
        }

        public static class Program
        {
            public static void Main(string[] args)
            {
                BenchmarkRunner.Run<SchoolQueryHandlerBenchmark>();
            }
        }
    }
}
