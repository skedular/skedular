using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("AddCustomerPreferredTeamInput")]
public class AddCustomerPreferredTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
}
