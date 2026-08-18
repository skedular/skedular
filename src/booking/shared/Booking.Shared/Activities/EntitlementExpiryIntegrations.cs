using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public sealed class EntitlementExpiryIntegrations(
    IRepositoryFactory repositoryFactory,
    IEntitlementExpiryService entitlementExpiryService,
    IEntitlementRenewalService entitlementRenewalService,
    IEntitlementPurchaseService entitlementPurchaseService,
    TimeProvider timeProvider,
    ILogger<EntitlementExpiryIntegrations> logger)
{
    [Activity]
    public async Task ExpireDueEntitlementsAsync()
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var now = timeProvider.GetUtcNow();
        await entitlementPurchaseService.ExpirePendingAsync(cancellationToken);
        var renewalDueEntitlements = await repositoryFactory.EntitlementRepository.GetRenewalDueActiveAsync(
            now.AddDays(1),
            cancellationToken);

        foreach (var entitlement in renewalDueEntitlements)
        {
            try
            {
                await entitlementRenewalService.CreatePendingRenewalAsync(
                    entitlement.Id,
                    entitlement.ExpiresAt,
                    cancellationToken);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                // A single renewal failure must not prevent the expiry sweep from
                // closing other entitlements in the same activity run.
                // The failed entitlement remains eligible for a later retry.
                logger.LogError(
                    exception,
                    "Entitlement renewal failed during expiry sweep. EntitlementId={EntitlementId}",
                    entitlement.Id);
            }
        }

        var entitlements = await repositoryFactory.EntitlementRepository.GetExpiredActiveAsync(
            now,
            cancellationToken);

        foreach (var entitlement in entitlements)
        {
            await entitlementExpiryService.ExpireAsync(entitlement.Id, cancellationToken);
        }
    }
}
