using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Stripe;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountDetails?> OrganizationStripeConnectAccountAsync(
        string id,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await organizationStripeConnectAccountService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationStripeConnectAccountDetails?> OrganizationStripeConnectAccountByIdAsync(
        [ID] string id,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken) =>
        await OrganizationStripeConnectAccountAsync(id, organizationStripeConnectAccountService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<OrganizationStripeConnectAccountEdge>> OrganizationStripeConnectAccountsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationStripeConnectAccountWhereInput where,
        IEnumerable<OrganizationStripeConnectAccountOrderInput>? orderBy,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await organizationStripeConnectAccountService.GetPaginatedAccountsAsync(
            new PaginationInputParam(after, first, before, last),
            new OrganizationStripeConnectAccountSearchCriteria(
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.NameContains,
                where.OnboardingCompleted),
            orderBy.ToSafeCollection().Select(item => new OrganizationStripeConnectAccountOrder(item.Direction, item.Field)),
            false,
            cancellationToken);

        return new Connection<OrganizationStripeConnectAccountEdge>
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
