using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("UpdateOrganizationStripeConnectAccountInput")]
public class UpdateOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
