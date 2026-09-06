using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using XeroRepeatingInvoiceScheduleSourceConstants = Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants;

namespace Booking.Shared.Services;

public record RecurringInvoiceBillingDefinition(string Source, MembershipTerm MembershipTerm, decimal InvoiceAmount);

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
        var membershipTerm = marketplaceBooking.ProductPricing.MembershipTerm;
        var shouldSplitByBillingCycle = ShouldSplitByBillingCycle(membershipTerm, organizationBillingCycle);

        if (!shouldSplitByBillingCycle)
        {
            return new RecurringInvoiceBillingDefinition(
                XeroRepeatingInvoiceScheduleSourceConstants.MembershipTerm,
                membershipTerm,
                CalculateTotalRecurringChargeAmount(marketplaceBooking));
        }

        return new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            MapBillingCycleToCadence(organizationBillingCycle),
            CalculateInstallmentAmount(recurringBooking, marketplaceBooking, membershipTerm, organizationBillingCycle));
    }

    private static MembershipTerm MapBillingCycleToCadence(OrganizationBillingCycle organizationBillingCycle) =>
        organizationBillingCycle switch
        {
            OrganizationBillingCycle.Weekly => MembershipTerm.Weekly,
            OrganizationBillingCycle.Fortnightly => MembershipTerm.Fortnightly,
            OrganizationBillingCycle.Monthly => MembershipTerm.Monthly,
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle)),
        };

    private static decimal CalculateInstallmentAmount(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        MembershipTerm membershipTerm,
        OrganizationBillingCycle organizationBillingCycle)
    {
        if (HasPersistedRecurringChargeAmount(marketplaceBooking))
        {
            return CalculateTotalRecurringChargeAmount(marketplaceBooking);
        }

        var totalAmount = CalculateTotalRecurringChargeAmount(marketplaceBooking);
        var cycleEndExclusive = ResolveCycleEndExclusive(recurringBooking, membershipTerm);
        var installmentCount = SplitIntoBillingCyclePeriodsFromStart(recurringBooking.StartDate, cycleEndExclusive, organizationBillingCycle).Count;

        return installmentCount <= 1
            ? totalAmount
            : decimal.Round(totalAmount / installmentCount, 4, MidpointRounding.AwayFromZero);
    }

    private static bool HasPersistedRecurringChargeAmount(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.ProductPricing.IsTaxInclusive
            ? marketplaceBooking.TotalAmount.HasValue || marketplaceBooking.TotalAmountExcludeTax.HasValue
            : marketplaceBooking.TotalAmountExcludeTax.HasValue || marketplaceBooking.TotalAmount.HasValue;

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

    private static DateTimeOffset ResolveCycleEndExclusive(RecurringBooking recurringBooking, MembershipTerm membershipTerm) =>
        recurringBooking.EndDate?.AddDays(1) ?? membershipTerm switch
        {
            MembershipTerm.Weekly => recurringBooking.StartDate.AddDays(7),
            MembershipTerm.Fortnightly => recurringBooking.StartDate.AddDays(14),
            MembershipTerm.Monthly => recurringBooking.StartDate.AddMonths(1),
            MembershipTerm.TwoMonths => recurringBooking.StartDate.AddMonths(2),
            MembershipTerm.Quarterly => recurringBooking.StartDate.AddMonths(3),
            MembershipTerm.FourMonths => recurringBooking.StartDate.AddMonths(4),
            MembershipTerm.FiveMonths => recurringBooking.StartDate.AddMonths(5),
            MembershipTerm.SixMonths => recurringBooking.StartDate.AddMonths(6),
            MembershipTerm.Yearly => recurringBooking.StartDate.AddYears(1),
            _ => recurringBooking.StartDate.AddDays(1),
        };

    private static bool ShouldSplitByBillingCycle(MembershipTerm membershipTerm, OrganizationBillingCycle billingCycle) =>
        billingCycle switch
        {
            OrganizationBillingCycle.Weekly => membershipTerm is MembershipTerm.Fortnightly or
                MembershipTerm.Monthly or
                MembershipTerm.TwoMonths or
                MembershipTerm.Quarterly or
                MembershipTerm.FourMonths or
                MembershipTerm.FiveMonths or
                MembershipTerm.SixMonths or
                MembershipTerm.Yearly,
            OrganizationBillingCycle.Fortnightly => membershipTerm is MembershipTerm.Monthly or
                MembershipTerm.TwoMonths or
                MembershipTerm.Quarterly or
                MembershipTerm.FourMonths or
                MembershipTerm.FiveMonths or
                MembershipTerm.SixMonths or
                MembershipTerm.Yearly,
            OrganizationBillingCycle.Monthly => membershipTerm is MembershipTerm.TwoMonths or
                MembershipTerm.Quarterly or
                MembershipTerm.FourMonths or
                MembershipTerm.FiveMonths or
                MembershipTerm.SixMonths or
                MembershipTerm.Yearly,
            _ => throw new ArgumentOutOfRangeException(nameof(billingCycle)),
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
                _ => throw new ArgumentOutOfRangeException(nameof(billingCycle)),
            };

            var periodEnd = nextBoundary < until ? nextBoundary : until;
            periods.Add((cursor, periodEnd));
            cursor = periodEnd;
        }

        return periods;
    }
}
