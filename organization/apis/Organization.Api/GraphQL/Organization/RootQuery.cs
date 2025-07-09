using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Organization;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public IEnumerable<OrganizationTypeDetails> OrganizationTypes() =>
    [
        new() { Type = OrganizationType.Private, Name = OrganizationTypeConstants.Private.ToOrganizationTypeName() },
        new() { Type = OrganizationType.Marketplace, Name = OrganizationTypeConstants.Marketplace.ToOrganizationTypeName() }
    ];

    [UseResolverScope]
    public IEnumerable<OrganizationMemberVisibilityPolicyDetails> OrganizationMemberVisibilityPolicies() =>
    [
        new()
        {
            Type = OrganizationMemberVisibilityPolicy.FullAccess,
            Name = OrganizationMemberVisibilityPolicy.FullAccess.ToOrganizationMemberVisibilityPolicyName()
        },
        new()
        {
            Type = OrganizationMemberVisibilityPolicy.LimitedAccess,
            Name = OrganizationMemberVisibilityPolicy.LimitedAccess.ToOrganizationMemberVisibilityPolicyName()
        }
    ];

    [UseResolverScope]
    public async Task<bool> OrganizationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTermsOfUse> ActiveOrganizationTermsOfUseAsync(
        [Service] IOrganizationTermsOfUseService organizationTermsOfUseService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationTermsOfUseService.GetActiveTermsOfUseAsync(cancellationToken))!;

    [UseResolverScope]
    public async Task<IEnumerable<OrganizationIndustryMainCategoryReferenceDetails>> OrganizationIndustryMainCategoriesReferencesAsync(
        [Service] IIndustryMainCategoryService industryMainCategoryService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await industryMainCategoryService.GetAllAsync(cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationDetails?> OrganizationAsync(
        string id,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    public async Task<Connection<OrganizationEdge>> OrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationWhereInput where,
        IEnumerable<OrganizationOrderInput>? orderBy,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await organizationService.GetPaginatedOrganizationsAsync(
            new PaginationInputParam(after, first, before, last),
            new OrganizationSearchCriteria(where.NameContains),
            orderBy.ToSafeCollection().Select(item => new OrganizationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<OrganizationEdge>
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

    [UseResolverScope]
    public async Task<IEnumerable<OrganizationDetails>> MyOrganizationsAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        !await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? []
            : mapper.MapTo(await organizationService.GetMyOrganizationsAsync(cancellationToken));
}
