using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Stripe;

[GraphQLName("AddOrganizationStripeConnectAccountInput")]
public class AddOrganizationStripeConnectAccountInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("redirectUrl")] public string RedirectUrl { get; set; } = string.Empty;
}
