using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Team;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<TeamPayload> AddTeamAsync(
        AddTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamAsync(
        UpdateTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.UpdateAsync(mapper.MapTo(input), false, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload> DeleteTeamAsync(
        DeleteTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.DeleteAsync(input.Id, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamAndTeamMembersAsync(
        UpdateTeamAndTeamMembersInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(input.OrganizationId);

        var team = await teamService.UpdateAsync(mapper.MapTo(input), true, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }
}
