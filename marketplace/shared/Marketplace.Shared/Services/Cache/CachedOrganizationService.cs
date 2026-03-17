using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Marketplace.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken);

    ValueTask UpdateByIdOrUniqueAlphanumericNameAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask RemoveByIdOrUniqueAlphanumericNameAsync(string? id, string? customDomain, CancellationToken cancellationToken);
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
                    async ct => await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(id, null, ct) ??
                                throw new OrganizationNotFound(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(customDomain))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyByUniqueAlphanumericName(customDomain),
                    async ct =>
                        await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(null, customDomain, ct) ??
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

    public async ValueTask UpdateByIdOrUniqueAlphanumericNameAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        await RemoveByIdOrUniqueAlphanumericNameAsync(id, customDomain, cancellationToken);

        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.SetAsync(
                CreateKeyById(id),
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(id, null, cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            await hybridCache.SetAsync(
                CreateKeyByUniqueAlphanumericName(customDomain),
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                    null,
                    customDomain,
                    cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
    }

    public async ValueTask RemoveByIdOrUniqueAlphanumericNameAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            await hybridCache.RemoveAsync(CreateKeyByUniqueAlphanumericName(customDomain), cancellationToken);
        }
    }

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-id:{id}";

    private string CreateKeyByUniqueAlphanumericName(string customDomain) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-customDomain:{customDomain}";
}
