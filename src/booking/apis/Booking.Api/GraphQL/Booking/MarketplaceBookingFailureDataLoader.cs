using Booking.Api.Mappers;
using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class MarketplaceBookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceBookingFailureReadService failureReadService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var failures = await failureReadService.GetByMarketplaceBookingIdsAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            marketplaceBookingId => marketplaceBookingId,
            marketplaceBookingId => failures.TryGetValue(marketplaceBookingId, out var failure)
                ? graphQlMapper.MapTo(failure)
                : null);
    }
}
