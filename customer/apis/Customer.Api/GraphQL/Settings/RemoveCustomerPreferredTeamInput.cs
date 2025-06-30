using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("RemoveCustomerPreferredTeamInput")]
public class RemoveCustomerPreferredTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
}
