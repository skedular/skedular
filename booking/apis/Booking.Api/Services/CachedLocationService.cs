using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Api.Services;

public interface ICachedLocationService
{
    Task<Location> GetByIdAsync(string id, CancellationToken cancellationToken);
    void CleanCache(string id);
}

public class CachedLocationService(IRepositoryFactory repositoryFactory, IMemoryCache memoryCache)
    : ICachedLocationService
{
    public async Task<Location> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var location = await memoryCache.GetOrCreateAsync<Location>($"location-id-{id}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return await repositoryFactory.LocationRepository.GetByIdAsync(id, false, cancellationToken) ?? throw new LocationNotFound();
            });

        if (location is null)
        {
            throw new LocationNotFound();
        }

        return location;
    }

    public void CleanCache(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        memoryCache.Remove($"location-id-{id}");
    }
}
