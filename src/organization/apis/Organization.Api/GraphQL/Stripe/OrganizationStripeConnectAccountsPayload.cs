using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("OrganizationStripeConnectAccountsPayload")]
public class OrganizationStripeConnectAccountsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationStripeConnectAccounts")]
    public IEnumerable<OrganizationStripeConnectAccountDetails> OrganizationStripeConnectAccounts { get; set; } = [];
}
