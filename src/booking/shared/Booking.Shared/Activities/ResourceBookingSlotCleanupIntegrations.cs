using Booking.Shared.Repositories;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public class ResourceBookingSlotCleanupIntegrations(
    IResourceBookingSlotRepository resourceBookingSlotRepository,
    TimeProvider timeProvider)
{
    [Activity]
    public async Task<int> DeleteUnbookedResourceBookingSlotsAsync()
    {
        var cutoff = timeProvider.GetUtcNow().AddMonths(-1);
        return await resourceBookingSlotRepository.DeleteUnbookedBeforeAsync(
            cutoff,
            ActivityExecutionContext.Current.CancellationToken);
    }
}
