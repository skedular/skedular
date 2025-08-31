using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using Location.Api.GraphQL.FloorPlan;
using Location.Api.GraphQL.Location;
using Location.Api.GraphQL.PhysicalAddress;
using Location.Api.GraphQL.Resource;
using NetTopologySuite.Geometries;
using Customer = Location.Shared.Models.Customer;
using DailyDeskCountRecording = Location.Shared.Models.DailyDeskCountRecording;
using Resource = Location.Shared.Database.Entities.Resource;
using LocationDesksOccupancyPercentage = Location.Shared.Models.LocationDesksOccupancyPercentage;
using Identity = Location.Shared.Models.Identity;
using LocationEdge = Location.Api.GraphQL.Location.LocationEdge;
using LocationDailyBookingsTotal = Location.Shared.Models.LocationDailyBookingsTotal;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationTag = Location.Shared.Models.OrganizationTag;
using Permissions = Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions;
using DailyRoomCountRecording = Location.Shared.Models.DailyRoomCountRecording;
using LocationRoomsOccupancyPercentage = Location.Shared.Models.LocationRoomsOccupancyPercentage;
using AddResourceInput = Location.Api.GraphQL.Resource.AddResourceInput;
using CdnFile = Api.Shared.Services.Grpc.Skedular.Location.V1.CdnFile;
using CdnImageFile = Api.Shared.Services.Grpc.Skedular.Location.V1.CdnImageFile;
using FloorPlan = Location.Shared.Models.FloorPlan;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using UpdateResourceInput = Location.Api.GraphQL.Resource.UpdateResourceInput;
using ResourceEdge = Location.Api.GraphQL.Resource.ResourceEdge;
using ResourcePosition = Location.Shared.Models.ResourcePosition;
using VariedDateOpeningHours = Api.Shared.Services.Grpc.Skedular.Location.V1.VariedDateOpeningHours;
using WeekOpeningHours = Api.Shared.Services.Models.WeekOpeningHours;
using LocationPhysicalAddress = Location.Shared.Database.Entities.LocationPhysicalAddress;
using LocationType = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType;

namespace Location.Api.Mappers;

public interface IMapper
{
    Shared.Models.Location MapTo(Shared.Database.Entities.Location src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);

    Shared.Database.Entities.Location MapTo(
        Shared.Models.Location src,
        Organization organization,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Database.Entities.Location MergeTo(
        Shared.Models.Location src,
        Shared.Database.Entities.Location dest,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Models.Resource MapTo(Resource src);

    Resource MapTo(
        Shared.Models.Resource src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Resource MergeTo(
        Shared.Models.Resource src,
        Resource dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Models.Resource MapTo(Resource src, Shared.Models.Location location);
    LocationDetails? MapTo(Shared.Models.Location? src);
    ResourceDetails MapTo(Shared.Models.Resource src);
    IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src);
    FloorPlanDetails? MapTo(FloorPlan? src);

    LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal,
        IEnumerable<LocationRoomsOccupancyPercentage> locationRoomsOccupancyPercentage);

    Shared.Models.Location MapTo(AddLocationInput src);
    Shared.Models.Location MapTo(UpdateLocationInput src);
    Shared.Models.Location MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src);
    Shared.Models.Location MapTo(AddInput src);
    Shared.Models.Location MapTo(UpdateInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource MapToGrpcResponse(Shared.Models.Resource src);
    LocationEdge MapTo(Edge<Shared.Models.Location> src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src);
    IEnumerable<Edge<Shared.Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Shared.Models.Location location);
    Shared.Models.Resource MapTo(AddResourceInput src);
    Shared.Models.Resource MapTo(UpdateResourceInput src);
    ResourceEdge MapTo(Edge<Shared.Models.Resource> src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.ResourceEdge MapToGrpcResponse(Edge<Shared.Models.Resource> src);
    Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddResourceInput src);
    Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateResourceInput src);
    WeekOpeningHours? MapTo(GraphQL.Location.WeekOpeningHours? src);

    Shared.Database.Entities.FloorPlan MapTo(
        FloorPlan src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.ResourcePosition>? resourcePositions);

    Shared.Database.Entities.FloorPlan MergeTo(
        FloorPlan src,
        Shared.Database.Entities.FloorPlan dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.ResourcePosition>? resourcePositions);

    FloorPlan MapTo(Shared.Database.Entities.FloorPlan src);
    FloorPlan MapTo(AddFloorPlanInput src);
    FloorPlan MapTo(UpdateFloorPlanInput src);
    FloorPlanEdge MapTo(Edge<FloorPlan> src);
    IEnumerable<ResourcePosition> MapTo(UpdateResourcePositionsInput src);
    Shared.Database.Entities.ResourcePosition MapToEntity(ResourcePosition src, Resource resource, Shared.Database.Entities.FloorPlan floorPlan);

    Shared.Database.Entities.ResourcePosition MergeToEntity(
        ResourcePosition src,
        Shared.Database.Entities.ResourcePosition dest,
        Resource resource,
        Shared.Database.Entities.FloorPlan floorPlan);

    LocationPhysicalAddress MapTo(Shared.Models.LocationPhysicalAddress src, Shared.Database.Entities.Location location);

    LocationPhysicalAddress MergeTo(
        Shared.Models.LocationPhysicalAddress src,
        LocationPhysicalAddress dest,
        Shared.Database.Entities.Location location);

    Shared.Models.LocationPhysicalAddress MapTo(LocationPhysicalAddress src);
    Shared.Models.LocationPhysicalAddress MapTo(AddLocationPhysicalAddressInput src);
    Shared.Models.LocationPhysicalAddress MapTo(UpdateLocationPhysicalAddressInput src);
    LocationPhysicalAddressDetails MapTo(Shared.Models.LocationPhysicalAddress src);
}

public class Mapper : IMapper
{
    public Shared.Models.Location MapTo(Shared.Database.Entities.Location src)
    {
        var location = new Shared.Models.Location
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type.ToLocationType(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            OpeningHours = src.OpeningHours,
            Organization = MapTo(src.Organization),
            Tags = MapTo(src.OrganizationTags).ToList()
        };

        location.DailyDeskCountRecordings = MapTo(src.DailyDeskCountRecordings, location).ToList();
        location.DailyRoomCountRecordings = MapTo(src.DailyRoomCountRecordings, location).ToList();
        location.Resources = MapTo(src.Resources, location).ToList();
        location.PhysicalAddress = MapTo(src.PhysicalAddress, location);
        location.FloorPlans = src.FloorPlans.Select(MapTo).ToList();

        return location;
    }

    public Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                Identities = MapTo(src.Identities).ToList()
            };

    public Shared.Database.Entities.Location MapTo(
        Shared.Models.Location src,
        Organization organization,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type.ToLocationType(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            OpeningHours = src.OpeningHours,
            Organization = organization,
            OrganizationTags = organizationTags
        };

    public Shared.Database.Entities.Location MergeTo(
        Shared.Models.Location src,
        Shared.Database.Entities.Location dest,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
        dest.Type = src.Type.ToLocationType();
        dest.ContactEmail = src.ContactEmail;
        dest.ContactPhone = src.ContactPhone;
        dest.PrimaryFeatureImage = src.PrimaryFeatureImage;
        dest.OpeningHours = src.OpeningHours;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public LocationDetails? MapTo(Shared.Models.Location? src) =>
        src is null
            ? null
            : new LocationDetails
            {
                Id = src.Id,
                Name = src.Name,
                About = src.About,
                Timezone = src.Timezone,
                Type = new LocationTypeDetails
                {
                    Type = src.Type,
                    Name = src.Type.ToLocationTypeName()
                },
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                PrimaryFeatureImage = src.PrimaryFeatureImage,
                OpeningHours = MapTo(src.OpeningHours),
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanViewAnalytics = src.Permissions.CanViewAnalytics,
                HasFutureBooking = src.HasFutureBooking,
                DeskCapacity = src.Resources.Count(item => item.Tags.Any(tag => tag.Type == OrganizationTagType.ResourceDesk)),
                RoomCapacity = src.Resources.Count(item => item.Tags.Any(tag => tag.Type == OrganizationTagType.ResourceRoom)),
                Organization = MapTo(src.Organization),
                Resources = MapTo(src.Resources),
                CustomTags = MapTo(src.CustomTags),
                Zones = MapTo(src.Zones),
                ResourceTypes = src.Organization.Tags
                    .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
                    .Select(MapTo),
                PhysicalAddress = MapToGraphQl(src.PhysicalAddress),
                LocationTags = MapTo(src.Tags)
            };

    public Shared.Models.Resource MapTo(Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden ?? false,
            AvailableHours = src.AvailableHours,
            Tags = MapTo(src.OrganizationTags).ToList(),
            ResourcePosition = MapTo(src.ResourcePosition)
        };

    public Resource MapTo(
        Shared.Models.Resource src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags) =>
        MergeTo(src, new Resource(), location, organizationTags);

    public Resource MergeTo(
        Shared.Models.Resource src,
        Resource dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Inactive = src.Inactive;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
        dest.Capacity = src.Capacity;
        dest.IsAvailableHoursOverridden = src.IsAvailableHoursOverridden;
        dest.AvailableHours = src.AvailableHours;
        dest.OrganizationTags = organizationTags;
        dest.Location = location;
        return dest;
    }

    public ResourceEdge MapTo(Edge<Shared.Models.Resource> src) => new(MapTo(src.Node), src.Cursor);

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.ResourceEdge MapToGrpcResponse(Edge<Shared.Models.Resource> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddResourceInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            Location = new Shared.Models.Location { Id = src.LocationId },
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateResourceInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

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

    public ResourceDetails MapTo(Shared.Models.Resource src) =>
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

    public IEnumerable<Edge<Shared.Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public LocationEdge MapTo(Edge<Shared.Models.Location> src) => new(MapTo(src.Node)!, src.Cursor);

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) => src.Select(MapTo)!;

    public LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal,
        IEnumerable<LocationRoomsOccupancyPercentage> locationRoomsOccupancyPercentage) =>
        new()
        {
            Name = name,
            DesksOccupancyPercentage = locationDesksOccupancyPercentage
                .Select(item => new DesksOccupancyPercentage { Date = item.Date, Percentage = item.Percentage }),
            DailyBookingsTotals = locationDailyBookingsTotal
                .Select(item => new GraphQL.Location.LocationDailyBookingsTotal { Date = item.Date, Total = item.Total }),
            RoomsOccupancyPercentage = locationRoomsOccupancyPercentage
                .Select(item => new RoomsOccupancyPercentage { Date = item.Date, Percentage = item.Percentage })
        };

    public Shared.Models.Location MapTo(AddLocationInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            Organization =
                new Shared.Models.Organization
                {
                    Id = src.OrganizationId.ToSafeString(), UniqueAlphanumericName = src.OrganizationUniqueAlphanumericName.ToSafeString()
                },
            Tags = src.LocationTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            PhysicalAddress = MapTo(src.PhysicalAddress),
        };

    public Shared.Models.Location MapTo(UpdateLocationInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = src.PrimaryFeatureImage,
            Tags = src.LocationTagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Resource MapTo(AddResourceInput src) =>
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
                .Concat([src.OrganizationResourceTypeId])
                .Select(item => new OrganizationTag { Id = item })
                .ToList(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Shared.Models.Resource MapTo(UpdateResourceInput src) =>
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
                .Concat([src.OrganizationResourceTypeId])
                .Select(item => new OrganizationTag { Id = item })
                .ToList()
        };

    public Shared.Models.Location MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace =>global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = MapTo(src.PrimaryFeatureImage),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Tags = src.LocationTagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src)
    {
        var location = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Models.LocationType.Private => LocationType.Private,
                global::Api.Shared.Services.Models.LocationType.Marketplace => LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            PrimaryFeatureImage = MapTo(src.PrimaryFeatureImage),
            OpeningHours = MapToGrpcResponse(src.OpeningHours),
            OrganizationId = src.Organization.Id,
            Permissions = new Permissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanViewAnalytics = src.Permissions.CanViewAnalytics
            },
            HasFutureBooking = src.HasFutureBooking
        };

        location.Resources.AddRange(MapToGrpcResponse(src.Resources));
        location.CustomTags.AddRange(MapToGrpcResponseOrganizationCustomTags(src.CustomTags));
        location.Zones.AddRange(MapToGrpcResponseOrganizationZones(src.Zones));
        location.LocationTags.AddRange(MapToGrpcResponseOrganizationLocationTags(src.Tags));

        return location;
    }

    public Shared.Models.Location MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = MapTo(src.PrimaryFeatureImage),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Tags = src.LocationTagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Location MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            PrimaryFeatureImage = MapTo(src.PrimaryFeatureImage),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Tags = src.LocationTagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Resource MapTo(Resource src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden ?? false,
            AvailableHours = src.AvailableHours,
            Location = location,
            Tags = MapTo(src.OrganizationTags, location.Organization).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource MapToGrpcResponse(Shared.Models.Resource src)
    {
        var resource = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden,
            AvailableHours = src.AvailableHours is null ? null : MapToGrpcResponse(src.AvailableHours),
            ResourceType = MapToGrpcResponse(src.Tags.First(item =>
                OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type)))
        };

        resource.OrganizationCustomTags.AddRange(
            MapToGrpcResponseOrganizationCustomTags(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)));
        resource.OrganizationZones.AddRange(MapToGrpcResponseOrganizationZones(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)));
        resource.OrganizationProductTags.AddRange(
            MapToGrpcResponseOrganizationProductTags(src.Tags.Where(item => item.Type == OrganizationTagType.Product)));

        return resource;
    }

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

    public Shared.Database.Entities.FloorPlan MapTo(
        FloorPlan src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.ResourcePosition>? resourcePositions) =>
        MergeTo(src, new Shared.Database.Entities.FloorPlan(), location, resourcePositions);

    public Shared.Database.Entities.FloorPlan MergeTo(
        FloorPlan src,
        Shared.Database.Entities.FloorPlan dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.ResourcePosition>? resourcePositions)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Image = src.Image;
        dest.Location = location;

        if (resourcePositions is not null)
        {
            dest.ResourcePositions = resourcePositions;
        }

        return dest;
    }

    public FloorPlan MapTo(Shared.Database.Entities.FloorPlan src)
    {
        var floorPlan = new FloorPlan
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Image = src.Image
        };

        floorPlan.ResourcePositions = MapTo(src.ResourcePositions, floorPlan).ToList();

        return floorPlan;
    }

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

    public Shared.Database.Entities.ResourcePosition MapToEntity(
        ResourcePosition src,
        Resource resource,
        Shared.Database.Entities.FloorPlan floorPlan) => MergeToEntity(src, new Shared.Database.Entities.ResourcePosition(), resource, floorPlan);

    public Shared.Database.Entities.ResourcePosition MergeToEntity(
        ResourcePosition src,
        Shared.Database.Entities.ResourcePosition dest,
        Resource resource,
        Shared.Database.Entities.FloorPlan floorPlan)
    {
        dest.Id = src.Id;
        dest.X = src.X;
        dest.Y = src.Y;
        dest.Resource = resource;
        dest.FloorPlan = floorPlan;
        return dest;
    }

    public LocationPhysicalAddress MapTo(Shared.Models.LocationPhysicalAddress src, Shared.Database.Entities.Location location) =>
        MergeTo(src, new LocationPhysicalAddress(), location);

    public LocationPhysicalAddress MergeTo(
        Shared.Models.LocationPhysicalAddress src,
        LocationPhysicalAddress dest,
        Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.OsmType = src.OsmType;
        dest.OsmId = src.OsmId;
        dest.PlaceId = src.PlaceId;
        dest.Coordinates = src.Coordinates;
        dest.FormattedAddress = src.FormattedAddress;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.CountryCode = src.CountryCode;
        dest.Location = location;
        return dest;
    }

    public Shared.Models.LocationPhysicalAddress MapTo(LocationPhysicalAddress src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Coordinates,
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode,
            Location = MapTo(src.Location)
        };

    public Shared.Models.LocationPhysicalAddress MapTo(AddLocationPhysicalAddressInput src) =>
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

    public Shared.Models.LocationPhysicalAddress MapTo(UpdateLocationPhysicalAddressInput src) =>
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

    public LocationPhysicalAddressDetails MapTo(Shared.Models.LocationPhysicalAddress src) =>
        new()
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
            CountryCode = src.CountryCode,
            Location = MapTo(src.Location)!
        };

    private static OrganizationTagDetails MapTo(OrganizationTag src) =>
        new() { UniqueId = src.Id, Name = src.Name, TagType = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color
        };

    private static OrganizationCustomTag MapToGrpcResponseOrganizationCustomTag(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static OrganizationZone MapToGrpcResponseOrganizationZone(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static OrganizationProductTag MapToGrpcResponseOrganizationProductTag(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static OrganizationLocationTag MapToGrpcResponseOrganizationLocationTag(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<OrganizationTag> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationTag> src,
        Shared.Models.Organization? organization) =>
        src.Select(item => MapTo(item, organization));

    private IEnumerable<Shared.Models.Resource> MapTo(IEnumerable<Resource> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static IEnumerable<OrganizationCustomTag> MapToGrpcResponseOrganizationCustomTags(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationCustomTag);

    private static IEnumerable<OrganizationZone> MapToGrpcResponseOrganizationZones(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationZone);

    private static IEnumerable<OrganizationProductTag> MapToGrpcResponseOrganizationProductTags(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationProductTag);

    private static IEnumerable<OrganizationLocationTag> MapToGrpcResponseOrganizationLocationTags(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationLocationTag);

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource> MapToGrpcResponse(IEnumerable<Shared.Models.Resource> src) =>
        src.Select(MapToGrpcResponse);

    private static OrganizationDetails MapTo(Shared.Models.Organization src) =>
        new() { UniqueId = src.Id, UniqueAlphanumericName = src.UniqueAlphanumericName, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private IEnumerable<ResourceDetails> MapTo(IEnumerable<Shared.Models.Resource> src) => src.Select(MapTo);

    private static ResourcePositionDetails MapToResourcePosition(ResourcePosition src) =>
        new() { Id = src.Id, X = src.X, Y = src.Y, Resource = new ResourceDetails { Id = src.Resource.Id, Name = src.Resource.Name } };

    private static Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            LogoUrl = src.LogoUrl,
            Offering = src.Offering,
            Type = src.Type.ToOrganizationType(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            Tags = MapTo(src.Tags).ToList()
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };

    private static IEnumerable<DailyDeskCountRecording> MapTo(
        IEnumerable<Shared.Database.Entities.DailyDeskCountRecording> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static DailyDeskCountRecording MapTo(Shared.Database.Entities.DailyDeskCountRecording src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Location = location,
            Date = src.Date,
            Count = src.Count
        };

    private static IEnumerable<DailyRoomCountRecording> MapTo(
        IEnumerable<Shared.Database.Entities.DailyRoomCountRecording> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static DailyRoomCountRecording MapTo(Shared.Database.Entities.DailyRoomCountRecording src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Location = location,
            Date = src.Date,
            Count = src.Count
        };

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src, Shared.Models.Organization? organization)
    {
        var organizationTag = new OrganizationTag
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color
        };

        if (organization is not null)
        {
            organizationTag.Organization = organization;
        }

        return organizationTag;
    }

    private Edge<Shared.Models.Resource> MapTo(Edge<Resource> src, Shared.Models.Location location)
    {
        var resource = MapTo(src.Node);
        resource.Location = location;
        return new Edge<Shared.Models.Resource>(resource, src.Cursor);
    }

    private static global::Api.Shared.Services.Grpc.Skedular.Location.V1.OpeningHours MapToGrpcResponse(OpeningHours? src)
    {
        if (src is null)
        {
            return new global::Api.Shared.Services.Grpc.Skedular.Location.V1.OpeningHours
            {
                WeekOpeningHours = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.WeekOpeningHours
                {
                    Monday = MapToGrpcDefault(),
                    Tuesday = MapToGrpcDefault(),
                    Wednesday = MapToGrpcDefault(),
                    Thursday = MapToGrpcDefault(),
                    Friday = MapToGrpcDefault(),
                    Saturday = MapToGrpcDefault(),
                    Sunday = MapToGrpcDefault()
                }
            };
        }

        var openingHours =
            new global::Api.Shared.Services.Grpc.Skedular.Location.V1.OpeningHours { WeekOpeningHours = MapToGrpcResponse(src.WeekOpeningHours) };
        openingHours.ClosedDates.AddRange(src.ClosedDates.Select(item => item.ToTimestamp()));
        openingHours.DatesWithVariedOpeningHours.AddRange(src.DatesWithVariedOpeningHours.Select(item => new VariedDateOpeningHours
        {
            Date = item.Key.ToTimestamp(), OpeningHoursDetails = MapToGrpcResponse(item.Value)
        }));

        return openingHours;
    }

    private static global::Api.Shared.Services.Grpc.Skedular.Location.V1.WeekOpeningHours MapToGrpcResponse(WeekOpeningHours src) =>
        new()
        {
            Monday = MapToGrpcResponse(src.Monday),
            Tuesday = MapToGrpcResponse(src.Tuesday),
            Wednesday = MapToGrpcResponse(src.Wednesday),
            Thursday = MapToGrpcResponse(src.Thursday),
            Friday = MapToGrpcResponse(src.Friday),
            Saturday = MapToGrpcResponse(src.Saturday),
            Sunday = MapToGrpcResponse(src.Sunday)
        };

    private static global::Api.Shared.Services.Grpc.Skedular.Location.V1.OpeningHoursDetails MapToGrpcResponse(OpeningHoursDetails src) =>
        new()
        {
            Closed = src.Closed,
            OpenAllDay = src.OpenAllDay,
            From = src.From is null ? string.Empty : $"{src.From.Value.Hour}:{src.From.Value.Minute}",
            Until = src.Until is null ? string.Empty : $"{src.Until.Value.Hour}:{src.Until.Value.Minute}"
        };

    private static global::Api.Shared.Services.Grpc.Skedular.Location.V1.OpeningHoursDetails MapToGrpcDefault() =>
        new() { Closed = false, OpenAllDay = true, From = string.Empty, Until = string.Empty };

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
            DatesWithVariedOpeningHours = src.DatesWithVariedOpeningHours.Select(item => new GraphQL.Location.VariedDateOpeningHours
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

    private static ResourceType MapToGrpcResponse(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString(), TagType = src.Type.ToNullableOrganizationTagType() };

    private static global::Api.Shared.Services.Models.CdnImageFile? MapTo(CdnImageFile? src) =>
        src is null ? null : new global::Api.Shared.Services.Models.CdnImageFile(MapTo(src.Original), MapTo(src.Thumbnail));

    private static global::Api.Shared.Services.Models.CdnFile? MapTo(CdnFile? src) =>
        src is null ? null : new global::Api.Shared.Services.Models.CdnFile(src.Url, src.Height.FromNullInt(), src.Width.FromNullInt());

    private static CdnImageFile? MapTo(global::Api.Shared.Services.Models.CdnImageFile? src) =>
        src is null ? null : new CdnImageFile { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(global::Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };

    private static ResourcePosition MapTo(ResourcePositionInput src, string floorPlanId) =>
        new() { X = src.X, Y = src.Y, Resource = new Shared.Models.Resource { Id = src.ResourceId }, FloorPlan = new FloorPlan { Id = floorPlanId } };

    private static IEnumerable<ResourcePosition> MapTo(IEnumerable<Shared.Database.Entities.ResourcePosition> src, FloorPlan floorPlan) =>
        src.Select(item => MapTo(item, floorPlan))!;

    private ResourcePosition? MapTo(Shared.Database.Entities.ResourcePosition? src) =>
        src is null
            ? null
            : new ResourcePosition
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                X = src.X,
                Y = src.Y,
                FloorPlan = MapTo(src.FloorPlan)
            };

    private static ResourcePosition? MapTo(Shared.Database.Entities.ResourcePosition? src, FloorPlan floorPlan) =>
        src is null
            ? null
            : new ResourcePosition
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                X = src.X,
                Y = src.Y,
                FloorPlan = floorPlan,
                Resource = new Shared.Models.Resource
                {
                    Id = src.Resource.Id,
                    CreatedAt = src.CreatedAt,
                    DeletedAt = src.Resource.DeletedAt,
                    ModifiedAt = src.ModifiedAt,
                    Name = src.Resource.Name,
                    Inactive = src.Resource.Inactive,
                    RequireBookingApproval = src.Resource.RequireBookingApproval,
                    Color = src.Resource.Color,
                    Capacity = src.Resource.Capacity,
                    IsAvailableHoursOverridden = src.Resource.IsAvailableHoursOverridden ?? false,
                    AvailableHours = src.Resource.AvailableHours
                }
            };

    private static LocationPhysicalAddressDetails? MapToGraphQl(Shared.Models.LocationPhysicalAddress? src) =>
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

    private static Shared.Models.LocationPhysicalAddress? MapTo(LocationPhysicalAddress? src, Shared.Models.Location location) =>
        src is null
            ? null
            : new Shared.Models.LocationPhysicalAddress
            {
                Id = src.Id,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Coordinates = src.Coordinates,
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode,
                Location = location
            };

    private static Shared.Models.LocationPhysicalAddress? MapTo(LocationPhysicalAddressInput? src) =>
        src is null
            ? null
            : new()
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
                CountryCode = src.CountryCode,
            };
}
