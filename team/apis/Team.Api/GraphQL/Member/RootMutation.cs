using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.GraphQL.Team;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Member;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamMembersAsync(
        UpdateTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var team = await teamMemberService.UpdateMembersAsync(input.Id, mapper.MapToTeamMembers(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload> AddTeamMemberAsync(
        AddTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var teamMember = await teamMemberService.AddAsync(input.Id, mapper.MapTo(input), cancellationToken);
        return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = mapper.MapTo(teamMember) };
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload> RemoveTeamMemberAsync(
        RemoveTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var teamMember = await teamMemberService.RemoveAsync(input.Id, cancellationToken);
        return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = mapper.MapTo(teamMember) };
    }

    [UseResolverScope]
    public async Task<TeamMemberDetailsPayload> ChangeTeamMemberRoleAsync(
        ChangeTeamMemberRoleInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Member = mapper.MapTo(await teamMemberService.ChangeRoleAsync(input.Id, input.Role, cancellationToken))
        };

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload> ChangeTeamMembersStatusAsync(
        ChangeTeamMembersStatusInput input,
        [Service] ITeamMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers =
            await organizationMemberService.ChangeStatusAsync(input.Ids.RemoveInvalidIds()!.ToList(), input.Status, cancellationToken);
        return new TeamMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload> RemoveTeamMembersAsync(
        RemoveTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await teamMemberService.RemoveAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new TeamMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
        };
    }
}
