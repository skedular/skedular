using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Relay;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Booking.Api.GraphQL;

[GraphQLName("AddBookingInput")]
public class AddBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerId")] public required string CustomerId { get; set; }
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("to")] public DateTimeOffset To { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("deskIds")] public string[] DeskIds { get; set; } = [];
    [GraphQLName("teamId")] public string? TeamId { get; set; }
}

[GraphQLName("BookingConnection")]
public class BookingConnection : Connection<BookingEdge>;

[GraphQLName("BookingCustomerDetails")]
public class BookingCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
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
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("deactivated")] public bool Deactivated { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("location")] public BookingLocationDetails? Location { get; set; }
    [GraphQLName("customTags")] public BookingOrganizationCustomTagDetails[] CustomTags { get; set; } = [];
    [GraphQLName("zones")] public BookingOrganizationZoneDetails[] Zones { get; set; } = [];
}

[GraphQLName("BookingDetails")]
public class BookingDetails : Node
{
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("to")] public DateTimeOffset To { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("customer")] public BookingCustomerDetails Customer { get; set; }
    [GraphQLName("organization")] public BookingOrganizationDetails? Organization { get; set; }
    [GraphQLName("location")] public BookingLocationDetails? Location { get; set; }
    [GraphQLName("desks")] public BookingDeskDetails[] Desks { get; set; } = [];
    [GraphQLName("team")] public BookingTeamDetails? Team { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("BookingEdge")]
public class BookingEdge : Edge<BookingDetails>;

[GraphQLName("BookingLocationDetails")]
public class BookingLocationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("BookingOrganizationCustomTagDetails")]
public class BookingOrganizationCustomTagDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("BookingOrganizationZoneDetails")]
public class BookingOrganizationZoneDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
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
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
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
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
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
    [GraphQLName("type")] public string? Type { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("organizationIds")] public string[]? OrganizationIds { get; set; }
    [GraphQLName("locationIds")] public string[]? LocationIds { get; set; }
    [GraphQLName("teamIds")] public string[]? TeamIds { get; set; }
    [GraphQLName("customerIds")] public string[]? CustomerIds { get; set; }
    [GraphQLName("includeMineOnly")] public bool? IncludeMineOnly { get; set; }

    [GraphQLName("includeFutureBookingsOnly")]
    public bool? IncludeFutureBookingsOnly { get; set; }

    [GraphQLName("combineOrganizationsLocationsTeams")]
    public bool? CombineOrganizationsLocationsTeams { get; set; }
}

[GraphQLName("DeleteBookingInput")]
public class DeleteBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
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
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("customerId")] public required string CustomerId { get; set; }
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("to")] public DateTimeOffset To { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("deskIds")] public string[] DeskIds { get; set; } = [];
    [GraphQLName("teamId")] public string? TeamId { get; set; }
}

[GraphQLName("AvailableDesksWhereInput")]
public class AvailableDesksWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("date")] public required DateTimeOffset Date { get; set; }
    [GraphQLName("deskIdsToInclude")] public required string[]? DeskIdsToInclude { get; set; }
    [GraphQLName("customTagIds")] public string[]? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public string[]? ZoneIds { get; set; }

    [GraphQLName("combineCustomTagsZones")]
    public bool? CombineCustomTagsZones { get; set; }
}

[GraphQLName("OrganizationAvailableDesksWhereInput")]
public class OrganizationAvailableDesksWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("date")] public required DateTimeOffset Date { get; set; }
}

[GraphQLName("OrganizationAvailableDesks")]
public class OrganizationAvailableDesks
{
    [GraphQLName("desksCount")] public int DesksCount { get; set; }
    [GraphQLName("availableDesksCount")] public int AvailableDesksCount { get; set; }
}
