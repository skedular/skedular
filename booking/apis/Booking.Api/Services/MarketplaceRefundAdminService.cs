using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.GraphQL;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.GraphQL;

namespace Booking.Api.Services;

public interface IMarketplaceRefundAdminService
{
    Task<MarketplaceRefund> MarkPendingAccountingAsync(string id, decimal? refundAmount, string? reason, CancellationToken cancellationToken);
    Task<MarketplaceRefund> MarkManualRequiredAsync(string id, string? reason, CancellationToken cancellationToken);
    Task<MarketplaceRefund> MarkManualCompletedAsync(string id, string? reason, CancellationToken cancellationToken);
    Task<MarketplaceRefund> CompleteAsync(string id, string? reason, CancellationToken cancellationToken);
    Task<MarketplaceRefund> FailAsync(string id, string? reason, CancellationToken cancellationToken);
    Task<MarketplaceRefund> ProcessInXeroAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceRefundAdminService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IMarketplaceRefundService marketplaceRefundService,
    IXeroRefundService xeroRefundService,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    IMarketplaceRefundNotificationService marketplaceRefundNotificationService,
    TimeProvider timeProvider) : IMarketplaceRefundAdminService
{
    public Task<MarketplaceRefund> MarkPendingAccountingAsync(
        string id,
        decimal? refundAmount,
        string? reason,
        CancellationToken cancellationToken) =>
        UpdateStatusAsync(id, MarketplaceRefundStatusConstants.PendingAccounting, refundAmount, reason, cancellationToken);

    public Task<MarketplaceRefund> MarkManualRequiredAsync(string id, string? reason, CancellationToken cancellationToken) =>
        UpdateStatusAsync(id, MarketplaceRefundStatusConstants.ManualRequired, null, reason, cancellationToken);

    public Task<MarketplaceRefund> MarkManualCompletedAsync(string id, string? reason, CancellationToken cancellationToken) =>
        UpdateStatusAsync(id, MarketplaceRefundStatusConstants.ManualCompleted, null, reason, cancellationToken);

    public Task<MarketplaceRefund> CompleteAsync(string id, string? reason, CancellationToken cancellationToken) =>
        UpdateStatusAsync(id, MarketplaceRefundStatusConstants.Completed, null, reason, cancellationToken);

    public Task<MarketplaceRefund> FailAsync(string id, string? reason, CancellationToken cancellationToken) =>
        UpdateStatusAsync(id, MarketplaceRefundStatusConstants.Failed, null, reason, cancellationToken);

    public async Task<MarketplaceRefund> ProcessInXeroAsync(string id, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(id, cancellationToken)
                     ?? throw new InvalidOperationException($"Marketplace refund {id} was not found.");
        var sentToXeroAt = timeProvider.GetUtcNow();

        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await marketplaceRefundService.HasConfirmedPaymentAsync(refund, cancellationToken))
        {
            throw new InvalidOperationException("Refund processing requires a confirmed payment.");
        }

        marketplaceRefundEventService.Add(
            refund,
            MarketplaceRefundEventTypeConstants.SentToXero,
            customerId,
            sentToXeroAt);
        refund = await xeroRefundService.ProcessAsync(refund, cancellationToken);
        marketplaceRefundEventService.Add(refund, MapStatusToEventType(refund.Status), customerId,
            refund.LastProcessedAt ?? timeProvider.GetUtcNow());
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await RaiseOwnerGraphQlChangeAsync(refund, cancellationToken);
        await marketplaceRefundNotificationService.NotifyStatusChangedAsync(refund, cancellationToken);

        return refund;
    }

    private async Task<MarketplaceRefund> UpdateStatusAsync(
        string id,
        string status,
        decimal? refundAmount,
        string? reason,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(id, cancellationToken)
                     ?? throw new InvalidOperationException($"Marketplace refund {id} was not found.");

        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        EnsureStatusTransitionAllowed(refund.Status, status);
        ApplyApprovedRefundAmount(refund, status, refundAmount);

        var processedAt = timeProvider.GetUtcNow();
        refund.Status = status;
        refund.Reason = reason;
        refund.LastProcessedAt = processedAt;
        refund.LastError = status == MarketplaceRefundStatusConstants.PendingAccounting ? null : refund.LastError;
        repositoryFactory.MarketplaceRefundRepository.Update(refund);
        marketplaceRefundEventService.Add(refund, MapStatusToEventType(status), customerId, processedAt);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await RaiseOwnerGraphQlChangeAsync(refund, cancellationToken);
        await marketplaceRefundNotificationService.NotifyStatusChangedAsync(refund, cancellationToken);

        return refund;
    }

    private static void ApplyApprovedRefundAmount(MarketplaceRefund refund, string status, decimal? refundAmount)
    {
        if (status != MarketplaceRefundStatusConstants.PendingAccounting || !refundAmount.HasValue)
        {
            return;
        }

        if (!refund.RefundAmount.HasValue)
        {
            throw new InvalidOperationException("Refund amount cannot be approved because the refund request has no calculated refund amount.");
        }

        if (refundAmount <= 0 || refundAmount > refund.RefundAmount.Value)
        {
            throw new InvalidOperationException(
                "Approved refund amount must be greater than zero and must not exceed the policy-calculated refund amount.");
        }

        refund.RefundAmount = refundAmount.Value;
    }

    private async Task RaiseOwnerGraphQlChangeAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        switch (refund.LocalEntityType)
        {
            case MarketplaceRefundEntityTypeConstants.MarketplaceBooking:
                await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                    Constants.BookingTopicName,
                    refund.LocalEntityId,
                    cancellationToken);
                break;

            case MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription:
                await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                    Constants.MarketplaceBookingSubscriptionTopicName,
                    refund.LocalEntityId,
                    cancellationToken);
                break;
        }
    }

    private static void EnsureStatusTransitionAllowed(string currentStatus, string nextStatus)
    {
        if (currentStatus == MarketplaceRefundStatusConstants.Completed && nextStatus != MarketplaceRefundStatusConstants.Completed)
        {
            throw new InvalidOperationException("Completed refunds cannot transition to another status.");
        }

        if (currentStatus == MarketplaceRefundStatusConstants.ManualCompleted && nextStatus != MarketplaceRefundStatusConstants.ManualCompleted)
        {
            throw new InvalidOperationException("Manually completed refunds cannot transition to another status.");
        }

        if (currentStatus == MarketplaceRefundStatusConstants.Requested)
        {
            if (nextStatus is MarketplaceRefundStatusConstants.PendingAccounting or MarketplaceRefundStatusConstants.ManualRequired)
            {
                return;
            }
        }

        if (currentStatus == MarketplaceRefundStatusConstants.PendingAccounting &&
            nextStatus is MarketplaceRefundStatusConstants.Completed
                or MarketplaceRefundStatusConstants.Failed
                or MarketplaceRefundStatusConstants.ManualRequired)
        {
            return;
        }

        if (currentStatus == MarketplaceRefundStatusConstants.Failed &&
            nextStatus is MarketplaceRefundStatusConstants.PendingAccounting or MarketplaceRefundStatusConstants.ManualRequired)
        {
            return;
        }

        if (currentStatus == MarketplaceRefundStatusConstants.ManualRequired &&
            nextStatus is MarketplaceRefundStatusConstants.ManualCompleted
                or MarketplaceRefundStatusConstants.PendingAccounting
                or MarketplaceRefundStatusConstants.Failed)
        {
            return;
        }

        if (currentStatus != nextStatus)
        {
            throw new InvalidOperationException($"Refund status cannot transition from {currentStatus} to {nextStatus}.");
        }
    }

    private static string MapStatusToEventType(string status) =>
        status switch
        {
            MarketplaceRefundStatusConstants.Requested => MarketplaceRefundEventTypeConstants.Requested,
            MarketplaceRefundStatusConstants.PendingAccounting => MarketplaceRefundEventTypeConstants.PendingAccounting,
            MarketplaceRefundStatusConstants.ManualRequired => MarketplaceRefundEventTypeConstants.ManualRequired,
            MarketplaceRefundStatusConstants.ManualCompleted => MarketplaceRefundEventTypeConstants.ManualCompleted,
            MarketplaceRefundStatusConstants.Completed => MarketplaceRefundEventTypeConstants.Completed,
            MarketplaceRefundStatusConstants.Failed => MarketplaceRefundEventTypeConstants.Failed,
            _ => status
        };
}
