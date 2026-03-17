using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Customer = Booking.Shared.Database.Entities.Customer;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IMarketplaceBookingSubscriptionService
{
    Task<MarketplaceBookingSubscription> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<MarketplaceBookingSubscription>>, int)> GetPaginatedMarketplaceBookingSubscriptionsAsync(
        PaginationInputParam paginationInputParam,
        MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
        ICollection<MarketplaceBookingSubscriptionOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<MarketplaceBookingSubscription> AddAsync(MarketplaceBookingSubscription subscription, CancellationToken cancellationToken);
    Task<MarketplaceBookingSubscription> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceBookingSubscriptionService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    ICachedCustomerService cachedCustomerService,
    ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
    IMapper mapper,
    Shared.Mappers.IMapper sharedMapper,
    Shared.Services.IMarketplaceBookingSubscriptionService sharedMarketplaceBookingSubscriptionService)
    : IMarketplaceBookingSubscriptionService
{
    public async Task<MarketplaceBookingSubscription> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var subscription = await cachedMarketplaceBookingSubscriptionService.GetByIdAsync(id, cancellationToken) ??
                           throw new MarketplaceBookingSubscriptionNotFound();

        await EnsureCustomerCanViewMarketplaceBookingSubscriptionAsync(subscription, customer, cancellationToken);

        return sharedMapper.MapTo(subscription);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<MarketplaceBookingSubscription>>, int)> GetPaginatedMarketplaceBookingSubscriptionsAsync(
        PaginationInputParam paginationInputParam,
        MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
        ICollection<MarketplaceBookingSubscriptionOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
        }

        if (customer is not null && searchCriteria.IncludeMineOnly.HasValue)
        {
            searchCriteria = searchCriteria with { CustomerIds = [customer.Id] };
        }

        List<string>? organizationIds = null;
        List<string>? organizationCustomDomains = null;
        List<string>? teamIds = null;

        if (searchCriteria.CustomerIds.Count != 0 &&
            customer is not null &&
            searchCriteria.CustomerIds.Any(item => item != customer.Id) &&
            searchCriteria.OrganizationIds.Count == 0 && searchCriteria.OrganizationCustomDomains.Count == 0)
        {
            throw new InvalidOperationException("You can only look for others' subscriptions if organization is included in your search");
        }

        if (searchCriteria.CustomerIds.Count != 0 &&
            customer is not null &&
            searchCriteria.CustomerIds.Any(item => item != customer.Id) &&
            searchCriteria.OrganizationIds.Count != 0)
        {
            var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
            organizationIds = organizationCustomerPairs.Item1.Keys.ToList();

            if (searchCriteria.CustomerIds
                .Any(customerId => !organizationCustomerPairs.Item1.Keys.Any(item => organizationCustomerPairs.Item1[item].Contains(customerId))))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (searchCriteria.CustomerIds.Count != 0 &&
            customer is not null &&
            searchCriteria.CustomerIds.Any(item => item != customer.Id) &&
            searchCriteria.OrganizationCustomDomains.Count != 0)
        {
            var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
            organizationCustomDomains = organizationCustomerPairs.Item2.Keys.ToList();

            if (searchCriteria.CustomerIds
                .Any(customerId => !organizationCustomerPairs.Item2.Keys.Any(item => organizationCustomerPairs.Item2[item].Contains(customerId))))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (customer is not null && searchCriteria.OrganizationIds.Count != 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
            }

            if (searchCriteria.OrganizationIds.Any(item => !organizationIds.Contains(item)))
            {
                throw new UnauthorizedAccessException();
            }
        }
        else if (customer is not null && searchCriteria.OrganizationCustomDomains.Count != 0)
        {
            if (organizationCustomDomains is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationCustomDomains = organizationCustomerPairs.Item2.Keys.ToList();
            }

            if (searchCriteria.OrganizationCustomDomains.Any(item => !organizationCustomDomains.Contains(item)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (customer is not null && searchCriteria.TeamIds.Count != 0)
        {
            var criteria = searchCriteria;
            var teams = await repositoryFactory.TeamRepository.Query(
                    new Specification<Team> { Criteria = query => !query.DeletedAt.HasValue && criteria.TeamIds.Contains(query.Id) }
                        .AddInclude(query => query.Organization!))
                .ToListAsync(cancellationToken);

            foreach (var team in teams)
            {
                if (organizationIds is null)
                {
                    var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                    organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
                }

                if (team.Organization is null || !organizationIds.Contains(team.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        if (customer is not null &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            searchCriteria.OrganizationIds.Count == 0 &&
            searchCriteria.OrganizationCustomDomains.Count == 0 &&
            searchCriteria.TeamIds.Count == 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
            }

            teamIds ??= await GetCustomerTeamIdsAsync(customer, cancellationToken);

            if (organizationIds.Count == 0 && teamIds.Count == 0)
            {
                return (new PaginatedInfo(false, false, null, null), [], 0);
            }

            searchCriteria = searchCriteria with { OrganizationIds = organizationIds, TeamIds = teamIds };
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

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
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .Select(item => item.CustomDomain!)
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

    private async Task<(IDictionary<string, List<string>>, IDictionary<string, List<string>>)> GetCustomerOrganizationIdsAsync(
        Customer customer,
        CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customer.Id, false, false, cancellationToken);

        return (organizations.ToDictionary(
                item => item.Id, item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList()),
            organizations
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .ToDictionary(
                    item => item.CustomDomain!,
                    item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList()));
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(Customer customer, CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewMarketplaceBookingSubscriptionAsync(
        Shared.Database.Entities.MarketplaceBookingSubscription subscription,
        Customer customer,
        CancellationToken cancellationToken)
    {
        var organizationIds = subscription.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
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
                if (!await organizationAuthorizationService.CanViewBookingsAsync(organization.Id, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        var teamIds = subscription.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teams)
            {
                if (!await teamAuthorizationService.CanViewBookingsAsync(team, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
