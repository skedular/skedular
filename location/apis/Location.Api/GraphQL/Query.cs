using System.Reflection;
using Enterprise.Shared;
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
    public async Task<LocationDetails?> LocationAsync(string id, [Service] ILocationService locationService, CancellationToken cancellationToken) =>
        mapper.MapTo(await locationService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    public async Task<LocationConnection> LocationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationWhereInput where,
        IEnumerable<LocationOrderInput>? orderBy,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(after, first, before, last),
            new LocationSearchCriteria(
                where.OrganizationId,
                where.LocationIds.ToSafeCollection(),
                where.NameContains,
                where.ZoneIds.ToSafeCollection().Concat(where.CustomTagIds.ToSafeCollection())),
            orderBy.ToSafeCollection().Select(item => new LocationOrder(item.Direction, item.Field)).ToList(),
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
            Edges = edges.Select(mapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<LocationDetails>?> MyLocationsAsync(
        string? organizationId,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? mapper.MapTo(await locationService.GetMyLocationsAsync(organizationId, cancellationToken))
            : null;

    [UseResolverScope]
    public async Task<LocationAnalytics?> LocationAnalyticsAsync(
        string locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var locationAnalytics = await locationAnalyticsService.GetAnalyticsAsync(locationId, from, until, cancellationToken);
        return mapper.MapTo(
            locationAnalytics.Name,
            locationAnalytics.DesksOccupancyPercentage,
            locationAnalytics.DailyBookingsTotal,
            locationAnalytics.RoomsOccupancyPercentage);
    }

    [UseResolverScope]
    public async Task<IEnumerable<LocationAnalytics>> LocationsAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationWhereInput where,
        IEnumerable<LocationOrderInput>? orderBy,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var locationsAnalytics = await locationAnalyticsService.GetAnalyticsAsync(
            from,
            until,
            new LocationSearchCriteria(
                where.OrganizationId,
                where.LocationIds.ToSafeCollection(),
                where.NameContains,
                where.CustomTagIds.ToSafeCollection().Concat(where.ZoneIds.ToSafeCollection())),
            orderBy.ToSafeCollection().Select(item => new LocationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return locationsAnalytics
            .Select(locationAnalytics =>
                mapper.MapTo(
                    locationAnalytics.Name,
                    locationAnalytics.DesksOccupancyPercentage,
                    locationAnalytics.DailyBookingsTotal,
                    locationAnalytics.RoomsOccupancyPercentage));
    }

    [UseResolverScope]
    public async Task<ResourceConnection> ResourcesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ResourceWhereInput where,
        IEnumerable<ResourceOrderInput>? orderBy,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await resourceService.GetPaginatedResourcesAsync(
            new PaginationInputParam(after, first, before, last),
            new ResourceSearchCriteria(
                where.LocationId,
                where.NameContains,
                where.CustomTagIds.ToSafeCollection().Concat(where.ZoneIds.ToSafeCollection()).Concat(where.ProductTagIds.ToSafeCollection())),
            orderBy.ToSafeCollection().Select(item => new ResourceOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new ResourceConnection
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
    public async Task<ResourceDetails?> ResourceAsync(string id, [Service] IResourceService resourceService, CancellationToken cancellationToken) =>
        mapper.MapTo(await resourceService.GetByIdAsync(id, cancellationToken));
}
