using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("CancelInvitationToJoinOrganizationPayload")]
public class CancelInvitationToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
