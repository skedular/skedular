using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("AcceptInvitationToJoinOrganizationPayload")]
public class AcceptInvitationToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
