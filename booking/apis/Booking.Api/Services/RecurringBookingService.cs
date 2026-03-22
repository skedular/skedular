using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IRecurringBookingService
{
    Task<RecurringBooking> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<RecurringBooking>>, int)> GetPaginatedRecurringBookingsAsync(
        PaginationInputParam paginationInputParam,
        RecurringBookingSearchCriteria searchCriteria,
        ICollection<RecurringBookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class RecurringBookingService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    Shared.Mappers.IMapper sharedMapper,
    ICachedRecurringBookingService cachedRecurringBookingService) : IRecurringBookingService
{
    public async Task<RecurringBooking> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var booking = await cachedRecurringBookingService.GetByIdAsync(id, cancellationToken) ?? throw new RecurringBookingNotFound();

        await EnsureCustomerCanViewRecurringBookingAsync(booking, customerId, cancellationToken);

        return sharedMapper.MapTo(booking);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<RecurringBooking>>, int)> GetPaginatedRecurringBookingsAsync(
        PaginationInputParam paginationInputParam,
        RecurringBookingSearchCriteria searchCriteria,
        ICollection<RecurringBookingOrder> orderByFields,
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

        RecurringBookingAccessScope? accessScope = null;
        List<string>? organizationIds = null;
        List<string>? teamIds = null;

        var requestedOwnBookingsOnly = IsRequestedOwnBookingsOnly(searchCriteria, customerId);
        var requestedOtherCustomersBookings = IsRequestedOtherCustomersBookings(searchCriteria, customerId);
        var scopedOrganization = await GetScopedOrganizationAsync(searchCriteria, cancellationToken);

        if (requestedOtherCustomersBookings && scopedOrganization is null)
        {
            throw new InvalidOperationException("You can only look for others' bookings if organization is included in your search");
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            scopedOrganization is not null &&
            (requestedOtherCustomersBookings || !requestedOwnBookingsOnly))
        {
            if (!await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(scopedOrganization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (scopedOrganization is not null)
        {
            accessScope = new RecurringBookingAccessScope(
                [scopedOrganization.Id],
                scopedOrganization.Teams.Where(item => !item.DeletedAt.HasValue).Select(item => item.Id).ToList());

            searchCriteria = searchCriteria with { OrganizationId = null, OrganizationCustomDomain = null };
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.TeamIds.Count != 0)
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
                    organizationIds = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                }

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
            accessScope = new RecurringBookingAccessScope(organizationIds ?? [], teamIds ?? []);
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain) &&
            searchCriteria.TeamIds.Count == 0)
        {
            if (organizationIds is null)
            {
                organizationIds = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
            }

            teamIds ??= await GetCustomerTeamIdsAsync(customerId, cancellationToken);

            if (organizationIds.Count == 0 && teamIds.Count == 0)
            {
                return (new PaginatedInfo(false, false, null, null), [], 0);
            }

            accessScope = new RecurringBookingAccessScope(organizationIds, teamIds);
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.RecurringBookingRepository.GetPaginatedRecurringBookingsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            accessScope,
            cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
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

    private async Task EnsureCustomerCanViewRecurringBookingAsync(
        Shared.Database.Entities.RecurringBooking booking,
        string customerId,
        CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Any(item => item.Id == customerId))
        {
            return;
        }

        var organizationIds = booking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
                organizationIds,
                null,
                false,
                false,
                cancellationToken);
            foreach (var organization in organizationEntities)
            {
                if (await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(organization.Id, customerId, cancellationToken))
                {
                    return;
                }
            }
        }

        var teamIds = booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teamEntities = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teamEntities)
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
        RecurringBookingSearchCriteria searchCriteria,
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

    private static bool IsRequestedOwnBookingsOnly(RecurringBookingSearchCriteria searchCriteria, string? customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        searchCriteria.CustomerIds.Count != 0 &&
        searchCriteria.CustomerIds.All(item => item == customerId);

    private static bool IsRequestedOtherCustomersBookings(RecurringBookingSearchCriteria searchCriteria, string? customerId) =>
        !string.IsNullOrWhiteSpace(customerId) &&
        searchCriteria.CustomerIds.Count != 0 &&
        searchCriteria.CustomerIds.Any(item => item != customerId);
}
