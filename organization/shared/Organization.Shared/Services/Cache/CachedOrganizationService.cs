using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Organization.Shared.Repositories;

namespace Organization.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Database.Entities.Organization?> GetByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask UpdateByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask UpdateAsync(ICollection<Database.Entities.Organization> organizations, CancellationToken cancellationToken);
    ValueTask RemoveByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
}

public class CachedOrganizationService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache)
    : ICachedOrganizationService
{
    public async ValueTask<Database.Entities.Organization?> GetByIdOrCustomDomainAsync(
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

    public async ValueTask UpdateByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        await RemoveByIdOrCustomDomainAsync(id, customDomain, cancellationToken);

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

    public async ValueTask UpdateAsync(ICollection<Database.Entities.Organization> organizations, CancellationToken cancellationToken)
    {
        foreach (var item in organizations)
        {
            await RemoveByIdOrCustomDomainAsync(item.Id, item.CustomDomain, cancellationToken);

            await hybridCache.SetAsync(
                CreateKeyById(item.Id),
                item,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);

            if (!string.IsNullOrWhiteSpace(item.CustomDomain))
            {
                await hybridCache.SetAsync(
                    CreateKeyByUniqueAlphanumericName(item.CustomDomain),
                    item,
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }
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
            await hybridCache.RemoveAsync(CreateKeyByUniqueAlphanumericName(customDomain), cancellationToken);
        }
    }

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-id:{id}";

    private string CreateKeyByUniqueAlphanumericName(string customDomain) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-customDomain:{customDomain}";
}
