using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;
using Location.Shared.Services.Cache;

namespace Location.Api.GraphQL.Location;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public IEnumerable<LocationTypeDetails> LocationTypes() =>
    [
        new() { Type = LocationType.Private, Name = LocationTypeConstants.Private.ToLocationTypeName() },
        new() { Type = LocationType.Marketplace, Name = LocationTypeConstants.Marketplace.ToLocationTypeName() }
    ];

    [UseResolverScope]
    public IEnumerable<LocationRestrictedInformationCategoryDetails> LocationRestrictedInformationCategories() =>
    [
        new()
        {
            Category = LocationRestrictedInformationCategory.Wifi,
            Name = LocationRestrictedInformationCategory.Wifi.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Access,
            Name = LocationRestrictedInformationCategory.Access.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Parking,
            Name = LocationRestrictedInformationCategory.Parking.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.CheckIn,
            Name = LocationRestrictedInformationCategory.CheckIn.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.CheckOut,
            Name = LocationRestrictedInformationCategory.CheckOut.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.AfterHours,
            Name = LocationRestrictedInformationCategory.AfterHours.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Deliveries,
            Name = LocationRestrictedInformationCategory.Deliveries.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Guests,
            Name = LocationRestrictedInformationCategory.Guests.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Equipment,
            Name = LocationRestrictedInformationCategory.Equipment.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Kitchen,
            Name = LocationRestrictedInformationCategory.Kitchen.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.MeetingRooms,
            Name = LocationRestrictedInformationCategory.MeetingRooms.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Noise,
            Name = LocationRestrictedInformationCategory.Noise.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Maintenance,
            Name = LocationRestrictedInformationCategory.Maintenance.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Accessibility,
            Name = LocationRestrictedInformationCategory.Accessibility.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Storage,
            Name = LocationRestrictedInformationCategory.Storage.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Cleaning,
            Name = LocationRestrictedInformationCategory.Cleaning.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Waste,
            Name = LocationRestrictedInformationCategory.Waste.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Security,
            Name = LocationRestrictedInformationCategory.Security.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Evacuation,
            Name = LocationRestrictedInformationCategory.Evacuation.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Pets,
            Name = LocationRestrictedInformationCategory.Pets.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Smoking,
            Name = LocationRestrictedInformationCategory.Smoking.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.HouseRules,
            Name = LocationRestrictedInformationCategory.HouseRules.ToLocationRestrictedInformationCategoryName()
        },
        new()
        {
            Category = LocationRestrictedInformationCategory.Other,
            Name = LocationRestrictedInformationCategory.Other.ToLocationRestrictedInformationCategoryName()
        }
    ];

    [UseResolverScope]
    public async Task<LocationDetails?> LocationAsync(string id, [Service] ILocationService locationService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await locationService.GetByIdAsync(id, false, cancellationToken));

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<LocationDetails?> LocationByIdAsync(
        [ID] string id,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await locationService.GetByIdAsync(id, true, cancellationToken));

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
                where.OrganizationCustomDomain,
                where.LocationIds.ToSafeCollection(),
                where.NameContains,
                where.ZoneIds.ToSafeCollection().Concat(where.CustomTagIds.ToSafeCollection()).ToList(),
                null,
                where.Types.ToSafeCollection(),
                where.SearchBoundaries,
                where.NotContactedYet,
                where.ResourceType,
                null,
                where.ProductIds.ToSafeCollection()),
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
            Edges = edges.Select(graphQlMapper.MapTo),
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
        if (where.SearchBoundaries is null &&
            string.IsNullOrWhiteSpace(where.OrganizationId) &&
            string.IsNullOrWhiteSpace(where.OrganizationCustomDomain) &&
            where.ProductIds.ToSafeCollection().Count == 0)
        {
            return Connection<LocationEdge>.Empty;
        }

        var (paginatedInfo, edges, totalCount) = await locationService.GetPaginatedLocationsAsync(
            new PaginationInputParam(after, first, before, last),
            new LocationSearchCriteria(
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.LocationIds.ToSafeCollection(),
                where.NameContains,
                where.ZoneIds.ToSafeCollection().Concat(where.CustomTagIds.ToSafeCollection()).ToList(),
                null,
                [LocationType.Marketplace],
                where.SearchBoundaries,
                null,
                where.ResourceType,
                true,
                where.ProductIds.ToSafeCollection()),
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
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<LocationDetails>?> MyLocationsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        await cachedCustomerService.DoesCustomerExistAsync(cancellationToken)
            ? graphQlMapper.MapTo(await locationService.GetMyLocationsAsync(organizationId, organizationCustomDomain, cancellationToken))
            : null;
}
