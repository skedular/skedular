using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomersToJoinTeamInput")]
public class InviteCustomersToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
    [GraphQLName("emails")] public IEnumerable<string> Emails { get; set; } = [];
}
