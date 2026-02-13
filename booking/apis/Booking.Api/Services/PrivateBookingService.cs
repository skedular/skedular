using Api.Shared.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IPrivateBookingService
{
    Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class PrivateBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IContext context,
    Shared.Services.IPrivateBookingService sharedPrivateBookingService) : IPrivateBookingService
{
    public async Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
        }

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        if (!string.IsNullOrWhiteSpace(booking.Id))
        {
            var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking is not null)
            {
                return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, customer.Id, false, cancellationToken);
        var teams = await GetTeamAndValidatePermissionsAsync(booking, customer.Id, false, cancellationToken);

        return await sharedPrivateBookingService.AddAsync(booking, customer, organizations, teams, cancellationToken);
    }

    public async Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken) ?? throw new BookingNotFound();

        return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
    }

    public async Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        var organizationIds = existingBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
                organizationIds,
                null,
                false,
                false,
                cancellationToken);

            foreach (var organization in organizations)
            {
                if (!await organizationAuthorizationService.CanDeleteBookingAsync(organization.Id, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        var teamIds = existingBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teams)
            {
                if (!await teamAuthorizationService.CanDeleteBookingAsync(team, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        return await sharedPrivateBookingService.DeleteAsync(existingBooking, customer, cancellationToken);
    }

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, callingCustomer.Id, true, cancellationToken);
        var teams = await GetTeamAndValidatePermissionsAsync(booking, callingCustomer.Id, true, cancellationToken);

        return await sharedPrivateBookingService.UpdateAsync(booking, existingBooking, callingCustomer, organizations, teams, cancellationToken);
    }

    private async Task<ICollection<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        var organizationIds = booking.InvolvedOrganizations
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .Select(item => item.Id)
            .Distinct()
            .ToList();
        var uniqueAlphanumericNames = booking.InvolvedOrganizations
            .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
            .Select(item => item.UniqueAlphanumericName!)
            .Distinct()
            .ToList();

        if (organizationIds.Count == 0 && uniqueAlphanumericNames.Count == 0)
        {
            return [];
        }

        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            uniqueAlphanumericNames,
            false,
            false,
            cancellationToken);
        if (organizationIds.Count + uniqueAlphanumericNames.Count != organizations.Count)
        {
            throw new OrganizationNotFound();
        }

        var result = new List<Organization>();
        foreach (var organization in booking.InvolvedOrganizations)
        {
            var organizationEntity = organizations.First(item =>
                item.Id == organization.Id || item.UniqueAlphanumericName == organization.UniqueAlphanumericName);
            if (existing)
            {
                if (!await organizationAuthorizationService.CanUpdateBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await organizationAuthorizationService.CanAddBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }

                if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new NoMoreInteractionAllowed();
                }
            }

            result.Add(organizationEntity);
        }

        return result;
    }

    private async Task<ICollection<Team>> GetTeamAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        var teamIds = booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count == 0)
        {
            return [];
        }

        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
        if (teamIds.Count != teams.Count)
        {
            throw new TeamNotFound();
        }

        var result = new List<Team>();
        foreach (var team in booking.InvolvedTeams)
        {
            var teamEntity = teams.First(item => item.Id == team.Id);
            if (existing)
            {
                if (!await teamAuthorizationService.CanUpdateBookingAsync(teamEntity, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await teamAuthorizationService.CanAddBookingAsync(teamEntity, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            result.Add(teamEntity);
        }

        return result;
    }
}
