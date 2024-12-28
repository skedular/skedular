using Api.Shared.Services.Models;
using Enterprise.Shared.Exceptions;
using Team.Shared.Models;
using Team.Shared.Repositories;

namespace Team.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    bool CanView(Shared.Database.Entities.Team team, Customer customer);
    bool CanModify(Shared.Database.Entities.Team team, Customer customer);
    bool CanDelete(Shared.Database.Entities.Team team, Customer customer);
    bool CanInvitePeople(Shared.Database.Entities.Team team, Customer customer);
    bool CanCancelPeopleExistingInvitations(Shared.Database.Entities.Team team, Customer customer);
    Task<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService)
    : ITeamAuthorizationService
{
    public bool CanView(Shared.Database.Entities.Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                MembershipType: TeamMembershipTypeConstants.Owner or TeamMembershipTypeConstants.Administrator
                or TeamMembershipTypeConstants.Member
            };
        }

        return organizationAuthorizationService.CanView(team.Organization, customer);
    }

    public bool CanModify(Shared.Database.Entities.Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                MembershipType: TeamMembershipTypeConstants.Owner or TeamMembershipTypeConstants.Administrator
            };
        }

        return organizationAuthorizationService.CanModify(team.Organization, customer);
    }

    public bool CanDelete(Shared.Database.Entities.Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                MembershipType: TeamMembershipTypeConstants.Owner
            };
        }

        return organizationAuthorizationService.CanDelete(team.Organization, customer);
    }

    public bool CanInvitePeople(Shared.Database.Entities.Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                MembershipType: TeamMembershipTypeConstants.Owner or TeamMembershipTypeConstants.Administrator
            };
        }

        return organizationAuthorizationService.CanInvitePeople(team.Organization, customer);
    }

    public bool CanCancelPeopleExistingInvitations(
        Shared.Database.Entities.Team team,
        Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                MembershipType: TeamMembershipTypeConstants.Owner or TeamMembershipTypeConstants.Administrator
            };
        }

        return organizationAuthorizationService.CanCancelPeopleExistingInvitations(team.Organization, customer);
    }

    public async Task<Permissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(
            teamId,
            cancellationToken);
        if (team is null)
        {
            throw new OrganizationNotFound();
        }

        return new Permissions
        {
            CanView = CanView(team, customer),
            CanModify = CanModify(team, customer),
            CanDelete = CanDelete(team, customer),
            CanInvitePeople = CanInvitePeople(team, customer),
            CanCancelPeopleExistingInvitations = CanCancelPeopleExistingInvitations(team, customer)
        };
    }
}
