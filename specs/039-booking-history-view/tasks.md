# Tasks: Unified Marketplace Booking History

**Input**: Design documents from `specs/039-booking-history-view/`  
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/graphql.md](contracts/graphql.md), [quickstart.md](quickstart.md)

**Tests**: Unit tests are required for isolated Booking/API behavior; integration tests cover repository filtering/pagination only; Spaces and Host use Vitest/RTL for visible operator behavior.

**Organization**: Tasks are grouped by user story so each can be completed and validated independently after the foundational contract is ready.

## Phase 1: Setup

**Purpose**: Establish the implementation boundaries and keep current behavior as the baseline.

- [X] T001 Record the retained-record source audit and existing repository retention behavior in `specs/039-booking-history-view/research.md`
- [X] T002 [P] Capture current Spaces subscriptions-page query behavior in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T003 [P] Capture current Host subscriptions-page query behavior in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.tsx`

---

## Phase 2: Foundational Contract and Read Infrastructure

**Purpose**: Build the shared, authorized, cursor-safe read foundation required by every story.

**⚠️ CRITICAL**: Complete this phase before user-story UI work.

- [X] T004 Define source type, lifecycle state, renewal state, search criteria, order fields, and cursor identity in `src/booking/shared/Booking.Shared/Models/MarketplacePurchaseHistory.cs`
- [X] T005 [P] Add explicit source-string/state mappings and display-name extensions in `src/booking/shared/Booking.Shared/Models/MarketplacePurchaseHistory.cs`
- [X] T006 [P] Add unit coverage for classification, lifecycle/renewal mappings, and cursor tie breakers in `src/booking/shared/Booking.Shared.UnitTests/Models/MarketplacePurchaseHistoryTests/MarketplacePurchaseHistoryShould.cs`
- [X] T007 Add the combined retained marketplace-booking/subscription query that preserves existing retention behavior without a new cutoff, plus keyset pagination implementation in `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`
- [X] T008 Add the repository interface beside its implementation in `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`
- [X] T009 Register the repository in `src/booking/shared/Booking.Shared/Repositories/RepositoryFactory.cs`
- [X] T010 Add only persistence-specific scenarios not covered by unit tests—retained-record visibility, deduplication, authorization scope, database filtering, and keyset-pagination behavior—in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistory/MarketplacePurchaseHistoryRepositoryShould.cs`
- [X] T011 Create the Booking API service interface and authorized shared-model orchestration in `src/booking/apis/Booking.Api/Services/MarketplacePurchaseHistoryService.cs`
- [X] T012 [P] Add service unit tests for authorization, all-retained default, source counts, and legacy relationship warnings in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplacePurchaseHistoryServiceTests/GetPaginatedAsyncShould.cs`
- [X] T013 Register the API service in `src/booking/apis/Booking.Api/Extensions.cs`
- [X] T014 Add GraphQL details, edge, where/order inputs, and query resolver rooted at `src/booking/apis/Booking.Api/GraphQL/MarketplacePurchaseHistory/RootQuery.cs`
- [X] T015 Map `MarketplacePurchaseHistoryEntry` only at the GraphQL boundary in `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`
- [X] T016 Add source GraphQL definitions, queryable choice types, and deterministic connection fields in `src/booking/apis/Booking.Api/schema.graphqls`
- [X] T017 Add GraphQL resolver and contract tests in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/RootQueryTests/MarketplacePurchasesShould.cs`
- [X] T018 Run `scripts/generate-graphql.sh` to regenerate `api-definitions/graphql/skedular/v1/schema.graphql`

**Checkpoint**: The authorized `marketplacePurchases` connection returns all retained records with stable cursors, but no operator page consumes it yet.

---

## Phase 3: User Story 1 - Review All Marketplace Purchases (Priority: P1) 🎯 MVP

**Goal**: Give an operator one renamed page containing standalone marketplace bookings and subscription roots exactly once.

**Independent Test**: Seed a standalone booking and a subscription in one organization; verify the Marketplace purchases page returns both once, opens their source details, and preserves existing URLs.

- [X] T019 [P] [US1] Add the `marketplacePurchases` Relay query, cursor variables, and pagination loading behavior in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T020 [P] [US1] Add the matching `marketplacePurchases` Relay query, cursor variables, and pagination loading behavior in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T021 [P] [US1] Rename Spaces navigation labels and accessible copy while preserving `/subscriptions` links in `src/web/apps/webapp-spaces/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [X] T022 [P] [US1] Rename Host navigation labels and accessible copy while preserving `/subscriptions` links in `src/web/apps/webapp-host/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [X] T023 [US1] Render source-type, customer, product, booking window, amount/currency, payment, lifecycle, and renewal fields in the Spaces list/grid views at `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T024 [US1] Render source-type, customer, product, booking window, amount/currency, payment, lifecycle, and renewal fields in the Host list/grid views at `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T025 [P] [US1] Add Spaces page tests for renamed copy, unified source rendering, first/next/previous page behavior, and list/grid equivalence in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.test.tsx`
- [X] T026 [P] [US1] Add Host page tests for renamed copy, unified source rendering, first/next/previous page behavior, and list/grid equivalence in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.test.tsx`
- [X] T027 [US1] Regenerate Spaces Relay artifacts with `pnpm --dir src/web/apps/webapp-spaces relay`
- [X] T028 [US1] Regenerate Host Relay artifacts with `pnpm --dir src/web/apps/webapp-host relay`

**Checkpoint**: The existing subscriptions URL presents Marketplace purchases and shows every active retained source once, with functioning keyset pagination in both apps.

---

## Phase 4: User Story 2 - Investigate Canceled or Deleted Activity (Priority: P1)

**Goal**: Retain inactive purchase evidence and make its authoritative history navigable without falsely conflating payment, cancellation, and refund state.

**Independent Test**: Cancel/delete paid standalone and subscription purchases, then locate each from Marketplace purchases and inspect its final state, timestamps, actor/reason where available, and refund timeline.

- [X] T029 [US2] Extend the unified repository projection with deletion/cancellation actor, reason, and independent payment/refund/lifecycle fields in `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`
- [X] T030 [P] [US2] Add API/service unit coverage for canceled, deleted, expired, payment-failed, pending-refund, and unavailable legacy relationships in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplacePurchaseHistoryServiceTests/GetPaginatedAsyncShould.cs`
- [X] T031 [US2] Extend inactive-history GraphQL mapping in `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs` and source fields in `src/booking/apis/Booking.Api/schema.graphqls`
- [X] T032 [US2] Add deleted/canceled/refund GraphQL contract coverage in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/RootQueryTests/MarketplacePurchasesShould.cs`
- [X] T033 [US2] Surface inactive-state evidence, refund links/timelines, partial-data, empty, loading, and authorization/error states in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T034 [US2] Surface matching inactive-state evidence, refund links/timelines, partial-data, empty, loading, and authorization/error states in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T035 [P] [US2] Add Spaces inactive-history and state-separation tests in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.test.tsx`
- [X] T036 [P] [US2] Add Host inactive-history and state-separation tests in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.test.tsx`
- [X] T037 Run `scripts/generate-graphql.sh` to regenerate `api-definitions/graphql/skedular/v1/schema.graphql`

**Checkpoint**: Inactive retained purchases are discoverable and their lifecycle, payment, and refund outcomes are independently understandable.

---

## Phase 5: User Story 3 - Search and Reconcile Lifecycle and Money (Priority: P2)

**Goal**: Give operators complete filtering and deterministic sorting over the unified history.

**Independent Test**: Apply source, lifecycle, payment, refund, customer, product, renewal, cadence, and date filters alone and together; verify correct total counts and stable pages.

- [X] T038 [US3] Implement all specified filter predicates and activity/purchase/booking/end ordering, with activity precedence and source-type/source-ID tie breakers, in `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`
- [X] T039 [P] [US3] Add only database-query scenarios not covered by unit tests—filter combinations, equal-sort-value cursors, and changing-data cursor behavior—in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistory/MarketplacePurchaseHistoryRepositoryShould.cs`
- [X] T040 [US3] Extend GraphQL where/order inputs and choice-detail queries in `src/booking/apis/Booking.Api/schema.graphqls`
- [X] T041 [US3] Add explicit unknown-filter logging and resolver validation tests in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/RootQueryTests/MarketplacePurchasesShould.cs`
- [X] T042 [P] [US3] Build the Spaces filter controls, URL/query-state reset behavior, and sorting controls in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T043 [P] [US3] Build the Host filter controls, URL/query-state reset behavior, and sorting controls in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.tsx`
- [X] T044 [P] [US3] Add Spaces filter/sort/pagination regression tests in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.test.tsx`
- [X] T045 [P] [US3] Add Host filter/sort/pagination regression tests in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/page.test.tsx`
- [X] T046 Run `scripts/generate-graphql.sh` to regenerate `api-definitions/graphql/skedular/v1/schema.graphql`
- [X] T047 Regenerate operator Relay artifacts with `pnpm --dir src/web/apps/webapp-spaces relay` and `pnpm --dir src/web/apps/webapp-host relay`

**Checkpoint**: Operators can reconcile the combined history using all required filters and deterministic pagination.

---

## Phase 6: User Story 4 - Preserve Correct Operational Behavior (Priority: P2)

**Goal**: Keep the history page read-only, put generated instances in subscription details, and provide safe cross-links from bookings.

**Independent Test**: Verify a standalone hourly purchase creates no subscription workflow; verify a subscription detail pages/filters its instances and a generated booking opens the correct parent subscription.

- [X] T048 [US4] Add the paginated, filterable subscription booking-instance repository query in `src/booking/shared/Booking.Shared/Repositories/MarketplaceBookingSubscriptionRepository.cs`
- [X] T049 [US4] Add subscription-instance pagination/filter integration coverage only for repository/database behavior not covered by unit tests in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplaceBookingSubscription/BookingInstancesShould.cs`
- [X] T050 [US4] Add shared instance models and subscription-service access method in `src/booking/apis/Booking.Api/Services/MarketplaceBookingSubscriptionService.cs`
- [X] T051 [US4] Add `bookingInstances` GraphQL field, edge/details/where/order types, and nullable booking parent-subscription field in `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/MarketplaceBookingSubscriptionDetails.cs`
- [X] T052 [P] [US4] Add GraphQL unit tests for instance pagination and standalone-null parent links in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplaceBookingSubscription/MarketplaceBookingSubscriptionDetailsTests/BookingInstancesShould.cs`
- [X] T053 [US4] Replace unbounded instance rendering with the paginated/filterable connection in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/subscription/page.tsx`
- [X] T054 [US4] Replace unbounded instance rendering with the paginated/filterable connection in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/subscription/page.tsx`
- [X] T055 [P] [US4] Add the generated-booking-to-parent-subscription navigation link in `src/web/apps/webapp-spaces/src/components/booking/bookings/booking-card.tsx`
- [X] T056 [P] [US4] Add the generated-booking-to-parent-subscription navigation link in `src/web/apps/webapp-host/src/components/booking/bookings/booking-card.tsx`
- [X] T057 [P] [US4] Add Spaces subscription-instance and parent-link tests in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/subscription/page.test.tsx`
- [X] T058 [P] [US4] Add Host subscription-instance and parent-link tests in `src/web/apps/webapp-host/src/rootPages/organizations/organization/subscriptions/subscription/page.test.tsx`
- [X] T059 Run `scripts/generate-graphql.sh` to regenerate `api-definitions/graphql/skedular/v1/schema.graphql`
- [X] T060 Regenerate operator Relay artifacts with `pnpm --dir src/web/apps/webapp-spaces relay` and `pnpm --dir src/web/apps/webapp-host relay`

**Checkpoint**: Subscription instances are discoverable only in their parent detail context, booking surfaces link to the parent, and no read path creates subscription work.

---

## Phase 7: Polish and Cross-Cutting Concerns

**Purpose**: Complete documentation, observability, generated outputs, and end-to-end validation.

- [X] T061 [P] Update Spaces operator guidance for the renamed Marketplace purchases page in `src/web/apps/public-web/src/content/docs/spaces/bookings/subscriptions.md`
- [X] T062 [P] Update Host operator guidance for the renamed Marketplace purchases page in `src/web/apps/public-web/src/content/docs/host/bookings/bookings-and-renters.md`
- [X] T063 [P] Update shared subscription guidance while preserving the one-time-booking distinction in `src/web/apps/public-web/doc-resources/subscriptions.md`
- [X] T064 Add structured query/reconciliation warning logs and runtime logging tests in `src/booking/apis/Booking.Api/Services/MarketplacePurchaseHistoryService.cs`
- [X] T065 Run Booking API and shared unit tests from `src/booking/apis/Booking.Api.UnitTests/Booking.Api.UnitTests.csproj`
- [X] T066 Run the targeted Booking repository integration tests that remain after unit coverage from `src/booking/domain/Booking.Domain.IntegrationTests/Booking.Domain.IntegrationTests.csproj`
- [X] T067 Run Spaces and Host focused Vitest suites from `src/web/apps/webapp-spaces/` and `src/web/apps/webapp-host/`
- [X] T068 Execute every scenario in `specs/039-booking-history-view/quickstart.md`
- [X] T069 Add unit coverage proving the read service only queries history data and never dispatches renewal, allocation, cancellation, or outbox work in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplacePurchaseHistoryServiceTests/GetPaginatedAsyncShould.cs`
- [X] T070 Add table-driven unit coverage for the agreed purchase-type and lifecycle-label state matrix in `src/booking/shared/Booking.Shared.UnitTests/Models/MarketplacePurchaseHistoryTests/MarketplacePurchaseHistoryShould.cs`
- [X] T071 Run `graphify update .` to refresh `graphify-out/graph.json`

---

## Dependencies & Execution Order

- **Phase 1** has no dependencies.
- **Phase 2** depends on Phase 1 and blocks all user stories.
- **US1 (Phase 3)** is the MVP and depends on Phase 2.
- **US2 (Phase 4)** depends on the unified contract from Phase 2; it can follow US1's UI shell.
- **US3 (Phase 5)** depends on Phase 2 and extends the US1 page.
- **US4 (Phase 6)** depends on Phase 2 and can be developed in parallel with US3 after the schema contract has settled.
- **Phase 7** follows all desired stories.

```text
Setup → Foundation → US1 (MVP) → US2 → US3
                         └──────────────→ US4
US2 + US3 + US4 → Polish
```

## Parallel Opportunities

- T002/T003, T005/T006, T010/T012, T019/T020, T021/T022, T025/T026, T033/T034, T035/T036, T042/T043, T044/T045, T055/T056, T057/T058, and T061/T063 can run in parallel as marked.
- After T018, Spaces and Host work can proceed in parallel for each story.
- Repository integration tests and API unit tests can run in parallel after their corresponding contract work is present.

## Implementation Strategy

### MVP first

1. Complete Phases 1–2.
2. Complete US1 to expose a single paginated Marketplace purchases list in both apps.
3. Demonstrate a standalone booking and subscription root appearing exactly once.

### Incremental delivery

1. Add inactive/refund history (US2).
2. Add the full reconciliation filter/sort matrix (US3).
3. Add subscription-instance pagination and booking-to-parent navigation (US4).
4. Finish documentation, generated artifacts, and the quickstart validation.

## Format Validation

All 71 tasks use the required checkbox, sequential ID, optional parallel marker, story label for story phases, and exact file path format.

## Durable Projection Revision

- [X] T072 Add the compact `MarketplacePurchaseHistory` entity, restrictive foreign-key relationships, indexes, and generated migration in `src/booking/shared/Booking.Shared/Database/Entities/MarketplacePurchaseHistory.cs` and `src/booking/shared/Booking.Shared/Database/Migrations/20260803110036_AddMarketplacePurchaseHistory.cs`
- [X] T073 Replace the non-translatable booking/subscription set operation with the durable projection query and transactionally maintain it from marketplace booking, subscription, booking, refund, and subscription-customer writes in `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`
- [X] T074 Validate the durable projection through the focused Booking repository integration suite in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistoryRepositoryShould.cs`
