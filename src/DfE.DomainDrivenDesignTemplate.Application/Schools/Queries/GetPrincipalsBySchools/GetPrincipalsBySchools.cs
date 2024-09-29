using AutoMapper;
using AutoMapper.QueryableExtensions;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using DfE.DomainDrivenDesignTemplate.Application.Common.Models;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalsBySchools
{
    public record GetPrincipalsBySchoolsQuery(List<string> SchoolNames) : IRequest<List<Principal>>;

    public class GetPrincipalsBySchoolsQueryHandler(
        ISchoolRepository schoolRepository,
        IMapper mapper,
        ICacheService cacheService)
        : IRequestHandler<GetPrincipalsBySchoolsQuery, List<Principal>>
    {
        public async Task<List<Principal>> Handle(GetPrincipalsBySchoolsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Principal_{CacheKeyHelper.GenerateHashedCacheKey(request.SchoolNames)}";

            var methodName = nameof(GetPrincipalsBySchoolsQueryHandler);

            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                var schoolsQuery = schoolRepository
                    .GetPrincipalsBySchoolsQueryable(request.SchoolNames);

                var membersOfParliament = await schoolsQuery
                    .ProjectTo<Principal>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return membersOfParliament;
            }, methodName);
        }
    }

}
