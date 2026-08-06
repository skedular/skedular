using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("RejectInvitationToJoinOrganizationInput")]
public class RejectInvitationToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;
}
