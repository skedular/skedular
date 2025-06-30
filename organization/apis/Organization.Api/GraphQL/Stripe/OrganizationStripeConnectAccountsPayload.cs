using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountsPayload")]
public class OrganizationStripeConnectAccountsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("accounts")] public IEnumerable<OrganizationStripeConnectAccountDetails> Accounts { get; set; } = [];
}
