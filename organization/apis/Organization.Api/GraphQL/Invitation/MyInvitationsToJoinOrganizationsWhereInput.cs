using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("MyInvitationsToJoinOrganizationsWhereInput")]
public class MyInvitationsToJoinOrganizationsWhereInput
{
    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("status")] public InvitationStatus? Status { get; set; }
}
