using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Member;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public IEnumerable<OrganizationMemberRole> OrganizationMemberRoles() =>
    [
        OrganizationMemberRole.Owner,
        OrganizationMemberRole.Administrator,
        OrganizationMemberRole.Member
    ];

    [UseResolverScope]
    public async Task<OrganizationMemberConnection> OrganizationMembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput where,
        IEnumerable<OrganizationMemberOrderInput>? orderBy,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.OrganizationId);

        var (paginatedInfo, edges, totalCount) = await organizationMemberService.GetPaginatedOrganizationMembersAsync(
            new PaginationInputParam(after, first, before, last),
            new OrganizationMemberSearchCriteria(where.OrganizationId, where.NameContains, where.CustomerId),
            orderBy.ToSafeCollection().Select(item => new OrganizationMemberOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new OrganizationMemberConnection
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
