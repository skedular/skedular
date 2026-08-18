using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.EntitlementPurchase;

public sealed class CreateEntitlementPurchaseInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")]
    public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("productVersionId")]
    public string ProductVersionId { get; set; } = string.Empty;

    [GraphQLName("pricingId")]
    public string PricingId { get; set; } = string.Empty;

    [GraphQLName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }

    [GraphQLName("autoRenew")]
    public bool AutoRenew { get; set; }

    [GraphQLName("serviceStartAt")]
    public DateTimeOffset ServiceStartAt { get; set; }

    [GraphQLName("checkoutReturnUrl")]
    public string? CheckoutReturnUrl { get; set; }

    [GraphQLName("invoiceEmailList")]
    public IEnumerable<string> InvoiceEmailList { get; set; } = [];
}
