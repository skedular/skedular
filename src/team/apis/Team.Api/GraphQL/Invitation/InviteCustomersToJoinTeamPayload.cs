using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomersToJoinTeamPayload")]
public class InviteCustomersToJoinTeamPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("invitesCustomersToJoinTeam")]
    public IEnumerable<InviteCustomerToJoinTeamDetails> InvitesCustomersToJoinTeam { get; set; } = [];
}
