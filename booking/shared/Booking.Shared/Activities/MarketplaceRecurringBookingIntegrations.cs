using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Random;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForMarketplaceRecurringBookingInput(string RecurringBookingId);

public record AdjustRequiredResourcesForMarketplaceRecurringBookingAsyncResponse(bool Deleted, bool Ended);

public record ReleaseMarketplaceRecurringBookingResourcesInput(string RecurringBookingId);

public class MarketplaceRecurringBookingIntegrations(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IMarketplaceBookingService marketplaceBookingService,
    IMapper mapper,
    IRandomHelper randomHelper)
{
    [Activity]
    public async Task<AdjustRequiredResourcesForMarketplaceRecurringBookingAsyncResponse> AdjustRequiredResourcesForMarketplaceRecurringBookingAsync(
        AdjustRequiredResourcesForMarketplaceRecurringBookingInput args) =>
        throw new NotImplementedException();

    [Activity]
    public async Task ReleaseMarketplaceRecurringBookingResourcesAsync(ReleaseMarketplaceRecurringBookingResourcesInput args) =>
        throw new NotImplementedException();
}
