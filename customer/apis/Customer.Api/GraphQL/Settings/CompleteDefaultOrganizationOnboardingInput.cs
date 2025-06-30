using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CompleteDefaultOrganizationOnboardingInput")]
public class CompleteDefaultOrganizationOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
