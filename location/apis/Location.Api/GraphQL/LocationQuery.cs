using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Location;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;
using DeskOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Location.DeskOrderInput;
using DeskOrderField = Api.Shared.Services.GraphQL.UnityHub.V1.Location.DeskOrderField;
using LocationMemberOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Location.LocationMemberOrderInput;
using LocationMemberOrderField = Api.Shared.Services.GraphQL.UnityHub.V1.Location.LocationMemberOrderField;
using LocationOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Location.LocationOrderInput;
using LocationOrderField = Api.Shared.Services.GraphQL.UnityHub.V1.Location.LocationOrderField;
using OrderDirection = Api.Shared.Services.GraphQL.UnityHub.V1.Location.OrderDirection;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Location.Version;

namespace Location.Api.GraphQL;

public class LocationQuery(IMapper mapper) : Query
{
    public override Task<Version> LocationVersionAsync(
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

    public override async Task<bool> LocationCustomerRecordSyncedAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public override Task<LocationMemberMembershipType[]> LocationMemberMembershipTypesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken) => Task.FromResult(new[]
    {
        LocationMemberMembershipType.OWNER, LocationMemberMembershipType.ADMINISTRATOR,
        LocationMemberMembershipType.MEMBER
    });

    public override async Task<LocationDetails?> LocationAsync(
        string id,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var location = await service.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(location);
    }

    public override async Task<LocationConnection> LocationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationWhereInput where,
        LocationOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new LocationConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedLocationsAsync(
                new PaginationInputParam(after, first, before, last),
                new LocationSearchCriteria(where.OrganizationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            LocationOrderField.name =>
                                Shared.Models.LocationOrderField.Name,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new LocationOrder(direction, field);
                    }).ToList(),
                false,
                cancellationToken);

        return new LocationConnection
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

    public override async Task<LocationDetails[]> MyLocationsAsync(
        string? organizationId,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return [];
        }

        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var locations = await service.GetMyLocationsAsync(organizationId, cancellationToken);
        return mapper.MapTo(locations).ToArray();
    }

    public override async Task<LocationMemberConnection> PaginatedLocationMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationMemberWhereInput where,
        LocationMemberOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(where.LocationId))
        {
            return new LocationMemberConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new LocationMemberConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        var service = scope.ServiceProvider.GetRequiredService<ILocationMemberService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedLocationMembersAsync(
                new PaginationInputParam(after, first, before, last),
                new LocationMemberSearchCriteria(where.LocationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            LocationMemberOrderField.membershipType => Shared.Models.LocationMemberOrderField
                                .MembershipType,
                            LocationMemberOrderField.name => Shared.Models.LocationMemberOrderField.Name,
                            LocationMemberOrderField.givenName => Shared.Models.LocationMemberOrderField.GivenName,
                            LocationMemberOrderField.middleName => Shared.Models.LocationMemberOrderField.MiddleName,
                            LocationMemberOrderField.familyName => Shared.Models.LocationMemberOrderField.FamilyName,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new LocationMemberOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new LocationMemberConnection
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

    public override async Task<LocationTagConnection> PaginatedLocationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationTagWhereInput where,
        LocationTagOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new LocationTagConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        var service = scope.ServiceProvider.GetRequiredService<ITagService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedTagsAsync(
                new PaginationInputParam(after, first, before, last),
                new TagSearchCriteria(where.LocationId, where.TagType, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            LocationTagOrderField.name => TagOrderField.Name,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new TagOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new LocationTagConnection
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

    public override async Task<DeskConnection> PaginatedLocationDesksAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        DeskWhereInput where,
        DeskOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        if (!await customerService.DoesCustomerExistAsync(cancellationToken))
        {
            return new DeskConnection { PageInfo = new PageInfo(), Edges = [], TotalCount = 0 };
        }

        var service = scope.ServiceProvider.GetRequiredService<IDeskService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedDesksAsync(
                new PaginationInputParam(after, first, before, last),
                new DeskSearchCriteria(where.LocationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            DeskOrderField.name => Shared.Models.DeskOrderField.Name,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new DeskOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new DeskConnection
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

    public override async Task<LocationAnalytics> LocationAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationAnalyticsService>();
        var (locationDesksOccupancyPercentages, locationDailyBookingsTotals) =
            await service.GetAnalyticsAsync(locationId, from, until, cancellationToken);
        return mapper.MapTo(locationDesksOccupancyPercentages, locationDailyBookingsTotals);
    }
}
