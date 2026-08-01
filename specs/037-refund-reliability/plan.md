# Implementation Plan: End-to-End Refund Reliability

**Branch**: `037-refund-reliability` | **Date**: 2026-07-25 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/037-refund-reliability/spec.md`

## Summary

### Corrective implementation prerequisites

Before price-changing marketplace edits or partial-booking outcomes can be released, Booking must provide one server-authoritative contract:

- `MarketplaceBookingPriceChangePatch` is the only accepted input for price-affecting edits; clients never submit a proposed total.
- `MarketplaceRefundOwnershipService` resolves the one billed owner for one-time, recurring-window, and subscription-window refunds.
- `MarketplacePartialBookingResolutionService` persists the decision, cancellation/refund record, and audit event in one transaction, then dispatches provider automation after commit through the durable outbox.
- Acceptance refunds the persisted allocated amount for unavailable occurrences; decline and expiry cancel created occurrences and refund the full billed owner.
- Every owner scope must have an integration test covering acceptance, decline, expiry, replay, and concurrent resolution.

Skedular already has a partial refund implementation (`MarketplaceRefund` entity, `MarketplaceRefundService`, `MarketplaceRefundAutomationService`, `StripeHostRefundService`, `XeroRefundService`, GraphQL queries/mutations, Stripe webhook handling). The current implementation uses the canonical ten-state model, durable idempotency/allocation records, provider reconciliation, bank-transfer workflow, and web operations surfaces described below.

This plan covers: (1) auditing and documenting the current state, (2) extending the existing booking domain with the additional states, fields, services, and workflows required, (3) updating the webapp and webapp-spaces UIs, and (4) updating public documentation.

## Technical Context

**Language/Version**: C# .NET 10 (backend — `Booking.Shared`, `Booking.Api`, `Booking.Processors`); TypeScript 6.0.3 / React 19.2.6 / Next.js 16.2.6 App Router (frontend — `webapp`, `webapp-spaces`); Astro static site (`public-web`)
**Primary Dependencies**: HotChocolate (GraphQL), Entity Framework Core, Stripe.net (`StripeHostRefundClient`), Xero SDK (`XeroRefundService`), Relay 21 + `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, Temporal workflows for refund processing, provider retry, and partial-booking expiry
**Storage**: PostgreSQL via EF Core in `Booking.Shared.Database` — `MarketplaceRefund`, `MarketplaceRefundEvent`, `MarketplaceRefundPaymentAllocation`, `MarketplaceExternalRefundReconciliation`, and durable notification-delivery records require migration support for the expanded state machine, payment-level caps, provider-path persistence, reconciliation leases, unmatched-provider tracking, and notification idempotency.
**Testing**: xUnit + AutoFixture + FakeItEasy (backend unit); integration tests in `Booking.Domain.IntegrationTests` using repository-layer assertions; Vitest + React Testing Library (frontend)
**Target Platform**: Cloud-hosted ASP.NET Core API + Next.js SSR web apps
**Project Type**: Full-stack web service (backend API + frontend web apps + static docs site)
**Constraints**: All monetary arithmetic must use `decimal` (never `float`/`double`); cancellation deadlines use the purchase-time location/organization timezone; no raw EF in integration tests; Relay artifacts must not be hand-edited; generated GraphQL schema must be regenerated via `scripts/generate-graphql.sh`
**Scale/Scope**: All active Spaces and Host organizations; expected refund volume is low relative to booking volume (< 5%)

## Stripe Charge and Refund Matrix

The implementation must resolve the original Stripe charge context before refund submission:

| Context | Account/charge context | Transfer behavior | Refund path |
|---|---|---|---|
| Host booking | Use the account and charge type recorded on the booking payment | Preserve the original transfer semantics | Refund the original payment through its owning Stripe account |
| Spaces booking | Use the connected-account/destination-charge context recorded on the booking payment | Prefer transfer reversal when payout has been disbursed | Reverse the transfer; fall back to a platform-funded refund when reversal is unavailable |
| Subscription | Use the billed recurring booking and its payment source for the active cycle | Follow that cycle's original transfer context | Refund only the resolved billed owner |
| Post-payout | Use the original charge metadata and transfer identifiers | Attempt transfer reversal first | Fall back to platform funds; route ambiguous outcomes to reconciliation |

The selected refund path and provider identifiers must be persisted before the operation is considered complete.

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — Yes. Changes touch `Booking.Api/schema.graphqls` (refund mutations for approval/rejection, retry, bank-transfer recording, cancellation, partial refunds, and reconciliation resolution; plus query fields for idempotency key and review states). `scripts/generate-graphql.sh` must be run after any schema change. Relay artifacts in `webapp` and `webapp-spaces` must be regenerated. OpenAPI client in `src/web/apps/webapp/scripts/generate.sh` must be rerun if any REST endpoint changes.
- [x] **II. Domain Boundaries** — Refund work stays within the booking domain (`Booking.Shared`, `Booking.Api`, `Booking.Processors`). Organization and customer context is accessed via existing replicated entities within the booking DB, consistent with established patterns. No direct cross-domain DB access required.
- [x] **III. Testing** — Unit tests are required for all new/changed services. Integration tests cover the persistence or provider/workflow boundaries where they add value: Stripe webhook/retry reconciliation, Xero credit-note projection and recovery, bank-transfer workflow, partial-booking acceptance, allocation caps/concurrency, and notification dispatch. Migration execution against the integration-test database is explicitly out of scope because the feature starts with a new empty refund schema. All integration-test assertions must go through repository-layer methods, not raw `DbContext`. Frontend: Vitest + React Testing Library for new refund status components and admin queue.
- [x] **IV. Frontend** — Yes. `webapp` (customer cancellation flow, refund preview, refund status view) and `webapp-spaces` (admin refund queue, partial refund, bank-transfer workflow, reconciliation view) are both affected. Relay fragments collocated with components. No hand-editing generated Relay artifacts. Typography wrappers from `@skedular/ui`. American spelling in all user-facing copy. `webapp-teams` is not affected.
- [x] **V. Pattern Consistency** — Refund transitions are centralized in `MarketplaceRefundTransitionService`; payout matching/state changes are isolated in `StripePayoutReconciliationService`; provider work remains behind the existing automation services. These are deliberate domain-owned services so audit, notification, GraphQL, and retry behavior share one path.
- [x] **VI. Logging** — Yes. `LOG-001` through `LOG-005` require structured logs for transitions, provider interactions, webhook receipt, calculations, reconciliation, and operational alerts. Logging tests must exercise success, failure, retry, and recovery paths. No PII or payment credentials in logs.

## Project Structure

### Documentation (this feature)

```text
specs/037-refund-reliability/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   ├── graphql-mutations.md
│   └── webhook-events.md
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created here)
```

### Source Code (repository root)

```text
src/booking/
├── shared/
│   ├── Booking.Shared/
│   │   ├── Database/Entities/
│   │   │   ├── MarketplaceRefund.cs          # extend: IdempotencyKey, ApprovedBy, new status fields
│   │   │   ├── MarketplaceRefundEvent.cs     # extend: ApprovedByCustomerId
│   │   │   └── MarketplaceRefundPaymentAllocation.cs # new: source-payment allocation and caps
│   │   ├── Models/
│   │   │   ├── MarketplaceRefundStatusConstants.cs    # add: UnderReview, Approved, Rejected, ReconciliationRequired, Cancelled
│   │   │   └── MarketplaceRefundEventTypeConstants.cs # extend to match new states
│   │   ├── Services/
│   │   │   ├── MarketplaceRefundService.cs            # extend: idempotency guard, new refund triggers
│   │   │   ├── MarketplaceRefundAutomationService.cs  # extend: UnderReview routing, retry path
│   │   │   ├── MarketplaceRefundPolicyService.cs      # extend: operator-override, subscription pro-rate
│   │   │   ├── StripeHostRefundService.cs             # extend: idempotency key, ReconciliationRequired
│   │   │   └── XeroRefundService.cs                  # extend: distinguish credit note vs money movement
│   │   └── Repositories/
│   │       └── MarketplaceRefundRepository.cs        # extend: GetPendingBeyondThresholdAsync, concurrency guard
│   └── Booking.Shared.UnitTests/
│       └── Services/                                  # new test classes per new behaviour
├── apis/
│   ├── Booking.Api/
│   │   ├── schema.graphqls                            # new mutations, new query fields
│   │   ├── Services/
│   │   │   ├── MarketplaceRefundAdminService.cs       # extend: approve, reject, retry, bank-transfer workflow
│   │   │   └── MarketplaceRefundReadService.cs        # extend: return new state fields
│   │   └── Controllers/
│   │       └── BookingStripeWebhookController.cs      # verify signature validation is complete
├── processors/
│   └── Booking.Processors/
│       └── Subscribers/
│           └── BookingInternalSubscriber.cs           # extend: ReconciliationRequired handling
└── domain/
    └── Booking.Domain.IntegrationTests/               # new integration test scenarios

src/web/
├── apps/
│   ├── webapp/
│   │   └── src/
│   │       ├── components/refund/                     # new: RefundStatusBadge, RefundPreviewPanel
│   │       └── __generated__/                         # regenerated Relay artifacts
│   └── webapp-spaces/
│       └── src/
│           ├── components/admin/refund/               # new: RefundQueue, BankTransferWorkflow, PartialRefundForm
│           └── __generated__/                         # regenerated Relay artifacts
│   └── public-web/
│       └── src/content/docs/
│           ├── spaces/bookings/refunds.md             # update: new states, bank transfer workflow
│           └── host/bookings/payments-cancellations-and-refunds.md  # new Host refund docs

api-definitions/graphql/skedular/v1/schema.graphql    # regenerated — do not hand-edit
```

**Structure Decision**: Extends the existing booking domain structure. All new code follows the `Booking.Shared` → `Booking.Api` layering. No new projects or service boundaries introduced.

## Complexity Tracking

_No constitution violations requiring justification._
