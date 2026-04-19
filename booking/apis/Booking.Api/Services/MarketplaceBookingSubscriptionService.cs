using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Constants = Booking.Shared.GraphQL.Constants;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.Services;

public interface IMarketplaceBookingSubscriptionService
{
    Task<MarketplaceBookingSubscription> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationArrearsInvoice>> GetArrearsInvoicesAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<MarketplaceBookingSubscription>>, int)> GetPaginatedMarketplaceBookingSubscriptionsAsync(
        PaginationInputParam paginationInputParam,
        MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
        ICollection<MarketplaceBookingSubscriptionOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<MarketplaceBookingSubscription> AddAsync(MarketplaceBookingSubscription subscription, CancellationToken cancellationToken);

    Task<MarketplaceBookingSubscription> DeleteAsync(
        string id,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingSubscriptionService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IContext context,
    ICachedCustomerService cachedCustomerService,
    ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IMapper mapper,
    Shared.Mappers.IMapper sharedMapper,
    Shared.Services.IMarketplaceBookingSubscriptionService sharedMarketplaceBookingSubscriptionService)
    : IMarketplaceBookingSubscriptionService
{
    public async Task<MarketplaceBookingSubscription> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var subscription = await cachedMarketplaceBookingSubscriptionService.GetByIdAsync(id, cancellationToken) ??
                           throw new MarketplaceBookingSubscriptionNotFound();

        await EnsureCustomerCanViewMarketplaceBookingSubscriptionAsync(subscription, customerId, cancellationToken);

        return sharedMapper.MapTo(subscription);
    }

    public async Task<ICollection<OrganizationArrearsInvoice>> GetArrearsInvoicesAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var subscription = await cachedMarketplaceBookingSubscriptionService.GetByIdAsync(id, cancellationToken) ??
                           throw new MarketplaceBookingSubscriptionNotFound();

        await EnsureCustomerCanViewMarketplaceBookingSubscriptionAsync(subscription, customerId, cancellationToken);

        var invoices =
            await repositoryFactory.OrganizationArrearsInvoiceRepository.GetByMarketplaceBookingSubscriptionIdUntrackedAsync(id, cancellationToken);
        return invoices.Select(sharedMapper.MapTo).ToList();
    }

    public async Task<(PaginatedInfo, ICollection<Edge<MarketplaceBookingSubscription>>, int)> GetPaginatedMarketplaceBookingSubscriptionsAsync(
        PaginationInputParam paginationInputParam,
        MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
        ICollection<MarketplaceBookingSubscriptionOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.IncludeMineOnly == true)
        {
            searchCriteria = searchCriteria with { CustomerIds = [customerId] };
        }

        MarketplaceBookingSubscriptionAccessScope? accessScope = null;
        List<string>? organizationIds = null;
        List<string>? teamIds = null;
        var requestedOwnSubscriptionsOnly = IsRequestedOwnSubscriptionsOnly(searchCriteria, customerId);
        var requestedOtherCustomersSubscriptions = IsRequestedOtherCustomersSubscriptions(searchCriteria, customerId);
        var scopedOrganization = await GetScopedOrganizationAsync(searchCriteria, cancellationToken);

        if (requestedOtherCustomersSubscriptions && scopedOrganization is null)
        {
            throw new InvalidOperationException("To view other people's subscriptions, narrow your search to a specific organisation first.");
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            scopedOrganization is not null &&
            (requestedOtherCustomersSubscriptions || !requestedOwnSubscriptionsOnly))
        {
            if (!await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(scopedOrganization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (scopedOrganization is not null)
        {
            accessScope = new MarketplaceBookingSubscriptionAccessScope(
                [scopedOrganization.Id],
                scopedOrganization.Teams.Where(item => !item.DeletedAt.HasValue).Select(item => item.Id).ToList());
            searchCriteria = searchCriteria with { OrganizationId = null, OrganizationCustomDomain = null };
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.TeamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetActiveByIdsAsync(searchCriteria.TeamIds.Distinct().ToList(), cancellationToken);

            foreach (var team in teams)
            {
                organizationIds ??= await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                if (team.Organization is null || !organizationIds.Contains(team.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            teamIds = searchCriteria.TeamIds.Distinct().ToList();
        }

        if (accessScope is null &&
            !string.IsNullOrWhiteSpace(customerId) &&
            (organizationIds?.Count > 0 || teamIds?.Count > 0))
        {
            accessScope = new MarketplaceBookingSubscriptionAccessScope(
                organizationIds ?? [],
                teamIds ?? []);
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain) &&
            searchCriteria.TeamIds.Count == 0)
        {
            organizationIds ??= await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
            teamIds ??= await GetCustomerTeamIdsAsync(customerId, cancellationToken);

            if (organizationIds.Count == 0 && teamIds.Count == 0)
            {
                return (new PaginatedInfo(false, false, null, null), [], 0);
            }

            accessScope = new MarketplaceBookingSubscriptionAccessScope(organizationIds, teamIds);
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                accessScope,
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

    public async Task<MarketplaceBookingSubscription> DeleteAsync(
        string id,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        CancellationToken cancellationToken)
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

        var subscription = await sharedMarketplaceBookingSubscriptionService.DeleteAsync(
            existingSubscription,
            customer,
            cancellationMode,
            cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
            Constants.MarketplaceBookingSubscriptionTopicName,
            subscription.Id,
            cancellationToken);
        return subscription;
    }

    private async Task<List<string>> GetCustomerOrganizationIdsAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customerId, false, false, cancellationToken);
        var result = new List<string>();
        foreach (var organization in organizations)
        {
            if (await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
            {
                result.Add(organization.Id);
            }
        }

        return result;
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(string customerId, CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewMarketplaceBookingSubscriptionAsync(
        Shared.Database.Entities.MarketplaceBookingSubscription subscription,
        string customerId,
        CancellationToken cancellationToken)
    {
        if (subscription.InvolvedCustomers.Any(item => item.Id == customerId))
        {
            return;
        }

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
                if (await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
                {
                    return;
                }
            }
        }

        var teamIds = subscription.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teams)
            {
                if (team.Organization is not null &&
                    await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(team.Organization.Id, customerId, cancellationToken))
                {
                    return;
                }
            }
        }

        throw new UnauthorizedAccessException();
    }

    private async Task<OrganizationEntity?> GetScopedOrganizationAsync(
        MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
        {
            return null;
        }

        return await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                   searchCriteria.OrganizationId,
                   searchCriteria.OrganizationCustomDomain,
                   false,
                   false,
                   cancellationToken) ??
               throw new OrganizationNotFound();
    }

    private static bool IsRequestedOwnSubscriptionsOnly(MarketplaceBookingSubscriptionSearchCriteria searchCriteria, string? customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        searchCriteria.CustomerIds.Count != 0 &&
        searchCriteria.CustomerIds.All(item => item == customerId);

    private static bool IsRequestedOtherCustomersSubscriptions(MarketplaceBookingSubscriptionSearchCriteria searchCriteria, string? customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        searchCriteria.CustomerIds.Count != 0 &&
        searchCriteria.CustomerIds.Any(item => item != customerId);
}
