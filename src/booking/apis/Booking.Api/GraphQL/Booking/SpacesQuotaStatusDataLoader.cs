using Api.Shared.Services.Offering;
using Booking.Shared.Services;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class SpacesQuotaStatusDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation)
    : ScopedBatchDataLoader<string, SpacesPublicBookingAvailabilityDetails>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, SpacesPublicBookingAvailabilityDetails>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var decisions =
            await ExecuteInScopeAsync<ISpacesBookingQuotaService, IReadOnlyDictionary<string, SpacesQuotaDecision>>(keys, service =>
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
