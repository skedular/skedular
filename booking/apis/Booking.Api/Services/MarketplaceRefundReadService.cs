using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;

namespace Booking.Api.Services;

public interface IMarketplaceRefundReadService
{
    Task<MarketplaceRefundDetails?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceRefundDetails?> GetByMarketplaceBookingIdAsync(string marketplaceBookingId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceRefundDetails>> GetByOrganizationCustomDomainAsync(
        string organizationCustomDomain,
        IReadOnlyList<string>? statuses,
        CancellationToken cancellationToken);

    Task<MarketplaceRefundDetails?> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundReadService(
    IRepositoryFactory repositoryFactory,
    IGraphQlMapper graphQlMapper,
    IXeroRefundService xeroRefundService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService) : IMarketplaceRefundReadService
{
    public async Task<MarketplaceRefundDetails?> GetByIdAsync(string id, CancellationToken cancellationToken)
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

    public async Task<MarketplaceRefundDetails?> GetByMarketplaceBookingIdAsync(
        string marketplaceBookingId,
        CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByLocalEntityAsync(
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            marketplaceBookingId,
            cancellationToken);

        return refund is null ? null : await MapWithAccessControlAsync(refund, cancellationToken);
    }

    public async Task<MarketplaceRefundDetails?> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByLocalEntityAsync(
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            marketplaceBookingSubscriptionId,
            cancellationToken);

        return refund is null ? null : await MapWithAccessControlAsync(refund, cancellationToken);
    }

    public async Task<IReadOnlyList<MarketplaceRefundDetails>> GetByOrganizationCustomDomainAsync(
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
            .ToDictionary(item => item.Key, item => (IReadOnlyList<MarketplaceRefundEvent>)item.ToList());
        var actorsById = await GetActorsByIdAsync(refunds, refundEvents, cancellationToken);
        var availabilityTasks = refunds.ToDictionary(
            item => item.Id,
            item => xeroRefundService.GetProcessingAvailabilityAsync(item, cancellationToken));
        await Task.WhenAll(availabilityTasks.Values);

        return refunds.Select(refund => MapToDetails(
            refund,
            refundEventsByRefundId.TryGetValue(refund.Id, out var events) ? events : [],
            actorsById,
            availabilityTasks[refund.Id].Result)).ToList();
    }

    private async Task<MarketplaceRefundDetails> MapWithAvailabilityAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var refundEvents = await repositoryFactory.MarketplaceRefundEventRepository.GetByMarketplaceRefundIdAsync(refund.Id, cancellationToken);
        var availability = await xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken);
        var actorsById = await GetActorsByIdAsync([refund], refundEvents, cancellationToken);
        return MapToDetails(refund, refundEvents, actorsById, availability);
    }

    private async Task<MarketplaceRefundDetails> MapWithAccessControlAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var canManageRefund =
            await organizationAuthorizationService.CanModifyPaymentMethodAsync(refund.OrganizationId, customerId, cancellationToken);
        if (canManageRefund)
        {
            return await MapWithAvailabilityAsync(refund, cancellationToken);
        }

        var result = graphQlMapper.MapTo(refund);
        result.Events = [];
        result.RequestedByCustomerName = null;
        result.LastError = null;
        result.CanProcessInXero = false;
        result.XeroProcessingBlockedReason = null;
        return result;
    }

    private MarketplaceRefundDetails MapToDetails(
        MarketplaceRefund refund,
        IReadOnlyList<MarketplaceRefundEvent> refundEvents,
        IReadOnlyDictionary<string, string> actorsById,
        XeroRefundProcessingAvailability availability)
    {
        var result = graphQlMapper.MapTo(refund);
        foreach (var refundEvent in refundEvents)
        {
            refundEvent.MarketplaceRefund = refund;
        }

        result.Events = refundEvents.Select(item =>
        {
            var mappedEvent = graphQlMapper.MapTo(item);
            mappedEvent.ActorName = item.ActorCustomerId is not null && actorsById.TryGetValue(item.ActorCustomerId, out var actorName)
                ? actorName
                : null;
            return mappedEvent;
        });
        result.RequestedByCustomerName = refund.RequestedByCustomerId is not null &&
                                         actorsById.TryGetValue(refund.RequestedByCustomerId, out var requestedByCustomerName)
            ? requestedByCustomerName
            : null;
        result.CanProcessInXero = availability.CanProcessInXero;
        result.XeroProcessingBlockedReason = availability.BlockedReason;
        return result;
    }

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
