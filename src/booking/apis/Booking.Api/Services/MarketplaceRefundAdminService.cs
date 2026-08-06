using System.Data;
using System.Diagnostics;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Api.Services;

public interface IMarketplaceRefundAdminService
{
    Task<MarketplaceRefundReadModel> ApproveAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceRefundReadModel> RejectAsync(string id, string reason, CancellationToken cancellationToken);
    Task<MarketplaceRefundReadModel> RecordBankTransferSentAsync(string id, string reference, CancellationToken cancellationToken);
    Task<MarketplaceRefundReadModel> ConfirmBankTransferReceivedAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceRefundReadModel> RetryAsync(string id, CancellationToken cancellationToken);

    Task<MarketplaceRefundReadModel> ResolveReconciliationAsync(string id, bool completed, string reason, string? providerReference,
        CancellationToken cancellationToken);

    Task<MarketplaceRefundReadModel> CancelAsync(string id, string reason, CancellationToken cancellationToken);

    Task<MarketplaceRefundReadModel> CreatePartialAsync(string allocationId, decimal amount, string reason, string idempotencyKey,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundAdminService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IMarketplaceRefundService marketplaceRefundService,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    IMarketplaceRefundTransitionService refundTransitionService,
    IXeroRefundService xeroRefundService,
    ITemporalOutboxService temporalOutboxService,
    IDbTransactionBuilder transactionBuilder,
    TimeProvider timeProvider,
    ILogger<MarketplaceRefundAdminService> logger) : IMarketplaceRefundAdminService
{
    public async Task<MarketplaceRefundReadModel> ApproveAsync(string id, CancellationToken cancellationToken)
    {
        var refund = await UpdateStatusAsync(id, MarketplaceRefundStatusConstants.Approved, null, cancellationToken);
        if (IsBankTransferRefund(refund) && !await xeroRefundService.HasInvoiceTargetAsync(refund, cancellationToken))
        {
            logger.LogInformation(
                "Approved bank-transfer refund {RefundId} has no Xero invoice target and remains queued for manual settlement",
                refund.Id);
            return ToModel(refund);
        }

        logger.LogInformation(
            "Queueing approved refund {RefundId} for provider processing; bank transfer={IsBankTransferRefund}",
            refund.Id,
            IsBankTransferRefund(refund));
        temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
            new ProcessMarketplaceRefundInput(refund.Id, null),
            repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return ToModel(refund);
    }

    public async Task<MarketplaceRefundReadModel> RejectAsync(string id, string reason, CancellationToken cancellationToken) =>
        ToModel(await UpdateStatusAsync(id, MarketplaceRefundStatusConstants.Rejected, reason, cancellationToken));

    public async Task<MarketplaceRefundReadModel> RecordBankTransferSentAsync(string id, string reference, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        var refund = await GetAuthorizedRefundAsync(id, cancellationToken);
        EnsureBankTransferRefund(refund);
        if (refund.Status is MarketplaceRefundStatusConstants.Processing or MarketplaceRefundStatusConstants.Completed)
        {
            throw new InvalidOperationException("This bank-transfer refund has already been sent or completed.");
        }

        var previousStatus = refund.Status;
        MarketplaceRefundStateMachine.EnsureAllowed(previousStatus, MarketplaceRefundStatusConstants.Processing);
        refund.BankTransferReference = reference;
        refund.BankTransferSentAt = timeProvider.GetUtcNow();
        logger.LogInformation("Bank-transfer refund {RefundId} escalated to manual settlement with reference {BankTransferReference}", refund.Id,
            reference);
        var actor = await cachedCustomerService.GetIdAsync(cancellationToken);
        return ToModel(await refundTransitionService.TransitionAsync(refund, MarketplaceRefundStatusConstants.Processing, null, actor,
            Activity.Current?.Id, cancellationToken));
    }

    public async Task<MarketplaceRefundReadModel> ConfirmBankTransferReceivedAsync(string id, CancellationToken cancellationToken)
    {
        var refund = await GetAuthorizedRefundAsync(id, cancellationToken);
        EnsureBankTransferRefund(refund);
        if (string.IsNullOrWhiteSpace(refund.BankTransferReference))
        {
            throw new InvalidOperationException("A bank transfer reference is required before confirmation.");
        }

        var actor = await cachedCustomerService.GetIdAsync(cancellationToken);
        return ToModel(await refundTransitionService.TransitionAsync(refund, MarketplaceRefundStatusConstants.Completed, null, actor,
            Activity.Current?.Id, cancellationToken));
    }

    public async Task<MarketplaceRefundReadModel> RetryAsync(string id, CancellationToken cancellationToken)
    {
        var refund = await GetAuthorizedRefundAsync(id, cancellationToken);
        if (refund.Status != MarketplaceRefundStatusConstants.Failed)
        {
            throw new InvalidOperationException("Only failed refunds can be retried.");
        }

        refund.RetryCount++;
        logger.LogInformation(
            "Retrying refund {RefundId}; transition to {NextStatus}, retry count {RetryCount}", refund.Id,
            MarketplaceRefundStatusConstants.Processing, refund.RetryCount);
        var actor = await cachedCustomerService.GetIdAsync(cancellationToken);
        await refundTransitionService.TransitionAsync(
            refund,
            MarketplaceRefundStatusConstants.Processing,
            null,
            actor,
            Activity.Current?.Id, cancellationToken);
        temporalOutboxService.StartWorkflowProcessMarketplaceRefund(new ProcessMarketplaceRefundInput(refund.Id, null), repositoryFactory.UnitOfWork);
        return ToModel(refund);
    }

    public async Task<MarketplaceRefundReadModel> ResolveReconciliationAsync(string id, bool completed, string reason, string? providerReference,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        var refund = await GetAuthorizedRefundAsync(id, cancellationToken);
        if (refund.Status != MarketplaceRefundStatusConstants.ReconciliationRequired)
        {
            throw new InvalidOperationException("Only refunds requiring reconciliation can be resolved.");
        }

        var nextStatus = completed ? MarketplaceRefundStatusConstants.Completed : MarketplaceRefundStatusConstants.Failed;
        refund.Reason = reason;
        refund.ExternalRefundId ??= providerReference;
        refund.ReconciledAt = timeProvider.GetUtcNow();
        refund.ReconciliationStatus = completed ? "Resolved" : "Rejected";
        var actor = await cachedCustomerService.GetIdAsync(cancellationToken);
        logger.LogInformation(
            "Refund transition {RefundId}: {PreviousStatus} to {NextStatus}; actor={ActorCustomerId}; entity={EntityType}/{EntityId}; correlationId={CorrelationId}; outcome={Outcome}",
            refund.Id, refund.Status, nextStatus, actor, refund.LocalEntityType, refund.LocalEntityId,
            Activity.Current?.Id, refund.ReconciliationStatus);
        return ToModel(await refundTransitionService.TransitionAsync(refund, nextStatus, reason, actor, Activity.Current?.Id, cancellationToken));
    }

    public async Task<MarketplaceRefundReadModel> CancelAsync(string id, string reason, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        var refund = await GetAuthorizedRefundAsync(id, cancellationToken);
        if (refund.ExternalRefundId is not null || refund.ExternalPaymentRefundId is not null ||
            refund.Status is MarketplaceRefundStatusConstants.ProviderPending or MarketplaceRefundStatusConstants.Processing)
        {
            throw new InvalidOperationException("A refund cannot be cancelled after provider submission.");
        }

        refund.CancellationReason = reason;
        refund.CancelledAt = timeProvider.GetUtcNow();
        logger.LogInformation(
            "Refund transition {RefundId}: {PreviousStatus} to {NextStatus}; actor={ActorCustomerId}; entity={EntityType}/{EntityId}; correlationId={CorrelationId}; outcome={Outcome}",
            refund.Id, refund.Status, MarketplaceRefundStatusConstants.Cancelled, null, refund.LocalEntityType, refund.LocalEntityId,
            Activity.Current?.Id, "cancelled");
        var actor = await cachedCustomerService.GetIdAsync(cancellationToken);
        return ToModel(await refundTransitionService.TransitionAsync(refund, MarketplaceRefundStatusConstants.Cancelled, reason, actor,
            Activity.Current?.Id, cancellationToken));
    }

    public async Task<MarketplaceRefundReadModel> CreatePartialAsync(
        string allocationId,
        decimal amount,
        string reason,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "The partial-refund amount must be more than zero.");
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var allocation = await repositoryFactory.MarketplaceRefundRepository.GetAllocationByIdAsync(allocationId, cancellationToken)
                         ?? throw new InvalidOperationException("The source payment allocation was not found.");
        var parent = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(allocation.MarketplaceRefundId, cancellationToken)
                     ?? throw new InvalidOperationException("The source refund was not found.");
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(parent.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await marketplaceRefundService.HasConfirmedPaymentAsync(parent, cancellationToken))
        {
            throw new InvalidOperationException("This refund can only be created after payment has been confirmed.");
        }

        var scopedIdempotencyKey = $"partial:{parent.OrganizationId}:{idempotencyKey.Trim()}";
        var existing = await repositoryFactory.MarketplaceRefundRepository.GetByIdempotencyKeyAsync(scopedIdempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return ToModel(existing);
        }

        var refund = repositoryFactory.MarketplaceRefundRepository.Add(new MarketplaceRefund
        {
            Id = Guid.NewGuid().ToString("N"),
            OrganizationId = parent.OrganizationId,
            LocalEntityType = parent.LocalEntityType,
            LocalEntityId = parent.LocalEntityId,
            RefundKind = MarketplaceRefundKindConstants.Partial,
            IdempotencyKey = scopedIdempotencyKey,
            Status = MarketplaceRefundStatusConstants.Requested,
            RequestedAt = timeProvider.GetUtcNow(),
            ReferenceTime = parent.ReferenceTime,
            RefundPercentage = 100,
            RefundAmount = amount,
            BaseAmount = amount,
            Currency = allocation.Currency,
            Reason = reason,
        });
        await repositoryFactory.MarketplaceRefundRepository.ReserveAllocationAsync(refund.Id, allocationId, amount, cancellationToken);
        logger.LogInformation(
            "Partial refund {RefundId} requested against allocation {AllocationId}; amount={Amount}; actor={ActorCustomerId}; entity={EntityType}/{EntityId}; correlationId={CorrelationId}; outcome={Outcome}",
            refund.Id, allocationId, amount, customerId, refund.LocalEntityType, refund.LocalEntityId,
            Activity.Current?.Id, "accepted");
        marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.Requested, customerId, refund.RequestedAt);
        temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
            new ProcessMarketplaceRefundInput(refund.Id, customerId),
            repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await RaiseOwnerGraphQlChangeAsync(refund, cancellationToken);
        return ToModel(refund);
    }

    private static MarketplaceRefundReadModel ToModel(MarketplaceRefund src) => new()
    {
        Id = src.Id,
        LocalEntityType = src.LocalEntityType.ToMarketplaceRefundEntityType(),
        LocalEntityId = src.LocalEntityId,
        Status = src.Status.ToMarketplaceRefundStatus(),
        RequestedAt = src.RequestedAt,
        ReferenceTime = src.ReferenceTime,
        RefundPercentage = src.RefundPercentage,
        AppliedRuleMinutesBefore = src.AppliedRuleMinutesBefore,
        BaseAmount = src.BaseAmount,
        RefundAmount = src.RefundAmount,
        Currency = src.Currency.ToNullableCurrency(),
        Reason = src.Reason,
        AccountingProvider = src.AccountingProvider,
        ExternalRefundId = src.ExternalRefundId,
        ExternalRefundNumber = src.ExternalRefundNumber,
        LastProcessedAt = src.LastProcessedAt,
        LastError = src.LastError,
        PaymentProvider = src.PaymentProvider,
        ExternalPaymentRefundId = src.ExternalPaymentRefundId,
        PaymentRefundStatus = src.PaymentRefundStatus,
        PaymentRefundLastProcessedAt = src.PaymentRefundLastProcessedAt,
        PaymentRefundLastError = src.PaymentRefundLastError,
        RequestedByCustomerId = src.RequestedByCustomerId,
        RefundKind = src.RefundKind.ToMarketplaceRefundKind(),
        IdempotencyKey = src.IdempotencyKey,
        PolicySnapshotJson = src.PolicySnapshotJson,
        CalculationResultJson = src.CalculationResultJson,
        TimezoneId = src.TimezoneId,
        RetryCount = src.RetryCount,
        ApprovedAt = src.ApprovedAt,
        ApprovedByCustomerId = src.ApprovedByCustomerId,
        RejectedAt = src.RejectedAt,
        RejectedByCustomerId = src.RejectedByCustomerId,
        RejectionReason = src.RejectionReason,
        CancelledAt = src.CancelledAt,
        CancellationReason = src.CancellationReason,
        BankTransferReference = src.BankTransferReference,
        BankTransferSentAt = src.BankTransferSentAt,
        ReconciledAt = src.ReconciledAt,
        ReconciliationStatus = src.ReconciliationStatus?.ToMarketplaceExternalRefundReconciliationStatus(),
        LastNotificationStatus = src.LastNotificationStatus?.ToMarketplaceRefundStatus(),
        PostPayoutRefund = src.PostPayoutRefund,
        StripeRefundPath = src.StripeRefundPath,
        StripeAccountId = src.StripeAccountId,
        StripeChargeType = src.StripeChargeType,
        StripeTransferId = src.StripeTransferId,
        StripeChargeId = src.StripeChargeId,
        StripePaymentIntentId = src.StripePaymentIntentId,
        StripeRefundPathSelectedAt = src.StripeRefundPathSelectedAt,
        ReconciliationLeaseOwner = src.ReconciliationLeaseOwner,
        ReconciliationLeaseExpiresAt = src.ReconciliationLeaseExpiresAt,
        ReconciliationLeaseRenewedAt = src.ReconciliationLeaseRenewedAt,
        PaymentAllocations = src.PaymentAllocations.Select(item => new MarketplaceRefundPaymentAllocationModel
        {
            SourcePaymentProvider = item.SourcePaymentProvider,
            SourcePaymentReference = item.SourcePaymentReference,
            SourcePaymentAmount = item.SourceCapturedAmount,
            AllocatedRefundAmount = item.AllocatedRefundAmount,
            Currency = item.Currency.ToCurrency(),
        }).ToList(),
    };

    private async Task<MarketplaceRefund> GetAuthorizedRefundAsync(string id, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(id, cancellationToken)
                     ?? throw new InvalidOperationException($"We couldn't find refund {id}.");
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return refund;
    }

    private async Task<MarketplaceRefund> UpdateStatusAsync(
        string id,
        string status,
        string? reason,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(id, cancellationToken)
                     ?? throw new InvalidOperationException($"We couldn't find refund {id}.");

        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var processedAt = timeProvider.GetUtcNow();
        refund.Reason = reason;
        if (status == MarketplaceRefundStatusConstants.Approved)
        {
            refund.ApprovedByCustomerId = customerId;
            refund.ApprovedAt = processedAt;
        }
        else if (status == MarketplaceRefundStatusConstants.Rejected)
        {
            refund.RejectedByCustomerId = customerId;
            refund.RejectedAt = processedAt;
            refund.RejectionReason = reason;
        }

        logger.LogInformation(
            "Refund transition {RefundId}: {PreviousStatus} to {NextStatus}; actor={ActorCustomerId}; entity={EntityType}/{EntityId}; correlationId={CorrelationId}; outcome={Outcome}",
            refund.Id, refund.Status, status, customerId, refund.LocalEntityType, refund.LocalEntityId,
            Activity.Current?.Id, "accepted");
        return await refundTransitionService.TransitionAsync(refund, status, reason, customerId, Activity.Current?.Id, cancellationToken);
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

    private static bool IsBankTransferRefund(MarketplaceRefund refund) =>
        refund.PaymentAllocations.Any(item =>
            item.IsSourcePayment &&
            string.Equals(item.SourcePaymentProvider, "BANK_TRANSFER", StringComparison.OrdinalIgnoreCase));

    private static void EnsureBankTransferRefund(MarketplaceRefund refund)
    {
        if (!IsBankTransferRefund(refund))
        {
            throw new InvalidOperationException("Only a bank-transfer refund can use the manual transfer workflow.");
        }
    }
}
