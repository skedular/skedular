using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Organization;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using OrderDirection = Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrderDirection;
using OrganizationOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationOrderInput;
using OrganizationMemberOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationMemberOrderInput;
using OrganizationOrderField = Organization.Shared.Models.OrganizationOrderField;
using OrganizationMemberOrderField = Organization.Shared.Models.OrganizationMemberOrderField;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Organization.Version;

namespace Organization.Api.GraphQL;

public class OrganizationQuery(IMapper mapper) : Query
{
    public override Task<Version> OrganizationVersionAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<bool> OrganizationCustomerRecordSyncedAsync(IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override async Task<OrganizationTermsOfUse> ActiveOrganizationTermsOfUseAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationTermsOfUseService>();
        var termsOfUse = await service.GetActiveTermsOfUseAsync(cancellationToken);
        return mapper.MapTo(termsOfUse)!;
    }

    public override Task<OrganizationMemberMembershipType[]> OrganizationMemberMembershipTypesAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken) =>
        Task.FromResult(new[]
        {
            OrganizationMemberMembershipType.OWNER, OrganizationMemberMembershipType.ADMINISTRATOR,
            OrganizationMemberMembershipType.MEMBER
        });

    public override async Task<OrganizationIndustryMainCategoryReferenceDetails[]>
        OrganizationIndustryMainCategoriesReferencesAsync(IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IIndustryMainCategoryService>();
        var industryMainCategories = await service.GetAllAsync(cancellationToken);
        return mapper.MapTo(industryMainCategories).ToArray();
    }

    public override async Task<OrganizationDetails?> OrganizationAsync(
        string id,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(organization);
    }

    public override async Task<OrganizationConnection> OrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationWhereInput where,
        OrganizationOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new OrganizationConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedOrganizationsAsync(
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
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationOrderField.name =>
                                OrganizationOrderField.Name,
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

    public override async Task<OrganizationDetails[]> MyOrganizationsAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();

        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return [];
        }

        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organizations = await service.GetMyOrganizationsAsync(cancellationToken);
        return mapper.MapTo(organizations).ToArray();
    }

    public override async Task<OrganizationMemberConnection> PaginatedOrganizationMembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput where,
        OrganizationMemberOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(where.OrganizationId))
        {
            return new OrganizationMemberConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new OrganizationMemberConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        var service = scope.ServiceProvider.GetRequiredService<IOrganizationMemberService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedOrganizationMembersAsync(
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
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationMemberOrderField
                                    .membershipType =>
                                OrganizationMemberOrderField.MembershipType,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationMemberOrderField
                                    .name =>
                                OrganizationMemberOrderField.Name,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationMemberOrderField
                                    .givenName =>
                                OrganizationMemberOrderField.GivenName,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationMemberOrderField
                                    .middleName =>
                                OrganizationMemberOrderField.MiddleName,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Organization.OrganizationMemberOrderField
                                    .familyName =>
                                OrganizationMemberOrderField.FamilyName,
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

    public override async Task<OrganizationAnalytics> OrganizationAnalyticsAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationAnalyticsService>();
        var (organizationMemberAttendancePercentages, organizationDailyBookingsTotals) =
            await service.GetAnalyticsAsync(organizationId, from, until, cancellationToken);
        return mapper.MapTo(organizationMemberAttendancePercentages, organizationDailyBookingsTotals);
    }

    public override async Task<bool> IsAzureTenantInstalledAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IAzureTenantService>();
        return await service.DoesTenantExistAsync(cancellationToken);
    }

    public override async Task<string> AzureTenantAdminConsentUrlAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IAzureTenantService>();
        return await service.GenerateAdminConsentUrlAsync(cancellationToken);
    }

    public override async Task<OrganizationDetails?> AzureTenantOrganizationAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.GetByAzureTenantAsync(cancellationToken);
        return mapper.MapTo(organization);
    }
}
