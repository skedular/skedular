using System.Reflection;
using Api.Shared.Models;
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
    public OrganizationMemberMembershipType[] OrganizationMemberMembershipTypes() =>
    [
        OrganizationMemberMembershipType.Owner,
        OrganizationMemberMembershipType.Administrator,
        OrganizationMemberMembershipType.Member
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
                        var field = item.Field switch
                        {
                            OrganizationOrderField.Name => Shared.Models.OrganizationOrderField.Name,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new OrganizationOrder(direction, field);
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
                        var field = item.Field switch
                        {
                            OrganizationMemberOrderField.MembershipType =>
                                Shared.Models.OrganizationMemberOrderField.MembershipType,
                            OrganizationMemberOrderField.Status => Shared.Models.OrganizationMemberOrderField.Status,
                            OrganizationMemberOrderField.Name => Shared.Models.OrganizationMemberOrderField.Name,
                            OrganizationMemberOrderField.GivenName =>
                                Shared.Models.OrganizationMemberOrderField.GivenName,
                            OrganizationMemberOrderField.MiddleName =>
                                Shared.Models.OrganizationMemberOrderField.MiddleName,
                            OrganizationMemberOrderField.FamilyName =>
                                Shared.Models.OrganizationMemberOrderField.FamilyName,
                            OrganizationMemberOrderField.PhoneNumber =>
                                Shared.Models.OrganizationMemberOrderField.PhoneNumber,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new OrganizationMemberOrder(direction, field);
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
    public async Task<OrganizationDetails?> AzureTenantOrganizationAsync(
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationService.GetByAzureTenantAsync(cancellationToken));

    [UseResolverScope]
    public async Task<OrganizationTagConnection?> DeskTypesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        DeskTypeOrganizationTagWhereInput where,
        OrganizationTagOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
        CancellationToken cancellationToken) =>
        await OrganizationTagsAsync(
            after,
            first,
            before,
            last,
            new TagSearchCriteria(where.OrganizationId, OrganizationTagType.DeskType, where.NameContains),
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
            new TagSearchCriteria(where.OrganizationId, OrganizationTagType.Zone, where.NameContains),
            orderBy,
            cachedCustomerService,
            tagService,
            cancellationToken);

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
                        var field = item.Field switch
                        {
                            OrganizationTagOrderField.Name => TagOrderField.Name,
                            OrganizationTagOrderField.Description => TagOrderField.Description,
                            OrganizationTagOrderField.TagType => TagOrderField.TagType,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new TagOrder(direction, field);
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
