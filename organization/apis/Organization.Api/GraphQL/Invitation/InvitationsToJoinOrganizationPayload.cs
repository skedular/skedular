using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("InvitationsToJoinOrganizationPayload")]
public class InvitationsToJoinOrganizationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("invitesCustomersToJoinOrganization")]
    public IEnumerable<InviteCustomerToJoinOrganizationDetails> InvitesCustomersToJoinOrganization { get; set; } = [];
}
