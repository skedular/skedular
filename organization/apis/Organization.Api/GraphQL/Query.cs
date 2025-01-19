using System.Reflection;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
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

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    [UseResolverScope]
    public async Task<bool> OrganizationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationTermsOfUse> ActiveOrganizationTermsOfUseAsync(
        [Service] IOrganizationTermsOfUseService organizationTermsOfUseService,
        CancellationToken cancellationToken)
    {
        var termsOfUse = await organizationTermsOfUseService.GetActiveTermsOfUseAsync(cancellationToken);
        return mapper.MapTo(termsOfUse)!;
    }

    [UseResolverScope]
    public OrganizationMemberRole[] OrganizationMemberRoles() =>
    [
        OrganizationMemberRole.Owner,
        OrganizationMemberRole.Administrator,
        OrganizationMemberRole.Member
    ];

    [UseResolverScope]
    public async Task<OrganizationIndustryMainCategoryReferenceDetails[]>
        OrganizationIndustryMainCategoriesReferencesAsync(
            [Service] IIndustryMainCategoryService industryMainCategoryService,
            CancellationToken cancellationToken)
    {
        var industryMainCategories = await industryMainCategoryService.GetAllAsync(cancellationToken);
        return mapper.MapTo(industryMainCategories).ToArray();
    }

    [UseResolverScope]
    public async Task<OrganizationDetails?> OrganizationAsync(
        string id,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(organization);
    }

    [UseResolverScope]
    public async Task<OrganizationConnection?> OrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationWhereInput where,
        OrganizationOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await organizationService.GetPaginatedOrganizationsAsync(
                new PaginationInputParam(after, first, before, last),
                new OrganizationSearchCriteria(where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        return new OrganizationOrder(direction, item.Field);
                    }).ToList(),
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
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<OrganizationDetails[]?> MyOrganizationsAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var organizations = await organizationService.GetMyOrganizationsAsync(cancellationToken);
        return mapper.MapTo(organizations).ToArray();
    }

    [UseResolverScope]
    public async Task<OrganizationMemberConnection?> OrganizationMembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput where,
        OrganizationMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.OrganizationId);

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await organizationMemberService.GetPaginatedOrganizationMembersAsync(
                new PaginationInputParam(after, first, before, last),
                new OrganizationMemberSearchCriteria(where.OrganizationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        return new OrganizationMemberOrder(direction, item.Field);
                    }).ToList(),
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
            Edges = edges.Select(mapper.MapTo).ToArray(),
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
        var (organizationMemberAttendancePercentages, organizationDailyBookingsTotals) =
            await organizationAnalyticsService.GetAnalyticsAsync(organizationId, from, until, cancellationToken);
        return mapper.MapTo(organizationMemberAttendancePercentages, organizationDailyBookingsTotals);
    }

    [UseResolverScope]
    public async Task<bool> IsAzureTenantInstalledAsync(
        [Service] IAzureTenantService azureTenantService,
        CancellationToken cancellationToken) =>
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
    public async Task<OrganizationTagConnection?> CustomTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomTagOrganizationTagWhereInput where,
        OrganizationTagOrderInput[]? orderBy,
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
    public async Task<OrganizationTagConnection?> ZonesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ZoneOrganizationTagWhereInput where,
        OrganizationTagOrderInput[]? orderBy,
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
    public async Task<OrganizationTagDetails?> ZoneAsync(
        string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(tag);
    }

    [UseResolverScope]
    public async Task<OrganizationTagDetails?> CustomTagAsync(
        string id,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(tag);
    }

    [UseResolverScope]
    public OrganizationOfferingDetails? OrganizationOffering(string code, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var matchedOffering = Offerings.AllOfferings.FirstOrDefault(item => item.ToOfferingCode() == code);
        var offering = matchedOffering.GetOffering();

        return new OrganizationOfferingDetails
        {
            Code = matchedOffering.ToOfferingCode(),
            Name = offering.Name,
            UnitPrice = offering.UnitPrice,
            FeatureSet = mapper.MapTo(offering).ToArray(),
            Free = matchedOffering.IsFreeOffering(),
            StartColor = offering.StartColor,
            EndColor = offering.EndColor,
            ColorTiltingAngle = offering.ColorTiltingAngle,
        };
    }

    private async Task<OrganizationTagConnection?> OrganizationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        TagSearchCriteria tagSearchCriteria,
        OrganizationTagOrderInput[]? orderBy,
        ICachedCustomerService cachedCustomerService,
        ITagService tagService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await tagService.GetPaginatedTagsAsync(
                new PaginationInputParam(after, first, before, last),
                tagSearchCriteria,
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        return new TagOrder(direction, item.Field);
                    }).ToList(),
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
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }
}
