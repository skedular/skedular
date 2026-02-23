using Api.Shared.Services;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
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

    Task<ICollection<Team>> GetBookingInvolvedTeamAndValidatePermissionsAsync(
        ICollection<string> ids,
        string customerId,
        bool existing,
        CancellationToken cancellationToken);
}

public class TeamAuthorizationService(
    IRepositoryFactory repositoryFactory,
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

    public async Task<ICollection<Team>> GetBookingInvolvedTeamAndValidatePermissionsAsync(
        ICollection<string> ids,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(ids, false, cancellationToken);
        if (ids.Count != teams.Count)
        {
            throw new TeamNotFound();
        }

        var result = new List<Team>();
        foreach (var id in ids)
        {
            var teamEntity = teams.First(item => item.Id == id);
            if (existing)
            {
                if (!await CanUpdateBookingAsync(teamEntity, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await CanAddBookingAsync(teamEntity, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            result.Add(teamEntity);
        }

        return result;
    }
}
