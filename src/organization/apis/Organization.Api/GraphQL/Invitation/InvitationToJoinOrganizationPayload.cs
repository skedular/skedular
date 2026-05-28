using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("InvitationToJoinOrganizationPayload")]
public class InvitationToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("inviteCustomerToJoinOrganization")]
    public InviteCustomerToJoinOrganizationDetails InviteCustomerToJoinOrganization { get; set; } = new();
}
