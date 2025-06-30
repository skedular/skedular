using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("SetOrganizationStripeConnectAccountAsDefaultInput")]
public class SetOrganizationStripeConnectAccountAsDefaultInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
