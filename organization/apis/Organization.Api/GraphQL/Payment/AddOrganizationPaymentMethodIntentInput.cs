using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Payment;

[GraphQLName("AddOrganizationPaymentMethodIntentInput")]
public class AddOrganizationPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }
}
