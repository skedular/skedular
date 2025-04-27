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
    Task<TeamPermissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService,
    ICachedTeamService cachedTeamService)
    : ITeamAuthorizationService
{
    public bool CanViewBookings(Team team, Customer customer) =>
        team.Organization is not null && organizationAuthorizationService.CanViewBookings(team.Organization, customer);

    public bool CanAddBooking(Team team, Customer customer) =>
        team.Organization is not null && organizationAuthorizationService.CanAddBooking(team.Organization, customer);

    public bool CanUpdateBooking(Team team, Customer customer) =>
        team.Organization is not null && organizationAuthorizationService.CanUpdateBooking(team.Organization, customer);

    public bool CanDeleteBooking(Team team, Customer customer) =>
        team.Organization is not null && organizationAuthorizationService.CanDeleteBooking(team.Organization, customer);

    public async Task<TeamPermissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await cachedTeamService.GetByIdAsync(teamId, cancellationToken);

        return new TeamPermissions
        {
            CanViewBookings = CanViewBookings(team, customer),
            CanAddBooking = CanAddBooking(team, customer),
            CanUpdateBooking = CanUpdateBooking(team, customer),
            CanDeleteBooking = CanDeleteBooking(team, customer)
        };
    }
}
