using Api.Shared.Models;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<TeamPayload?> AddTeamAsync(
        AddTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload?> UpdateTeamAsync(
        UpdateTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload?> DeleteTeamAsync(
        DeleteTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.DeleteAsync(input.Id, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
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
                    TeamMemberMembershipType.Owner => OldTeamMembershipType.Owner,
                    TeamMemberMembershipType.Administrator => OldTeamMembershipType.Administrator,
                    TeamMemberMembershipType.Member => OldTeamMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                cancellationToken);
        return new TeamMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(teamMember)
        };
    }

    [UseResolverScope]
    public async Task<InviteCustomersToJoinTeamPayload?> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<AcceptInvitationToJoinTeamPayload?> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<RejectInvitationToJoinTeamPayload?> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelInvitationToJoinTeamPayload?> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }
}
