using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Customer = Booking.Shared.Database.Entities.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using Resource = Booking.Shared.Database.Entities.Resource;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.Services;

public interface IBookingService
{
    Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken);

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
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    IMapper mapper,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService) : IBookingService
{
    public async Task<Shared.Models.Booking> AddAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
        }

        if (booking.LineItems.Any(item => item.Quantity <= 0 || string.IsNullOrWhiteSpace(item.ProductVersionId)))
        {
            throw new ArgumentException(nameof(booking.LineItems));
        }

        var (customer, callingCustomerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(booking.Id))
        {
            var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking is not null)
            {
                return await UpdateInternalAsync(booking, existingBooking, customer, callingCustomerEntity, cancellationToken);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        var teams = await GetTeamAndValidatePermissionsAsync(booking, customer, false, cancellationToken);
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await GetResourcesAsync(booking.From, booking.Until, resourceIds, cancellationToken);
        var productVersions = await GetProductVersionsAsync(booking.LineItems.Select(item => item.ProductVersionId).ToList(), cancellationToken);

        booking.BookedOnMarketplace = booking.LineItems.Count != 0;
        booking.IsPaymentRequired = booking.LineItems.Count != 0;
        booking.PaymentStatus = booking.IsPaymentRequired ? PaymentStatus.Pending : PaymentStatus.Confirmed;

        if (booking.IsPaymentRequired)
        {
            if (!booking.PaymentMethod.HasValue)
            {
                throw new BookingPaymentMethodRequired();
            }

            if (productVersions.Any(item => !item.AcceptedBookingPaymentMethods.Contains(booking.PaymentMethod.Value.ToPaymentMethod())))
            {
                throw new BookingPaymentMethodNotAccepted();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (booking.InvolvedCustomers.Count == 1)
        {
            var (_, customerEntity) = await customerService.GetCustomerAsync(booking.InvolvedCustomers.First().Id, cancellationToken);
            (organizations, teams, resources) = await TryToSetDefaultValuesAsync(
                booking,
                customerEntity,
                organizations,
                teams,
                resources,
                cancellationToken);
        }

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

        var bookingEntity = mapper.MapTo(
            booking,
            customerEntities,
            organizations,
            ResourcesToLocations(resources),
            teams,
            resources,
            booking.IsPaymentRequired ? callingCustomerEntity : null,
            null,
            callingCustomerEntity,
            null,
            null,
            productVersions,
            null);

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingCheckoutSessionExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        if (booking.IsPaymentRequired)
        {
            bookingOutboxPublisher.ExecuteWorkflowPayBookingByCardSession([booking], repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if ((bookingEntity.InvolvedOrganizations.Count == 0 ||
             bookingEntity.InvolvedOrganizations.Any(item => !organizationAuthorizationService.CanViewMemberPersonalDetails(item, customer))) &&
            bookingEntity.InvolvedOrganizations.Any(item =>
                item.MemberVisibilityPolicy == OrganizationMemberVisibilityPolicyConstants.LimitedAccess))
        {
            booking.InvolvedCustomers = booking.InvolvedCustomers.Select(item =>
            {
                item = item.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                foreach (var identity in item.Identities)
                {
                    identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                }

                return item;
            }).ToList();
        }

        return booking;
    }

    public async Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var (customer, callingCustomerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken) ?? throw new BookingNotFound();

        return await UpdateInternalAsync(booking, existingBooking, customer, callingCustomerEntity, cancellationToken);
    }

    public async Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, callingCustomerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        var organizationIds = existingBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, false, false, cancellationToken);
            if (!organizations.Any(item => organizationAuthorizationService.CanDeleteBooking(item, customer)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var teamIds = existingBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            if (!teams.Any(item => teamAuthorizationService.CanDeleteBooking(item, customer)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

        existingBooking.DeletedByCustomer = callingCustomerEntity;
        existingBooking = repositoryFactory.BookingRepository.Update(existingBooking);
        var deletedBooking = mapper.MapTo(
            repositoryFactory.BookingRepository.Remove(existingBooking),
            bookingCheckoutSessionHelperService.GetBookingCheckoutSessionExpiry(existingBooking));

        bookingOutboxPublisher.PublishBookings([deletedBooking], repositoryFactory.UnitOfWork);

        if (existingBooking.IsPaymentRequired)
        {
            bookingOutboxPublisher.SignalWorkflowPayBookingUsingStripeCheckoutSessionDeleteBooking(deletedBooking.Id, repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if ((existingBooking.InvolvedOrganizations.Count == 0 ||
             existingBooking.InvolvedOrganizations.Any(item => !organizationAuthorizationService.CanViewMemberPersonalDetails(item, customer))) &&
            existingBooking.InvolvedOrganizations.Any(item =>
                item.MemberVisibilityPolicy == OrganizationMemberVisibilityPolicyConstants.LimitedAccess))
        {
            deletedBooking.InvolvedCustomers = deletedBooking.InvolvedCustomers.Select(item =>
            {
                item = item.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                foreach (var identity in item.Identities)
                {
                    identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                }

                return item;
            }).ToList();
        }

        return deletedBooking;
    }

    public async Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        await EnsureCustomerCanViewBookingAsync(booking, customer, cancellationToken);
        var result = mapper.MapTo(booking, bookingCheckoutSessionHelperService.GetBookingCheckoutSessionExpiry(booking));

        if ((booking.InvolvedOrganizations.Count == 0 ||
             booking.InvolvedOrganizations.Any(item => !organizationAuthorizationService.CanViewMemberPersonalDetails(item, customer))) &&
            booking.InvolvedOrganizations.Any(item =>
                item.MemberVisibilityPolicy == OrganizationMemberVisibilityPolicyConstants.LimitedAccess))
        {
            result.InvolvedCustomers = result.InvolvedCustomers.Select(item =>
            {
                item = item.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                foreach (var identity in item.Identities)
                {
                    identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                }

                return item;
            }).ToList();
        }

        return result;
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

            if (searchCriteria.CustomerIds.Any(customerId =>
                    !organizationCustomerPairs.Keys.Any(item => organizationCustomerPairs[item].Contains(customerId))))
            {
                throw new UnauthorizedAccessException();
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
                throw new UnauthorizedAccessException();
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
                if (organizationIds is null)
                {
                    var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                    organizationIds = organizationCustomerPairs.Keys.ToList();
                }

                if (location.Organization is null || !organizationIds.Contains(location.Organization.Id))
                {
                    throw new UnauthorizedAccessException();
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
                if (organizationIds is null)
                {
                    var organizationCustomerPairs = await GetCustomerOrganizationIdsAsync(customer, cancellationToken);
                    organizationIds = organizationCustomerPairs.Keys.ToList();
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

        var result = (paginatedInfo,
            edges.Select(item => mapper.MapTo(item, bookingCheckoutSessionHelperService.GetBookingCheckoutSessionExpiry(item.Node))).ToList(),
            totalCount);
        if (customer is null)
        {
            return result;
        }

        foreach (var booking in result.Item2
                     .Select(item => item.Node)
                     .Where(item => !item.InvolvedCustomers.Select(involvedCustomer => involvedCustomer.Id).Contains(customer.Id)))
        {
            var bookingEntity = edges.Select(item => item.Node).First(item => item.Id == booking.Id);
            if ((bookingEntity.InvolvedOrganizations.Count == 0 ||
                 bookingEntity.InvolvedOrganizations.Any(item => !organizationAuthorizationService.CanViewMemberPersonalDetails(item, customer))) &&
                bookingEntity.InvolvedOrganizations.Any(item =>
                    item.MemberVisibilityPolicy == OrganizationMemberVisibilityPolicyConstants.LimitedAccess))
            {
                booking.InvolvedCustomers = booking.InvolvedCustomers.Select(item =>
                {
                    item = item.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                    foreach (var identity in item.Identities)
                    {
                        identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                    }

                    return item;
                }).ToList();
            }
        }

        return result;
    }

    private async Task<ICollection<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        Shared.Models.Customer customer,
        bool existing,
        CancellationToken cancellationToken)
    {
        var organizationIds = booking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count == 0)
        {
            return [];
        }

        var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, false, false, cancellationToken);
        if (organizationIds.Count != organizationEntities.Count)
        {
            throw new OrganizationNotFound();
        }

        var result = new List<Organization>();
        foreach (var organization in booking.InvolvedOrganizations)
        {
            var organizationEntity = organizationEntities.First(item => item.Id == organization.Id);
            if (existing)
            {
                if (!organizationAuthorizationService.CanUpdateBooking(organizationEntity, customer))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!organizationAuthorizationService.CanAddBooking(organizationEntity, customer))
                {
                    throw new UnauthorizedAccessException();
                }

                if (!organizationOfferingService.IsMoreInteractionAllowed(organizationEntity, customer))
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
        Shared.Models.Customer customer,
        bool existing,
        CancellationToken cancellationToken)
    {
        var teamIds = booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count == 0)
        {
            return [];
        }

        var teamEntities = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
        if (teamIds.Count != teamEntities.Count)
        {
            throw new TeamNotFound();
        }

        var result = new List<Team>();
        foreach (var team in booking.InvolvedTeams)
        {
            var teamEntity = teamEntities.First(item => item.Id == team.Id);
            if (existing)
            {
                if (!teamAuthorizationService.CanUpdateBooking(teamEntity, customer))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!teamAuthorizationService.CanAddBooking(teamEntity, customer))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            result.Add(teamEntity);
        }

        return result;
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
        var locations = await repositoryFactory.LocationRepository.GetByCustomerIdAsync(customer.Id, false, cancellationToken);
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
        var organizationIds = booking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
        if (organizationIds.Count != 0)
        {
            var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, false, false, cancellationToken);
            if (!organizationEntities.Any(item => organizationAuthorizationService.CanViewBookings(item, customer)))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var teamIds = booking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
        if (teamIds.Count != 0)
        {
            var teamEntities = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, false, cancellationToken);
            if (!teamEntities.Any(item => teamAuthorizationService.CanViewBookings(item, customer)))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Shared.Models.Customer customer,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        var teams = await GetTeamAndValidatePermissionsAsync(booking, customer, true, cancellationToken);
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now first remove all existing resource as part of the transaction to make later resource availability simpler
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await GetResourcesAsync(booking.From, booking.Until, resourceIds, cancellationToken);

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

        booking.LineItems = existingBooking.LineItems;
        booking.BookedOnMarketplace = existingBooking.BookedOnMarketplace;
        booking.IsPaymentRequired = existingBooking.IsPaymentRequired;
        booking.PaymentStatus = existingBooking.PaymentStatus.ToPaymentStatus();
        booking.PaymentMethod = existingBooking.PaymentMethod.ToNullablePaymentMethod();
        booking.InvoiceUrl = existingBooking.InvoiceUrl;

        var bookingEntity = mapper.MergeTo(
            booking,
            existingBooking,
            customerEntities,
            organizations,
            ResourcesToLocations(resources),
            teams,
            resources,
            null,
            null,
            existingBooking.CreatedByCustomer,
            callingCustomer,
            null,
            existingBooking.ProductVersions,
            existingBooking.StripeCheckoutSession);

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingCheckoutSessionExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if ((bookingEntity.InvolvedOrganizations.Count == 0 ||
             bookingEntity.InvolvedOrganizations.Any(item => !organizationAuthorizationService.CanViewMemberPersonalDetails(item, customer))) &&
            bookingEntity.InvolvedOrganizations.Any(item =>
                item.MemberVisibilityPolicy == OrganizationMemberVisibilityPolicyConstants.LimitedAccess))
        {
            booking.InvolvedCustomers = booking.InvolvedCustomers.Select(item =>
            {
                item = item.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                foreach (var identity in item.Identities)
                {
                    identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                }

                return item;
            }).ToList();
        }

        return booking;
    }

    private async Task<(ICollection<Organization>, ICollection<Team>, ICollection<Resource>)> TryToSetDefaultValuesAsync(
        Shared.Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        ICollection<Resource> resources,
        CancellationToken cancellationToken)
    {
        if (booking.Resources.Count != 0 || (booking.InvolvedOrganizations.Count != 0 && booking.InvolvedTeams.Count != 0) ||
            (booking.InvolvedOrganizations.Count != 0 && booking.InvolvedOrganizations.Count != 1) ||
            (booking.InvolvedTeams.Count != 0 && booking.InvolvedTeams.Count != 1))
        {
            return (organizations, teams, resources);
        }

        var organization = booking.InvolvedOrganizations.FirstOrDefault();
        var team = booking.InvolvedTeams.FirstOrDefault();
        var organizationEntity = organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, false, false, cancellationToken);
        Location? locationEntity = null;
        Team? teamEntity = null;

        if ((organization is null && team is not null) || (organization is not null && team is not null))
        {
        }
        else if (organization is not null && team is null)
        {
            locationEntity =
                customer.PreferredLocations.FirstOrDefault(item => item.Organization is not null && item.Organization.Id == organization.Id);
            teamEntity = customer.PreferredTeams.FirstOrDefault(item => item.Organization is not null && item.Organization.Id == organization.Id);
        }
        else
        {
            if (customer.DefaultOrganization is null)
            {
                teamEntity = customer.PreferredTeams.FirstOrDefault();
                if (team is null)
                {
                    locationEntity = customer.PreferredLocations.FirstOrDefault();
                    if (locationEntity is not null)
                    {
                        locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, false, cancellationToken);
                    }
                }

                if (team is not null && team.Organization is not null)
                {
                    organizationEntity =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(team.Organization.Id, false, false, cancellationToken);
                }
                else if (locationEntity is not null)
                {
                    organizationEntity = locationEntity.Organization;
                }
            }
            else
            {
                organizationEntity = customer.DefaultOrganization;
                locationEntity = customer.PreferredLocations.FirstOrDefault(item =>
                    item.Organization is not null && item.Organization.Id == customer.DefaultOrganization.Id);
                if (locationEntity is not null)
                {
                    locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(locationEntity.Id, false, cancellationToken);
                }

                teamEntity = customer.PreferredTeams.FirstOrDefault(item =>
                    item.Organization is not null && item.Organization.Id == customer.DefaultOrganization.Id);
                if (team is not null)
                {
                    teamEntity = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, false, cancellationToken);
                }
            }
        }

        if (locationEntity is null)
        {
            return (organizationEntity is null ? [] : [organizationEntity], teamEntity is null ? [] : [teamEntity], resources);
        }

        resources = resources.Where(item => item.Location is { DeletedAt: null } && item.Location.Id == locationEntity.Id).ToList();
        if (resources.Count != 0)
        {
            return (organizationEntity is null ? [] : [organizationEntity], teamEntity is null ? [] : [teamEntity], resources);
        }

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            locationEntity.Id,
            booking.From,
            booking.Until,
            [],
            [],
            [OrganizationTagTypeConstants.ResourceDesk],
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
                resources = resource is null ? availableResources.Count != 0 ? [availableResources.First()] : [] : [resource];
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

        return (organizationEntity is null ? [] : [organizationEntity], teamEntity is null ? [] : [teamEntity], resources);
    }

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

    private async Task<ICollection<ProductVersion>> GetProductVersionsAsync(List<string> productVersionIds, CancellationToken cancellationToken)
    {
        if (productVersionIds.Count == 0)
        {
            return [];
        }

        productVersionIds = productVersionIds.Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);

        return productVersions.Count != productVersionIds.Count || !productVersions.All(item => productVersionIds.Contains(item.Id))
            ? throw new ProductNotFound()
            : productVersions;
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
