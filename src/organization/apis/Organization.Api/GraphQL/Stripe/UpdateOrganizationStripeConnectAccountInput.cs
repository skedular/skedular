using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("UpdateOrganizationStripeConnectAccountInput")]
public class UpdateOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("fieldsToUpdate")]
    public HashSet<OrganizationStripeConnectAccountPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("name")]
    public string? Name { get; set; }
}
