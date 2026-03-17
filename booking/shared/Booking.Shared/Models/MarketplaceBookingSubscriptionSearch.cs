using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

public record MarketplaceBookingSubscriptionSearchCriteria(
    DateTimeOffset? StartedAtGt,
    DateTimeOffset? StartedAtGte,
    DateTimeOffset? StartedAtLt,
    DateTimeOffset? StartedAtLte,
    DateTimeOffset? CancelledAtGt,
    DateTimeOffset? CancelledAtGte,
    DateTimeOffset? CancelledAtLt,
    DateTimeOffset? CancelledAtLte,
    DateTimeOffset? NextRenewalAtGt,
    DateTimeOffset? NextRenewalAtGte,
    DateTimeOffset? NextRenewalAtLt,
    DateTimeOffset? NextRenewalAtLte,
    string? NameContains,
    MarketplaceBookingSubscriptionStatus? Status,
    bool? IncludeMineOnly,
    ICollection<string> OrganizationIds,
    ICollection<string> OrganizationCustomDomains,
    ICollection<string> TeamIds,
    ICollection<string> CustomerIds);

public record MarketplaceBookingSubscriptionOrder(OrderDirection Direction, MarketplaceBookingSubscriptionOrderField Field);

public enum MarketplaceBookingSubscriptionOrderField
{
    StartedAt,
    CancelledAt,
    NextRenewalAt,
    Status
}
