using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("CompletePreferredZoneOnboardingInput")]
public class CompletePreferredZoneOnboardingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
