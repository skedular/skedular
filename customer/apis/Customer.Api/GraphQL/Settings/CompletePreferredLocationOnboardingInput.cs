using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CompletePreferredLocationOnboardingInput")]
public class CompletePreferredLocationOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
