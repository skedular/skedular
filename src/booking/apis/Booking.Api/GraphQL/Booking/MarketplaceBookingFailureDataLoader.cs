using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class MarketplaceBookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var failures =
            await ExecuteInScopeAsync<IMarketplaceBookingFailureReadService, IReadOnlyDictionary<string, MarketplaceBookingFailureSummary>>(keys,
                service =>
                    service.GetByMarketplaceBookingIdsAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            marketplaceBookingId => marketplaceBookingId,
            marketplaceBookingId => failures.TryGetValue(marketplaceBookingId, out var failure)
                ? graphQlMapper.MapTo(failure)
                : null);
    }
}
