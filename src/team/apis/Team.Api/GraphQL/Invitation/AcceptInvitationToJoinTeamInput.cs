using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("AcceptInvitationToJoinTeamInput")]
public class AcceptInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;
}
