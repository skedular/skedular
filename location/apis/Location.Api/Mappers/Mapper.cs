using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Google.Protobuf.WellKnownTypes;
using Location.Api.GraphQL;
using Location.Shared.Database.Entities;
using Booking = Location.Shared.Models.Booking;
using Customer = Location.Shared.Models.Customer;
using DailyDeskCountRecording = Location.Shared.Models.DailyDeskCountRecording;
using Resource = Location.Shared.Database.Entities.Resource;
using LocationDesksOccupancyPercentage = Location.Shared.Models.LocationDesksOccupancyPercentage;
using Identity = Location.Shared.Models.Identity;
using JoinInvitation = Location.Shared.Models.JoinInvitation;
using LocationEdge = Location.Api.GraphQL.LocationEdge;
using LocationDailyBookingsTotal = Location.Shared.Models.LocationDailyBookingsTotal;
using LocationMember = Location.Shared.Models.LocationMember;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationTag = Location.Shared.Models.OrganizationTag;
using Permissions = Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions;
using DailyRoomCountRecording = Location.Shared.Models.DailyRoomCountRecording;
using LocationRoomsOccupancyPercentage = Location.Shared.Models.LocationRoomsOccupancyPercentage;
using AddResourceInput = Location.Api.GraphQL.AddResourceInput;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using UpdateResourceInput = Location.Api.GraphQL.UpdateResourceInput;
using ResourceEdge = Location.Api.GraphQL.ResourceEdge;
using VariedDateOpeningHours = Api.Shared.Services.Grpc.Skedular.Location.V1.VariedDateOpeningHours;
using WeekOpeningHours = Api.Shared.Services.Models.WeekOpeningHours;

namespace Location.Api.Mappers;

public interface IMapper
{
    Shared.Models.Location MapTo(Shared.Database.Entities.Location src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Shared.Database.Entities.Location MapTo(Shared.Models.Location src, Organization organization);
    Shared.Database.Entities.Location MergeTo(Shared.Models.Location src, Shared.Database.Entities.Location dest, Address? physicalAddress);
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
    LocationMember MapTo(Shared.Database.Entities.LocationMember src, Shared.Models.Location location);
    LocationMemberDetails MapTo(LocationMember src);
    LocationDetails? MapTo(Shared.Models.Location? src);
    ResourceDetails MapTo(Shared.Models.Resource src);
    IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src);

    LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal,
        IEnumerable<LocationRoomsOccupancyPercentage> locationRoomsOccupancyPercentage);

    Shared.Models.Location MapTo(AddLocationInput src);
    Shared.Models.Location MapTo(UpdateLocationInput src);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);
    Shared.Models.Location MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src);
    Shared.Models.Location MapTo(AddInput src);
    Shared.Models.Location MapTo(UpdateInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource MapToGrpcResponse(Shared.Models.Resource src);
    LocationEdge MapTo(Edge<Shared.Models.Location> src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src);
    LocationMemberEdge MapTo(Edge<LocationMember> src);
    IEnumerable<Edge<LocationMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src, Shared.Models.Location location);
    Address MapTo(Shared.Models.Address src, Shared.Database.Entities.Location location);
    Address MergeToEntity(Shared.Models.Address src, Address dest, Shared.Database.Entities.Location location);
    IEnumerable<Edge<Shared.Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Shared.Models.Location location);
    Shared.Models.Resource MapTo(AddResourceInput src);
    Shared.Models.Resource MapTo(UpdateResourceInput src);
    ResourceEdge MapTo(Edge<Shared.Models.Resource> src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.ResourceEdge MapToGrpcResponse(Edge<Shared.Models.Resource> src);
    Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddResourceInput src);
    Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateResourceInput src);
    public WeekOpeningHours? MapTo(GraphQL.WeekOpeningHours? src);
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
            OpeningHours = src.OpeningHours,
            Organization = MapTo(src.Organization)
        };

        location.LocationMembers = MapTo(src.LocationMembers, location).ToList();
        location.Bookings = MapTo(src.Bookings, location).ToList();
        location.DailyDeskCountRecordings = MapTo(src.DailyDeskCountRecordings, location).ToList();
        location.DailyRoomCountRecordings = MapTo(src.DailyRoomCountRecordings, location).ToList();
        location.JoinInvitations = MapTo(src.JoinInvitations, location).ToList();
        location.Resources = MapTo(src.Resources, location).ToList();
        location.PhysicalAddress = MapTo(src.PhysicalAddress, location);

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

    public Shared.Database.Entities.Location MapTo(Shared.Models.Location src, Organization organization) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            OpeningHours = src.OpeningHours,
            Organization = organization
        };

    public Shared.Database.Entities.Location MergeTo(Shared.Models.Location src, Shared.Database.Entities.Location dest, Address? physicalAddress)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
        dest.OpeningHours = src.OpeningHours;
        dest.PhysicalAddress = physicalAddress;
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
                OpeningHours = MapTo(src.OpeningHours),
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
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
                PhysicalAddress = MapToGraphQl(src.PhysicalAddress)
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
            Tags = MapTo(src.OrganizationTags).ToList()
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

    public LocationMember
        MapTo(Shared.Database.Entities.LocationMember src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToLocationMemberRole(),
            Customer = MapTo(src.Customer)!,
            Location = location
        };

    public LocationMemberDetails MapTo(LocationMember src) =>
        new() { Id = src.Id, Role = src.Role, Customer = MapTo(src.Customer) };

    public ResourceEdge MapTo(Edge<Shared.Models.Resource> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

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

    public WeekOpeningHours? MapTo(GraphQL.WeekOpeningHours? src) =>
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
            ResourceType = MapTo(src.Tags.First(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type)))
        };

    public IEnumerable<Edge<Shared.Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public LocationEdge MapTo(Edge<Shared.Models.Location> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node)! };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public LocationMemberEdge MapTo(Edge<LocationMember> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public IEnumerable<Edge<LocationMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public Address MapTo(Shared.Models.Address src, Shared.Database.Entities.Location location) => MergeToEntity(src, new Address(), location);

    public Address MergeToEntity(Shared.Models.Address src, Address dest, Shared.Database.Entities.Location location)
    {
        dest.FormattedAddress = src.FormattedAddress;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.Location = location;
        return dest;
    }

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
                .Select(item => new GraphQL.LocationDailyBookingsTotal { Date = item.Date, Total = item.Total }),
            RoomsOccupancyPercentage = locationRoomsOccupancyPercentage
                .Select(item => new RoomsOccupancyPercentage { Date = item.Date, Percentage = item.Percentage })
        };

    public Shared.Models.Location MapTo(AddLocationInput src)
    {
        var location = new Shared.Models.Location
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

        location.PhysicalAddress = MapTo(src.PhysicalAddress, location);

        return location;
    }

    public Shared.Models.Location MapTo(UpdateLocationInput src)
    {
        var location = new Shared.Models.Location { Id = src.Id.ToSafeString(), Name = src.Name, About = src.About, Timezone = src.Timezone };

        location.PhysicalAddress = MapTo(src.PhysicalAddress, location);

        return location;
    }

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
                .Concat([src.OrganizationResourceTypeId])
                .Select(item => new OrganizationTag { Id = item })
                .ToList()
        };

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Role = src.Role.ToLocationMemberRole(),
            Location = MapTo(src.Location),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public Shared.Models.Location MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src)
    {
        var location = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OpeningHours = MapToGrpcResponse(src.OpeningHours),
            OrganizationId = src.Organization.Id,
            Permissions = new Permissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanCancelPeopleExistingInvitations = src.Permissions.CanCancelPeopleExistingInvitations,
                CanViewAnalytics = src.Permissions.CanViewAnalytics
            },
            HasFutureBooking = src.HasFutureBooking
        };

        location.Resources.AddRange(MapToGrpcResponse(src.Resources));
        location.CustomTags.AddRange(MapToGrpcResponseOrganizationCustomTags(src.CustomTags));
        location.Zones.AddRange(MapToGrpcResponseOrganizationZones(src.Zones));

        return location;
    }

    public Shared.Models.Location MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.Location MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
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

        return resource;
    }

    private IEnumerable<LocationMember> MapTo(IEnumerable<Shared.Database.Entities.LocationMember> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

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

    private static Shared.Models.Address? MapTo(LocationAddressDetails? src, Shared.Models.Location location) =>
        src is null
            ? null
            : new Shared.Models.Address
            {
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                Location = location
            };

    private static OrganizationCustomTag MapToGrpcResponseOrganizationCustomTag(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static OrganizationZone MapToGrpcResponseOrganizationZone(OrganizationTag src) =>
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

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource> MapToGrpcResponse(IEnumerable<Shared.Models.Resource> src) =>
        src.Select(MapToGrpcResponse);

    private static OrganizationDetails MapTo(Shared.Models.Organization src) =>
        new() { UniqueId = src.Id, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private IEnumerable<ResourceDetails> MapTo(IEnumerable<Shared.Models.Resource> src) => src.Select(MapTo);

    private static LocationCustomerDetails MapTo(Customer src) =>
        new()
        {
            UniqueId = src.Id,
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
            PhotoUrl512 = src.PhotoUrl512
        };

    private static Shared.Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            LogoUrl = src.LogoUrl,
            Offering = src.Offering,
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

    private static IEnumerable<Booking> MapTo(
        IEnumerable<Shared.Database.Entities.Booking> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Booking MapTo(Shared.Database.Entities.Booking src,
        Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            From = src.From,
            Until = src.Until,
            Location = location
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

    private IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Location = location,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
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

    private Edge<LocationMember> MapTo(Edge<Shared.Database.Entities.LocationMember> src, Shared.Models.Location location) =>
        new(src.Cursor, MapTo(src.Node, location));

    private Edge<Shared.Models.Resource> MapTo(Edge<Resource> src, Shared.Models.Location location)
    {
        var resource = MapTo(src.Node);
        resource.Location = location;
        return new Edge<Shared.Models.Resource>(src.Cursor, resource);
    }

    private static LocationAddressDetails? MapToGraphQl(Shared.Models.Address? src) =>
        src is null
            ? null
            : new LocationAddressDetails
            {
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };

    private static Shared.Models.Address? MapTo(Address? src, Shared.Models.Location location) =>
        src is null
            ? null
            : new Shared.Models.Address
            {
                Id = src.Id,
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                Location = location
            };

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

    private static GraphQL.OpeningHours MapTo(OpeningHours? src)
    {
        if (src is null)
        {
            return new GraphQL.OpeningHours
            {
                WeekOpeningHours = new GraphQL.WeekOpeningHours
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

        return new GraphQL.OpeningHours
        {
            WeekOpeningHours = MapTo(src.WeekOpeningHours),
            ClosedDates = src.ClosedDates,
            DatesWithVariedOpeningHours = src.DatesWithVariedOpeningHours.Select(item => new GraphQL.VariedDateOpeningHours
            {
                Date = item.Key, OpeningHoursDetails = MapTo(item.Value)
            })
        };
    }

    private static GraphQL.WeekOpeningHours MapTo(WeekOpeningHours src) =>
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

    private static GraphQL.OpeningHoursDetails MapTo(OpeningHoursDetails src) =>
        new()
        {
            Closed = src.Closed,
            OpenAllDay = src.OpenAllDay,
            From = src.From is null ? string.Empty : $"{src.From.Value.Hour}:{src.From.Value.Minute}",
            Until = src.Until is null ? string.Empty : $"{src.Until.Value.Hour}:{src.Until.Value.Minute}"
        };

    private static OpeningHoursDetails MapTo(GraphQL.OpeningHoursDetails src) =>
        new(
            src.Closed,
            src.OpenAllDay,
            string.IsNullOrWhiteSpace(src.From) ? null : TimeOnly.Parse(src.From),
            string.IsNullOrWhiteSpace(src.Until) ? null : TimeOnly.Parse(src.Until));

    private static GraphQL.OpeningHoursDetails MapToDefault() => new()
    {
        Closed = false, OpenAllDay = true, From = string.Empty, Until = string.Empty
    };

    private static ResourceType MapToGrpcResponse(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString(), TagType = src.Type.ToNullableOrganizationTagType() };
}
