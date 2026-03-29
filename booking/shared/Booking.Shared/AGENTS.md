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
- example symptom: start date near month-end created a first invoice around `47` instead of the expected monthly installment

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

## Edit Checklist

If you edit recurring in-arrears logic, verify:

1. first invoice amount
2. tax amount and tax rate
3. invoice template
4. Stripe checkout breakdown
5. scheduled later billing-cycle behavior
6. unit tests for segment splitting
