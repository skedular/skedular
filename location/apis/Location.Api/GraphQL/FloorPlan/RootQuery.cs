using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;

namespace Location.Api.GraphQL.FloorPlan;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<FloorPlanDetails?> FloorPlanAsync(
        string id,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await floorPlanService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<FloorPlanDetails?> FloorPlanByIdAsync(
        [ID] string id,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        await FloorPlanAsync(id, floorPlanService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<FloorPlanEdge>> FloorPlansAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        FloorPlanWhereInput where,
        IEnumerable<FloorPlanOrderInput>? orderBy,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await floorPlanService.GetPaginatedFloorPlansAsync(
            new PaginationInputParam(after, first, before, last),
            new FloorPlanSearchCriteria(where.LocationId),
            orderBy.ToSafeCollection().Select(item => new FloorPlanOrder(item.Direction, item.Field)),
            cancellationToken);

        return new Connection<FloorPlanEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount
        };
    }
}
