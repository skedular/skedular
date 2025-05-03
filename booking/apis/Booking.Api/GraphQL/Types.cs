using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;

// ReSharper disable ClassNeverInstantiated.Global

namespace Booking.Api.GraphQL;

[GraphQLName("LineItemInput")]
public class LineItemInput
{
    [GraphQLName("productVersionId")] public string ProductVersionId { get; set; } = string.Empty;
    [GraphQLName("quantity")] public int Quantity { get; set; }
}

[GraphQLName("LineItemDetails")]
public class LineItemDetails
{
    [GraphQLName("productVersion")] public ProductVersionDetails ProductVersionDetails { get; set; } = new();
    [GraphQLName("quantity")] public int Quantity { get; set; }
}

[GraphQLName("AddBookingInput")]
public class AddBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string> OrganizationIds { get; set; } = [];
    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; } = [];
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
    [GraphQLName("lineItems")] public IEnumerable<LineItemInput> LineItems { get; set; } = [];
}

[GraphQLName("UpdateBookingInput")]
public class UpdateBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string> OrganizationIds { get; set; } = [];
    [GraphQLName("teamIds")] public IEnumerable<string> TeamIds { get; set; } = [];
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("resourceIds")] public IEnumerable<string> ResourceIds { get; set; } = [];
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
    [GraphQLName("type")] public BookingTypeDetails Type { get; set; } = new();
    [GraphQLName("status")] public BookingStatusDetails Status { get; set; } = new();
    [GraphQLName("resources")] public IEnumerable<BookingResourceDetails> Resources { get; set; } = [];
    [GraphQLName("lineItems")] public IEnumerable<LineItemDetails> LineItems { get; set; } = [];
    [GraphQLName("involvedCustomers")] public IEnumerable<CustomerDetails> InvolvedCustomers { get; set; } = [];
    [GraphQLName("involvedOrganizations")] public IEnumerable<OrganizationDetails> InvolvedOrganizations { get; set; } = [];
    [GraphQLName("involvedLocations")] public IEnumerable<LocationDetails> InvolvedLocations { get; set; } = [];
    [GraphQLName("involvedTeams")] public IEnumerable<TeamDetails> InvolvedTeams { get; set; } = [];
    [GraphQLName("paidByCustomer")] public CustomerDetails? PaidByCustomer { get; set; }
    [GraphQLName("paidByOrganization")] public OrganizationDetails? PaidByOrganization { get; set; }
    [GraphQLName("createdByCustomer")] public CustomerDetails? CreatedByCustomer { get; set; }

    [GraphQLName("lastModifiedByCustomer")]
    public CustomerDetails? LastModifiedByCustomer { get; set; }

    [GraphQLName("deletedByCustomer")] public CustomerDetails? DeletedByCustomer { get; set; }
    [GraphQLName("isPaymentRequired")] public bool IsPaymentRequired { get; set; }

    [GraphQLName("bookingCheckoutSession")]
    public BookingCheckoutSessionDetails? BookingCheckoutSession { get; set; }

    [GraphQLName("bookedOnMarketplace")] public bool BookedOnMarketplace { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("BookingEdge")]
public class BookingEdge(BookingDetails node, string cursor) : Edge<BookingDetails>(node, cursor);

[GraphQLName("Booking_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("Booking_OrganizationCustomTagDetails")]
public class OrganizationCustomTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("Booking_OrganizationZoneDetails")]
public class OrganizationZoneDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
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
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("BookingPayload")]
public class BookingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("booking")] public BookingDetails Booking { get; set; } = new();
}

[GraphQLName("Booking_TeamDetails")]
public class TeamDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("BookingWhereInput")]
public class BookingWhereInput
{
    [GraphQLName("fromGt")] public DateTimeOffset? FromGt { get; set; }
    [GraphQLName("fromGte")] public DateTimeOffset? FromGte { get; set; }
    [GraphQLName("fromLt")] public DateTimeOffset? FromLt { get; set; }
    [GraphQLName("fromLte")] public DateTimeOffset? FromLte { get; set; }
    [GraphQLName("toGt")] public DateTimeOffset? ToGt { get; set; }
    [GraphQLName("toGte")] public DateTimeOffset? ToGte { get; set; }
    [GraphQLName("toLt")] public DateTimeOffset? ToLt { get; set; }
    [GraphQLName("toLte")] public DateTimeOffset? ToLte { get; set; }
    [GraphQLName("notesContains")] public string? NotesContains { get; set; }
    [GraphQLName("type")] public BookingType? Type { get; set; }
    [GraphQLName("status")] public BookingStatus? Status { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }
    [GraphQLName("locationIds")] public IEnumerable<string>? LocationIds { get; set; }
    [GraphQLName("teamIds")] public IEnumerable<string>? TeamIds { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string>? CustomerIds { get; set; }
    [GraphQLName("includeMineOnly")] public bool? IncludeMineOnly { get; set; }

    [GraphQLName("includeFutureBookingsOnly")]
    public bool? IncludeFutureBookingsOnly { get; set; }
}

[GraphQLName("OrganizationBookingPermissions")]
public class OrganizationBookingPermissions
{
    [GraphQLName("canAddBooking")] public bool CanAddBooking { get; set; }
    [GraphQLName("canUpdateBooking")] public bool CanUpdateBooking { get; set; }
    [GraphQLName("canDeleteBooking")] public bool CanDeleteBooking { get; set; }
}

[GraphQLName("TeamBookingPermissions")]
public class TeamBookingPermissions
{
    [GraphQLName("canAddBooking")] public bool CanAddBooking { get; set; }
    [GraphQLName("canUpdateBooking")] public bool CanUpdateBooking { get; set; }
    [GraphQLName("canDeleteBooking")] public bool CanDeleteBooking { get; set; }
}

[GraphQLName("AvailableResourcesWhereInput")]
public class AvailableResourcesWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("locationId")] public string? LocationId { get; set; }
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("resourceIdsToInclude")] public IEnumerable<string>? ResourceIdsToInclude { get; set; }
    [GraphQLName("productId")] public string? ProductId { get; set; }
}

[GraphQLName("BookingResourceDetails")]
public class BookingResourceDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
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
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
}

[GraphQLName("Booking_ProductVersionDetails")]
public class ProductVersionDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("price")] public string Price { get; set; } = string.Empty;
    [GraphQLName("priceToDisplay")] public string PriceToDisplay { get; set; } = string.Empty;
    [GraphQLName("priceUnit")] public PriceUnitDetails PriceUnit { get; set; } = new();
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
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

[GraphQLName("BookingTypeDetails")]
public class BookingTypeDetails
{
    [GraphQLName("type")] public BookingType Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("BookingStatusDetails")]
public class BookingStatusDetails
{
    [GraphQLName("type")] public BookingStatus Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("BookingCheckoutSessionDetails")]
public class BookingCheckoutSessionDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("checkoutUrl")] public string CheckoutUrl { get; set; } = string.Empty;
    [GraphQLName("paymentStatus")] public PaymentStatus PaymentStatus { get; set; }
}
