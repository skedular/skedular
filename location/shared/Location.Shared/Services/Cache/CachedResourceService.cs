using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Location.Shared.Services.Cache;

public interface ICachedResourceService
{
    ValueTask<Resource?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
}

public class CachedResourceService(ApplicationConfiguration applicationConfiguration, IRepositoryFactory repositoryFactory, HybridCache hybridCache)
    : ICachedResourceService
{
    public async ValueTask<Resource?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.ResourceRepository.GetByIdAsync(id, ct) ?? throw new ResourceNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromHours(1) },
                cancellationToken: cancellationToken);
        }
        catch (ResourceNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.ResourceRepository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromHours(1) },
            cancellationToken: cancellationToken);

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:resource-id-{id}";
}
