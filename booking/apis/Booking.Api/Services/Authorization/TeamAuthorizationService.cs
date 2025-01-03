using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Customer = Booking.Shared.Models.Customer;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    bool CanViewBookings(Team team, Customer customer);
    bool CanAddBooking(Team team, Customer customer);
    bool CanUpdateBooking(Team team, Customer customer);
    bool CanDeleteBooking(Team team, Customer customer);
    bool CanAddBookingOnBehalf(Team team, Customer customer);
    bool CanUpdateBookingOnBehalf(Team team, Customer customer);
    bool CanDeleteBookingOnBehalf(Team team, Customer customer);
    Task<TeamPermissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService,
    ICachedTeamService cachedTeamService)
    : ITeamAuthorizationService
{
    public bool CanViewBookings(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanViewBookings(team.Organization, customer);
    }

    public bool CanAddBooking(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanAddBooking(team.Organization, customer);
    }

    public bool CanUpdateBooking(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanUpdateBooking(team.Organization, customer);
    }

    public bool CanDeleteBooking(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanDeleteBooking(team.Organization, customer);
    }

    public bool CanAddBookingOnBehalf(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanAddBookingOnBehalf(team.Organization, customer);
    }

    public bool CanUpdateBookingOnBehalf(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanUpdateBookingOnBehalf(team.Organization, customer);
    }

    public bool CanDeleteBookingOnBehalf(Team team, Customer customer)
    {
        if (team.Organization is null)
        {
            return team.TeamMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
            {
                Status: TeamMemberStatusConstants.Active,
                Role: TeamMemberRoleConstants.Owner or TeamMemberRoleConstants.Administrator
                or TeamMemberRoleConstants.Member
            };
        }

        return organizationAuthorizationService.CanDeleteBookingOnBehalf(team.Organization, customer);
    }

    public async Task<TeamPermissions> GetPermissionsAsync(
        string teamId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await cachedTeamService.GetByIdAsync(teamId, cancellationToken);

        return new TeamPermissions
        {
            CanViewBookings = CanViewBookings(team, customer),
            CanAddBooking = CanAddBooking(team, customer),
            CanUpdateBooking = CanUpdateBooking(team, customer),
            CanDeleteBooking = CanDeleteBooking(team, customer),
            CanAddBookingOnBehalf = CanAddBookingOnBehalf(team, customer),
            CanUpdateBookingOnBehalf = CanUpdateBookingOnBehalf(team, customer),
            CanDeleteBookingOnBehalf = CanDeleteBookingOnBehalf(team, customer)
        };
    }
}
