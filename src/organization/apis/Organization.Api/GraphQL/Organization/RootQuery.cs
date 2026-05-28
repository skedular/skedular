using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Organization.Api.GraphQL.Xero;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using Organization.Shared.Services.Cache;

namespace Organization.Api.GraphQL.Organization;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public IEnumerable<OrganizationTypeDetails> OrganizationTypes() =>
    [
        new() { Type = OrganizationType.Private, Name = OrganizationTypeConstants.Private.ToOrganizationTypeName() },
        new() { Type = OrganizationType.Marketplace, Name = OrganizationTypeConstants.Marketplace.ToOrganizationTypeName() },
        new() { Type = OrganizationType.Individual, Name = OrganizationTypeConstants.Individual.ToOrganizationTypeName() }
    ];

    [UseResolverScope]
    public IEnumerable<OrganizationBillingCycleDetails> OrganizationBillingCycles() =>
    [
        new() { Type = OrganizationBillingCycle.Weekly, Name = OrganizationBillingCycle.Weekly.ToOrganizationBillingCycleName() },
        new() { Type = OrganizationBillingCycle.Fortnightly, Name = OrganizationBillingCycle.Fortnightly.ToOrganizationBillingCycleName() },
        new() { Type = OrganizationBillingCycle.Monthly, Name = OrganizationBillingCycle.Monthly.ToOrganizationBillingCycleName() }
    ];

    [UseResolverScope]
    public IEnumerable<OrganizationXeroBillingModeDetails> OrganizationXeroBillingModes() =>
    [
        new() { Type = OrganizationXeroBillingMode.Disabled, Name = OrganizationXeroBillingMode.Disabled.ToOrganizationXeroBillingModeName() },
        new() { Type = OrganizationXeroBillingMode.Enabled, Name = OrganizationXeroBillingMode.Enabled.ToOrganizationXeroBillingModeName() },
        new()
        {
            Type = OrganizationXeroBillingMode.RepeatingInvoices,
            Name = OrganizationXeroBillingMode.RepeatingInvoices.ToOrganizationXeroBillingModeName()
        }
    ];

    [UseResolverScope]
    public async Task<OrganizationTermsOfUse> ActiveOrganizationTermsOfUseAsync(
        [Service] IOrganizationTermsOfUseService organizationTermsOfUseService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await organizationTermsOfUseService.GetActiveTermsOfUseAsync(cancellationToken))!;

    [UseResolverScope]
    public async Task<IEnumerable<OrganizationIndustryMainCategoryReferenceDetails>> OrganizationIndustryMainCategoriesReferencesAsync(
        [Service] IIndustryMainCategoryService industryMainCategoryService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await industryMainCategoryService.GetAllAsync(cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationDetails?> OrganizationAsync(
        string? id,
        string? customDomain,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await organizationService.GetByIdOrCustomDomainAsync(id, customDomain, false, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationPublicDetails?> OrganizationPublicAsync(
        string? id,
        string? customDomain,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapToPublic(await organizationService.GetByIdOrCustomDomainPublicAsync(id, customDomain, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<OrganizationDetails?> OrganizationByIdAsync(
        [ID] string id,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await organizationService.GetByIdOrCustomDomainAsync(id, null, true, cancellationToken));

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
            new OrganizationSearchCriteria(where.NameContains, null),
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
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<MyOrganizationDetails>> MyOrganizationsAsync(
        IEnumerable<OrganizationType>? types,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return [];
        }

        var organizations = graphQlMapper.MapTo(await organizationService.GetMyOrganizationsAsync(cancellationToken));
        var typeFilters = types?.ToHashSet();

        return typeFilters is null || typeFilters.Count == 0
            ? organizations
            : organizations.Where(organization => typeFilters.Contains(organization.Type.Type));
    }
}
