using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.GraphQL;
using Microsoft.Extensions.Logging;
using GraphQlConstants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementPurchasePaymentReconciliationService
{
    Task ConfirmAsync(string purchaseId, DateTimeOffset activatesAt, CancellationToken cancellationToken);
}

public sealed class EntitlementPurchasePaymentReconciliationService(
    IRepositoryFactory repositoryFactory,
    IEntitlementService entitlementService,
    TimeProvider timeProvider,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ILogger<EntitlementPurchasePaymentReconciliationService> logger) : IEntitlementPurchasePaymentReconciliationService
{
    public async Task ConfirmAsync(string purchaseId, DateTimeOffset activatesAt, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Starting entitlement payment reconciliation. PurchaseId={PurchaseId}, ActivatesAt={ActivatesAt}",
            purchaseId, activatesAt);
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
        if (purchase is null)
        {
            logger.LogWarning("Payment reconciliation could not find entitlement purchase {PurchaseId}", purchaseId);
            return;
        }

        logger.LogInformation(
            "Entitlement purchase loaded for payment reconciliation. PurchaseId={PurchaseId}, OrganizationId={OrganizationId}, PaymentStatus={PaymentStatus}, PaymentMethod={PaymentMethod}, EntitlementId={EntitlementId}, PaymentExpiry={PaymentExpiry}, InvoiceNumber={InvoiceNumber}",
            purchase.Id, purchase.OrganizationId, purchase.PaymentStatus, purchase.PaymentMethod, purchase.EntitlementId, purchase.PaymentExpiry,
            purchase.InvoiceNumber);

        if (purchase.EntitlementId is null && purchase.PaymentStatus != PaymentStatusConstants.Pending)
        {
            logger.LogInformation(
                "Ignored entitlement payment reconciliation after terminal payment state. PurchaseId={PurchaseId}, PaymentStatus={PaymentStatus}",
                purchaseId, purchase.PaymentStatus);
            return;
        }

        if (purchase.EntitlementId is null && purchase.PaymentExpiry <= timeProvider.GetUtcNow())
        {
            purchase.PaymentStatus = PaymentStatusConstants.Expired;
            purchase.FailureReason = "The entitlement purchase payment deadline has passed.";
            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogWarning("Rejected late entitlement payment reconciliation. PurchaseId={PurchaseId}", purchaseId);
            return;
        }

        var entitlement = await entitlementService.GrantAsync(
            purchase.Id,
            purchase.CustomerId,
            purchase.OrganizationId,
            purchase.ProductPricing,
            purchase.ServiceStartAt,
            purchase.Currency,
            purchase.AutoRenew,
            cancellationToken);

        logger.LogInformation(
            "Entitlement granted during payment reconciliation. PurchaseId={PurchaseId}, EntitlementId={EntitlementId}",
            purchase.Id, entitlement.Id);

        purchase.EntitlementId = entitlement.Id;
        purchase.PaymentStatus = PaymentStatusConstants.Confirmed;
        purchase.PaymentConfirmedAt ??= activatesAt;
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(GraphQlConstants.EntitlementPurchaseTopicName, purchase.Id, cancellationToken);
        logger.LogInformation(
            "Entitlement payment reconciliation completed and persisted. PurchaseId={PurchaseId}, EntitlementId={EntitlementId}, PaymentStatus={PaymentStatus}, PaymentConfirmedAt={PaymentConfirmedAt}",
            purchase.Id, purchase.EntitlementId, purchase.PaymentStatus, purchase.PaymentConfirmedAt);
    }
}
