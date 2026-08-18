using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;

namespace Booking.Api.Services;

public interface IMarketplaceRefundReadService
{
    Task<MarketplaceRefundReadModel?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceRefundReadModel?> GetByMarketplaceBookingIdAsync(string marketplaceBookingId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefundReadModel>> GetByOrganizationCustomDomainAsync(
        string organizationCustomDomain,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<(MarketplaceRefundReadModel Node, string Cursor)>, int)> GetPaginatedByOrganizationCustomDomainAsync(
        string organizationCustomDomain,
        IReadOnlyList<string>? statuses,
        DateTimeOffset? requestedAtFrom,
        DateTimeOffset? requestedAtTo,
        PaginationInputParam paginationInputParam,
        CancellationToken cancellationToken);

    Task<MarketplaceRefundReadModel?> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundReadService(
    IRepositoryFactory repositoryFactory,
    IXeroRefundService xeroRefundService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService) : IMarketplaceRefundReadService
{
    public async Task<MarketplaceRefundReadModel?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(id, cancellationToken);
        if (refund is null)
        {
            return null;
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await MapWithAvailabilityAsync(refund, cancellationToken);
    }

    public async Task<MarketplaceRefundReadModel?> GetByMarketplaceBookingIdAsync(
        string marketplaceBookingId,
        CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetLatestByLocalEntityAsync(
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            marketplaceBookingId,
            cancellationToken);

        return refund is null ? null : await MapWithAccessControlAsync(refund, cancellationToken);
    }

    public async Task<MarketplaceRefundReadModel?> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetLatestByLocalEntityAsync(
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            marketplaceBookingSubscriptionId,
            cancellationToken);

        return refund is null ? null : await MapWithAccessControlAsync(refund, cancellationToken);
    }

    public async Task<IReadOnlyList<MarketplaceRefundReadModel>> GetByOrganizationCustomDomainAsync(
        string organizationCustomDomain,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            null,
            organizationCustomDomain,
            false,
            false,
            cancellationToken) ?? throw new OrganizationNotFound();
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organization.Id, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var refunds = await repositoryFactory.MarketplaceRefundRepository.GetByOrganizationIdAsync(organization.Id, statuses, cancellationToken);
        if (refunds.Count == 0)
        {
            return [];
        }

        var refundIds = refunds.Select(item => item.Id).ToList();
        var refundEvents = await repositoryFactory.MarketplaceRefundEventRepository.GetByMarketplaceRefundIdsAsync(refundIds, cancellationToken);
        var refundEventsByRefundId = refundEvents
            .GroupBy(item => item.MarketplaceRefundId)
            .ToDictionary(item => item.Key, item => (IReadOnlyList<MarketplaceRefundEvent>)[.. item]);
        var actorsById = await GetActorsByIdAsync(refunds, refundEvents, cancellationToken);
        var customerNames = await repositoryFactory.MarketplaceRefundRepository.GetCustomerNamesByRefundsAsync(refunds, cancellationToken);
        var availabilities = new Dictionary<string, XeroRefundProcessingAvailability>(refunds.Count);
        foreach (var refund in refunds)
        {
            availabilities[refund.Id] = await xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken);
        }

        var results = new List<MarketplaceRefundReadModel>(refunds.Count);
        foreach (var refund in refunds)
        {
            results.Add(MapToDetails(
                refund,
                refundEventsByRefundId.TryGetValue(refund.Id, out var events) ? events : [],
                actorsById,
                availabilities[refund.Id],
                customerNames.GetValueOrDefault($"{refund.LocalEntityType}:{refund.LocalEntityId}")));
        }

        return results;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<(MarketplaceRefundReadModel Node, string Cursor)>, int)>
        GetPaginatedByOrganizationCustomDomainAsync(
            string organizationCustomDomain,
            IReadOnlyList<string>? statuses,
            DateTimeOffset? requestedAtFrom,
            DateTimeOffset? requestedAtTo,
            PaginationInputParam paginationInputParam,
            CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            null, organizationCustomDomain, false, false, cancellationToken) ?? throw new OrganizationNotFound();
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organization.Id, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var (pageInfo, refundEdges, totalCount) = await repositoryFactory.MarketplaceRefundRepository
            .GetPaginatedByOrganizationIdAsync(organization.Id, statuses, requestedAtFrom, requestedAtTo, paginationInputParam, cancellationToken);
        var refunds = refundEdges.Select(edge => edge.Node).ToList();
        if (refunds.Count == 0)
        {
            return (pageInfo, [], totalCount);
        }

        var refundIds = refunds.Select(item => item.Id).ToList();
        var refundEvents = await repositoryFactory.MarketplaceRefundEventRepository
            .GetByMarketplaceRefundIdsAsync(refundIds, cancellationToken);
        var refundEventsByRefundId = refundEvents
            .GroupBy(item => item.MarketplaceRefundId)
            .ToDictionary(item => item.Key, item => (IReadOnlyList<MarketplaceRefundEvent>)[.. item]);
        var actorsById = await GetActorsByIdAsync(refunds, refundEvents, cancellationToken);
        var customerNames = await repositoryFactory.MarketplaceRefundRepository.GetCustomerNamesByRefundsAsync(refunds, cancellationToken);
        var availabilities = new Dictionary<string, XeroRefundProcessingAvailability>(refunds.Count);
        foreach (var refund in refunds)
        {
            availabilities[refund.Id] = await xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken);
        }

        var results = new List<(MarketplaceRefundReadModel Node, string Cursor)>(refundEdges.Count);
        foreach (var edge in refundEdges)
        {
            results.Add((
                MapToDetails(
                    edge.Node,
                    refundEventsByRefundId.TryGetValue(edge.Node.Id, out var events) ? events : [],
                    actorsById,
                    availabilities[edge.Node.Id],
                    customerNames.GetValueOrDefault($"{edge.Node.LocalEntityType}:{edge.Node.LocalEntityId}")),
                edge.Cursor));
        }

        return (pageInfo, results, totalCount);
    }

    private async Task<MarketplaceRefundReadModel> MapWithAvailabilityAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var refundEvents = await repositoryFactory.MarketplaceRefundEventRepository.GetByMarketplaceRefundIdAsync(refund.Id, cancellationToken);
        var availability = await xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken);
        var actorsById = await GetActorsByIdAsync([refund], refundEvents, cancellationToken);
        var customerNames = await repositoryFactory.MarketplaceRefundRepository.GetCustomerNamesByRefundsAsync([refund], cancellationToken);
        return MapToDetails(refund, refundEvents, actorsById, availability,
            customerNames.GetValueOrDefault($"{refund.LocalEntityType}:{refund.LocalEntityId}"));
    }

    private async Task<MarketplaceRefundReadModel> MapWithAccessControlAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var canManageRefund =
            await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken);
        if (canManageRefund)
        {
            return await MapWithAvailabilityAsync(refund, cancellationToken);
        }

        var result = MapToModel(refund);
        result.Events = [];
        result.RequestedByCustomerName = null;
        result.LastError = null;
        result.CanProcessInXero = false;
        result.XeroProcessingBlockedReason = null;
        return result;
    }

    private MarketplaceRefundReadModel MapToDetails(
        MarketplaceRefund refund,
        IReadOnlyList<MarketplaceRefundEvent> refundEvents,
        IReadOnlyDictionary<string, string> actorsById,
        XeroRefundProcessingAvailability availability,
        string? fallbackCustomerName)
    {
        var result = MapToModel(refund);
        foreach (var refundEvent in refundEvents)
        {
            refundEvent.MarketplaceRefund = refund;
        }

        result.Events =
        [
            .. refundEvents.Select(item =>
            {
                var mappedEvent = MapToModel(item);
                mappedEvent.ActorName = item.ActorCustomerId is not null && actorsById.TryGetValue(item.ActorCustomerId, out var actorName)
                    ? actorName
                    : null;
                return mappedEvent;
            }),
        ];
        result.RequestedByCustomerName = refund.RequestedByCustomerId is not null &&
                                         actorsById.TryGetValue(refund.RequestedByCustomerId, out var requestedByCustomerName)
            ? requestedByCustomerName
            : fallbackCustomerName;
        result.CanProcessInXero = availability.CanProcessInXero;
        result.XeroProcessingBlockedReason = availability.BlockedReason;
        return result;
    }

    private static MarketplaceRefundReadModel MapToModel(MarketplaceRefund src) => new()
    {
        Id = src.Id,
        CreatedAt = src.CreatedAt,
        ModifiedAt = src.ModifiedAt,
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
        PaymentAllocations =
        [
            .. src.PaymentAllocations.Select(item => new MarketplaceRefundPaymentAllocationModel
            {
                SourcePaymentProvider = item.SourcePaymentProvider,
                SourcePaymentReference = item.SourcePaymentReference,
                SourcePaymentAmount = item.SourceCapturedAmount,
                AllocatedRefundAmount = item.AllocatedRefundAmount,
                Currency = item.Currency.ToCurrency(),
            }),
        ],
    };

    private static MarketplaceRefundEventModel MapToModel(MarketplaceRefundEvent src) => new()
    {
        Id = src.Id,
        CreatedAt = src.CreatedAt,
        ModifiedAt = src.ModifiedAt,
        EventType = src.EventType.ToMarketplaceRefundEventType(),
        OccurredAt = src.OccurredAt,
        RefundAmount = src.RefundAmount,
        Currency = src.MarketplaceRefund.Currency.ToNullableCurrency(),
        Reason = src.Reason,
        AccountingProvider = src.AccountingProvider,
        ExternalRefundId = src.ExternalRefundId,
        ExternalRefundNumber = src.ExternalRefundNumber,
        LastError = src.LastError,
        ActorCustomerId = src.ActorCustomerId,
        PreviousStatus = src.PreviousStatus?.ToMarketplaceRefundStatus(),
        NewStatus = src.NewStatus?.ToMarketplaceRefundStatus(),
        CorrelationId = src.CorrelationId,
    };

    private async Task<IReadOnlyDictionary<string, string>> GetActorsByIdAsync(
        IReadOnlyList<MarketplaceRefund> refunds,
        IReadOnlyList<MarketplaceRefundEvent> refundEvents,
        CancellationToken cancellationToken)
    {
        var actorIds = refundEvents
            .Select(item => item.ActorCustomerId)
            .Concat(refunds.Select(item => item.RequestedByCustomerId))
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct()
            .Cast<string>()
            .ToList();

        if (actorIds.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        return (await repositoryFactory.CustomerRepository.GetByIdsAsync(actorIds, true, cancellationToken))
            .ToDictionary(item => item.Id, item => item.ToDisplayableName().Trim());
    }
}
