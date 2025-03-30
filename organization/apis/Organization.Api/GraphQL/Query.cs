using System.Reflection;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Organization.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version OrganizationVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public int OpeningHoursMinutesStep() => 15;

    [UseResolverScope]
    public IEnumerable<OrganizationTypeDetails> OrganizationTypes() =>
    [
        new() { Type = OrganizationType.Private, Name = OrganizationTypeConstants.Private.ToOrganizationTypeName() },
        new() { Type = OrganizationType.Marketplace, Name = OrganizationTypeConstants.Marketplace.ToOrganizationTypeName() }
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
    public IEnumerable<OrganizationMemberRole> OrganizationMemberRoles() =>
    [
        OrganizationMemberRole.Owner,
        OrganizationMemberRole.Administrator,
        OrganizationMemberRole.Member
    ];

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
        mapper.MapTo(await organizationService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationConnection?> OrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationWhereInput where,
        IEnumerable<OrganizationOrderInput>? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) = await organizationService.GetPaginatedOrganizationsAsync(
            new PaginationInputParam(after, first, before, last),
            new OrganizationSearchCriteria(where.NameContains),
            orderBy.ToSafeCollection().Select(item => new OrganizationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new OrganizationConnection
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

    [UseResolverScope]
    public async Task<OrganizationMemberConnection?> OrganizationMembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput where,
        IEnumerable<OrganizationMemberOrderInput>? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.OrganizationId);

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

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

    [UseResolverScope]
    public async Task<OrganizationAnalytics?> OrganizationAnalyticsAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        [Service] IOrganizationAnalyticsService organizationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var organizationAnalytics = await organizationAnalyticsService.GetAnalyticsAsync(organizationId, from, until, cancellationToken);
        return mapper.MapTo(organizationAnalytics.MemberAttendancePercentage, organizationAnalytics.DailyBookingsTotal);
    }

    [UseResolverScope]
    public async Task<bool> IsAzureTenantInstalledAsync([Service] IAzureTenantService azureTenantService, CancellationToken cancellationToken) =>
        await azureTenantService.DoesTenantExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<string> AzureTenantAdminConsentUrlAsync(
        [Service] IAzureTenantService azureTenantService,
        CancellationToken cancellationToken) =>
        await azureTenantService.GenerateAdminConsentUrlAsync(cancellationToken);

    [UseResolverScope]
    public async Task<string> SsoLoginUrlAsync(
        string organizationId,
        string redirectUrl,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        await organizationSsoService.SsoLoginAsync(organizationId, redirectUrl, cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationDetails?> AzureTenantOrganizationAsync(
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationService.GetByAzureTenantAsync(cancellationToken));

    [UseResolverScope]
    public OrganizationOfferingDetails OrganizationOffering(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var matchedOffering = Offerings.AllOfferings.FirstOrDefault(item => item.ToOfferingCode() == code);
        var offering = matchedOffering.GetOffering();

        return new OrganizationOfferingDetails
        {
            Code = matchedOffering.ToOfferingCode(),
            IsEnterprise = matchedOffering.IsEnterpriseOffering(),
            Name = offering.Name,
            UnitPrice = offering.UnitPrice,
            FeatureSet = mapper.MapTo(offering).ToArray(),
            UnderPriceLines = offering.UnderPriceLines.ToArray(),
            Free = matchedOffering.IsFreeOffering(),
            EarlyBird = matchedOffering.IsEarlyBirdOffering()
        };
    }

    [UseResolverScope]
    public async Task<OrganizationTagConnection?> CustomTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomTagOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Custom, where.NameContains),
            orderBy,
            cachedCustomerService,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> CustomTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection?> ZonesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ZoneOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Zone, where.NameContains),
            orderBy,
            cachedCustomerService,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ZoneAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection?> ProductTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ProductTagOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Product, where.NameContains),
            orderBy,
            cachedCustomerService,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> ProductTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection?> LocationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationTagOrganizationTagWhereInput where,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagTypeConstants.Location, where.NameContains),
            orderBy,
            cachedCustomerService,
            tagService,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> LocationTagAsync(string id, [Service] ITagService tagService, CancellationToken cancellationToken) =>
        mapper.MapTo(await tagService.GetByIdAsync(id, cancellationToken));

    private async Task<OrganizationTagConnection?> OrganizationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TagSearchCriteria tagSearchCriteria,
        IEnumerable<OrganizationTagOrderInput>? orderBy,
        ICachedCustomerService cachedCustomerService,
        ITagService tagService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(after, first, before, last),
            tagSearchCriteria,
            orderBy.ToSafeCollection().Select(item => new TagOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new OrganizationTagConnection
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
