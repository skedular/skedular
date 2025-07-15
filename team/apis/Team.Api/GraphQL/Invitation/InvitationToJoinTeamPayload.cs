using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("InvitationToJoinTeamPayload")]
public class InvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("inviteCustomersToJoinTeam")]
    public InviteCustomerToJoinTeamDetails InviteCustomerToJoinTeam { get; set; } = new();
}
