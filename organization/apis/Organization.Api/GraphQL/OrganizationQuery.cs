using System.Reflection;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL;

public class OrganizationQuery
{
    [UseServiceScope]
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

    [UseServiceScope]
    public async Task<bool> OrganizationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseServiceScope]
    public async Task<OrganizationTermsOfUse> ActiveOrganizationTermsOfUseAsync(
        [Service] IOrganizationTermsOfUseService organizationTermsOfUseService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var termsOfUse = await organizationTermsOfUseService.GetActiveTermsOfUseAsync(cancellationToken);
        return mapper.MapTo(termsOfUse)!;
    }

    [UseServiceScope]
    public OrganizationMemberMembershipType[] OrganizationMemberMembershipTypes() =>
    [
        OrganizationMemberMembershipType.Owner,
        OrganizationMemberMembershipType.Administrator,
        OrganizationMemberMembershipType.Member
    ];

    [UseServiceScope]
    public async Task<OrganizationIndustryMainCategoryReferenceDetails[]>
        OrganizationIndustryMainCategoriesReferencesAsync(
            [Service] IIndustryMainCategoryService industryMainCategoryService,
            [Service] IMapper mapper,
            CancellationToken cancellationToken)
    {
        var industryMainCategories = await industryMainCategoryService.GetAllAsync(cancellationToken);
        return mapper.MapTo(industryMainCategories).ToArray();
    }

    [UseServiceScope]
    public async Task<OrganizationDetails?> OrganizationAsync(
        string id,
        [Service] IOrganizationService organizationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(organization);
    }

    [UseServiceScope]
    public async Task<OrganizationConnection?> OrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationWhereInput where,
        OrganizationOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        [Service] IMapper mapper,
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
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
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

    [UseServiceScope]
    public async Task<OrganizationDetails[]?> MyOrganizationsAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationService organizationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var organizations = await organizationService.GetMyOrganizationsAsync(cancellationToken);
        return mapper.MapTo(organizations).ToArray();
    }

    [UseServiceScope]
    public async Task<OrganizationMemberConnection?> PaginatedOrganizationMembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput where,
        OrganizationMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationMemberService organizationMemberService,
        [Service] IMapper mapper,
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
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            OrganizationMemberOrderField.MembershipType =>
                                Shared.Models.OrganizationMemberOrderField.MembershipType,
                            OrganizationMemberOrderField.Name =>
                                Shared.Models.OrganizationMemberOrderField.Name,
                            OrganizationMemberOrderField.GivenName =>
                                Shared.Models.OrganizationMemberOrderField.GivenName,
                            OrganizationMemberOrderField.MiddleName =>
                                Shared.Models.OrganizationMemberOrderField.MiddleName,
                            OrganizationMemberOrderField.FamilyName =>
                                Shared.Models.OrganizationMemberOrderField.FamilyName,
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

    [UseServiceScope]
    public async Task<OrganizationMemberDetails[]?> OrganizationMembersAsync(
        OrganizationMemberWhereInput where,
        OrganizationMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationMemberService organizationMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedOrganizationMembersAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cachedCustomerService,
            organizationMemberService,
            mapper,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }

    [UseServiceScope]
    public async Task<OrganizationAnalytics?> OrganizationAnalyticsAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        [Service] IOrganizationAnalyticsService organizationAnalyticsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var (organizationMemberAttendancePercentages, organizationDailyBookingsTotals) =
            await organizationAnalyticsService.GetAnalyticsAsync(organizationId, from, until, cancellationToken);
        return mapper.MapTo(organizationMemberAttendancePercentages, organizationDailyBookingsTotals);
    }

    [UseServiceScope]
    public async Task<bool> IsAzureTenantInstalledAsync(
        [Service] IAzureTenantService azureTenantService,
        CancellationToken cancellationToken) =>
        await azureTenantService.DoesTenantExistAsync(cancellationToken);

    [UseServiceScope]
    public async Task<string> AzureTenantAdminConsentUrlAsync(
        [Service] IAzureTenantService azureTenantService,
        CancellationToken cancellationToken) =>
        await azureTenantService.GenerateAdminConsentUrlAsync(cancellationToken);

    [UseServiceScope]
    public async Task<OrganizationDetails?> AzureTenantOrganizationAsync(
        [Service] IOrganizationService organizationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await organizationService.GetByAzureTenantAsync(cancellationToken));
}
