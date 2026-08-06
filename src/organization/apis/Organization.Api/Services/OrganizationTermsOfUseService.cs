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
    IGraphQlMapper graphQlMapper,
    IMemoryCache memoryCache,
    TimeProvider timeProvider)
    : IOrganizationTermsOfUseService
{
    private const string ActiveTermsOfUseCacheKey = "organization-active-term-of-use";
    private static readonly TimeSpan s_activeTermsOfUseCacheDuration = TimeSpan.FromHours(1);

    public async Task<TermsOfUse> GetActiveTermsOfUseAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue<CachedTermsOfUse>(ActiveTermsOfUseCacheKey, out var cachedTermsOfUse) &&
            cachedTermsOfUse is not null &&
            cachedTermsOfUse.ExpiresAt > timeProvider.GetUtcNow())
        {
            return cachedTermsOfUse.Value;
        }

        var termsOfUse = await repositoryFactory.TermsOfUseRepository.GetActiveUntrackedAsync(cancellationToken);
        var mappedTermsOfUse = graphQlMapper.MapTo(termsOfUse)!;

        memoryCache.Set(
            ActiveTermsOfUseCacheKey,
            new CachedTermsOfUse(mappedTermsOfUse, timeProvider.GetUtcNow().Add(s_activeTermsOfUseCacheDuration)),
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = s_activeTermsOfUseCacheDuration,
            });

        return mappedTermsOfUse;
    }

    public async Task<Shared.Database.Entities.TermsOfUse> GetActiveTermsOfUseEntityAsync(CancellationToken cancellationToken) =>
        await repositoryFactory.TermsOfUseRepository.GetActiveAsync(cancellationToken);

    private sealed record CachedTermsOfUse(TermsOfUse Value, DateTimeOffset ExpiresAt);
}
