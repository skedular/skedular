using Booking.Shared.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class SpacesQuotaStatusDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    ISpacesBookingQuotaService spacesBookingQuotaService)
    : BatchDataLoader<string, SpacesPublicBookingAvailabilityDetails>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, SpacesPublicBookingAvailabilityDetails>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var decisions = await spacesBookingQuotaService.GetQuotaStatusesAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            organizationId => organizationId,
            organizationId =>
            {
                var decision = decisions[organizationId];
                return new SpacesPublicBookingAvailabilityDetails
                {
                    Available = decision.CanCreate,
                    Message = decision.CanCreate
                        ? "Bookings are available."
                        : "Bookings are currently unavailable for this workspace.",
                };
            });
    }
}
