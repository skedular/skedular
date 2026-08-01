using Booking.Shared.Models;
using Booking.Shared.Services;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record ResolveExpiredMarketplaceBookingFailureInput(string FailureId);

public class MarketplaceBookingFailureResolutionIntegrations(IMarketplaceBookingFailureService failureService)
{
    [Activity]
    public async Task ResolveExpiredAsync(ResolveExpiredMarketplaceBookingFailureInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        await failureService.ResolvePartialAsync(
            input.FailureId,
            MarketplaceBookingFailureResolutionDecisionConstants.Expired,
            null,
            cancellationToken);
    }
}
