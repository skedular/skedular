using Api.Shared.Services.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementRenewalService
{
    Task<EntitlementPurchase?> CreatePendingRenewalAsync(
        string entitlementId,
        DateTimeOffset paymentExpiry,
        CancellationToken cancellationToken);
}

public sealed class EntitlementRenewalService(
    IRepositoryFactory repositoryFactory,
    IEntitlementPurchaseService entitlementPurchaseService,
    IEntitlementPurchaseCheckoutService entitlementPurchaseCheckoutService,
    IEntitlementPurchaseBankTransferService entitlementPurchaseBankTransferService,
    IProductVersionHelperService productVersionHelperService,
    TimeProvider timeProvider,
    ILogger<EntitlementRenewalService> logger) : IEntitlementRenewalService
{
    public async Task<EntitlementPurchase?> CreatePendingRenewalAsync(
        string entitlementId,
        DateTimeOffset paymentExpiry,
        CancellationToken cancellationToken)
    {
        var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        if (entitlement is null ||
            entitlement.Status != EntitlementStatus.Active ||
            entitlement.ExpiresAt <= now ||
            !entitlement.AutoRenew ||
            entitlement.CancelAtPeriodEnd)
        {
            logger.LogInformation(
                "Skipped entitlement renewal because the entitlement is missing, inactive, or expired. EntitlementId={EntitlementId}, Status={Status}, ExpiresAt={ExpiresAt}, CurrentTime={CurrentTime}",
                entitlementId,
                entitlement?.Status,
                entitlement?.ExpiresAt,
                now);
            return null;
        }

        var sourcePurchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(
            entitlement.PurchaseReference,
            cancellationToken);
        if (sourcePurchase is null ||
            sourcePurchase.PaymentStatus != PaymentStatusConstants.Confirmed ||
            !sourcePurchase.ProductPricing.SupportsSubscriptionAutoRenewal)
        {
            logger.LogInformation(
                "Skipped entitlement renewal because the source purchase is missing or non-renewing. EntitlementId={EntitlementId}, PurchaseReference={PurchaseReference}",
                entitlementId,
                entitlement.PurchaseReference);
            return null;
        }

        var currentProductVersion = await repositoryFactory.ProductVersionRepository.GetCurrentByProductIdAsync(
            sourcePurchase.ProductVersion.ProductId,
            cancellationToken);
        var currentPricing = currentProductVersion?.PricingOptions is { } pricingOptions
            ? productVersionHelperService.FindMatchingPricing(pricingOptions, sourcePurchase.ProductPricing)
            : null;
        if (currentPricing is not null &&
            (currentPricing.FulfillmentType != ProductPricingFulfillmentType.Entitlement ||
             !currentPricing.SupportsSubscriptionAutoRenewal))
        {
            currentPricing = null;
        }

        if (currentPricing is null)
        {
            entitlement.RenewalFailureReason = "No compatible active entitlement pricing is available for renewal.";
            entitlement.NextRenewalAt = null;
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                "Entitlement renewal failed because no compatible current token pricing exists. EntitlementId={EntitlementId}, PricingId={PricingId}",
                entitlementId,
                entitlement.PricingId);
            return null;
        }

        var renewalReference = $"entitlement-renewal:{entitlement.Id}:{entitlement.ExpiresAt.UtcDateTime.Ticks}";
        var existingRenewal = await repositoryFactory.EntitlementPurchaseRepository.GetByRenewalReferenceAsync(
            renewalReference,
            cancellationToken);
        if (existingRenewal is not null)
        {
            logger.LogInformation(
                "Entitlement renewal is idempotent. EntitlementId={EntitlementId}, RenewalReference={RenewalReference}, PurchaseId={PurchaseId}",
                entitlement.Id,
                renewalReference,
                existingRenewal.Id);

            if (string.IsNullOrWhiteSpace(existingRenewal.StripeCheckoutUrl) &&
                string.IsNullOrWhiteSpace(existingRenewal.PaymentInstructions) &&
                existingRenewal.PaymentStatus == PaymentStatusConstants.Pending)
            {
                if (existingRenewal.PaymentMethod == PaymentMethod.Card.ToPaymentMethod())
                {
                    await entitlementPurchaseCheckoutService.CreateCardCheckoutAsync(
                        existingRenewal.Id,
                        cancellationToken);
                }
                else
                {
                    await entitlementPurchaseBankTransferService.CreateInvoiceAsync(
                        existingRenewal.Id,
                        cancellationToken);
                }
            }

            return await entitlementPurchaseService.GetByIdAsync(existingRenewal.Id, cancellationToken);
        }

        var renewal = await entitlementPurchaseService.CreatePendingAsync(
            sourcePurchase.CustomerId,
            sourcePurchase.OrganizationId,
            currentProductVersion!.Id,
            currentPricing,
            currentProductVersion.Currency ?? sourcePurchase.Currency,
            sourcePurchase.PaymentMethod.ToPaymentMethod(),
            paymentExpiry,
            entitlement.ExpiresAt,
            sourcePurchase.CheckoutReturnUrl,
            [.. sourcePurchase.InvoiceEmailList],
            true,
            cancellationToken);

        var renewalEntity = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(renewal.Id, cancellationToken)
                            ?? throw new InvalidOperationException("The entitlement renewal purchase could not be reloaded.");
        renewalEntity.RenewalOfPurchaseId = sourcePurchase.Id;
        renewalEntity.RenewalReference = renewalReference;
        repositoryFactory.EntitlementPurchaseRepository.Update(renewalEntity);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // A renewal is a standalone purchase, so initiate the same payment action as
        // a first-cycle purchase rather than routing it through booking workflows.
        if (renewalEntity.PaymentMethod == PaymentMethod.Card.ToPaymentMethod())
        {
            var checkout = await entitlementPurchaseCheckoutService.CreateCardCheckoutAsync(renewalEntity.Id, cancellationToken);
            renewal.PaymentAction = checkout.CheckoutUrl;
        }
        else
        {
            var invoice = await entitlementPurchaseBankTransferService.CreateInvoiceAsync(renewalEntity.Id, cancellationToken);
            renewal.InvoiceNumber = invoice.InvoiceNumber;
            renewal.PaymentInstructions = invoice.PaymentInstructions;
            renewal.PaymentAction = invoice.PaymentInstructions;
        }

        logger.LogInformation(
            "Created pending entitlement renewal purchase. EntitlementId={EntitlementId}, PurchaseId={PurchaseId}, PricingId={PricingId}, PaymentExpiry={PaymentExpiry}, CreatedAt={CreatedAt}",
            entitlementId,
            renewal.Id,
            currentPricing.Id,
            paymentExpiry,
            timeProvider.GetUtcNow());
        return renewal;
    }
}
