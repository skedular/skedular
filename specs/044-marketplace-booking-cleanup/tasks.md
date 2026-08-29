# Tasks: Reliable Marketplace Booking Cleanup

**Input**: Design documents from `/specs/044-marketplace-booking-cleanup/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/cleanup-reliability.md, quickstart.md

## Phase 1: Setup

- [X] T001 Confirm affected workflow, activity, repository, job, API, and web paths against `specs/044-marketplace-booking-cleanup/plan.md` and record any missing abstraction in `specs/044-marketplace-booking-cleanup/research.md`.
- [X] T002 [P] Inventory existing allocation/release callers and terminal payment/invoice failure transitions in `src/booking/shared/Booking.Shared/` and add the trace matrix to `specs/044-marketplace-booking-cleanup/quickstart.md`.
- [X] T003 [P] Inventory current GraphQL failure/status fields and Relay consumers in `src/booking/apis/Booking.Api/schema.graphqls` and `src/web/apps/`.

## Phase 2: Foundational

- [X] T004 Define local cleanup and accounting-cleanup state constants and explicit mappings in `src/booking/shared/Booking.Shared/Models/MarketplaceBookingCleanupConstants.cs`.
- [X] T005 Define repository/service models for cleanup identity, effective payment owner or durable failure source, idempotency, and transition state in `src/booking/shared/Booking.Shared/Models/MarketplaceBookingCleanup.cs`.
- [X] T006 Add durable cleanup/accounting transition persistence and EF configuration in `src/booking/shared/Booking.Shared/Database/Entities/`, `src/booking/shared/Booking.Shared/Database/Configurations/`, and the owning `DbContext`; generate the migration under `src/booking/shared/Booking.Shared/Database/Migrations/`.
- [X] T007 [P] Add repository methods for cleanup lookup, eligibility, idempotency, leases, and reconciliation candidates in `src/booking/shared/Booking.Shared/Repositories/`.
- [X] T008 [P] Add the shared Temporal cleanup workflow/activity contract and reconciliation enqueue path behind repository/unit-of-work boundaries in `src/booking/shared/Booking.Shared/Workflows/MarketplaceBookingCleanup.cs`, `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingCleanupIntegrations.cs`, and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingCleanupReconciliationService.cs`.
- [X] T009 Add structured logging event/property conventions for cleanup transitions, provider follow-up, retries, and reconciliation in affected activities and reconciliation services.
- [X] T010 Add foundational unit tests for cleanup state mapping and invalid persistence values in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingCleanupServiceTests/`.

## Phase 3: User Story 1 - Release resources after payment failure (Priority: P1) 🎯 MVP

**Goal**: Every one-time and recurring terminal payment path releases local resources transactionally and safely on replay.

**Independent Test**: Drive card and bank-transfer one-time/recurring flows to terminal failure and verify local slots, allocations, and generated instances converge to released without provider availability.

- [X] T011 [P] [US1] Cover one-time card failure, expiry, rejection, and null Stripe setup cleanup through shared release/activity unit tests in `src/booking/shared/Booking.Shared.UnitTests/Activities/`.
- [X] T012 [P] [US1] Cover recurring card failure, expiry, rejection, and null Stripe setup cleanup through recurring release/activity unit tests in `src/booking/shared/Booking.Shared.UnitTests/Activities/`.
- [X] T013 [P] [US1] Cover one-time and recurring bank-transfer failure/expiry cleanup through shared release/activity unit tests in `src/booking/shared/Booking.Shared.UnitTests/Activities/`.
- [X] T014 [P] [US1] Add activity unit tests with mocked repositories/transactions for transactional one-time release and idempotent replay in `src/booking/shared/Booking.Shared.UnitTests/Activities/BookingIntegrationsTests/`.
- [X] T015 [P] [US1] Add activity unit tests with mocked repositories/transactions for recurring generated-instance deletion, resource release, and replay in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`.
- [X] T016 Refactor `src/booking/shared/Booking.Shared/Activities/BookingIntegrations.cs` so one-time release commits local slot/allocation changes before any external cleanup and persists the cleanup outcome.
- [X] T017 Refactor `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs` so recurring generated bookings/resources are released locally before Xero/accounting cancellation.
- [X] T018 Replace silent null Stripe responses with explicit failure and cleanup signals in `src/booking/shared/Booking.Shared/Workflows/PayBookingViaCard.cs` and `PayRecurringBookingViaCard.cs`.
- [X] T019 Add at most five delayed/exponential-backoff retry attempts to every changed local resource-release activity invocation in the payment and marketplace subscription workflows; on exhaustion enqueue immediate idempotent reconciliation while retaining the scheduled safety-net reconciler.
- [X] T020 Add initial-arrears permanent-failure cleanup to `src/booking/shared/Booking.Shared/Workflows/GenerateInitialArrearsBookingInvoice.cs` and `GenerateInitialArrearsRecurringBookingInvoice.cs`.
- [X] T021 Add Temporal workflow/activity unit coverage for fallback reconciliation enqueue and idempotent replay in `src/booking/shared/Booking.Shared.UnitTests/Workflows/` and `Activities/MarketplaceBookingCleanupIntegrationsTests/`; route-specific payment workflow cases remain covered by the shared release/activity tests.

## Phase 4: User Story 2 - Keep local cancellation independent of providers (Priority: P1)

**Goal**: Xero, Stripe, invoice, notification, event, and worker failures leave local release authoritative and provider work recoverable.

**Independent Test**: Inject each provider failure after booking creation and verify local release commits while accounting/notification state remains durable and retryable.

- [X] T022 [P] [US2] Add unit tests proving Xero/accounting cancellation failure does not fail local release in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsShould.cs`.
- [X] T023 [P] [US2] Add unit tests for provider transition states and retry/replay in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingCleanupServiceTests/AccountingCleanupShould.cs`.
- [X] T024 [P] [US2] Add unit tests proving failure notification/event publication cannot claim release before commit in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingFailureServiceTests/`.
- [X] T025 Implement independent accounting cleanup dispatch and durable `Pending`/`TransitionRequired` handling in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingAccountingCleanupService.cs` and the daily reconciliation host without consuming the local release retry budget.
- [X] T026 Ensure failure finalization and notification/outbox publication use post-commit release state in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingFailureService.cs`, `MarketplaceBookingFailureNotificationService.cs`, and related activities.
- [X] T027 Add activity/service unit coverage proving provider outage leaves local release authoritative and records `Pending` or `TransitionRequired` accounting state in `src/booking/shared/Booking.Shared.UnitTests/Activities/` and `Services/MarketplaceBookingCleanupServiceTests/`.
- [X] T028 Add structured warning/error logs and correlation fields for provider failure, retry exhaustion, and recovery in affected workflows, activities, and services.

## Phase 5: User Story 3 - Show truthful recovery state (Priority: P2)

**Goal**: Customers and operators see release and accounting state that reflects committed local state.

**Independent Test**: Observe status before, during, and after cleanup/provider failure and verify the UI labels each state accurately without reload.

- [X] T029 [P] [US3] Add API service unit tests for mapping cleanup states and stable IDs in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingFailureReadServiceTests/`.
- [X] T030 Update source GraphQL fields/payloads for cleanup and accounting status in `src/booking/apis/Booking.Api/schema.graphqls` and map shared models in `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`.
- [X] T031 Update customer/operator failure status components and mutation fragments under `src/web/apps/webapp/src/` and `src/web/apps/webapp-spaces/src/` to distinguish failure recorded, release pending, resources released, and accounting pending.
- [X] T032 Update Relay mutation store/connection handling in affected `src/web/apps/webapp/src/` and `src/web/apps/webapp-spaces/src/` operations; no mutation-success browser reload remains for this state.
- [X] T033 Regenerate GraphQL and Relay artifacts with `scripts/generate-graphql.sh` and `pnpm --dir src/web relay`; do not hand-edit generated files.
- [X] T034 Add Vitest/React Testing Library coverage for pre-commit and post-commit status rendering and verify no-reload mutation updates under `src/web/apps/webapp*/src/`.
- [X] T035 Review and update customer/operator documentation under `src/web/apps/public-web/src/content/docs/` for the new status semantics.

## Phase 6: User Story 4 - Reconcile existing orphaned allocations (Priority: P2)

**Goal**: Historical and retry-exhausted terminal bookings are automatically found and safely re-enqueued, including subscription-linked ownership.

**Independent Test**: Seed terminal bookings with allocations, run reconciliation, and verify automatic cleanup enqueue, lease safety, effective owner resolution, and no subscription recreation.

- [X] T036 [P] [US4] Add repository integration tests for terminal effective-owner eligibility, durable failure records with no payment record, confirmed-entitlement exclusion, and reconciliation leases in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplaceBookingCleanupReconciliationShould.cs`.
- [X] T037 [P] [US4] Add reconciliation service unit tests for automatic enqueue and lease-safe duplicate runs in `src/booking/jobs/Booking.Jobs.UnitTests/Services/MarketplaceBookingCleanupReconciliationServiceTests/`.
- [X] T038 Extend `src/booking/shared/Booking.Shared/Repositories/MarketplaceBookingFailureRepository.cs` and related booking repositories to query rejected/expired effective payments and durable terminal failures with remaining allocations, including cases without a payment record.
- [X] T039 Extend `src/booking/jobs/Booking.Jobs/Services/MarketplaceRefundReconciliationHostedService.cs` with a cleanup reconciliation service that leases candidates, records attempts, and automatically enqueues idempotent cleanup.
- [X] T040 Prevent canceled/terminal subscriptions from renewal or resource rematerialization in `src/booking/shared/Booking.Shared/Workflows/BookMarketplaceBookingSubscriptionResources.cs` and related subscription/payment workflows.
- [X] T041 Add workflow/service unit coverage for orphan repair enqueue and concurrent lease skips, with subscription cancellation and no-recreation guards covered by existing subscription activity unit tests in `src/booking/shared/Booking.Shared.UnitTests/`.

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T043 [P] Audit changed C# signatures for required-parameter-before-`CancellationToken` and nullability compliance across `src/booking/`.
- [X] T044 [P] Audit repository and transport code for prohibited direct EF/repository-factory access in workflows, activities, API resolvers, and integration tests under `src/booking/`.
- [X] T045 [P] Run `git diff --check`, affected backend tests, web tests/type checks, and the scenarios in `specs/044-marketplace-booking-cleanup/quickstart.md`.
- [X] T046 Run `graphify update .` after implementation changes and review the updated relationships for cleanup/reconciliation coverage.
- [X] T047 Finalize implementation notes and unresolved operational decisions in `specs/044-marketplace-booking-cleanup/research.md` and verify [spec.md](spec.md), [data-model.md](data-model.md), and [contracts/cleanup-reliability.md](contracts/cleanup-reliability.md) remain aligned.

## Dependencies & Execution Order

- Setup T001-T003 precedes foundational work.
- Foundational T004-T010 blocks all user stories.
- US1 T011-T021 is the MVP and should land first.
- US2 T022-T028 depends on the local cleanup contract from US1 but can parallelize after T016-T019.
- US3 T029-T035 depends on stable cleanup status fields from US1/US2.
- US4 T036-T042 depends on cleanup idempotency and effective-owner resolution from the foundational/US1 work.
- Polish T043-T047 follows the selected stories.

## Parallel Opportunities

- T002-T003, T007-T009, and T011-T015 can run in parallel.
- After the foundational checkpoint, US1 backend tests and US2 provider tests can proceed in parallel when shared cleanup contracts are stable.
- US3 UI work can proceed in parallel with US4 repository/service work after status and eligibility models are agreed.
- Documentation, signature audits, and generated-artifact verification are parallelizable near completion.

## Implementation Strategy

1. Deliver US1 as the MVP: local transactional release, explicit Stripe failures, bounded retries, arrears cleanup, and tests.
2. Add US2 provider independence and durable transition recovery.
3. Add US3 truthful API/UI state and regenerated contracts.
4. Add US4 automatic reconciliation and subscription recreation protection.
5. Run polish, quickstart validation, and graph update before implementation handoff.
