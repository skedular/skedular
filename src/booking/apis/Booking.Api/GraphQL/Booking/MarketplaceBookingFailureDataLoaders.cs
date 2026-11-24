using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceBookingFailureReadService failureReadService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        MarketplaceBookingFailureDataLoaderHelper.LoadAsync(
            keys,
            failureReadService.GetByBookingIdsAsync,
            graphQlMapper,
            cancellationToken);
}

public sealed class RecurringBookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceBookingFailureReadService failureReadService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        MarketplaceBookingFailureDataLoaderHelper.LoadAsync(
            keys,
            failureReadService.GetByRecurringBookingIdsAsync,
            graphQlMapper,
            cancellationToken);
}

public sealed class SubscriptionFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceBookingFailureReadService failureReadService,
    IGraphQlMapper graphQlMapper)
    : BatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        MarketplaceBookingFailureDataLoaderHelper.LoadAsync(
            keys,
            failureReadService.GetBySubscriptionIdsAsync,
            graphQlMapper,
            cancellationToken);
}

internal static class MarketplaceBookingFailureDataLoaderHelper
{
    public static async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadAsync(
        IReadOnlyList<string> keys,
        Func<IReadOnlyCollection<string>, CancellationToken, Task<IReadOnlyDictionary<string, MarketplaceBookingFailureSummary>>> load,
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var failures = await load(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            id => id,
            id => failures.TryGetValue(id, out var failure)
                ? graphQlMapper.MapTo(failure)
                : null);
    }
}
