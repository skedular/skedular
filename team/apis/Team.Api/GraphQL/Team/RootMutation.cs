using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Team;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper, ILogger<RootMutation> logger)
{
    [UseResolverScope]
    public async Task<TeamPayload> AddTeamAsync(
        AddTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName}", nameof(AddTeamAsync));
        var team = await teamService.AddAsync(graphQlMapper.MapTo(input), cancellationToken);
        logger.LogInformation("Completed {OperationName}", nameof(AddTeamAsync));
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = graphQlMapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamAsync(
        UpdateTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.UpdateAsync(graphQlMapper.MapTo(input), false, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = graphQlMapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload> DeleteTeamAsync(
        DeleteTeamInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.DeleteAsync(input.Id, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = graphQlMapper.MapTo(team)! };
    }

    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamAndTeamMembersAsync(
        UpdateTeamAndTeamMembersInput input,
        [Service] ITeamService teamService,
        CancellationToken cancellationToken)
    {
        var team = await teamService.UpdateAsync(graphQlMapper.MapTo(input), true, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = graphQlMapper.MapTo(team)! };
    }
}
