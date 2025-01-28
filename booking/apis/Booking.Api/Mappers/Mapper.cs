using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Google.Protobuf.WellKnownTypes;
using BookingEdge = Booking.Api.GraphQL.BookingEdge;
using BookingType = Api.Shared.Services.Models.BookingType;
using Customer = Booking.Shared.Models.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Room = Booking.Shared.Database.Entities.Room;
using Identity = Booking.Shared.Models.Identity;
using Location = Booking.Shared.Database.Entities.Location;
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
    IEnumerable<BookingRoomDetails> MapTo(IEnumerable<Shared.Models.Room> src);

    Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks,
        ICollection<Room> rooms);

    Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks,
        ICollection<Room> rooms);

    IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location);
    IEnumerable<Shared.Models.Room> MapTo(IEnumerable<Room> src, Shared.Models.Location location);
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddInput src);
    Shared.Models.Booking MapTo(Admin_AddInput src);
    Shared.Models.Booking MapTo(UpdateInput src);
    IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Desk> MapToGrpcResponse(IEnumerable<Shared.Models.Desk> src);
    IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src);
    IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Room> MapToGrpcResponse(IEnumerable<Shared.Models.Room> src);
    IEnumerable<Shared.Models.Room> MapTo(IEnumerable<Room> src);
    Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src);
    BookingEdge MapTo(Edge<Shared.Models.Booking> src);
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src);
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
            Type = src.Type switch
            {
                BookingTypeConstants.WorkingFromHome => BookingType.WorkingFromHome,
                BookingTypeConstants.WorkingFromOffice => BookingType.WorkingFromOffice,
                BookingTypeConstants.SickLeave => BookingType.SickLeave,
                BookingTypeConstants.AnnualLeave => BookingType.AnnualLeave,
                BookingTypeConstants.WellBeingLeave => BookingType.WellBeingLeave,
                BookingTypeConstants.ClientOffices => BookingType.ClientOffices,
                BookingTypeConstants.Vacation => BookingType.Vacation,
                BookingTypeConstants.TravelingForWork => BookingType.TravelingForWork,
                BookingTypeConstants.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)!,
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Desks = MapTo(src.Desks).ToList(),
            Rooms = MapTo(src.Rooms).ToList(),
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
            Type = src.Type,
            Customer = MapTo(src.Customer),
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Desks = MapTo(src.Desks).ToArray(),
            Rooms = MapTo(src.Rooms).ToArray(),
            Team = MapTo(src.Team)
        };

    public Shared.Models.Booking MapTo(AddBookingInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            To = src.To,
            Notes = src.Notes,
            Type = src.Type,
            Customer = new Customer { Id = src.CustomerId },
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList(),
            Rooms = src.RoomIds.Select(item => new Shared.Models.Room { Id = item }).ToList()
        };

    public Shared.Models.Booking MapTo(UpdateBookingInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From,
            To = src.To,
            Notes = src.Notes,
            Type = src.Type,
            Customer = new Customer { Id = src.CustomerId },
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList(),
            Rooms = src.RoomIds.Select(item => new Shared.Models.Room { Id = item }).ToList()
        };

    public Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks,
        ICollection<Room> rooms) =>
        MergeTo(src, new Shared.Database.Entities.Booking(), customer, organization, location, team, desks, rooms);

    public Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Desk> desks,
        ICollection<Room> rooms)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.To = src.To;
        dest.Notes = src.Notes;
        dest.Type = src.Type switch
        {
            BookingType.WorkingFromHome => BookingTypeConstants.WorkingFromHome,
            BookingType.WorkingFromOffice => BookingTypeConstants.WorkingFromOffice,
            BookingType.SickLeave => BookingTypeConstants.SickLeave,
            BookingType.AnnualLeave => BookingTypeConstants.AnnualLeave,
            BookingType.WellBeingLeave => BookingTypeConstants.WellBeingLeave,
            BookingType.ClientOffices => BookingTypeConstants.ClientOffices,
            BookingType.Vacation => BookingTypeConstants.Vacation,
            BookingType.TravelingForWork => BookingTypeConstants.TravelingForWork,
            BookingType.NonWorkingDay => BookingTypeConstants.NonWorkingDay,
            _ => throw new ArgumentOutOfRangeException()
        };
        dest.Customer = customer;
        dest.Organization = organization;
        dest.Location = location;
        dest.Team = team;
        dest.Desks = desks;
        dest.Rooms = rooms;
        return dest;
    }

    public IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src, Shared.Models.Location location) => src.Select(item => MapTo(item, location));
    public IEnumerable<Shared.Models.Room> MapTo(IEnumerable<Room> src, Shared.Models.Location location) => src.Select(item => MapTo(item, location));

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src)
    {
        var booking = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.To.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Customer = MapToGrpcResponse(src.Customer),
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice,
                BookingType.SickLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave,
                BookingType.AnnualLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave,
                BookingType.WellBeingLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave,
                BookingType.ClientOffices => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices,
                BookingType.Vacation => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation,
                BookingType.TravelingForWork => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Organization = MapToGrpcResponse(src.Organization),
            Location = MapToGrpcResponse(src.Location),
            Team = MapToGrpcResponse(src.Team)
        };

        booking.Desks.AddRange(MapToGrpcResponse(src.Desks));
        booking.Rooms.AddRange(MapToGrpcResponse(src.Rooms));

        return booking;
    }

    public Shared.Models.Booking MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome => BookingType.WorkingFromHome,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice => BookingType.WorkingFromOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave => BookingType.WellBeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices => BookingType.ClientOffices,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.CustomerId },
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId)
                    ? null
                    : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId)
                    ? null
                    : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList(),
            Rooms = src.RoomIds.Select(item => new Shared.Models.Room { Id = item }).ToList()
        };

    public Shared.Models.Booking MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome => BookingType.WorkingFromHome,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice => BookingType.WorkingFromOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave => BookingType.WellBeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices => BookingType.ClientOffices,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.CustomerId },
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList(),
            Rooms = src.RoomIds.Select(item => new Shared.Models.Room { Id = item }).ToList()
        };

    public Shared.Models.Booking MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            To = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome => BookingType.WorkingFromHome,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice => BookingType.WorkingFromOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave => BookingType.WellBeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices => BookingType.ClientOffices,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.CustomerId },
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Desks = src.DeskIds.Select(item => new Shared.Models.Desk { Id = item }).ToList(),
            Rooms = src.RoomIds.Select(item => new Shared.Models.Room { Id = item }).ToList()
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

    public IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Desk> MapToGrpcResponse(IEnumerable<Shared.Models.Desk> src) =>
        src.Select(MapToGrpcResponse);

    public IEnumerable<Shared.Models.Desk> MapTo(IEnumerable<Desk> src) => src.Select(MapTo);

    public IEnumerable<BookingRoomDetails> MapTo(IEnumerable<Shared.Models.Room> src) => src.Select(MapTo);

    public IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Room> MapToGrpcResponse(IEnumerable<Shared.Models.Room> src) =>
        src.Select(MapToGrpcResponse);

    public IEnumerable<Shared.Models.Room> MapTo(IEnumerable<Room> src) => src.Select(MapTo);
    public Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src) => new(src.Cursor, MapTo(src.Node));
    public BookingEdge MapTo(Edge<Shared.Models.Booking> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);
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
            Color = src.Color,
            Location = location
        };

    private static Shared.Models.Room MapTo(Room src, Shared.Models.Location location) =>
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
            Location = location
        };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer MapToGrpcResponse(Customer src)
    {
        var customer = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer
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

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity> MapToGrpcResponse(IEnumerable<Identity> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity MapToGrpcResponse(Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization? MapToGrpcResponse(Shared.Models.Organization? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization { Id = src.Id, Name = src.Name.ToSafeString() };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location? MapToGrpcResponse(Shared.Models.Location? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location { Id = src.Id, Name = src.Name.ToSafeString() };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team? MapToGrpcResponse(Shared.Models.Team? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team { Id = src.Id, Name = src.Name.ToSafeString() };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Desk MapToGrpcResponse(Shared.Models.Desk src)
    {
        var desk = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Desk
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Color = src.Color.ToSafeString(),
            Location = src.Location is null
                ? null
                : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location { Id = src.Location.Id, Name = src.Location.Name.ToSafeString() }
        };

        desk.OrganizationCustomTags.AddRange(MapToGrpcResponseCustomTags(src.OrganizationTags));
        desk.OrganizationZones.AddRange(MapToGrpcResponseZones(src.OrganizationTags));

        return desk;
    }

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Room MapToGrpcResponse(Shared.Models.Room src)
    {
        var room = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Room
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Color = src.Color.ToSafeString(),
            Location = src.Location is null
                ? null
                : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location { Id = src.Location.Id, Name = src.Location.Name.ToSafeString() }
        };

        room.OrganizationCustomTags.AddRange(MapToGrpcResponseCustomTags(src.OrganizationTags));
        room.OrganizationZones.AddRange(MapToGrpcResponseZones(src.OrganizationTags));

        return room;
    }

    private static IEnumerable<OrganizationCustomTag> MapToGrpcResponseCustomTags(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Custom).Select(MapToGrpcResponseCustomTag);

    private static OrganizationCustomTag MapToGrpcResponseCustomTag(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapToGrpcResponseZones(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Zone).Select(MapToGrpcResponseZone);

    private static OrganizationZone MapToGrpcResponseZone(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

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
        src is null            ? null            : new BookingOrganizationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static BookingLocationDetails? MapTo(Shared.Models.Location? src) =>
        src is null            ? null            : new BookingLocationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static BookingTeamDetails? MapTo(Shared.Models.Team? src) =>
        src is null            ? null            : new BookingTeamDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static BookingDeskDetails MapTo(Shared.Models.Desk src) =>
        new()
        {
            UniqueId = src.Id,
            Name = src.Name.ToSafeString(),
            CustomTags = MapToCustomTags(src.OrganizationTags).ToArray(),
            Zones = MapToZones(src.OrganizationTags).ToArray(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(), 
            Location = MapTo(src.Location)
        };

    private static BookingRoomDetails MapTo(Shared.Models.Room src) =>
        new()
        {
            UniqueId = src.Id,
            Name = src.Name.ToSafeString(),
            CustomTags = MapToCustomTags(src.OrganizationTags).ToArray(),
            Zones = MapToZones(src.OrganizationTags).ToArray(),
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Location = MapTo(src.Location)
        };

    private static IEnumerable<BookingOrganizationCustomTagDetails> MapToCustomTags(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Custom).Select(MapToCustomTag);

    private static BookingOrganizationCustomTagDetails MapToCustomTag(OrganizationTag src) =>
        new() { UniqueId = src.Id, Name = src.Name, Color = src.Color };

    private static IEnumerable<BookingOrganizationZoneDetails> MapToZones(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Zone).Select(MapToZone);

    private static BookingOrganizationZoneDetails MapToZone(OrganizationTag src) => new() { UniqueId = src.Id, Name = src.Name, Color = src.Color };

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
            Color = src.Color,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private static Shared.Models.Room MapTo(Room src) =>
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
            Color = src.Color,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
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

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Type = src.Type switch
            {
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                _ => throw new ArgumentOutOfRangeException()
            },
            Color = src.Color
        };
}
