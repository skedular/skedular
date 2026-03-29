using Api.Shared.Services.Models;
using Booking.Shared.Models;

namespace Booking.Shared.Services;

public interface IOrganizationArrearsBillingPlannerService
{
    ICollection<ArrearsInvoiceDraft> BuildInvoiceDrafts(
        BillingPeriod billingPeriod,
        OrganizationBillingCycle billingCycle,
        ICollection<Models.Booking> bookings,
        ICollection<string>? excludedSegmentKeys = null);

    ArrearsInvoiceDraft? BuildInitialRecurringInvoiceDraft(
        RecurringBooking recurringBooking,
        OrganizationBillingCycle billingCycle,
        ICollection<string>? excludedSegmentKeys = null);
}

public class OrganizationArrearsBillingPlannerService(IOrganizationArrearsChargeSegmentService organizationArrearsChargeSegmentService)
    : IOrganizationArrearsBillingPlannerService
{
    public ICollection<ArrearsInvoiceDraft> BuildInvoiceDrafts(
        BillingPeriod billingPeriod,
        OrganizationBillingCycle billingCycle,
        ICollection<Models.Booking> bookings,
        ICollection<string>? excludedSegmentKeys = null) =>
        bookings
            .SelectMany(item => organizationArrearsChargeSegmentService.BuildChargeSegments(item, billingCycle))
            // Workflow state keeps segment keys that were already invoiced so retries/manual reruns
            // do not emit duplicate invoices for the same earned usage slice.
            .Where(item => billingPeriod.Contains(item.EarnedAt))
            .Where(item => excludedSegmentKeys is null || !excludedSegmentKeys.Contains(item.SegmentKey))
            .GroupBy(item => new { item.OrganizationId, item.CustomerId, item.Currency })
            .Select(group => new ArrearsInvoiceDraft(
                group.Key.OrganizationId,
                group.Key.CustomerId,
                group.Key.Currency,
                billingPeriod,
                group
                    .OrderBy(item => item.EarnedAt)
                    .Select(item => new ArrearsInvoiceDraftLine(
                        item.SegmentKey,
                        item.BookingId,
                        item.ServicePeriod,
                        item.EarnedAt,
                        item.Amount,
                        item.Description))
                    .ToList()))
            .ToList();

    public ArrearsInvoiceDraft? BuildInitialRecurringInvoiceDraft(
        RecurringBooking recurringBooking,
        OrganizationBillingCycle billingCycle,
        ICollection<string>? excludedSegmentKeys = null) =>
        organizationArrearsChargeSegmentService.BuildInitialRecurringChargeSegments(recurringBooking, billingCycle)
            .Where(item => excludedSegmentKeys is null || !excludedSegmentKeys.Contains(item.SegmentKey))
            .OrderBy(item => item.EarnedAt)
            .GroupBy(item => new { item.OrganizationId, item.CustomerId, item.Currency })
            .Select(group =>
            {
                var firstSegment = group.First();

                return new ArrearsInvoiceDraft(
                    group.Key.OrganizationId,
                    group.Key.CustomerId,
                    group.Key.Currency,
                    firstSegment.ServicePeriod,
                    [
                        new ArrearsInvoiceDraftLine(
                            firstSegment.SegmentKey,
                            firstSegment.BookingId,
                            firstSegment.ServicePeriod,
                            firstSegment.EarnedAt,
                            firstSegment.Amount,
                            firstSegment.Description)
                    ]);
            })
            .FirstOrDefault();
}
