using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("DeleteOrganizationStripeConnectAccountInput")]
public class DeleteOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
