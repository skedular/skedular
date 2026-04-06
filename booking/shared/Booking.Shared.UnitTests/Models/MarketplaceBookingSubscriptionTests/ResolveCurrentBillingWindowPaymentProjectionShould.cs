using Api.Shared.Services.Models;
using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.ModelTests.MarketplaceBookingSubscriptionTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveCurrentBillingWindowPaymentProjectionShould
{
    [Fact]
    public void Aggregate_Current_Billing_Window_Bookings_When_Cadence_Is_Shorter_Than_Billing_Cycle()
    {
        var subscription = CreateSubscription(
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            OrganizationBillingCycle.Monthly,
            [
                CreateRecurringBooking(
                    "recurring-booking-previous",
                    new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Daily,
                    PaymentStatus.Confirmed,
                    "INV-PREVIOUS"),
                CreateRecurringBooking(
                    "recurring-booking-1",
                    new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Daily,
                    PaymentStatus.Confirmed,
                    "INV-APR-1"),
                CreateRecurringBooking(
                    "recurring-booking-2",
                    new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Daily,
                    PaymentStatus.Pending,
                    "INV-APR-2")
            ]);

        var result = subscription.ResolveCurrentBillingWindowPaymentProjection(new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero));

        result.ShouldNotBeNull();
        result.PaymentStatus.ShouldBe(PaymentStatus.Pending);
        result.RepresentativeMarketplaceBooking.InvoiceNumber.ShouldBe("INV-APR-2");
    }

    [Fact]
    public void Ignore_Bookings_Outside_The_Current_Billing_Window()
    {
        var subscription = CreateSubscription(
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            OrganizationBillingCycle.Monthly,
            [
                CreateRecurringBooking(
                    "recurring-booking-april",
                    new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Daily,
                    PaymentStatus.Pending,
                    "INV-APR"),
                CreateRecurringBooking(
                    "recurring-booking-may-1",
                    new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Daily,
                    PaymentStatus.Confirmed,
                    "INV-MAY-1"),
                CreateRecurringBooking(
                    "recurring-booking-may-2",
                    new DateTimeOffset(2026, 5, 2, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 5, 2, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Daily,
                    PaymentStatus.Confirmed,
                    "INV-MAY-2")
            ]);

        var result = subscription.ResolveCurrentBillingWindowPaymentProjection(new DateTimeOffset(2026, 5, 20, 10, 0, 0, TimeSpan.Zero));

        result.ShouldNotBeNull();
        result.PaymentStatus.ShouldBe(PaymentStatus.Confirmed);
        result.RepresentativeMarketplaceBooking.InvoiceNumber.ShouldBe("INV-MAY-2");
    }

    [Fact]
    public void Include_Long_Cadence_Bookings_That_Intersect_The_Current_Billing_Window()
    {
        var subscription = CreateSubscription(
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            OrganizationBillingCycle.Monthly,
            [
                CreateRecurringBooking(
                    "recurring-booking-quarterly",
                    new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero),
                    ProductPricingCadence.Quarterly,
                    PaymentStatus.Confirmed,
                    "INV-Q2")
            ]);

        var result = subscription.ResolveCurrentBillingWindowPaymentProjection(new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero));

        result.ShouldNotBeNull();
        result.PaymentStatus.ShouldBe(PaymentStatus.Confirmed);
        result.RepresentativeMarketplaceBooking.InvoiceNumber.ShouldBe("INV-Q2");
    }

    private static MarketplaceBookingSubscription CreateSubscription(
        DateTimeOffset startedAt,
        OrganizationBillingCycle organizationBillingCycle,
        ICollection<RecurringBooking> recurringBookings) =>
        new()
        {
            Id = "subscription-1",
            StartedAt = startedAt,
            Status = MarketplaceBookingSubscriptionStatus.Active,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "subscription-marketplace-booking",
                PaymentStatus = PaymentStatus.Pending,
                ProductPricing = ProductPricing.Empty("pricing-1") with { PurchaseCadence = ProductPricingCadence.Monthly },
                ProductVersion = new ProductVersion
                {
                    Id = "pv-subscription",
                    Product = new Product { Organization = new Organization { BillingCycle = organizationBillingCycle } }
                }
            },
            RecurringBookings = recurringBookings
        };

    private static RecurringBooking CreateRecurringBooking(
        string id,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        ProductPricingCadence purchaseCadence,
        PaymentStatus paymentStatus,
        string invoiceNumber) =>
        new()
        {
            Id = id,
            StartDate = startDate,
            EndDate = endDate,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = $"marketplace-booking-{id}",
                IsPaymentRequired = true,
                PaymentStatus = paymentStatus,
                InvoiceNumber = invoiceNumber,
                ProductPricing = ProductPricing.Empty($"pricing-{id}") with { PurchaseCadence = purchaseCadence },
                ProductVersion = new ProductVersion
                {
                    Id = $"pv-{id}",
                    Product = new Product { Organization = new Organization { BillingCycle = OrganizationBillingCycle.Monthly } }
                }
            }
        };
}
