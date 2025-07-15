using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;

namespace Team.Api.GraphQL.Invitation;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<int> PendingTeamInvitationsCountAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? await teamInvitationService.PendingInvitationsCountAsync(cancellationToken)
            : 0;

    [UseResolverScope]
    public async Task<Connection<TeamJoinInvitationEdge>> MyInvitationsToJoinTeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MyInvitationsToJoinTeamsWhereInput where,
        IEnumerable<JoinTeamInvitationOrder>? orderBy,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamInvitationService.GetMyPaginatedJoinInvitationsAsync(
            new PaginationInputParam(after, first, before, last),
            new JoinInvitationSearchCriteria(where.OrganizationId, where.TeamId),
            orderBy.ToSafeCollection().Select(item => new JoinTeamInvitationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<TeamJoinInvitationEdge>
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
