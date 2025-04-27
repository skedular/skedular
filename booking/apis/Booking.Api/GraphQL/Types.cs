using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
// ReSharper disable ClassNeverInstantiated.Global

namespace Booking.Api.GraphQL;

[GraphQLName("AddBookingInput")]
public class AddBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; }
    [GraphQLName("organizationIds")] public IEnumerable<string> OrganizationIds { get; set; }
    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; }
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
    [GraphQLName("productVersionIds")] public IEnumerable<string> ProductVersionIds { get; set; } = [];
}

[GraphQLName("UpdateBookingInput")]
public class UpdateBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; }
    [GraphQLName("organizationIds")] public IEnumerable<string> OrganizationIds { get; set; }
    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; }
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
    [GraphQLName("productVersionIds")] public IEnumerable<string> ProductVersionIds { get; set; } = [];
}

[GraphQLName("DeleteBookingInput")]
public class DeleteBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("BookingConnection")]
public class BookingConnection : Enterprise.Shared.GraphQL.Types.Connection<BookingEdge>;

[GraphQLName("Booking_CustomerDetails")]
public class CustomerDetails
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
    [GraphQLName("resources")] public IEnumerable<BookingResourceDetails> Resources { get; set; } = [];
    [GraphQLName("productVersions")] public IEnumerable<ProductVersionDetails> ProductVersions { get; set; } = [];
    [GraphQLName("involvedCustomers")] public IEnumerable<CustomerDetails> InvolvedCustomers { get; set; }
    [GraphQLName("involvedOrganizations")] public IEnumerable<OrganizationDetails> InvolvedOrganizations { get; set; }
    [GraphQLName("involvedLocations")] public IEnumerable<LocationDetails> InvolvedLocations { get; set; }
    [GraphQLName("involvedTeams")] public IEnumerable<TeamDetails> InvolvedTeams { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("BookingEdge")]
public class BookingEdge(BookingDetails node, string cursor) : Edge<BookingDetails>(node, cursor);

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

[GraphQLName("AvailableResourcesWhereInput")]
public class AvailableResourcesWhereInput
{
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("from")] public required DateTimeOffset From { get; set; }
    [GraphQLName("until")] public required DateTimeOffset Until { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("resourceIdsToInclude")] public IEnumerable<string>? ResourceIdsToInclude { get; set; }
    [GraphQLName("productId")] public string? ProductId { get; set; }
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
    [GraphQLName("customers")] public IEnumerable<CustomerDetails> Customers { get; set; } = [];
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

[GraphQLName("Booking_ProductVersionDetails")]
public class ProductVersionDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("price")] public required string Price { get; set; }
    [GraphQLName("priceToDisplay")] public required string PriceToDisplay { get; set; }
    [GraphQLName("priceUnit")] public PriceUnitDetails PriceUnit { get; set; }
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; }
    [GraphQLName("minDurationMinutes")] public int? MinDurationMinutes { get; set; }
    [GraphQLName("maxDurationMinutes")] public int? MaxDurationMinutes { get; set; }

    [GraphQLName("bookAllLocationResources")]
    public bool BookAllLocationResources { get; set; }

    [GraphQLName("recurrenceWindowDays")] public int RecurrenceWindowDays { get; set; }

    [GraphQLName("requireConsecutiveDays")]
    public bool RequireConsecutiveDays { get; set; }

    [GraphQLName("maxBookingSpreadDays")] public int? MaxBookingSpreadDays { get; set; }

    [GraphQLName("numberOfResourcesToBook")]
    public int NumberOfResourcesToBook { get; set; }

    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("locationTags")] public IEnumerable<OrganizationTagDetails> LocationTags { get; set; } = [];

    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; }
}

[GraphQLName("Booking_CurrencyDetails")]
public class CurrencyDetails
{
    [GraphQLName("type")] public Currency Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("Booking_PriceUnitDetails")]
public class PriceUnitDetails
{
    [GraphQLName("type")] public PriceUnit Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("Booking_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("tagType")] public string? TagType { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
