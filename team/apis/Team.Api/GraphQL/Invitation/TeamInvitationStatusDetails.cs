using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("TeamInvitationStatusDetails")]
public class TeamInvitationStatusDetails
{
    [GraphQLName("type")] public InvitationStatus Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
