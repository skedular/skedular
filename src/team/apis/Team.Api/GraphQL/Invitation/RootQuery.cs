using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;
using Team.Shared.Services.Cache;

namespace Team.Api.GraphQL.Invitation;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    public IEnumerable<TeamInvitationStatusDetails> TeamInvitationStatuses() =>
    [
        new() { Type = InvitationStatus.Pending, Name = InvitationStatus.Pending.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Accepted, Name = InvitationStatus.Accepted.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Rejected, Name = InvitationStatus.Rejected.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Cancelled, Name = InvitationStatus.Cancelled.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Expired, Name = InvitationStatus.Expired.ToInvitationStatusName() }
    ];

    [UseResolverScope]
    public async Task<int> PendingTeamInvitationsCountAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? await invitationService.PendingInvitationsCountAsync(cancellationToken)
            : 0;

    [UseResolverScope]
    public async Task<Connection<TeamJoinInvitationEdge>> MyInvitationsToJoinTeamsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MyInvitationsToJoinTeamsWhereInput? where,
        IEnumerable<JoinTeamInvitationOrder>? orderBy,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await invitationService.GetMyPaginatedJoinInvitationsAsync(
            new PaginationInputParam(after, first, before, last),
            new JoinInvitationSearchCriteria(where?.OrganizationCustomDomain, where?.TeamId, where?.Status, null),
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
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount
        };
    }
}
