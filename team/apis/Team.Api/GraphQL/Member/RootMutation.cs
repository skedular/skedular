using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Team.Api.GraphQL.Team;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Member;

[MutationType]
public class RootMutation(IMapper mapper, ILogger<RootMutation> logger)
{
    [UseResolverScope]
    public async Task<TeamPayload> UpdateTeamMembersAsync(
        UpdateTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Operation} for Team {TeamId}", nameof(UpdateTeamMembersAsync), input.Id);

        try
        {
            var team = await teamMemberService.UpdateMembersAsync(input.Id, mapper.MapToTeamMembers(input), cancellationToken);
            logger.LogInformation("Completed {Operation} for Team {TeamId}", nameof(UpdateTeamMembersAsync), input.Id);
            return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {Operation} for Team {TeamId}", nameof(UpdateTeamMembersAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload> AddTeamMemberAsync(
        AddTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Operation} for Team {TeamId}", nameof(AddTeamMemberAsync), input.Id);

        try
        {
            var teamMember = await teamMemberService.AddAsync(input.Id, mapper.MapTo(input), cancellationToken);
            logger.LogInformation("Completed {Operation} for Team {TeamId}", nameof(AddTeamMemberAsync), input.Id);
            return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = mapper.MapTo(teamMember) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {Operation} for Team {TeamId}", nameof(AddTeamMemberAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMemberPayload> RemoveTeamMemberAsync(
        RemoveTeamMemberInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Operation} for TeamMember {TeamMemberId}", nameof(RemoveTeamMemberAsync), input.Id);

        try
        {
            var teamMember = await teamMemberService.RemoveAsync(input.Id, cancellationToken);
            logger.LogInformation("Completed {Operation} for TeamMember {TeamMemberId}", nameof(RemoveTeamMemberAsync), input.Id);
            return new TeamMemberPayload { ClientMutationId = input.ClientMutationId, TeamMember = mapper.MapTo(teamMember) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {Operation} for TeamMember {TeamMemberId}", nameof(RemoveTeamMemberAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMemberDetailsPayload> ChangeTeamMemberRoleAsync(
        ChangeTeamMemberRoleInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Operation} for TeamMember {TeamMemberId}", nameof(ChangeTeamMemberRoleAsync), input.Id);

        try
        {
            var member = await teamMemberService.ChangeRoleAsync(input.Id, input.Role, cancellationToken);
            logger.LogInformation("Completed {Operation} for TeamMember {TeamMemberId}", nameof(ChangeTeamMemberRoleAsync), input.Id);
            return new TeamMemberDetailsPayload { ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(member) };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {Operation} for TeamMember {TeamMemberId}", nameof(ChangeTeamMemberRoleAsync), input.Id);
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload> ChangeTeamMembersStatusAsync(
        ChangeTeamMembersStatusInput input,
        [Service] ITeamMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Operation}", nameof(ChangeTeamMembersStatusAsync));

        try
        {
            var organizationMembers =
                await organizationMemberService.ChangeStatusAsync(input.Ids.RemoveInvalidIds().ToList(), input.Status, cancellationToken);
            logger.LogInformation("Completed {Operation}", nameof(ChangeTeamMembersStatusAsync));
            return new TeamMembersDetailsPayload
            {
                ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {Operation}", nameof(ChangeTeamMembersStatusAsync));
            throw;
        }
    }

    [UseResolverScope]
    public async Task<TeamMembersDetailsPayload> RemoveTeamMembersAsync(
        RemoveTeamMembersInput input,
        [Service] ITeamMemberService teamMemberService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {Operation}", nameof(RemoveTeamMembersAsync));

        try
        {
            var organizationMembers = await teamMemberService.RemoveAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
            logger.LogInformation("Completed {Operation}", nameof(RemoveTeamMembersAsync));
            return new TeamMembersDetailsPayload
            {
                ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {Operation}", nameof(RemoveTeamMembersAsync));
            throw;
        }
    }
}
