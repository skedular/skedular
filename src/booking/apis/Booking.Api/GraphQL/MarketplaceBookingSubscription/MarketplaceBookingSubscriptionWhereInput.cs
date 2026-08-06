using Api.Shared.Services.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("MarketplaceBookingSubscriptionWhereInput")]
public class MarketplaceBookingSubscriptionWhereInput
{
    [GraphQLName("startedAtGt")]
    public DateTimeOffset? StartedAtGt { get; set; }

    [GraphQLName("startedAtGte")]
    public DateTimeOffset? StartedAtGte { get; set; }

    [GraphQLName("startedAtLt")]
    public DateTimeOffset? StartedAtLt { get; set; }

    [GraphQLName("startedAtLte")]
    public DateTimeOffset? StartedAtLte { get; set; }

    [GraphQLName("cancelledAtGt")]
    public DateTimeOffset? CancelledAtGt { get; set; }

    [GraphQLName("cancelledAtGte")]
    public DateTimeOffset? CancelledAtGte { get; set; }

    [GraphQLName("cancelledAtLt")]
    public DateTimeOffset? CancelledAtLt { get; set; }

    [GraphQLName("cancelledAtLte")]
    public DateTimeOffset? CancelledAtLte { get; set; }

    [GraphQLName("nextRenewalAtGt")]
    public DateTimeOffset? NextRenewalAtGt { get; set; }

    [GraphQLName("nextRenewalAtGte")]
    public DateTimeOffset? NextRenewalAtGte { get; set; }

    [GraphQLName("nextRenewalAtLt")]
    public DateTimeOffset? NextRenewalAtLt { get; set; }

    [GraphQLName("nextRenewalAtLte")]
    public DateTimeOffset? NextRenewalAtLte { get; set; }

    [GraphQLName("status")]
    public MarketplaceBookingSubscriptionStatus? Status { get; set; }

    [GraphQLName("statuses")]
    public IEnumerable<MarketplaceBookingSubscriptionStatus>? Statuses { get; set; }

    [GraphQLName("paymentStatuses")]
    public IEnumerable<PaymentStatus>? PaymentStatuses { get; set; }

    [GraphQLName("nameContains")]
    public string? NameContains { get; set; }

    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("teamIds")]
    public IEnumerable<string>? TeamIds { get; set; }

    [GraphQLName("customerIds")]
    public IEnumerable<string>? CustomerIds { get; set; }

    [GraphQLName("includeMineOnly")]
    public bool? IncludeMineOnly { get; set; }
}
