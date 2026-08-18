# Quickstart: Credit-Based Booking Entitlements

## Backend

1. Apply the single generated Booking EF migration set.
2. Build the affected .NET solution/projects.
3. Run Booking shared/API unit tests.
4. Run Booking integration tests for persistence, GraphQL, Stripe webhook correlation, bank-transfer confirmation, Xero projection, renewal, concurrency, and booking modification/cancellation.
5. Regenerate GraphQL output with `scripts/generate-graphql.sh` and verify generated files are consistent.

## End-to-end scenarios

### Purchase without reservation

1. Configure an entitlement pricing with token quantity, validity, restrictions, refund policy, supported payment method, and auto-renew choice.
2. Start purchase from customer UI.
3. Verify pending purchase/payment action exists and no booking, schedule, resource allocation, reservation, or quota usage exists.
4. Confirm Stripe payment or manually confirm bank transfer.
5. Verify exactly one entitlement cycle and grant.

### Renew

1. Use an auto-renewing entitlement and advance to its cycle boundary.
2. Verify current pricing is re-matched and payment begins using the existing renewal path.
3. Confirm payment and verify exactly one new cycle with current quantity/validity/restrictions.
4. Repeat with failed/pending payment and verify the old cycle expires and no new tokens are granted.
5. Remove compatible token auto-renew pricing and verify renewal fails without reservation fallback.

### Customer and operator booking lifecycle

1. As customer, choose an eligible date/time/resource and create a booking using a token.
2. Verify one token consumption and atomic resource allocation.
3. Modify date/time/resource as customer and as authorized Spaces/Host owner/admin; verify one consumed token remains linked.
4. Cancel within and outside restoration policy; verify release or forfeiture and ledger audit.
5. Attempt cross-organization/unauthorized operator actions and verify denial with no state change.

## Web and documentation

Run each affected app’s unit/lint/build checks, regenerate Relay artifacts from source GraphQL, verify equivalent Spaces/Host customer/operator flows, and validate public help pages for purchase, renewal, booking, cancellation, and refund behavior.

Expected result: token purchase never creates a booking; later customer/operator booking uses and audits tokens exactly once; renewal/payment and reservation-based behavior remain unchanged.

## Current implementation validation

- The clarified implementation has one new `AddEntitlementPurchase` migration set; no duplicate purchase migration was added.
- `MarketplaceBookingService` rejects entitlement pricing without an existing entitlement reference before checkout, availability, or resource allocation.
- `EntitlementPurchaseService.CreatePendingAsync` persists a pending purchase and does not create a booking, schedule, resource allocation, reservation, or quota record.
- `git diff --check` and the focused entitlement purchase, grant, booking, and eligibility tests must pass before these invariants are considered complete.
