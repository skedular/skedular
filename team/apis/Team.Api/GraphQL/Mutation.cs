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
        var team = await teamService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload?> UpdateTeamAsync(
        UpdateTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.UpdateAsync(mapper.MapTo(input), false, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload?> UpdateTeamAndTeamMembersAsync(
        UpdateTeamAndTeamMembersInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(input.OrganizationId);

        var team = await teamService.UpdateAsync(mapper.MapTo(input), true, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload?> UpdateTeamMembersAsync(
        UpdateTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var team = await teamMemberService.UpdateMembersAsync(input.Id, mapper.MapToTeamMembers(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload?> AddTeamMemberAsync(
        AddTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var teamMember = await teamMemberService.AddAsync(input.Id, mapper.MapTo(input), cancellationToken);
        return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = mapper.MapTo(teamMember) };
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
    public async Task<TeamMemberPayload?> RemoveTeamMemberAsync(
        RemoveTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var teamMember = await teamMemberService.RemoveAsync(input.Id, cancellationToken);
        return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = mapper.MapTo(teamMember) };
    }

    [UseResolverScope]
    public async Task<TeamMemberDetailsPayload?> ChangeTeamMemberRoleAsync(
        ChangeTeamMemberRoleInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Member = mapper.MapTo(await teamMemberService.ChangeRoleAsync(input.Id, input.Role, cancellationToken))
        };

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload?> ChangeTeamMembersStatusAsync(
        ChangeTeamMembersStatusInput input,
        [Service] ITeamMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await organizationMemberService.ChangeStatusAsync(
            input.Ids.ToList(),
            input.Status,
            cancellationToken);
        return new TeamMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload?> RemoveTeamMembersAsync(
        RemoveTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await teamMemberService.RemoveAsync(input.Ids.ToList(), cancellationToken);
        return new TeamMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<InviteCustomersToJoinTeamPayload?> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        [Service] ITeamInvitationService teamInvitationService,
        CancellationToken cancellationToken)
    {
        await teamInvitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails.ToList(), cancellationToken);
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
