using Api.Shared.Services.Models;
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
        string? organizationCustomDomain,
        MarketplacePurchaseSourceType[]? sourceTypes,
        MarketplacePurchaseLifecycleState[]? lifecycleStates,
        PaymentStatus[]? paymentStatuses,
        string? customerId,
        string? productVersionId,
        DateTimeOffset? activityFrom,
        DateTimeOffset? activityUntil,
        DateTimeOffset? bookingFrom,
        DateTimeOffset? bookingUntil,
        MarketplacePurchaseHistoryOrderInput[]? orderBy,
        [Service]
        IMarketplacePurchaseHistoryService service,
        CancellationToken cancellationToken)
    {
        var definedSources = Enum.GetValues<MarketplacePurchaseSourceType>();
        var unknownSources = sourceTypes?.Where(item => !definedSources.Contains(item)).ToList() ?? [];
        if (unknownSources.Count > 0)
        {
            logger.LogWarning("MarketplacePurchasesAsync received {Count} unrecognised source type value(s): {Values}", unknownSources.Count,
                string.Join(", ", unknownSources));
        }

        var definedLifecycleStates = Enum.GetValues<MarketplacePurchaseLifecycleState>();
        var unknownLifecycleStates = lifecycleStates?.Where(item => !definedLifecycleStates.Contains(item)).ToList() ?? [];
        if (unknownLifecycleStates.Count > 0)
        {
            logger.LogWarning("MarketplacePurchasesAsync received {Count} unrecognised lifecycle state value(s): {Values}",
                unknownLifecycleStates.Count, string.Join(", ", unknownLifecycleStates));
        }

        var (paginatedInfo, entries, totalCount) = await service.GetPaginatedAsync(
            new PaginationInputParam(after, first, before, last),
            organizationCustomDomain,
            new MarketplacePurchaseHistorySearchCriteria(
                organizationCustomDomain,
                customerId,
                productVersionId,
                sourceTypes,
                lifecycleStates,
                paymentStatuses,
                activityFrom,
                activityUntil,
                bookingFrom,
                bookingUntil),
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
