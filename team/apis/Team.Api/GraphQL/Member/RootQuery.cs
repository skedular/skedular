using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;

namespace Team.Api.GraphQL.Member;

[QueryType]
public class RootQuery(IMapper mapper, IVersionService versionService)
{
    [UseResolverScope]
    public IEnumerable<TeamMemberRole> TeamMemberRoles() => [TeamMemberRole.Owner, TeamMemberRole.Administrator, TeamMemberRole.Member];

    [UseResolverScope]
    public async Task<TeamMemberConnection> TeamMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TeamMemberWhereInput where,
        IEnumerable<TeamMemberOrderInput>? orderBy,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.TeamId);

        var (paginatedInfo, edges, totalCount) = await teamMemberService.GetPaginatedMembersAsync(
            new PaginationInputParam(after, first, before, last),
            new TeamMemberSearchCriteria(where.TeamId, where.NameContains),
            orderBy.ToSafeCollection().Select(item => new TeamMemberOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new TeamMemberConnection
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
}
