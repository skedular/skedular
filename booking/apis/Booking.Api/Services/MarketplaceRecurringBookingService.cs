using Api.Shared.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;

namespace Booking.Api.Services;

public interface IMarketplaceRecurringBookingService
{
    Task<RecurringBooking> AddAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken);
    Task<RecurringBooking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceRecurringBookingService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    Shared.Services.IMarketplaceRecurringBookingService sharedMarketplaceRecurringBookingService) : IMarketplaceRecurringBookingService
{
    public async Task<RecurringBooking> AddAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken)
    {
        if (recurringBooking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(recurringBooking.InvolvedCustomers));
        }

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        if (string.IsNullOrWhiteSpace(recurringBooking.Id))
        {
            recurringBooking.Id = randomHelper.Generate();
        }
        else
        {
            var existingRecurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBooking.Id, cancellationToken);
            if (existingRecurringBooking is not null)
            {
                throw new MarketplaceRecurringBookingCannotBeUpdated();
            }
        }

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            recurringBooking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            recurringBooking.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .Select(item => item.CustomDomain!)
                .Distinct()
                .ToList(),
            customer.Id,
            false,
            cancellationToken);

        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            recurringBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList(),
            customer.Id,
            false,
            cancellationToken);

        return await sharedMarketplaceRecurringBookingService.AddAsync(recurringBooking, customer, organizations, teams, cancellationToken);
    }

    public async Task<RecurringBooking> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingRecurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(id, cancellationToken) ??
                                       throw new RecurringBookingNotFound();
        var organizationIds = existingRecurringBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
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

        var teamIds = existingRecurringBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
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

        return await sharedMarketplaceRecurringBookingService.DeleteAsync(existingRecurringBooking, customer, cancellationToken);
    }
}
