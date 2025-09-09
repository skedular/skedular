using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Fusion.SourceSchema.Types;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public IEnumerable<LocationTypeDetails> LocationTypes() =>
    [
        new() { Type = LocationType.Private, Name = LocationTypeConstants.Private.ToLocationTypeName() },
        new() { Type = LocationType.Marketplace, Name = LocationTypeConstants.Marketplace.ToLocationTypeName() }
    ];

    [UseResolverScope]
    public async Task<LocationDetails?> LocationAsync(string id, [Service] ILocationService locationService, CancellationToken cancellationToken) =>
        mapper.MapTo(await locationService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<LocationDetails?> LocationByIdAsync(
        string id,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        await LocationAsync(id, locationService, cancellationToken);

    [UseResolverScope]
    public async Task<Connection<LocationEdge>> LocationsAsync(
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
                null,
                where.OrganizationUniqueAlphanumericName,
                where.LocationIds.ToSafeCollection(),
                where.NameContains,
                where.ZoneIds.ToSafeCollection().Concat(where.CustomTagIds.ToSafeCollection()).ToList(),
                null,
                where.Types.ToSafeCollection(),
                where.SearchBoundaries),
            orderBy.ToSafeCollection().Select(item => new LocationOrder(item.Direction, item.Field)).ToList(),
            false,
            cancellationToken);

        return new Connection<LocationEdge>
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
    public async Task<Connection<LocationEdge>> MarketplaceLocationsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        MarketplaceLocationWhereInput where,
        IEnumerable<LocationOrderInput>? orderBy,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        if (where.SearchBoundaries is null)
        {
            return Connection<LocationEdge>.Empty;
        }

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(after, first, before, last),
            new LocationSearchCriteria(
                null,
                null,
                where.LocationIds.ToSafeCollection(),
                where.NameContains,
                where.ZoneIds.ToSafeCollection().Concat(where.CustomTagIds.ToSafeCollection()).ToList(),
                null,
                [LocationType.Marketplace],
                where.SearchBoundaries),
            orderBy.ToSafeCollection().Select(item => new LocationOrder(item.Direction, item.Field)).ToList(),
            true,
            cancellationToken);

        return new Connection<LocationEdge>
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
        string? organizationUniqueAlphanumericName,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? mapper.MapTo(await locationService.GetMyLocationsAsync(organizationUniqueAlphanumericName, cancellationToken))
            : null;
}
