using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("InvitationsToJoinTeamPayload")]
public class InvitationsToJoinTeamPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("invitesCustomersToJoinTeam")]
    public IEnumerable<InviteCustomerToJoinTeamDetails> InvitesCustomersToJoinTeam { get; set; } = [];
}
