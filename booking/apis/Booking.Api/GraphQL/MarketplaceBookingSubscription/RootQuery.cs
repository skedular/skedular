using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionDetails?> MarketplaceBookingSubscriptionAsync(
        string id,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await marketplaceBookingSubscriptionService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<MarketplaceBookingSubscriptionDetails?> MarketplaceBookingSubscriptionByIdAsync(
        string id,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken) =>
        await MarketplaceBookingSubscriptionAsync(id, marketplaceBookingSubscriptionService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<MarketplaceBookingSubscriptionEdge>> MarketplaceBookingSubscriptionsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MarketplaceBookingSubscriptionWhereInput where,
        IEnumerable<MarketplaceBookingSubscriptionOrderInput>? orderBy,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        where.TeamIds = where.TeamIds.RemoveInvalidIds();
        where.CustomerIds = where.CustomerIds.RemoveInvalidIds();

        var (paginatedInfo, edges, totalCount) = await marketplaceBookingSubscriptionService.GetPaginatedMarketplaceBookingSubscriptionsAsync(
            new PaginationInputParam(after, first, before, last),
            new MarketplaceBookingSubscriptionSearchCriteria(
                where.StartedAtGt,
                where.StartedAtGte,
                where.StartedAtLt,
                where.StartedAtLte,
                where.CancelledAtGt,
                where.CancelledAtGte,
                where.CancelledAtLt,
                where.CancelledAtLte,
                where.NextRenewalAtGt,
                where.NextRenewalAtGte,
                where.NextRenewalAtLt,
                where.NextRenewalAtLte,
                where.NameContains,
                where.Status,
                where.IncludeMineOnly,
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.TeamIds.ToSafeCollection(),
                where.CustomerIds.ToSafeCollection()),
            orderBy.ToSafeCollection().Select(item => new MarketplaceBookingSubscriptionOrder(item.Direction, item.Field)).ToList(),
            false,
            cancellationToken);

        return new Connection<MarketplaceBookingSubscriptionEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<MarketplaceBookingSubscriptionDetails>> AllMarketplaceBookingSubscriptionsAsync(
        MarketplaceBookingSubscriptionWhereInput where,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        var result = await MarketplaceBookingSubscriptionsAsync(
            null,
            null,
            null,
            null,
            where,
            [],
            marketplaceBookingSubscriptionService,
            cancellationToken);
        return result.Edges.Select(item => item.Node);
    }
}
