using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("AddMarketplaceBookingInput")]
public class AddMarketplaceBookingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string? Id { get; set; }

    [GraphQLName("customerIds")]
    public IEnumerable<string> CustomerIds { get; set; } = [];

    [GraphQLName("organizationIds")]
    public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationCustomDomains")]
    public IEnumerable<string>? OrganizationCustomDomains { get; set; }

    [GraphQLName("teamIds")]
    public IEnumerable<string>? TeamIds { get; set; } = [];

    [GraphQLName("from")]
    public DateTimeOffset From { get; set; }

    [GraphQLName("until")]
    public DateTimeOffset Until { get; set; }

    [GraphQLName("notes")]
    public string? Notes { get; set; }

    [GraphQLName("category")]
    public BookingCategory? Category { get; set; }

    [GraphQLName("resourceIds")]
    public IEnumerable<string>? ResourceIds { get; set; } = [];

    [GraphQLName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }

    [GraphQLName("invoiceEmailList")]
    public IEnumerable<string>? InvoiceEmailList { get; set; } = [];

    [GraphQLName("quantity")]
    public int Quantity { get; set; }

    [GraphQLName("productVersionId")]
    public string ProductVersionId { get; set; } = string.Empty;

    [GraphQLName("pricingId")]
    public string PricingId { get; set; } = string.Empty;

    [GraphQLName("checkoutReturnUrl")]
    public string? CheckoutReturnUrl { get; set; }
}
