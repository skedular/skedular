using Api.Shared.Models;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL;

public class TeamMutation(IMapper mapper)
{
    [UseServiceScope]
    public async Task<TeamPayload?> AddTeamAsync(
        AddTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseServiceScope]
    public async Task<TeamPayload?> UpdateTeamAsync(
        UpdateTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseServiceScope]
    public async Task<TeamPayload?> DeleteTeamAsync(
        DeleteTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.DeleteAsync(input.Id, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseServiceScope]
    public async Task<TeamMemberDetailsPayload?> ChangeTeamMemberOwnershipTypeAsync(
        ChangeTeamMemberOwnershipTypeInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var teamMember =
            await teamMemberService.ChangeMembershipTypeAsync(
                input.Id,
                input.MembershipType switch
                {
                    TeamMemberMembershipType.Owner => TeamMembershipType.Owner,
                    TeamMemberMembershipType.Administrator => TeamMembershipType.Administrator,
                    TeamMemberMembershipType.Member => TeamMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                cancellationToken);
        return new TeamMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(teamMember)
        };
    }

    [UseServiceScope]
    public async Task<InviteCustomersToJoinTeamPayload?> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<AcceptInvitationToJoinTeamPayload?> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<RejectInvitationToJoinTeamPayload?> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<CancelInvitationToJoinTeamPayload?> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }
}
