using System.Reflection;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;

namespace Location.Api.GraphQL;

public class LocationQuery
{
    [UseServiceScope]
    public Version LocationVersion()
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
    public async Task<bool> LocationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseServiceScope]
    public LocationMemberMembershipType[] LocationMemberMembershipTypes(
        CancellationToken cancellationToken) =>
    [
        LocationMemberMembershipType.OWNER,
        LocationMemberMembershipType.ADMINISTRATOR,
        LocationMemberMembershipType.MEMBER
    ];

    [UseServiceScope]
    public async Task<LocationDetails?> LocationAsync(
        string id,
        [Service] ILocationService locationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var location = await locationService.GetByIdAsync(id, false, cancellationToken);
        return mapper.MapTo(location);
    }

    [UseServiceScope]
    public async Task<LocationConnection?> LocationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationWhereInput where,
        LocationOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await locationService.GetPaginatedLocationsAsync(
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

    [UseServiceScope]
    public async Task<LocationDetails[]?> MyLocationsAsync(
        string? organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var locations = await locationService.GetMyLocationsAsync(organizationId, cancellationToken);
        return mapper.MapTo(locations).ToArray();
    }

    [UseServiceScope]
    public async Task<LocationMemberConnection?> PaginatedLocationMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationMemberWhereInput where,
        LocationMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationMemberService locationMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.LocationId);

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await locationMemberService.GetPaginatedLocationMembersAsync(
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

    [UseServiceScope]
    public async Task<LocationMemberDetails[]?> LocationMembersAsync(
        LocationMemberWhereInput where,
        LocationMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationMemberService locationMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedLocationMembersAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cachedCustomerService,
            locationMemberService,
            mapper,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }

    [UseServiceScope]
    public async Task<LocationTagConnection?> PaginatedLocationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationTagWhereInput where,
        LocationTagOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await tagService.GetPaginatedTagsAsync(
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

    [UseServiceScope]
    public async Task<DeskConnection?> PaginatedLocationDesksAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        DeskWhereInput where,
        DeskOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IDeskService deskService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await deskService.GetPaginatedDesksAsync(
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

    [UseServiceScope]
    public async Task<LocationAnalytics?> LocationAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var (locationDesksOccupancyPercentages, locationDailyBookingsTotals) =
            await locationAnalyticsService.GetAnalyticsAsync(locationId, from, until, cancellationToken);
        return mapper.MapTo(locationDesksOccupancyPercentages, locationDailyBookingsTotals);
    }
}
