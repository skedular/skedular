using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Models.MarketplaceBookingSubscription;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingSubscriptionService
{
    Task<MarketplaceBookingSubscription> AddAsync(
        MarketplaceBookingSubscription subscription,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<MarketplaceBookingSubscription> DeleteAsync(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        Customer? deletedByCustomer,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingSubscriptionService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IProductVersionHelperService productVersionHelperService,
    IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
    ITemporalOutboxService temporalOutboxService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : IMarketplaceBookingSubscriptionService
{
    public async Task<MarketplaceBookingSubscription> AddAsync(
        MarketplaceBookingSubscription subscription,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken)
    {
        var customerIds = subscription.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var requestedResourceIds = subscription.RequestedResources.Select(item => item.Id).Distinct().ToList();
        var resourceEntities = requestedResourceIds.Count == 0
            ? []
            : await repositoryFactory.ResourceRepository.GetByIdsAsync(requestedResourceIds, false, cancellationToken);
        if (resourceEntities.Count != requestedResourceIds.Count)
        {
            throw new ResourceNotFound();
        }

        var marketplaceBooking = subscription.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        if (productVersion.Type == ProductTypeConstants.Event)
        {
            throw new MarketplaceEventProductRecurringBookingNotSupported();
        }

        ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

        marketplaceBooking.ProductPricing =
            productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions, marketplaceBooking.ProductPricing) ??
            throw new ProductPricingNotFound();
        await EnsureRequestedResourceCanBeBookedAsync(subscription, productVersion, marketplaceBooking, marketplaceBookingOpeningHoursService,
            cancellationToken);
        // Subscription checkout also happens asynchronously later in Temporal, so the initial
        // marketplace-booking template must carry the storefront URL that Stripe should return
        // the customer to after hosted checkout finishes or is cancelled.
        marketplaceBooking.CheckoutReturnUrl = NormalizeCheckoutReturnUrl(marketplaceBooking.CheckoutReturnUrl);
        // Subscriptions should be manageable by the coworking-space owner organization too,
        // so the product owner's organization is always merged into involved organizations.
        organizations = MergeOrganizationsWithProductOwner(organizations, productVersion);

        if (subscription.AutoRenew && !marketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal)
        {
            throw new MarketplaceBookingSubscriptionAutoRenewalNotSupported();
        }

        marketplaceBooking.Id = randomHelper.Generate();
        marketplaceBooking.BillingMode = marketplaceBooking.ProductPricing.BillingMode;
        marketplaceBooking.IsPaymentRequired = true;
        marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;

        if (!marketplaceBooking.ProductPricing.AcceptedPaymentMethods.Contains(marketplaceBooking.PaymentMethod))
        {
            throw new BookingPaymentMethodNotAccepted();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var marketplaceBookingEntity = repositoryFactory.MarketplaceBookingRepository.Add(mapper.MapTo(
            marketplaceBooking,
            customer,
            null,
            productVersion,
            null));

        var subscriptionEntity = repositoryFactory.MarketplaceBookingSubscriptionRepository.Add(mapper.MapTo(
            subscription,
            customerEntities,
            organizations,
            teams,
            resourceEntities,
            customer,
            null,
            null,
            marketplaceBookingEntity,
            productVersion));

        subscription = mapper.MapTo(subscriptionEntity);

        temporalOutboxService.StartBookMarketplaceBookingSubscriptionResources(
            new BookMarketplaceBookingSubscriptionResourcesInput(subscription.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return subscription;
    }

    public async Task<MarketplaceBookingSubscription> DeleteAsync(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        Customer? deletedByCustomer,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        CancellationToken cancellationToken)
    {
        if (deletedByCustomer is not null)
        {
            EnsureSubscriptionCanStillBeCancelled(existingSubscription, cancellationMode);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (cancellationMode == MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd)
        {
            existingSubscription.LastModifiedByCustomer = deletedByCustomer;
            existingSubscription.CancelledAt = timeProvider.GetUtcNow();
            existingSubscription.CancelAtPeriodEnd = true;
            existingSubscription.AutoRenew = false;
            existingSubscription.NextRenewalAt ??= ResolveNextRenewalAt(
                existingSubscription.StartedAt,
                existingSubscription.MarketplaceBooking.ProductPricing.PurchaseCadence);
            existingSubscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(existingSubscription);
        }
        else
        {
            existingSubscription.LastModifiedByCustomer = deletedByCustomer;
            existingSubscription.CancelledAt = timeProvider.GetUtcNow();
            existingSubscription.Status = MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus();
            existingSubscription.AutoRenew = false;
            existingSubscription.CancelAtPeriodEnd = false;
            existingSubscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(existingSubscription);

            temporalOutboxService.SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
                existingSubscription.Id,
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(existingSubscription);
    }

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

    private static async Task EnsureRequestedResourceCanBeBookedAsync(
        MarketplaceBookingSubscription subscription,
        ProductVersion productVersion,
        MarketplaceBooking marketplaceBooking,
        IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
        CancellationToken cancellationToken)
    {
        var requestedResourceIds = subscription.RequestedResources.Select(item => item.Id).Distinct().ToList();
        if (requestedResourceIds.Count == 0)
        {
            return;
        }

        var bookingDay = DateOnly.FromDateTime(subscription.StartedAt.UtcDateTime.Date);
        var requiredResourceCount = marketplaceBooking.Quantity * marketplaceBooking.ProductPricing.NumberOfResourcesToBook;
        var dailyPlan = await marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
            null,
            productVersion,
            marketplaceBooking.ProductPricing,
            bookingDay,
            requiredResourceCount,
            requestedResourceIds,
            [],
            null,
            cancellationToken);
        if (dailyPlan is null || dailyPlan.Resources.Count != requiredResourceCount)
        {
            throw new ResourceNotAvailable();
        }
    }

    private void EnsureSubscriptionCanStillBeCancelled(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode)
    {
        var marketplaceBooking = existingSubscription.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (cancellationMode == MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd)
        {
            if (!existingSubscription.AutoRenew)
            {
                throw new MarketplaceBookingSubscriptionCancellationNotAllowed();
            }

            return;
        }

        var referenceTime = existingSubscription.NextRenewalAt ?? existingSubscription.StartedAt;
        if (!CanBeCancelled(marketplaceBooking.ProductPricing, referenceTime, timeProvider.GetUtcNow()))
        {
            throw new MarketplaceBookingSubscriptionCancellationNotAllowed();
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

    private static DateTimeOffset ResolveNextRenewalAt(DateTimeOffset startedAt, ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.Daily => startedAt.AddDays(1),
            ProductPricingCadence.Weekly => startedAt.AddDays(7),
            ProductPricingCadence.Fortnightly => startedAt.AddDays(14),
            ProductPricingCadence.Monthly => startedAt.AddMonths(1),
            ProductPricingCadence.TwoMonths => startedAt.AddMonths(2),
            ProductPricingCadence.Quarterly => startedAt.AddMonths(3),
            ProductPricingCadence.FourMonths => startedAt.AddMonths(4),
            ProductPricingCadence.FiveMonths => startedAt.AddMonths(5),
            ProductPricingCadence.SixMonths => startedAt.AddMonths(6),
            ProductPricingCadence.Yearly => startedAt.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(cadence), cadence, null)
        };
}
