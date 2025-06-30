using HotChocolate;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("DeleteOrganizationStripeConnectAccountsInput")]
public class DeleteOrganizationStripeConnectAccountsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
