using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public record MarketplaceBookingPaymentProjection(
    MarketplaceBooking RepresentativeMarketplaceBooking,
    PaymentStatus PaymentStatus);

public class MarketplaceBookingSubscription : ModelBaseWithDeleted
{
    public DateTimeOffset StartedAt { get; set; }
    public TimeOnly From { get; set; }
    public TimeOnly Until { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public DateTimeOffset? NextRenewalAt { get; set; }
    public MarketplaceBookingSubscriptionStatus Status { get; set; }
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public bool CancellationPolicyOverridden { get; set; }
    public string? CancellationOverrideReason { get; set; }
    public IReadOnlyList<DayOfWeek> WeeklySelectedDays { get; set; } = [];
    public MarketplaceBooking MarketplaceBooking { get; set; } = new();
    public IReadOnlyList<Customer> InvolvedCustomers { get; set; } = [];
    public IReadOnlyList<Organization> InvolvedOrganizations { get; set; } = [];
    public IReadOnlyList<Team> InvolvedTeams { get; set; } = [];
    public IReadOnlyList<Resource> RequestedResources { get; set; } = [];
    public Customer? CreatedByCustomer { get; set; }
    public Customer? LastModifiedByCustomer { get; set; }
    public Customer? DeletedByCustomer { get; set; }
    public IReadOnlyList<RecurringBooking> RecurringBookings { get; set; } = [];

    public (DateTimeOffset From, DateTimeOffset Until) ResolveBookingWindow() =>
        ResolveBookingWindow(StartedAt, From, Until);

    public static (DateTimeOffset From, DateTimeOffset Until) ResolveBookingWindow(
        DateTimeOffset termStart,
        TimeOnly from,
        TimeOnly until)
    {
        var resolvedFrom = new DateTimeOffset(termStart.Date.Add(from.ToTimeSpan()), termStart.Offset);
        var resolvedUntil = new DateTimeOffset(termStart.Date.Add(until.ToTimeSpan()), termStart.Offset);
        return (resolvedFrom, resolvedUntil);
    }

    public MarketplaceBookingPaymentProjection? ResolveCurrentBillingWindowPaymentProjection(DateTimeOffset now)
    {
        var organization = MarketplaceBooking?.ProductVersion?.Product?.Organization;
        if (organization is null)
        {
            return null;
        }

        var organizationBillingCycle = organization.BillingCycle;
        var recurringBookingsInWindow = RecurringBookings
            .Where(item => !item.IsDeleted() && item.MarketplaceBooking is not null)
            .Where(item => IntersectsBillingWindow(item, now, organizationBillingCycle))
            .OrderBy(item => item.StartDate)
            .ToList();

        if (recurringBookingsInWindow.Count == 0)
        {
            return null;
        }

        var representativeMarketplaceBooking = recurringBookingsInWindow.Last().MarketplaceBooking!;
        var paymentStatus = ResolveAggregatedPaymentStatus(
        [
            .. recurringBookingsInWindow
                .Select(item => item.MarketplaceBooking!),
        ]);

        return new MarketplaceBookingPaymentProjection(representativeMarketplaceBooking, paymentStatus);
    }

    private bool IntersectsBillingWindow(
        RecurringBooking recurringBooking,
        DateTimeOffset now,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var (windowStartInclusive, windowEndExclusive) = ResolveCurrentBillingWindow(now, organizationBillingCycle);
        var recurringBookingEndExclusive = recurringBooking.EndDate?.AddDays(1) ??
                                           ResolveRecurringBookingCycleEndExclusive(recurringBooking);

        return recurringBooking.StartDate < windowEndExclusive && recurringBookingEndExclusive > windowStartInclusive;
    }

    private (DateTimeOffset StartInclusive, DateTimeOffset EndExclusive) ResolveCurrentBillingWindow(
        DateTimeOffset now,
        OrganizationBillingCycle organizationBillingCycle)
    {
        var startInclusive = StartedAt;
        var endExclusive = AdvanceBillingWindow(startInclusive, organizationBillingCycle);

        while (now >= endExclusive)
        {
            startInclusive = endExclusive;
            endExclusive = AdvanceBillingWindow(startInclusive, organizationBillingCycle);
        }

        return (startInclusive, endExclusive);
    }

    private static DateTimeOffset AdvanceBillingWindow(DateTimeOffset startInclusive, OrganizationBillingCycle organizationBillingCycle) =>
        organizationBillingCycle switch
        {
            OrganizationBillingCycle.Weekly => startInclusive.AddDays(7),
            OrganizationBillingCycle.Fortnightly => startInclusive.AddDays(14),
            OrganizationBillingCycle.Monthly => startInclusive.AddMonths(1),
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle)),
        };

    private static DateTimeOffset ResolveRecurringBookingCycleEndExclusive(RecurringBooking recurringBooking)
    {
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking);

        return recurringBooking.MarketplaceBooking.ProductPricing.PurchaseCadence switch
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
            _ => recurringBooking.StartDate.AddDays(1),
        };
    }

    private static PaymentStatus ResolveAggregatedPaymentStatus(IReadOnlyList<MarketplaceBooking> marketplaceBookings)
    {
        var paymentRequiredBookings = marketplaceBookings.Where(item => item.IsPaymentRequired).ToList();
        if (paymentRequiredBookings.Count == 0)
        {
            return PaymentStatus.NoPaymentRequired;
        }

        var paymentStatuses = paymentRequiredBookings.Select(item => item.PaymentStatus).ToList();
        if (paymentStatuses.All(item => item is PaymentStatus.Confirmed or PaymentStatus.NoPaymentRequired))
        {
            return PaymentStatus.Confirmed;
        }

        if (paymentStatuses.Contains(PaymentStatus.Rejected))
        {
            return PaymentStatus.Rejected;
        }

        if (paymentStatuses.Contains(PaymentStatus.Expired))
        {
            return PaymentStatus.Expired;
        }

        return paymentStatuses.Contains(PaymentStatus.RecordNeverCreated) ? PaymentStatus.RecordNeverCreated : PaymentStatus.Pending;
    }
}
