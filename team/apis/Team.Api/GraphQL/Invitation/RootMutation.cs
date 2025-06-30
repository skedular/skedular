using HotChocolate;
using HotChocolate.Types;
using Team.Api.Services;

namespace Team.Api.GraphQL.Invitation;

[MutationType]
public class RootMutation
{
    [UseResolverScope]
    public async Task<InviteCustomersToJoinTeamPayload> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails.ToList(), cancellationToken);
        return new InviteCustomersToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<AcceptInvitationToJoinTeamPayload> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<RejectInvitationToJoinTeamPayload> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelInvitationToJoinTeamPayload> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }
}
