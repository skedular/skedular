using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.GraphQL;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementExpiryService
{
    Task<bool> ExpireAsync(string entitlementId, CancellationToken cancellationToken);
}

public sealed class EntitlementExpiryService(
    IRepositoryFactory repositoryFactory,
    ICreditLedgerService creditLedgerService,
    IMarketplaceRefundService marketplaceRefundService,
    MarketplaceRefundPolicyService marketplaceRefundPolicyService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ILogger<EntitlementExpiryService> logger) : IEntitlementExpiryService
{
    public async Task<bool> ExpireAsync(string entitlementId, CancellationToken cancellationToken)
    {
        var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        if (entitlement is null || entitlement.Status != EntitlementStatus.Active || entitlement.ExpiresAt > timeProvider.GetUtcNow())
        {
            logger.LogDebug(
                "Skipped entitlement expiry because the entitlement is missing, inactive, or not due. EntitlementId={EntitlementId}, Status={Status}, ExpiresAt={ExpiresAt}",
                entitlementId,
                entitlement?.Status,
                entitlement?.ExpiresAt);
            return false;
        }

        var available = creditLedgerService.GetAvailableCredits(entitlement);
        var purchase = available > 0
            ? await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(entitlement.PurchaseReference, cancellationToken)
            : null;
        var refundQuote = purchase?.ProductPricing is not { } pricing
            ? new MarketplaceRefundQuote(false, false, 0, null)
            : marketplaceRefundPolicyService.GetQuote(
                pricing,
                entitlement.ExpiresAt,
                entitlement.ExpiresAt);
        var createRefund = available > 0 &&
                           purchase?.PaymentStatus == PaymentStatusConstants.Confirmed &&
                           refundQuote.CanCancel &&
                           refundQuote.IsRefundable;
        var transitionType = createRefund ? CreditLedgerTransactionType.Expired : CreditLedgerTransactionType.Forfeited;
        var transitionTypeValue = transitionType.ToPersistedValue();
        var referenceKey = $"entitlement:{entitlement.Id}:{transitionTypeValue}";
        if (entitlement.LedgerEntries.All(item => item.ReferenceKey != referenceKey) && available > 0)
        {
            var transitionEntry = repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
            {
                Id = randomHelper.Generate(),
                EntitlementId = entitlement.Id,
                Quantity = available,
                TransactionType = transitionTypeValue,
                ReferenceKey = referenceKey,
                ActorOrSource = "entitlement-expiry",
                Metadata = new CreditLedgerEntryMetadata
                {
                    UnusedCredits = available,
                },
                CreatedAt = timeProvider.GetUtcNow(),
            });

            if (createRefund)
            {
                try
                {
                    var refund = await marketplaceRefundService.CreateEntitlementExpiryRefundAsync(
                        entitlement,
                        purchase!,
                        available,
                        cancellationToken);
                    if (refund is not null)
                    {
                        transitionEntry.Metadata = new CreditLedgerEntryMetadata
                        {
                            UnusedCredits = available,
                            RefundId = refund.Id,
                        };
                        entitlement.RefundLinks.Add(new EntitlementRefundLink
                        {
                            Id = randomHelper.Generate(),
                            EntitlementId = entitlement.Id,
                            MarketplaceRefundId = refund.Id,
                            UnusedCreditQuantity = available,
                            RefundAmount = refund.RefundAmount ?? 0,
                        });
                    }
                }
                catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
                {
                    transitionEntry.Metadata = new CreditLedgerEntryMetadata
                    {
                        UnusedCredits = available,
                        RefundError = exception.Message,
                    };
                    logger.LogError(
                        exception,
                        "Entitlement expiry refund projection failed; local expiry remains authoritative. EntitlementId={EntitlementId}, PurchaseReference={PurchaseReference}",
                        entitlement.Id,
                        entitlement.PurchaseReference);
                }
            }
        }

        entitlement.Status = EntitlementStatus.Expired;
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(
            entitlement.PurchaseReference,
            cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.EntitlementPurchaseTopicName,
            entitlement.PurchaseReference, cancellationToken);

        logger.LogInformation(
            "Expired entitlement cycle. EntitlementId={EntitlementId}, PurchaseReference={PurchaseReference}, TransitionType={TransitionType}, AvailableCredits={AvailableCredits}",
            entitlement.Id,
            entitlement.PurchaseReference,
            transitionTypeValue,
            available);
        return true;
    }
}
