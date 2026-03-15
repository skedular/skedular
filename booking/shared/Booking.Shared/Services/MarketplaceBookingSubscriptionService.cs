using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
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
    IProductVersionHelperService productVersionHelperService) : IMarketplaceBookingSubscriptionService
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

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(subscription.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

        subscription.ProductPricing =
            productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions, subscription.ProductPricing) ??
            throw new ProductPricingNotFound();

        if (!IsRecurringPurchaseCadence(subscription.ProductPricing.PurchaseCadence))
        {
            throw new MarketplaceBookingSubscriptionCadenceMustBeRecurring();
        }

        if (subscription.AutoRenew && !subscription.ProductPricing.SupportsSubscriptionAutoRenewal)
        {
            throw new MarketplaceBookingSubscriptionAutoRenewalNotSupported();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var subscriptionEntity = mapper.MapTo(
            subscription,
            customerEntities,
            organizations,
            teams,
            customer,
            null,
            null,
            productVersion);

        subscriptionEntity = repositoryFactory.MarketplaceBookingSubscriptionRepository.Add(subscriptionEntity);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(subscriptionEntity);
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

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(existingSubscription);
    }

    private static bool IsRecurringPurchaseCadence(ProductPricingCadence cadence) =>
        cadence is ProductPricingCadence.Weekly or
            ProductPricingCadence.Fortnightly or
            ProductPricingCadence.Monthly or
            ProductPricingCadence.TwoMonths or
            ProductPricingCadence.Quarterly or
            ProductPricingCadence.FourMonths or
            ProductPricingCadence.FiveMonths or
            ProductPricingCadence.SixMonths or
            ProductPricingCadence.Yearly;
}
