using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Invitation;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<InviteCustomersToJoinTeamPayload> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InvitesCustomersToJoinTeam = mapper.MapTo(
                    await invitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails.ToList(), cancellationToken))
                .ToList()
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = mapper.MapTo(await invitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = mapper.MapTo(await invitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        [Service] IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = mapper.MapTo(await invitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken))
        };
}
