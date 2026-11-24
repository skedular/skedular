using Booking.Api.Mappers;
using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public class MarketplaceBookingRefundDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceRefundReadService marketplaceRefundReadService,
    IGraphQlMapper graphQlMapper) : BatchDataLoader<string, MarketplaceRefundDetails?>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceRefundDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var refunds = await marketplaceRefundReadService.GetByMarketplaceBookingIdsAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            bookingId => bookingId,
            bookingId => refunds.TryGetValue(bookingId, out var refund)
                ? graphQlMapper.MapTo(refund)
                : null);
    }
}
