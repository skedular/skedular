using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Invitation;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<InvitationsToJoinOrganizationPayload> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InvitesCustomersToJoinOrganization = graphQlMapper.MapTo(
                    await invitationService.InviteMembersByEmailsAsync(
                        input.OrganizationId,
                        input.OrganizationCustomDomain,
                        input.Emails.ToList(),
                        cancellationToken))
                .ToList()
        };

    [UseResolverScope]
    public async Task<InvitationToJoinOrganizationPayload> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinOrganization =
                graphQlMapper.MapTo(await invitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinOrganizationPayload> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinOrganization =
                graphQlMapper.MapTo(await invitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinOrganizationPayload> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinOrganization =
                graphQlMapper.MapTo(await invitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken))
        };
}
