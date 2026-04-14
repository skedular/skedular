using Api.Shared.Services;
using Team.Shared.Models;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    ValueTask<bool> CanViewAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken);

    ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Team team,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    ICachedTeamService cachedTeamService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILogger<TeamAuthorizationService> logger) : ITeamAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken)
    {
        var allowed = await organizationAuthorizationService.CanViewAsync(team.OrganizationId, customerId, cancellationToken);
        if (allowed)
        {
            logger.LogInformation("Team view permission granted for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }
        else
        {
            logger.LogWarning("Team view permission denied for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }

        return allowed;
    }

    public async ValueTask<bool> CanModifyAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken)
    {
        var allowed = await organizationAuthorizationService.CanModifyAsync(team.OrganizationId, customerId, cancellationToken);
        if (allowed)
        {
            logger.LogInformation("Team modify permission granted for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }
        else
        {
            logger.LogWarning("Team modify permission denied for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }

        return allowed;
    }

    public async ValueTask<bool> CanDeleteAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken)
    {
        var allowed = await organizationAuthorizationService.CanDeleteAsync(team.OrganizationId, customerId, cancellationToken);
        if (allowed)
        {
            logger.LogInformation("Team delete permission granted for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }
        else
        {
            logger.LogWarning("Team delete permission denied for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }

        return allowed;
    }

    public async ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken)
    {
        var allowed = await organizationAuthorizationService.CanInvitePeopleAsync(team.OrganizationId, customerId, cancellationToken);
        if (allowed)
        {
            logger.LogInformation("Team invite permission granted for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }
        else
        {
            logger.LogWarning("Team invite permission denied for customer {CustomerId} on team {TeamId}", customerId, team.Id);
        }

        return allowed;
    }

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Team team,
        string customerId,
        CancellationToken cancellationToken)
    {
        var allowed = await organizationAuthorizationService.CanCancelPeopleExistingInvitationsAsync(
            team.OrganizationId,
            customerId,
            cancellationToken);

        if (allowed)
        {
            logger.LogInformation(
                "Team invitation-cancellation permission granted for customer {CustomerId} on team {TeamId}",
                customerId,
                team.Id);
        }
        else
        {
            logger.LogWarning(
                "Team invitation-cancellation permission denied for customer {CustomerId} on team {TeamId}",
                customerId,
                team.Id);
        }

        return allowed;
    }

    public async ValueTask<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var team = await cachedTeamService.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();

        return new Permissions
        {
            CanView = await CanViewAsync(team, customerId, cancellationToken),
            CanModify = await CanModifyAsync(team, customerId, cancellationToken),
            CanDelete = await CanDeleteAsync(team, customerId, cancellationToken),
            CanInvitePeople = await CanInvitePeopleAsync(team, customerId, cancellationToken),
            CanCancelPeopleExistingInvitations = await CanCancelPeopleExistingInvitationsAsync(team, customerId, cancellationToken)
        };
    }
}
