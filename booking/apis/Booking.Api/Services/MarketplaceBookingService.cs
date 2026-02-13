using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using Resource = Booking.Shared.Database.Entities.Resource;

namespace Booking.Api.Services;

public interface IMarketplaceBookingService
{
    Task<Shared.Models.Booking> BookProductAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> UpdateAsync(Shared.Models.Booking booking, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> DeleteAsync(string id, CancellationToken cancellationToken);
}

public class MarketplaceBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    Shared.Mappers.IMapper sharedMapper,
    IContext context,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    ICachedBookingService cachedBookingService,
    Shared.Services.IResourceService sharedResourceService,
    IProductService sharedProductService,
    IPrivateBookingPreferenceService sharedPrivateBookingPreferenceService,
    Shared.Services.IPrivateBookingService sharedPrivateBookingService) : IMarketplaceBookingService
{
    public async Task<Shared.Models.Booking> BookProductAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        if (booking.InvolvedCustomers.Count == 0)
        {
            throw new ArgumentException(nameof(booking.InvolvedCustomers));
        }

        if (booking.LineItems.Count == 0)
        {
            throw new ArgumentException(nameof(booking.LineItems));
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

        if (!productVersions.All(item => item.IsPriceTaxInclusive is null) &&
            !productVersions.All(item => item.IsPriceTaxInclusive!.Value) &&
            productVersions.Any(item => item.IsPriceTaxInclusive!.Value))
        {
            throw new BookingProductWithMixedTaxSetupNotAllowed();
        }

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

        booking.IsPaymentRequired = true;
        booking.PaymentStatus = PaymentStatus.Pending;

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
            [],
            resources,
            booking.IsPaymentRequired ? customer : null,
            null,
            customer,
            null,
            null,
            productVersions,
            null);

        bookingEntity.Channel = BookingChannelConstants.Marketplace;

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
        booking = sharedMapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

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

        return await sharedPrivateBookingService.DeleteAsync(existingBooking, customer, cancellationToken);
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

    private async Task<Shared.Models.Booking> UpdateInternalAsync(
        Shared.Models.Booking booking,
        Shared.Database.Entities.Booking existingBooking,
        Customer callingCustomer,
        CancellationToken cancellationToken)
    {
        var organizations = await GetOrganizationsAndValidatePermissionsAsync(booking, callingCustomer.Id, true, cancellationToken);

        return await sharedPrivateBookingService.UpdateAsync(booking, existingBooking, callingCustomer, organizations, [], cancellationToken);
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
