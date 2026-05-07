using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services;
using XeroRepeatingInvoiceScheduleSourceConstants = Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants;

namespace Booking.Shared.UnitTests.Services.RecurringInvoiceBillingScheduleServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetScheduleShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Purchase_Cadence_When_It_Is_Shorter_Than_Organization_Billing_Cycle(
        RecurringInvoiceBillingScheduleService sut,
        string pricingId)
    {
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            TotalAmount = 58m, ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Daily }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence);
        result.Cadence.ShouldBe(ProductPricingCadence.Daily);
        result.InvoiceAmount.ShouldBe(58m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Purchase_Cadence_When_It_Matches_Organization_Billing_Cycle(
        RecurringInvoiceBillingScheduleService sut,
        string pricingId)
    {
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 4, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            TotalAmount = 100m, ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Monthly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence);
        result.Cadence.ShouldBe(ProductPricingCadence.Monthly);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Split_To_Organization_Billing_Cycle_When_Purchase_Cadence_Is_Longer(
        RecurringInvoiceBillingScheduleService sut,
        string pricingId)
    {
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 9, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            TotalAmount = 600m, ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.SixMonths }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Cadence.ShouldBe(ProductPricingCadence.Monthly);
        result.InvoiceAmount.ShouldBe(600m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Split_To_Organization_Billing_Cycle_When_Quarterly_Purchase_Cadence_Uses_Weekly_Billing(
        RecurringInvoiceBillingScheduleService sut,
        string pricingId)
    {
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            TotalAmount = 300m, ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Quarterly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Weekly);

        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Cadence.ShouldBe(ProductPricingCadence.Weekly);
        result.InvoiceAmount.ShouldBe(300m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Split_Full_Cadence_Product_Price_Into_Installments_When_Persisted_Totals_Are_Not_Populated(
        RecurringInvoiceBillingScheduleService sut,
        string pricingId)
    {
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 9, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.SixMonths, Price = 600m }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Cadence.ShouldBe(ProductPricingCadence.Monthly);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Fall_Back_To_Product_Price_When_Marketplace_Booking_Total_Amounts_Are_Not_Populated(
        RecurringInvoiceBillingScheduleService sut,
        string pricingId)
    {
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            Quantity = 3, ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Daily, Price = 20m }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence);
        result.Cadence.ShouldBe(ProductPricingCadence.Daily);
        result.InvoiceAmount.ShouldBe(60m);
    }
}
