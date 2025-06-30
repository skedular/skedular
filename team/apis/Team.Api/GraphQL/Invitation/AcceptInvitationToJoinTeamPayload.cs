using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("AcceptInvitationToJoinTeamPayload")]
public class AcceptInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
