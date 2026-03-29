# Booking Shared Agent Notes

This file covers the shared booking layer. Most non-trivial booking billing behavior lives here.

## Scope

- Applies to `booking/shared/`
- Most important for:
  - arrears billing
  - recurring billing-cycle splitting
  - invoice generation
  - Stripe checkout integration
  - repository loading assumptions

## Arrears Billing Has Two Different Invoice Shapes

Do not merge these mentally.

### First recurring arrears invoice

- Triggered during recurring marketplace subscription creation.
- Uses `BuildInitialRecurringInvoiceDraft(...)`.
- Produces a single first-cycle invoice slice.
- Intended invoice template is the marketplace recurring invoice template via `RecurringInvoiceDocument`.

### Scheduled billing-cycle arrears invoice

- Triggered by the organization billing-cycle workflow later.
- Uses `BuildInvoiceDrafts(...)`.
- Groups all earned uninvoiced arrears lines for a customer into one invoice.
- Can contain multiple items and multiple bookings.

If you only patch one of these flows, you are likely leaving the other one inconsistent.

## Critical Invariant

For recurring in-arrears bookings, these must stay aligned:

- invoice PDF amount
- stored marketplace booking totals
- Stripe checkout amount
- tax treatment

The code paths are separate enough that it is easy to fix one and leave the others wrong.

## Important Files

- Segment generation:
  - `booking/shared/Booking.Shared/Services/OrganizationArrearsChargeSegmentService.cs`
- Planner:
  - `booking/shared/Booking.Shared/Services/OrganizationArrearsBillingPlannerService.cs`
- Scheduled workflow activity:
  - `booking/shared/Booking.Shared/Activities/OrganizationArrearsBillingIntegrations.cs`
- Recurring invoice rendering:
  - `booking/shared/Booking.Shared/Services/BookingInvoiceService.cs`
- Stripe checkout:
  - `booking/shared/Booking.Shared/Activities/StripeIntegrations.cs`
- Repository loading:
  - `booking/shared/Booking.Shared/Repositories/BookingRepository.cs`

## Common Failure Modes

- Missing `ProductVersion -> Product -> Organization` on loaded bookings causes arrears generation to return no segments.
- Missing customer identity on the booking graph causes arrears generation to return no segments.
- Fixing first recurring invoice amount without re-checking tax produces GST `0%` regressions.
- Fixing PDF totals without re-checking Stripe checkout produces checkout mismatch regressions.

## Testing Guidance

- Pure billing math belongs in `Booking.Shared.UnitTests`.
- End-to-end workflow behavior does not belong in booking shared unit tests.
- If you are validating real API + Temporal + DB effects, use system tests instead of inventing fakes in booking-local integration tests.
