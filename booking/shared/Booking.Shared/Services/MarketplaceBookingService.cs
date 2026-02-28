using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingService
{
    Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        bool bookResourceIfNoResourceProvidedOrAvailable,
        CancellationToken cancellationToken);

    Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);

    Task AdjustRequiredResourcesAsync(Database.Entities.Booking booking, CancellationToken cancellationToken);
}

public class MarketplaceBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    ICachedBookingService cachedBookingService,
    IResourceService resourceService,
    IMarketplaceBookingPreferenceService marketplaceBookingPreferenceService,
    IProductService productService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    TimeProvider timeProvider,
    IRandomHelper randomHelper) : IMarketplaceBookingService
{
    public async Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken)
    {
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var productVersions = await productService.GetProductVersionsAsync(
            marketplaceBooking.LineItems.Select(item => item.ProductVersionId).ToList(),
            cancellationToken);
        var resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
            booking.From,
            booking.Until,
            resourceIds,
            // TODO: 20260218 : Morteza: We currently only support a single product version per booking. This should be changed to support multiple product versions in the future. 
            productVersions.Single().ProductTags.Select(item => item.Id).ToList(),
            cancellationToken);

        if (productVersions.Any(item => item.ProductTags.Count == 0))
        {
            throw new ProductMissingProductTag();
        }

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
        var maxAllowedResourcesToBook = marketplaceBooking.LineItems
            .Select(item =>
            {
                var matchedProductVersion = productVersions.First(productVersion => productVersion.Id == item.ProductVersionId);

                return item.Quantity * matchedProductVersion.NumberOfResourcesToBook;
            }).Sum();

        if (resourceIds.Count > maxAllowedResourcesToBook!.Value)
        {
            throw new MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking();
        }

        marketplaceBooking.Id = randomHelper.Generate();
        marketplaceBooking.IsPaymentRequired = true;
        marketplaceBooking.PaymentStatus = PaymentStatus.Pending;

        if (productVersions.Any(item =>
                !item.AcceptedBookingPaymentMethods.ToSafeCollection().Contains(marketplaceBooking.PaymentMethod.ToPaymentMethod())))
        {
            throw new BookingPaymentMethodNotAccepted();
        }

        var currencies = productVersions.Select(item => item.Currency).Distinct().ToList();
        if (currencies.Count > 1)
        {
            throw new BookingsProductsWithMultipleCurrenciesAreNotSupported();
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (resources.Count == 0)
        {
            resources = await marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                booking.InvolvedCustomers.Count == 1 ? customerEntities.First() : null,
                booking.From,
                booking.Until,
                // TODO: 20260218 : Morteza: We currently only support a single product version per booking. This should be changed to support multiple product versions in the future. 
                productVersions.Single(),
                marketplaceBooking.LineItems.First().Quantity,
                cancellationToken);
        }

        var slots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        foreach (var slot in slots)
        {
            foreach (var matchingCustomerEntity in customerEntities)
            {
                slot.Customers.Add(matchingCustomerEntity);
            }
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(slots);

        var marketplaceBookingEntity = mapper.MapTo(
            marketplaceBooking,
            customer,
            null,
            productVersions,
            null);

        var paymentExpiry = timeProvider
            .GetUtcNow()
            .TrimAllAfterSeconds()
            .AddMinutes(GetBookingPaymentExpiryInMinutes(productVersions, marketplaceBooking.PaymentMethod));

        marketplaceBookingEntity.PaymentExpiry = paymentExpiry;

        marketplaceBookingEntity = repositoryFactory.MarketplaceBookingRepository.Add(marketplaceBookingEntity);

        var bookingEntity = mapper.MapTo(
            booking,
            customerEntities,
            organizations,
            ResourcesToLocations(resources),
            teams,
            resources,
            customer,
            null,
            null,
            marketplaceBookingEntity,
            recurringBooking);

        bookingEntity.Channel = BookingChannelConstants.Marketplace;

        bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);

        booking = mapper.MapTo(bookingEntity);

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        switch (marketplaceBooking.PaymentMethod)
        {
            case PaymentMethod.Card:
                temporalOutboxService.StartWorkflowPayBookingViaCard(
                    new PayBookingViaCardInput(
                        booking.Id,
                        paymentExpiry,
                        marketplaceBooking.InvoiceEmailList.ToSafeCollection()), repositoryFactory.UnitOfWork);
                break;

            case PaymentMethod.BankTransfer:
                temporalOutboxService.StartWorkflowPayBookingViaBankTransfer(
                    new PayBookingViaBankTransferInput(
                        booking.Id,
                        paymentExpiry,
                        marketplaceBooking.InvoiceEmailList.ToSafeCollection()),
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
        RecurringBooking? recurringBooking,
        bool bookResourceIfNoResourceProvidedOrAvailable,
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

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var marketplaceBooking = existingBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersions = await productService.GetProductVersionsAsync(
            marketplaceBooking.LineItems.Select(item => item.ProductVersionId).ToList(),
            cancellationToken);

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        ICollection<Resource> resources;

        // For non-customized recurring instances, the scheduler can request a best-effort rebooking.
        // In that mode we first try resources provided on the booking model, and only if none are
        // currently available, we fall back to preference-based auto assignment (the same strategy as AddAsync).
        if (bookResourceIfNoResourceProvidedOrAvailable && existingBooking.HasRecurringInstanceOverrides != true)
        {
            resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                booking.From,
                booking.Until,
                resourceIds,
                [],
                [],
                cancellationToken);

            // If no requested resource is available, try to auto-pick one by customer preference.
            if (resources.Count == 0 && booking.InvolvedCustomers.Count == 1)
            {
                resources = await marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                    customerEntities.First(),
                    booking.From,
                    booking.Until,
                    // TODO: 20260218 : Morteza: We currently only support a single product version per booking. This should be changed to support multiple product versions in the future. 
                    productVersions.Single(),
                    marketplaceBooking.LineItems.First().Quantity,
                    cancellationToken);
            }
        }
        else
        {
            // Non-recurring or customized instances keep strict behavior:
            // caller-provided resources must all be available.
            resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
                booking.From,
                booking.Until,
                resourceIds,
                // TODO: 20260218 : Morteza: We currently only support a single product version per booking. This should be changed to support multiple product versions in the future. 
                productVersions.Single().ProductTags.Select(item => item.Id).ToList(),
                cancellationToken);
        }

        var slots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        foreach (var slot in slots)
        {
            foreach (var matchingCustomerEntity in customerEntities)
            {
                slot.Customers.Add(matchingCustomerEntity);
            }
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(slots);

        var bookingEntity = mapper.MergeTo(
            booking,
            existingBooking,
            customerEntities,
            organizations,
            ResourcesToLocations(resources),
            teams,
            resources,
            existingBooking.CreatedByCustomer,
            lastModifiedByCustomer,
            null,
            existingBooking.MarketplaceBooking,
            recurringBooking);

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity);

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

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

        existingBooking.DeletedByCustomer = deletedByCustomer;
        existingBooking = repositoryFactory.BookingRepository.Update(existingBooking);
        var deletedBooking = mapper.MapTo(repositoryFactory.BookingRepository.Remove(existingBooking));

        bookingOutboxPublisher.PublishBookings([deletedBooking], repositoryFactory.UnitOfWork);

        var marketplaceBooking = existingBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (marketplaceBooking.IsPaymentRequired)
        {
            switch (marketplaceBooking.PaymentMethod.ToPaymentMethod())
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

    public async Task AdjustRequiredResourcesAsync(Database.Entities.Booking booking, CancellationToken cancellationToken)
    {
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersions = await productService.GetProductVersionsAsync(
            marketplaceBooking.LineItems.Select(item => item.ProductVersionId).ToList(),
            cancellationToken);

        var resourceIds = booking.InvolvedResources.Select(item => item.Id).ToList();
        var resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            null,
            booking.From,
            booking.Until,
            resourceIds,
            [],
            [],
            cancellationToken);

        // If no requested resource is available, try to auto-pick one by customer preference.
        if (resources.Count == 0 && booking.InvolvedCustomers.Count == 1)
        {
            resources = await marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                customerEntities.First(),
                booking.From,
                booking.Until,
                // TODO: 20260218 : Morteza: We currently only support a single product version per booking. This should be changed to support multiple product versions in the future. 
                productVersions.Single(),
                marketplaceBooking.LineItems.First().Quantity,
                cancellationToken);
        }

        var slots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        foreach (var slot in slots)
        {
            foreach (var matchingCustomerEntity in customerEntities)
            {
                slot.Customers.Add(matchingCustomerEntity);
            }
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(slots);

        _ = repositoryFactory.BookingRepository.Update(booking);

        bookingOutboxPublisher.PublishBookings([mapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;

    private static int GetBookingPaymentExpiryInMinutes(ICollection<ProductVersion> productVersions, PaymentMethod paymentMethod) =>
        productVersions.Count == 0
            ? paymentMethod switch
            {
                PaymentMethod.Card => Api.Shared.Services.Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard,
                PaymentMethod.BankTransfer => Api.Shared.Services.Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            }
            : paymentMethod switch
            {
                PaymentMethod.Card => productVersions.Select(item => item.MaxAllowedResourcesLockTimePaidViaCard).Min(),
                PaymentMethod.BankTransfer => productVersions.Select(item => item.MaxAllowedResourcesLockTimePaidViaBankTransfer).Min(),
                _ => throw new ArgumentOutOfRangeException()
            };
}
