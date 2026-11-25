using Api.Shared.Services.Offering;
using Booking.Shared.Services;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class SpacesQuotaStatusDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory)
    : ScopedBatchDataLoader<string, SpacesPublicBookingAvailabilityDetails>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, SpacesPublicBookingAvailabilityDetails>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var decisions =
            await ExecuteInScopeAsync<ISpacesBookingQuotaService, IReadOnlyDictionary<string, SpacesQuotaDecision>>(service =>
                service.GetQuotaStatusesAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            organizationId => organizationId,
            organizationId =>
            {
                var canCreate = decisions.TryGetValue(organizationId, out var decision) && decision.CanCreate;
                return new SpacesPublicBookingAvailabilityDetails
                {
                    Available = canCreate,
                    Message = canCreate
                        ? "Bookings are available."
                        : "Bookings are currently unavailable for this workspace.",
                };
            });
    }
}
