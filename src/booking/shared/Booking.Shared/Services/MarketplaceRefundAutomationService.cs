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
}

public class MarketplaceRefundAutomationService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    IXeroRefundService xeroRefundService,
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
