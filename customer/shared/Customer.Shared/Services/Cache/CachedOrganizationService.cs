using Api.Shared.Services;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;

namespace Customer.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken);

    ValueTask UpdateByIdOrUniqueAlphanumericNameAsync(string? id, string? uniqueAlphanumericName, CancellationToken cancellationToken);
    ValueTask RemoveByIdOrUniqueAlphanumericNameAsync(string? id, string? uniqueAlphanumericName, CancellationToken cancellationToken);
}

public class CachedOrganizationService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache)
    : ICachedOrganizationService
{
    public async ValueTask<Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyById(id),
                    async ct => await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(id, null, false, false, ct) ??
                                throw new OrganizationNotFound(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                    cancellationToken: cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
            {
                return await hybridCache.GetOrCreateAsync(
                    CreateKeyByUniqueAlphanumericName(uniqueAlphanumericName),
                    async ct =>
                        await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                            null,
                            uniqueAlphanumericName,
                            false,
                            false,
                            ct) ??
                        throw new OrganizationNotFound(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                    cancellationToken: cancellationToken);
            }

            throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
        }
        catch (OrganizationNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdOrUniqueAlphanumericNameAsync(string? id, string? uniqueAlphanumericName, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.SetAsync(
                CreateKeyById(id),
                await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(id, null, false, false, cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                cancellationToken: cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            await hybridCache.SetAsync(
                CreateKeyByUniqueAlphanumericName(uniqueAlphanumericName),
                await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                    null,
                    uniqueAlphanumericName,
                    false,
                    false,
                    cancellationToken) ??
                throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                cancellationToken: cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async ValueTask RemoveByIdOrUniqueAlphanumericNameAsync(string? id, string? uniqueAlphanumericName, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            await hybridCache.RemoveAsync(CreateKeyByUniqueAlphanumericName(uniqueAlphanumericName), cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-id:{id}";

    private string CreateKeyByUniqueAlphanumericName(string uniqueAlphanumericName) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-uniqueAlphanumericName:{uniqueAlphanumericName}";
}
