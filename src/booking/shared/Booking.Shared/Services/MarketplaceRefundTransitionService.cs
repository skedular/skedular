using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundTransitionService
{
    Task<MarketplaceRefund> TransitionAsync(
        MarketplaceRefund refund,
        string nextStatus,
        string? error,
        string? actorCustomerId,
        string? correlationId,
        CancellationToken cancellationToken);
}

/// <summary>
///     Applies a refund status transition and publishes all associated durable and
///     customer-facing side effects from one canonical path.
/// </summary>
public sealed class MarketplaceRefundTransitionService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundEventService refundEventService,
    IMarketplaceRefundNotificationService notificationService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    TimeProvider timeProvider) : IMarketplaceRefundTransitionService
{
    public async Task<MarketplaceRefund> TransitionAsync(
        MarketplaceRefund refund,
        string nextStatus,
        string? error,
        string? actorCustomerId,
        string? correlationId,
        CancellationToken cancellationToken)
    {
        var previousStatus = refund.Status;
        MarketplaceRefundStateMachine.EnsureAllowed(previousStatus, nextStatus);

        var processedAt = timeProvider.GetUtcNow();
        refund.Status = nextStatus;
        refund.LastProcessedAt = processedAt;
        refund.LastError = error;
        repositoryFactory.MarketplaceRefundRepository.Update(refund);
        refundEventService.Add(
            refund,
            MapStatusToEventType(nextStatus),
            actorCustomerId,
            processedAt,
            previousStatus,
            correlationId);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await notificationService.NotifyStatusChangedAsync(refund, cancellationToken);
        await RaiseOwnerGraphQlChangeAsync(refund, cancellationToken);
        return refund;
    }

    private Task RaiseOwnerGraphQlChangeAsync(MarketplaceRefund refund, CancellationToken cancellationToken) =>
        refund.LocalEntityType switch
        {
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking =>
                graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                    Constants.BookingTopicName, refund.LocalEntityId, cancellationToken),
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription =>
                graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                    Constants.MarketplaceBookingSubscriptionTopicName, refund.LocalEntityId, cancellationToken),
            _ => Task.CompletedTask
        };

    private static string MapStatusToEventType(string status) =>
        status switch
        {
            MarketplaceRefundStatusConstants.ProviderPending => MarketplaceRefundEventTypeConstants.ProviderPending,
            MarketplaceRefundStatusConstants.UnderReview => MarketplaceRefundEventTypeConstants.UnderReview,
            MarketplaceRefundStatusConstants.Approved => MarketplaceRefundEventTypeConstants.Approved,
            MarketplaceRefundStatusConstants.Processing => MarketplaceRefundEventTypeConstants.Processing,
            MarketplaceRefundStatusConstants.Rejected => MarketplaceRefundEventTypeConstants.Rejected,
            MarketplaceRefundStatusConstants.Cancelled => MarketplaceRefundEventTypeConstants.Cancelled,
            MarketplaceRefundStatusConstants.Completed => MarketplaceRefundEventTypeConstants.Completed,
            MarketplaceRefundStatusConstants.Failed => MarketplaceRefundEventTypeConstants.Failed,
            MarketplaceRefundStatusConstants.ReconciliationRequired => MarketplaceRefundEventTypeConstants.ReconciliationRequired,
            MarketplaceRefundStatusConstants.Requested => MarketplaceRefundEventTypeConstants.Requested,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown marketplace refund status.")
        };
}
