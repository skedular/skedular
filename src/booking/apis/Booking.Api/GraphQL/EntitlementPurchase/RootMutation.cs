using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Services.Entitlements;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.EntitlementPurchase;

[MutationType]
public sealed class RootMutation
{
    [UseResolverScope]
    public async Task<EntitlementPurchasePayload> CreateEntitlementAutomaticPaymentRecoveryAsync(
        CreateAutomaticPaymentRecoveryInput input,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Service]
        IMarketplaceAutomaticPaymentStatusService automaticPaymentStatusService,
        [Service]
        IMarketplaceStripeBillingPortalService billingPortalService,
        CancellationToken cancellationToken)
    {
        try
        {
            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            var purchase = await entitlementPurchaseReadService.GetAuthorizedAsync(input.PurchaseId, customerId, cancellationToken);
            if (purchase is null)
            {
                return new EntitlementPurchasePayload
                {
                    ClientMutationId = input.ClientMutationId,
                    Error = "The entitlement purchase could not be found.",
                };
            }

            if (!purchase.AutoRenew || !string.Equals(purchase.PaymentMethod, "CARD", StringComparison.OrdinalIgnoreCase))
            {
                return new EntitlementPurchasePayload
                {
                    ClientMutationId = input.ClientMutationId,
                    Error = "Automatic payment recovery is only available for card-backed auto-renewing purchases.",
                };
            }

            var automaticPaymentStatus = await automaticPaymentStatusService.GetForEntitlementAsync(
                purchase.Id,
                cancellationToken);
            if (automaticPaymentStatus is null || automaticPaymentStatus.Status is not
                    ("past_due" or "action_required" or "finalization_failed" or "incomplete"))
            {
                return new EntitlementPurchasePayload
                {
                    ClientMutationId = input.ClientMutationId,
                    Error = "Automatic payment recovery is not available for this purchase.",
                };
            }

            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                AutomaticPaymentRecoveryUrl = await billingPortalService.CreateForEntitlementAsync(
                    input.PurchaseId,
                    input.ReturnUrl,
                    cancellationToken),
            };
        }
        catch (InvalidOperationException exception)
        {
            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = exception.Message,
            };
        }
    }

    [UseResolverScope]
    public async Task<EntitlementPurchasePayload> CreateEntitlementPurchaseAsync(
        CreateEntitlementPurchaseInput input,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Service]
        IEntitlementPurchaseService entitlementPurchaseService,
        [Service]
        IEntitlementPurchaseCheckoutService entitlementPurchaseCheckoutService,
        [Service]
        IEntitlementPurchaseBankTransferService entitlementPurchaseBankTransferService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        [Service]
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await entitlementPurchaseReadService.CanCreateAsync(input.OrganizationId, customerId, cancellationToken))
        {
            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = "You are not authorized to create this purchase.",
            };
        }

        var product = await entitlementPurchaseService.GetProductAsync(input.ProductVersionId, input.PricingId, cancellationToken);
        if (product is null)
        {
            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = "Pricing was not found.",
            };
        }

        if (product.OrganizationId != input.OrganizationId)
        {
            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = "Pricing is not available for this organization.",
            };
        }

        try
        {
            var purchase = await entitlementPurchaseService.CreatePendingAsync(
                customerId,
                input.OrganizationId,
                input.ProductVersionId,
                product.Pricing,
                product.Currency,
                input.PaymentMethod,
                timeProvider.GetUtcNow().AddMinutes(input.PaymentMethod == PaymentMethod.Card
                    ? product.Pricing.MaxAllowedResourcesLockTimePaidViaCard
                    : product.Pricing.MaxAllowedResourcesLockTimePaidViaBankTransfer),
                input.ServiceStartAt,
                input.CheckoutReturnUrl,
                [.. input.InvoiceEmailList],
                input.AutoRenew,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(input.CheckoutReturnUrl) && input.CheckoutReturnUrl.Contains("__PURCHASE_ID__", StringComparison.Ordinal))
            {
                var checkoutReturnUrl = input.CheckoutReturnUrl.Replace("__PURCHASE_ID__", purchase.Id, StringComparison.Ordinal);
                await entitlementPurchaseService.SetCheckoutReturnUrlAsync(purchase.Id, checkoutReturnUrl, cancellationToken);
                purchase.CheckoutReturnUrl = checkoutReturnUrl;
            }

            if (input.PaymentMethod == PaymentMethod.Card)
            {
                purchase.PaymentAction = (await entitlementPurchaseCheckoutService.CreateCardCheckoutAsync(purchase.Id, cancellationToken))
                    .CheckoutUrl;
            }
            else
            {
                var bankTransfer = await entitlementPurchaseBankTransferService.CreateInvoiceAsync(purchase.Id, cancellationToken);
                purchase.InvoiceNumber = bankTransfer.InvoiceNumber;
                purchase.PaymentInstructions = bankTransfer.PaymentInstructions;
                purchase.PaymentAction = bankTransfer.PaymentInstructions;
            }

            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Purchase = graphQlMapper.Map(purchase),
            };
        }
        catch (InvalidOperationException exception)
        {
            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = exception.Message,
            };
        }
    }

    [UseResolverScope]
    public async Task<EntitlementPurchasePayload> ConfirmEntitlementPurchaseAsync(
        ConfirmEntitlementPurchaseInput input,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchasePaymentService entitlementPurchasePaymentService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        [Service]
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var actorCustomerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        try
        {
            var purchase = await entitlementPurchasePaymentService.ConfirmManualBankTransferAsync(
                input.PurchaseId,
                actorCustomerId,
                timeProvider.GetUtcNow(),
                cancellationToken);
            if (purchase is null)
            {
                return new EntitlementPurchasePayload
                {
                    ClientMutationId = input.ClientMutationId,
                    Error = "Purchase was not found.",
                };
            }

            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Purchase = graphQlMapper.Map(purchase),
            };
        }
        catch (InvalidOperationException exception)
        {
            return new EntitlementPurchasePayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = exception.Message,
            };
        }
    }

    [UseResolverScope]
    public async Task<EntitlementPurchasePayload> RejectEntitlementPurchaseAsync(
        RejectEntitlementPurchaseInput input,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchasePaymentService entitlementPurchasePaymentService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        [Service]
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var actorCustomerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var purchase = await entitlementPurchasePaymentService.RejectManualBankTransferAsync(
            input.PurchaseId,
            actorCustomerId,
            timeProvider.GetUtcNow(),
            cancellationToken);

        return new EntitlementPurchasePayload
        {
            ClientMutationId = input.ClientMutationId,
            Purchase = purchase is null ? null : graphQlMapper.Map(purchase),
        };
    }

    [UseResolverScope]
    public async Task<EntitlementPurchasePayload> MakeEntitlementPurchasePaymentNotRequiredAsync(
        MakeEntitlementPurchasePaymentNotRequiredInput input,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchasePaymentService entitlementPurchasePaymentService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        [Service]
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var actorCustomerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var purchase = await entitlementPurchasePaymentService.MakePaymentNotRequiredAsync(
            input.PurchaseId,
            actorCustomerId,
            timeProvider.GetUtcNow(),
            cancellationToken);

        return new EntitlementPurchasePayload
        {
            ClientMutationId = input.ClientMutationId,
            Purchase = purchase is null ? null : graphQlMapper.Map(purchase),
        };
    }
}
