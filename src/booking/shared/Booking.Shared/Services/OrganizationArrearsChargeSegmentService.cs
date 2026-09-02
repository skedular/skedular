using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface IOrganizationArrearsChargeSegmentService
{
    IReadOnlyList<ArrearsChargeSegment> BuildChargeSegments(Models.Booking booking, OrganizationBillingCycle billingCycle);
    IReadOnlyList<ArrearsChargeSegment> BuildInitialRecurringChargeSegments(RecurringBooking recurringBooking, OrganizationBillingCycle billingCycle);
}

public class OrganizationArrearsChargeSegmentService(ILogger<OrganizationArrearsChargeSegmentService> logger)
    : IOrganizationArrearsChargeSegmentService
{
    public IReadOnlyList<ArrearsChargeSegment> BuildChargeSegments(Models.Booking booking, OrganizationBillingCycle billingCycle)
    {
        var marketplaceBooking = booking.MarketplaceBooking;
        if (marketplaceBooking is null || marketplaceBooking.BillingMode != ProductPricingBillingMode.InArrears)
        {
            return [];
        }

        var organizationId = marketplaceBooking.ProductVersion.Product.Organization.Id;
        var customerId = marketplaceBooking.PaidByCustomer?.Id
                         ?? booking.CreatedByCustomer?.Id
                         ?? booking.InvolvedCustomers.Select(item => item.Id).Distinct().SingleOrDefault();
        var currency = marketplaceBooking.Currency ?? marketplaceBooking.ProductVersion.Currency;

        if (string.IsNullOrWhiteSpace(organizationId) || string.IsNullOrWhiteSpace(customerId))
        {
            return [];
        }

        var purchaseCadence = marketplaceBooking.ProductPricing.PurchaseCadence;

        var split = ShouldSplitByBillingCycle(purchaseCadence, billingCycle);
        logger.LogInformation(
            "Built arrears charge plan. BookingId={BookingId}, PurchaseCadence={PurchaseCadence}, BillingCycle={BillingCycle}, SplitByBillingCycle={SplitByBillingCycle}",
            booking.Id,
            purchaseCadence,
            billingCycle,
            split);
        return split
            ? BuildInstallmentsByBillingCycle(booking, organizationId, customerId, currency, billingCycle)
            : [BuildSingleChargeSegment(booking, organizationId, customerId, currency)];
    }

    public IReadOnlyList<ArrearsChargeSegment> BuildInitialRecurringChargeSegments(
        RecurringBooking recurringBooking,
        OrganizationBillingCycle billingCycle)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        if (marketplaceBooking is null || marketplaceBooking.BillingMode != ProductPricingBillingMode.InArrears || !recurringBooking.EndDate.HasValue)
        {
            return [];
        }

        var organizationId = marketplaceBooking.ProductVersion.Product.Organization.Id;
        var customerId = marketplaceBooking.PaidByCustomer?.Id
                         ?? recurringBooking.CreatedByCustomer?.Id
                         ?? recurringBooking.InvolvedCustomers.Select(item => item.Id).Distinct().SingleOrDefault();
        var currency = marketplaceBooking.Currency ?? marketplaceBooking.ProductVersion.Currency;

        if (string.IsNullOrWhiteSpace(organizationId) || string.IsNullOrWhiteSpace(customerId))
        {
            return [];
        }

        var servicePeriod = new BillingPeriod(recurringBooking.StartDate, recurringBooking.EndDate.Value.AddDays(1));
        var purchaseCadence = marketplaceBooking.ProductPricing.PurchaseCadence;

        return ShouldSplitByBillingCycle(purchaseCadence, billingCycle)
            ? BuildRecurringInstallmentsByBillingCycle(recurringBooking, organizationId, customerId, currency, billingCycle, servicePeriod)
            : [BuildSingleRecurringChargeSegment(recurringBooking, organizationId, customerId, currency, servicePeriod)];
    }

    private static ArrearsChargeSegment BuildSingleChargeSegment(
        Models.Booking booking,
        string organizationId,
        string customerId,
        Currency currency)
    {
        var amount = CalculateBookingChargeAmount(booking).RoundedDecimal();
        var earnedAt = GetEarnedAt(new BillingPeriod(booking.From, booking.Until));

        return new ArrearsChargeSegment(
            BuildSegmentKey(booking.Id, customerId, booking.From, booking.Until),
            booking.Id,
            organizationId,
            customerId,
            currency,
            new BillingPeriod(booking.From, booking.Until),
            earnedAt,
            amount,
            BuildDescription(booking));
    }

    private static IReadOnlyList<ArrearsChargeSegment> BuildInstallmentsByBillingCycle(
        Models.Booking booking,
        string organizationId,
        string customerId,
        Currency currency,
        OrganizationBillingCycle billingCycle)
    {
        var totalAmount = CalculateBookingChargeAmount(booking).RoundedDecimal();
        var servicePeriods = SplitIntoBillingCyclePeriods(booking.From, booking.Until, billingCycle);
        var totalDurationTicks = servicePeriods.Sum(item => (item.EndExclusive - item.StartInclusive).Ticks);
        var remaining = totalAmount;
        var items = new List<ArrearsChargeSegment>(servicePeriods.Count);

        for (var index = 0; index < servicePeriods.Count; index++)
        {
            var servicePeriod = servicePeriods[index];
            var earnedAt = GetEarnedAt(servicePeriod);
            var installmentAmount = index == servicePeriods.Count - 1
                ? remaining.RoundedDecimal()
                : (totalAmount * (servicePeriod.EndExclusive - servicePeriod.StartInclusive).Ticks / totalDurationTicks).RoundedDecimal();
            remaining -= installmentAmount;

            items.Add(new ArrearsChargeSegment(
                BuildSegmentKey(booking.Id, customerId, servicePeriod.StartInclusive, servicePeriod.EndExclusive),
                booking.Id,
                organizationId,
                customerId,
                currency,
                servicePeriod,
                earnedAt,
                installmentAmount,
                BuildDescription(booking)));
        }

        return items;
    }

    private static ArrearsChargeSegment BuildSingleRecurringChargeSegment(
        RecurringBooking recurringBooking,
        string organizationId,
        string customerId,
        Currency currency,
        BillingPeriod servicePeriod)
    {
        var amount = CalculateRecurringChargeAmount(recurringBooking, servicePeriod).RoundedDecimal();
        var earnedAt = GetEarnedAt(servicePeriod);

        return new ArrearsChargeSegment(
            BuildSegmentKey(recurringBooking.Id, customerId, servicePeriod.StartInclusive, servicePeriod.EndExclusive),
            recurringBooking.Id,
            organizationId,
            customerId,
            currency,
            servicePeriod,
            earnedAt,
            amount,
            BuildDescription(recurringBooking, servicePeriod));
    }

    private static IReadOnlyList<ArrearsChargeSegment> BuildRecurringInstallmentsByBillingCycle(
        RecurringBooking recurringBooking,
        string organizationId,
        string customerId,
        Currency currency,
        OrganizationBillingCycle billingCycle,
        BillingPeriod fullServicePeriod)
    {
        var totalAmount = CalculateRecurringChargeAmount(recurringBooking, fullServicePeriod).RoundedDecimal();
        var servicePeriods = SplitIntoBillingCyclePeriodsFromStart(fullServicePeriod.StartInclusive, fullServicePeriod.EndExclusive, billingCycle);
        var remaining = totalAmount;
        var items = new List<ArrearsChargeSegment>(servicePeriods.Count);

        for (var index = 0; index < servicePeriods.Count; index++)
        {
            var servicePeriod = servicePeriods[index];
            var earnedAt = GetEarnedAt(servicePeriod);
            var installmentAmount = index == servicePeriods.Count - 1
                ? remaining.RoundedDecimal()
                : (totalAmount / servicePeriods.Count).RoundedDecimal();
            remaining -= installmentAmount;

            items.Add(new ArrearsChargeSegment(
                BuildSegmentKey(recurringBooking.Id, customerId, servicePeriod.StartInclusive, servicePeriod.EndExclusive),
                recurringBooking.Id,
                organizationId,
                customerId,
                currency,
                servicePeriod,
                earnedAt,
                installmentAmount,
                BuildDescription(recurringBooking, servicePeriod)));
        }

        return items;
    }

    private static List<BillingPeriod> SplitIntoBillingCyclePeriodsFromStart(
        DateTimeOffset from,
        DateTimeOffset until,
        OrganizationBillingCycle billingCycle)
    {
        var periods = new List<BillingPeriod>();
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
            periods.Add(new BillingPeriod(cursor, periodEnd));
            cursor = periodEnd;
        }

        return periods;
    }

    private static decimal CalculateBookingChargeAmount(Models.Booking booking)
    {
        var marketplaceBooking = booking.MarketplaceBooking!;
        var pricing = marketplaceBooking.ProductPricing;
        return pricing.Price * marketplaceBooking.Quantity;
    }

    private static decimal CalculateRecurringChargeAmount(RecurringBooking recurringBooking, BillingPeriod servicePeriod)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking!;
        var pricing = marketplaceBooking.ProductPricing;
        return pricing.Price * marketplaceBooking.Quantity;
    }

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
            _ => false,
        };

    private static List<BillingPeriod> SplitIntoBillingCyclePeriods(
        DateTimeOffset from,
        DateTimeOffset until,
        OrganizationBillingCycle billingCycle)
    {
        var periods = new List<BillingPeriod>();
        var cursor = from;
        while (cursor < until)
        {
            var currentBoundaryStart = GetCurrentBillingCycleStart(cursor, billingCycle);
            var nextBoundary = GetNextBillingCycleBoundary(currentBoundaryStart, billingCycle);
            var periodStart = cursor > currentBoundaryStart ? cursor : currentBoundaryStart;
            var periodEnd = nextBoundary < until ? nextBoundary : until;

            periods.Add(new BillingPeriod(periodStart, periodEnd));
            cursor = periodEnd;
        }

        return periods;
    }

    private static DateTimeOffset GetCurrentBillingCycleStart(DateTimeOffset value, OrganizationBillingCycle billingCycle) =>
        billingCycle switch
        {
            OrganizationBillingCycle.Weekly => GetCurrentWeekStart(value),
            OrganizationBillingCycle.Fortnightly => GetCurrentFortnightStart(value),
            OrganizationBillingCycle.Monthly => GetCurrentMonthStart(value),
            _ => throw new ArgumentOutOfRangeException(nameof(billingCycle)),
        };

    private static DateTimeOffset GetNextBillingCycleBoundary(DateTimeOffset currentBoundaryStart, OrganizationBillingCycle billingCycle) =>
        billingCycle switch
        {
            OrganizationBillingCycle.Weekly => currentBoundaryStart.AddDays(7),
            OrganizationBillingCycle.Fortnightly => currentBoundaryStart.AddDays(14),
            OrganizationBillingCycle.Monthly => currentBoundaryStart.AddMonths(1),
            _ => throw new ArgumentOutOfRangeException(nameof(billingCycle)),
        };

    private static DateTimeOffset GetCurrentMonthStart(DateTimeOffset value) =>
        new(value.Year, value.Month, 1, 0, 0, 0, value.Offset);

    private static DateTimeOffset GetCurrentWeekStart(DateTimeOffset value)
    {
        var daysSinceMonday = ((int)value.DayOfWeek + 6) % 7;
        return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset).AddDays(-daysSinceMonday);
    }

    private static DateTimeOffset GetCurrentFortnightStart(DateTimeOffset value)
    {
        var weekStart = GetCurrentWeekStart(value);
        var baseMonday = new DateTimeOffset(1970, 1, 5, 0, 0, 0, value.Offset);
        var weeksSinceBase = (int)((weekStart - baseMonday).TotalDays / 7);
        return weeksSinceBase % 2 == 0 ? weekStart : weekStart.AddDays(-7);
    }

    private static DateTimeOffset GetEarnedAt(BillingPeriod servicePeriod) => servicePeriod.EndExclusive.AddTicks(-1);

    private static string BuildDescription(Models.Booking booking)
    {
        var title = booking.MarketplaceBooking?.ProductPricing.ListingMetadata.Title;
        return string.IsNullOrWhiteSpace(title)
            ? BookingInvoiceService.FormatInvoicePeriod(booking.From, booking.Until)
            : $"{title}{Environment.NewLine}{BookingInvoiceService.FormatInvoicePeriod(booking.From, booking.Until)}";
    }

    private static string BuildDescription(RecurringBooking recurringBooking, BillingPeriod servicePeriod)
    {
        var title = recurringBooking.MarketplaceBooking?.ProductPricing.ListingMetadata.Title;
        return string.IsNullOrWhiteSpace(title)
            ? BookingInvoiceService.FormatInvoicePeriod(servicePeriod.StartInclusive, servicePeriod.EndExclusive)
            : $"{title}{Environment.NewLine}{BookingInvoiceService.FormatInvoicePeriod(servicePeriod.StartInclusive, servicePeriod.EndExclusive)}";
    }

    private static string BuildSegmentKey(string bookingId, string customerId, DateTimeOffset periodStart, DateTimeOffset periodEnd) =>
        $"{bookingId}:{customerId}:{periodStart:O}:{periodEnd:O}";
}
