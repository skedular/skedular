using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceRefundWhereInput")]
public sealed class MarketplaceRefundWhereInput
{
    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("statuses")]
    public IEnumerable<string>? Statuses { get; set; }

    [GraphQLName("requestedAtGte")]
    public DateTimeOffset? RequestedAtGte { get; set; }

    [GraphQLName("requestedAtLte")]
    public DateTimeOffset? RequestedAtLte { get; set; }
}
