using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationTermsOfUseService
{
    Task<TermsOfUse> GetActiveTermsOfUseAsync(CancellationToken cancellationToken);
    Task<Shared.Database.Entities.TermsOfUse> GetActiveTermsOfUseEntityAsync(CancellationToken cancellationToken);
}

public class OrganizationTermsOfUseService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IMemoryCache memoryCache)
    : IOrganizationTermsOfUseService
{
    public async Task<TermsOfUse> GetActiveTermsOfUseAsync(CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync("organization-active-term-of-use",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var termsOfUse = await repositoryFactory.TermsOfUseRepository
                    .Query(new Specification<Shared.Database.Entities.TermsOfUse>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Active
                    }).AsNoTracking().FirstAsync(cancellationToken);

                return mapper.MapTo(termsOfUse)!;
            }))!;

    public async Task<Shared.Database.Entities.TermsOfUse> GetActiveTermsOfUseEntityAsync(
        CancellationToken cancellationToken) =>
        await repositoryFactory.TermsOfUseRepository
            .Query(new Specification<Shared.Database.Entities.TermsOfUse>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Active
            }).FirstAsync(cancellationToken);
}
