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
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
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

[GraphQLName("BookingDetails")]
public class BookingDetails : Node
{
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("customer")] public BookingCustomerDetails Customer { get; set; }
    [GraphQLName("organization")] public OrganizationDetails? Organization { get; set; }
    [GraphQLName("location")] public LocationDetails? Location { get; set; }
    [GraphQLName("resources")] public IEnumerable<BookingResourceDetails> Resources { get; set; } = [];
    [GraphQLName("team")] public TeamDetails? Team { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("BookingEdge")]
public class BookingEdge : Edge<BookingDetails>;

[GraphQLName("Booking_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("Booking_OrganizationCustomTagDetails")]
public class OrganizationCustomTagDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("Booking_OrganizationZoneDetails")]
public class OrganizationZoneDetails
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

[GraphQLName("Booking_OrganizationDetails")]
public class OrganizationDetails
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

[GraphQLName("Booking_TeamDetails")]
public class TeamDetails
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
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }
    [GraphQLName("locationIds")] public IEnumerable<string>? LocationIds { get; set; }
    [GraphQLName("teamIds")] public IEnumerable<string>? TeamIds { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string>? CustomerIds { get; set; }
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
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
    [GraphQLName("teamId")] public string? TeamId { get; set; }
}

[GraphQLName("AvailableResourcesWhereInput")]
public class AvailableResourcesWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("from")] public required DateTimeOffset From { get; set; }
    [GraphQLName("until")] public required DateTimeOffset Until { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("resourceIdsToInclude")] public IEnumerable<string>? ResourceIdsToInclude { get; set; }
}

[GraphQLName("BookingResourceDetails")]
public class BookingResourceDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("location")] public LocationDetails? Location { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationCustomTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationZoneDetails> Zones { get; set; } = [];
    [GraphQLName("customers")] public IEnumerable<BookingCustomerDetails> Customers { get; set; } = [];
}

[GraphQLName("OrganizationAvailableResources")]
public class OrganizationAvailableResources
{
    [GraphQLName("resourcesCount")] public int ResourcesCount { get; set; }

    [GraphQLName("availableResourcesCount")]
    public int AvailableResourcesCount { get; set; }
}

[GraphQLName("OrganizationAvailableResourcesWhereInput")]
public class OrganizationAvailableResourcesWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("from")] public required DateTimeOffset From { get; set; }
    [GraphQLName("until")] public required DateTimeOffset Until { get; set; }
}
