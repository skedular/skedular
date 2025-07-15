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
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InvitesCustomersToJoinTeam = mapper.MapTo(
                    await teamInvitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails.ToList(), cancellationToken))
                .ToList()
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = mapper.MapTo(await teamInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = mapper.MapTo(await teamInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = mapper.MapTo(await teamInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken))
        };
}
