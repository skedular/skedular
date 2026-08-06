using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[QueryType]
public class MarketplaceRefundRootQuery
{
    [UseResolverScope]
    public Task<MarketplaceRefundPreviewDetails> MarketplaceBookingRefundPreviewAsync(
        string bookingId,
        [Service]
        IMarketplaceRefundPreviewService marketplaceRefundPreviewService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        MapPreviewAsync(marketplaceRefundPreviewService.GetByBookingIdAsync(bookingId, cancellationToken), graphQlMapper);

    [UseResolverScope]
    public Task<MarketplaceRefundPreviewDetails> MarketplaceBookingSubscriptionRefundPreviewAsync(
        string subscriptionId,
        [Service]
        IMarketplaceRefundPreviewService marketplaceRefundPreviewService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        MapPreviewAsync(marketplaceRefundPreviewService.GetByMarketplaceBookingSubscriptionIdAsync(subscriptionId, cancellationToken), graphQlMapper);

    [UseResolverScope]
    public Task<MarketplaceRefundDetails?> MarketplaceRefundAsync(
        string id,
        [Service]
        IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        MapRefundAsync(marketplaceRefundReadService.GetByIdAsync(id, cancellationToken), graphQlMapper);

    [UseResolverScope]
    public async Task<IEnumerable<MarketplaceRefundDetails>> MarketplaceRefundsAsync(
        string organizationCustomDomain,
        IEnumerable<string>? statuses,
        [Service]
        IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken) =>
        (await marketplaceRefundReadService.GetByOrganizationCustomDomainAsync(organizationCustomDomain, statuses?.ToList(), cancellationToken))
        .Select(graphQlMapper.MapTo).ToList();

    [UseResolverScope]
    public async Task<Connection<MarketplaceRefundEdge>> MarketplaceRefundQueueAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MarketplaceRefundWhereInput where,
        [Service]
        IMarketplaceRefundReadService marketplaceRefundReadService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await marketplaceRefundReadService
            .GetPaginatedByOrganizationCustomDomainAsync(
                where.OrganizationCustomDomain ?? throw new ArgumentException("Organization custom domain is required.", nameof(where)),
                where.Statuses?.ToList(),
                where.RequestedAtGte,
                where.RequestedAtLte,
                new PaginationInputParam(after, first, before, last),
                cancellationToken);
        return new Connection<MarketplaceRefundEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = edges.Select(edge => new MarketplaceRefundEdge(graphQlMapper.MapTo(edge.Node), edge.Cursor)),
            TotalCount = totalCount,
        };
    }

    private static async Task<MarketplaceRefundPreviewDetails> MapPreviewAsync(Task<MarketplaceRefundPreviewModel> task, IGraphQlMapper mapper) =>
        mapper.MapTo(await task);

    private static async Task<MarketplaceRefundDetails?> MapRefundAsync(Task<MarketplaceRefundReadModel?> task, IGraphQlMapper mapper)
    {
        var model = await task;
        return model is null ? null : mapper.MapTo(model);
    }

    [UseResolverScope]
    public async Task<Connection<MarketplaceExternalRefundReconciliationEdge>> MarketplaceExternalRefundReconciliationsAsync(
        string organizationCustomDomain,
        string? after,
        int? first,
        string? before,
        int? last,
        string? provider,
        string? status,
        [Service]
        IMarketplaceRefundOperationsService marketplaceRefundOperationsService,
        [Service]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Service]
        ICachedOrganizationService cachedOrganizationService,
        [Service]
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await GetAuthorizedExternalRefundsAsync(organizationCustomDomain, marketplaceRefundOperationsService, organizationAuthorizationService,
            cachedOrganizationService,
            cachedCustomerService,
            new PaginationInputParam(after, first, before, last),
            provider,
            status,
            cancellationToken);

    [UseResolverScope]
    public async Task<Connection<MarketplaceExternalRefundReconciliationEdge>> MarketplaceUnassignedExternalRefundReconciliationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        string? provider,
        string? status,
        [Service]
        IMarketplaceRefundOperationsService marketplaceRefundOperationsService,
        [Service]
        IPlatformOperationsAuthorizationService platformOperationsAuthorizationService,
        CancellationToken cancellationToken)
    {
        if (!platformOperationsAuthorizationService.IsAuthorized())
        {
            throw new UnauthorizedAccessException();
        }

        var (paginatedInfo, edges, totalCount) = await marketplaceRefundOperationsService.GetUnassignedExternalRefundsAsync(
            provider, status, new PaginationInputParam(after, first, before, last), cancellationToken);
        return new Connection<MarketplaceExternalRefundReconciliationEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = edges.Select(edge => new MarketplaceExternalRefundReconciliationEdge(
                new MarketplaceExternalRefundReconciliationDetails
                {
                    Id = edge.Node.Id,
                    Provider = edge.Node.Provider.ToMarketplaceExternalRefundReconciliationProviderValue(),
                    ExternalRefundId = edge.Node.ExternalRefundId,
                    Amount = edge.Node.Amount,
                    Currency = edge.Node.Currency?.ToString().ToLowerInvariant(),
                    Status = edge.Node.Status.ToString(),
                    FirstSeenAt = edge.Node.FirstSeenAt,
                    LastSeenAt = edge.Node.LastSeenAt,
                    ResolutionReason = edge.Node.ResolutionReason,
                    ResolutionActorCustomerId = edge.Node.ResolutionActorCustomerId,
                    ResolutionCorrelationId = edge.Node.ResolutionCorrelationId,
                }, edge.Cursor)),
            TotalCount = totalCount,
        };
    }

    private static async Task<Connection<MarketplaceExternalRefundReconciliationEdge>> GetAuthorizedExternalRefundsAsync(
        string organizationCustomDomain,
        IMarketplaceRefundOperationsService marketplaceRefundOperationsService,
        IOrganizationAuthorizationService organizationAuthorizationService,
        ICachedOrganizationService cachedOrganizationService,
        ICachedCustomerService cachedCustomerService,
        PaginationInputParam paginationInputParam,
        string? provider,
        string? status,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(null, organizationCustomDomain, cancellationToken)
                           ?? throw new OrganizationNotFound();
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organization.Id, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var (paginatedInfo, edges, totalCount) = await marketplaceRefundOperationsService.GetExternalRefundsAsync(
            organization.Id, provider, status, paginationInputParam, cancellationToken);

        return new Connection<MarketplaceExternalRefundReconciliationEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = edges.Select(edge => new MarketplaceExternalRefundReconciliationEdge(
                new MarketplaceExternalRefundReconciliationDetails
                {
                    Id = edge.Node.Id,
                    Provider = edge.Node.Provider.ToMarketplaceExternalRefundReconciliationProviderValue(),
                    ExternalRefundId = edge.Node.ExternalRefundId,
                    Amount = edge.Node.Amount,
                    Currency = edge.Node.Currency?.ToString().ToLowerInvariant(),
                    Status = edge.Node.Status.ToString(),
                    FirstSeenAt = edge.Node.FirstSeenAt,
                    LastSeenAt = edge.Node.LastSeenAt,
                    ResolutionReason = edge.Node.ResolutionReason,
                    ResolutionActorCustomerId = edge.Node.ResolutionActorCustomerId,
                    ResolutionCorrelationId = edge.Node.ResolutionCorrelationId,
                },
                edge.Cursor)),
            TotalCount = totalCount,
        };
    }

    public IEnumerable<MarketplaceRefundStatusDetails> MarketplaceRefundStatuses() =>
        Enum.GetValues<MarketplaceRefundStatus>()
            .Select(status => new MarketplaceRefundStatusDetails
            {
                Type = status,
                Name = status.ToMarketplaceRefundStatusName(),
            });

    public IEnumerable<MarketplaceRefundKindDetails> MarketplaceRefundKinds() =>
        Enum.GetValues<MarketplaceRefundKind>()
            .Select(kind => new MarketplaceRefundKindDetails
            {
                Type = kind,
                Name = kind.ToString(),
            });

    public IEnumerable<MarketplaceRefundEventTypeDetails> MarketplaceRefundEventTypes() =>
        Enum.GetValues<MarketplaceRefundEventType>()
            .Select(eventType => new MarketplaceRefundEventTypeDetails
            {
                Type = eventType,
                Name = eventType.ToMarketplaceRefundEventTypeName(),
            });
}
