using Enterprise.Shared.Configurations;
using Location.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Location.Shared.Services.Cache;

public interface ICachedLocationBookingAccessService
{
    ValueTask<bool> HasAccessToLocationAsync(string customerId, string locationId, CancellationToken cancellationToken);
    ValueTask RemoveByCustomerAndLocationAsync(string customerId, string locationId, CancellationToken cancellationToken);
}

public class CachedLocationBookingAccessService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache)
    : ICachedLocationBookingAccessService
{
    public async ValueTask<bool> HasAccessToLocationAsync(string customerId, string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyByCustomerAndLocation(customerId, locationId),
                async ct => await repositoryFactory.LocationBookingAccessRepository.AnyActiveByCustomerAndLocationAsync(
                    customerId,
                    locationId,
                    ct)
                    ? true
                    : throw new LocationBookingAccessNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (LocationBookingAccessNotFound)
        {
            return false;
        }
    }

    public async ValueTask RemoveByCustomerAndLocationAsync(string customerId, string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        await hybridCache.RemoveAsync(CreateKeyByCustomerAndLocation(customerId, locationId), cancellationToken);
    }

    private string CreateKeyByCustomerAndLocation(string customerId, string locationId) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:location-booking-access-customer:{customerId}:location:{locationId}";

    private class LocationBookingAccessNotFound : Exception;
}
