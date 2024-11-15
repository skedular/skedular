using System.Reflection;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL;

public class OrganizationQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> OrganizationVersionAsync(CancellationToken cancellationToken)
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

    public async Task<bool> OrganizationCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public async Task<OrganizationTermsOfUse> ActiveOrganizationTermsOfUseAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationTermsOfUseService>();
        var termsOfUse = await service.GetActiveTermsOfUseAsync(cancellationToken);
        return mapper.MapTo(termsOfUse)!;
    }

    public Task<OrganizationMemberMembershipType[]>
        OrganizationMemberMembershipTypesAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new[]
        {
            OrganizationMemberMembershipType.OWNER, OrganizationMemberMembershipType.ADMINISTRATOR,
            OrganizationMemberMembershipType.MEMBER
        });

    public async Task<OrganizationIndustryMainCategoryReferenceDetails[]>
        OrganizationIndustryMainCategoriesReferencesAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IIndustryMainCategoryService>();
        var industryMainCategories = await service.GetAllAsync(cancellationToken);
        return mapper.MapTo(industryMainCategories).ToArray();
    }

    public async Task<OrganizationDetails?> OrganizationAsync(string id, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(organization);
    }

    public async Task<OrganizationConnection?> OrganizationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        OrganizationWhereInput where,
        OrganizationOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
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
                            OrganizationOrderField.name => Shared.Models.OrganizationOrderField.Name,
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

    public async Task<OrganizationDetails[]?> MyOrganizationsAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();

        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organizations = await service.GetMyOrganizationsAsync(cancellationToken);
        return mapper.MapTo(organizations).ToArray();
    }

    public async Task<OrganizationMemberConnection?> PaginatedOrganizationMembersAsync(
        string? after,
        int? first,
        string? before, int? last,
        OrganizationMemberWhereInput where,
        OrganizationMemberOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.OrganizationId);

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
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
                            OrganizationMemberOrderField.membershipType =>
                                Shared.Models.OrganizationMemberOrderField.MembershipType,
                            OrganizationMemberOrderField.name =>
                                Shared.Models.OrganizationMemberOrderField.Name,
                            OrganizationMemberOrderField.givenName =>
                                Shared.Models.OrganizationMemberOrderField.GivenName,
                            OrganizationMemberOrderField.middleName =>
                                Shared.Models.OrganizationMemberOrderField.MiddleName,
                            OrganizationMemberOrderField.familyName =>
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

    public async Task<OrganizationMemberDetails[]?> OrganizationMembersAsync(
        OrganizationMemberWhereInput where,
        OrganizationMemberOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedOrganizationMembersAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }

    public async Task<OrganizationAnalytics?> OrganizationAnalyticsAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationAnalyticsService>();
        var (organizationMemberAttendancePercentages, organizationDailyBookingsTotals) =
            await service.GetAnalyticsAsync(organizationId, from, until, cancellationToken);
        return mapper.MapTo(organizationMemberAttendancePercentages, organizationDailyBookingsTotals);
    }

    public async Task<bool> IsAzureTenantInstalledAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IAzureTenantService>();
        return await service.DoesTenantExistAsync(cancellationToken);
    }

    public async Task<string> AzureTenantAdminConsentUrlAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IAzureTenantService>();
        return await service.GenerateAdminConsentUrlAsync(cancellationToken);
    }

    public async Task<OrganizationDetails?> AzureTenantOrganizationAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.GetByAzureTenantAsync(cancellationToken);
        return mapper.MapTo(organization);
    }
}
