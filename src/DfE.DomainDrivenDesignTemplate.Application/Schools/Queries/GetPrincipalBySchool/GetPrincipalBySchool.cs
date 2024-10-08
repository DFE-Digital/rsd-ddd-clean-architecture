using AutoMapper;
using DfE.CoreLibs.Caching.Helpers;
using DfE.CoreLibs.Caching.Interfaces;
using DfE.DomainDrivenDesignTemplate.Application.Common.Models;
using MediatR;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetPrincipalBySchool
{
    public record GetPrincipalBySchoolQuery(string SchoolName) : IRequest<Principal>;

    public class GetPrincipalBySchoolQueryHandler(
        ISchoolRepository schoolRepository,
        IMapper mapper,
        ICacheService<IMemoryCacheType> cacheService)
        : IRequestHandler<GetPrincipalBySchoolQuery, Principal?>
    {
        public async Task<Principal?> Handle(GetPrincipalBySchoolQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Principal_{CacheKeyHelper.GenerateHashedCacheKey(request.SchoolName)}";

            var methodName = nameof(GetPrincipalBySchoolQueryHandler);

            return await cacheService.GetOrAddAsync(cacheKey, async () =>
            {
                var principal= await schoolRepository
                    .GetPrincipalBySchoolAsync(request.SchoolName, cancellationToken);

                var result = mapper.Map<Principal?>(principal);

                return result;
            }, methodName);
        }
    }
}
