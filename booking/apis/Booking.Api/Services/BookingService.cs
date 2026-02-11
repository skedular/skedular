using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Constants = Booking.Shared.GraphQL.Constants;
using Customer = Booking.Shared.Database.Entities.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
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
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    IContext context,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    ICachedBookingService cachedBookingService,
    IGraphQlHelperService graphQlHelperService,
    Shared.Services.IResourceService sharedResourceService,
    IProductService sharedProductService,
    IPrivateBookingPreferenceService sharedPrivateBookingPreferenceService) : IBookingService
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

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        if (!string.IsNullOrWhiteSpace(booking.Id))
        {
            var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking is not null)
            {
                return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
            }
        }
        else
        {
            booking.Id = randomHelper.Generate();
        }

        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, customer.Id, false, cancellationToken);
        var teams = await GetTeamAndValidatePermissionsAsync(booking, customer.Id, false, cancellationToken);
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await sharedResourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
            booking.From,
            booking.Until,
            resourceIds,
            cancellationToken);
        var productVersions = await sharedProductService.GetProductVersionsAsync(
            booking.LineItems.Select(item => item.ProductVersionId).ToList(),
            cancellationToken);

        var organizationIds = productVersions.Select(item => item.Product.Organization.Id).Distinct().ToList();
        if (organizationIds.Count > 1)
        {
            throw new CrossOrganizationProductBookingNotAllowed();
        }

        if (!productVersions.All(item => item.IsPriceTaxInclusive is null) && !productVersions.All(item => item.IsPriceTaxInclusive!.Value) &&
            productVersions.Any(item => item.IsPriceTaxInclusive!.Value))
        {
            throw new BookingProductWithMixedTaxSetupNotAllowed();
        }

        if (booking.LineItems.Count != 0)
        {
            // TODO: 20260211 : Morteza: The current implementation does not work when different products with different resources are selected, as it only validates the total quantity and ignores the requested resource types.
            var maxAllowedResourcesToBook = booking.LineItems
                .Select(item =>
                {
                    var matchedProductVersion = productVersions.First(productVersion => productVersion.Id == item.ProductVersionId);

                    return item.Quantity * matchedProductVersion.NumberOfResourcesToBook;
                }).Sum();

            if (resourceIds.Count > maxAllowedResourcesToBook!.Value)
            {
                throw new MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking();
            }
        }

        booking.BookedOnMarketplace = booking.LineItems.Count != 0;
        booking.IsPaymentRequired = booking.LineItems.Count != 0;
        booking.PaymentStatus = booking.IsPaymentRequired ? PaymentStatus.Pending : PaymentStatus.NoPaymentRequired;

        if (booking.IsPaymentRequired)
        {
            if (!booking.PaymentMethod.HasValue)
            {
                throw new PaymentMethodRequired();
            }

            if (productVersions.Any(item =>
                    !item.AcceptedBookingPaymentMethods.ToSafeCollection().Contains(booking.PaymentMethod.Value.ToPaymentMethod())))
            {
                throw new BookingPaymentMethodNotAccepted();
            }

            var currencies = productVersions.Select(item => item.Currency).Distinct().ToList();
            if (currencies.Count > 1)
            {
                throw new BookingsProductsWithMultipleCurrenciesAreNotSupported();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (booking.InvolvedCustomers.Count == 1)
        {
            (organizations, resources) = await sharedPrivateBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                booking.InvolvedCustomers.First().Id,
                booking.From,
                booking.Until,
                booking.InvolvedOrganizations
                    .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
                    .Select(item => item.UniqueAlphanumericName!)
                    .ToList(),
                organizations,
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
            booking.IsPaymentRequired ? customer : null,
            null,
            customer,
            null,
            null,
            productVersions,
            null);

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        if (booking.IsPaymentRequired)
        {
            switch (booking.PaymentMethod)
            {
                case PaymentMethod.Card:
                    temporalOutboxService.StartWorkflowPayBookingViaCard(
                        new PayBookingViaCardInput(
                            booking.Id,
                            booking.PaymentExpiry,
                            booking.InvoiceEmailList.ToSafeCollection()), repositoryFactory.UnitOfWork);
                    break;

                case PaymentMethod.BankTransfer:
                    temporalOutboxService.StartWorkflowPayBookingViaBankTransfer(
                        new PayBookingViaBankTransferInput(
                            booking.Id,
                            booking.PaymentExpiry,
                            booking.InvoiceEmailList.ToSafeCollection()),
                        repositoryFactory.UnitOfWork);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        return booking;
    }

    public async Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken) ?? throw new BookingNotFound();

        return await UpdateInternalAsync(booking, existingBooking, customer, cancellationToken);
    }

    public async Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        var organizationIds = existingBooking.InvolvedOrganizations.Select(item => item.Id).Distinct().ToList();
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

        var teamIds = existingBooking.InvolvedTeams.Select(item => item.Id).Distinct().ToList();
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

        existingBooking.DeletedByCustomer = customer;
        existingBooking = repositoryFactory.BookingRepository.Update(existingBooking);
        var deletedBooking = mapper.MapTo(
            repositoryFactory.BookingRepository.Remove(existingBooking),
            bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(existingBooking));

        bookingOutboxPublisher.PublishBookings([deletedBooking], repositoryFactory.UnitOfWork);

        if (existingBooking.IsPaymentRequired)
        {
            if (!deletedBooking.PaymentMethod.HasValue)
            {
                throw new PaymentMethodRequired();
            }

            switch (deletedBooking.PaymentMethod)
            {
                case PaymentMethod.Card:
                    temporalOutboxService.SignalWorkflowPayBookingViaCardDeleteBooking(deletedBooking.Id, repositoryFactory.UnitOfWork);
                    break;

                case PaymentMethod.BankTransfer:
                    temporalOutboxService.SignalWorkflowPayBookingViaBankTransferDeleteBooking(deletedBooking.Id, repositoryFactory.UnitOfWork);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.RemoveByIdAsync(deletedBooking.Id, cancellationToken);

        return deletedBooking;
    }

    public async Task<Shared.Models.Booking> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var booking = await cachedBookingService.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();

        await EnsureCustomerCanViewBookingAsync(booking, customer, cancellationToken);

        return mapper.MapTo(booking, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(booking));
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

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.BookingRepository.GetPaginatedBookingsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo,
            edges.Select(item => mapper.MapTo(item, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(item.Node))).ToList(),
            totalCount);
    }

    private async Task<ICollection<Organization>> GetOrganizationsAndValidatePermissionsAsync(
        Shared.Models.Booking booking,
        string customerId,
        bool existing,
        CancellationToken cancellationToken)
    {
        var organizationIds = booking.InvolvedOrganizations
            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
            .Select(item => item.Id)
            .Distinct()
            .ToList();
        var uniqueAlphanumericNames = booking.InvolvedOrganizations
            .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
            .Select(item => item.UniqueAlphanumericName!)
            .Distinct()
            .ToList();

        if (organizationIds.Count == 0 && uniqueAlphanumericNames.Count == 0)
        {
            return [];
        }

        var organizationEntities = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            uniqueAlphanumericNames,
            false,
            false,
            cancellationToken);
        if (organizationIds.Count + uniqueAlphanumericNames.Count != organizationEntities.Count)
        {
            throw new OrganizationNotFound();
        }

        var result = new List<Organization>();
        foreach (var organization in booking.InvolvedOrganizations)
        {
            var organizationEntity = organizationEntities.First(item =>
                item.Id == organization.Id || item.UniqueAlphanumericName == organization.UniqueAlphanumericName);
            if (existing)
            {
                if (!await organizationAuthorizationService.CanUpdateBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await organizationAuthorizationService.CanAddBookingAsync(organizationEntity.Id, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }

                if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organizationEntity.Id, customerId, cancellationToken))
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
        string customerId,
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
                if (!await teamAuthorizationService.CanUpdateBookingAsync(teamEntity, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await teamAuthorizationService.CanAddBookingAsync(teamEntity, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            result.Add(teamEntity);
        }

        return result;
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

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, callingCustomer.Id, true, cancellationToken);
        var teams = await GetTeamAndValidatePermissionsAsync(booking, callingCustomer.Id, true, cancellationToken);
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await sharedResourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
            booking.From,
            booking.Until,
            resourceIds,
            cancellationToken);

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
        booking.InvoiceNumber = existingBooking.InvoiceNumber;
        booking.TotalAmountExcludeTax = existingBooking.TotalAmountExcludeTax;
        booking.TaxAmount = existingBooking.TaxAmount;
        booking.TaxRatePercentage = existingBooking.TaxRatePercentage;
        booking.TotalAmount = existingBooking.TotalAmount;
        booking.Currency = existingBooking.Currency;

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
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        await graphQlHelperService.RaiseGraphqlChange(Constants.BookingTopicName, booking.Id, cancellationToken);

        return booking;
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
