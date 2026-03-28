using Api.Shared.Services.Models;
using Enterprise.Shared;

namespace Booking.Shared.Models;

// These records are the workflow/activity boundary for organization-level arrears billing.
// They stay in Models because they are passed across services, activities, workflows, and tests.
public record OrganizationArrearsBillingConfiguration(string OrganizationId, OrganizationBillingCycle BillingCycle);

public record BillingPeriod(DateTimeOffset StartInclusive, DateTimeOffset EndExclusive)
{
    public bool Contains(DateTimeOffset value) => value >= StartInclusive && value < EndExclusive;
}

// A charge segment represents one earned amount that can appear on a customer invoice.
// Long-running cadences are split into multiple segments so reruns can dedupe at segment level.
public record ArrearsChargeSegment(
    string SegmentKey,
    string BookingId,
    string OrganizationId,
    string CustomerId,
    string Currency,
    BillingPeriod ServicePeriod,
    DateTimeOffset EarnedAt,
    decimal Amount,
    string Description);

public record ArrearsInvoiceDraftLine(
    string SegmentKey,
    string BookingId,
    BillingPeriod ServicePeriod,
    DateTimeOffset EarnedAt,
    decimal Amount,
    string Description);

public record ArrearsInvoiceDraft(
    string OrganizationId,
    string CustomerId,
    string Currency,
    BillingPeriod BillingPeriod,
    ICollection<ArrearsInvoiceDraftLine> Lines)
{
    public decimal TotalAmount => Lines.Sum(item => item.Amount).RoundedDecimal();
}
