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
    IOrganizationAuthorizationService organizationAuthorizationService) : ITeamAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanViewAsync(team.OrganizationId, customerId, cancellationToken);

    public async ValueTask<bool> CanModifyAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanModifyAsync(team.OrganizationId, customerId, cancellationToken);

    public async ValueTask<bool> CanDeleteAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanDeleteAsync(team.OrganizationId, customerId, cancellationToken);

    public async ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Team team, string customerId, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanInvitePeopleAsync(team.OrganizationId, customerId, cancellationToken);

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Team team,
        string customerId,
        CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanCancelPeopleExistingInvitationsAsync(team.OrganizationId, customerId, cancellationToken);

    public async ValueTask<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await cachedTeamService.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();

        return new Permissions
        {
            CanView = await CanViewAsync(team, customer.Id, cancellationToken),
            CanModify = await CanModifyAsync(team, customer.Id, cancellationToken),
            CanDelete = await CanDeleteAsync(team, customer.Id, cancellationToken),
            CanInvitePeople = await CanInvitePeopleAsync(team, customer.Id, cancellationToken),
            CanCancelPeopleExistingInvitations = await CanCancelPeopleExistingInvitationsAsync(team, customer.Id, cancellationToken)
        };
    }
}
