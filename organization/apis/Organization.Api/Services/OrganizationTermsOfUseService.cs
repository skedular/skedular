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
    IMemoryCache memoryCache,
    TimeProvider timeProvider)
    : IOrganizationTermsOfUseService
{
    private const string ActiveTermsOfUseCacheKey = "organization-active-term-of-use";
    private static readonly TimeSpan ActiveTermsOfUseCacheDuration = TimeSpan.FromHours(1);

    public async Task<TermsOfUse> GetActiveTermsOfUseAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue<CachedTermsOfUse>(ActiveTermsOfUseCacheKey, out var cachedTermsOfUse) &&
            cachedTermsOfUse is not null &&
            cachedTermsOfUse.ExpiresAt > timeProvider.GetUtcNow())
        {
            return cachedTermsOfUse.Value;
        }

        var termsOfUse = await repositoryFactory.TermsOfUseRepository.GetActiveAsync(cancellationToken);
        var mappedTermsOfUse = mapper.MapTo(termsOfUse)!;

        memoryCache.Set(
            ActiveTermsOfUseCacheKey,
            new CachedTermsOfUse(mappedTermsOfUse, timeProvider.GetUtcNow().Add(ActiveTermsOfUseCacheDuration)),
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = ActiveTermsOfUseCacheDuration });

        return mappedTermsOfUse;
    }

    public async Task<Shared.Database.Entities.TermsOfUse> GetActiveTermsOfUseEntityAsync(CancellationToken cancellationToken) =>
        await repositoryFactory.TermsOfUseRepository.GetActiveAsync(cancellationToken);

    private sealed record CachedTermsOfUse(TermsOfUse Value, DateTimeOffset ExpiresAt);
}
