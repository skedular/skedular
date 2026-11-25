using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingModificationHistoryDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, IReadOnlyList<MarketplaceBookingModificationDetails>>(batchScheduler, options, serviceScopeFactory,
        instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, IReadOnlyList<MarketplaceBookingModificationDetails>>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var historiesByBookingId =
            await ExecuteInScopeAsync<IMarketplaceBookingModificationService,
                IReadOnlyDictionary<string, IReadOnlyList<MarketplaceBookingModificationSummary>>>(keys, service =>
                service.GetHistoriesAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            bookingId => bookingId,
            bookingId => (IReadOnlyList<MarketplaceBookingModificationDetails>)(
                historiesByBookingId.TryGetValue(bookingId, out var histories)
                    ? histories.Select(graphQlMapper.MapTo).ToList()
                    : []));
    }
}
