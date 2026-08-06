using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CompleteLocationOnboardingInput")]
public class CompleteLocationOnboardingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
