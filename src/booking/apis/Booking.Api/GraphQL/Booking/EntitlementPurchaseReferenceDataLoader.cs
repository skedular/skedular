using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Metrics;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class EntitlementPurchaseReferenceDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation)
    : ScopedBatchDataLoader<string, string?>(batchScheduler, options, serviceScopeFactory, instrumentation)
{
    protected override async Task<IReadOnlyDictionary<string, string?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var purchaseReferences =
            await ExecuteInScopeAsync<IEntitlementService, IReadOnlyDictionary<string, string>>(keys, service =>
                service.GetPurchaseReferencesByIdsAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            entitlementId => entitlementId,
            entitlementId => purchaseReferences.GetValueOrDefault(entitlementId));
    }
}
