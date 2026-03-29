# Booking Domain Agent Notes

This file is the entry point for AI agents working anywhere under `booking/`.

## Mental Model

- `Booking` is the one-off booking model.
- `RecurringBooking` is the subscription-style booking model.
- `MarketplaceBooking` carries pricing, payment, invoice, tax, and checkout state.
- `ProductVersion -> Product -> Organization` is the owner chain for pricing context, tax settings, and organization billing cycle.
- `InArrears` means usage is earned first, then invoiced later.

## Where To Read Next

- For billing, arrears, invoice, and Stripe rules:
  - `booking/shared/AGENTS.md`
  - `booking/shared/Booking.Shared/AGENTS.md`
- For API trigger and workflow-entry behavior:
  - `booking/apis/AGENTS.md`
- For booking-domain test placement:
  - `booking/domain/AGENTS.md`

## Important Domain Boundaries

- `booking/apis/`
  - HTTP/GraphQL/grpc entry points and workflow triggers
- `booking/shared/`
  - most billing logic, repositories, workflows, invoice generation, Stripe checkout
- `booking/domain/`
  - domain-local Aspire app host and domain integration tests
- `booking/processors/`
  - event/subscriber driven processing
- `booking/jobs/`
  - scheduled/background job hosting

## Rules That Matter Across The Whole Booking Domain

- Do not simplify billing logic in one layer without checking PDF invoice generation, stored totals, and Stripe checkout.
- Do not assume first recurring arrears invoicing and later scheduled arrears billing use the same code path.
- When arrears logic returns nothing unexpectedly, inspect loaded entity graph and owner/customer/currency availability before assuming billing math is wrong.
- Prefer adding tests when changing billing-cycle behavior.

## Source Map

- API trigger:
  - `booking/apis/Booking.Api/Controllers/BookingController.cs`
  - `booking/apis/Booking.Api/Services/WorkaroundService.cs`
- Shared billing logic:
  - `booking/shared/Booking.Shared/Services/OrganizationArrearsChargeSegmentService.cs`
  - `booking/shared/Booking.Shared/Services/OrganizationArrearsBillingPlannerService.cs`
  - `booking/shared/Booking.Shared/Activities/OrganizationArrearsBillingIntegrations.cs`
  - `booking/shared/Booking.Shared/Services/BookingInvoiceService.cs`
  - `booking/shared/Booking.Shared/Activities/StripeIntegrations.cs`

## Working Style For Agents In This Domain

- Preserve end-to-end consistency across DB state, invoice output, and checkout behavior.
- Stay close to the existing domain patterns before introducing new abstractions.
- If a change affects recurring in-arrears billing, read the `booking/shared` agent files before editing.
