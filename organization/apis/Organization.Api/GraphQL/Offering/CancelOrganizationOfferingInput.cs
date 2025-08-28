using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("CancelOrganizationOfferingInput")]
public class CancelOrganizationOfferingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }
}
