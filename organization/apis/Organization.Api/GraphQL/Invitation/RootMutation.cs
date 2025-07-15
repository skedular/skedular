using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Invitation;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<InvitationsToJoinOrganizationPayload> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InvitesCustomersToJoinOrganization = mapper.MapTo(
                    await organizationInvitationService.InviteMembersByEmailsAsync(input.OrganizationId, input.Emails.ToList(), cancellationToken))
                .ToList()
        };

    [UseResolverScope]
    public async Task<InvitationToJoinOrganizationPayload> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinOrganization =
                mapper.MapTo(await organizationInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinOrganizationPayload> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinOrganization =
                mapper.MapTo(await organizationInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinOrganizationPayload> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinOrganization =
                mapper.MapTo(await organizationInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken))
        };
}
