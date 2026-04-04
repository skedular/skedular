# Booking Shared Agent Notes

This file covers the shared booking layer. Most non-trivial booking billing behavior lives here.

## Scope

- Applies to `booking/shared/`
- Most important for:
    - arrears billing
    - recurring billing-cycle splitting
    - invoice generation
    - Stripe checkout integration
    - Xero bank-transfer and arrears invoice export
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
- Xero export/reconciliation:
    - `booking/shared/Booking.Shared/Activities/InvoiceIntegrations.cs`
    - `booking/shared/Booking.Shared/Activities/OrganizationArrearsBillingIntegrations.cs`
- Repository loading:
    - `booking/shared/Booking.Shared/Repositories/BookingRepository.cs`

## Xero Integration Rules

- Xero handling in booking is export/reconciliation logic, not org connection ownership.
- Booking reads org Xero connection state from organization and exports invoices only when the org connection is active,
  has a refresh token, and has a selected tenant.
- Keep using side tables such as accounting invoice/contact/payment links for provider state.
- Do not add Xero-specific fields directly to `MarketplaceBooking`, `RecurringBooking`, or `OrganizationArrearsInvoice`
  unless there is a deliberate domain redesign.
- Use `IXeroTokenEncryptionService` for Xero token values when booking needs to refresh or reuse encrypted Xero tokens.
- When org Xero billing mode is `Enabled`, booking should treat Xero as the invoice/export/reconciliation provider for
  supported invoiceable flows.
- Xero webhook handling is raw-body -> Kafka -> processors -> shared reconciliation. Do not add a second direct mutation
  path beside the existing accounting link/payment-event flow.
- Webhook processing should wake the existing per-invoice Temporal monitor workflow immediately through
  `ITemporalService` signal-or-start behavior.
- Keep the invoice monitor workflows per invoice-bearing local entity, not per organization.
- Xero invoice creation and reconciliation must keep local invoice references in sync so `InvoiceUrl` / invoice number
  surfaces can point at the Xero-hosted invoice when available.
- Xero email-send is best-effort. Export should not fail just because Xero email delivery fails after the invoice
  already exists.

## Common Failure Modes

- Missing `ProductVersion -> Product -> Organization` on loaded bookings causes arrears generation to return no
  segments.
- Missing customer identity on the booking graph causes arrears generation to return no segments.
- Fixing first recurring invoice amount without re-checking tax produces GST `0%` regressions.
- Fixing PDF totals without re-checking Stripe checkout produces checkout mismatch regressions.

## Testing Guidance

- Pure billing math belongs in `Booking.Shared.UnitTests`.
- End-to-end workflow behavior does not belong in booking shared unit tests.
- If you are validating real API + Temporal + DB effects, use system tests instead of inventing fakes in booking-local
  integration tests.

## Workflow ID Rule

- Booking workflow IDs belong in `booking/shared/Booking.Shared/Services/WorkflowIdService.cs`.
- Do not inline Temporal workflow ID prefixes or string interpolation in booking services, outbox services, or tests.
- If a workflow ID shape changes, update the workflow ID service and its unit tests instead of patching call sites one by one.

## Workflow ID Test Shape

- Keep booking workflow ID unit tests under `Booking.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- Use one test class/file per workflow ID method, not one monolithic workflow ID test file.
- In booking unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after `sut`.
