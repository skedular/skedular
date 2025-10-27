using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Analytics;
using Location.Api.GraphQL.PhysicalAddress;
using Location.Api.GraphQL.Resource;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;
using Constants = Api.Shared.Services.Constants;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationDetails")]
public class LocationDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string OrganizationUniqueAlphanumericName { get; set; } = string.Empty;

    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("type")] public LocationTypeDetails Type { get; set; } = new();
    [GraphQLName("openingHours")] public OpeningHours OpeningHours { get; set; } = new();
    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }
    [GraphQLName("roomCapacity")] public int RoomCapacity { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("physicalAddress")] public LocationPhysicalAddressDetails? PhysicalAddress { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("resourceTypeIds")] public IEnumerable<string> ResourceTypeIds { get; set; } = [];
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("locationSpaceTypeIds")] public IEnumerable<string> LocationSpaceTypeIds { get; set; } = [];
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
    [GraphQLName("extraMetadata")] public LocationExtraMetadata? ExtraMetadata { get; set; }
    [GraphQLName("uniqueClaimCode")] public string? UniqueClaimCode { get; set; }
    [GraphQLName("contactedViaEmail")] public bool ContactedViaEmail { get; set; }
    [GraphQLName("contactedViaSms")] public bool ContactedViaSms { get; set; }
    [GraphQLName("contactedViaCall")] public bool ContactedViaCall { get; set; }
    [GraphQLName("contactedViaWhatsapp")] public bool ContactedViaWhatsapp { get; set; }
    [GraphQLName("productIds")] public IEnumerable<string> ProductIds { get; set; } = [];

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;

    [UseResolverScope]
    public async Task<LocationAnalytics> AnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        [Parent] LocationDetails location,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var locationAnalytics = await locationAnalyticsService.GetAnalyticsAsync(location.Id, from, until, cancellationToken);
        return mapper.MapTo(
            locationAnalytics.Name,
            locationAnalytics.DesksOccupancyPercentage,
            locationAnalytics.DailyBookingsTotal,
            locationAnalytics.RoomsOccupancyPercentage);
    }

    [UseResolverScope]
    public async Task<Connection<ResourceEdge>> ResourcesAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        ResourceWhereInput? where,
        IEnumerable<ResourceOrderInput>? orderBy,
        [Parent] LocationDetails location,
        [Service] IResourceService resourceService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (location.OrganizationUniqueAlphanumericName == Constants.SkedularPublicLocationsUniqueAlphanumericName)
        {
            return Connection<ResourceEdge>.Empty;
        }

        var (paginatedInfo, edges, totalCount) = await resourceService.GetPaginatedResourcesAsync(
            new PaginationInputParam(after, first, before, last),
            new ResourceSearchCriteria(
                location.Id,
                where?.NameContains,
                where is null
                    ? []
                    : where.CustomTagIds.ToSafeCollection().Concat(
                        where.ZoneIds.ToSafeCollection()).Concat(where.ProductTagIds.ToSafeCollection()).ToList(),
                where?.FloorPlanId),
            orderBy.ToSafeCollection().Select(item => new ResourceOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return new Connection<ResourceEdge>
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
    public async Task<bool> HasFutureBookingAsync(
        [Parent] LocationDetails location,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        location.OrganizationUniqueAlphanumericName != Constants.SkedularPublicLocationsUniqueAlphanumericName &&
        await locationService.HasFutureBookingAsync(location.Id, false, cancellationToken);
}

[ObjectType<LocationDetails>]
public static partial class LocationDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<LocationDetails> descriptor)
    {
        descriptor.Ignore(item => item.OrganizationId);
        descriptor.Ignore(item => item.OrganizationUniqueAlphanumericName);
        descriptor.Ignore(item => item.CustomTagIds);
        descriptor.Ignore(item => item.ZoneIds);
        descriptor.Ignore(item => item.ResourceTypeIds);
        descriptor.Ignore(item => item.LocationTagIds);
        descriptor.Ignore(item => item.LocationSpaceTypeIds);
        descriptor.Ignore(item => item.ProductIds);
    }

    public static OrganizationDetails GetOrganization([Parent] LocationDetails item) =>
        new(item.OrganizationId, item.OrganizationUniqueAlphanumericName);

    public static IEnumerable<OrganizationTagDetails> GetCustomTags([Parent] LocationDetails item) =>
        item.CustomTagIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetZones([Parent] LocationDetails item) =>
        item.ZoneIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetResourceTypes([Parent] LocationDetails item) =>
        item.ResourceTypeIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetLocationTags([Parent] LocationDetails item) =>
        item.LocationTagIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetLocationSpaceTypes([Parent] LocationDetails item) =>
        item.LocationSpaceTypeIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<ProductDetails> GetProducts([Parent] LocationDetails item) =>
        item.ProductIds.Select(id => new ProductDetails(id));
}
