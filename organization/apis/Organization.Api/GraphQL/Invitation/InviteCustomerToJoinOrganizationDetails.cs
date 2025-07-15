using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Organization.Api.GraphQL.Member;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomerToJoinOrganizationDetails")]
public class InviteCustomerToJoinOrganizationDetails : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("status")] public OrganizationInvitationStatusDetails Status { get; set; } = new();
    [GraphQLName("role")] public OrganizationMemberRole Role { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("createdBy")] public CustomerDetails CreatedBy { get; set; } = new();
    [GraphQLName("invitee")] public CustomerDetails? Invitee { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
