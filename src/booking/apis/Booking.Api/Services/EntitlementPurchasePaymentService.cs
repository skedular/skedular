using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.Services;

public interface IEntitlementPurchasePaymentService
{
    Task<EntitlementPurchase?> ConfirmManualBankTransferAsync(
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset confirmedAt,
        CancellationToken cancellationToken);

    Task<EntitlementPurchase?> RejectManualBankTransferAsync(
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset rejectedAt,
        CancellationToken cancellationToken);

    Task<EntitlementPurchase?> MakePaymentNotRequiredAsync(
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset activatesAt,
        CancellationToken cancellationToken);
}

public sealed class EntitlementPurchasePaymentService(
    IEntitlementPurchaseService entitlementPurchaseService,
    IEntitlementPurchaseReadService entitlementPurchaseReadService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IEntitlementPurchasePaymentService
{
    public async Task<EntitlementPurchase?> ConfirmManualBankTransferAsync(
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset confirmedAt,
        CancellationToken cancellationToken)
    {
        var purchase = await entitlementPurchaseReadService.GetAuthorizedAsync(purchaseId, actorCustomerId, cancellationToken);
        if (purchase is null)
        {
            return null;
        }

        if (purchase.CustomerId == actorCustomerId &&
            !await organizationAuthorizationService.CanModifyPaymentMethodAsync(
                purchase.OrganizationId,
                actorCustomerId,
                cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (purchase.PaymentMethod != PaymentMethod.BankTransfer.ToPaymentMethod())
        {
            throw new InvalidOperationException("Only bank-transfer entitlement purchases can be manually confirmed.");
        }

        return await entitlementPurchaseService.UpdatePaymentStatusAsync(
            purchaseId,
            PaymentStatus.Confirmed,
            confirmedAt,
            cancellationToken);
    }

    public async Task<EntitlementPurchase?> RejectManualBankTransferAsync(
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset rejectedAt,
        CancellationToken cancellationToken)
    {
        var purchase = await GetAuthorizedBankTransferPurchaseAsync(purchaseId, actorCustomerId, cancellationToken);
        return await entitlementPurchaseService.UpdatePaymentStatusAsync(
            purchase.Id,
            PaymentStatus.Rejected,
            rejectedAt,
            cancellationToken);
    }

    public async Task<EntitlementPurchase?> MakePaymentNotRequiredAsync(
        string purchaseId,
        string actorCustomerId,
        DateTimeOffset activatesAt,
        CancellationToken cancellationToken)
    {
        var purchase = await GetAuthorizedBankTransferPurchaseAsync(purchaseId, actorCustomerId, cancellationToken);
        return await entitlementPurchaseService.UpdatePaymentStatusAsync(
            purchase.Id,
            PaymentStatus.NoPaymentRequired,
            activatesAt,
            cancellationToken);
    }

    private async Task<EntitlementPurchase> GetAuthorizedBankTransferPurchaseAsync(
        string purchaseId,
        string actorCustomerId,
        CancellationToken cancellationToken)
    {
        var purchase = await entitlementPurchaseReadService.GetAuthorizedAsync(purchaseId, actorCustomerId, cancellationToken)
                       ?? throw new InvalidOperationException("The entitlement purchase could not be found.");

        if (purchase.PaymentMethod != PaymentMethod.BankTransfer.ToPaymentMethod())
        {
            throw new InvalidOperationException("Only bank-transfer entitlement purchases can use this payment workflow.");
        }

        if (purchase.CustomerId == actorCustomerId &&
            !await organizationAuthorizationService.CanModifyPaymentMethodAsync(
                purchase.OrganizationId,
                actorCustomerId,
                cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return purchase;
    }
}
