using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;

namespace Team.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Organization?> GetByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask UpdateByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask RemoveByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
}

public class CachedOrganizationService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache,
    ILogger<CachedOrganizationService> logger)
    : ICachedOrganizationService
{
    public async ValueTask<Organization?> GetByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyById(id),
                    async ct => await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(id, null, false, ct) ??
                                throw new OrganizationNotFound(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(customDomain))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyByUniqueAlphanumericName(customDomain),
                    async ct =>
                        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(
                            null,
                            customDomain,
                            false,
                            ct) ??
                        throw new OrganizationNotFound(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }

            throw new InvalidOperationException("Either id or customDomain must be provided.");
        }
        catch (OrganizationNotFound)
        {
            logger.LogDebug(
                "Organization lookup returned no result for organization id {OrganizationId} and custom domain {OrganizationCustomDomain}",
                id,
                customDomain);
            return null;
        }
    }

    public async ValueTask UpdateByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        await RemoveByIdOrCustomDomainAsync(id, customDomain, cancellationToken);

        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.SetAsync(
                CreateKeyById(id),
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(id, null, false, cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);

            logger.LogDebug("Cache refresh for organization {OrganizationId}", id);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            await hybridCache.SetAsync(
                CreateKeyByUniqueAlphanumericName(customDomain),
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(
                    null,
                    customDomain,
                    false,
                    cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);

            logger.LogDebug("Cache refresh for organization custom domain {OrganizationCustomDomain}", customDomain);
        }
    }

    public async ValueTask RemoveByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);
            logger.LogDebug("Cache eviction for organization {OrganizationId}", id);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            await hybridCache.RemoveAsync(CreateKeyByUniqueAlphanumericName(customDomain), cancellationToken);
            logger.LogDebug("Cache eviction for organization custom domain {OrganizationCustomDomain}", customDomain);
        }
    }

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-id:{id}";

    private string CreateKeyByUniqueAlphanumericName(string customDomain) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-customDomain:{customDomain}";
}
