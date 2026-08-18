using HotChocolate;

namespace Booking.Api.GraphQL.Entitlement;

[GraphQLName("SetEntitlementRenewalPolicyInput")]
public sealed class SetEntitlementRenewalPolicyInput
{
    public string? ClientMutationId { get; set; }
    public string EntitlementId { get; set; } = string.Empty;
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
}

[GraphQLName("SetEntitlementRenewalPolicyPayload")]
public sealed class SetEntitlementRenewalPolicyPayload
{
    public string? ClientMutationId { get; set; }
    public EntitlementDetails? Entitlement { get; set; }
    public string? Error { get; set; }
}
