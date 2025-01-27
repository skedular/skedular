using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Location.Api.GraphQL;
using Location.Shared.Database.Entities;
using AddDeskInput = Location.Api.GraphQL.AddDeskInput;
using Booking = Location.Shared.Models.Booking;
using Customer = Location.Shared.Models.Customer;
using DailyDeskCountRecording = Location.Shared.Models.DailyDeskCountRecording;
using Desk = Location.Shared.Database.Entities.Desk;
using DeskEdge = Location.Api.GraphQL.DeskEdge;
using Identity = Location.Shared.Models.Identity;
using JoinInvitation = Location.Shared.Models.JoinInvitation;
using LocationEdge = Location.Api.GraphQL.LocationEdge;
using LocationDailyBookingsTotal = Location.Shared.Models.LocationDailyBookingsTotal;
using LocationDesksOccupancyPercentage = Location.Shared.Models.LocationDesksOccupancyPercentage;
using LocationMember = Location.Shared.Models.LocationMember;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationTag = Location.Shared.Models.OrganizationTag;
using Permissions = Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions;
using UpdateDeskInput = Location.Api.GraphQL.UpdateDeskInput;

namespace Location.Api.Mappers;

public interface IMapper
{
    Shared.Models.Location MapTo(Shared.Database.Entities.Location src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Shared.Database.Entities.Location MapTo(Shared.Models.Location src, Organization? organization);

    Shared.Database.Entities.Location MergeTo(
        Shared.Models.Location src,
        Shared.Database.Entities.Location dest,
        Address? physicalAddress);

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

    Shared.Models.Desk MapTo(Desk src, Shared.Models.Location location);
    LocationMember MapTo(Shared.Database.Entities.LocationMember src, Shared.Models.Location location);
    LocationMemberDetails MapTo(LocationMember src);
    LocationDetails? MapTo(Shared.Models.Location? src);
    DeskDetails MapTo(Shared.Models.Desk src);
    IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src);

    LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal);

    Shared.Models.Location MapTo(AddLocationInput src);
    Shared.Models.Location MapTo(UpdateLocationInput src);
    Shared.Models.Desk MapTo(AddDeskInput src);
    Shared.Models.Desk MapTo(UpdateDeskInput src);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);
    Shared.Models.Location MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src);
    Shared.Models.Location MapTo(AddInput src);
    Shared.Models.Location MapTo(UpdateInput src);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk MapToGrpcResponse(Shared.Models.Desk src);
    Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.AddDeskInput src);
    Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateDeskInput src);

    DeskEdge MapTo(Edge<Shared.Models.Desk> src);
    IEnumerable<Edge<Shared.Models.Desk>> MapTo(IEnumerable<Edge<Desk>> src, Shared.Models.Location location);
    global::Api.Shared.Services.Grpc.Skedular.Location.V1.DeskEdge MapToGrpcResponse(Edge<Shared.Models.Desk> src);

    LocationEdge MapTo(Edge<Shared.Models.Location> src);

    global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(
        Edge<Shared.Models.Location> src);

    LocationMemberEdge MapTo(Edge<LocationMember> src);

    IEnumerable<Edge<LocationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src,
        Shared.Models.Location location);

    Address MapTo(Shared.Models.Address src, Shared.Database.Entities.Location location);
    Address MergeToEntity(Shared.Models.Address src, Address dest, Shared.Database.Entities.Location location);
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
        location.JoinInvitations = MapTo(src.JoinInvitations, location).ToList();
        location.Desks = MapTo(src.Desks, location).ToList();
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
                Organization = MapTo(src.Organization),
                Desks = MapTo(src.Desks).ToArray(),
                CustomTags = MapTo(src.CustomTags).ToArray(),
                Zones = MapTo(src.Zones).ToArray(),
                PhysicalAddress = MapToGraphQl(src.PhysicalAddress)
            };

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
            CustomTags =
                MapTo(src.OrganizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Custom)).ToList(),
            Zones = MapTo(src.OrganizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Zone)).ToList()
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

    public DeskEdge MapTo(Edge<Shared.Models.Desk> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public DeskDetails MapTo(Shared.Models.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            CustomTags = MapTo(src.CustomTags).ToArray(),
            Zones = MapTo(src.Zones).ToArray()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.DeskEdge MapToGrpcResponse(
        Edge<Shared.Models.Desk> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public LocationEdge MapTo(Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node)! };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.LocationEdge MapToGrpcResponse(
        Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public LocationMemberEdge MapTo(Edge<LocationMember> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public IEnumerable<Edge<LocationMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public Address MapTo(Shared.Models.Address src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new Address(), location);

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

    public IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) =>
        src.Select(MapTo)!;

    public LocationAnalytics MapTo(
        string name,
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal) =>
        new()
        {
            Name = name,
            DesksOccupancyPercentage = locationDesksOccupancyPercentage
                .Select(item => new DesksOccupancyPercentage { Date = item.Date, Percentage = item.Percentage })
                .ToArray(),
            DailyBookingsTotals = locationDailyBookingsTotal
                .Select(item => new GraphQL.LocationDailyBookingsTotal { Date = item.Date, Total = item.Total })
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
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId }
        };

        location.PhysicalAddress = MapTo(src.PhysicalAddress, location);

        return location;
    }

    public Shared.Models.Location MapTo(UpdateLocationInput src)
    {
        var location = new Shared.Models.Location
        {
            Id = src.Id.ToSafeString(), Name = src.Name, About = src.About, Timezone = src.Timezone
        };

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
            CustomTags = src.CustomTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            Zones = src.ZoneIds.Select(item => new OrganizationTag { Id = item }).ToList(),
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
            CustomTags = src.CustomTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            Zones = src.ZoneIds.Select(item => new OrganizationTag { Id = item }).ToList()
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
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId }
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

        location.Desks.AddRange(MapToGrpcResponse(src.Desks));
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
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.Location MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId }
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
            CustomTags = MapTo(
                    src.OrganizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Custom),
                    location.Organization)
                .ToList(),
            Zones = MapTo(
                    src.OrganizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Zone),
                    location.Organization)
                .ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk MapToGrpcResponse(
        Shared.Models.Desk src)
    {
        var desk = new global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString()
        };

        desk.OrganizationCustomTags.AddRange(MapToGrpcResponseOrganizationCustomTags(src.CustomTags));
        desk.OrganizationZones.AddRange(MapToGrpcResponseOrganizationZones(src.Zones));

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
            CustomTags = src.CustomTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            Zones = src.ZoneIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.Skedular.Location.V1.UpdateDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            CustomTags = src.CustomTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            Zones = src.ZoneIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public IEnumerable<Edge<Shared.Models.Desk>> MapTo(IEnumerable<Edge<Desk>> src, Shared.Models.Location location) =>
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
                OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
                OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
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
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
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

    private IEnumerable<OrganizationTag> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationTag> src,
        Shared.Models.Organization? organization) =>
        src.Select(item => MapTo(item, organization));

    private IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) =>
        src.Select(MapTo);

    private static IEnumerable<OrganizationCustomTag> MapToGrpcResponseOrganizationCustomTags(
        IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationCustomTag);

    private static IEnumerable<OrganizationZone> MapToGrpcResponseOrganizationZones(IEnumerable<OrganizationTag> src) =>
        src.Select(MapToGrpcResponseOrganizationZone);

    private IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Location.V1.Desk> MapToGrpcResponse(
        IEnumerable<Shared.Models.Desk> src) => src.Select(MapToGrpcResponse);

    private static LocationOrganizationDetails? MapTo(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new LocationOrganizationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl };

    private static IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private IEnumerable<DeskDetails> MapTo(IEnumerable<Shared.Models.Desk> src) => src.Select(MapTo);

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

    private static IEnumerable<Booking> MapTo(IEnumerable<Shared.Database.Entities.Booking> src,
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

    private static DailyDeskCountRecording MapTo(
        Shared.Database.Entities.DailyDeskCountRecording src,
        Shared.Models.Location location) =>
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

    private IEnumerable<JoinInvitation> MapTo(
        IEnumerable<Shared.Database.Entities.JoinInvitation> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private JoinInvitation MapTo(
        Shared.Database.Entities.JoinInvitation src,
        Shared.Models.Location location) =>
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

    private static OrganizationTag MapTo(
        Shared.Database.Entities.OrganizationTag src,
        Shared.Models.Organization? organization)
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
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
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

    private Edge<LocationMember> MapTo(
        Edge<Shared.Database.Entities.LocationMember> src,
        Shared.Models.Location location) =>
        new(src.Cursor, MapTo(src.Node, location));

    private Edge<Shared.Models.Desk> MapTo(Edge<Desk> src, Shared.Models.Location location)
    {
        var desk = MapTo(src.Node);
        desk.Location = location;
        return new Edge<Shared.Models.Desk>(src.Cursor, desk);
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
