using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using Organization.Shared.Services.Cache;

namespace Organization.Api.GraphQL.Invitation;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public IEnumerable<OrganizationInvitationStatusDetails> OrganizationInvitationStatuses() =>
    [
        new() { Type = InvitationStatus.Pending, Name = InvitationStatus.Pending.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Accepted, Name = InvitationStatus.Accepted.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Rejected, Name = InvitationStatus.Rejected.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Cancelled, Name = InvitationStatus.Cancelled.ToInvitationStatusName() },
        new() { Type = InvitationStatus.Expired, Name = InvitationStatus.Expired.ToInvitationStatusName() }
    ];

    [UseResolverScope]
    public async Task<int> PendingOrganizationInvitationsCountAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IInvitationService teamInvitationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? await teamInvitationService.PendingInvitationsCountAsync(cancellationToken)
            : 0;

    [UseResolverScope]
    public async Task<Connection<OrganizationJoinInvitationEdge>> MyInvitationsToJoinOrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MyInvitationsToJoinOrganizationsWhereInput where,
        IEnumerable<JoinOrganizationInvitationOrder>? orderBy,
        [Service] IInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamInvitationService.GetMyPaginatedJoinInvitationsAsync(
            new PaginationInputParam(after, first, before, last),
            new JoinInvitationSearchCriteria(where.OrganizationCustomDomain, where.Status, null, null),
            orderBy.ToSafeCollection().Select(item => new JoinOrganizationInvitationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<OrganizationJoinInvitationEdge>
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
