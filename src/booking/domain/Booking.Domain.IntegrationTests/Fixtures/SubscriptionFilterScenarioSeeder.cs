using Booking.Shared.Repositories;

namespace Booking.Domain.IntegrationTests.Fixtures;

public static class SubscriptionFilterScenarioSeeder
{
    public static async Task SeedAsync(
        IRepositoryFactory repositoryFactory,
        SubscriptionFilterScenario scenario,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            scenario.Organization.Id,
            cancellationToken);
        organization.Name = scenario.Organization.Name;
        organization.CustomDomain = scenario.Organization.CustomDomain;
        organization.Type = scenario.Organization.Type;
        organization.BillingCycle = scenario.Organization.BillingCycle;
        organization.ContactEmail = scenario.Organization.ContactEmail;

        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync(
            scenario.ProductVersion.Product.Id,
            organization,
            cancellationToken);
        product.Organization = organization;
        product.OrganizationId = organization.Id;

        var productVersion = await repositoryFactory.ProductVersionRepository.UpsertNakedAsync(
            scenario.ProductVersion.Id,
            product,
            cancellationToken);
        productVersion.Product = product;
        productVersion.ProductId = product.Id;
        productVersion.Type = scenario.ProductVersion.Type;
        productVersion.Currency = scenario.ProductVersion.Currency;
        productVersion.ListingMetadata = scenario.ProductVersion.ListingMetadata;
        productVersion.PricingOptions = scenario.ProductVersion.PricingOptions;

        var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(
            scenario.Location.Id,
            organization,
            cancellationToken);
        location.Name = scenario.Location.Name;
        location.Timezone = scenario.Location.Timezone;
        location.Type = scenario.Location.Type;

        var resource = await repositoryFactory.ResourceRepository.UpsertNakedAsync(
            scenario.Resource.Id,
            location,
            cancellationToken);
        resource.Name = scenario.Resource.Name;
        resource.Capacity = scenario.Resource.Capacity;

        foreach (var (subscription, marketplaceBooking) in new[]
                 {
                     scenario.ActivePending, scenario.ActiveConfirmed, scenario.CancelledPending, scenario.CancelledConfirmed,
                 })
        {
            subscription.ProductVersion = productVersion;
            subscription.InvolvedOrganizations = [organization];
            subscription.RequestedResources = subscription.RequestedResources
                .Select(item => item.Id == resource.Id ? resource : item)
                .ToList();
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Add(subscription);

            marketplaceBooking.ProductVersion = productVersion;
            marketplaceBooking.MarketplaceBookingSubscription = subscription;
            marketplaceBooking.MarketplaceBookingSubscriptionId = subscription.Id;
            repositoryFactory.MarketplaceBookingRepository.Add(marketplaceBooking);
            subscription.MarketplaceBooking = marketplaceBooking;
            await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
                subscription, null, cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
