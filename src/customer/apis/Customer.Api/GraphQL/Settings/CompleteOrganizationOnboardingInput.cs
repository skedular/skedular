using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CompleteOrganizationOnboardingInput")]
public class CompleteOrganizationOnboardingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
