using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Resource;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<ResourceConnection> ResourcesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ResourceWhereInput where,
        IEnumerable<ResourceOrderInput>? orderBy,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await resourceService.GetPaginatedResourcesAsync(
            new PaginationInputParam(after, first, before, last),
            new ResourceSearchCriteria(
                where.LocationId,
                where.NameContains,
                where.CustomTagIds.ToSafeCollection().Concat(where.ZoneIds.ToSafeCollection()).Concat(where.ProductTagIds.ToSafeCollection()),
                where.FloorPlanId),
            orderBy.ToSafeCollection().Select(item => new ResourceOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new ResourceConnection
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
    public async Task<ResourceDetails?> ResourceAsync(string id, [Service] IResourceService resourceService, CancellationToken cancellationToken) =>
        mapper.MapTo(await resourceService.GetByIdAsync(id, cancellationToken));
}
