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
- When org Xero billing mode is `RepeatingInvoices`, only the recurring marketplace booking invoice path should switch
  to Xero repeating invoice templates.
- Keep the organization billing cycle and recurring purchase cadence separate in that mode. They are not interchangeable
  inputs.
- Keep invoice due days separate from both of them. Invoice due days are payment terms, not cadence.
- Under `RepeatingInvoices`, booking must first calculate the effective invoice cadence before building a Xero repeating
  template:
    - if the product purchase cadence is shorter than or equal to the organization billing cycle, invoice on the
      purchase cadence
    - if the product purchase cadence is longer than the organization billing cycle, split the recurring charge down to
      the organization billing cycle
- That means short cadences like daily or weekly must not be coerced into a monthly repeating invoice just because the
  organization billing cycle is monthly.
- When booking splits a longer recurring cadence down to the organization billing cycle, the repeating invoice amount
  must also be split to the per-installment amount instead of reusing the full recurring charge on every invoice.
- Booking should read invoice due days from organization billing details and apply them consistently to:
    - invoice PDFs
    - normal Xero invoice exports
    - Xero repeating invoice templates
- In practice:
    - the effective repeating cadence is the smaller of the purchase cadence and the organization billing cycle
    - longer purchase cadences are split to organization-cycle installments before export
- Supported effective recurring cadences for Xero repeating templates are `Weekly`, `Fortnightly`, `Monthly`,
  `TwoMonths`, `Quarterly`, `FourMonths`, `FiveMonths`, `SixMonths`, and `Yearly`.
- If the effective recurring cadence is daily or otherwise not representable by Xero repeating invoices, fall back to
  the normal Xero invoice export path instead of forcing an incompatible repeating template.
- This effective recurring billing schedule is provider-agnostic. Internal recurring invoice PDFs and standard Xero
  recurring invoice exports should use the same cadence and per-installment amount decision, not a separate rule.
- Existing recurring exports are not auto-migrated when org billing mode or billing cycle changes.
- Only live external Xero invoices/templates should be frozen for manual migration. Pending local invoice links with no
  external Xero id should still follow the current org configuration.
- If a recurring booking already has a Xero repeating template and the org configuration drifts, freeze that existing
  template locally and mark the export as transition-required instead of silently rewriting the live Xero schedule.
- If a recurring booking is already on standard Xero invoices when the org later enables `RepeatingInvoices`, keep that
  recurring export on standard invoices until an explicit migration path exists.
- If a recurring cadence cannot be represented by Xero repeating invoices, fall back to the normal Xero invoice export
  path instead of inventing a new partial schedule shape.
- On the non-Xero/self-generated path, recurring cancellation should stop future recurring invoice workflows/sends.
  Already-generated or already-emailed invoices are not retracted.
- One-time marketplace booking cancellation must use the same accounting invoice cancellation boundary as recurring
  cancellation. Do not leave booking delete as a payment-workflow-only cleanup path.
- On the Xero repeating path, marketplace subscription cancellation must also cancel the live repeating template in
  Xero for the affected recurring booking instances instead of only freezing local export state.
- Subscription cancellation is not complete if it only releases bookings/resources while recurring payment or recurring
  invoice flows are still active.
- `CancelAtPeriodEnd` should not soft-delete the subscription immediately. It should keep the current cycle alive,
  disable renewal, and let the scheduler transition the subscription to `Cancelled` at the cycle boundary.
- Cancellation mode should be an explicit command/input from the API layer. Booking shared should not infer “cancel now”
  versus “cancel at period end” only from pre-existing entity state.
- Timeout, expiry, or failed-payment cleanup for marketplace booking and recurring booking flows must also route through
  the accounting invoice cancellation path, not only explicit user deletion.
- If Xero invoice/template cancellation cannot be completed during local cancellation, keep the local cancellation
  authoritative and mark the export as `TransitionRequired` for retry/manual follow-up.
- Recurring booking cleanup must also mark the parent recurring marketplace booking payment status as terminal
  (`Expired` or `RecordNeverCreated`) so subscription reconciliation does not restart payment workflows or recreate
  future booking instances after cancellation/expiry cleanup.
- Internal Skedular-hosted invoices need the same durable cancelled-accounting state as Xero-backed invoices. Do not
  make invoice cancellation persistence depend only on Xero links existing first.
- Xero webhook handling is raw-body -> Kafka -> processors -> shared reconciliation. Do not add a second direct mutation
  path beside the existing accounting link/payment-event flow.
- Webhook processing should wake the existing per-invoice Temporal monitor workflow immediately through
  `ITemporalService` signal-or-start behavior.
- Keep the invoice monitor workflows per invoice-bearing local entity, not per organization.
- Xero invoice creation and reconciliation must keep local invoice references in sync so `InvoiceUrl` / invoice number
  surfaces can point at the Xero-hosted invoice when available.
- Xero email-send is best-effort. Export should not fail just because Xero email delivery fails after the invoice
  already exists.

## Marketplace Subscription Auto-Renew

- `BookMarketplaceBookingSubscriptionResources` is the owning workflow for marketplace subscription maintenance.
- It runs reconciliation on a daily cadence, not on every resource-state change.
- Daily reconciliation:
    - ensures the current cycle recurring booking exists
    - repairs required resources for existing generated marketplace bookings
    - removes obsolete or duplicate future generated bookings
    - materializes missing future booking days inside the current cycle
- Auto-renew uses the subscription purchase cadence and `NextRenewalAt` to advance cycles.
- Renewal reloads the current `ProductVersion` and re-matches pricing through product-version pricing selection; if no
  compatible auto-renewable pricing remains, the subscription moves to renewal failed rather than renewing incorrectly.
- Existing recurring instances with `HasRecurringInstanceOverrides == true` are intentionally excluded from automatic
  removal/repair.
- Known gap: `MarketplaceBookingService.AdjustRequiredResourcesAsync(...)` is still best-effort and does not yet assert
  that the reassigned resource set fully satisfies the required resource count before persisting.

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

## GraphQL Regeneration

- If booking-side work depends on backend GraphQL schema changes, regenerate schemas via
  `scripts/generate-graphql.sh`.
- Do not run direct `dotnet run -- schema export ...` commands against individual APIs as a substitute for the repo
  script.

## Workflow ID Rule

- Booking workflow IDs belong in `booking/shared/Booking.Shared/Services/WorkflowIdService.cs`.
- Do not inline Temporal workflow ID prefixes or string interpolation in booking services, outbox services, or tests.
- If a workflow ID shape changes, update the workflow ID service and its unit tests instead of patching call sites one
  by one.

## Workflow ID Test Shape

- Keep booking workflow ID unit tests under `Booking.Shared.UnitTests/Services/WorkflowIdServiceTests`.
- Use one test class/file per workflow ID method, not one monolithic workflow ID test file.
- In booking unit tests, keep frozen/injected constructor dependencies before `sut`, and keep random inputs after `sut`.

## GraphQL Choice Pattern

- If booking exposes a selectable enum-like option to GraphQL clients, follow the standard pattern:
    - shared model/constants + name mapping
    - GraphQL `...Details` type with `type` and `name`
    - query field returning the available choices
- Do not make UI choice lists depend only on raw enum literals or duplicated hardcoded labels.
