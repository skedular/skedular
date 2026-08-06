using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ResolveMarketplaceExternalRefundReconciliationInput")]
public class ResolveMarketplaceExternalRefundReconciliationInput
{
    [GraphQLName("organizationId")]
    public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("provider")]
    public string Provider { get; set; } = string.Empty;

    [GraphQLName("externalRefundId")]
    public string ExternalRefundId { get; set; } = string.Empty;

    [GraphQLName("status")]
    public string Status { get; set; } = string.Empty;

    [GraphQLName("reason")]
    public string Reason { get; set; } = string.Empty;

    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
