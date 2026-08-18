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

## Xero Unit-Test Boundary

- Xero unit tests that belong here are the deterministic shared-layer seams:
    - webhook payload routing
    - Temporal signal-or-start behavior
    - billing-period and invoice-export decision logic
- Do not try to prove real Xero API delivery or end-to-end webhook ingress here. Those are system or manual integration
  concerns.

## Unit-test construction

- Prefer `[Theory]` plus `AutoFakeItEasyData`.
- Inject constructor dependencies, then `sut`, then generated scenario inputs through method parameters.
- Avoid in-test `A.Fake<T>()` when auto-data can provide the dependency; configure only required calls.
- Required collaborators are non-null by contract. Do not pass null loggers, transaction builders, repository factories,
  mappers, or services, or weaken production code with `?.` to accommodate tests.
