using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
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
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("organizationCustomDomain")]
    public string OrganizationCustomDomain { get; set; } = string.Empty;

    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("type")] public LocationTypeDetails Type { get; set; } = new();
    [GraphQLName("openingHours")] public OpeningHours OpeningHours { get; set; } = new();
    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }
    [GraphQLName("roomCapacity")] public int RoomCapacity { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("physicalAddress")] public LocationPhysicalAddressDetails? PhysicalAddress { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("spaceTypes")] public IEnumerable<OrganizationTagDetails> SpaceTypes { get; set; } = [];
    [GraphQLName("amenities")] public IEnumerable<OrganizationTagDetails> Amenities { get; set; } = [];
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("extraMetadata")] public LocationExtraMetadata? ExtraMetadata { get; set; }
    [GraphQLName("uniqueClaimCode")] public string? UniqueClaimCode { get; set; }
    [GraphQLName("contactedViaEmail")] public bool ContactedViaEmail { get; set; }
    [GraphQLName("contactedViaSms")] public bool ContactedViaSms { get; set; }
    [GraphQLName("contactedViaCall")] public bool ContactedViaCall { get; set; }
    [GraphQLName("contactedViaWhatsapp")] public bool ContactedViaWhatsapp { get; set; }
    [GraphQLName("productIds")] public IEnumerable<string> ProductIds { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;

    [UseResolverScope]
    public async Task<LocationAnalytics> AnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        [Parent] LocationDetails location,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        [Service] IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var locationAnalytics = await locationAnalyticsService.GetAnalyticsAsync(location.Id, from, until, cancellationToken);
        return graphQlMapper.MapTo(
            locationAnalytics.Name,
            locationAnalytics.DesksOccupancyPercentage,
            locationAnalytics.DailyBookingsTotal,
            locationAnalytics.RoomsOccupancyPercentage,
            locationAnalytics.ResourceAvailabilitySnapshots);
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
        [Service] IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        if (location.OrganizationCustomDomain == Constants.SkedularPublicLocationsCustomDomainName)
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
            orderBy.ToSafeCollection().Select(item => new ResourceOrder(item.Direction, item.Field)),
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
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount
        };
    }
}

[ObjectType<LocationDetails>]
public static partial class LocationDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<LocationDetails> descriptor)
    {
        descriptor.Ignore(item => item.OrganizationId);
        descriptor.Ignore(item => item.OrganizationCustomDomain);
        descriptor.Ignore(item => item.ProductIds);
    }

    public static OrganizationDetails GetOrganization([Parent] LocationDetails item) =>
        new(item.OrganizationId, item.OrganizationCustomDomain);

    public static IEnumerable<ProductDetails> GetProducts([Parent] LocationDetails item) =>
        item.ProductIds.Select(id => new ProductDetails(id));
}
