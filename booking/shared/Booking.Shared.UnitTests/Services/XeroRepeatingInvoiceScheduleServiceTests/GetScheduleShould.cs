using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroRepeatingInvoiceScheduleSourceConstants = Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants;

namespace Booking.Shared.UnitTests.Services.XeroRepeatingInvoiceScheduleServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetScheduleShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Organization_Billing_Cycle_When_Recurring_Booking_Is_In_Arrears(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 9, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
            TotalAmount = 600m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.SixMonths }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Fortnightly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(2);
        result.InvoiceAmount.ShouldBe(42.8571m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Weekly_Schedule_When_Organization_Billing_Cycle_Is_Weekly_For_In_Arrears_Recurring_Bookings(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
            TotalAmount = 1200m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Yearly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Weekly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(22.6415m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Purchase_Cadence_When_It_Is_Shorter_Than_The_Organization_Billing_Cycle_For_In_Arrears_Recurring_Bookings(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 4, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
            TotalAmount = 100m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Weekly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Split_To_Organization_Billing_Cycle_When_Recurring_Purchase_Cadence_Is_Longer(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
            TotalAmount = 1200m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Yearly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Weekly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Product_Purchase_Cadence_When_Recurring_Booking_Is_Not_In_Arrears(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 6, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 300m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Quarterly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Weekly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(23.0769m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Ignore_Organization_Billing_Cycle_When_Recurring_Booking_Is_Not_In_Arrears(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 4, 14, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 140m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Fortnightly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(2);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Multi_Month_Schedule_When_Product_Purchase_Cadence_Is_Two_Months(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 5, 31, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 200m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.TwoMonths }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.MONTHLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Multi_Month_Schedule_When_Product_Purchase_Cadence_Is_Four_Months(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 7, 31, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 400m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.FourMonths }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.MONTHLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Multi_Month_Schedule_When_Product_Purchase_Cadence_Is_Five_Months(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 8, 31, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 500m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.FiveMonths }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.MONTHLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Multi_Month_Schedule_When_Product_Purchase_Cadence_Is_Six_Months(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 9, 30, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 600m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.SixMonths }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.MONTHLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(100m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Yearly_Schedule_As_Twelve_Month_Periods_When_Product_Purchase_Cadence_Is_Yearly(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 1200m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Yearly }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Weekly);

        result.ShouldNotBeNull();
        result.Source.ShouldBe(XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle);
        result.Unit.ShouldBe(Schedule.UnitEnum.WEEKLY);
        result.Period.ShouldBe(1);
        result.InvoiceAmount.ShouldBe(22.6415m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_When_Organization_Billing_Cycle_Is_Unsupported_And_The_Purchase_Cadence_Needs_Splitting(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking { StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero) };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
            TotalAmount = 100m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Yearly }
        };

        Should.Throw<ArgumentOutOfRangeException>(() => sut.GetSchedule(recurringBooking, marketplaceBooking, (OrganizationBillingCycle)999));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Null_When_Cadence_Cannot_Be_Represented_As_A_Xero_Repeating_Invoice(
        string pricingId)
    {
        var sut = new XeroRepeatingInvoiceScheduleService(new RecurringInvoiceBillingScheduleService());
        var recurringBooking = new RecurringBooking
        {
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero), EndDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var marketplaceBooking = new MarketplaceBooking
        {
            BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            TotalAmount = 58m,
            ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Daily }
        };

        var result = sut.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycle.Monthly);

        result.ShouldBeNull();
    }
}
