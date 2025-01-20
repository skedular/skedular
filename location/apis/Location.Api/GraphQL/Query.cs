using System.Reflection;
using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Location.Api.GraphQL;

[QueryType]
public class Query(IMapper mapper)
{
    [UseResolverScope]
    public Version LocationVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision };
    }

    [UseResolverScope]
    public async Task<bool> LocationCustomerRecordSyncedAsync(
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken);

    [UseResolverScope]
    public LocationMemberRole[] LocationMemberRoles() =>
    [
        LocationMemberRole.Owner,
        LocationMemberRole.Administrator,
        LocationMemberRole.Member
    ];

    [UseResolverScope]
    public async Task<LocationDetails?> LocationAsync(
        string id,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await locationService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    public async Task<LocationConnection?> LocationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationWhereInput where,
        LocationOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(after, first, before, last),
            new LocationSearchCriteria(
                where.OrganizationId,
                where.LocationIds,
                where.NameContains,
                where.ZoneIds ?? [],
                where.CustomTagIds ?? []),
            orderBy is null
                ? []
                : orderBy.Select(item =>
                {
                    var direction = item.Direction == OrderDirection.Ascending ? OrderDirection.Ascending : OrderDirection.Descending;
                    return new LocationOrder(direction, item.Field);
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

    [UseResolverScope]
    public async Task<LocationDetails[]?> MyLocationsAsync(
        string? organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        return mapper.MapTo(await locationService.GetMyLocationsAsync(organizationId, cancellationToken)).ToArray();
    }

    [UseResolverScope]
    public async Task<LocationMemberConnection?> LocationMembersAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationMemberWhereInput where,
        LocationMemberOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationMemberService locationMemberService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.LocationId);

        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) = await locationMemberService.GetPaginatedLocationMembersAsync(
            new PaginationInputParam(after, first, before, last),
            new LocationMemberSearchCriteria(where.LocationId, where.NameContains),
            orderBy is null
                ? []
                : orderBy.Select(item =>
                {
                    var direction = item.Direction == OrderDirection.Ascending ? OrderDirection.Ascending : OrderDirection.Descending;
                    return new LocationMemberOrder(direction, item.Field);
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

    [UseResolverScope]
    public async Task<DeskConnection?> DesksAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        DeskWhereInput where,
        DeskOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) = await deskService.GetPaginatedDesksAsync(
            new PaginationInputParam(after, first, before, last),
            new DeskSearchCriteria(where.LocationId, where.NameContains, where.ZoneIds, where.CustomTagIds),
            orderBy is null
                ? []
                : orderBy.Select(item =>
                {
                    var direction = item.Direction == OrderDirection.Ascending ? OrderDirection.Ascending : OrderDirection.Descending;
                    return new DeskOrder(direction, item.Field);
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

    [UseResolverScope]
    public async Task<DeskDetails?> DeskAsync(
        string id,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.GetByIdAsync(id, cancellationToken);
        return mapper.MapTo(desk);
    }

    [UseResolverScope]
    public async Task<LocationAnalytics?> LocationAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var locationAnalytics = await locationAnalyticsService.GetAnalyticsAsync(locationId, from, until, cancellationToken);
        return mapper.MapTo(locationAnalytics.Name, locationAnalytics.DesksOccupancyPercentage, locationAnalytics.DailyBookingsTotal);
    }

    [UseResolverScope]
    public async Task<LocationAnalytics[]> LocationsAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationWhereInput where,
        LocationOrderInput[]? orderBy,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var locationsAnalytics = await locationAnalyticsService.GetAnalyticsAsync(
            from,
            until,
            new LocationSearchCriteria(
                where.OrganizationId,
                where.LocationIds,
                where.NameContains,
                where.ZoneIds ?? [],
                where.CustomTagIds ?? []),
            orderBy is null
                ? []
                : orderBy.Select(item =>
                {
                    var direction = item.Direction == OrderDirection.Ascending ? OrderDirection.Ascending : OrderDirection.Descending;
                    return new LocationOrder(direction, item.Field);
                }).ToList(), cancellationToken);

        return locationsAnalytics
            .Select(locationAnalytics =>
                mapper.MapTo(locationAnalytics.Name, locationAnalytics.DesksOccupancyPercentage, locationAnalytics.DailyBookingsTotal))
            .ToArray();
    }
}
