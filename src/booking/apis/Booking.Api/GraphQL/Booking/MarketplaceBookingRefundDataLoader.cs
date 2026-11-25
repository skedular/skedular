using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public class MarketplaceBookingRefundDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceRefundDetails?>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceRefundDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var refunds = await ExecuteInScopeAsync<IMarketplaceRefundReadService, IReadOnlyDictionary<string, MarketplaceRefundReadModel>>(service =>
            service.GetByMarketplaceBookingIdsAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            bookingId => bookingId,
            bookingId => refunds.TryGetValue(bookingId, out var refund)
                ? graphQlMapper.MapTo(refund)
                : null);
    }
}
