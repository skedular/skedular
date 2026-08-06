using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Member;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomerToJoinOrganizationDetails")]
public class InviteCustomerToJoinOrganizationDetails : Node
{
    [GraphQLName("email")]
    public string? Email { get; set; }

    [GraphQLName("status")]
    public OrganizationInvitationStatusDetails Status { get; set; } = new();

    [GraphQLName("role")]
    public OrganizationMemberRole Role { get; set; }

    [GraphQLName("organization")]
    public OrganizationDetails Organization { get; set; } = new();

    [GraphQLName("createdById")]
    public string CreatedById { get; set; } = string.Empty;

    [GraphQLName("inviteeId")]
    public string? InviteeId { get; set; }
}

[ObjectType<InviteCustomerToJoinOrganizationDetails>]
public static partial class InviteCustomerToJoinOrganizationDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<InviteCustomerToJoinOrganizationDetails> descriptor)
    {
        descriptor.Ignore(item => item.CreatedById);
        descriptor.Ignore(item => item.InviteeId);
    }

    public static CustomerDetails GetCreatedBy([Parent] InviteCustomerToJoinOrganizationDetails item)
        => new(item.CreatedById);

    public static CustomerDetails? GetInvitee([Parent] InviteCustomerToJoinOrganizationDetails item)
        => string.IsNullOrWhiteSpace(item.InviteeId) ? null : new CustomerDetails(item.InviteeId);
}
