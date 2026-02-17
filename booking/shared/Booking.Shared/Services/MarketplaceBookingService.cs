using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingService
{
    Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    ICachedBookingService cachedBookingService,
    IResourceService resourceService,
    IPrivateBookingPreferenceService privateBookingPreferenceService,
    IProductService productService,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IMarketplaceBookingService
{
    public async Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken)
    {
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
            booking.From,
            booking.Until,
            resourceIds,
            cancellationToken);
        var productVersions = await productService.GetProductVersionsAsync(
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
            (organizations, resources) = await privateBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                booking.InvolvedCustomers.First().Id,
                booking.From,
                booking.Until,
                booking.InvolvedOrganizations
                    .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                    .Select(item => item.Id)
                    .ToList(),
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

        bookingEntity.Channel = BookingChannelConstants.Marketplace;

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(bookingEntity));

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

    public async Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken)
    {
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace)
        {
            throw new BookingIsNotMarketplace();
        }

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
        var resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
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
        booking.Channel = existingBooking.Channel.ToBookingChannel();
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
            lastModifiedByCustomer,
            null,
            existingBooking.ProductVersions,
            existingBooking.StripeCheckoutSession);

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return booking;
    }

    public async Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken)
    {
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace)
        {
            throw new BookingIsNotMarketplace();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

        existingBooking.DeletedByCustomer = deletedByCustomer;
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

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
