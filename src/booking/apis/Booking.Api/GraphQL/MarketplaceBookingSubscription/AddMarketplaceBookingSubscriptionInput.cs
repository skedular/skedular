using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("AddMarketplaceBookingSubscriptionInput")]
public class AddMarketplaceBookingSubscriptionInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationIds")] public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationCustomDomains")]
    public IEnumerable<string>? OrganizationCustomDomains { get; set; }

    [GraphQLName("teamIds")] public IEnumerable<string>? TeamIds { get; set; } = [];
    [GraphQLName("requestedResourceIds")] public IEnumerable<string>? RequestedResourceIds { get; set; } = [];
    [GraphQLName("startedAt")] public DateTimeOffset StartedAt { get; set; }
    [GraphQLName("autoRenew")] public bool AutoRenew { get; set; }
    [GraphQLName("cancelAtPeriodEnd")] public bool CancelAtPeriodEnd { get; set; }
    [GraphQLName("paymentMethod")] public PaymentMethod PaymentMethod { get; set; }
    [GraphQLName("invoiceEmailList")] public IEnumerable<string>? InvoiceEmailList { get; set; } = [];
    [GraphQLName("quantity")] public int Quantity { get; set; }
    [GraphQLName("productVersionId")] public string ProductVersionId { get; set; } = string.Empty;
    [GraphQLName("pricingId")] public string PricingId { get; set; } = string.Empty;
    [GraphQLName("checkoutReturnUrl")] public string? CheckoutReturnUrl { get; set; }
}
