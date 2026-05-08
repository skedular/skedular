using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Location.Api.GraphQL.Analytics;
using Location.Api.GraphQL.FloorPlan;
using Location.Api.GraphQL.Location;
using Location.Api.GraphQL.PhysicalAddress;
using Location.Api.GraphQL.Resource;
using Location.Shared.Models;
using NetTopologySuite.Geometries;
using LocationDesksOccupancyPercentage = Location.Shared.Models.LocationDesksOccupancyPercentage;
using LocationEdge = Location.Api.GraphQL.Location.LocationEdge;
using LocationDailyBookingsTotal = Location.Shared.Models.LocationDailyBookingsTotal;
using ResourceAvailabilitySnapshotReport = Location.Shared.Models.ResourceAvailabilitySnapshotReport;
using LocationRoomsOccupancyPercentage = Location.Shared.Models.LocationRoomsOccupancyPercentage;
using AddResourceInput = Location.Api.GraphQL.Resource.AddResourceInput;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using UpdateResourceInput = Location.Api.GraphQL.Resource.UpdateResourceInput;
using ResourceEdge = Location.Api.GraphQL.Resource.ResourceEdge;
using ResourcePosition = Location.Shared.Models.ResourcePosition;
using WeekOpeningHours = Api.Shared.Services.Models.WeekOpeningHours;

namespace Location.Api.Mappers;

public interface IGraphQlMapper
{
    LocationDetails? MapTo(Shared.Models.Location? src);
    ResourceDetails MapTo(Resource src);
    IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src);
    FloorPlanDetails? MapTo(FloorPlan? src);

    LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal,
        IEnumerable<LocationRoomsOccupancyPercentage> locationRoomsOccupancyPercentage,
        IEnumerable<ResourceAvailabilitySnapshotReport> resourceAvailabilitySnapshots);

    Shared.Models.Location MapTo(AddLocationInput src);
    Shared.Models.Location MapTo(UpdateLocationInput src);
    LocationEdge MapTo(Edge<Shared.Models.Location> src);
    Resource MapTo(AddResourceInput src);
    Resource MapTo(UpdateResourceInput src);
    ResourceEdge MapTo(Edge<Resource> src);
    WeekOpeningHours? MapTo(GraphQL.Location.WeekOpeningHours? src);
    FloorPlan MapTo(AddFloorPlanInput src);
    FloorPlan MapTo(UpdateFloorPlanInput src);
    FloorPlanEdge MapTo(Edge<FloorPlan> src);
    IEnumerable<ResourcePosition> MapTo(UpdateResourcePositionsInput src);
    LocationPhysicalAddress MapTo(AddLocationPhysicalAddressInput src);
    LocationPhysicalAddress MapTo(UpdateLocationPhysicalAddressInput src);
}

public class GraphQlMapper : IGraphQlMapper
{
    public LocationDetails? MapTo(Shared.Models.Location? src) =>
        src is null
            ? null
            : new LocationDetails
            {
                Id = src.Id,
                Name = src.Name,
                ListingMetadata = src.ListingMetadata,
                Timezone = src.Timezone,
                Type = new LocationTypeDetails { Type = src.Type, Name = src.Type.ToLocationTypeName() },
                ExtraMetadata = src.ExtraMetadata,
                FeatureImages = src.FeatureImages,
                FloorPlanCount = src.FloorPlans.Count,
                OpeningHours = MapTo(src.OpeningHours),
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanViewAnalytics = src.Permissions.CanViewAnalytics,
                DeskCapacity = src.Resources.Count(item => item.Tags.Any(tag => tag.Type == OrganizationTagType.ResourceDesk)),
                RoomCapacity = src.Resources.Count(item => item.Tags.Any(tag => tag.Type == OrganizationTagType.ResourceRoom)),
                OrganizationId = src.Organization.Id,
                OrganizationCustomDomain = src.Organization.CustomDomain.ToSafeString(),
                CustomTags = MapTo(src.CustomTags),
                Zones = MapTo(src.Zones),
                SpaceTypes = MapTo(src.SpaceTypes),
                Amenities = MapTo(src.Amenities),
                ResourceTypes = MapTo(src.Organization.Tags
                    .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))),
                PhysicalAddress = MapToGraphQl(src.PhysicalAddress),
                UniqueClaimCode = src.UniqueClaimCode,
                ContactedViaEmail = src.ContactedViaEmail,
                ContactedViaSms = src.ContactedViaSms,
                ContactedViaCall = src.ContactedViaCall,
                ContactedViaWhatsapp = src.ContactedViaWhatsapp,
                ProductIds = src.PrecomputedLocationProducts.Select(item => item.Product.Id)
            };

    public ResourceDetails MapTo(Resource src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden,
            AvailableHours = src.AvailableHours is null ? null : MapTo(src.AvailableHours),
            CustomTags = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)),
            Zones = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)),
            ProductTags = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Product)),
            ResourceType = MapTo(src.Tags.First(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type)))
        };

    public IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) => src.Select(MapTo)!;

    public FloorPlanDetails? MapTo(FloorPlan? src)
    {
        var resourcePositions = src is null ? [] : src.ResourcePositions.Select(MapToResourcePosition).ToList();

        return src is null
            ? null
            : new FloorPlanDetails
            {
                Id = src.Id,
                Name = src.Name,
                Image = src.Image,
                ResourcePositions = resourcePositions,
                ResourceCount = resourcePositions.Count
            };
    }

    public LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal,
        IEnumerable<LocationRoomsOccupancyPercentage> locationRoomsOccupancyPercentage,
        IEnumerable<ResourceAvailabilitySnapshotReport> resourceAvailabilitySnapshots) =>
        new()
        {
            Name = name,
            DesksOccupancyPercentage = locationDesksOccupancyPercentage
                .Select(item => new DesksOccupancyPercentage { Date = item.Date, Percentage = item.Percentage }),
            DailyBookingsTotals = locationDailyBookingsTotal
                .Select(item => new GraphQL.Location.LocationDailyBookingsTotal { Date = item.Date, Total = item.Total }),
            RoomsOccupancyPercentage = locationRoomsOccupancyPercentage
                .Select(item => new RoomsOccupancyPercentage { Date = item.Date, Percentage = item.Percentage }),
            ResourceAvailabilitySnapshots = resourceAvailabilitySnapshots
                .Select(item => new ResourceAvailabilityDailySnapshot
                {
                    Date = item.Date,
                    ResourceType = item.ResourceType,
                    AvailableCount = item.AvailableCount,
                    UnavailableCount = item.UnavailableCount,
                    BookedCount = item.BookedCount,
                    AvailableResourceNames = item.AvailableResourceNames,
                    UnavailableResourceNames = item.UnavailableResourceNames,
                    BookedResourceNames = item.BookedResourceNames
                })
        };

    public Shared.Models.Location MapTo(AddLocationInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Timezone = src.Timezone,
            Type = src.Type,
            ExtraMetadata = src.ExtraMetadata,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            Organization =
                new Organization { Id = src.OrganizationId.ToSafeString(), CustomDomain = src.OrganizationCustomDomain.ToSafeString() },
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            OpeningHours = src.WeekOpeningHours is null ? null : new OpeningHours(MapTo(src.WeekOpeningHours)!, [], [])
        };

    public Shared.Models.Location MapTo(UpdateLocationInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Timezone = src.Timezone,
            Type = src.Type,
            ExtraMetadata = src.ExtraMetadata,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public LocationEdge MapTo(Edge<Shared.Models.Location> src) => new(MapTo(src.Node)!, src.Cursor);

    public Resource MapTo(AddResourceInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            Tags = src.CustomTagIds
                .Concat(src.ZoneIds)
                .Concat(src.ProductTagIds)
                .Append(src.OrganizationResourceTypeId)
                .Select(item => new OrganizationTag { Id = item })
                .ToList(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Resource MapTo(UpdateResourceInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            Tags = src.CustomTagIds
                .Concat(src.ZoneIds)
                .Concat(src.ProductTagIds)
                .Append(src.OrganizationResourceTypeId)
                .Select(item => new OrganizationTag { Id = item })
                .ToList()
        };

    public ResourceEdge MapTo(Edge<Resource> src) => new(MapTo(src.Node), src.Cursor);

    public WeekOpeningHours? MapTo(GraphQL.Location.WeekOpeningHours? src) =>
        src is null
            ? null
            : new WeekOpeningHours(MapTo(src.Monday),
                MapTo(src.Tuesday),
                MapTo(src.Wednesday),
                MapTo(src.Thursday),
                MapTo(src.Friday),
                MapTo(src.Saturday),
                MapTo(src.Sunday));

    public FloorPlan MapTo(AddFloorPlanInput src)
    {
        var floorPlanId = src.Id.ToSafeString();

        return new FloorPlan
        {
            Id = floorPlanId,
            Name = src.Name,
            Image = src.Image,
            ResourcePositions = src.ResourcePositions.ToSafeCollection().Select(item => MapTo(item, floorPlanId)).ToList(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };
    }

    public FloorPlan MapTo(UpdateFloorPlanInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Image = src.Image,
            ResourcePositions = src.ResourcePositions.ToSafeCollection().Select(item => MapTo(item, src.Id)).ToList()
        };

    public FloorPlanEdge MapTo(Edge<FloorPlan> src) => new(MapTo(src.Node)!, src.Cursor);

    public IEnumerable<ResourcePosition> MapTo(UpdateResourcePositionsInput src) =>
        src.ResourcePositions.Select(item => MapTo(item, src.FloorPlanId));

    public LocationPhysicalAddress MapTo(AddLocationPhysicalAddressInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode,
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public LocationPhysicalAddress MapTo(UpdateLocationPhysicalAddressInput src) =>
        new()
        {
            Id = src.Id,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode
        };

    private static GraphQL.Location.OpeningHours MapTo(OpeningHours? src)
    {
        if (src is null)
        {
            return new GraphQL.Location.OpeningHours
            {
                WeekOpeningHours = new GraphQL.Location.WeekOpeningHours
                {
                    Monday = MapToDefault(),
                    Tuesday = MapToDefault(),
                    Wednesday = MapToDefault(),
                    Thursday = MapToDefault(),
                    Friday = MapToDefault(),
                    Saturday = MapToDefault(),
                    Sunday = MapToDefault()
                },
                ClosedDates = [],
                DatesWithVariedOpeningHours = []
            };
        }

        return new GraphQL.Location.OpeningHours
        {
            WeekOpeningHours = MapTo(src.WeekOpeningHours),
            ClosedDates = src.ClosedDates,
            DatesWithVariedOpeningHours = src.DatesWithVariedOpeningHours.Select(item => new VariedDateOpeningHours
            {
                Date = item.Key, OpeningHoursDetails = MapTo(item.Value)
            })
        };
    }

    private static GraphQL.Location.WeekOpeningHours MapTo(WeekOpeningHours src) =>
        new()
        {
            Monday = MapTo(src.Monday),
            Tuesday = MapTo(src.Tuesday),
            Wednesday = MapTo(src.Wednesday),
            Thursday = MapTo(src.Thursday),
            Friday = MapTo(src.Friday),
            Saturday = MapTo(src.Saturday),
            Sunday = MapTo(src.Sunday)
        };

    private static GraphQL.Location.OpeningHoursDetails MapTo(OpeningHoursDetails src) =>
        new()
        {
            Closed = src.Closed,
            OpenAllDay = src.OpenAllDay,
            From = src.From is null ? string.Empty : $"{src.From.Value.Hour}:{src.From.Value.Minute}",
            Until = src.Until is null ? string.Empty : $"{src.Until.Value.Hour}:{src.Until.Value.Minute}"
        };

    private static OpeningHoursDetails MapTo(GraphQL.Location.OpeningHoursDetails src) =>
        new(
            src.Closed,
            src.OpenAllDay,
            string.IsNullOrWhiteSpace(src.From) ? null : TimeOnly.Parse(src.From),
            string.IsNullOrWhiteSpace(src.Until) ? null : TimeOnly.Parse(src.Until));

    private static GraphQL.Location.OpeningHoursDetails MapToDefault() => new()
    {
        Closed = false, OpenAllDay = true, From = string.Empty, Until = string.Empty
    };

    private static ResourcePosition MapTo(ResourcePositionInput src, string floorPlanId) =>
        new() { X = src.X, Y = src.Y, Resource = new Resource { Id = src.ResourceId }, FloorPlan = new FloorPlan { Id = floorPlanId } };

    private static LocationPhysicalAddressDetails? MapToGraphQl(LocationPhysicalAddress? src) =>
        src is null
            ? null
            : new LocationPhysicalAddressDetails
            {
                Id = src.Id,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Longitude = src.Coordinates?.X,
                Latitude = src.Coordinates?.Y,
                FormattedAddress = src.ToFormattedAddress(),
                MultilinesFormattedAddress = src.ToMultilinesFormattedAddress(),
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode
            };

    private static LocationPhysicalAddress? MapTo(LocationPhysicalAddressInput? src) =>
        src is null
            ? null
            : new LocationPhysicalAddress
            {
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Coordinates =
                    src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode
            };

    private static ResourcePositionDetails MapToResourcePosition(ResourcePosition src) =>
        new() { Id = src.Id, X = src.X, Y = src.Y, Resource = new ResourceDetails { Id = src.Resource.Id, Name = src.Resource.Name } };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTagDetails MapTo(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color, Type = src.Type };
}
