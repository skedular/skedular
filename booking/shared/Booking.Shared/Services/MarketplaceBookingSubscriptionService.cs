using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Customer = Booking.Shared.Database.Entities.Customer;
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
        CancellationToken cancellationToken);
}

public class MarketplaceBookingSubscriptionService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IProductVersionHelperService productVersionHelperService,
    ITemporalOutboxService temporalOutboxService,
    IRandomHelper randomHelper) : IMarketplaceBookingSubscriptionService
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

        var marketplaceBooking = subscription.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

        marketplaceBooking.ProductPricing =
            productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions, marketplaceBooking.ProductPricing) ??
            throw new ProductPricingNotFound();
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
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingSubscription.DeletedByCustomer = deletedByCustomer;
        existingSubscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(existingSubscription);
        existingSubscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Remove(existingSubscription);

        temporalOutboxService.SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
            existingSubscription.Id,
            repositoryFactory.UnitOfWork);

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
}
