using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Location.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Location.Shared.Services.Cache;

public interface ICachedLocationService
{
    ValueTask<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
}

public class CachedLocationService(ApplicationConfiguration applicationConfiguration, IRepositoryFactory repositoryFactory, HybridCache hybridCache)
    : ICachedLocationService
{
    public async ValueTask<Database.Entities.Location?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.LocationRepository.GetByIdAsync(id, ct) ?? throw new TeamNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                cancellationToken: cancellationToken);
        }
        catch (TeamNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.LocationRepository.GetByIdAsync(id, cancellationToken) ?? throw new TeamNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
            cancellationToken: cancellationToken);

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:lcoation-id-{id}";
}
