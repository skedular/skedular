using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Invitation;

[MutationType]
public class RootMutation
{
    [UseResolverScope]
    public async Task<InviteCustomersToJoinOrganizationPayload> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.InviteMembersByEmailsAsync(input.OrganizationId, input.Emails.ToList(), cancellationToken);
        return new InviteCustomersToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<AcceptInvitationToJoinOrganizationPayload> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<RejectInvitationToJoinOrganizationPayload> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelInvitationToJoinOrganizationPayload> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }
}
