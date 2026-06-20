using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface ISpacesBookingUsageRolloverService
{
    Task<int> RolloverCurrentPeriodsAsync(CancellationToken cancellationToken);
}

public class SpacesBookingUsageRolloverService(ILogger<SpacesBookingUsageRolloverService> logger) : ISpacesBookingUsageRolloverService
{
    public Task<int> RolloverCurrentPeriodsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{EventId}: Spaces booking usage rollover skipped because current usage is counted from booking rows",
            SpacesPricingLogEvents.OfferingRolloverCompleted);

        return Task.FromResult(0);
    }
}
