using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Marketplace.Shared.Services.Cache;

public interface ICachedOrganizationService
{
    ValueTask<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
}

public class CachedOrganizationService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache)
    : ICachedOrganizationService
{
    public async ValueTask<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(id, null, ct) ??
                            throw new OrganizationNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                cancellationToken: cancellationToken);
        }
        catch (OrganizationNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(id, null, cancellationToken) ??
            throw new OrganizationNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
            cancellationToken: cancellationToken);

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:organization-id-{id}";
}
