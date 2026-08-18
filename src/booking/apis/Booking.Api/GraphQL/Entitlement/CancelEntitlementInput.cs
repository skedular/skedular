using HotChocolate;

namespace Booking.Api.GraphQL.Entitlement;

[GraphQLName("CancelEntitlementInput")]
public sealed class CancelEntitlementInput
{
    public string? ClientMutationId { get; set; }
    public string EntitlementId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
