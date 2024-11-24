using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Location.Api.GraphQL;
using Location.Shared.Models;
using AddDeskInput = Location.Api.GraphQL.AddDeskInput;
using Customer = Location.Shared.Models.Customer;
using Desk = Location.Shared.Database.Entities.Desk;
using DeskEdge = Location.Api.GraphQL.DeskEdge;
using Identity = Location.Shared.Models.Identity;
using LocationTagEdge = Location.Api.GraphQL.LocationTagEdge;
using LocationEdge = Location.Api.GraphQL.LocationEdge;
using LocationDailyBookingsTotal = Location.Shared.Models.LocationDailyBookingsTotal;
using LocationDesksOccupancyPercentage = Location.Shared.Models.LocationDesksOccupancyPercentage;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationTag = Location.Shared.Models.OrganizationTag;
using Permissions = Api.Shared.Services.Grpc.UnityHub.Location.V1.Permissions;
using Tag = Location.Shared.Database.Entities.Tag;
using UpdateDeskInput = Location.Api.GraphQL.UpdateDeskInput;

namespace Location.Api.Mappers;

public interface IMapper
{
    Shared.Models.Location MapTo(Shared.Database.Entities.Location src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    Shared.Database.Entities.Location MapTo(Shared.Models.Location src, Organization? organization);
    Shared.Database.Entities.Location MergeTo(Shared.Models.Location src, Shared.Database.Entities.Location dest);
    Tag MapTo(Shared.Models.Tag src, Shared.Database.Entities.Location location);
    Tag MergeTo(Shared.Models.Tag src, Tag dest, Shared.Database.Entities.Location location);
    Shared.Models.Tag MapTo(Tag src);
    Shared.Models.Desk MapTo(Desk src);

    Desk MapTo(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);

    Desk MergeTo(
        Shared.Models.Desk src,
        Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags);
    Shared.Models.Desk MapTo(Desk src, Shared.Models.Location location);

    IEnumerable<LocationMember> MapTo(
        IEnumerable<Shared.Database.Entities.LocationMember> src,
        Shared.Models.Location location);

    LocationMember MapTo(Shared.Database.Entities.LocationMember src, Shared.Models.Location location);
    LocationMemberDetails MapTo(LocationMember src);
    LocationDetails? MapTo(Shared.Models.Location? src);
    DeskDetails MapTo(Shared.Models.Desk src);
    LocationTagDetails MapTo(Shared.Models.Tag src);
    OrganizationTagDetails MapTo(OrganizationTag src);
    IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src);

    LocationAnalytics MapTo(
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal);

    Shared.Models.Location MapTo(AddLocationInput src);
    Shared.Models.Location MapTo(UpdateLocationInput src);
    Shared.Models.Tag MapTo(AddLocationTagInput src);
    Shared.Models.Tag MapTo(UpdateLocationTagInput src);
    Shared.Models.Desk MapTo(AddDeskInput src);
    Shared.Models.Desk MapTo(UpdateDeskInput src);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);
    Shared.Models.Location MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src);
    Shared.Models.Location MapTo(AddInput src);
    Shared.Models.Location MapTo(UpdateInput src);
    global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Tag MapToGrpcResponse(Shared.Models.Tag src);
    global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk MapToGrpcResponse(Shared.Models.Desk src);
    Shared.Models.Tag MapTo(AddTagInput src);
    Shared.Models.Tag MapTo(Admin_AddTagInput src);
    Shared.Models.Tag MapTo(UpdateTagInput src);
    Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.AddDeskInput src);
    Shared.Models.Desk MapTo(Admin_AddDeskInput src);
    Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.UpdateDeskInput src);

    DeskEdge MapTo(Edge<Shared.Models.Desk> src);
    IEnumerable<Edge<Shared.Models.Desk>> MapTo(IEnumerable<Edge<Desk>> src, Shared.Models.Location location);
    global::Api.Shared.Services.Grpc.UnityHub.Location.V1.DeskEdge MapToGrpcResponse(Edge<Shared.Models.Desk> src);

    LocationTagEdge MapTo(Edge<Shared.Models.Tag> src);
    IEnumerable<Edge<Shared.Models.Tag>> MapTo(IEnumerable<Edge<Tag>> src, Shared.Models.Location location);
    TagEdge MapToGrpcResponse(Edge<Shared.Models.Tag> src);
    LocationEdge MapTo(Edge<Shared.Models.Location> src);

    global::Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationEdge MapToGrpcResponse(
        Edge<Shared.Models.Location> src);

    LocationMemberEdge MapTo(Edge<LocationMember> src);

    IEnumerable<Edge<LocationMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src,
        Shared.Models.Location location);

    Shared.Database.Entities.LocationMember MapToEntity(
        LocationMember src,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer);

    Shared.Database.Entities.LocationMember MergeToEntity(
        LocationMember src,
        Shared.Database.Entities.LocationMember dest,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer);

    ICollection<LocationMember> MapTo(Admin_UpdateMembersInput src);
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
        location.Tags = MapTo(src.Tags, location).ToList();
        location.Desks = MapTo(src.Desks, location).ToList();

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

    public Shared.Database.Entities.Location MergeTo(Shared.Models.Location src, Shared.Database.Entities.Location dest)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
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
                LocationTags = MapTo(src.Tags).ToArray(),
                Desks = MapTo(src.Desks).ToArray()
            };

    public Tag MapTo(Shared.Models.Tag src, Shared.Database.Entities.Location location) =>
        MergeTo(src, new Tag(), location);

    public Tag MergeTo(Shared.Models.Tag src, Tag dest, Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Type = src.Type;
        dest.Location = location;
        return dest;
    }

    public Shared.Models.Tag MapTo(Tag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type
        };

    public OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type
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
            Tags = MapTo(src.Tags).ToList(),
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    public Desk MapTo(
        Shared.Models.Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags) =>
        MergeTo(src, new Desk(), location, tags, organizationTags);

    public Desk MergeTo(
        Shared.Models.Desk src,
        Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<Shared.Database.Entities.OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Deactivated = src.Deactivated;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Tags = tags;
        dest.OrganizationTags = organizationTags;
        dest.Location = location;
        return dest;
    }

    public IEnumerable<LocationMember> MapTo(IEnumerable<Shared.Database.Entities.LocationMember> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public LocationMember
        MapTo(Shared.Database.Entities.LocationMember src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            MembershipType = src.MembershipType,
            Customer = MapTo(src.Customer)!,
            Location = location
        };

    public LocationMemberDetails MapTo(LocationMember src) =>
        new()
        {
            Id = src.Id,
            MembershipType = src.MembershipType switch
            {
                LocationMembershipType.Owner => LocationMemberMembershipType.Owner,
                LocationMembershipType.Administrator => LocationMemberMembershipType.Administrator,
                LocationMembershipType.Member => LocationMemberMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)
        };

    public LocationTagDetails MapTo(Shared.Models.Tag src) =>
        new() { Id = src.Id, Name = src.Name, Description = src.Description, TagType = src.Type };

    public OrganizationTagDetails MapTo(OrganizationTag src) =>
        new() { UniqueId = src.Id, Name = src.Name, TagType = src.Type };

    public DeskEdge MapTo(Edge<Shared.Models.Desk> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public DeskDetails MapTo(Shared.Models.Desk src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            LocationTags = MapTo(src.Tags).ToArray(),
            OrganizationTags = MapTo(src.OrganizationTags).ToArray()
        };

    public global::Api.Shared.Services.Grpc.UnityHub.Location.V1.DeskEdge MapToGrpcResponse(
        Edge<Shared.Models.Desk> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public LocationTagEdge MapTo(Edge<Shared.Models.Tag> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public IEnumerable<Edge<Shared.Models.Tag>> MapTo(IEnumerable<Edge<Tag>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public TagEdge MapToGrpcResponse(Edge<Shared.Models.Tag> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public LocationEdge MapTo(Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node)! };

    public global::Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationEdge MapToGrpcResponse(
        Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public LocationMemberEdge MapTo(Edge<LocationMember> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public IEnumerable<Edge<LocationMember>> MapTo(IEnumerable<Edge<Shared.Database.Entities.LocationMember>> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public Shared.Database.Entities.LocationMember MapToEntity(
        LocationMember src,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new Shared.Database.Entities.LocationMember(), location, customer);

    public Shared.Database.Entities.LocationMember MergeToEntity(
        LocationMember src,
        Shared.Database.Entities.LocationMember dest,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.MembershipType = src.MembershipType;
        dest.Location = location;
        dest.Customer = customer;
        return dest;
    }

    public ICollection<LocationMember> MapTo(Admin_UpdateMembersInput src) =>
        src.Members.Select(item => MapTo(item, new Shared.Models.Location { Id = src.Id })).ToList();

    public IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) =>
        src.Select(MapTo)!;

    public LocationAnalytics MapTo(
        IEnumerable<LocationDesksOccupancyPercentage> locationDesksOccupancyPercentage,
        IEnumerable<LocationDailyBookingsTotal> locationDailyBookingsTotal) =>
        new()
        {
            DesksOccupancyPercentage = locationDesksOccupancyPercentage.Select(item =>
                    new GraphQL.LocationDesksOccupancyPercentage { Date = item.Date, Percentage = item.Percentage })
                .ToArray(),
            DailyBookingsTotals = locationDailyBookingsTotal.Select(item =>
                    new GraphQL.LocationDailyBookingsTotal { Date = item.Date, Total = item.Total })
                .ToArray()
        };

    public Shared.Models.Location MapTo(AddLocationInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                ? null
                : new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Shared.Models.Location MapTo(UpdateLocationInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone
        };

    public Shared.Models.Tag MapTo(AddLocationTagInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            Description = src.Description,
            Location = new Shared.Models.Location { Id = src.LocationId },
            Type = src.TagType
        };

    public Shared.Models.Tag MapTo(UpdateLocationTagInput src) =>
        new() { Id = src.Id, Name = src.Name, Description = src.Description, Type = src.TagType };

    public Shared.Models.Desk MapTo(AddDeskInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            Name = src.Name,
            Deactivated = false,
            RequireBookingApproval = false,
            Tags = src.LocationTagIds.Select(item => new Shared.Models.Tag { Id = item }).ToList(),
            OrganizationTags = src.OrganizationTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Shared.Models.Desk MapTo(UpdateDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Tags = src.LocationTagIds.Select(item => new Shared.Models.Tag { Id = item }).ToList(),
            OrganizationTags = src.OrganizationTagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status,
            MembershipType = src.MembershipType,
            Location = MapTo(src.Location),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public Shared.Models.Location MapTo(
        Admin_AddInput src) =>
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

    public global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Location MapToGrpcResponse(Shared.Models.Location src)
    {
        var location = new global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Location
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

        location.Tags.AddRange(MapToGrpcResponse(src.Tags));
        location.Desks.AddRange(MapToGrpcResponse(src.Desks));

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
            Location = location,
            Tags = MapTo(src.Tags, location).ToList(),
            OrganizationTags = MapTo(src.OrganizationTags, location.Organization).ToList()
        };

    public global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Tag MapToGrpcResponse(
        Shared.Models.Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = src.Type.ToSafeString()
        };

    public global::Api.Shared.Services.Grpc.UnityHub.Location.V1.OrganizationTag MapToGrpcResponse(
        OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Type = src.Type.ToSafeString()
        };

    public global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk MapToGrpcResponse(
        Shared.Models.Desk src)
    {
        var desk = new global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval
        };

        desk.Tags.AddRange(MapToGrpcResponse(src.Tags));
        desk.OrganizationTags.AddRange(MapToGrpcResponse(src.OrganizationTags));

        return desk;
    }

    public Shared.Models.Tag MapTo(AddTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = src.Type.ToSafeString(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Shared.Models.Tag MapTo(Admin_AddTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = src.Type.ToSafeString(),
            Location = new Shared.Models.Location { Id = src.LocationId }
        };

    public Shared.Models.Tag MapTo(UpdateTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = src.Type.ToSafeString()
        };

    public Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.AddDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Location = new Shared.Models.Location { Id = src.LocationId },
            Tags = src.TagIds.Select(item => new Shared.Models.Tag { Id = item }).ToList(),
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
        };

    public Shared.Models.Desk MapTo(Admin_AddDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Location = new Shared.Models.Location { Id = src.LocationId },
            Tags = src.TagIds.Select(item => new Shared.Models.Tag { Id = item }).ToList(),
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
        };

    public Shared.Models.Desk MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.UpdateDeskInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Tags = src.TagIds.Select(item => new Shared.Models.Tag { Id = item }).ToList(),
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
        };

    public IEnumerable<Edge<Shared.Models.Desk>> MapTo(IEnumerable<Edge<Desk>> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static LocationMember MapTo(Member src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            MembershipType = src.MembershipType switch
            {
                MembershipType.Owner => LocationMembershipType.Owner,
                MembershipType.Administrator => LocationMembershipType.Administrator,
                MembershipType.Member => LocationMembershipType.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.Customer.Id },
            Location = location
        };

    private IEnumerable<Shared.Models.Tag> MapTo(IEnumerable<Tag> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private IEnumerable<OrganizationTag> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationTag> src,
        Shared.Models.Organization? organization) =>
        src.Select(item => MapTo(item, organization));

    private IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private IEnumerable<Shared.Models.Tag> MapTo(IEnumerable<Tag> src) =>
        src.Select(MapTo);

    private IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) =>
        src.Select(MapTo);

    public Shared.Models.Tag MapTo(global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = src.Type.ToSafeString()
        };

    private IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Tag> MapToGrpcResponse(
        IEnumerable<Shared.Models.Tag> src) => src.Select(MapToGrpcResponse);

    private IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Location.V1.OrganizationTag> MapToGrpcResponse(
        IEnumerable<OrganizationTag> src) => src.Select(MapToGrpcResponse);

    private IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Location.V1.Desk> MapToGrpcResponse(
        IEnumerable<Shared.Models.Desk> src) => src.Select(MapToGrpcResponse);

    private static LocationOrganizationDetails? MapTo(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new LocationOrganizationDetails
            {
                UniqueId = src.Id, Name = src.Name.ToSafeString(), LogoUrl = src.LogoUrl
            };

    private IEnumerable<LocationTagDetails> MapTo(IEnumerable<Shared.Models.Tag> src) => src.Select(MapTo);

    private IEnumerable<OrganizationTagDetails> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

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

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) =>
        src.Select(MapTo);

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
            Status = src.Status,
            Location = location,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private static Shared.Models.Tag MapTo(Tag src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type,
            Location = location
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
            Type = src.Type
        };

        if (organization is not null)
        {
            organizationTag.Organization = organization;
        }

        return organizationTag;
    }

    private Edge<LocationMember> MapTo(Edge<Shared.Database.Entities.LocationMember> src,
        Shared.Models.Location location) =>
        new(src.Cursor, MapTo(src.Node, location));

    private Edge<Shared.Models.Desk> MapTo(Edge<Desk> src, Shared.Models.Location location)
    {
        var desk = MapTo(src.Node);
        desk.Location = location;
        return new Edge<Shared.Models.Desk>(src.Cursor, desk);
    }

    private Edge<Shared.Models.Tag> MapTo(Edge<Tag> src, Shared.Models.Location location)
    {
        var tag = MapTo(src.Node);
        tag.Location = location;
        return new Edge<Shared.Models.Tag>(src.Cursor, tag);
    }
}
