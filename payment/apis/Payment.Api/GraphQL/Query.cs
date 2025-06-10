using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using HotChocolate;
using HotChocolate.Types;
using Payment.Api.Mappers;
using Payment.Api.Services;
using Payment.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Payment.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper, IVersionService versionService)
{
    [UseResolverScope]
    public Version PaymentVersion()
    {
        var version = versionService.GetVersion();

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> PaymentCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

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
