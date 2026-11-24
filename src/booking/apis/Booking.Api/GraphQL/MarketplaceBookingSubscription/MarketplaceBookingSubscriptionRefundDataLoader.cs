using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

public class MarketplaceBookingSubscriptionRefundDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceRefundReadService marketplaceRefundReadService,
    IGraphQlMapper graphQlMapper) : BatchDataLoader<string, MarketplaceRefundDetails?>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceRefundDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var refunds = await marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdsAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            subscriptionId => subscriptionId,
            subscriptionId => refunds.TryGetValue(subscriptionId, out var refund)
                ? graphQlMapper.MapTo(refund)
                : null);
    }
}
