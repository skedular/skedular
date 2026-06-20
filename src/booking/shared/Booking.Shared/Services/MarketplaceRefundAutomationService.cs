using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundAutomationService
{
    Task<MarketplaceRefund> ProcessAfterRequestAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken);

    Task<MarketplaceRefund> ProjectAccountingAfterStripeAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundAutomationService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    IXeroRefundService xeroRefundService,
    IStripeHostRefundService stripeHostRefundService,
    TimeProvider timeProvider) : IMarketplaceRefundAutomationService
{
    public async Task<MarketplaceRefund> ProcessAfterRequestAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken)
    {
        if (refund.Status != MarketplaceRefundStatusConstants.Requested)
        {
            return refund;
        }

        if (await stripeHostRefundService.IsHostRefundAsync(refund, cancellationToken))
        {
            if (!await stripeHostRefundService.CanProcessAsync(refund, cancellationToken))
            {
                refund.Status = MarketplaceRefundStatusConstants.ManualRequired;
                refund.LastProcessedAt = timeProvider.GetUtcNow();
                refund.LastError = "The Host card payment could not be correlated to a Stripe Checkout session.";
                refund = repositoryFactory.MarketplaceRefundRepository.Update(refund);
                marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.ManualRequired, actorCustomerId,
                    refund.LastProcessedAt);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                return refund;
            }

            refund = repositoryFactory.MarketplaceRefundRepository.Update(ToPendingAccounting(refund));
            marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.PendingAccounting, actorCustomerId, refund.LastProcessedAt);
            refund = await stripeHostRefundService.ProcessAsync(refund, cancellationToken);
            marketplaceRefundEventService.Add(refund, MapStatusToEventType(refund.Status), actorCustomerId, refund.LastProcessedAt);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return refund.Status == MarketplaceRefundStatusConstants.Completed
                ? await ProjectAccountingAfterStripeAsync(refund, actorCustomerId, cancellationToken)
                : refund;
        }

        return await ProcessXeroProjectionAsync(refund, actorCustomerId, cancellationToken);
    }

    public Task<MarketplaceRefund> ProjectAccountingAfterStripeAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken) =>
        ProcessXeroProjectionAsync(refund, actorCustomerId, cancellationToken);

    private async Task<MarketplaceRefund> ProcessXeroProjectionAsync(
        MarketplaceRefund refund,
        string? actorCustomerId,
        CancellationToken cancellationToken)
    {
        var availability = await xeroRefundService.GetProcessingAvailabilityAsync(
            repositoryFactory.MarketplaceRefundRepository.Update(ToPendingAccounting(refund)),
            cancellationToken);

        if (!availability.CanProcessInXero)
        {
            refund.Status = MarketplaceRefundStatusConstants.ManualRequired;
            refund.LastProcessedAt = timeProvider.GetUtcNow();
            refund.LastError = availability.BlockedReason;
            refund = repositoryFactory.MarketplaceRefundRepository.Update(refund);
            marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.ManualRequired, actorCustomerId, refund.LastProcessedAt);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return refund;
        }

        marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.PendingAccounting, actorCustomerId, refund.LastProcessedAt);
        marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.SentToXero, actorCustomerId, refund.LastProcessedAt);
        refund = await xeroRefundService.ProcessAsync(refund, cancellationToken);
        marketplaceRefundEventService.Add(
            refund,
            MapStatusToEventType(refund.Status),
            actorCustomerId,
            refund.LastProcessedAt ?? timeProvider.GetUtcNow());
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return refund;
    }

    private MarketplaceRefund ToPendingAccounting(MarketplaceRefund refund)
    {
        refund.Status = MarketplaceRefundStatusConstants.PendingAccounting;
        refund.LastProcessedAt = timeProvider.GetUtcNow();
        refund.LastError = null;
        return refund;
    }

    private static string MapStatusToEventType(string status) =>
        status switch
        {
            MarketplaceRefundStatusConstants.PendingAccounting => MarketplaceRefundEventTypeConstants.PendingAccounting,
            MarketplaceRefundStatusConstants.ManualRequired => MarketplaceRefundEventTypeConstants.ManualRequired,
            MarketplaceRefundStatusConstants.ManualCompleted => MarketplaceRefundEventTypeConstants.ManualCompleted,
            MarketplaceRefundStatusConstants.Completed => MarketplaceRefundEventTypeConstants.Completed,
            MarketplaceRefundStatusConstants.Failed => MarketplaceRefundEventTypeConstants.Failed,
            _ => status
        };
}
