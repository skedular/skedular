using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("MyInvitationsToJoinOrganizationsWhereInput")]
public class MyInvitationsToJoinOrganizationsWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
}
