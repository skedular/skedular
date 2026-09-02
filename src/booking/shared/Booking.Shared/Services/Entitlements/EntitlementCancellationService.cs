using System.Data;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.GraphQL;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementCancellationService
{
    Task<CreditLedgerEntryModel?> CancelBookingAsync(string bookingId, bool restoreCredit, string reason, CancellationToken cancellationToken);

    Task<CreditLedgerEntryModel?> CancelBookingAsync(
        string bookingId,
        bool restoreCredit,
        string reason,
        bool useExistingTransaction,
        CancellationToken cancellationToken);

    Task<EntitlementModel?> CancelEntitlementAsync(string entitlementId, string reason, CancellationToken cancellationToken);
}

public sealed class EntitlementCancellationService(
    IEntitlementModelMapper entitlementModelMapper,
    IRepositoryFactory repositoryFactory,
    ICreditLedgerService creditLedgerService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IDbTransactionBuilder transactionBuilder,
    IMarketplaceRefundService marketplaceRefundService,
    MarketplaceRefundPolicyService marketplaceRefundPolicyService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ILogger<EntitlementCancellationService> logger) : IEntitlementCancellationService
{
    public async Task<CreditLedgerEntryModel?> CancelBookingAsync(
        string bookingId,
        bool restoreCredit,
        string reason,
        CancellationToken cancellationToken) =>
        await CancelBookingAsync(bookingId, restoreCredit, reason, false, cancellationToken);

    public async Task<CreditLedgerEntryModel?> CancelBookingAsync(
        string bookingId,
        bool restoreCredit,
        string reason,
        bool useExistingTransaction,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            useExistingTransaction
                ? null
                : await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);
        var entry = await repositoryFactory.EntitlementRepository.GetConsumedByBookingIdAsync(bookingId, cancellationToken);
        if (entry is null)
        {
            logger.LogInformation("No consumed entitlement credit found for booking cancellation. BookingId={BookingId}", bookingId);
            return null;
        }

        var referenceKey = $"booking:{bookingId}:" + (restoreCredit ? "released" : "forfeited");
        var existing = entry.Entitlement.LedgerEntries.SingleOrDefault(item => item.ReferenceKey == referenceKey);
        if (existing is not null)
        {
            logger.LogInformation(
                "Entitlement cancellation transition is idempotent. BookingId={BookingId}, RestoreCredit={RestoreCredit}, Reason={Reason}",
                bookingId,
                restoreCredit,
                reason);
            return entitlementModelMapper.Map(existing);
        }

        var transition = new CreditLedgerEntry
        {
            Id = randomHelper.Generate(),
            EntitlementId = entry.EntitlementId,
            BookingId = bookingId,
            Quantity = entry.Quantity,
            TransactionType = (restoreCredit ? CreditLedgerTransactionType.Released : CreditLedgerTransactionType.Forfeited).ToPersistedValue(),
            ReferenceKey = referenceKey,
            ActorOrSource = "booking-cancellation",
            Metadata = new CreditLedgerEntryMetadata
            {
                Reason = reason,
            },
            CreatedAt = timeProvider.GetUtcNow(),
        };
        repositoryFactory.EntitlementRepository.AddLedgerEntry(transition);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.EntitlementPurchaseTopicName,
            entry.Entitlement.PurchaseReference, cancellationToken);

        logger.LogInformation(
            "Recorded entitlement cancellation transition. BookingId={BookingId}, EntitlementId={EntitlementId}, RestoreCredit={RestoreCredit}, Quantity={Quantity}, Reason={Reason}",
            bookingId, entry.EntitlementId, restoreCredit, transition.Quantity, reason);
        return entitlementModelMapper.Map(transition);
    }

    public async Task<EntitlementModel?> CancelEntitlementAsync(
        string entitlementId,
        string reason,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entitlementId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);
        var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        if (entitlement is null)
        {
            return null;
        }

        if (entitlement.Status != EntitlementStatus.Active)
        {
            return entitlementModelMapper.Map(entitlement);
        }

        if (await repositoryFactory.EntitlementRepository.HasActiveMarketplaceBookingsAsync(
                entitlement.Id,
                timeProvider.GetUtcNow(),
                cancellationToken))
        {
            throw new InvalidOperationException(
                "Cancel the active bookings made with this entitlement before cancelling the entitlement.");
        }

        var available = creditLedgerService.GetAvailableCredits(entitlement);
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(
            entitlement.PurchaseReference,
            cancellationToken);
        var pricing = purchase?.ProductPricing;
        var refundQuote = pricing is null
            ? new MarketplaceRefundQuote(false, false, 0, null)
            : marketplaceRefundPolicyService.GetQuote(pricing, entitlement.ExpiresAt, timeProvider.GetUtcNow());
        var createRefund = refundQuote.CanCancel && refundQuote.IsRefundable &&
                           purchase?.PaymentStatus == PaymentStatusConstants.Confirmed;
        var transitionType = createRefund ? CreditLedgerTransactionType.Expired : CreditLedgerTransactionType.Forfeited;
        var referenceKey = $"entitlement:{entitlement.Id}:cancelled:{transitionType.ToPersistedValue()}";
        if (available > 0 && entitlement.LedgerEntries.All(item => item.ReferenceKey != referenceKey))
        {
            var transition = repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
            {
                Id = randomHelper.Generate(),
                EntitlementId = entitlement.Id,
                Quantity = available,
                TransactionType = transitionType.ToPersistedValue(),
                ReferenceKey = referenceKey,
                ActorOrSource = "entitlement-cancellation",
                Metadata = new CreditLedgerEntryMetadata
                {
                    Reason = reason,
                    UnusedCredits = available,
                },
                CreatedAt = timeProvider.GetUtcNow(),
            });

            if (createRefund)
            {
                var refund = await marketplaceRefundService.CreateEntitlementCancellationRefundAsync(
                    entitlement,
                    purchase!,
                    available,
                    cancellationToken);
                if (refund is not null)
                {
                    transition.Metadata = new CreditLedgerEntryMetadata
                    {
                        Reason = reason,
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
        }

        entitlement.Status = EntitlementStatus.Cancelled;
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(
            entitlement.PurchaseReference,
            cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.EntitlementPurchaseTopicName,
            entitlement.PurchaseReference, cancellationToken);
        logger.LogInformation(
            "Cancelled entitlement. EntitlementId={EntitlementId}, AvailableCredits={AvailableCredits}, RefundRequested={RefundRequested}",
            entitlement.Id,
            available,
            createRefund);
        return entitlementModelMapper.Map(entitlement);
    }
}
