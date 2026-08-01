using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundAutomationService
{
    Task<MarketplaceRefund> ProcessAfterRequestAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundAutomationService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    IMarketplaceRefundTransitionService transitionService,
    IXeroRefundService xeroRefundService,
    IStripeHostRefundService stripeHostRefundService,
    TimeProvider timeProvider,
    ILogger<MarketplaceRefundAutomationService> logger) : IMarketplaceRefundAutomationService
{
    public async Task<MarketplaceRefund> ProcessAfterRequestAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken)
    {
        if (refund.Status is not (MarketplaceRefundStatusConstants.Requested
            or MarketplaceRefundStatusConstants.Approved
            or MarketplaceRefundStatusConstants.Processing
            or MarketplaceRefundStatusConstants.Failed))
        {
            return refund;
        }

        logger.LogInformation(
            "Refund {RefundId} ({RefundKind}) processing started for {LocalEntityType}/{LocalEntityId} actor={ActorCustomerId}",
            refund.Id, refund.RefundKind, refund.LocalEntityType, refund.LocalEntityId, actorCustomerId);

        if (refund.Status != MarketplaceRefundStatusConstants.Processing)
        {
            refund.RetryCount++;
            refund = await transitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.Processing,
                null,
                actorCustomerId,
                null,
                cancellationToken);
        }

        if (await stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken))
        {
            if (!await stripeHostRefundService.CanProcessAsync(refund, cancellationToken))
            {
                refund = await transitionService.TransitionAsync(
                    refund,
                    MarketplaceRefundStatusConstants.Failed,
                    "The Host card payment could not be correlated to a Stripe Checkout session.",
                    actorCustomerId,
                    null,
                    cancellationToken);
                logger.LogWarning(
                    "Refund {RefundId} failed because its Stripe Checkout session was not found",
                    refund.Id);
                return refund;
            }

            logger.LogInformation("Refund {RefundId} submitting to Stripe provider", refund.Id);
            var beforeStripe = refund.Status;
            refund = await stripeHostRefundService.ProcessAsync(refund, cancellationToken);
            if (refund.Status != beforeStripe)
            {
                refund = await transitionService.TransitionAsync(
                    refund,
                    refund.Status,
                    refund.LastError,
                    actorCustomerId,
                    null,
                    cancellationToken);
            }

            logger.LogInformation(
                "Refund {RefundId} Stripe result status={Status} externalId={ExternalPaymentRefundId}",
                refund.Id, refund.Status, refund.ExternalPaymentRefundId);

            // Stripe is the payment provider for this refund. Do not create a second
            // accounting-side refund in Xero; Xero projection is reserved for refunds
            // whose payment flow is owned by the accounting/bank-transfer path.
            return refund;
        }

        return await ProcessXeroProjectionAsync(refund, actorCustomerId, cancellationToken);
    }

    private async Task<MarketplaceRefund> ProcessXeroProjectionAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken,
        string? correlationId = null)
    {
        // A completed Stripe refund is the financial source of truth. Xero is only an
        // accounting projection at this point, so never demote the customer-facing refund
        // state in order to submit the projection.
        var isCompletedStripeProjection = refund.Status == MarketplaceRefundStatusConstants.Completed;
        var availability = await xeroRefundService.GetProcessingAvailabilityAsync(
            refund,
            cancellationToken);

        if (!availability.CanProcessInXero)
        {
            if (isCompletedStripeProjection)
            {
                refund.LastError = availability.BlockedReason;
                refund.ReconciliationStatus = "AccountingProjectionRequired";
                refund = repositoryFactory.MarketplaceRefundRepository.Update(refund);
                marketplaceRefundEventService.Add(
                    refund,
                    MarketplaceRefundEventTypeConstants.AccountingProjectionRequired,
                    actorCustomerId,
                    refund.LastProcessedAt ?? timeProvider.GetUtcNow(),
                    null,
                    correlationId);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogWarning("Refund {RefundId} completed in Stripe but requires Xero accounting follow-up: {BlockedReason}",
                    refund.Id, availability.BlockedReason);
                return refund;
            }

            refund = await transitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.Failed,
                availability.BlockedReason,
                actorCustomerId,
                correlationId,
                cancellationToken);
            logger.LogWarning(
                "Refund {RefundId} cannot be processed in Xero and failed: {BlockedReason}",
                refund.Id, availability.BlockedReason);
            return refund;
        }

        logger.LogInformation("Refund {RefundId} submitting to Xero", refund.Id);
        marketplaceRefundEventService.Add(
            refund,
            MarketplaceRefundEventTypeConstants.SentToXero,
            actorCustomerId,
            refund.LastProcessedAt,
            null,
            correlationId);
        var beforeXero = refund.Status;
        refund = await xeroRefundService.ProcessAsync(refund, cancellationToken);
        if (isCompletedStripeProjection)
        {
            marketplaceRefundEventService.Add(
                refund,
                refund.ReconciliationStatus == "AccountingProjectionRequired"
                    ? MarketplaceRefundEventTypeConstants.AccountingProjectionRequired
                    : MarketplaceRefundEventTypeConstants.AccountingProjected,
                actorCustomerId,
                refund.LastProcessedAt ?? timeProvider.GetUtcNow(),
                beforeXero,
                correlationId);
        }
        else
        {
            if (refund.Status != beforeXero)
            {
                refund = await transitionService.TransitionAsync(
                    refund,
                    refund.Status,
                    refund.LastError,
                    actorCustomerId,
                    correlationId,
                    cancellationToken);
            }
        }

        if (isCompletedStripeProjection)
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation(
            "Refund {RefundId} Xero result status={Status} externalRefundId={ExternalRefundId}",
            refund.Id, refund.Status, refund.ExternalRefundId);

        return refund;
    }
}
