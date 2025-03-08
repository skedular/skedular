using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Location.Api.GraphQL;
using Location.Shared.Database.Entities;
using Booking = Location.Shared.Models.Booking;
using Customer = Location.Shared.Models.Customer;
using AddDeskInput = Location.Api.GraphQL.AddDeskInput;
using DailyDeskCountRecording = Location.Shared.Models.DailyDeskCountRecording;
using Resource = Location.Shared.Database.Entities.Resource;
using Desk = Location.Shared.Database.Entities.Desk;
using DeskEdge = Location.Api.GraphQL.DeskEdge;
using LocationDesksOccupancyPercentage = Location.Shared.Models.LocationDesksOccupancyPercentage;
using UpdateDeskInput = Location.Api.GraphQL.UpdateDeskInput;
using Identity = Location.Shared.Models.Identity;
using JoinInvitation = Location.Shared.Models.JoinInvitation;
using LocationEdge = Location.Api.GraphQL.LocationEdge;
using LocationDailyBookingsTotal = Location.Shared.Models.LocationDailyBookingsTotal;
using LocationMember = Location.Shared.Models.LocationMember;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationTag = Location.Shared.Models.OrganizationTag;
using Permissions = Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions;
using AddRoomInput = Location.Api.GraphQL.AddRoomInput;
using DailyRoomCountRecording = Location.Shared.Models.DailyRoomCountRecording;
using Room = Location.Shared.Database.Entities.Room;
using RoomEdge = Location.Api.GraphQL.RoomEdge;
using LocationRoomsOccupancyPercentage = Location.Shared.Models.LocationRoomsOccupancyPercentage;
using UpdateRoomInput = Location.Api.GraphQL.UpdateRoomInput;
using AddResourceInput = Location.Api.GraphQL.AddResourceInput;
using UpdateResourceInput = Location.Api.GraphQL.UpdateResourceInput;
using ResourceEdge = Location.Api.GraphQL.ResourceEdge;

namespace Location.Api.Mappers;

public interface IMapper
{
    Shared.Models.Location MapTo(Shared.Database.Entities.Location src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Shared.Database.Entities.Location MapTo(Shared.Models.Location src, Organization? organization);
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

    Shared.Models.Desk MapTo(Desk src);

    Desk MapTo(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Desk MergeTo(
        Shared.Models.Desk src,
        Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Models.Room MapTo(Room src);

    Room MapTo(
        Shared.Models.Room src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Room MergeTo(
        Shared.Models.Room src,
        Room dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Shared.Models.Resource MapTo(Resource src, Shared.Models.Location location);
    Shared.Models.Desk MapTo(Desk src, Shared.Models.Location location);
    Shared.Models.Room MapTo(Room src, Shared.Models.Location location);
    LocationMember MapTo(Shared.Database.Entities.LocationMember src, Shared.Models.Location location);
    LocationMemberDetails MapTo(LocationMember src);
    LocationDetails? MapTo(Shared.Models.Location? src);
    DeskDetails MapTo(Shared.Models.Desk src);
    RoomDetails MapTo(Shared.Models.Room src);
    ResourceDetails MapTo(Shared.Models.Resource src);
    IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src);

    LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal,
        IEnumerable<LocationRoomsOccupancyPercentage> locationRoomsOccupancyPercentage);

    Shared.Models.Location MapTo(AddLocationInput src);
    Shared.Models.Location MapTo(UpdateLocationInput src);
    Shared.Models.Desk MapTo(AddDeskInput src);
    Shared.Models.Desk MapTo(UpdateDeskInput src);
    Shared.Models.Room MapTo(AddRoomInput src);
    Shared.Models.Room MapTo(UpdateRoomInput src);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);
    Shared.Models.Location MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src);
    Shared.Models.Location MapTo(AddInput src);
    Shared.Models.Location MapTo(UpdateInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource MapToGrpcResponse(Shared.Models.Resource src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk MapToGrpcResponse(Shared.Models.Desk src);
    Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddDeskInput src);
    Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateDeskInput src);
    DeskEdge MapTo(Edge<Shared.Models.Desk> src);
    IEnumerable<Edge<Shared.Models.Desk>> MapTo(IEnumerable<Edge<Desk>> src, Shared.Models.Location location);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.DeskEdge MapToGrpcResponse(Edge<Shared.Models.Desk> src);
    LocationEdge MapTo(Edge<Shared.Models.Location> src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src);
    LocationMemberEdge MapTo(Edge<LocationMember> src);
    IEnumerable<Edge<LocationMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src, Shared.Models.Location location);
    Address MapTo(Shared.Models.Address src, Shared.Database.Entities.Location location);
    Address MergeToEntity(Shared.Models.Address src, Address dest, Shared.Database.Entities.Location location);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Room MapToGrpcResponse(Shared.Models.Room src);
    Shared.Models.Room MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddRoomInput src);
    Shared.Models.Room MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateRoomInput src);
    RoomEdge MapTo(Edge<Shared.Models.Room> src);
    IEnumerable<Edge<Shared.Models.Room>> MapTo(IEnumerable<Edge<Room>> src, Shared.Models.Location location);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.RoomEdge MapToGrpcResponse(Edge<Shared.Models.Room> src);
    IEnumerable<Edge<Shared.Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Shared.Models.Location location);
    Shared.Models.Resource MapTo(AddResourceInput src);
    Shared.Models.Resource MapTo(UpdateResourceInput src);
    ResourceEdge MapTo(Edge<Shared.Models.Resource> src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.ResourceEdge MapToGrpcResponse(Edge<Shared.Models.Resource> src);
    Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddResourceInput src);
    Shared.Models.Resource MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateResourceInput src);
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
            Organization = MapTo(src.Organization)
        };

        location.LocationMembers = MapTo(src.LocationMembers, location).ToList();
        location.Bookings = MapTo(src.Bookings, location).ToList();
        location.DailyDeskCountRecordings = MapTo(src.DailyDeskCountRecordings, location).ToList();
        location.DailyRoomCountRecordings = MapTo(src.DailyRoomCountRecordings, location).ToList();
        location.JoinInvitations = MapTo(src.JoinInvitations, location).ToList();
        location.Resources = MapTo(src.Resources, location).ToList();
        location.Desks = MapTo(src.Desks, location).ToList();
        location.Rooms = MapTo(src.Rooms, location).ToList();
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

    public Shared.Database.Entities.Location MapTo(Shared.Models.Location src, Organization? organization) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = organization
        };

    public Shared.Database.Entities.Location MergeTo(
        Shared.Models.Location src,
        Shared.Database.Entities.Location dest,
        Address? physicalAddress)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
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
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanViewAnalytics = src.Permissions.CanViewAnalytics,
                HasFutureBooking = src.HasFutureBooking,
                DeskCapacity = src.Desks.Count,
                RoomCapacity = src.Rooms.Count,
                Organization = MapTo(src.Organization),
                Desks = MapTo(src.Desks).ToArray(),
                Rooms = MapTo(src.Rooms).ToArray(),
                CustomTags = MapTo(src.CustomTags).ToArray(),
                Zones = MapTo(src.Zones).ToArray(),
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
        dest.OrganizationTags = organizationTags;
        dest.Location = location;
        return dest;
    }

    public Shared.Models.Desk MapTo(Desk src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Tags = MapTo(src.OrganizationTags).ToList()
        };

    public Desk MapTo(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags) =>
        MergeTo(src, new Desk(), location, organizationTags);

    public Desk MergeTo(
        Shared.Models.Desk src,
        Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Deactivated = src.Deactivated;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
        dest.OrganizationTags = organizationTags;
        dest.Location = location;
        return dest;
    }

    public Shared.Models.Room MapTo(Room src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Tags = MapTo(src.OrganizationTags).ToList()
        };

    public Room MapTo(
        Shared.Models.Room src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags) =>
        MergeTo(src, new Room(), location, organizationTags);

    public Room MergeTo(
        Shared.Models.Room src,
        Room dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Deactivated = src.Deactivated;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
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
            Role = src.Role switch
            {
                LocationRoleConstants.Owner => LocationMemberRole.Owner,
                LocationRoleConstants.Administrator => LocationMemberRole.Administrator,
                LocationRoleConstants.Member => LocationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)!,
            Location = location
        };

    public LocationMemberDetails MapTo(LocationMember src) =>
        new() { Id = src.Id, Role = src.Role, Customer = MapTo(src.Customer) };

    public DeskEdge MapTo(Edge<Shared.Models.Desk> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public DeskDetails MapTo(Shared.Models.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            CustomTags = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)).ToArray(),
            Zones = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)).ToArray()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.DeskEdge MapToGrpcResponse(Edge<Shared.Models.Desk> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public RoomEdge MapTo(Edge<Shared.Models.Room> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public RoomDetails MapTo(Shared.Models.Room src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            CustomTags = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)).ToArray(),
            Zones = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)).ToArray()
        };

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
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public ResourceDetails MapTo(Shared.Models.Resource src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            CustomTags = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)).ToArray(),
            Zones = MapTo(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)).ToArray()
        };

    public IEnumerable<Edge<Shared.Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.RoomEdge MapToGrpcResponse(Edge<Shared.Models.Room> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

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
                .Select(item => new DesksOccupancyPercentage { Date = item.Date, Percentage = item.Percentage })
                .ToArray(),
            DailyBookingsTotals = locationDailyBookingsTotal
                .Select(item => new GraphQL.LocationDailyBookingsTotal { Date = item.Date, Total = item.Total })
                .ToArray(),
            RoomsOccupancyPercentage = locationRoomsOccupancyPercentage
                .Select(item => new RoomsOccupancyPercentage { Date = item.Date, Percentage = item.Percentage })
                .ToArray()
        };

    public Shared.Models.Location MapTo(AddLocationInput src)
    {
        var location = new Shared.Models.Location
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId }
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

    public Shared.Models.Desk MapTo(AddDeskInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Deactivated = false,
            RequireBookingApproval = false,
            Color = src.Color,
            Tags = src.CustomTagIds.Concat(src.ZoneIds).Select(item => new OrganizationTag { Id = item }).ToList(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Shared.Models.Desk MapTo(UpdateDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Tags = src.CustomTagIds.Concat(src.ZoneIds).Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Room MapTo(AddRoomInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Deactivated = false,
            RequireBookingApproval = false,
            Color = src.Color,
            Tags = src.CustomTagIds.Concat(src.ZoneIds).Select(item => new OrganizationTag { Id = item }).ToList(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Shared.Models.Room MapTo(UpdateRoomInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Tags = src.CustomTagIds.Concat(src.ZoneIds).Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Resource MapTo(AddResourceInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Inactive = false,
            RequireBookingApproval = false,
            Color = src.Color,
            Tags = src.CustomTagIds.Concat(src.ZoneIds).Select(item => new OrganizationTag { Id = item }).ToList(),
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
            Tags = src.CustomTagIds.Concat(src.ZoneIds).Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status switch
            {
                InvitationStatusConstants.Pending => InvitationStatus.Pending,
                InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
                InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
                InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            },
            Role = src.Role switch
            {
                LocationRoleConstants.Owner => LocationMemberRole.Owner,
                LocationRoleConstants.Administrator => LocationMemberRole.Administrator,
                LocationRoleConstants.Member => LocationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
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
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src)
    {
        var location = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            OrganizationId = string.IsNullOrWhiteSpace(src.Organization?.Id) ? string.Empty : src.Organization.Id,
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
        location.Desks.AddRange(MapToGrpcResponse(src.Desks));
        location.Rooms.AddRange(MapToGrpcResponse(src.Rooms));
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
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.Location MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.Desk MapTo(Desk src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Location = location,
            Tags = MapTo(src.OrganizationTags, location.Organization).ToList()
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
            Color = src.Color.ToSafeString()
        };

        resource.OrganizationCustomTags.AddRange(
            MapToGrpcResponseOrganizationCustomTags(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)));
        resource.OrganizationZones.AddRange(MapToGrpcResponseOrganizationZones(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)));

        return resource;
    }

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk MapToGrpcResponse(Shared.Models.Desk src)
    {
        var desk = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString()
        };

        desk.OrganizationCustomTags.AddRange(
            MapToGrpcResponseOrganizationCustomTags(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)));
        desk.OrganizationZones.AddRange(MapToGrpcResponseOrganizationZones(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)));

        return desk;
    }

    public Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Location = new Shared.Models.Location { Id = src.LocationId },
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public IEnumerable<Edge<Shared.Models.Desk>> MapTo(IEnumerable<Edge<Desk>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public Shared.Models.Room MapTo(Room src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Location = location,
            Tags = MapTo(src.OrganizationTags, location.Organization).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Room MapToGrpcResponse(Shared.Models.Room src)
    {
        var room = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Room
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString()
        };

        room.OrganizationCustomTags.AddRange(
            MapToGrpcResponseOrganizationCustomTags(src.Tags.Where(item => item.Type == OrganizationTagType.Custom)));
        room.OrganizationZones.AddRange(MapToGrpcResponseOrganizationZones(src.Tags.Where(item => item.Type == OrganizationTagType.Zone)));

        return room;
    }

    public Shared.Models.Room MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddRoomInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Location = new Shared.Models.Location { Id = src.LocationId },
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Room MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateRoomInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public IEnumerable<Edge<Shared.Models.Room>> MapTo(IEnumerable<Edge<Room>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private IEnumerable<LocationMember> MapTo(IEnumerable<Shared.Database.Entities.LocationMember> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static OrganizationTagDetails MapTo(OrganizationTag src) =>
        new()
        {
            UniqueId = src.Id,
            Name = src.Name,
            TagType = src.Type switch
            {
                OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
                OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
                OrganizationTagType.Desk => OrganizationTagTypeConstants.Desk,
                OrganizationTagType.Room => OrganizationTagTypeConstants.Room,
                _ => throw new ArgumentOutOfRangeException()
            },
            Color = src.Color
        };

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type switch
            {
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Desk => OrganizationTagType.Desk,
                OrganizationTagTypeConstants.Room => OrganizationTagType.Room,
                _ => throw new ArgumentOutOfRangeException()
            },
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

    private IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private IEnumerable<Shared.Models.Room> MapTo(IEnumerable<Room> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static IEnumerable<OrganizationCustomTag> MapToGrpcResponseOrganizationCustomTags(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationCustomTag);

    private static IEnumerable<OrganizationZone> MapToGrpcResponseOrganizationZones(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationZone);

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Resource> MapToGrpcResponse(IEnumerable<Shared.Models.Resource> src) =>
        src.Select(MapToGrpcResponse);

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk> MapToGrpcResponse(IEnumerable<Shared.Models.Desk> src) =>
        src.Select(MapToGrpcResponse);

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Room> MapToGrpcResponse(IEnumerable<Shared.Models.Room> src) =>
        src.Select(MapToGrpcResponse);

    private static LocationOrganizationDetails? MapTo(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new LocationOrganizationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private IEnumerable<DeskDetails> MapTo(IEnumerable<Shared.Models.Desk> src) => src.Select(MapTo);

    private IEnumerable<RoomDetails> MapTo(IEnumerable<Shared.Models.Room> src) => src.Select(MapTo);

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

    private static Shared.Models.Organization? MapTo(Organization? src) =>
        src is null
            ? null
            : new Shared.Models.Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering
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
            To = src.To,
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
            Status = src.Status switch
            {
                InvitationStatusConstants.Pending => InvitationStatus.Pending,
                InvitationStatusConstants.Accepted => InvitationStatus.Accepted,
                InvitationStatusConstants.Rejected => InvitationStatus.Rejected,
                InvitationStatusConstants.Cancelled => InvitationStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException()
            },
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
            Type = src.Type switch
            {
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Desk => OrganizationTagType.Desk,
                OrganizationTagTypeConstants.Room => OrganizationTagType.Room,
                _ => throw new ArgumentOutOfRangeException()
            },
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

    private Edge<Shared.Models.Desk> MapTo(Edge<Desk> src, Shared.Models.Location location)
    {
        var desk = MapTo(src.Node);
        desk.Location = location;
        return new Edge<Shared.Models.Desk>(src.Cursor, desk);
    }

    private Edge<Shared.Models.Room> MapTo(Edge<Room> src, Shared.Models.Location location)
    {
        var room = MapTo(src.Node);
        room.Location = location;
        return new Edge<Shared.Models.Room>(src.Cursor, room);
    }

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
}
