using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Random;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundEventService
{
    MarketplaceRefundEvent Add(
        MarketplaceRefund refund,
        string eventType,
        string? actorCustomerId,
        DateTimeOffset? occurredAt = null,
        string? previousStatus = null,
        string? correlationId = null);
}

public class MarketplaceRefundEventService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper) : IMarketplaceRefundEventService
{
    public MarketplaceRefundEvent Add(
        MarketplaceRefund refund,
        string eventType,
        string? actorCustomerId,
        DateTimeOffset? occurredAt = null,
        string? previousStatus = null,
        string? correlationId = null) =>
        repositoryFactory.MarketplaceRefundEventRepository.Add(
            new MarketplaceRefundEvent
            {
                Id = randomHelper.Generate(),
                MarketplaceRefundId = refund.Id,
                EventType = eventType,
                PreviousStatus = previousStatus,
                NewStatus = refund.Status,
                CorrelationId = correlationId,
                OccurredAt = occurredAt ?? refund.LastProcessedAt ?? refund.RequestedAt,
                RefundAmount = refund.RefundAmount,
                Reason = refund.Reason,
                AccountingProvider = refund.AccountingProvider,
                ExternalRefundId = refund.ExternalRefundId,
                ExternalRefundNumber = refund.ExternalRefundNumber,
                LastError = refund.LastError,
                ActorCustomerId = actorCustomerId,
            });
}
