using Api.Shared.Models;
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
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IBookingService
{
    Task<Shared.Models.Booking> AddAsync(
        Shared.Models.Booking booking,
        bool ignoreAuthorizationCheck,
        bool ignoreDeskAvailability,
        CancellationToken cancellationToken);

    Task<Shared.Models.Booking> UpdateAsync(
        Shared.Models.Booking booking,
        bool ignoreDeskAvailability,
        CancellationToken cancellationToken);

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
        bool ignoreDeskAvailability,
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
            var existingBooking =
                await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking is not null)
            {
                return await UpdateInternalAsync(
                    booking,
                    existingBooking,
                    customer,
                    customerEntity,
                    ignoreDeskAvailability,
                    cancellationToken);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organization =
            await GetOrganizationAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        var location = await GetLocationAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        var team = await GetTeamAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        (var desks, location) =
            await GetDesksAndSetLocationIfRequiredAsync(booking, location, ignoreDeskAvailability, cancellationToken);

        if (customer is null || customer.Id != booking.Customer.Id)
        {
            (_, customerEntity) = await customerService.GetCustomerAsync(booking.Customer.Id, cancellationToken);
        }

        (organization, location, team, desks) = await TryToSetDefaultValuesAsync(booking, customerEntity!, organization,
            location, team, desks, cancellationToken);
        organization = PopulateRequiredFields(organization, location, team);

        var bookingEntity = mapper.MapTo(booking, customerEntity!, organization, location, team, desks);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.BookingRepository.UnitOfWork,
            cancellationToken);

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
        booking = mapper.MapTo(bookingEntity);

        await bookingOutboxPublisher.PublishBookingAsync(
            [booking],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return booking;
    }

    public async Task<Shared.Models.Booking> DeleteAsync(
        string bookingId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking =
            await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (existingBooking is null)
        {
            throw new BookingNotFound();
        }

        if (existingBooking.Organization is not null)
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                existingBooking.Organization.Id,
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
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(
                existingBooking.Location.Id,
                cancellationToken);
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
            var team = await repositoryFactory.TeamRepository.GetByIdAsync(
                existingBooking.Team.Id,
                cancellationToken);
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

        existingBooking.Desks = [];

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.BookingRepository.UnitOfWork,
            cancellationToken);
        _ = repositoryFactory.BookingRepository.Update(existingBooking);
        var deletedBooking = mapper.MapTo(repositoryFactory.BookingRepository.Remove(existingBooking));

        await bookingOutboxPublisher.PublishBookingAsync(
            [deletedBooking],
            repositoryFactory.BookingRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedBooking;
    }

    public async Task<Shared.Models.Booking> GetByIdAsync(
        string bookingId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var booking =
            await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
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

        List<string>? organizationIds = null;
        List<string>? locationIds = null;
        List<string>? teamIds = null;

        if (customer is not null && searchCriteria.OrganizationIds.Count != 0)
        {
            organizationIds ??= await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
            if (searchCriteria.OrganizationIds.Any(item => !organizationIds.Contains(item)))
            {
                throw new Unauthorized();
            }
        }

        if (customer is not null && searchCriteria.LocationIds.Count != 0)
        {
            var locations = await repositoryFactory.LocationRepository.Query(new Specification<Location>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(query.Id)
                    }
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
                    organizationIds ??= await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                    if (!organizationIds.Contains(location.Organization.Id))
                    {
                        throw new Unauthorized();
                    }
                }
            }
        }

        if (customer is not null && searchCriteria.TeamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.Query(new Specification<Team>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(query.Id)
                    }
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
                    organizationIds ??= await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                    if (!organizationIds.Contains(team.Organization.Id))
                    {
                        throw new Unauthorized();
                    }
                }
            }
        }

        if (customer is not null && searchCriteria.IncludeMineOnly.HasValue && searchCriteria.IncludeMineOnly.Value)
        {
            searchCriteria.CustomerId = customer.Id;
        }

        if (customer is not null &&
            (!searchCriteria.IncludeMineOnly.HasValue || !searchCriteria.IncludeMineOnly.Value) &&
            searchCriteria.OrganizationIds.Count == 0 &&
            searchCriteria.LocationIds.Count == 0 &&
            searchCriteria.TeamIds.Count == 0)
        {
            organizationIds ??= await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
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

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.BookingRepository.GetPaginatedBookingsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

    public async Task<Shared.Models.Booking> UpdateAsync(
        Shared.Models.Booking booking,
        bool ignoreDeskAvailability,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking =
            await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
        if (existingBooking is null)
        {
            throw new BookingNotFound();
        }

        return await UpdateInternalAsync(
            booking,
            existingBooking,
            customer,
            customerEntity,
            ignoreDeskAvailability,
            cancellationToken);
    }

    private async Task<(List<Desk> desks, Location? location)> GetDesksAndSetLocationIfRequiredAsync(
        Shared.Models.Booking booking,
        Location? location,
        bool ignoreDeskAvailability,
        CancellationToken cancellationToken)
    {
        var desks = new List<Desk>();
        if (booking.Desks.Count == 0)
        {
            return (desks, location);
        }

        var deskIds = booking.Desks.Select(item => item.Id).ToList();
        desks = await repositoryFactory.DeskRepository
            .Query(new Specification<Desk>
                {
                    Criteria = query => !query.DeletedAt.HasValue &&
                                        !query.Deactivated &&
                                        query.Location != null &&
                                        !query.Location.DeletedAt.HasValue &&
                                        deskIds.Contains(query.Id)
                }
                .AddInclude(query => query.Location))
            .ToListAsync(cancellationToken);

        if ((!ignoreDeskAvailability && desks.Count != booking.Desks.Count) || desks.Any(item => item.Deactivated))
        {
            throw new DeskNotAvailable();
        }

        deskIds = desks.Select(item => item.Id).ToList();
        var existingBookingWithSameDesksFound = await repositoryFactory.BookingRepository.Query(
            new Specification<Shared.Database.Entities.Booking>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Id != booking.Id &&
                                    query.From >= booking.From &&
                                    query.To <= booking.To &&
                                    query.Desks.Any(item => deskIds.Contains(item.Id))
            }).AnyAsync(cancellationToken);
        if (!ignoreDeskAvailability && existingBookingWithSameDesksFound)
        {
            throw new DeskNotAvailable();
        }

        var locations = desks
            .Where(item => item.Location is not null)
            .Select(item => item.Location!.Id)
            .ToHashSet();
        if (locations.Count > 1)
        {
            throw new CrossLocationDeskBookingNotAllowed();
        }

        if (location is null && locations.Count != 0)
        {
            location = desks.Select(item => item.Location).FirstOrDefault(item => item is not null);
        }
        else if (location is not null && locations.Count != 0)
        {
            if (location.Id != locations.First())
            {
                throw new DeskBelongToDifferentLocationBookingNotAllowed();
            }
        }

        return (desks, location);
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

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
            booking.Organization.Id,
            cancellationToken);
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

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(
            booking.Location.Id,
            cancellationToken);
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

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(
            booking.Team.Id,
            cancellationToken);
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

    private async Task<List<string>> GetCustomerOrganizationIdsAsync(
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        var organizationMembers =
            await repositoryFactory.OrganizationMemberRepository.GetByCustomerIdAsync(
                customer.Id,
                cancellationToken);
        return organizationMembers.Select(item => item.Organization.Id).ToList();
    }

    private async Task<List<string>> GetCustomerLocationIdsAsync(
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        var locations =
            await repositoryFactory.LocationRepository.GetByCustomerIdAsync(
                customer.Id,
                cancellationToken);
        return locations.Select(item => item.Id).ToList();
    }

    private async Task<List<string>> GetCustomerTeamIdsAsync(
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        var teams =
            await repositoryFactory.TeamRepository.GetByCustomerIdAsync(
                customer.Id,
                cancellationToken);
        return teams.Select(item => item.Id).ToList();
    }

    private async Task EnsureCustomerCanViewBookingAsync(
        Shared.Database.Entities.Booking booking,
        Shared.Models.Customer customer, CancellationToken cancellationToken)
    {
        if (booking.Organization is not null)
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                booking.Organization.Id,
                cancellationToken);
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
            var location = await repositoryFactory.LocationRepository.GetByIdAsync(
                booking.Location.Id,
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
            var team = await repositoryFactory.TeamRepository.GetByIdAsync(
                booking.Team.Id,
                cancellationToken);
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
        bool ignoreDeskAvailability,
        CancellationToken cancellationToken)
    {
        var organization =
            await GetOrganizationAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        var location = await GetLocationAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        var team = await GetTeamAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        (var desks, location) =
            await GetDesksAndSetLocationIfRequiredAsync(booking, location, ignoreDeskAvailability, cancellationToken);

        if (customer is null || customer.Id != booking.Customer.Id)
        {
            (_, customerEntity) = await customerService.GetCustomerAsync(booking.Customer.Id, cancellationToken);
        }

        organization = PopulateRequiredFields(organization, location, team);

        var bookingEntity = mapper.MergeTo(
            booking,
            existingBooking,
            customerEntity!,
            organization,
            location,
            team,
            desks);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.BookingRepository.UnitOfWork,
            cancellationToken);

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity);

        await bookingOutboxPublisher.PublishBookingAsync(
            [booking],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.BookingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return booking;
    }

    private async Task<(Organization?, Location?, Team?, List<Desk>)> TryToSetDefaultValuesAsync(
        Shared.Models.Booking booking,
        Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        List<Desk> desks,
        CancellationToken cancellationToken)
    {
        if (booking.Organization is not null &&
            booking.Location is not null &&
            booking.Team is not null &&
            booking.Desks.Count != 0)
        {
            // Only use default values if given booking has no attachment to any of the resources available through default values
            return (organization, location, team, desks);
        }

        if (booking.Organization is not null &&
            booking.Location is null &&
            booking.Team is null)
        {
            location = customer.DefaultLocations
                .FirstOrDefault(
                    item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (location is not null)
            {
                location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
            }

            team = customer.DefaultTeams
                .FirstOrDefault(
                    item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (team is not null)
            {
                team = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
            }
        }
        else if (booking.Organization is not null &&
                 booking.Location is not null &&
                 booking.Team is null)
        {
            team = customer.DefaultTeams
                .FirstOrDefault(
                    item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (team is not null)
            {
                team = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
            }
        }
        else if (booking.Organization is not null &&
                 booking.Location is null &&
                 booking.Team is not null)
        {
            location = customer.DefaultLocations
                .FirstOrDefault(
                    item => item.Organization is not null && item.Organization.Id == booking.Organization.Id);
            if (location is not null)
            {
                location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
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
                team = customer.DefaultTeams.FirstOrDefault(item => item.Organization is not null) ??
                       customer.DefaultTeams.FirstOrDefault();

                if (team is null)
                {
                    location = customer.DefaultLocations.FirstOrDefault(item => item.Organization is not null) ??
                               customer.DefaultLocations.FirstOrDefault();
                    if (location is not null)
                    {
                        location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id,
                            cancellationToken);
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
                location = customer.DefaultLocations
                    .FirstOrDefault(
                        item => item.Organization is not null &&
                                item.Organization.Id == customer.DefaultOrganization.Id);
                if (location is not null)
                {
                    location = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                }

                team = customer.DefaultTeams
                    .FirstOrDefault(
                        item => item.Organization is not null &&
                                item.Organization.Id == customer.DefaultOrganization.Id);
                if (team is not null)
                {
                    team = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                }
            }
        }

        if (organization is null && location is null && team is null && booking.Desks.Count != 0)
        {
            desks = [];
        }

        if (location is not null)
        {
            desks = desks.Where(item => item.Location is { DeletedAt: null } && item.Location.Id == location.Id)
                .ToList();

            if (desks.Count == 0)
            {
                var bookings = await repositoryFactory.BookingRepository
                    .Query(new Specification<Shared.Database.Entities.Booking>
                        {
                            Criteria = query =>
                                !query.DeletedAt.HasValue &&
                                booking.From <= query.From && query.To <= booking.To
                        }
                        .AddInclude(query => query.Desks))
                    .Where(query => query.Location != null)
                    .Where(query => query.Location!.Id == location.Id)
                    .ToListAsync(cancellationToken);

                var desk = GetFirstAvailableDeskUsingPreferredDesks(bookings, customer, location);
                if (desk is null)
                {
                    desk = GetFirstAvailableDeskUsingPreferredOrganizationZones(bookings, customer, location);
                    if (desk is null)
                    {
                        desk = GetFirstAvailableDesk(bookings, location);
                        if (desk is not null)
                        {
                            desks = [desk];
                        }
                    }
                    else
                    {
                        desks = [desk];
                    }
                }
                else
                {
                    desks = [desk];
                }
            }
        }

        return (organization, location, team, desks);
    }

    private static Desk? GetFirstAvailableDeskUsingPreferredDesks(
        ICollection<Shared.Database.Entities.Booking> bookings,
        Customer customer,
        Location location)
    {
        var preferredDeskIds = customer.PreferredDesks
            .Where(item =>
                item.Location != null && item.Location.Id == location.Id)
            .Select(item => item.Id)
            .ToList();
        if (preferredDeskIds.Count == 0)
        {
            return null;
        }

        var allDesks = location.Desks.Where(item => preferredDeskIds.Contains(item.Id)).ToList();
        if (allDesks.Count == 0)
        {
            return null;
        }

        var bookedDeskIds = bookings.SelectMany(item => item.Desks).Select(item => item.Id).ToHashSet();
        var allDeskIds = allDesks.Select(item => item.Id).ToList();
        var locationDesksIds = location.Desks.Select(item => item.Id).ToList();
        var availableDeskIds = allDeskIds.Except(bookedDeskIds).ToList();
        if (availableDeskIds.Count == 0)
        {
            availableDeskIds = locationDesksIds.Except(bookedDeskIds).ToList();
        }

        return availableDeskIds.Count == 0 ? null : location.Desks.First(item => item.Id == availableDeskIds.First());
    }

    private static Desk? GetFirstAvailableDeskUsingPreferredOrganizationZones(
        ICollection<Shared.Database.Entities.Booking> bookings,
        Customer customer,
        Location location)
    {
        var preferredZoneIds = customer.PreferredOrganizationTags
            .Where(item => item.Type == OrganizationTagType.Zone)
            .Select(item => item.Id)
            .ToList();
        if (preferredZoneIds.Count == 0)
        {
            return null;
        }

        var allDesks = location.Desks
            .Where(item => item.OrganizationTags.Any(tag => preferredZoneIds.Contains(tag.Id)))
            .ToList();

        if (allDesks.Count == 0)
        {
            return null;
        }

        var bookedDeskIds = bookings.SelectMany(item => item.Desks).Select(item => item.Id).ToHashSet();
        var allDeskIds = allDesks.Select(item => item.Id).ToList();
        var locationDesksIds = location.Desks.Select(item => item.Id).ToList();
        var availableDeskIds = allDeskIds.Except(bookedDeskIds).ToList();
        if (availableDeskIds.Count == 0)
        {
            availableDeskIds = locationDesksIds.Except(bookedDeskIds).ToList();
        }

        return availableDeskIds.Count == 0 ? null : location.Desks.First(item => item.Id == availableDeskIds.First());
    }

    private static Desk? GetFirstAvailableDesk(
        ICollection<Shared.Database.Entities.Booking> bookings,
        Location location)
    {
        var allDesks = location.Desks.ToList();
        if (allDesks.Count == 0)
        {
            return null;
        }

        var bookedDeskIds = bookings.SelectMany(item => item.Desks).Select(item => item.Id).ToHashSet();
        var allDeskIds = allDesks.Select(item => item.Id).ToList();
        var locationDesksIds = location.Desks.Select(item => item.Id).ToList();
        var availableDeskIds = allDeskIds.Except(bookedDeskIds).ToList();
        if (availableDeskIds.Count == 0)
        {
            availableDeskIds = locationDesksIds.Except(bookedDeskIds).ToList();
        }

        return availableDeskIds.Count == 0 ? null : location.Desks.First(item => item.Id == availableDeskIds.First());
    }

    private static Organization? PopulateRequiredFields(Organization? organization, Location? location, Team? team)
    {
        if (organization is null && location?.Organization is not null)
        {
            organization = location.Organization;
        }
        else if (organization is null && team?.Organization is not null)
        {
            organization = team.Organization;
        }

        return organization;
    }
}
