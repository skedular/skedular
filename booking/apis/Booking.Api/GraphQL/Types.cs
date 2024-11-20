using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL;

[GraphQLName("AddBookingInput")]
public class AddBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("customerId")] public string CustomerId { get; set; }

    [GraphQLName("from")] public DateTimeOffset From { get; set; }

    [GraphQLName("to")] public DateTimeOffset To { get; set; }

    [GraphQLName("notes")] public string? Notes { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("locationId")] public string? LocationId { get; set; }

    [GraphQLName("deskIds")] public string[] DeskIds { get; set; }

    [GraphQLName("teamId")] public string? TeamId { get; set; }
}

[GraphQLName("BookingConnection")]
public class BookingConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public BookingEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("BookingCustomerDetails")]
public class BookingCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string? Name { get; set; }

    [GraphQLName("givenName")] public string? GivenName { get; set; }

    [GraphQLName("middleName")] public string? MiddleName { get; set; }

    [GraphQLName("familyName")] public string? FamilyName { get; set; }

    [GraphQLName("photoUrl")] public string? PhotoUrl { get; set; }

    [GraphQLName("photoUrl24")] public string? PhotoUrl24 { get; set; }

    [GraphQLName("photoUrl32")] public string? PhotoUrl32 { get; set; }

    [GraphQLName("photoUrl48")] public string? PhotoUrl48 { get; set; }

    [GraphQLName("photoUrl72")] public string? PhotoUrl72 { get; set; }

    [GraphQLName("photoUrl192")] public string? PhotoUrl192 { get; set; }

    [GraphQLName("photoUrl512")] public string? PhotoUrl512 { get; set; }
}

[GraphQLName("BookingDeskDetails")]
public class BookingDeskDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("locationTags")] public BookingLocationTagDetails[] LocationTags { get; set; }
}

[GraphQLName("BookingDetails")]
public class BookingDetails : Node
{
    [GraphQLName("from")] public DateTimeOffset From { get; set; }

    [GraphQLName("to")] public DateTimeOffset To { get; set; }

    [GraphQLName("notes")] public string? Notes { get; set; }

    [GraphQLName("customer")] public BookingCustomerDetails Customer { get; set; }

    [GraphQLName("organization")] public BookingOrganizationDetails? Organization { get; set; }

    [GraphQLName("location")] public BookingLocationDetails? Location { get; set; }

    [GraphQLName("desks")] public BookingDeskDetails[] Desks { get; set; }

    [GraphQLName("team")] public BookingTeamDetails? Team { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("BookingEdge")]
public class BookingEdge
{
    [GraphQLName("node")] public BookingDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

[GraphQLName("BookingLocationDetails")]
public class BookingLocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }
}

[GraphQLName("BookingLocationTagDetails")]
public class BookingLocationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("tagType")] public string? TagType { get; set; }
}

public enum BookingOrderField
{
    From,
    To,
    Notes,
    Name,
    GivenName,
    MiddleName,
    FamilyName,
    OrganizationName,
    LocationName,
    TeamName
}

[GraphQLName("BookingOrderInput")]
public class BookingOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public BookingOrderField Field { get; set; }
}

[GraphQLName("BookingOrganizationDetails")]
public class BookingOrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }
}

[GraphQLName("BookingPayload")]
public class BookingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("booking")] public BookingDetails Booking { get; set; }
}

[GraphQLName("BookingTeamDetails")]
public class BookingTeamDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }
}

[GraphQLName("BookingWhereInput")]
public class BookingWhereInput
{
    [GraphQLName("fromGT")] public DateTimeOffset? FromGT { get; set; }

    [GraphQLName("fromGTE")] public DateTimeOffset? FromGTE { get; set; }

    [GraphQLName("fromLT")] public DateTimeOffset? FromLT { get; set; }

    [GraphQLName("fromLTE")] public DateTimeOffset? FromLTE { get; set; }

    [GraphQLName("toGT")] public DateTimeOffset? ToGT { get; set; }

    [GraphQLName("toGTE")] public DateTimeOffset? ToGTE { get; set; }

    [GraphQLName("toLT")] public DateTimeOffset? ToLT { get; set; }

    [GraphQLName("toLTE")] public DateTimeOffset? ToLTE { get; set; }

    [GraphQLName("notesContains")] public string? NotesContains { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }

    [GraphQLName("organizationIds")] public string[]? OrganizationIds { get; set; }

    [GraphQLName("locationIds")] public string[]? LocationIds { get; set; }

    [GraphQLName("teamIds")] public string[]? TeamIds { get; set; }

    [GraphQLName("includeMineOnly")] public bool? IncludeMineOnly { get; set; }
    
    [GraphQLName("includeFutureBookingsOnly")] public bool? IncludeFutureBookingsOnly { get; set; }
}

[GraphQLName("DeleteBookingInput")]
public class DeleteBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("LocationBookingPermissions")]
public class LocationBookingPermissions
{
    [GraphQLName("canAddBooking")] public bool CanAddBooking { get; set; }

    [GraphQLName("canUpdateBooking")] public bool CanUpdateBooking { get; set; }

    [GraphQLName("canDeleteBooking")] public bool CanDeleteBooking { get; set; }

    [GraphQLName("canAddBookingOnBehalf")] public bool CanAddBookingOnBehalf { get; set; }

    [GraphQLName("canUpdateBookingOnBehalf")]
    public bool CanUpdateBookingOnBehalf { get; set; }

    [GraphQLName("canDeleteBookingOnBehalf")]
    public bool CanDeleteBookingOnBehalf { get; set; }
}

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; }
}

public enum OrderDirection
{
    Ascending,
    Descending
}

[GraphQLName("OrganizationBookingPermissions")]
public class OrganizationBookingPermissions
{
    [GraphQLName("canAddBooking")] public bool CanAddBooking { get; set; }

    [GraphQLName("canUpdateBooking")] public bool CanUpdateBooking { get; set; }

    [GraphQLName("canDeleteBooking")] public bool CanDeleteBooking { get; set; }

    [GraphQLName("canAddBookingOnBehalf")] public bool CanAddBookingOnBehalf { get; set; }

    [GraphQLName("canUpdateBookingOnBehalf")]
    public bool CanUpdateBookingOnBehalf { get; set; }

    [GraphQLName("canDeleteBookingOnBehalf")]
    public bool CanDeleteBookingOnBehalf { get; set; }
}

[GraphQLName("PageInfo")]
public class PageInfo
{
    [GraphQLName("hasNextPage")] public bool HasNextPage { get; set; }

    [GraphQLName("hasPreviousPage")] public bool HasPreviousPage { get; set; }

    [GraphQLName("startCursor")] public string? StartCursor { get; set; }

    [GraphQLName("endCursor")] public string? EndCursor { get; set; }
}

[GraphQLName("TeamBookingPermissions")]
public class TeamBookingPermissions
{
    [GraphQLName("canAddBooking")] public bool CanAddBooking { get; set; }

    [GraphQLName("canUpdateBooking")] public bool CanUpdateBooking { get; set; }

    [GraphQLName("canDeleteBooking")] public bool CanDeleteBooking { get; set; }

    [GraphQLName("canAddBookingOnBehalf")] public bool CanAddBookingOnBehalf { get; set; }

    [GraphQLName("canUpdateBookingOnBehalf")]
    public bool CanUpdateBookingOnBehalf { get; set; }

    [GraphQLName("canDeleteBookingOnBehalf")]
    public bool CanDeleteBookingOnBehalf { get; set; }
}

[GraphQLName("UpdateBookingInput")]
public class UpdateBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("customerId")] public string CustomerId { get; set; }

    [GraphQLName("from")] public DateTimeOffset From { get; set; }

    [GraphQLName("to")] public DateTimeOffset To { get; set; }

    [GraphQLName("notes")] public string? Notes { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("locationId")] public string? LocationId { get; set; }

    [GraphQLName("deskIds")] public string[] DeskIds { get; set; }

    [GraphQLName("teamId")] public string? TeamId { get; set; }
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
