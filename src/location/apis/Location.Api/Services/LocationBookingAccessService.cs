using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationBookingAccessService
{
    Task<bool> HasCurrentCustomerAccessToLocationAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationBookingAccessService(
    ICachedCustomerService cachedCustomerService,
    ICachedLocationBookingAccessService cachedLocationBookingAccessService)
    : ILocationBookingAccessService
{
    public async Task<bool> HasCurrentCustomerAccessToLocationAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is null)
        {
            return false;
        }

        return await cachedLocationBookingAccessService.HasAccessToLocationAsync(
            customer.Id,
            locationId,
            cancellationToken);
    }
}
