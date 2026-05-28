using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.BookingInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveRecurringInvoiceDisplayPeriodShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Full_Term_When_Billing_Definition_Comes_From_Purchase_Cadence(string pricingId)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            CreatedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Quarterly }
            }
        };
        var billingDefinition = new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            ProductPricingCadence.Quarterly,
            300m);

        var result = BookingInvoiceService.ResolveRecurringInvoiceDisplayPeriod(recurringBooking, billingDefinition);

        result.StartInclusive.ShouldBe(recurringBooking.StartDate);
        result.EndInclusive.ShouldBe(recurringBooking.EndDate.Value);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Billing_Cycle_Period_When_Billing_Definition_Comes_From_Organization_Billing_Cycle(string pricingId)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            CreatedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 9, 30, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.SixMonths }
            }
        };
        var billingDefinition = new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            ProductPricingCadence.Monthly,
            100m);

        var result = BookingInvoiceService.ResolveRecurringInvoiceDisplayPeriod(recurringBooking, billingDefinition);

        result.StartInclusive.ShouldBe(new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        result.EndInclusive.ShouldBe(new DateTimeOffset(2026, 4, 30, 0, 0, 0, TimeSpan.Zero));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Clamp_Billing_Cycle_Period_To_Full_Term_End(string pricingId)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            CreatedAt = new DateTimeOffset(2026, 4, 15, 0, 0, 0, TimeSpan.Zero),
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 4, 20, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Monthly }
            }
        };
        var billingDefinition = new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            ProductPricingCadence.Weekly,
            25m);

        var result = BookingInvoiceService.ResolveRecurringInvoiceDisplayPeriod(recurringBooking, billingDefinition);

        result.StartInclusive.ShouldBe(new DateTimeOffset(2026, 4, 15, 0, 0, 0, TimeSpan.Zero));
        result.EndInclusive.ShouldBe(new DateTimeOffset(2026, 4, 20, 0, 0, 0, TimeSpan.Zero));
    }
}
