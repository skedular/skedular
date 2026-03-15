using Api.Shared.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;

namespace Booking.Api.Services;

public interface IMarketplaceBookingSubscriptionService
{
    Task<MarketplaceBookingSubscription> AddAsync(MarketplaceBookingSubscription subscription, CancellationToken cancellationToken);
    Task<MarketplaceBookingSubscription> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceBookingSubscriptionService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    Shared.Services.IMarketplaceBookingSubscriptionService sharedMarketplaceBookingSubscriptionService)
    : IMarketplaceBookingSubscriptionService
{
    public async Task<MarketplaceBookingSubscription> AddAsync(MarketplaceBookingSubscription subscription, CancellationToken cancellationToken)
    {
        if (subscription.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(subscription.InvolvedCustomers));
        }

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        if (string.IsNullOrWhiteSpace(subscription.Id))
        {
            subscription.Id = randomHelper.Generate();
        }
        else
        {
            var existingSubscription =
                await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(subscription.Id, cancellationToken);
            if (existingSubscription is not null)
            {
                throw new MarketplaceBookingSubscriptionCannotBeUpdated();
            }
        }

        var organizations = await organizationAuthorizationService.GetOrganizationsAndValidatePermissionsAsync(
            subscription.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                .Select(item => item.Id)
                .Distinct()
                .ToList(),
            subscription.InvolvedOrganizations
                .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
                .Select(item => item.UniqueAlphanumericName!)
                .Distinct()
                .ToList(),
            customer.Id,
            false,
            cancellationToken);

        var teams = await teamAuthorizationService.GetBookingInvolvedTeamAndValidatePermissionsAsync(
            subscription.InvolvedTeams.Select(item => item.Id).Distinct().ToList(),
            customer.Id,
            false,
            cancellationToken);

        return await sharedMarketplaceBookingSubscriptionService.AddAsync(subscription, customer, organizations, teams, cancellationToken);
    }

    public async Task<MarketplaceBookingSubscription> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingSubscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(id, cancellationToken) ??
                                   throw new MarketplaceBookingSubscriptionNotFound();

        var organizationIds = existingSubscription.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
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

        var teamIds = existingSubscription.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
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

        return await sharedMarketplaceBookingSubscriptionService.DeleteAsync(existingSubscription, customer, cancellationToken);
    }
}
