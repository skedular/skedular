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

namespace Organization.Api.GraphQL.BankAccount;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationBankAccountDetails?> OrganizationBankAccountAsync(
        string id,
        [Service]
        IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await organizationBankAccountService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationBankAccountDetails?> OrganizationBankAccountByIdAsync(
        [ID]
        string id,
        [Service]
        IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken) =>
        await OrganizationBankAccountAsync(id, organizationBankAccountService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<OrganizationBankAccountEdge>> OrganizationBankAccountsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationBankAccountWhereInput where,
        IEnumerable<OrganizationBankAccountOrderInput>? orderBy,
        [Service]
        IOrganizationBankAccountService organizationBankAccountService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await organizationBankAccountService.GetPaginatedAccountsAsync(
            new PaginationInputParam(after, first, before, last),
            new OrganizationBankAccountSearchCriteria(null, where.OrganizationCustomDomain, where.NameContains),
            orderBy.ToSafeCollection().Select(item => new OrganizationBankAccountOrder(item.Direction, item.Field)),
            false,
            cancellationToken);

        return new Connection<OrganizationBankAccountEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount,
        };
    }
}
