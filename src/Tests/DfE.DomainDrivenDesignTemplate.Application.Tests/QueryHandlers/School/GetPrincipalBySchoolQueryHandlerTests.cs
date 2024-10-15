using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.DomainDrivenDesignTemplate.Application.Common.Models;
using DfE.DomainDrivenDesignTemplate.Application.MappingProfiles;
using DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalBySchool;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations.Entities;
using DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations.Models;
using NSubstitute;

namespace DfE.DomainDrivenDesignTemplate.Application.Tests.QueryHandlers.School
{
    public class GetPrincipalBySchoolQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(
            typeof(PrincipalCustomization),
            typeof(SchoolCustomization),
            typeof(AutoMapperCustomization<SchoolProfile>))]
        public async Task Handle_ShouldReturnMemberOfParliament_WhenSchoolExists(
            [Frozen] ISchoolRepository mockSchoolRepository,
            [Frozen] ICacheService<IMemoryCacheType> mockCacheService,
            GetPrincipalBySchoolQueryHandler handler,
            GetPrincipalBySchoolQuery query,
            Domain.Entities.Schools.School school,
            IFixture fixture)
        {
            // Arrange
            var expectedMp = fixture.Customize(new PrincipalCustomization()
                {
                    FirstName = school.NameDetails.NameListAs!.Split(",")[1].Trim(),
                    LastName = school.NameDetails.NameListAs.Split(",")[0].Trim(),
                    SchoolName = school.SchoolName,
            }).Create<Principal>();

            var cacheKey = $"Principal_{CacheKeyHelper.GenerateHashedCacheKey(query.SchoolName)}";

            mockSchoolRepository.GetPrincipalBySchoolAsync(query.SchoolName, default)
                .Returns(school);

            mockCacheService.GetOrAddAsync(
                    cacheKey,
                    Arg.Any<Func<Task<Result<Principal>>>>(),
                    Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var callback = callInfo.ArgAt<Func<Task<Result<Principal>>>>(1);
                    return callback();
                });

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMp.FirstName, result.Value?.FirstName);
            Assert.Equal(expectedMp.LastName, result.Value?.LastName);
            Assert.Equal(expectedMp.SchoolName, result.Value?.SchoolName);

            await mockSchoolRepository.Received(1).GetPrincipalBySchoolAsync(query.SchoolName, default);
        }
    }
}
