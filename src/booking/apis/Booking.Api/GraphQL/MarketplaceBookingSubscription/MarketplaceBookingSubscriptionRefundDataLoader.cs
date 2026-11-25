using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

public class MarketplaceBookingSubscriptionRefundDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceRefundDetails?>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceRefundDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var refunds = await ExecuteInScopeAsync<IMarketplaceRefundReadService, IReadOnlyDictionary<string, MarketplaceRefundReadModel>>(keys,
            service =>
                service.GetByMarketplaceBookingSubscriptionIdsAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            subscriptionId => subscriptionId,
            subscriptionId => refunds.TryGetValue(subscriptionId, out var refund)
                ? graphQlMapper.MapTo(refund)
                : null);
    }
}
