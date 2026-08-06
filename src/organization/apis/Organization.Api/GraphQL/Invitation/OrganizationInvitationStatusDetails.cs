using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("OrganizationInvitationStatusDetails")]
public class OrganizationInvitationStatusDetails
{
    [GraphQLName("type")]
    public InvitationStatus Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
