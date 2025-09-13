using Api.Shared.Services;
using Booking.Shared.Models;
using Booking.Shared.Services.Cache;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services.Authorization;

public interface ITeamAuthorizationService
{
    ValueTask<bool> CanViewBookingsAsync(Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanAddBookingAsync(Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanUpdateBookingAsync(Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteBookingAsync(Team team, string customerId, CancellationToken cancellationToken);
    ValueTask<TeamPermissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedTeamService cachedTeamService,
    ICachedCustomerService cachedCustomerService)
    : ITeamAuthorizationService
{
    public async ValueTask<bool> CanViewBookingsAsync(Team team, string customerId, CancellationToken cancellationToken) =>
        team.Organization is not null &&
        await organizationAuthorizationService.CanViewBookingsAsync(team.Organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanAddBookingAsync(Team team, string customerId, CancellationToken cancellationToken) =>
        team.Organization is not null &&
        await organizationAuthorizationService.CanAddBookingAsync(team.Organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanUpdateBookingAsync(Team team, string customerId, CancellationToken cancellationToken) =>
        team.Organization is not null &&
        await organizationAuthorizationService.CanUpdateBookingAsync(team.Organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanDeleteBookingAsync(Team team, string customerId, CancellationToken cancellationToken) =>
        team.Organization is not null &&
        await organizationAuthorizationService.CanDeleteBookingAsync(team.Organization.Id, customerId, cancellationToken);

    public async ValueTask<TeamPermissions> GetPermissionsAsync(string teamId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await cachedTeamService.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();

        return new TeamPermissions
        {
            CanViewBookings = await CanViewBookingsAsync(team, customer.Id, cancellationToken),
            CanAddBooking = await CanAddBookingAsync(team, customer.Id, cancellationToken),
            CanUpdateBooking = await CanUpdateBookingAsync(team, customer.Id, cancellationToken),
            CanDeleteBooking = await CanDeleteBookingAsync(team, customer.Id, cancellationToken)
        };
    }
}
