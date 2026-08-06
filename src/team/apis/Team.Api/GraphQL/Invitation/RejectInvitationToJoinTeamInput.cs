using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("RejectInvitationToJoinTeamInput")]
public class RejectInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;
}
