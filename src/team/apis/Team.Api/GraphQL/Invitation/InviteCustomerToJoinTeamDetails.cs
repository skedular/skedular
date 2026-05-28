using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.GraphQL.Member;
using Team.Api.GraphQL.Team;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomerToJoinTeamDetails")]
public class InviteCustomerToJoinTeamDetails : Node
{
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("status")] public TeamInvitationStatusDetails Status { get; set; } = new();
    [GraphQLName("role")] public TeamMemberRole Role { get; set; }
    [GraphQLName("team")] public TeamDetails Team { get; set; } = new();
    [GraphQLName("createdById")] public string CreatedById { get; set; } = string.Empty;
    [GraphQLName("inviteeId")] public string? InviteeId { get; set; }
}

[ObjectType<InviteCustomerToJoinTeamDetails>]
public static partial class InviteCustomerToJoinTeamDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<InviteCustomerToJoinTeamDetails> descriptor)
    {
        descriptor.Ignore(item => item.CreatedById);
        descriptor.Ignore(item => item.InviteeId);
    }

    public static CustomerDetails GetCreatedBy([Parent] InviteCustomerToJoinTeamDetails item)
        => new(item.CreatedById);

    public static CustomerDetails? GetInvitee([Parent] InviteCustomerToJoinTeamDetails item)
        => string.IsNullOrWhiteSpace(item.InviteeId) ? null : new CustomerDetails(item.InviteeId);
}
