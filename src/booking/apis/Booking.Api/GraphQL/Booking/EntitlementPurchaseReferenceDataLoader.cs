using Booking.Shared.Services.Entitlements;
using GreenDonut;

namespace Booking.Api.GraphQL.Booking;

public sealed class EntitlementPurchaseReferenceDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IEntitlementService entitlementService) : BatchDataLoader<string, string?>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, string?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var purchaseReferences = await entitlementService
            .GetPurchaseReferencesByIdsAsync(keys, cancellationToken);
        return keys.Distinct().ToDictionary(
            entitlementId => entitlementId,
            entitlementId => purchaseReferences.GetValueOrDefault(entitlementId));
    }
}
