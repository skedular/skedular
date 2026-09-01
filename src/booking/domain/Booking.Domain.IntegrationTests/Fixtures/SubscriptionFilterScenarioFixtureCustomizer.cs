using Api.Shared.Services.Models;
using AutoFixture;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Domain.IntegrationTests.Fixtures;

public class SubscriptionFilterScenarioFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register(CreateScenario);

    private static SubscriptionFilterScenario CreateScenario()
    {
        var organization = new OrganizationEntity
        {
            Id = NewId(),
            Name = "Filter Test Org",
            CustomDomain = $"filter-{Guid.CreateVersion7():N}",
            Type = OrganizationType.Private.ToOrganizationType(),
            BillingCycle = OrganizationBillingCycle.Monthly.ToOrganizationBillingCycle(),
            ContactEmail = "test@filter.test",
        };

        var product = new ProductEntity
        {
            Id = NewId(),
            Organization = organization,
            OrganizationId = organization.Id,
        };

        var pricing = new ProductPricing(
            NewId(),
            0,
            new ListingMetadata("Filter Product", "For filtering tests", null, []),
            ProductPricingCadence.Monthly,
            10m,
            false,
            false,
            [PaymentMethod.BankTransfer],
            ProductPricingBillingMode.InArrears,
            null,
            null,
            30,
            30,
            1,
            ProductPricingCancellationPolicyType.NotSet,
            []);

        var productVersion = new ProductVersionEntity
        {
            Id = NewId(),
            Product = product,
            ProductId = product.Id,
            Type = ProductType.Resource.ToProductType(),
            Currency = Currency.Nzd.ToCurrency(),
            ListingMetadata = new ListingMetadata("Filter Product", "For filtering tests", null, []),
            PricingOptions = [pricing],
        };

        var activePending = CreateSubscriptionPair(
            organization,
            productVersion,
            pricing,
            MarketplaceBookingSubscriptionStatusConstants.Active,
            PaymentStatusConstants.Pending);

        var activeConfirmed = CreateSubscriptionPair(
            organization,
            productVersion,
            pricing,
            MarketplaceBookingSubscriptionStatusConstants.Active,
            PaymentStatusConstants.Confirmed);

        var cancelledPending = CreateSubscriptionPair(
            organization,
            productVersion,
            pricing,
            MarketplaceBookingSubscriptionStatusConstants.Cancelled,
            PaymentStatusConstants.Pending);

        var cancelledConfirmed = CreateSubscriptionPair(
            organization,
            productVersion,
            pricing,
            MarketplaceBookingSubscriptionStatusConstants.Cancelled,
            PaymentStatusConstants.Confirmed);

        return new SubscriptionFilterScenario(
            organization,
            productVersion,
            activePending,
            activeConfirmed,
            cancelledPending,
            cancelledConfirmed);
    }

    private static (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) CreateSubscriptionPair(
        OrganizationEntity organization,
        ProductVersionEntity productVersion,
        ProductPricing pricing,
        string status,
        string paymentStatus)
    {
        var subscriptionId = NewId();

        var subscription = new MarketplaceBookingSubscriptionEntity
        {
            Id = subscriptionId,
            StartedAt = TimeProvider.System.GetUtcNow(),
            Status = status,
            AutoRenew = false,
            CancelAtPeriodEnd = false,
            ProductVersion = productVersion,
            InvolvedOrganizations = [organization],
        };

        var marketplaceBooking = new MarketplaceBookingEntity
        {
            Id = NewId(),
            PaymentStatus = paymentStatus,
            IsPaymentRequired = true,
            Quantity = 1,
            ProductPricing = pricing,
            ProductVersion = productVersion,
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
            PaymentExpiry = TimeProvider.System.GetUtcNow().AddDays(30),
            TotalAmountExcludeTax = 10m,
            TotalAmount = 10m,
            Currency = Currency.Nzd.ToCurrency(),
            InvoiceEmailList = [],
            BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
            MarketplaceBookingSubscription = subscription,
            MarketplaceBookingSubscriptionId = subscriptionId,
        };

        return (subscription, marketplaceBooking);
    }

    private static string NewId() => Guid.CreateVersion7().ToString();
}
