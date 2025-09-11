using Api.Shared.Services;
using Team.Shared.Models;
using Team.Shared.Repositories;

namespace Team.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    ValueTask<bool> CanViewAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken);
    bool CanView(Shared.Database.Entities.Team team, Customer customer);
    ValueTask<bool> CanModifyAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken);
    bool CanModify(Shared.Database.Entities.Team team, Customer customer);
    ValueTask<bool> CanDeleteAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken);
    bool CanDelete(Shared.Database.Entities.Team team, Customer customer);
    ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken);
    bool CanInvitePeople(Shared.Database.Entities.Team team, Customer customer);

    ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Team team,
        Customer customer,
        CancellationToken cancellationToken);

    bool CanCancelPeopleExistingInvitations(Shared.Database.Entities.Team team, Customer customer);
    ValueTask<bool> CanViewMemberPersonalDetailsAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken);
    bool CanViewMemberPersonalDetails(Shared.Database.Entities.Team team, Customer customer);
    ValueTask<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService) : ITeamAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanViewAsync(team.OrganizationId, customer, cancellationToken);

    public bool CanView(Shared.Database.Entities.Team team, Customer customer) =>
        organizationAuthorizationService.CanView(team.Organization, customer);

    public async ValueTask<bool> CanModifyAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanModifyAsync(team.OrganizationId, customer, cancellationToken);

    public bool CanModify(Shared.Database.Entities.Team team, Customer customer) =>
        organizationAuthorizationService.CanModify(team.Organization, customer);

    public async ValueTask<bool> CanDeleteAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanDeleteAsync(team.OrganizationId, customer, cancellationToken);

    public bool CanDelete(Shared.Database.Entities.Team team, Customer customer) =>
        organizationAuthorizationService.CanDelete(team.Organization, customer);

    public async ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Team team, Customer customer, CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanInvitePeopleAsync(team.OrganizationId, customer, cancellationToken);

    public bool CanInvitePeople(Shared.Database.Entities.Team team, Customer customer) =>
        organizationAuthorizationService.CanInvitePeople(team.Organization, customer);

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Team team,
        Customer customer,
        CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanCancelPeopleExistingInvitationsAsync(team.OrganizationId, customer, cancellationToken);

    public bool CanCancelPeopleExistingInvitations(Shared.Database.Entities.Team team, Customer customer) =>
        organizationAuthorizationService.CanCancelPeopleExistingInvitations(team.Organization, customer);

    public async ValueTask<bool> CanViewMemberPersonalDetailsAsync(
        Shared.Database.Entities.Team team,
        Customer customer,
        CancellationToken cancellationToken) =>
        await organizationAuthorizationService.CanViewMemberPersonalDetailsAsync(team.OrganizationId, customer, cancellationToken);

    public bool CanViewMemberPersonalDetails(Shared.Database.Entities.Team team, Customer customer) =>
        organizationAuthorizationService.CanViewMemberPersonalDetails(team.Organization, customer);

    public async ValueTask<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();

        return new Permissions
        {
            CanView = await CanViewAsync(team, customer, cancellationToken),
            CanModify = await CanModifyAsync(team, customer, cancellationToken),
            CanDelete = await CanDeleteAsync(team, customer, cancellationToken),
            CanInvitePeople = await CanInvitePeopleAsync(team, customer, cancellationToken),
            CanCancelPeopleExistingInvitations = await CanCancelPeopleExistingInvitationsAsync(team, customer, cancellationToken)
        };
    }
}
