using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
using Booking.Api.GraphQL;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Google.Protobuf.WellKnownTypes;
using BookingEdge = Booking.Api.GraphQL.BookingEdge;
using Customer = Booking.Shared.Models.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Identity = Booking.Shared.Models.Identity;
using Location = Booking.Shared.Database.Entities.Location;
using LocationTag = Api.Shared.Services.Grpc.UnityHub.Booking.V1.LocationTag;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Mappers;

public interface IMapper
{
    Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    BookingDetails MapTo(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddBookingInput src);
    Shared.Models.Booking MapTo(UpdateBookingInput src);
    Shared.Models.Location? MapTo(Location? src);
    IEnumerable<BookingDeskDetails> MapTo(IEnumerable<Shared.Models.Desk> src);

    Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks);

    Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks);

    IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location);
    global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddInput src);
    Shared.Models.Booking MapTo(Admin_AddInput src);
    Shared.Models.Booking MapTo(UpdateInput src);

    IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk> MapToGrpcResponse(
        IEnumerable<Shared.Models.Desk> src);

    IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src);
    Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src);
    BookingEdge MapTo(Edge<Shared.Models.Booking> src);

    global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.BookingEdge MapToGrpcResponse(
        Edge<Shared.Models.Booking> src);
}

public class Mapper : IMapper
{
    public Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src)
    {
        var team = new Shared.Models.Booking
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            To = src.To,
            Notes = src.Notes,
            Customer = MapTo(src.Customer)!,
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Desks = MapTo(src.Desks).ToList(),
            Team = MapTo(src.Team)
        };

        return team;
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

    public BookingDetails MapTo(Shared.Models.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From,
            To = src.To,
            Notes = src.Notes,
            Customer = MapTo(src.Customer),
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Desks = MapTo(src.Desks).ToArray(),
            Team = MapTo(src.Team)
        };

    public Shared.Models.Booking MapTo(AddBookingInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            From = src.From,
            To = src.To,
            Notes = src.Notes,
            Customer = new Customer { Id = src.CustomerId },
            Organization =
                string.IsNullOrWhiteSpace(src.OrganizationId)
                    ? null
                    : new Shared.Models.Organization { Id = src.OrganizationId },
            Location =
                string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList()
        };

    public Shared.Models.Booking MapTo(UpdateBookingInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From,
            To = src.To,
            Notes = src.Notes,
            Customer = new Customer { Id = src.CustomerId },
            Organization =
                string.IsNullOrWhiteSpace(src.OrganizationId)
                    ? null
                    : new Shared.Models.Organization { Id = src.OrganizationId },
            Location =
                string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList()
        };

    public Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks) =>
        MergeTo(src, new Shared.Database.Entities.Booking(), customer, organization, location, team, desks);

    public Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.To = src.To;
        dest.Notes = src.Notes;
        dest.Customer = customer;
        dest.Organization = organization;
        dest.Location = location;
        dest.Team = team;
        dest.Desks = desks;
        return dest;
    }

    public IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src)
    {
        var booking = new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.To.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Customer = MapToGrpcResponse(src.Customer),
            Organization = MapToGrpcResponse(src.Organization),
            Location = MapToGrpcResponse(src.Location),
            Team = MapToGrpcResponse(src.Team)
        };

        booking.Desks.AddRange(MapToGrpcResponse(src.Desks));

        return booking;
    }

    public Shared.Models.Booking MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Customer = new Customer { Id = src.CustomerId },
            Organization =
                string.IsNullOrWhiteSpace(src.OrganizationId)
                    ? null
                    : new Shared.Models.Organization { Id = src.OrganizationId },
            Location =
                string.IsNullOrWhiteSpace(src.LocationId)
                    ? null
                    : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList()
        };

    public Shared.Models.Booking MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Customer = new Customer { Id = src.CustomerId },
            Organization =
                string.IsNullOrWhiteSpace(src.OrganizationId)
                    ? null
                    : new Shared.Models.Organization { Id = src.OrganizationId },
            Location =
                string.IsNullOrWhiteSpace(src.LocationId)
                    ? null
                    : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList()
        };

    public Shared.Models.Booking MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Customer = new Customer { Id = src.CustomerId },
            Organization =
                string.IsNullOrWhiteSpace(src.OrganizationId)
                    ? null
                    : new Shared.Models.Organization { Id = src.OrganizationId },
            Location =
                string.IsNullOrWhiteSpace(src.LocationId)
                    ? null
                    : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList()
        };

    public Shared.Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Shared.Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    public IEnumerable<BookingDeskDetails> MapTo(IEnumerable<Shared.Models.Desk> src) => src.Select(MapTo);

    public IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk> MapToGrpcResponse(
        IEnumerable<Shared.Models.Desk> src) =>
        src.Select(MapToGrpcResponse);

    public IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src) =>
        src.Select(MapTo);

    public Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src) =>
        new(src.Cursor, MapTo(src.Node));

    public BookingEdge MapTo(Edge<Shared.Models.Booking> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.BookingEdge MapToGrpcResponse(
        Edge<Shared.Models.Booking> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static Shared.Models.Desk MapTo(Desk src, Shared.Models.Location location) =>
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
            Tags = MapTo(src.Tags, location).ToList()
        };

    private static IEnumerable<Shared.Models.LocationTag> MapTo(IEnumerable<Shared.Database.Entities.LocationTag> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Shared.Models.LocationTag MapTo(Shared.Database.Entities.LocationTag src,
        Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type,
            Location = location
        };

    private static global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Customer MapToGrpcResponse(Customer src)
    {
        var customer = new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Customer
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString()
        };

        customer.Identities.AddRange(MapToGrpcResponse(src.Identities));

        return customer;
    }

    private static IEnumerable<global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Identity> MapToGrpcResponse(
        IEnumerable<Identity> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Identity MapToGrpcResponse(Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };

    private static global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Organization? MapToGrpcResponse(
        Shared.Models.Organization? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Organization
            {
                Id = src.Id, Name = src.Name.ToSafeString()
            };

    private static global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Location? MapToGrpcResponse(
        Shared.Models.Location? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Location
            {
                Id = src.Id, Name = src.Name.ToSafeString()
            };

    private static global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Team?
        MapToGrpcResponse(Shared.Models.Team? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Team
            {
                Id = src.Id, Name = src.Name.ToSafeString()
            };

    private static global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk MapToGrpcResponse(Shared.Models.Desk src)
    {
        var desk = new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Desk
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Location = src.Location is null
                ? null
                : new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.Location
                {
                    Id = src.Location.Id, Name = src.Location.Name.ToSafeString()
                }
        };

        desk.LocationTags.AddRange(MapToGrpcResponse(src.Tags));
        desk.OrganizationDeskTypes.AddRange(MapToGrpcResponseDeskTypes(src.OrganizationTags));
        desk.OrganizationZones.AddRange(MapToGrpcResponseZones(src.OrganizationTags));

        return desk;
    }

    private static IEnumerable<LocationTag> MapToGrpcResponse(IEnumerable<Shared.Models.LocationTag> src) =>
        src.Select(MapToGrpcResponse);

    private static LocationTag MapToGrpcResponse(Shared.Models.LocationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), TagType = src.Type.ToSafeString() };

    private static IEnumerable<OrganizationDeskType> MapToGrpcResponseDeskTypes(IEnumerable<Shared.Models.OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.DeskType).Select(MapToGrpcResponseDeskType);

    private static OrganizationDeskType MapToGrpcResponseDeskType(Shared.Models.OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapToGrpcResponseZones(IEnumerable<Shared.Models.OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Zone).Select(MapToGrpcResponseZone);

    private static OrganizationZone MapToGrpcResponseZone(Shared.Models.OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString() };

    private static BookingCustomerDetails MapTo(Customer src) =>
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

    private static BookingOrganizationDetails? MapTo(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new BookingOrganizationDetails
            {
                UniqueId = src.Id, Name = string.IsNullOrWhiteSpace(src.Name) ? string.Empty : src.Name
            };

    private static BookingLocationDetails? MapTo(Shared.Models.Location? src) =>
        src is null
            ? null
            : new BookingLocationDetails
            {
                UniqueId = src.Id, Name = string.IsNullOrWhiteSpace(src.Name) ? string.Empty : src.Name
            };

    private static BookingTeamDetails? MapTo(Shared.Models.Team? src) =>
        src is null
            ? null
            : new BookingTeamDetails
            {
                UniqueId = src.Id, Name = string.IsNullOrWhiteSpace(src.Name) ? string.Empty : src.Name
            };

    private static BookingDeskDetails MapTo(Shared.Models.Desk src) =>
        new()
        {
            UniqueId = src.Id,
            Name = string.IsNullOrWhiteSpace(src.Name) ? string.Empty : src.Name,
            LocationTags = MapTo(src.Tags).ToArray(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Location = MapTo(src.Location)
        };

    private static IEnumerable<BookingLocationTagDetails> MapTo(IEnumerable<Shared.Models.LocationTag> src) =>
        src.Select(MapTo);

    private static BookingLocationTagDetails MapTo(Shared.Models.LocationTag src) =>
        new()
        {
            UniqueId = src.Id,
            Name = string.IsNullOrWhiteSpace(src.Name) ? string.Empty : src.Name,
            TagType = src.Type
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

    private static Shared.Models.Desk MapTo(Desk src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Tags = MapTo(src.Tags).ToList()
        };

    private static Shared.Models.Team? MapTo(Team? src) =>
        src is null
            ? null
            : new Shared.Models.Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    private static IEnumerable<Shared.Models.LocationTag>
        MapTo(IEnumerable<Shared.Database.Entities.LocationTag> src) =>
        src.Select(MapTo);

    private static Shared.Models.LocationTag MapTo(Shared.Database.Entities.LocationTag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            Type = src.Type
        };
}
