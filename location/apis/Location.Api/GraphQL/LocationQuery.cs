using System.Reflection;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;

namespace Location.Api.GraphQL;

public class LocationQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> LocationVersionAsync(CancellationToken cancellationToken)
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

    public async Task<bool> LocationCustomerRecordSyncedAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        return await service.DoesCustomerExistAsync(cancellationToken);
    }

    public Task<LocationMemberMembershipType[]> LocationMemberMembershipTypesAsync(
        CancellationToken cancellationToken) => Task.FromResult(new[]
    {
        LocationMemberMembershipType.OWNER, LocationMemberMembershipType.ADMINISTRATOR,
        LocationMemberMembershipType.MEMBER
    });

    public async Task<LocationDetails?> LocationAsync(string id, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var location = await service.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(location);
    }

    public async Task<LocationConnection?> LocationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationWhereInput where,
        LocationOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
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

    public async Task<LocationDetails[]?> MyLocationsAsync(
        string? organizationId,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var locations = await service.GetMyLocationsAsync(organizationId, cancellationToken);
        return mapper.MapTo(locations).ToArray();
    }

    public async Task<LocationMemberConnection?> PaginatedLocationMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationMemberWhereInput where,
        LocationMemberOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.LocationId);

        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
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

    public async Task<LocationMemberDetails[]?> LocationMembersAsync(
        LocationMemberWhereInput where,
        LocationMemberOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedLocationMembersAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }

    public async Task<LocationTagConnection?> PaginatedLocationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationTagWhereInput where,
        LocationTagOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
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

    public async Task<DeskConnection?> PaginatedLocationDesksAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        DeskWhereInput where,
        DeskOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
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

    public async Task<LocationAnalytics?> LocationAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationAnalyticsService>();
        var (locationDesksOccupancyPercentages, locationDailyBookingsTotals) =
            await service.GetAnalyticsAsync(locationId, from, until, cancellationToken);
        return mapper.MapTo(locationDesksOccupancyPercentages, locationDailyBookingsTotals);
    }
}
