using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Invitation;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<int> PendingOrganizationInvitationsCountAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationInvitationService teamInvitationService,
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
        [Service] IOrganizationInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await teamInvitationService.GetMyPaginatedJoinInvitationsAsync(
            new PaginationInputParam(after, first, before, last),
            new JoinInvitationSearchCriteria(where.OrganizationId),
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
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }
}
