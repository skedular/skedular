using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Team.Api.GraphQL.Team;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomerToJoinTeamDetails")]
public class InviteCustomerToJoinTeamDetails : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("status")] public TeamInvitationStatusDetails Status { get; set; } = new();
    [GraphQLName("role")] public TeamMemberRole Role { get; set; }
    [GraphQLName("team")] public TeamDetails Team { get; set; } = new();
    [GraphQLName("createdBy")] public CustomerDetails CreatedBy { get; set; } = new();
    [GraphQLName("invitee")] public CustomerDetails? Invitee { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
