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

/// <summary>
///     Service for managing marketplace bookings.
///     Provides functionality to add, update, delete, and adjust resources for marketplace bookings.
/// </summary>
public interface IMarketplaceBookingService
{
    /// <summary>
    ///     Adds a new marketplace booking.
    /// </summary>
    /// <param name="booking">The booking model to add.</param>
    /// <param name="customer">The customer creating the booking.</param>
    /// <param name="organizations">The organizations involved in the booking.</param>
    /// <param name="teams">The teams involved in the booking.</param>
    /// <param name="recurringBooking">The recurring booking if applicable.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added booking model.</returns>
    Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Updates an existing marketplace booking.
    /// </summary>
    /// <param name="booking">The updated booking model.</param>
    /// <param name="existingBooking">The existing booking entity.</param>
    /// <param name="lastModifiedByCustomer">The customer making the modification.</param>
    /// <param name="organizations">The organizations involved in the booking.</param>
    /// <param name="teams">The teams involved in the booking.</param>
    /// <param name="recurringBooking">The recurring booking if applicable.</param>
    /// <param name="bookResourceIfNoResourceProvidedOrAvailable">Whether to book a resource if none is provided or available.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated booking model.</returns>
    Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        bool bookResourceIfNoResourceProvidedOrAvailable,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a marketplace booking.
    /// </summary>
    /// <param name="existingBooking">The existing booking entity to delete.</param>
    /// <param name="deletedByCustomer">The customer performing the deletion.</param>
    /// <param name="ignoreCancellationPolicy">Whether operator permissions should bypass the customer cancellation window.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deleted booking model.</returns>
    Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        bool ignoreCancellationPolicy,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Adjusts the required resources for a booking.
    /// </summary>
    /// <param name="booking">The booking entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AdjustRequiredResourcesAsync(Database.Entities.Booking booking, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the marketplace booking service.
/// </summary>
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
    IMarketplaceEventResourceService marketplaceEventResourceService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    TimeProvider timeProvider,
    IRandomHelper randomHelper,
    IProductVersionHelperService productVersionHelperService,
    IAccountingInvoiceCancellationService accountingInvoiceCancellationService) : IMarketplaceBookingService
{
    /// <summary>
    ///     Adds a new marketplace booking.
    ///     Validates the booking window, customer entities, product version, pricing, and resources.
    ///     Creates the booking entity, starts payment workflows if needed, and publishes events.
    /// </summary>
    /// <param name="booking">The booking model to add.</param>
    /// <param name="customer">The customer creating the booking.</param>
    /// <param name="organizations">The organizations involved in the booking.</param>
    /// <param name="teams">The teams involved in the booking.</param>
    /// <param name="recurringBooking">The recurring booking if applicable.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added booking model.</returns>
    /// <exception cref="CustomerNotFound">Thrown when customer entities cannot be found.</exception>
    /// <exception cref="ProductVersionNotFound">Thrown when the product version is not found.</exception>
    /// <exception cref="ProductMissingProductTag">Thrown when the product version is missing a product tag.</exception>
    /// <exception cref="ProductPricingNotFound">Thrown when matching pricing cannot be found.</exception>
    /// <exception cref="MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking">Thrown when too many resources are selected.</exception>
    /// <exception cref="BookingPaymentMethodNotAccepted">Thrown when the payment method is not accepted.</exception>
    public async Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken)
    {
        ValidateBookingWindowWithinSingleDay(booking);

        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

        marketplaceBooking.ProductPricing =
            (productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions!, marketplaceBooking.ProductPricing) ??
             throw new ProductPricingNotFound()) with
            {
                BookingCadence = marketplaceBooking.ProductPricing.BookingCadence
            };
        // Stripe checkout is created asynchronously in Temporal, so we persist the exact
        // storefront page that should receive the user again after success or cancellation.
        marketplaceBooking.CheckoutReturnUrl = NormalizeCheckoutReturnUrl(marketplaceBooking.CheckoutReturnUrl);
        // Marketplace bookings should remain manageable by the coworking-space owner as well
        // as by any buyer-side organizations supplied by the caller. The product's owning
        // organization is therefore always merged into the involved organizations set here.
        organizations = MergeOrganizationsWithProductOwner(organizations, productVersion);

        marketplaceBooking.BillingMode = marketplaceBooking.ProductPricing.BillingMode;

        ValidateMarketplaceCadenceForBookingFlow(marketplaceBooking.ProductPricing.BookingCadence, recurringBooking);

        var isEventProduct = productVersion.Type == ProductTypeConstants.Event;
        NormalizeEventQuantity(isEventProduct, marketplaceBooking);
        var requestedResourceCount = ResolveRequestedResourceCount(isEventProduct, marketplaceBooking);
        ICollection<Resource> resources = [];
        if (!isEventProduct)
        {
            resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
                booking.From,
                booking.Until,
                booking.Resources.Select(item => item.Resource.Id).ToList(),
                productVersion.OrganizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Product).Select(item => item.Id).ToList(),
                cancellationToken);
            if (resources.Count > requestedResourceCount)
            {
                throw new MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking();
            }
        }

        marketplaceBooking.Id = randomHelper.Generate();
        marketplaceBooking.IsPaymentRequired = recurringBooking is null;
        marketplaceBooking.PaymentStatus = recurringBooking is null ? PaymentStatus.Pending : PaymentStatus.NotSet;

        if (!marketplaceBooking.ProductPricing.AcceptedPaymentMethods.Contains(marketplaceBooking.PaymentMethod))
        {
            throw new BookingPaymentMethodNotAccepted();
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (resources.Count == 0)
        {
            resources = isEventProduct
                ? await marketplaceEventResourceService.PickEventResourcesAsync(
                    booking.From,
                    booking.Until,
                    productVersion,
                    cancellationToken)
                : await marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                    booking.InvolvedCustomers.Count == 1 ? customerEntities.First() : null,
                    booking.From,
                    booking.Until,
                    productVersion,
                    requestedResourceCount,
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

        var marketplaceBookingEntity = mapper.MapTo(marketplaceBooking, customer, null, productVersion, null);
        var paymentExpiry = timeProvider
            .GetUtcNow()
            .TrimAllAfterSeconds()
            .AddMinutes(GetBookingPaymentExpiryInMinutes(marketplaceBooking.ProductPricing, marketplaceBooking.PaymentMethod));

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

        if (recurringBooking is null)
        {
            if (marketplaceBooking.BillingMode == ProductPricingBillingMode.InArrears)
            {
                temporalOutboxService.StartWorkflowGenerateInitialArrearsBookingInvoice(
                    new GenerateInitialArrearsBookingInvoiceInput(
                        booking.Id,
                        marketplaceBooking.InvoiceEmailList.ToSafeCollection()),
                    repositoryFactory.UnitOfWork);
            }
            else
            {
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

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        return booking;
    }

    /// <summary>
    ///     Updates an existing marketplace booking.
    ///     Validates the booking is marketplace type, removes existing resources, validates new resources,
    ///     and updates the booking entity while preserving checkout session information.
    /// </summary>
    /// <param name="booking">The updated booking model.</param>
    /// <param name="existingBooking">The existing booking entity.</param>
    /// <param name="lastModifiedByCustomer">The customer making the modification.</param>
    /// <param name="organizations">The organizations involved in the booking.</param>
    /// <param name="teams">The teams involved in the booking.</param>
    /// <param name="recurringBooking">The recurring booking if applicable.</param>
    /// <param name="bookResourceIfNoResourceProvidedOrAvailable">Whether to book a resource if none is provided or available.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated booking model.</returns>
    /// <exception cref="BookingIsNotMarketplace">Thrown when the booking is not a marketplace booking.</exception>
    /// <exception cref="CustomerNotFound">Thrown when customer entities cannot be found.</exception>
    /// <exception cref="ProductVersionNotFound">Thrown when the product version is not found.</exception>
    /// <exception cref="ProductMissingProductTag">Thrown when the product version is missing a product tag.</exception>
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
        ValidateBookingWindowWithinSingleDay(booking);

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
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability check easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var marketplaceBooking = existingBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);
        var existingCheckoutReturnUrl = marketplaceBooking.CheckoutReturnUrl;
        var existingStripeCheckoutSession = marketplaceBooking.StripeCheckoutSession;

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        // Keep the product owner organization attached even when the booking is updated later.
        // That way marketplace admins on the product-owning organization do not lose visibility
        // if the caller only sends their own organizations back on update.
        organizations = MergeOrganizationsWithProductOwner(organizations, productVersion);

        ValidateMarketplaceCadenceForBookingFlow(marketplaceBooking.ProductPricing.BookingCadence, recurringBooking);

        var resourceIds = booking.Resources.Count == 0
            ? existingBooking.InvolvedResources.Select(item => item.Id).ToList()
            : booking.Resources.Select(item => item.Resource.Id).ToList();
        var isEventProduct = productVersion.Type == ProductTypeConstants.Event;
        NormalizeEventQuantity(isEventProduct, booking.MarketplaceBooking);
        NormalizeEventQuantity(isEventProduct, marketplaceBooking);
        var requestedResourceCount = ResolveRequestedResourceCount(isEventProduct, marketplaceBooking);

        ICollection<Resource> resources;

        // For non-customized recurring instances, the scheduler can request a best-effort rebooking.
        // In that mode we first try resources provided on the booking model, and only if none are
        // currently available, we fall back to preference-based auto assignment (the same strategy as AddAsync).
        if (bookResourceIfNoResourceProvidedOrAvailable && existingBooking.HasRecurringInstanceOverrides != true)
        {
            resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                existingBooking.From,
                existingBooking.Until,
                resourceIds,
                [],
                [],
                cancellationToken);

            // If no requested resource is available, try to auto-pick one by customer preference.
            var requiredAvailableResources = isEventProduct ? existingBooking.InvolvedResources.Count : requestedResourceCount;
            if (resources.Count < requiredAvailableResources && (isEventProduct || booking.InvolvedCustomers.Count == 1))
            {
                resources = isEventProduct
                    ? await marketplaceEventResourceService.PickEventResourcesAsync(
                        existingBooking.From,
                        existingBooking.Until,
                        productVersion,
                        cancellationToken)
                    : await marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                        customerEntities.First(),
                        existingBooking.From,
                        existingBooking.Until,
                        productVersion,
                        requestedResourceCount,
                        cancellationToken);
            }
        }
        else
        {
            // Non-recurring or customized instances keep strict behavior:
            // caller-provided resources must all be available in the original persisted window.
            if (isEventProduct)
            {
                resources = HasBookingWindowChanged(booking, existingBooking)
                    ? await marketplaceEventResourceService.PickEventResourcesAsync(
                        existingBooking.From,
                        existingBooking.Until,
                        productVersion,
                        cancellationToken)
                    : existingBooking.InvolvedResources.ToList();
            }
            else
            {
                resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
                    existingBooking.From,
                    existingBooking.Until,
                    resourceIds,
                    productVersion.OrganizationTags.Where(item => item.Type == OrganizationTagTypeConstants.Product).Select(item => item.Id).ToList(),
                    cancellationToken);
            }
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

        // Marketplace booking updates must not replace the original hosted-checkout wiring.
        // Once a booking exists, the Stripe checkout session URL and the stored storefront
        // return URL stay tied to that original booking/payment flow rather than to later
        // edits to notes, people, or resources.
        ArgumentNullException.ThrowIfNull(bookingEntity.MarketplaceBooking);
        bookingEntity.MarketplaceBooking.CheckoutReturnUrl = existingCheckoutReturnUrl;
        bookingEntity.MarketplaceBooking.StripeCheckoutSession = existingStripeCheckoutSession;

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity);

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return booking;
    }

    /// <summary>
    ///     Deletes a marketplace booking.
    ///     Validates the booking is marketplace type, removes all resource slots, marks as deleted,
    ///     and signals payment workflows to cancel if payment is required.
    /// </summary>
    /// <param name="existingBooking">The existing booking entity to delete.</param>
    /// <param name="deletedByCustomer">The customer performing the deletion.</param>
    /// <param name="ignoreCancellationPolicy">Whether operator permissions should bypass the customer cancellation window.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deleted booking model.</returns>
    /// <exception cref="BookingIsNotMarketplace">Thrown when the booking is not a marketplace booking.</exception>
    public async Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        bool ignoreCancellationPolicy,
        CancellationToken cancellationToken)
    {
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace)
        {
            throw new BookingIsNotMarketplace();
        }

        if (deletedByCustomer is not null && !ignoreCancellationPolicy)
        {
            EnsureBookingCanStillBeCancelled(existingBooking);
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

        existingBooking.InvolvedResources.Clear();
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

        await accountingInvoiceCancellationService.CancelBookingAsync(existingBooking, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.RemoveByIdAsync(deletedBooking.Id, cancellationToken);

        return deletedBooking;
    }

    /// <summary>
    ///     Adjusts the required resources for a booking.
    ///     Removes existing resources, validates customer entities, and assigns new resources
    ///     based on availability or customer preferences.
    /// </summary>
    /// <param name="booking">The booking entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="CustomerNotFound">Thrown when customer entities cannot be found.</exception>
    /// <exception cref="ProductVersionNotFound">Thrown when the product version is not found.</exception>
    /// <exception cref="ProductMissingProductTag">Thrown when the product version is missing a product tag.</exception>
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
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability check easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        var resourceIds = booking.InvolvedResources.Select(item => item.Id).ToList();
        var isEventProduct = productVersion.Type == ProductTypeConstants.Event;
        NormalizeEventQuantity(isEventProduct, marketplaceBooking);
        var requestedResourceCount = ResolveRequestedResourceCount(isEventProduct, marketplaceBooking);
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
        var requiredAvailableResources = isEventProduct
            ? booking.InvolvedResources.Count
            : requestedResourceCount;
        if (resources.Count < requiredAvailableResources && (isEventProduct || booking.InvolvedCustomers.Count == 1))
        {
            resources = isEventProduct
                ? await marketplaceEventResourceService.PickEventResourcesAsync(
                    booking.From,
                    booking.Until,
                    productVersion,
                    cancellationToken)
                : await marketplaceBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                    customerEntities.First(),
                    booking.From,
                    booking.Until,
                    productVersion,
                    requestedResourceCount,
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

    /// <summary>
    ///     Determines if the product pricing cadence represents a single instance booking.
    /// </summary>
    /// <param name="cadence">The product pricing cadence to check.</param>
    /// <returns>True if the cadence is for single instance bookings, false otherwise.</returns>
    private static bool IsSingleInstanceMarketplaceCadence(ProductPricingCadence cadence) =>
        cadence is ProductPricingCadence.OneTime or
            ProductPricingCadence.PerMinute or
            ProductPricingCadence.Per15Minutes or
            ProductPricingCadence.Per30Minutes or
            ProductPricingCadence.PerHour or
            ProductPricingCadence.HalfDay or
            ProductPricingCadence.Daily;

    private static int ResolveRequestedResourceCount(ProductVersion productVersion, MarketplaceBooking marketplaceBooking) =>
        ResolveRequestedResourceCount(productVersion.Type == ProductTypeConstants.Event, marketplaceBooking);

    private static int ResolveRequestedResourceCount(bool isEventProduct, Models.MarketplaceBooking marketplaceBooking) =>
        isEventProduct ? 0 : marketplaceBooking.Quantity * marketplaceBooking.ProductPricing.NumberOfResourcesToBook;

    private static int ResolveRequestedResourceCount(bool isEventProduct, MarketplaceBooking marketplaceBooking) =>
        isEventProduct ? 0 : marketplaceBooking.Quantity * marketplaceBooking.ProductPricing.NumberOfResourcesToBook;

    private static void NormalizeEventQuantity(bool isEventProduct, Models.MarketplaceBooking? marketplaceBooking)
    {
        if (isEventProduct && marketplaceBooking is not null)
        {
            marketplaceBooking.Quantity = 1;
        }
    }

    private static void NormalizeEventQuantity(bool isEventProduct, MarketplaceBooking marketplaceBooking)
    {
        if (isEventProduct)
        {
            marketplaceBooking.Quantity = 1;
        }
    }

    /// <summary>
    ///     Validates that the marketplace cadence is compatible with the booking flow (single or recurring).
    /// </summary>
    /// <param name="cadence">The product pricing cadence.</param>
    /// <param name="recurringBooking">The recurring booking if applicable.</param>
    /// <exception cref="MarketplaceBookingCadenceRequiresRecurringFlow">Thrown when cadence validation fails.</exception>
    private static void ValidateMarketplaceCadenceForBookingFlow(ProductPricingCadence cadence, RecurringBooking? recurringBooking)
    {
        if (recurringBooking is null)
        {
            if (!IsSingleInstanceMarketplaceCadence(cadence))
            {
                throw new MarketplaceBookingCadenceRequiresRecurringFlow();
            }

            return;
        }

        if (cadence != ProductPricingCadence.Daily)
        {
            throw new MarketplaceBookingCadenceRequiresRecurringFlow();
        }
    }

    /// <summary>
    ///     Validates that the booking window starts and ends within the same day.
    /// </summary>
    /// <param name="booking">The booking model to validate.</param>
    /// <exception cref="BookingMustStartAndEndWithinSameDay">Thrown when the booking spans multiple days.</exception>
    private static void ValidateBookingWindowWithinSingleDay(Models.Booking booking)
    {
        var from = booking.From.UtcDateTime;
        var until = booking.Until.UtcDateTime;

        if (from.Date != until.Date && (from.Date.AddDays(1) != until.Date || until.TimeOfDay != TimeSpan.Zero))
        {
            throw new BookingMustStartAndEndWithinSameDay();
        }
    }

    private static bool HasBookingWindowChanged(Models.Booking booking, Database.Entities.Booking existingBooking) =>
        booking.From != existingBooking.From || booking.Until != existingBooking.Until;

    /// <summary>
    ///     Converts a collection of resources to their unique locations.
    /// </summary>
    /// <param name="resources">The resources to convert.</param>
    /// <returns>A list of unique locations from the resources.</returns>
    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;

    /// <summary>
    ///     Merges the provided organizations with the product owner's organization.
    ///     Ensures the product owner organization is always included.
    /// </summary>
    /// <param name="organizations">The organizations to merge.</param>
    /// <param name="productVersion">The product version containing the product owner.</param>
    /// <returns>A list of unique organizations including the product owner.</returns>
    private static List<Organization> MergeOrganizationsWithProductOwner(
        ICollection<Organization> organizations,
        ProductVersion productVersion)
    {
        ArgumentNullException.ThrowIfNull(productVersion.Product);
        ArgumentNullException.ThrowIfNull(productVersion.Product.Organization);

        return organizations
            .Append(productVersion.Product.Organization)
            .GroupBy(item => item.Id)
            .Select(item => item.First())
            .ToList();
    }

    /// <summary>
    ///     Gets the booking payment expiry time in minutes based on pricing and payment method.
    /// </summary>
    /// <param name="pricing">The product pricing containing expiry settings.</param>
    /// <param name="paymentMethod">The payment method used.</param>
    /// <returns>The payment expiry time in minutes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the payment method is not supported.</exception>
    private static int GetBookingPaymentExpiryInMinutes(ProductPricing pricing, PaymentMethod paymentMethod) =>
        paymentMethod switch
        {
            PaymentMethod.Card => pricing.MaxAllowedResourcesLockTimePaidViaCard,
            PaymentMethod.BankTransfer => pricing.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            _ => throw new ArgumentOutOfRangeException()
        };

    /// <summary>
    ///     Enforces the pricing-option cancellation policy for user-initiated deletes.
    ///     The policy is evaluated against the booking start time so the storefront can evolve
    ///     from simple full-refund cutoffs to tiered cancellation rules without changing the API.
    /// </summary>
    private void EnsureBookingCanStillBeCancelled(Database.Entities.Booking existingBooking)
    {
        var marketplaceBooking = existingBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (!CanBeCancelled(marketplaceBooking.ProductPricing, existingBooking.From, timeProvider.GetUtcNow()))
        {
            throw new MarketplaceBookingCancellationNotAllowed();
        }
    }

    private static bool CanBeCancelled(
        ProductPricing pricing,
        DateTimeOffset referenceTime,
        DateTimeOffset cancelledAt)
    {
        if (pricing.CancellationPolicyType == ProductPricingCancellationPolicyType.NoCancellation)
        {
            return false;
        }

        if (pricing.CancellationPolicyType == ProductPricingCancellationPolicyType.FullRefundBeforeCutoff &&
            pricing.CancellationRefundRules.Count == 0)
        {
            return cancelledAt <= referenceTime;
        }

        var applicableRule = pricing.CancellationRefundRules
            .OrderByDescending(item => item.MinutesBefore)
            .FirstOrDefault(item => cancelledAt <= referenceTime.AddMinutes(-item.MinutesBefore));

        return applicableRule is not null;
    }

    /// <summary>
    ///     Normalizes and validates the checkout return URL.
    ///     Ensures the URL is valid and uses HTTP/HTTPS scheme.
    /// </summary>
    /// <param name="checkoutReturnUrl">The checkout return URL to normalize.</param>
    /// <returns>The normalized URL string, or null if empty.</returns>
    /// <exception cref="MarketplaceBookingCheckoutReturnUrlInvalid">Thrown when the URL is invalid.</exception>
    private static string? NormalizeCheckoutReturnUrl(string? checkoutReturnUrl)
    {
        if (string.IsNullOrWhiteSpace(checkoutReturnUrl))
        {
            return null;
        }

        if (!Uri.TryCreate(checkoutReturnUrl, UriKind.Absolute, out var returnUri) ||
            (returnUri.Scheme != Uri.UriSchemeHttps && returnUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new MarketplaceBookingCheckoutReturnUrlInvalid();
        }

        return returnUri.ToString();
    }
}
