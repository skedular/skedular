using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.GraphQL.Team;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Member;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper, ILogger<RootMutation> logger)
{
    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamMembersAsync(
        UpdateTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName} for Team {TeamId}", nameof(UpdateTeamMembersAsync), input.Id);

        try
        {
            var team = await teamMemberService.UpdateMembersAsync(input.Id, graphQlMapper.MapToTeamMembers(input), input.FieldsToUpdate,
                cancellationToken);
            logger.LogInformation("Completed {OperationName} for Team {TeamId}", nameof(UpdateTeamMembersAsync), input.Id);
            return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = graphQlMapper.MapTo(team)! };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {OperationName} for Team {TeamId}", nameof(UpdateTeamMembersAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload> AddTeamMemberAsync(
        AddTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName} for Team {TeamId}", nameof(AddTeamMemberAsync), input.Id);

        try
        {
            var teamMember = await teamMemberService.AddAsync(input.Id, graphQlMapper.MapTo(input), cancellationToken);
            logger.LogInformation("Completed {OperationName} for Team {TeamId}", nameof(AddTeamMemberAsync), input.Id);
            return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = graphQlMapper.MapTo(teamMember) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {OperationName} for Team {TeamId}", nameof(AddTeamMemberAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload> RemoveTeamMemberAsync(
        RemoveTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName} for TeamMember {TeamMemberId}", nameof(RemoveTeamMemberAsync), input.Id);

        try
        {
            var teamMember = await teamMemberService.RemoveAsync(input.Id, cancellationToken);
            logger.LogInformation("Completed {OperationName} for TeamMember {TeamMemberId}", nameof(RemoveTeamMemberAsync), input.Id);
            return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = graphQlMapper.MapTo(teamMember) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {OperationName} for TeamMember {TeamMemberId}", nameof(RemoveTeamMemberAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMemberDetailsPayload> ChangeTeamMemberRoleAsync(
        ChangeTeamMemberRoleInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName} for TeamMember {TeamMemberId}", nameof(ChangeTeamMemberRoleAsync), input.Id);

        try
        {
            var member = await teamMemberService.ChangeRoleAsync(input.Id, input.Role, cancellationToken);
            logger.LogInformation("Completed {OperationName} for TeamMember {TeamMemberId}", nameof(ChangeTeamMemberRoleAsync), input.Id);
            return new TeamMemberDetailsPayload { ClientMutationId = input.ClientMutationId, Member = graphQlMapper.MapTo(member) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {OperationName} for TeamMember {TeamMemberId}", nameof(ChangeTeamMemberRoleAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload> ChangeTeamMembersStatusAsync(
        ChangeTeamMembersStatusInput input,
        [Service] ITeamMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName}", nameof(ChangeTeamMembersStatusAsync));

        try
        {
            var organizationMembers =
                await organizationMemberService.ChangeStatusAsync(input.Ids.RemoveInvalidIds().ToList(), input.Status, cancellationToken);
            logger.LogInformation("Completed {OperationName}", nameof(ChangeTeamMembersStatusAsync));
            return new TeamMembersDetailsPayload
            {
                ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(graphQlMapper.MapTo).ToArray()
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {OperationName}", nameof(ChangeTeamMembersStatusAsync));
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload> RemoveTeamMembersAsync(
        RemoveTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName}", nameof(RemoveTeamMembersAsync));

        try
        {
            var organizationMembers = await teamMemberService.RemoveAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
            logger.LogInformation("Completed {OperationName}", nameof(RemoveTeamMembersAsync));
            return new TeamMembersDetailsPayload
            {
                ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(graphQlMapper.MapTo).ToArray()
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {OperationName}", nameof(RemoveTeamMembersAsync));
            throw;
        }
    }
}
