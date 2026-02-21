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
using Customer = Booking.Shared.Database.Entities.Customer;
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

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var booking = await cachedBookingService.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();

        await EnsureCustomerCanViewBookingAsync(booking, customer, cancellationToken);

        return sharedMapper.MapTo(booking);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
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
        List<string>? organizationUniqueAlphanumericNames = null;
        List<string>? locationIds = null;
        List<string>? teamIds = null;

        if (searchCriteria.CustomerIds.Count != 0 &&
            customer is not null &&
            searchCriteria.CustomerIds.Any(item => item != customer.Id) &&
            searchCriteria.OrganizationIds.Count == 0 && searchCriteria.OrganizationUniqueAlphanumericNames.Count == 0)
        {
            throw new InvalidOperationException("You can only look for others' bookings if organization is included in your search");
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
            searchCriteria.OrganizationUniqueAlphanumericNames.Count != 0)
        {
            var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
            organizationUniqueAlphanumericNames = organizationCustomerPairs.Item2.Keys.ToList();

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
        else if (customer is not null && searchCriteria.OrganizationUniqueAlphanumericNames.Count != 0)
        {
            if (organizationUniqueAlphanumericNames is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationUniqueAlphanumericNames = organizationCustomerPairs.Item2.Keys.ToList();
            }

            if (searchCriteria.OrganizationUniqueAlphanumericNames.Any(item => !organizationUniqueAlphanumericNames.Contains(item)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        if (customer is not null && searchCriteria.LocationIds.Count != 0)
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
                    var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                    organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
                }

                if (location.Organization is null || !organizationIds.Contains(location.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        if (customer is not null && searchCriteria.TeamIds.Count != 0)
        {
            var criteria = searchCriteria;
            var teams = await repositoryFactory.TeamRepository.Query(
                    new Specification<Team> { Criteria = query => !query.DeletedAt.HasValue && criteria.LocationIds.Contains(query.Id) }
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
            searchCriteria.OrganizationUniqueAlphanumericNames.Count == 0 &&
            searchCriteria.LocationIds.Count == 0 &&
            searchCriteria.TeamIds.Count == 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationIds = organizationCustomerPairs.Item1.Keys.ToList();
            }

            locationIds ??= await GetCustomerLocationIdsAsync(customer, cancellationToken);
            teamIds ??= await GetCustomerTeamIdsAsync(customer, cancellationToken);

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
        Customer customer,
        CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customer.Id, false, false, cancellationToken);

        return (organizations.ToDictionary(
                item => item.Id, item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList()),
            organizations
                .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
                .ToDictionary(
                    item => item.UniqueAlphanumericName!,
                    item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList()));
    }

    private async Task<List<string>> GetCustomerLocationIdsAsync(Customer customer, CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(customer.Id, false, cancellationToken);
        return locations.Select(item => item.Id).ToList();
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(Customer customer, CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewBookingAsync(
        Shared.Database.Entities.Booking booking,
        Customer customer,
        CancellationToken cancellationToken)
    {
        var organizationIds = booking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
                organizationIds,
                null,
                false,
                false,
                cancellationToken);
            foreach (var organization in organizationEntities)
            {
                if (!await organizationAuthorizationService.CanViewBookingsAsync(organization.Id, customer.Id, cancellationToken))
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
                if (!await teamAuthorizationService.CanViewBookingsAsync(team, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
