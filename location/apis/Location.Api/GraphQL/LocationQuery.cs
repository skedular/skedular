using System.Reflection;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Location.Api.GraphQL;

public class LocationQuery(IMapper mapper)
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
        LocationMemberMembershipType.Owner,
        LocationMemberMembershipType.Administrator,
        LocationMemberMembershipType.Member
    ];

    [UseServiceScope]
    public async Task<LocationDetails?> LocationAsync(
        string id,
        [Service] ILocationService locationService,
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
        CancellationToken cancellationToken)
    {
        if (!await cachedCustomerService.DoesCustomerExistAsync(cancellationToken))
        {
            return null;
        }

        var (paginatedInfo, edges, totalCount) =
            await locationService.GetPaginatedLocationsAsync(
                new PaginationInputParam(after, first, before, last),
                new LocationSearchCriteria(
                    where.OrganizationId, 
                    where.NameContains,
                    where.ZoneIds ?? [],
                    where.DeskTypeIds ?? []),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            LocationOrderField.Name => Shared.Models.LocationOrderField.Name,
                            LocationOrderField.About => Shared.Models.LocationOrderField.About,
                            LocationOrderField.Timezone => Shared.Models.LocationOrderField.Timezone,
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

        var (paginatedInfo, edges, totalCount) =
            await locationMemberService.GetPaginatedLocationMembersAsync(
                new PaginationInputParam(after, first, before, last),
                new LocationMemberSearchCriteria(where.LocationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            LocationMemberOrderField.MembershipType => Shared.Models.LocationMemberOrderField
                                .MembershipType,
                            LocationMemberOrderField.Name => Shared.Models.LocationMemberOrderField.Name,
                            LocationMemberOrderField.GivenName => Shared.Models.LocationMemberOrderField.GivenName,
                            LocationMemberOrderField.MiddleName => Shared.Models.LocationMemberOrderField.MiddleName,
                            LocationMemberOrderField.FamilyName => Shared.Models.LocationMemberOrderField.FamilyName,
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
    public async Task<LocationTagConnection?> LocationTagsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        LocationTagWhereInput where,
        LocationTagOrderInput[]? orderBy,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ITagService tagService,
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
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            LocationTagOrderField.Name => TagOrderField.Name,
                            LocationTagOrderField.Description => TagOrderField.Description,
                            LocationTagOrderField.TagType => TagOrderField.TagType,
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
    public async Task<DeskConnection?> LocationDesksAsync(
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

        var (paginatedInfo, edges, totalCount) =
            await deskService.GetPaginatedDesksAsync(
                new PaginationInputParam(after, first, before, last),
                new DeskSearchCriteria(where.LocationId, where.NameContains),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            DeskOrderField.Name => Shared.Models.DeskOrderField.Name,
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
        CancellationToken cancellationToken)
    {
        var (locationDesksOccupancyPercentages, locationDailyBookingsTotals) =
            await locationAnalyticsService.GetAnalyticsAsync(locationId, from, until, cancellationToken);
        return mapper.MapTo(locationDesksOccupancyPercentages, locationDailyBookingsTotals);
    }
}
