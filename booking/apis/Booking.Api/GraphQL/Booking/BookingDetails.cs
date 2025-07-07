using Booking.Api.GraphQL.Payment;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingDetails")]
public class BookingDetails : Node
{
    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
    [GraphQLName("notes")] public string? Notes { get; set; }
    [GraphQLName("type")] public BookingTypeDetails Type { get; set; } = new();
    [GraphQLName("paymentStatus")] public PaymentStatusDetails PaymentStatus { get; set; } = new();
    [GraphQLName("resources")] public IEnumerable<BookingResourceDetails> Resources { get; set; } = [];
    [GraphQLName("lineItems")] public IEnumerable<LineItemDetails> LineItems { get; set; } = [];
    [GraphQLName("involvedCustomers")] public IEnumerable<CustomerDetails> InvolvedCustomers { get; set; } = [];
    [GraphQLName("involvedOrganizations")] public IEnumerable<OrganizationDetails> InvolvedOrganizations { get; set; } = [];
    [GraphQLName("involvedLocations")] public IEnumerable<LocationDetails> InvolvedLocations { get; set; } = [];
    [GraphQLName("involvedTeams")] public IEnumerable<TeamDetails> InvolvedTeams { get; set; } = [];
    [GraphQLName("paidByCustomer")] public CustomerDetails? PaidByCustomer { get; set; }
    [GraphQLName("paidByOrganization")] public OrganizationDetails? PaidByOrganization { get; set; }
    [GraphQLName("createdByCustomer")] public CustomerDetails? CreatedByCustomer { get; set; }
    [GraphQLName("paymentMethod")] public PaymentMethodTypeDetails? PaymentMethod { get; set; }
    [GraphQLName("sendInvoice")] public bool? SendInvoice { get; set; }
    [GraphQLName("invoiceUrl")] public string? InvoiceUrl { get; set; }
    [GraphQLName("totalAmount")] public string? TotalAmount { get; set; }
    [GraphQLName("totalAmountToDisplay")] public string TotalAmountToDisplay { get; set; } = string.Empty;
    [GraphQLName("currency")] public string? Currency { get; set; }
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;

    [GraphQLName("lastModifiedByCustomer")]
    public CustomerDetails? LastModifiedByCustomer { get; set; }

    [GraphQLName("deletedByCustomer")] public CustomerDetails? DeletedByCustomer { get; set; }
    [GraphQLName("isPaymentRequired")] public bool IsPaymentRequired { get; set; }

    [GraphQLName("bookingCheckoutSession")]
    public BookingCheckoutSessionDetails? BookingCheckoutSession { get; set; }

    [GraphQLName("paymentExpiry")]
    public DateTimeOffset PaymentExpiry { get; set; }

    [GraphQLName("bookedOnMarketplace")] public bool BookedOnMarketplace { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
