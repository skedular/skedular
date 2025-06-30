using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CompleteTeamOnboardingInput")]
public class CompleteTeamOnboardingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
