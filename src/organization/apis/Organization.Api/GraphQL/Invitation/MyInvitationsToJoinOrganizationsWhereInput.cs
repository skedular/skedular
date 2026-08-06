using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("MyInvitationsToJoinOrganizationsWhereInput")]
public class MyInvitationsToJoinOrganizationsWhereInput
{
    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("status")]
    public InvitationStatus? Status { get; set; }
}
