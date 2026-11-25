using Booking.Api.Services;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class BookingCancellationAvailabilityDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation)
    : ScopedBatchDataLoader<string, MarketplaceCancellationAvailability>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceCancellationAvailability>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await
            ExecuteInScopeAsync<IMarketplaceCancellationAvailabilityService,
                IReadOnlyDictionary<string, MarketplaceCancellationAvailability>>(keys, service => service.GetBookingsAsync(keys, cancellationToken));
}

public sealed class SubscriptionCancellationAvailabilityDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation)
    : ScopedBatchDataLoader<string, MarketplaceSubscriptionCancellationAvailability>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken) =>
        await ExecuteInScopeAsync<IMarketplaceCancellationAvailabilityService,
            IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>>(keys, service =>
            service.GetSubscriptionsAsync(keys, cancellationToken));
}
