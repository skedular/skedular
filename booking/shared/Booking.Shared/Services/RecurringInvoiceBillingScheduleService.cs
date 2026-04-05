using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using XeroRepeatingInvoiceScheduleSourceConstants = Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants;

namespace Booking.Shared.Services;

public record RecurringInvoiceBillingDefinition(
    string Source,
    ProductPricingCadence Cadence,
    decimal InvoiceAmount);

public interface IRecurringInvoiceBillingScheduleService
{
    RecurringInvoiceBillingDefinition GetSchedule(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle);
}

public class RecurringInvoiceBillingScheduleService : IRecurringInvoiceBillingScheduleService
{
    public RecurringInvoiceBillingDefinition GetSchedule(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var purchaseCadence = marketplaceBooking.ProductPricing.PurchaseCadence;
        var shouldSplitByBillingCycle = ShouldSplitByBillingCycle(purchaseCadence, organizationBillingCycle);

        if (!shouldSplitByBillingCycle)
        {
            return new RecurringInvoiceBillingDefinition(
                XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
                purchaseCadence,
                CalculateTotalRecurringChargeAmount(marketplaceBooking));
        }

        return new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            MapBillingCycleToCadence(organizationBillingCycle),
            CalculateInstallmentAmount(recurringBooking, marketplaceBooking, purchaseCadence, organizationBillingCycle));
    }

    private static ProductPricingCadence MapBillingCycleToCadence(OrganizationBillingCycle organizationBillingCycle) =>
        organizationBillingCycle switch
        {
            OrganizationBillingCycle.Weekly => ProductPricingCadence.Weekly,
            OrganizationBillingCycle.Fortnightly => ProductPricingCadence.Fortnightly,
            OrganizationBillingCycle.Monthly => ProductPricingCadence.Monthly,
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle))
        };

    private static decimal CalculateInstallmentAmount(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductPricingCadence purchaseCadence,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var totalAmount = CalculateTotalRecurringChargeAmount(marketplaceBooking);
        var cycleEndExclusive = ResolveCycleEndExclusive(recurringBooking, purchaseCadence);
        var installmentCount = SplitIntoBillingCyclePeriodsFromStart(recurringBooking.StartDate, cycleEndExclusive, organizationBillingCycle).Count;

        return installmentCount <= 1
            ? totalAmount
            : decimal.Round(totalAmount / installmentCount, 4, MidpointRounding.AwayFromZero);
    }

    private static decimal CalculateTotalRecurringChargeAmount(MarketplaceBooking marketplaceBooking)
    {
        var quantity = marketplaceBooking.Quantity <= 0 ? 1 : marketplaceBooking.Quantity;
        var fallbackTotalAmount = decimal.Round(
            marketplaceBooking.ProductPricing.Price * quantity,
            4,
            MidpointRounding.AwayFromZero);
        var totalAmount = marketplaceBooking.ProductPricing.IsTaxInclusive
            ? marketplaceBooking.TotalAmount ?? marketplaceBooking.TotalAmountExcludeTax ?? fallbackTotalAmount
            : marketplaceBooking.TotalAmountExcludeTax ?? marketplaceBooking.TotalAmount ?? fallbackTotalAmount;

        return decimal.Round(totalAmount, 4, MidpointRounding.AwayFromZero);
    }

    private static DateTimeOffset ResolveCycleEndExclusive(RecurringBooking recurringBooking, ProductPricingCadence purchaseCadence) =>
        recurringBooking.EndDate?.AddDays(1) ?? purchaseCadence switch
        {
            ProductPricingCadence.Weekly => recurringBooking.StartDate.AddDays(7),
            ProductPricingCadence.Fortnightly => recurringBooking.StartDate.AddDays(14),
            ProductPricingCadence.Monthly => recurringBooking.StartDate.AddMonths(1),
            ProductPricingCadence.TwoMonths => recurringBooking.StartDate.AddMonths(2),
            ProductPricingCadence.Quarterly => recurringBooking.StartDate.AddMonths(3),
            ProductPricingCadence.FourMonths => recurringBooking.StartDate.AddMonths(4),
            ProductPricingCadence.FiveMonths => recurringBooking.StartDate.AddMonths(5),
            ProductPricingCadence.SixMonths => recurringBooking.StartDate.AddMonths(6),
            ProductPricingCadence.Yearly => recurringBooking.StartDate.AddYears(1),
            _ => recurringBooking.StartDate.AddDays(1)
        };

    private static bool ShouldSplitByBillingCycle(ProductPricingCadence cadence, OrganizationBillingCycle billingCycle) =>
        billingCycle switch
        {
            OrganizationBillingCycle.Weekly => cadence is ProductPricingCadence.Fortnightly or
                ProductPricingCadence.Monthly or
                ProductPricingCadence.TwoMonths or
                ProductPricingCadence.Quarterly or
                ProductPricingCadence.FourMonths or
                ProductPricingCadence.FiveMonths or
                ProductPricingCadence.SixMonths or
                ProductPricingCadence.Yearly,
            OrganizationBillingCycle.Fortnightly => cadence is ProductPricingCadence.Monthly or
                ProductPricingCadence.TwoMonths or
                ProductPricingCadence.Quarterly or
                ProductPricingCadence.FourMonths or
                ProductPricingCadence.FiveMonths or
                ProductPricingCadence.SixMonths or
                ProductPricingCadence.Yearly,
            OrganizationBillingCycle.Monthly => cadence is ProductPricingCadence.TwoMonths or
                ProductPricingCadence.Quarterly or
                ProductPricingCadence.FourMonths or
                ProductPricingCadence.FiveMonths or
                ProductPricingCadence.SixMonths or
                ProductPricingCadence.Yearly,
            _ => throw new ArgumentOutOfRangeException(nameof(billingCycle))
        };

    private static List<(DateTimeOffset StartInclusive, DateTimeOffset EndExclusive)> SplitIntoBillingCyclePeriodsFromStart(
        DateTimeOffset from,
        DateTimeOffset until,
        OrganizationBillingCycle billingCycle)
    {
        var periods = new List<(DateTimeOffset StartInclusive, DateTimeOffset EndExclusive)>();
        var cursor = from;

        while (cursor < until)
        {
            var nextBoundary = billingCycle switch
            {
                OrganizationBillingCycle.Weekly => cursor.AddDays(7),
                OrganizationBillingCycle.Fortnightly => cursor.AddDays(14),
                OrganizationBillingCycle.Monthly => cursor.AddMonths(1),
                _ => throw new ArgumentOutOfRangeException(nameof(billingCycle))
            };

            var periodEnd = nextBoundary < until ? nextBoundary : until;
            periods.Add((cursor, periodEnd));
            cursor = periodEnd;
        }

        return periods;
    }
}
