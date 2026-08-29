# Tasks: Backend-Owned Marketplace Purchase Lifecycle History

**Input**: Design documents from `/specs/045-marketplace-purchase-history/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, and `quickstart.md` are complete.
**Implementation gate**: The complete documentation set was approved before implementation; all tasks below are complete.

## Phase 1: Setup

- [x] T001 Review `specs/045-marketplace-purchase-history/spec.md`, `plan.md`, `research.md`, `data-model.md`, `contracts/`, and `quickstart.md` for approval and consistent terminology.
- [x] T002 [P] Inventory current history entity, repository, service, GraphQL, lifecycle, and frontend call sites under `src/booking/` and `src/web/apps/webapp/`.
- [x] T003 [P] Identify the owning Booking EF migration project and migration conventions under `src/booking/shared/Booking.Shared/Database/`.

## Phase 2: Foundational (Blocking Prerequisites)

- [x] T004 Add event type constants, source mappings, and explicit unknown-value behavior in `src/booking/shared/Booking.Shared/Models/MarketplacePurchaseHistory.cs`.
- [x] T005 Add immutable event identity/configuration with source identity, idempotency uniqueness, ordering indexes, and dedicated typed event-value columns (not a serialized payload) in `src/booking/shared/Booking.Shared/Database/Entities/MarketplacePurchaseHistory.cs`.
- [x] T006 Add shared event/detail models in `src/booking/shared/Booking.Shared/Models/MarketplacePurchaseHistory.cs` without GraphQL or database types in service contracts.
- [x] T007 Implement deterministic ordering and current-snapshot reduction in `src/booking/shared/Booking.Shared/Models/MarketplacePurchaseHistory.cs`, covering late events, ties, independent payment/refund state, and no synthetic events.
- [x] T008 [P] Add reducer and persisted-string mapping unit tests under `src/booking/shared/Booking.Shared.UnitTests/Models/MarketplacePurchaseHistoryTests/` using AutoFakeItEasyData.
- [x] T009 Create the clean-database EF migration for event storage and derived snapshot support under `src/booking/shared/Booking.Shared/Database/Migrations/`, with no legacy backfill.
- [x] T010 [P] Add structured logging names/properties and logging tests under `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplacePurchaseHistoryTests/` without secrets or sensitive data.

## Phase 3: User Story 1 — Review Subscription Lifecycle (Priority: P1) 🎯 MVP

**Goal**: Customers and authorized operators can view backend-recorded subscription events newest first.

**Independent Test**: Append subscription creation, start, renewal, cancellation, payment, and refund events; query, refresh, and deep-link the history connection; verify stable ordering and cancellation dates.

- [x] T011 [P] [US1] At final validation, add only subscription repository integration coverage not already provided by unit/contract tests for append, duplicate replay, concurrency, late ordering, and keyset paging in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistoryRepositoryShould.cs`.
- [x] T012 [P] [US1] Add subscription lifecycle unit tests for creation/start/renewal/cancellation/payment/refund write points in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplacePurchaseHistoryTests/SubscriptionLifecycleHistoryShould.cs`.
- [x] T013 [P] [US1] Add GraphQL subscription authorization/payload tests in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/MarketplacePurchaseHistoryEventsShould.cs`.
- [x] T014 [US1] Implement append/read/snapshot repository methods in `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`, returning existing event identity on idempotent replay and keeping `CancellationToken` last.
- [x] T015 [US1] Implement the shared lifecycle coordinator in `src/booking/shared/Booking.Shared/Services/MarketplacePurchaseHistoryEventService.cs` using `contracts/lifecycle-write-points.md`.
- [x] T016 [US1] Wire subscription creation/start/renewal/cancellation transitions in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs` and `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.
- [x] T017 [US1] Wire subscription payment/refund transitions in `src/booking/shared/Booking.Shared/Activities/StripeIntegrations.cs`, `src/booking/shared/Booking.Shared/Activities/OrganizationArrearsBillingIntegrations.cs`, and `src/booking/shared/Booking.Shared/Services/MarketplaceRefundTransitionService.cs`.
- [x] T018 [US1] Add authorized subscription history service/model mapping in `src/booking/apis/Booking.Api/Services/MarketplacePurchaseHistoryService.cs` and `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`.
- [x] T019 [US1] Add the subscription history connection/resolver in `src/booking/apis/Booking.Api/schema.graphqls` and `src/booking/apis/Booking.Api/GraphQL/MarketplacePurchaseHistory/`.
- [x] T020 [US1] Add source-schema contract coverage for the subscription history connection in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/MarketplacePurchaseHistoryEventsShould.cs`.

## Phase 4: User Story 2 — Review Credit Entitlement Lifecycle (Priority: P1)

**Goal**: Customers and authorized operators can view entitlement creation, consumption, expiration, payment, and refund events.

**Independent Test**: Append entitlement grant, repeated consumption, expiration, payment, and refund events; query the connection; verify quantities, empty history, and newest-first order.

- [x] T022 [P] [US2] Add entitlement lifecycle unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplacePurchaseHistoryTests/EntitlementLifecycleHistoryShould.cs`.
- [x] T023 [P] [US2] Add entitlement GraphQL contract tests in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/EntitlementHistoryShould.cs`.
- [x] T024 [US2] Wire entitlement creation/payment transitions in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementPurchaseService.cs` and `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementPurchasePaymentReconciliationService.cs`.
- [x] T025 [US2] Wire entitlement expiration/consumption transitions in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementExpiryService.cs`, `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementBookingService.cs`, and `src/booking/shared/Booking.Shared/Services/Entitlements/CreditLedgerService.cs`.
- [x] T026 [US2] Wire entitlement refund transitions in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceRefundTransitionService.cs`.
- [x] T027 [US2] Extend `src/booking/apis/Booking.Api/Services/MarketplacePurchaseHistoryService.cs` and `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs` for entitlement payloads without aggregate reconstruction.

## Phase 5: User Story 3 — Preserve Purchase List and One-Time Booking Behavior (Priority: P1)

**Goal**: The mixed purchases list uses derived snapshots, while one-time booking details remain unchanged and history-free.

**Independent Test**: Query mixed purchases, open eligible subscription/entitlement details, then open a standalone booking and verify no history tab/query and unchanged content.

- [x] T028 [P] [US3] Cover purchases-list snapshot derivation, filters, authorization, pagination, and mixed sources with unit tests; defer only repository behavior that cannot be covered there to final validation integration coverage in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistoryRepositoryShould.cs` and `src/booking/apis/Booking.Api.UnitTests/Services/MarketplacePurchaseHistoryServiceTests/GetPaginatedAsyncShould.cs`.
- [x] T029 [P] [US3] Add frontend tests for backend-only events, newest-first rendering, empty/loading/error, refresh, and deep-link behavior in `src/web/apps/webapp/src/components/marketplacePurchaseHistory/marketplace-purchase-history-section.test.tsx`.
- [x] T030 [P] [US3] Add one-time booking regression tests proving no history tab/query in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.test.tsx`.
- [x] T031 [US3] Update `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs` so the list reads the derived snapshot and standalone bookings never append lifecycle events.
- [x] T032 [US3] Add subscription and entitlement history UI using only backend event records under `src/web/apps/webapp/src/components/marketplacePurchaseHistory/` and `src/web/apps/webapp/src/components/marketplaceEntitlement/`.
- [x] T033 [US3] Update colocated Relay queries/fragments under `src/web/apps/webapp/src/queries/` and regenerate artifacts with `pnpm --dir src/web relay`; do not hand-edit outputs or use reload-based invalidation.

## Phase 6: Polish and Cross-Cutting Validation

- [x] T034 [P] Review affected frontend copy/typography and customer/operator documentation under `src/web/apps/public-web/src/content/docs/`, updating only stale behavior.
- [x] T035 [P] Regenerate the integration schema fixture under `src/booking/domain/Booking.Domain.IntegrationTests/schema.graphql` from the source contract.
- [x] T036 Run `specs/045-marketplace-purchase-history/quickstart.md` scenarios and all focused backend/frontend tests; verify migration, duplicate delivery, ordering, cancellation dates, missing history, refresh/deep-link, and one-time booking regression.
- [x] T037 Run `git diff --check` and `graphify update .`; review generated-output and contract consistency.
- [x] T038 Run `scripts/generate-graphql.sh` after all backend schema changes and verify the generated Booking, composed, and integration schema outputs.

## Integration-test policy

Behavior must be covered by unit or GraphQL contract tests whenever possible. Do not add duplicate integration tests for behavior already covered at those levels. Execute the remaining integration-test tasks only after implementation and final unit/contract validation, and add them only for persistence, concurrency, or generated-schema behavior that cannot be meaningfully verified otherwise.

## Dependencies and Execution Order

- Phase 1 precedes Phase 2; Phase 2 blocks every user story.
- US1 and US2 can proceed in parallel after Phase 2, with coordination for shared repository/service/schema files.
- US3 depends on the finalized event read contract from US1/US2; its one-time booking regression can be tested independently.
- Phase 6 depends on all selected stories.

## Parallel Execution Examples

- Foundation: T008 and T010 can run in parallel after T004–T007; T003 is independent of T002.
- US1: T011–T013 can run in parallel before T014–T019.
- US2: T022–T023 can run in parallel before T024–T027.
- US3: T028–T030 can run in parallel before T031–T033.
- Multiple developers can take US1 and US2 concurrently after the foundation, provided shared-file edits are coordinated.
- T038 must run after all backend GraphQL schema changes and before final generated-artifact validation is accepted.

## Implementation Strategy

### MVP First

1. Complete Phases 1–2.
2. Complete US1 and validate subscription history independently.
3. Stop for review before entitlement/frontend breadth.

### Incremental Delivery

1. Add US2 entitlement history.
2. Add US3 list/frontend behavior and one-time booking regressions.
3. Run generated-contract and quickstart validation.

## Format Validation

Every task uses `- [ ] T###`, has an exact repository-relative file path, uses `[P]` only for parallelizable tasks, and includes `[US1]`, `[US2]`, or `[US3]` on user-story tasks.
