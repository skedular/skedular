using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomersToJoinOrganizationPayload")]
public class InviteCustomersToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
