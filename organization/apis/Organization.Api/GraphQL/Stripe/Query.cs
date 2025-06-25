using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Stripe;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountDetails?> OrganizationStripeConnectAccountAsync(
        string id,
        [Service] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationStripeConnectAccountService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationStripeConnectAccountConnection> OrganizationStripeConnectAccountsAsync(
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
            new OrganizationStripeConnectAccountSearchCriteria(where.OrganizationId, where.NameContains, where.OnboardingCompleted),
            orderBy.ToSafeCollection().Select(item => new OrganizationStripeConnectAccountOrder(item.Direction, item.Field)).ToList(),
            false,
            cancellationToken);

        return new OrganizationStripeConnectAccountConnection
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
