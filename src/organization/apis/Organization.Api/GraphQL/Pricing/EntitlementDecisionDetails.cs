using Api.Shared.Services.Offering;
using HotChocolate;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("EntitlementDecision")]
public class EntitlementDecisionDetails
{
    [GraphQLName("isAllowed")]
    public bool IsAllowed { get; set; }

    [GraphQLName("reasonCode")]
    public EntitlementReasonCode ReasonCode { get; set; }

    [GraphQLName("message")]
    public string? Message { get; set; }
}
