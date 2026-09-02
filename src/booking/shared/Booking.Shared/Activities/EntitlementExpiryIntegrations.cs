using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public sealed class EntitlementExpiryIntegrations(
    IRepositoryFactory repositoryFactory,
    IEntitlementExpiryService entitlementExpiryService,
    IEntitlementPurchaseService entitlementPurchaseService,
    TimeProvider timeProvider)
{
    [Activity]
    public async Task ExpireDueEntitlementsAsync()
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var now = timeProvider.GetUtcNow();
        await entitlementPurchaseService.ExpirePendingAsync(cancellationToken);
        var entitlements = await repositoryFactory.EntitlementRepository.GetExpiredActiveAsync(
            now,
            cancellationToken);

        foreach (var entitlement in entitlements)
        {
            await entitlementExpiryService.ExpireAsync(entitlement.Id, cancellationToken);
        }
    }
}
