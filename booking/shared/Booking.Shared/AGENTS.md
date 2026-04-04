# Booking.Shared Agent Notes

This file is for agents editing `booking/shared/Booking.Shared/`.

## Recurring In-Arrears Rules

### First invoice behavior

For a recurring in-arrears subscription, the first invoice is not the full purchase cadence amount.

Current expected behavior:

- If purchase cadence is larger than the organization billing cycle, split into billing-cycle installments.
- The initial recurring split is stepped from the subscription start date.
- The installment amounts are equal-split by installment count, with rounding remainder applied to the last installment.

Example:

- organization billing cycle: monthly
- purchase cadence: quarterly
- price: `1450`
- expected installments: `483.33`, `483.33`, `483.34`

Historical bug to avoid reintroducing:

- aligning the first split to calendar month boundaries created tiny stub periods
- example symptom: start date near month-end created a first invoice around `47` instead of the expected monthly
  installment

Relevant code:

- `Services/OrganizationArrearsChargeSegmentService.cs`
- `Services/OrganizationArrearsBillingPlannerService.cs`

## Tax Rules

Do not zero out tax for first recurring arrears invoices.

Correct behavior:

- tax source: owner organization tax configuration
- inclusion mode source: `MarketplaceBooking.ProductPricing.IsTaxInclusive`
- first-slice amount changes the amount being invoiced
- tax logic itself should remain the same as the normal marketplace logic

If you shortcut this, PDF, stored totals, and Stripe drift apart.

Relevant code:

- `Activities/BookingIntegrations.cs`
- `Services/BookingInvoiceService.cs`
- `Activities/StripeIntegrations.cs`

## Stripe Rules

For first recurring in-arrears checkout, do not send Stripe a single opaque total when tax breakdown should be visible.

Expected behavior:

- use a custom line item
- set `TaxBehavior` from `IsTaxInclusive`
- set `TaxCode`
- enable `AutomaticTax`
- send the right base amount:
    - inclusive total for tax-inclusive pricing
    - ex-tax subtotal for tax-exclusive pricing

Relevant code:

- `Activities/StripeIntegrations.cs`

## Xero Rules

- Xero does not replace Stripe for card checkout in the current design.
- When org Xero billing mode is `Enabled`, Xero is the invoice sender, host, and reconciliation source for supported
  invoiceable flows.
- If Xero is enabled but configured not to send invoices, keep the local fallback behavior instead of creating a draft
  in Xero and returning silently.
- Reconciliation should flow through the accounting link/event model and existing booking payment workflows, not around
  them.
- Use the normal accounting payment-event flow when confirming Xero-paid invoices so downstream subscribers still see
  the normal booking payment transitions.
- The booking webhook path should only publish raw payloads; async processor code then signals or starts the per-invoice
  monitor workflows.
- `MaintainAccountingInvoiceState` and `MaintainOrganizationArrearsInvoiceAccountingState` are per-invoice monitor
  workflows. Webhooks should wake them early; polling remains the safety net.
- Xero invoice line descriptions should stay aligned with the local booking invoice description rules rather than
  collapsing down to just the product name.
- Tax-inclusive vs tax-exclusive pricing must be preserved when exporting Xero invoice lines. Do not send an inclusive
  amount as an exclusive line.

## Invoice Template Rules

Current intended template behavior:

- first recurring in-arrears invoice:
    - use `RecurringInvoiceDocument`
    - preserve the marketplace invoice template structure
    - preserve bank-transfer details and due-date section behavior
- later scheduled billing-cycle invoice:
    - still uses `OrganizationArrearsInvoiceService`
    - supports multi-line grouped billing

Do not assume the later scheduled invoice should automatically reuse the first recurring invoice template.

## Scheduled Billing-Cycle Behavior

The scheduled billing-cycle invoice is grouped per customer and can include more than one line.

That means:

- one invoice can include the next slice of a subscription plus other in-arrears bookings
- template assumptions must support multi-line grouped billing

Relevant code:

- `Activities/OrganizationArrearsBillingIntegrations.cs`
- `Services/OrganizationArrearsInvoiceService.cs`

## Manual Trigger Behavior

`RunNow` is not a future-cycle simulator.

When the workflow is manually triggered:

- it only processes earned uninvoiced usage inside the current billing-period window
- if everything in the current period is already invoiced, it will do nothing
- it does not create hypothetical future earned segments

Relevant code:

- `Activities/OrganizationArrearsBillingIntegrations.cs`
- `Workflows/RunOrganizationArrearsBilling.cs`

## Repository Loading Rule

When arrears logic needs owner billing settings, loaded bookings must include:

- `MarketplaceBooking -> ProductVersion -> Product -> Organization`

This applies to one-off and recurring booking loading paths.

Relevant code:

- `Repositories/BookingRepository.cs`

## Integration Test Query Rule

- Repository methods may be added specifically to support booking integration-test assertions.
- If an integration test needs persisted-state inspection, expose that query through a repository instead of reaching
  for `BookingDbContext` or Entity Framework directly in the test project.

Relevant code:

- `Repositories/OrganizationArrearsInvoiceRepository.cs`

## Edit Checklist

If you edit recurring in-arrears logic, verify:

1. first invoice amount
2. tax amount and tax rate
3. invoice template
4. Stripe checkout breakdown
5. scheduled later billing-cycle behavior
6. unit tests for segment splitting
7. Xero export and reconciliation behavior if the change affects bank-transfer or arrears invoices
