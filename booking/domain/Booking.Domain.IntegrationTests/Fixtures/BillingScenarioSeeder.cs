using Booking.Shared.Repositories;

namespace Booking.Domain.IntegrationTests.Fixtures;

public static class BillingScenarioSeeder
{
    public static async Task SeedAsync(
        IRepositoryFactory repositoryFactory,
        UpfrontArrearsTriggerScenario scenario,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(scenario.Organization.Id, cancellationToken);
        organization.Name = scenario.Organization.Name;
        organization.CustomDomain = scenario.Organization.CustomDomain;
        organization.Type = scenario.Organization.Type;
        organization.BillingCycle = scenario.Organization.BillingCycle;
        organization.ContactEmail = scenario.Organization.ContactEmail;

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(scenario.Customer.Id, false, cancellationToken);
        customer.Type = scenario.Customer.Type;
        customer.Name = scenario.Customer.Name;
        customer.GivenName = scenario.Customer.GivenName;
        customer.FamilyName = scenario.Customer.FamilyName;
        customer.Locale = scenario.Customer.Locale;

        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync(
            scenario.Product.Id,
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

        var booking = scenario.Booking;
        booking.InvolvedCustomers = [customer];
        booking.InvolvedOrganizations = [organization];
        booking.CreatedByCustomer = customer;

        var marketplaceBooking = scenario.MarketplaceBooking;
        marketplaceBooking.Booking = booking;
        marketplaceBooking.BookingId = booking.Id;
        marketplaceBooking.ProductVersion = productVersion;
        marketplaceBooking.PaidByCustomer = customer;

        booking.MarketplaceBooking = marketplaceBooking;

        repositoryFactory.BookingRepository.Add(booking);
        repositoryFactory.MarketplaceBookingRepository.Add(marketplaceBooking);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
