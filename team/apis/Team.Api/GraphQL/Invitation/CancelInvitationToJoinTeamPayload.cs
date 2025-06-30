using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("CancelInvitationToJoinTeamPayload")]
public class CancelInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
