using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountPayload")]
public class OrganizationStripeConnectAccountPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("account")] public OrganizationStripeConnectAccountDetails Account { get; set; } = new();
}
