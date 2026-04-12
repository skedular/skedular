using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Location.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Organization?> GetByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask UpdateByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask RemoveByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
}

public class CachedOrganizationService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache)
    : ICachedOrganizationService
{
    public async ValueTask<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyById(id),
                    async ct =>
                        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(id, null, false, false, ct) ??
                        throw new OrganizationNotFound(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(customDomain))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyByUniqueCustomDomain(customDomain),
                    async ct =>
                        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(
                            null,
                            customDomain,
                            false,
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
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(id, null, false, false,
                    cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            await hybridCache.SetAsync(
                CreateKeyByUniqueCustomDomain(customDomain),
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainUntrackedAsync(
                    null,
                    customDomain,
                    false,
                    false,
                    cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
    }

    public async ValueTask RemoveByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            await hybridCache.RemoveAsync(CreateKeyByUniqueCustomDomain(customDomain), cancellationToken);
        }
    }

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-id:{id}";

    private string CreateKeyByUniqueCustomDomain(string customDomain) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-customDomain:{customDomain}";
}
