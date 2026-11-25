using Booking.Api.Services;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingCancellationAvailabilityDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory)
    : ScopedBatchDataLoader<string, MarketplaceCancellationAvailability>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceCancellationAvailability>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await
            ExecuteInScopeAsync<IMarketplaceCancellationAvailabilityService,
                IReadOnlyDictionary<string, MarketplaceCancellationAvailability>>(service => service.GetBookingsAsync(keys, cancellationToken));
}

public sealed class SubscriptionCancellationAvailabilityDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory)
    : ScopedBatchDataLoader<string, MarketplaceSubscriptionCancellationAvailability>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await ExecuteInScopeAsync<IMarketplaceCancellationAvailabilityService,
            IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>>(service =>
            service.GetSubscriptionsAsync(keys, cancellationToken));
}
