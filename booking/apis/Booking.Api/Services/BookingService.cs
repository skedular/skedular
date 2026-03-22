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
using Location = Booking.Shared.Database.Entities.Location;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IBookingService
{
    Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Booking>>, int )> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class BookingService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IMapper mapper,
    Shared.Mappers.IMapper sharedMapper,
    ICachedBookingService cachedBookingService) : IBookingService
{
    public async Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var booking = await cachedBookingService.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();

        await EnsureCustomerCanViewBookingAsync(booking, customerId, cancellationToken);

        return sharedMapper.MapTo(booking);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.IncludeMineOnly.HasValue)
        {
            searchCriteria = searchCriteria with { CustomerIds = [customerId] };
        }

        List<string>? organizationIds = null;
        List<string>? organizationCustomDomains = null;
        List<string>? locationIds = null;
        List<string>? teamIds = null;

        if (searchCriteria.CustomerIds.Count != 0 &&
            !string.IsNullOrWhiteSpace(customerId) &&
            searchCriteria.CustomerIds.Any(item => item != customerId) &&
            searchCriteria.OrganizationIds.Count == 0 && searchCriteria.OrganizationCustomDomains.Count == 0)
        {
            throw new InvalidOperationException("You can only look for others' bookings if organization is included in your search");
        }

        if (searchCriteria.CustomerIds.Count != 0 &&
            !string.IsNullOrWhiteSpace(customerId) &&
            searchCriteria.CustomerIds.Any(item => item != customerId) &&
            searchCriteria.OrganizationIds.Count != 0)
        {
            var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
            organizationIds = organizationCustomerPairs.Item1.Keys.ToList();

            if (searchCriteria.CustomerIds
                .Any(item => !organizationCustomerPairs.Item1.Keys.Any(key => organizationCustomerPairs.Item1[key].Contains(item))))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (searchCriteria.CustomerIds.Count != 0 &&
            !string.IsNullOrWhiteSpace(customerId) &&
            searchCriteria.CustomerIds.Any(item => item != customerId) &&
            searchCriteria.OrganizationCustomDomains.Count != 0)
        {
            var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
            organizationCustomDomains = organizationCustomerPairs.Item2.Keys.ToList();

            if (searchCriteria.CustomerIds
                .Any(item => !organizationCustomerPairs.Item2.Keys.Any(key => organizationCustomerPairs.Item2[key].Contains(item))))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.OrganizationIds.Count != 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
            }

            if (searchCriteria.OrganizationIds.Any(item => !organizationIds.Contains(item)))
            {
                throw new UnauthorizedAccessException();
            }
        }
        else if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.OrganizationCustomDomains.Count != 0)
        {
            if (organizationCustomDomains is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                organizationCustomDomains = organizationCustomerPairs.Item2.Keys.ToList();
            }

            if (searchCriteria.OrganizationCustomDomains.Any(item => !organizationCustomDomains.Contains(item)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (!string.IsNullOrWhiteSpace(customerId) && searchCriteria.LocationIds.Count != 0)
        {
            var criteria = searchCriteria;
            var locations = await repositoryFactory.LocationRepository.Query(
                    new Specification<Location> { Criteria = query => !query.DeletedAt.HasValue && criteria.LocationIds.Contains(query.Id) }
                        .AddInclude(query => query.Organization!))
                .ToListAsync(cancellationToken);

            foreach (var location in locations)
            {
                if (organizationIds is null)
                {
                    var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                    organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
                }

                if (location.Organization is null || !organizationIds.Contains(location.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }
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
                    var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                    organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
                }

                if (team.Organization is null || !organizationIds.Contains(team.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            searchCriteria.OrganizationIds.Count == 0 &&
            searchCriteria.OrganizationCustomDomains.Count == 0 &&
            searchCriteria.LocationIds.Count == 0 &&
            searchCriteria.TeamIds.Count == 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customerId, cancellationToken);
                organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
            }

            locationIds ??= await GetCustomerLocationIdsAsync(customerId, cancellationToken);
            teamIds ??= await GetCustomerTeamIdsAsync(customerId, cancellationToken);

            if (organizationIds.Count == 0 && locationIds.Count == 0 && teamIds.Count == 0)
            {
                return (new PaginatedInfo(false, false, null, null), [], 0);
            }

            searchCriteria = searchCriteria with { OrganizationIds = organizationIds, LocationIds = locationIds, TeamIds = teamIds };
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.BookingRepository.GetPaginatedBookingsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

    private async Task<(IDictionary<string, List<string>>, IDictionary<string, List<string>>)> GetCustomerOrganizationIdsAsync(
        string customerId,
        CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customerId, false, false, cancellationToken);

        return (organizations.ToDictionary(
                item => item.Id, item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList()),
            organizations
                .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                .ToDictionary(
                    item => item.CustomDomain!,
                    item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList()));
    }

    private async Task<List<string>> GetCustomerLocationIdsAsync(string customerId, CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(customerId, false, cancellationToken);
        return locations.Select(item => item.Id).ToList();
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(string customerId, CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewBookingAsync(
        Shared.Database.Entities.Booking booking,
        string customerId,
        CancellationToken cancellationToken)
    {
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
                if (!await organizationAuthorizationService.CanViewBookingsAsync(organization.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        var teamIds = booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teamEntities = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            foreach (var team in teamEntities)
            {
                if (!await teamAuthorizationService.CanViewBookingsAsync(team, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
