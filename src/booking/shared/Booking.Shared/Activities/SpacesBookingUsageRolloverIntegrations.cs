using Booking.Shared.Services;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public class SpacesBookingUsageRolloverIntegrations(ISpacesBookingUsageRolloverService spacesBookingUsageRolloverService)
{
    [Activity]
    public async Task<int> RolloverCurrentPeriodsAsync() =>
        await spacesBookingUsageRolloverService.RolloverCurrentPeriodsAsync(ActivityExecutionContext.Current.CancellationToken);
}
