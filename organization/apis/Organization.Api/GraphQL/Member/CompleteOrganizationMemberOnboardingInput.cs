using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("CompleteOrganizationMemberOnboardingInput")]
public class CompleteOrganizationMemberOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }
}
