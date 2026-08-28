using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplacePurchaseHistory;

[QueryType]
public sealed class RootQuery(IGraphQlMapper graphQlMapper, ILogger<RootQuery> logger)
{
    [GraphQLName("marketplacePurchases")]
    [UseResolverScope]
    public async Task<Connection<MarketplacePurchaseHistoryEdge>> MarketplacePurchasesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MarketplacePurchaseHistoryWhereInput where,
        MarketplacePurchaseHistoryOrderInput[]? orderBy,
        [Service]
        IMarketplacePurchaseHistoryService service,
        CancellationToken cancellationToken)
    {
        var definedSources = Enum.GetValues<MarketplacePurchaseSourceType>();
        var unknownSources = where.SourceTypes?.Where(item => !definedSources.Contains(item)).ToList() ?? [];
        if (unknownSources.Count > 0)
        {
            logger.LogWarning("MarketplacePurchasesAsync received {Count} unrecognised source type value(s): {Values}", unknownSources.Count,
                string.Join(", ", unknownSources));
        }

        var definedLifecycleStates = Enum.GetValues<MarketplacePurchaseLifecycleState>();
        var unknownLifecycleStates = where.LifecycleStates?.Where(item => !definedLifecycleStates.Contains(item)).ToList() ?? [];
        if (unknownLifecycleStates.Count > 0)
        {
            logger.LogWarning("MarketplacePurchasesAsync received {Count} unrecognised lifecycle state value(s): {Values}",
                unknownLifecycleStates.Count, string.Join(", ", unknownLifecycleStates));
        }

        var (paginatedInfo, entries, totalCount) = await service.GetPaginatedAsync(
            new PaginationInputParam(after, first, before, last),
            where.OrganizationCustomDomain,
            new MarketplacePurchaseHistorySearchCriteria(
                where.OrganizationCustomDomain,
                where.CustomerId,
                where.ProductVersionId,
                where.SourceTypes,
                where.LifecycleStates,
                where.PaymentStatuses,
                where.ActivityFrom,
                where.ActivityUntil,
                where.BookingFrom,
                where.BookingUntil,
                where.IncludeMineOnly == true),
            orderBy?.Select(item => new MarketplacePurchaseHistoryOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        var mapped = entries.Select(item => new MarketplacePurchaseHistoryEdge(graphQlMapper.MapTo(item.Node), item.Cursor)).ToList();
        return new Connection<MarketplacePurchaseHistoryEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = mapped,
            TotalCount = totalCount,
        };
    }
}
