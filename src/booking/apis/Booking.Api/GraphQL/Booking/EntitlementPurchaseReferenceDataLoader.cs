using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.GraphQL;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class EntitlementPurchaseReferenceDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory) : ScopedBatchDataLoader<string, string?>(batchScheduler, options, serviceScopeFactory)
{
    protected override async Task<IReadOnlyDictionary<string, string?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var purchaseReferences =
            await ExecuteInScopeAsync<IEntitlementService, IReadOnlyDictionary<string, string>>(service =>
                service.GetPurchaseReferencesByIdsAsync(keys, cancellationToken));
        return keys.Distinct().ToDictionary(
            entitlementId => entitlementId,
            entitlementId => purchaseReferences.GetValueOrDefault(entitlementId));
    }
}
