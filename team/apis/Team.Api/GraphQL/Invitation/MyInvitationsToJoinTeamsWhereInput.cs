using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("MyInvitationsToJoinTeamsWhereInput")]
public class MyInvitationsToJoinTeamsWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("teamId")] public string? TeamId { get; set; }
    [GraphQLName("status")] public InvitationStatus? Status { get; set; }
}
