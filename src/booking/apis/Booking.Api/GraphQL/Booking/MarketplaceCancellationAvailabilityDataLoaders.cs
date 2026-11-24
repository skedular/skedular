using Booking.Api.Services;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingCancellationAvailabilityDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceCancellationAvailabilityService cancellationAvailabilityService)
    : BatchDataLoader<string, MarketplaceCancellationAvailability>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<string, MarketplaceCancellationAvailability>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        cancellationAvailabilityService.GetBookingsAsync(keys, cancellationToken);
}

public sealed class SubscriptionCancellationAvailabilityDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IMarketplaceCancellationAvailabilityService cancellationAvailabilityService)
    : BatchDataLoader<string, MarketplaceSubscriptionCancellationAvailability>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        cancellationAvailabilityService.GetSubscriptionsAsync(keys, cancellationToken);
}
