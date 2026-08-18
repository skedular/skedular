using HotChocolate;

namespace Booking.Api.GraphQL.Entitlement;

[GraphQLName("CancelEntitlementPayload")]
public sealed class CancelEntitlementPayload
{
    public string? ClientMutationId { get; set; }
    public EntitlementDetails? Entitlement { get; set; }
    public string? Error { get; set; }
}
