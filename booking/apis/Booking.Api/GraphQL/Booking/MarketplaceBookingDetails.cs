using Booking.Api.GraphQL.Payment;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceBookingDetails")]
public class MarketplaceBookingDetails : Node
{
    [GraphQLName("lineItems")] public IEnumerable<LineItemDetails> LineItems { get; set; } = [];
    [GraphQLName("paidByCustomerId")] public string? PaidByCustomerId { get; set; }
    [GraphQLName("paidByOrganizationId")] public string? PaidByOrganizationId { get; set; }

    [GraphQLName("paidByOrganizationUniqueAlphanumericName")]
    public string? PaidByOrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("paymentMethod")] public PaymentMethodTypeDetails PaymentMethod { get; set; } = new();
    [GraphQLName("totalAmountExcludeTax")] public decimal? TotalAmountExcludeTax { get; set; }

    [GraphQLName("totalAmountExcludeTaxToDisplay")]
    public string TotalAmountExcludeTaxToDisplay { get; set; } = string.Empty;

    [GraphQLName("taxAmount")] public decimal? TaxAmount { get; set; }
    [GraphQLName("taxAmountToDisplay")] public string TaxAmountToDisplay { get; set; } = string.Empty;
    [GraphQLName("taxRatePercentage")] public decimal? TaxRatePercentage { get; set; }

    [GraphQLName("taxRatePercentageToDisplay")]
    public string TaxRatePercentageToDisplay { get; set; } = string.Empty;

    [GraphQLName("totalAmount")] public decimal? TotalAmount { get; set; }
    [GraphQLName("totalAmountToDisplay")] public string TotalAmountToDisplay { get; set; } = string.Empty;
    [GraphQLName("currency")] public string? Currency { get; set; }
    [GraphQLName("currencyToDisplay")] public string CurrencyToDisplay { get; set; } = string.Empty;
    [GraphQLName("invoiceUrl")] public string? InvoiceUrl { get; set; }
    [GraphQLName("invoiceNumber")] public string? InvoiceNumber { get; set; }
    [GraphQLName("invoiceEmailList")] public IEnumerable<string> InvoiceEmailList { get; set; } = [];
    [GraphQLName("isPaymentRequired")] public bool IsPaymentRequired { get; set; }

    [GraphQLName("bookingCheckoutSession")]
    public BookingCheckoutSessionDetails? BookingCheckoutSession { get; set; }

    [GraphQLName("paymentExpiry")] public DateTimeOffset PaymentExpiry { get; set; }
    [GraphQLName("paymentStatus")] public PaymentStatusDetails PaymentStatus { get; set; } = new();
}

[ObjectType<MarketplaceBookingDetails>]
public static partial class MarketplaceBookingDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<MarketplaceBookingDetails> descriptor)
    {
        descriptor.Ignore(item => item.PaidByCustomerId);
        descriptor.Ignore(item => item.PaidByOrganizationId);
        descriptor.Ignore(item => item.PaidByOrganizationUniqueAlphanumericName);
    }

    public static CustomerDetails? GetPaidByCustomer([Parent] MarketplaceBookingDetails item) => string.IsNullOrWhiteSpace(item.PaidByCustomerId)
        ? null
        : new CustomerDetails(item.PaidByCustomerId);

    public static OrganizationDetails? GetPaidByOrganization([Parent] MarketplaceBookingDetails item) =>
        string.IsNullOrWhiteSpace(item.PaidByOrganizationId)
            ? null
            : new OrganizationDetails(item.PaidByOrganizationId, item.PaidByOrganizationUniqueAlphanumericName.ToSafeString());
}
