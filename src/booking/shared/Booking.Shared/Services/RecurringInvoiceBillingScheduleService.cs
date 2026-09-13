using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using XeroRepeatingInvoiceScheduleSourceConstants = Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants;

namespace Booking.Shared.Services;

public record RecurringInvoiceBillingDefinition(
    string Source,
    MembershipTerm MembershipTerm,
    decimal InvoiceAmount,
    int InstallmentCount = 1);

public interface IRecurringInvoiceBillingScheduleService
{
    RecurringInvoiceBillingDefinition GetSchedule(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle);

    RecurringInvoiceBillingDefinition GetSchedule(
        ProductPricing pricing,
        decimal totalAmount,
        OrganizationBillingCycle organizationBillingCycle);
}

public class RecurringInvoiceBillingScheduleService : IRecurringInvoiceBillingScheduleService
{
    public RecurringInvoiceBillingDefinition GetSchedule(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle) =>
        HasPersistedRecurringChargeAmount(marketplaceBooking)
            ? GetPersistedSchedule(marketplaceBooking, organizationBillingCycle)
            : GetSchedule(
                marketplaceBooking.ProductPricing,
                CalculateTotalRecurringChargeAmount(marketplaceBooking),
                organizationBillingCycle);

    public RecurringInvoiceBillingDefinition GetSchedule(
        ProductPricing pricing,
        decimal totalAmount,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var membershipTerm = pricing.MembershipTerm;
        var shouldSplitByBillingCycle = ShouldSplitByBillingCycle(membershipTerm, organizationBillingCycle);

        if (!shouldSplitByBillingCycle)
        {
            return new RecurringInvoiceBillingDefinition(
                XeroRepeatingInvoiceScheduleSourceConstants.MembershipTerm,
                membershipTerm,
                decimal.Round(totalAmount, 4, MidpointRounding.AwayFromZero));
        }

        var installmentCount = CalculateInstallmentCount(membershipTerm, organizationBillingCycle);

        return new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            MapBillingCycleToCadence(organizationBillingCycle),
            decimal.Round(totalAmount / installmentCount, 4, MidpointRounding.AwayFromZero),
            installmentCount);
    }

    private static MembershipTerm MapBillingCycleToCadence(OrganizationBillingCycle organizationBillingCycle) =>
        organizationBillingCycle switch
        {
            OrganizationBillingCycle.Weekly => MembershipTerm.Weekly,
            OrganizationBillingCycle.Fortnightly => MembershipTerm.Fortnightly,
            OrganizationBillingCycle.Monthly => MembershipTerm.Monthly,
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle)),
        };

    private static int CalculateInstallmentCount(MembershipTerm membershipTerm, OrganizationBillingCycle organizationBillingCycle) =>
        SplitIntoBillingCyclePeriodsFromStart(
                DateTimeOffset.UnixEpoch,
                ResolveCycleEndExclusive(DateTimeOffset.UnixEpoch, membershipTerm),
                organizationBillingCycle)
            .Count;

    private static bool HasPersistedRecurringChargeAmount(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.ProductPricing.IsTaxInclusive
            ? marketplaceBooking.TotalAmount.HasValue || marketplaceBooking.TotalAmountExcludeTax.HasValue
            : marketplaceBooking.TotalAmountExcludeTax.HasValue || marketplaceBooking.TotalAmount.HasValue;

    private static RecurringInvoiceBillingDefinition GetPersistedSchedule(
        MarketplaceBooking marketplaceBooking,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var membershipTerm = marketplaceBooking.ProductPricing.MembershipTerm;
        var totalAmount = CalculateTotalRecurringChargeAmount(marketplaceBooking);
        if (!ShouldSplitByBillingCycle(membershipTerm, organizationBillingCycle))
        {
            return new RecurringInvoiceBillingDefinition(
                XeroRepeatingInvoiceScheduleSourceConstants.MembershipTerm,
                membershipTerm,
                totalAmount);
        }

        var installmentCount = CalculateInstallmentCount(membershipTerm, organizationBillingCycle);
        return new RecurringInvoiceBillingDefinition(
            XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            MapBillingCycleToCadence(organizationBillingCycle),
            totalAmount,
            installmentCount);
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

    private static DateTimeOffset ResolveCycleEndExclusive(DateTimeOffset start, MembershipTerm membershipTerm) => membershipTerm switch
    {
        MembershipTerm.Weekly => start.AddDays(7),
        MembershipTerm.Fortnightly => start.AddDays(14),
        MembershipTerm.Monthly => start.AddMonths(1),
        MembershipTerm.TwoMonths => start.AddMonths(2),
        MembershipTerm.Quarterly => start.AddMonths(3),
        MembershipTerm.FourMonths => start.AddMonths(4),
        MembershipTerm.FiveMonths => start.AddMonths(5),
        MembershipTerm.SixMonths => start.AddMonths(6),
        MembershipTerm.Yearly => start.AddYears(1),
        _ => start.AddDays(1),
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
