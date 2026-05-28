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

        foreach (var (subscription, marketplaceBooking) in new[]
                 {
                     scenario.ActivePending, scenario.ActiveConfirmed, scenario.CancelledPending, scenario.CancelledConfirmed
                 })
        {
            subscription.ProductVersion = productVersion;
            subscription.InvolvedOrganizations = [organization];
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Add(subscription);

            marketplaceBooking.ProductVersion = productVersion;
            marketplaceBooking.MarketplaceBookingSubscription = subscription;
            marketplaceBooking.MarketplaceBookingSubscriptionId = subscription.Id;
            repositoryFactory.MarketplaceBookingRepository.Add(marketplaceBooking);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
