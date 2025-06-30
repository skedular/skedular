using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("RejectInvitationToJoinTeamPayload")]
public class RejectInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
