using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("CancelInvitationToJoinTeamInput")]
public class CancelInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
