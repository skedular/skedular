using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await ExecuteInScopeAsync<IMarketplaceBookingFailureReadService, IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>>(service =>
            MarketplaceBookingFailureDataLoaderHelper.LoadAsync(
                keys,
                service.GetByBookingIdsAsync,
                graphQlMapper,
                cancellationToken));
}

public sealed class RecurringBookingFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await ExecuteInScopeAsync<IMarketplaceBookingFailureReadService, IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>>(service =>
            MarketplaceBookingFailureDataLoaderHelper.LoadAsync(
                keys,
                service.GetByRecurringBookingIdsAsync,
                graphQlMapper,
                cancellationToken));
}

public sealed class SubscriptionFailureDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IGraphQlMapper graphQlMapper)
    : ScopedBatchDataLoader<string, MarketplaceBookingFailureDetails?>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await ExecuteInScopeAsync<IMarketplaceBookingFailureReadService, IReadOnlyDictionary<string, MarketplaceBookingFailureDetails?>>(service =>
            MarketplaceBookingFailureDataLoaderHelper.LoadAsync(
                keys,
                service.GetBySubscriptionIdsAsync,
                graphQlMapper,
                cancellationToken));
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
