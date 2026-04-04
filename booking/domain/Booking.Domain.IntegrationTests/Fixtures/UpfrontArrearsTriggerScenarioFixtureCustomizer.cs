using Api.Shared.Services.Models;
using AutoFixture;
using Booking.Shared.Database.Entities;

namespace Booking.Domain.IntegrationTests.Fixtures;

public class UpfrontArrearsTriggerScenarioFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture) => fixture.Register(CreateScenario);

    private static UpfrontArrearsTriggerScenario CreateScenario()
    {
        var organization = new Organization
        {
            Id = NewId(),
            Name = "Acme Cowork",
            CustomDomain = $"acme-{Guid.CreateVersion7():N}"[..13],
            Type = OrganizationType.Private.ToOrganizationType(),
            BillingCycle = OrganizationBillingCycle.Monthly.ToOrganizationBillingCycle(),
            ContactEmail = "billing@acme.test"
        };

        var customer = new Customer
        {
            Id = NewId(),
            Type = CustomerType.Registered.ToCustomerType(),
            Name = "Taylor Example",
            GivenName = "Taylor",
            FamilyName = "Example",
            Locale = "en-NZ"
        };

        var product = new Product { Id = NewId(), Organization = organization, OrganizationId = organization.Id };

        var productVersion = new ProductVersion
        {
            Id = NewId(),
            Product = product,
            ProductId = product.Id,
            Type = ProductType.Resource.ToProductType(),
            Currency = Currency.Nzd.ToCurrency(),
            ListingMetadata = new ListingMetadata("Desk access", "Dedicated desk", null, []),
            PricingOptions =
            [
                new ProductPricing(
                    NewId(),
                    0,
                    new ListingMetadata("Desk access", "Dedicated desk", null, []),
                    ProductPricingCadence.OneTime,
                    ProductPricingCadence.OneTime,
                    125m,
                    false,
                    false,
                    [PaymentMethod.BankTransfer],
                    ProductPricingBillingMode.Upfront,
                    null,
                    null,
                    30,
                    30,
                    1,
                    ProductPricingCancellationPolicyType.NotSet,
                    [])
            ]
        };

        var booking = new Shared.Database.Entities.Booking
        {
            Id = NewId(),
            From = new DateTimeOffset(2026, 3, 5, 9, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 3, 5, 17, 0, 0, TimeSpan.Zero),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [organization],
            CreatedByCustomer = customer
        };

        var marketplaceBooking = new MarketplaceBooking
        {
            Id = NewId(),
            Booking = booking,
            BookingId = booking.Id,
            ProductVersion = productVersion,
            PaidByCustomer = customer,
            PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus(),
            IsPaymentRequired = true,
            Quantity = 1,
            ProductPricing = productVersion.PricingOptions!.Single(),
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
            PaymentExpiry = new DateTimeOffset(2026, 3, 6, 0, 0, 0, TimeSpan.Zero),
            TotalAmountExcludeTax = 125m,
            TotalAmount = 125m,
            Currency = Currency.Nzd.ToCurrency(),
            InvoiceEmailList = [],
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode()
        };

        booking.MarketplaceBooking = marketplaceBooking;

        return new UpfrontArrearsTriggerScenario(
            organization,
            customer,
            product,
            productVersion,
            booking,
            marketplaceBooking);
    }

    private static string NewId() => Guid.CreateVersion7().ToString();
}
