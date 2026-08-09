# Quickstart Validation: Modify Marketplace Bookings

## Prerequisites

- Run from the repository root on branch `040-modify-marketplace-bookings`.
- Have local Booking API/jobs dependencies and the web workspace available according to repository development guidance.
- Use a confirmed or no-payment-required future marketplace booking. For subscription coverage, use a Spaces subscription with a future occurrence in its current cycle.

## Contract and generation

1. Change the Booking GraphQL source and domain models, not exported/composed schema files or generated Relay files.
2. Run `scripts/generate-graphql.sh`.
3. Regenerate Relay artifacts in each client that consumes the new operation: `webapp`, `webapp-spaces`, and `webapp-host`.
4. Verify the generated composed schema exposes `modifyMarketplaceBooking` and each client operation typechecks.

## Implementation surface

- The Booking shared service owns the atomic modification command, resource claim, audit records, and subscription-occurrence override.
- The Booking API owns the GraphQL input, mutation, and read models; update its source and run `scripts/generate-graphql.sh` rather than editing exported schemas.
- Customer, Scheduler Spaces, and Scheduler Host each consume their own Relay operation. Regenerate the artifacts from the relevant app package after the GraphQL contract changes.
- Run focused .NET unit/integration tests and the three affected web test suites before handoff.

## Core validation scenarios

1. **Customer date/resource change (Spaces)**: From customer booking details, choose an eligible date/time and a valid replacement resource set. Confirm the same booking id has the new schedule/resources, commercial fields are unchanged, history records the customer action, and no notification delivery is required.
2. **Operator change and notification**: In Spaces operator booking details, change a customer's booking with a reason. Confirm the reason/audit record, customer notification delivery, and customer hub/details update. Simulate delivery failure and verify the recoverable in-app record remains.
3. **Host behavior**: From Host operator and customer booking details, change date/time. Verify no resource picker appears and the whole-place assignment remains the only fulfillment option.
4. **Subscription occurrence**: Move a confirmed Spaces subscription occurrence within the current cycle and optionally select eligible resources. Run/replay reconciliation and verify the occurrence persists as an override, no duplicate original-date booking appears, later occurrences and parent preferences are unchanged, and next-cycle planning does not inherit the one-off resources.
5. **Negative cases**: Verify rejection and no persisted change for unconfirmed payment, started booking, outside-cycle occurrence, missing operator reason, over-limit/ineligible/unavailable resources, unauthorized actor, and stale version/concurrent command.
6. **Financial/workflow regression**: Confirm a successful modification creates no payment, invoice, refund, cancellation, or deletion workflow transition; race a change with cancellation/expiry and verify cancellation remains authoritative.

## Automated checks

Run focused Booking shared/API unit suites, the smallest repository/GraphQL/Temporal integration tests for the new persistence and outbox boundaries, and Vitest/React Testing Library tests for all three application flows. Run `git diff --check` and inspect generated artifacts before handoff.

## Implementation Status

**Completed Tasks**:

- ✅ T001: Quickstart documentation recorded
- ✅ T002: Feature-specific test fixture/builders for confirmed marketplace bookings
- ✅ T003: Feature-specific test fixture/builders for Marketplace API command tests
- ✅ T004-T016: Foundational implementation (command, persistence, notification, GraphQL, unit tests)
- ✅ T017: Focused persistence/serializable conflict/GraphQL wiring integration coverage
- ✅ T018-T024: User Story 1 (Reschedule a Marketplace Booking) - customer-facing flow
- ⏸️ T025: Durable notification render coverage remains; dispatch coverage requires alignment with the current email and persistence contracts
- ✅ T027-T031: User Story 2 (Modify a Booking for a Customer) - operator flows
- ✅ T032-T036: User Story 3 (Choose Different Resources) - resource selection
- ✅ T037: GraphQL and Relay artifacts regenerated
- ✅ T038: Unit tests for cycle boundaries, override persistence, next-cycle preference isolation
- ✅ T039: Focused Temporal/repository integration coverage for daily reconciliation
- ✅ T040-T041: User Story 4 (Subscription occurrence modification) - core logic
- ✅ T044-T046: Documentation updates for shared, Spaces, and Host
- ✅ T047: Regression tests for workflow transitions (no payment/invoice/refund/cancellation/deletion)
- ✅ T050: Graphify update completed

**Deferred Tasks** (require additional work):

- ⏸️ T026: Spaces and Host operator UI tests (requires proper Relay test setup)
- ⏸️ T042: Customer and Spaces subscription occurrence entry-state/history coverage (requires UI design)
- ⏸️ T043: Relay artifacts for subscription occurrence query changes (depends on T042)

**Next Steps for Manual Validation**:

1. Deploy the Booking API and jobs services with the new modification command
2. Test customer date/time changes from booking details
3. Test operator modifications with required reason and notification delivery
4. Verify Host date/time-only behavior (no resource picker)
5. Test subscription occurrence modification and reconciliation behavior
6. Validate negative cases (payment status, stale version, unauthorized actors, etc.)

## Documentation check

Verify published behavior and terminology in:

- `src/web/apps/public-web/src/content/docs/shared/core-concepts/bookings.md`
- `src/web/apps/public-web/src/content/docs/shared/core-concepts/resources.md`
- `src/web/apps/public-web/src/content/docs/shared/core-concepts/availability.md`
- `src/web/apps/public-web/src/content/docs/shared/marketplace/products.md`
- `src/web/apps/public-web/src/content/docs/shared/marketplace/subscriptions.md`
- `src/web/apps/public-web/src/content/docs/spaces/bookings/bookings.md`
- `src/web/apps/public-web/src/content/docs/spaces/bookings/subscriptions.md`
- `src/web/apps/public-web/src/content/docs/host/bookings/bookings-and-renters.md`
Validation update: `scripts/generate-graphql.sh` completed; `dotnet build src/booking/apis/Booking.Api/Booking.Api.csproj --no-restore` passed; Booking shared unit tests passed (418 passed, 1 skipped); `git diff --check` passed. API test execution requires an environment permitting local test-host socket binding.
