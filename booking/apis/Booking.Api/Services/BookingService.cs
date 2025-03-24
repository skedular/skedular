using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Customer = Booking.Shared.Database.Entities.Customer;
using Desk = Booking.Shared.Database.Entities.Desk;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using Resource = Booking.Shared.Database.Entities.Resource;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IBookingService
{
    Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string bookingId, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> GetByIdAsync(string bookingId, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Booking>>, int )> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class BookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationAuthorizationService locationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    IMapper mapper) : IBookingService
{
    public async Task<Shared.Models.Booking> AddAsync(
        Shared.Models.Booking booking,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Shared.Models.Customer? customer = null;
        Customer? customerEntity = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(booking.Id))
        {
            var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking is not null)
            {
                return await UpdateInternalAsync(booking, existingBooking, customer, customerEntity, cancellationToken);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organization = await GetOrganizationAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        var location = await GetLocationAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        var team = await GetTeamAndValidatePermissionsAsync(booking, customer, false, cancellationToken);

        var customerIds = booking.Resources.SelectMany(item => item.Customers).Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.Query(
                new Specification<Customer> { Criteria = query => !query.DeletedAt.HasValue && customerIds.Contains(query.Id) })
            .ToListAsync(cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await GetResourcesAsync(booking.From, booking.Until, resourceIds, cancellationToken);
        if (customer is null || customer.Id != booking.Customer.Id)
        {
            (_, customerEntity) = await customerService.GetCustomerAsync(booking.Customer.Id, cancellationToken);
        }

        (organization, location, team, resources) = await TryToSetDefaultValuesAsync(
            booking,
            customerEntity!,
            organization,
            location,
            team,
            resources,
            cancellationToken);

        organization = PopulateRequiredFields(organization, location, team);

        foreach (var resource in resources)
        {
            var matchingResource = booking.Resources.FirstOrDefault(item => item.Resource.Id == resource.Id);
            if (matchingResource is null)
            {
                continue;
            }

            var matchingCustomerEntities = customerEntities.Where(item => matchingResource.Customers.Select(x => x.Id).Contains(item.Id)).ToList();

            foreach (var slot in resource.ResourceBookingSlots)
            {
                foreach (var matchingCustomerEntity in matchingCustomerEntities
                             .Where(matchingCustomerEntity => !slot.Customers.Select(item => item.Id).Contains(matchingCustomerEntity.Id)))
                {
                    slot.Customers.Add(matchingCustomerEntity);
                }
            }

            repositoryFactory.ResourceBookingSlotRepository.UpdateRange(resource.ResourceBookingSlots);
        }

        var bookingEntity = mapper.MapTo(booking, customerEntity!, organization, location, team, [], [], resources);

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
        booking = mapper.MapTo(bookingEntity);

        await bookingOutboxPublisher.PublishBookingAsync([booking], repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return booking;
    }

    public async Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
        if (existingBooking is null)
        {
            throw new BookingNotFound();
        }

        return await UpdateInternalAsync(booking, existingBooking, customer, customerEntity, cancellationToken);
    }

    public async Task<Shared.Models.Booking> DeleteAsync(string bookingId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (existingBooking is null)
        {
            throw new BookingNotFound();
        }

        if (existingBooking.Organization is not null)
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                existingBooking.Organization.Id,
                false,
                false,
                cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (customer.Id == existingBooking.Customer.Id)
            {
                if (!organizationAuthorizationService.CanDeleteBooking(organization, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!organizationAuthorizationService.CanDeleteBookingOnBehalf(organization, customer))
                {
                    throw new Unauthorized();
                }
            }
        }

        if (existingBooking.Location is not null)
        {
            var location =
                await repositoryFactory.LocationRepository.GetByIdAsync(existingBooking.Location.Id, false, false, false, false, cancellationToken);
            if (location is null)
            {
                throw new LocationNotFound();
            }

            if (customer.Id == existingBooking.Customer.Id)
            {
                if (!locationAuthorizationService.CanDeleteBooking(location, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!locationAuthorizationService.CanDeleteBookingOnBehalf(location, customer))
                {
                    throw new Unauthorized();
                }
            }
        }

        if (existingBooking.Team is not null)
        {
            var team = await repositoryFactory.TeamRepository.GetByIdAsync(existingBooking.Team.Id, false, cancellationToken);
            if (team is null)
            {
                throw new TeamNotFound();
            }

            if (customer.Id == existingBooking.Customer.Id)
            {
                if (!teamAuthorizationService.CanDeleteBooking(team, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!teamAuthorizationService.CanDeleteBookingOnBehalf(team, customer))
                {
                    throw new Unauthorized();
                }
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        RemoveAllSlotsFromBooking(existingBooking);

        _ = repositoryFactory.BookingRepository.Update(existingBooking);
        var deletedBooking = mapper.MapTo(repositoryFactory.BookingRepository.Remove(existingBooking));

        await bookingOutboxPublisher.PublishBookingAsync([deletedBooking], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedBooking;
    }

    public async Task<Shared.Models.Booking> GetByIdAsync(string bookingId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            throw new BookingNotFound();
        }

        await EnsureCustomerCanViewBookingAsync(booking, customer, cancellationToken);
        return mapper.MapTo(booking);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Booking>>, int)> GetPaginatedBookingsAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Shared.Models.Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        }

        if (customer is not null && searchCriteria.IncludeMineOnly.HasValue && searchCriteria.IncludeMineOnly.Value)
        {
            searchCriteria.CustomerIds = [customer.Id];
        }

        List<string>? organizationIds = null;
        List<string>? locationIds = null;
        List<string>? teamIds = null;

        if (searchCriteria.CustomerIds.Count != 0 &&
            customer is not null &&
            searchCriteria.CustomerIds.Any(item => item != customer.Id) &&
            searchCriteria.OrganizationIds.Count == 0)
        {
            throw new InvalidOperationException("You can only look for others' bookings if organization is included in your search");
        }

        if (searchCriteria.CustomerIds.Count != 0 &&
            customer is not null &&
            searchCriteria.CustomerIds.Any(item => item != customer.Id) &&
            searchCriteria.OrganizationIds.Count != 0)
        {
            var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
            organizationIds = organizationCustomerPairs.Keys.ToList();

            if (searchCriteria.CustomerIds.Any(
                    customerId => !organizationCustomerPairs.Keys.Any(item => organizationCustomerPairs[item].Contains(customerId))))
            {
                throw new Unauthorized();
            }
        }

        if (customer is not null && searchCriteria.OrganizationIds.Count != 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationIds = organizationCustomerPairs.Keys.ToList();
            }

            if (searchCriteria.OrganizationIds.Any(item => !organizationIds.Contains(item)))
            {
                throw new Unauthorized();
            }
        }

        if (customer is not null && searchCriteria.LocationIds.Count != 0)
        {
            var locations = await repositoryFactory.LocationRepository.Query(
                    new Specification<Location> { Criteria = query => !query.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(query.Id) }
                        .AddInclude(query => query.Organization))
                .ToListAsync(cancellationToken);

            foreach (var location in locations)
            {
                if (location.Organization is null)
                {
                    locationIds ??= await GetCustomerLocationIdsAsync(customer, cancellationToken);
                    if (!locationIds.Contains(location.Id))
                    {
                        throw new Unauthorized();
                    }
                }
                else
                {
                    if (organizationIds is null)
                    {
                        var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                        organizationIds = organizationCustomerPairs.Keys.ToList();
                    }

                    if (!organizationIds.Contains(location.Organization.Id))
                    {
                        throw new Unauthorized();
                    }
                }
            }
        }

        if (customer is not null && searchCriteria.TeamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.Query(
                    new Specification<Team> { Criteria = query => !query.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(query.Id) }
                        .AddInclude(query => query.Organization))
                .ToListAsync(cancellationToken);

            foreach (var team in teams)
            {
                if (team.Organization is null)
                {
                    teamIds ??= await GetCustomerTeamIdsAsync(customer, cancellationToken);
                    if (!teamIds.Contains(team.Id))
                    {
                        throw new Unauthorized();
                    }
                }
                else
                {
                    if (organizationIds is null)
                    {
                        var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                        organizationIds = organizationCustomerPairs.Keys.ToList();
                    }

                    if (!organizationIds.Contains(team.Organization.Id))
                    {
                        throw new Unauthorized();
                    }
                }
            }
        }

        if (customer is not null &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            searchCriteria.OrganizationIds.Count == 0 &&
            searchCriteria.LocationIds.Count == 0 &&
            searchCriteria.TeamIds.Count == 0)
        {
            if (organizationIds is null)
            {
                var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                organizationIds = organizationCustomerPairs.Keys.ToList();
            }

            locationIds ??= await GetCustomerLocationIdsAsync(customer, cancellationToken);
            teamIds ??= await GetCustomerTeamIdsAsync(customer, cancellationToken);

            if (organizationIds.Count == 0 && locationIds.Count == 0 && teamIds.Count == 0)
            {
                return (new PaginatedInfo(false, false, null, null), [], 0);
            }

            searchCriteria.OrganizationIds = organizationIds;
            searchCriteria.LocationIds = locationIds;
            searchCriteria.TeamIds = teamIds;
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.BookingRepository.GetPaginatedBookingsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

    private async Task<Organization?> GetOrganizationAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        Shared.Models.Customer? customer,
        bool existing,
        CancellationToken cancellationToken)
    {
        if (booking.Organization is null)
        {
            return null;
        }

        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(booking.Organization.Id, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (customer is null)
        {
            return organization;
        }

        if (customer.Id == booking.Customer.Id)
        {
            if (existing)
            {
                if (!organizationAuthorizationService.CanUpdateBooking(organization, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!organizationAuthorizationService.CanAddBooking(organization, customer))
                {
                    throw new Unauthorized();
                }
            }
        }
        else
        {
            if (existing)
            {
                if (!organizationAuthorizationService.CanUpdateBookingOnBehalf(organization, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!organizationAuthorizationService.CanAddBookingOnBehalf(organization, customer))
                {
                    throw new Unauthorized();
                }
            }
        }

        if (!organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        return organization;
    }

    private async Task<Location?> GetLocationAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        Shared.Models.Customer? customer,
        bool existing,
        CancellationToken cancellationToken)
    {
        if (booking.Location is null)
        {
            return null;
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(booking.Location.Id, false, false, false, false, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (customer is null)
        {
            return location;
        }

        if (customer.Id == booking.Customer.Id)
        {
            if (existing)
            {
                if (!locationAuthorizationService.CanUpdateBooking(location, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!locationAuthorizationService.CanAddBooking(location, customer))
                {
                    throw new Unauthorized();
                }
            }
        }
        else
        {
            if (existing)
            {
                if (!locationAuthorizationService.CanUpdateBookingOnBehalf(location, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!locationAuthorizationService.CanAddBookingOnBehalf(location, customer))
                {
                    throw new Unauthorized();
                }
            }
        }

        return location;
    }

    private async Task<Team?> GetTeamAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        Shared.Models.Customer? customer,
        bool existing,
        CancellationToken cancellationToken)
    {
        if (booking.Team is null)
        {
            return null;
        }

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(booking.Team.Id, false, cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (customer is null)
        {
            return team;
        }

        if (customer.Id == booking.Customer.Id)
        {
            if (existing)
            {
                if (!teamAuthorizationService.CanUpdateBooking(team, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!teamAuthorizationService.CanAddBooking(team, customer))
                {
                    throw new Unauthorized();
                }
            }
        }
        else
        {
            if (existing)
            {
                if (!teamAuthorizationService.CanUpdateBookingOnBehalf(team, customer))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (!teamAuthorizationService.CanAddBookingOnBehalf(team, customer))
                {
                    throw new Unauthorized();
                }
            }
        }

        return team;
    }

    private async Task<IDictionary<string, List<string>>> GetCustomerOrganizationIdsAsync(
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customer.Id, false, false, cancellationToken);

        return organizations.ToDictionary(
            item => item.Id,
            item => item.OrganizationMembers.Select(organizationMember => organizationMember.Customer.Id).ToList());
    }

    private async Task<List<string>> GetCustomerLocationIdsAsync(Shared.Models.Customer customer, CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(customer.Id, false, false, false, false, cancellationToken);
        return locations.Select(item => item.Id).ToList();
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(Shared.Models.Customer customer, CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewBookingAsync(
        Shared.Database.Entities.Booking booking,
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        if (booking.Organization is not null)
        {
            var organization =
                await repositoryFactory.OrganizationRepository.GetByIdAsync(booking.Organization.Id, false, false, cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!organizationAuthorizationService.CanViewBookings(organization, customer))
            {
                throw new Unauthorized();
            }

            return;
        }

        if (booking.Location is not null)
        {
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(booking.Location.Id, false, false, false, false,
                cancellationToken);
            if (location is null)
            {
                throw new OrganizationNotFound();
            }

            if (!locationAuthorizationService.CanViewBookings(location, customer))
            {
                throw new Unauthorized();
            }
        }

        if (booking.Team is not null)
        {
            var team = await repositoryFactory.TeamRepository.GetByIdAsync(booking.Team.Id, false, cancellationToken);
            if (team is null)
            {
                throw new OrganizationNotFound();
            }

            if (!teamAuthorizationService.CanViewBookings(team, customer))
            {
                throw new Unauthorized();
            }
        }
    }

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Shared.Models.Customer? customer,
        Customer? customerEntity,
        CancellationToken cancellationToken)
    {
        var organization = await GetOrganizationAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        var location = await GetLocationAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        var team = await GetTeamAndValidatePermissionsAsync(booking, customer, true, cancellationToken);

        var customerIds = booking.Resources.SelectMany(item => item.Customers).Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.Query(
                new Specification<Customer> { Criteria = query => !query.DeletedAt.HasValue && customerIds.Contains(query.Id) })
            .ToListAsync(cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now first remove all existing resource as part of the transaction to make later resource availability simpler
        RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await GetResourcesAsync(booking.From, booking.Until, resourceIds, cancellationToken);

        if (customer is null || customer.Id != booking.Customer.Id)
        {
            (_, customerEntity) = await customerService.GetCustomerAsync(booking.Customer.Id, cancellationToken);
        }

        organization = PopulateRequiredFields(organization, location, team);

        foreach (var resource in resources)
        {
            var matchingResource = booking.Resources.FirstOrDefault(item => item.Resource.Id == resource.Id);
            if (matchingResource is null)
            {
                continue;
            }

            var matchingCustomerEntities = customerEntities.Where(item => matchingResource.Customers.Select(x => x.Id).Contains(item.Id)).ToList();

            foreach (var slot in resource.ResourceBookingSlots)
            {
                foreach (var matchingCustomerEntity in matchingCustomerEntities
                             .Where(matchingCustomerEntity => !slot.Customers.Select(item => item.Id).Contains(matchingCustomerEntity.Id)))
                {
                    slot.Customers.Add(matchingCustomerEntity);
                }
            }

            repositoryFactory.ResourceBookingSlotRepository.UpdateRange(resource.ResourceBookingSlots);
        }

        var bookingEntity = mapper.MergeTo(booking, existingBooking, customerEntity!, organization, location, team, [], [], resources);

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity);

        await bookingOutboxPublisher.PublishBookingAsync([booking], repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return booking;
    }

    private async Task<(Organization?, Location?, Team?, ICollection<Resource>)> TryToSetDefaultValuesAsync(
        Shared.Models.Booking booking,
        Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Resource> resources,
        CancellationToken cancellationToken)
    {
        if (booking.Organization is not null && booking.Location is not null && booking.Team is not null && booking.Resources.Count != 0)
        {
            // Only use default values if given booking has no attachment to any of the resources available through default values
            return (organization, location, team, resources);
        }

        if (booking.Organization is not null && booking.Location is null && booking.Team is null)
        {
            location = customer.PreferredLocations
                .FirstOrDefault(item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (location is not null)
            {
                location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, false, false, false, false, cancellationToken);
            }

            team = customer.PreferredTeams.FirstOrDefault(item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (team is not null)
            {
                team = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, false, cancellationToken);
            }
        }
        else if (booking.Organization is not null && booking.Location is not null && booking.Team is null)
        {
            team = customer.PreferredTeams.FirstOrDefault(item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (team is not null)
            {
                team = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, false, cancellationToken);
            }
        }
        else if (booking.Organization is not null && booking.Location is null && booking.Team is not null)
        {
            location = customer.PreferredLocations.FirstOrDefault(
                item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (location is not null)
            {
                location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, false, false, false, false, cancellationToken);
            }
        }
        else if (booking.Organization is null && (booking.Location is not null || booking.Team is not null))
        {
        }
        else if (booking.Organization is not null && booking.Location is not null && booking.Team is not null)
        {
        }
        else
        {
            if (customer.DefaultOrganization is null)
            {
                team = customer.PreferredTeams.FirstOrDefault(item => item.Organization is not null) ?? customer.PreferredTeams.FirstOrDefault();
                if (team is null)
                {
                    location = customer.PreferredLocations.FirstOrDefault(item => item.Organization is not null) ??
                               customer.PreferredLocations.FirstOrDefault();
                    if (location is not null)
                    {
                        location =
                            await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, false, false, false, false, cancellationToken);
                    }
                }

                if (team is not null)
                {
                    organization = team.Organization;
                }
                else if (location is not null)
                {
                    organization = location.Organization;
                }
            }
            else
            {
                organization = customer.DefaultOrganization;
                location = customer.PreferredLocations
                    .FirstOrDefault(item => item.Organization is not null && item.Organization.Id == customer.DefaultOrganization.Id);
                if (location is not null)
                {
                    location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, false, false, false, false, cancellationToken);
                }

                team = customer.PreferredTeams.FirstOrDefault(
                    item => item.Organization is not null && item.Organization.Id == customer.DefaultOrganization.Id);
                if (team is not null)
                {
                    team = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, false, cancellationToken);
                }
            }
        }

        if (location is not null)
        {
            resources = resources.Where(item => item.Location is { DeletedAt: null } && item.Location.Id == location.Id).ToList();
            if (resources.Count != 0)
            {
                return (organization, location, team, resources);
            }

            var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                organization?.Id,
                location.Id,
                booking.From,
                booking.Until,
                [],
                [],
                [OrganizationTagTypeConstants.Desk],
                cancellationToken);

            var resource = availableResources
                .FirstOrDefault(item => customer.PreferredResources.Select(preferredResource => preferredResource.Id).Contains(item.Id));
            if (resource is null)
            {
                var preferredZones = customer.PreferredOrganizationTags
                    .Where(tag => tag.Type == OrganizationTagTypeConstants.Zone)
                    .Select(tag => tag.Id)
                    .ToList();
                resource = availableResources.FirstOrDefault(item => item.OrganizationTags.Any(tag => preferredZones.Contains(tag.Id)));
                if (resource is null)
                {
                    var preferredTags = customer.PreferredOrganizationTags
                        .Where(tag => tag.Type == OrganizationTagTypeConstants.Custom)
                        .Select(tag => tag.Id)
                        .ToList();
                    resource = availableResources.FirstOrDefault(item => item.OrganizationTags.Any(tag => preferredTags.Contains(tag.Id)));
                    if (resource is null)
                    {
                        if (availableResources.Count != 0)
                        {
                            resources = [availableResources.First()];
                        }
                    }
                    else
                    {
                        resources = [resource];
                    }
                }
                else
                {
                    resources = [resource];
                }
            }
            else
            {
                resources = [resource];
            }
        }

        return (organization, location, team, resources);
    }

    private static Organization? PopulateRequiredFields(Organization? organization, Location? location, Team? team) =>
        organization switch
        {
            null when location?.Organization is not null => location.Organization,
            null when team?.Organization is not null => team.Organization,
            _ => organization
        };

    private async Task<ICollection<Resource>> GetResourcesAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        List<string> resourceIds,
        CancellationToken cancellationToken)
    {
        if (resourceIds.Count == 0)
        {
            return [];
        }

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            null,
            from,
            until,
            resourceIds,
            [],
            [],
            cancellationToken);

        return availableResources.Count != resourceIds.Count || !availableResources.All(item => resourceIds.Contains(item.Id))
            ? throw new ResourceNotAvailable()
            : availableResources;
    }

    private void RemoveAllSlotsFromBooking(Shared.Database.Entities.Booking booking)
    {
        foreach (var slot in booking.ResourceBookingSlots)
        {
            slot.Customers.Clear();
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(booking.ResourceBookingSlots);
        booking.ResourceBookingSlots.Clear();
        repositoryFactory.BookingRepository.Update(booking);
    }
}
