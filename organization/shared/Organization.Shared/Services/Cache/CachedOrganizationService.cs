using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Organization.Shared.Repositories;

namespace Organization.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Database.Entities.Organization?> GetByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);

    ValueTask<IReadOnlyList<Database.Entities.Organization>>
        GetMyOrganizationsByCustomerIdAsync(string customerId, CancellationToken cancellationToken);

    ValueTask UpdateByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    ValueTask RemoveMyOrganizationsByCustomerIdsAsync(IReadOnlyList<string> customerIds, CancellationToken cancellationToken);
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
                    CreateKeyByUniqueCustomDomain(customDomain),
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
                CreateKeyByUniqueCustomDomain(customDomain),
                await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                    null,
                    customDomain,
                    cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
    }

    public async ValueTask<IReadOnlyList<Database.Entities.Organization>> GetMyOrganizationsByCustomerIdAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        return await hybridCache.GetOrCreateAsync(
            CreateKeyByCustomerIdOrganizations(customerId),
            async ct => await repositoryFactory.OrganizationRepository.GetMinimalOrganizationByCustomerIdUntrackedAsync(customerId, ct),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);
    }

    public async ValueTask RemoveMyOrganizationsByCustomerIdsAsync(IReadOnlyList<string> customerIds, CancellationToken cancellationToken)
    {
        foreach (var customerId in customerIds.Where(customerId => !string.IsNullOrWhiteSpace(customerId)).Distinct())
        {
            await hybridCache.RemoveAsync(CreateKeyByCustomerIdOrganizations(customerId), cancellationToken);
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

    private string CreateKeyByCustomerIdOrganizations(string customerId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-customer-id-organizations:{customerId}";
}
