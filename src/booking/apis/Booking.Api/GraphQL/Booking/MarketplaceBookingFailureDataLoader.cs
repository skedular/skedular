using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class MarketplaceBookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var failures =
            await ExecuteInScopeAsync<IMarketplaceBookingFailureReadService, IReadOnlyDictionary<string, MarketplaceBookingFailureSummary>>(service =>
                service.GetByMarketplaceBookingIdsAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            marketplaceBookingId => marketplaceBookingId,
            marketplaceBookingId => failures.TryGetValue(marketplaceBookingId, out var failure)
                ? graphQlMapper.MapTo(failure)
                : null);
    }
}
