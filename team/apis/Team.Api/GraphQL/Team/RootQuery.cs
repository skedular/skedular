using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;

namespace Team.Api.GraphQL.Team;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<TeamDetails?> TeamAsync(string id, [Service] ITeamService teamService, CancellationToken cancellationToken) =>
        mapper.MapTo(await teamService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<TeamDetails?> TeamByIdAsync(
        [ID] string id,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await teamService.GetByIdAsync(id, true, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<TeamEdge>> TeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamWhereInput? where,
        IEnumerable<TeamOrderInput>? orderBy,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamService.GetPaginatedTeamsAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamSearchCriteria(
                where?.OrganizationId,
                where?.OrganizationCustomDomain,
                null,
                where?.NameContains,
                where is null ? [] : where.PrimaryLocationIds.ToSafeCollection()),
            orderBy.ToSafeCollection().Select(item => new TeamOrder(item.Direction, item.Field)),
            cancellationToken);

        return new Connection<TeamEdge>
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
    public async Task<Connection<TeamEdge>> CustomerTeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomerTeamWhereInput? where,
        IEnumerable<TeamOrderInput>? orderBy,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamService.GetPaginatedTeamsAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamSearchCriteria(
                where?.OrganizationId,
                where?.OrganizationCustomDomain,
                where?.CustomerId,
                where?.NameContains,
                where is null ? [] : where.PrimaryLocationIds.ToSafeCollection()),
            orderBy.ToSafeCollection().Select(item => new TeamOrder(item.Direction, item.Field)),
            cancellationToken);

        return new Connection<TeamEdge>
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
    public async Task<IEnumerable<TeamDetails>> MyTeamsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await teamService.GetMyTeamsAsync(organizationId, organizationCustomDomain, cancellationToken));
}
