# Booking.Shared Unit Test Agent Notes

This file covers `booking/shared/Booking.Shared.UnitTests/`.

## What Belongs Here

Use these tests for deterministic logic in the shared booking layer, especially:

- arrears segment splitting
- billing-period math
- installment calculation
- initial recurring draft selection
- workflow period calculation

Good examples already exist around:

- `OrganizationArrearsChargeSegmentService`
- `OrganizationArrearsBillingPlannerService`
- `OrganizationArrearsBillingIntegrations`

## What Does Not Belong Here

Do not try to prove end-to-end workflow behavior here.

Examples that should move to system tests instead:

- create real data
- call real booking API
- wait for Temporal side effects
- assert persisted arrears invoice records across the running stack

## Billing Regression Priorities

If you change recurring in-arrears logic, the minimum useful unit coverage should include:

1. billing-cycle splitting shape
2. equal installment amounts for initial recurring draft
3. month-end / boundary regressions
4. earned-at selection relative to the billing period
