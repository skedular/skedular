# Tasks: Subscription Landing Page Filtering

**Input**: Design documents from `specs/005-subscription-landing-page-filter/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/graphql-filter-api.md](contracts/graphql-filter-api.md)

**Feature**: Replace existing single-select, client-side subscription status/payment filters with multi-select, backend-driven filters. Extend the Booking domain GraphQL API with multi-value filter inputs and two new filter-option queries; update the Management Portal subscription page to use them with URL-synced state.

---

## Phase 1: Setup

**Purpose**: Establish the foundational backend and frontend structures needed before any user story work begins.

- [x] T001 Verify `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/` directory structure matches plan and identify all files to change
- [x] T002 [P] Confirm `scripts/generate-graphql.sh` and `web/apps/webapp/scripts/generate.sh` are executable and accessible from the repo root

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core backend model, GraphQL type, and name-mapping extension that ALL user stories depend on. Must be complete before any user story phase begins.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T003 Add `MarketplaceBookingPaymentStatusDetails` C# class to `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/MarketplaceBookingPaymentStatusDetails.cs` — mirrors `MarketplaceBookingSubscriptionStatusDetails`, with `[GraphQLName("type")] PaymentStatus Type` and `[GraphQLName("name")] string Name`
- [x] T004 Add `ToMarketplaceBookingPaymentStatusName()` C# extension method mapping each `PaymentStatus` enum value to its display name (British spelling) in `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/` — follows the pattern of `ToMarketplaceBookingSubscriptionCancellationModeName()`
- [x] T005 Extend `booking/shared/Booking.Shared/Models/MarketplaceBookingSubscriptionSearch.cs` — add `ICollection<MarketplaceBookingSubscriptionStatus> Statuses` and `ICollection<PaymentStatus> PaymentStatuses` parameters to the `MarketplaceBookingSubscriptionSearchCriteria` record (default to empty collections; preserve existing `Status?` parameter for backward compatibility)
- [x] T006 Add structured logging contract comment in `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/RootQuery.cs` documenting the LOG-001 through LOG-004 log events (filter inputs received, unrecognised values, option-load failures, correlation context) — no code change yet, just establishes the contract before the resolver tasks

**Checkpoint**: Foundation complete — user story phases can begin.

---

## Phase 3: User Story 4 — Backend-Driven Filter Option Values (Priority: P2) 🎯 Pre-requisite for US1 & US2

**Goal**: Two new GraphQL queries (`marketplaceBookingSubscriptionStatuses`, `marketplaceBookingPaymentStatuses`) return all valid filter option values so the frontend never hard-codes them.

**Independent Test**: Query `marketplaceBookingSubscriptionStatuses` and `marketplaceBookingPaymentStatuses` from the booking API; both return non-empty arrays of `{ type, name }` objects matching the enum definitions.

### Tests for User Story 4

- [x] T007 [P] [US4] Add unit test `MarketplaceBookingSubscriptionStatusesShould.cs` in `booking/apis/Booking.Api.UnitTests/` — assert resolver returns all 5 `MarketplaceBookingSubscriptionStatus` values each with a non-empty `Name`
- [x] T008 [P] [US4] Add unit test `MarketplaceBookingPaymentStatusesShould.cs` in `booking/apis/Booking.Api.UnitTests/` — assert resolver returns all operator-relevant `PaymentStatus` values each with a non-empty `Name`

### Implementation for User Story 4

- [x] T009 [US4] Add `MarketplaceBookingSubscriptionStatuses()` resolver method to `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/RootQuery.cs` — returns `IEnumerable<MarketplaceBookingSubscriptionStatusDetails>` with all 5 enum values using `ToMarketplaceBookingSubscriptionStatus()` name mapping; follows the `MarketplaceBookingSubscriptionCancellationModes()` pattern exactly
- [x] T010 [US4] Add `MarketplaceBookingPaymentStatuses()` resolver method to the same `RootQuery.cs` — returns `IEnumerable<MarketplaceBookingPaymentStatusDetails>` using the `ToMarketplaceBookingPaymentStatusName()` extension from T004
- [x] T011 [US4] Add LOG-001 structured log call in both new resolver methods (log resolver invocation and result count per LOG-001 and LOG-004)
- [x] T012 [US4] Run `scripts/generate-graphql.sh` from repo root — regenerates `booking/apis/Booking.Api/schema.graphql`, composed gateway schema, and integration-test schema files; verify no hand-edits to generated files
- [x] T013 [P] [US4] Add integration test `MarketplaceBookingSubscriptionStatusesQueryShould.cs` in `booking/domain/Booking.Domain.IntegrationTests/` — queries `marketplaceBookingSubscriptionStatuses` via the API and asserts all 5 values are returned (uses repository layer for any setup data)
- [x] T014 [P] [US4] Add integration test `MarketplaceBookingPaymentStatusesQueryShould.cs` in `booking/domain/Booking.Domain.IntegrationTests/` — queries `marketplaceBookingPaymentStatuses` via the API and asserts expected values are returned

**Checkpoint**: Both filter-option queries work end-to-end. Frontend can now be wired to them.

---

## Phase 4: User Story 1 — Filter Subscriptions by Subscription Status (Priority: P1) 🎯 MVP

**Goal**: The subscription list accepts a multi-value `statuses` filter input; the backend returns only matching subscriptions; the UI renders a multi-select combo box populated from US4 data; selecting statuses triggers an immediate backend re-query and reflects selections in the URL.

**Independent Test**: Apply `statuses: [ACTIVE]` — every returned subscription has `status.type == "ACTIVE"`. Remove all selections — full unfiltered list returns.

### Tests for User Story 1

- [x] T015 [P] [US1] Add unit test `FilterByStatusesShould.cs` in `booking/shared/Booking.Shared.UnitTests/Repositories/` — assert `AddSearchCriteria` with a single `Statuses` value produces a query returning only matching subscriptions; with empty `Statuses` produces no restriction; with multiple values uses OR semantics
- [x] T016 [P] [US1] Add unit test `MarketplaceBookingSubscriptionsWithStatusFilterShould.cs` in `booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/` — assert `GetPaginatedMarketplaceBookingSubscriptionsAsync` passes `Statuses` from `WhereInput` through to `SearchCriteria`

### Backend Implementation for User Story 1

- [x] T017 [US1] Extend `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/MarketplaceBookingSubscriptionWhereInput.cs` — add `[GraphQLName("statuses")] IEnumerable<MarketplaceBookingSubscriptionStatus>? Statuses` property
- [x] T018 [US1] Update `booking/shared/Booking.Shared/Repositories/MarketplaceBookingSubscriptionRepository.cs` `AddSearchCriteria` extension — add `if (searchCriteria.Statuses.Count != 0)` predicate: `originalQuery = originalQuery.Where(item => searchCriteria.Statuses.Select(s => s.ToMarketplaceBookingSubscriptionStatus()).Contains(item.Status))`
- [x] T019 [US1] Update `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/RootQuery.cs` `MarketplaceBookingSubscriptionsAsync` — add `where.Statuses.ToSafeCollection()` mapping into `MarketplaceBookingSubscriptionSearchCriteria` constructor call; add LOG-001 log statement logging received `statuses` count and result count
- [x] T020 [US1] Add LOG-002 warning log in `MarketplaceBookingSubscriptionsAsync` for any unrecognised status string values submitted (validate against known enum values before passing to service)
- [x] T021 [US1] Add integration test `FilterBySubscriptionStatusShould.cs` in `booking/domain/Booking.Domain.IntegrationTests/` — seeds subscriptions with mixed statuses; queries with `statuses: [ACTIVE]` via repository; asserts only active subscriptions returned; asserts empty `statuses` returns all

### Frontend Implementation for User Story 1

- [x] T022 [P] [US1] Create `web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-booking-subscription-status.ts` — export `SupportedMarketplaceBookingSubscriptionStatusForFilter` union type of all valid status strings, `MarketplaceBookingSubscriptionStatusForFilterDetails` type `{ type, name }`, and `isSupportedMarketplaceBookingSubscriptionStatusForFilter` guard function; follow pattern of `marketplace-booking-subscription-cancellation-mode.ts`
- [x] T023 [P] [US1] Create `web/apps/webapp/src/components/organization/multiple-choices-marketplace-booking-subscription-statuses.tsx` — Relay fragment on `Query { marketplaceBookingSubscriptionStatuses { type name } }`, `Autocomplete` from `mui-rff` with `multiple={true}`, `disableCloseOnSelect`, `getOptionValue={(o) => o.type}`, `getOptionLabel={(o) => o.name}`, `BodyIconTypography` in `renderOption`; follows `multiple-choices-product-pricing-billing-modes.tsx` pattern exactly
- [x] T024 [US1] Update `web/apps/webapp/src/rootPages/organizations/organization/subscriptions/page.tsx` root query — add `marketplaceBookingSubscriptionStatuses { type name }` fragment; add `$statuses: [MarketplaceBookingSubscriptionStatus!]` variable to `marketplaceBookingSubscriptions` call; add `statuses: $statuses` to `where` input
- [x] T025 [US1] Replace `statusFilter` string state in `page.tsx` with `selectedStatuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[]` array state initialised from `useSearchParams().get('statuses')` URL parameter (parse comma-separated values on mount)
- [x] T026 [US1] Add `useRouter` URL write in `page.tsx` — on `selectedStatuses` change, call `router.replace` updating `?statuses=` query param (comma-joined type strings; remove param when array is empty); reset pagination cursor to first page
- [x] T027 [US1] Wire `MultipleChoicesMarketplaceBookingSubscriptionStatuses` component into `page.tsx` toolbar area; remove existing single-select `statusFilter` TextField; pass `rootDataRelay` and `name="statuses"` props; on `onChange` update `selectedStatuses` state and trigger `loadQuery` with updated `$statuses` variable
- [x] T028 [US1] Add skeleton/loading overlay on the subscription list in `page.tsx` while `isLoading` Relay state is true; keep filter controls interactive during loading (do not disable them)
- [x] T029 [US1] Add Vitest unit test for `MultipleChoicesMarketplaceBookingSubscriptionStatuses` — mock relay data with 5 status options; assert all options rendered; assert `multiple` behaviour
- [x] T030 [US1] Add Vitest unit test for `page.tsx` status filter interaction — selecting a status updates `?statuses=` URL param; deselecting all clears the param; page re-loads query with correct variables
- [x] T031 [US1] Run `web/apps/webapp/scripts/generate.sh` — regenerate Relay artefacts; verify no hand-edits to generated files under `src/queries/__generated__/`

**Checkpoint**: Subscription status filter fully functional end-to-end. MVP is shippable.

---

## Phase 5: User Story 2 — Filter Subscriptions by Payment Status (Priority: P2)

**Goal**: The subscription list accepts a multi-value `paymentStatuses` filter; the backend applies it via an EF Core navigation property predicate on `MarketplaceBooking.PaymentStatus`; the UI renders a separate multi-select combo box for payment status, backend-driven from `marketplaceBookingPaymentStatuses`.

**Independent Test**: Apply `paymentStatuses: [PENDING]` — every returned subscription has `marketplaceBooking.paymentStatus.type == "PENDING"`. Remove all selections — full list returns.

### Tests for User Story 2

- [x] T032 [P] [US2] Add unit test `FilterByPaymentStatusesShould.cs` in `booking/shared/Booking.Shared.UnitTests/Repositories/` — assert `AddSearchCriteria` with a single `PaymentStatuses` value filters correctly via navigation property; empty collection returns no restriction
- [x] T033 [P] [US2] Add unit test `MarketplaceBookingSubscriptionsWithPaymentFilterShould.cs` in `booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/` — assert `GetPaginatedMarketplaceBookingSubscriptionsAsync` passes `PaymentStatuses` from `WhereInput` through to `SearchCriteria`

### Backend Implementation for User Story 2

- [x] T034 [US2] Extend `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/MarketplaceBookingSubscriptionWhereInput.cs` — add `[GraphQLName("paymentStatuses")] IEnumerable<PaymentStatus>? PaymentStatuses` property
- [x] T035 [US2] Update `booking/shared/Booking.Shared/Repositories/MarketplaceBookingSubscriptionRepository.cs` `AddSearchCriteria` — add `if (searchCriteria.PaymentStatuses.Count != 0)` predicate: `originalQuery = originalQuery.Where(item => searchCriteria.PaymentStatuses.Select(s => s.ToPaymentStatusString()).Contains(item.MarketplaceBooking.PaymentStatus))` (via navigation property; no explicit Include needed — EF Core generates JOIN)
- [x] T036 [US2] Update `RootQuery.cs` `MarketplaceBookingSubscriptionsAsync` — add `where.PaymentStatuses.ToSafeCollection()` mapping into `SearchCriteria`; extend LOG-001 log to include `paymentStatuses` count
- [x] T037 [US2] Add integration test `FilterByPaymentStatusShould.cs` in `booking/domain/Booking.Domain.IntegrationTests/` — seeds subscriptions with mixed payment statuses; queries with `paymentStatuses: [PENDING]`; asserts only PENDING payment subscriptions returned; empty `paymentStatuses` returns all

### Frontend Implementation for User Story 2

- [x] T038 [P] [US2] Create `web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-booking-payment-status.ts` — export `SupportedMarketplaceBookingPaymentStatusForFilter` union type, `MarketplaceBookingPaymentStatusForFilterDetails` type, and guard function; follow same pattern as T022
- [x] T039 [P] [US2] Create `web/apps/webapp/src/components/organization/multiple-choices-marketplace-booking-payment-statuses.tsx` — Relay fragment on `Query { marketplaceBookingPaymentStatuses { type name } }`, same `Autocomplete multiple` pattern as T023
- [x] T040 [US2] Update `page.tsx` root query — add `marketplaceBookingPaymentStatuses { type name }` fragment; add `$paymentStatuses: [PaymentStatus!]` variable; add `paymentStatuses: $paymentStatuses` to `where` input
- [x] T041 [US2] Replace `paymentFilter` string state in `page.tsx` with `selectedPaymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[]` array state initialised from `useSearchParams().get('paymentStatuses')` URL parameter
- [x] T042 [US2] Add `useRouter` URL write for `selectedPaymentStatuses` — on change update `?paymentStatuses=` param (comma-joined); remove param when empty; reset pagination cursor
- [x] T043 [US2] Wire `MultipleChoicesMarketplaceBookingPaymentStatuses` component into `page.tsx`; remove existing single-select `paymentFilter` TextField; on `onChange` update `selectedPaymentStatuses` state and trigger `loadQuery` with updated `$paymentStatuses` variable
- [x] T044 [US2] Add Vitest unit test for `MultipleChoicesMarketplaceBookingPaymentStatuses` — mock relay data with expected payment status options; assert all options rendered
- [x] T045 [US2] Add Vitest unit test for `page.tsx` payment status filter interaction — selecting a payment status updates `?paymentStatuses=` URL param; deselecting clears it
- [x] T046 [US2] Run `web/apps/webapp/scripts/generate.sh` — regenerate Relay artefacts for updated root query

**Checkpoint**: Payment status filter fully functional end-to-end.

---

## Phase 6: User Story 3 — Combined Filtering (Priority: P3)

**Goal**: Both filters active simultaneously; backend applies AND logic; pagination resets correctly; URL reflects both params; stale in-flight queries are discarded.

**Independent Test**: Apply `statuses: [ACTIVE]` AND `paymentStatuses: [PENDING]` — every result has `status.type == "ACTIVE"` AND `paymentStatus.type == "PENDING"`. Clearing one filter returns to single-filter results.

### Tests for User Story 3

- [x] T047 [P] [US3] Add unit test `FilterByCombinedStatusAndPaymentStatusShould.cs` in `booking/shared/Booking.Shared.UnitTests/Repositories/` — assert AND semantics: only subscriptions matching BOTH criteria returned; clearing one collection removes its restriction

### Backend Implementation for User Story 3

- [x] T048 [US3] Add integration test `FilterByCombinedStatusAndPaymentStatusShould.cs` in `booking/domain/Booking.Domain.IntegrationTests/` — seeds 4 subscriptions covering all combinations of status×payment status; queries with both `statuses` and `paymentStatuses` set; asserts only the intersection is returned

### Frontend Implementation for User Story 3

- [x] T049 [US3] Verify `page.tsx` `loadQuery` call passes both `$statuses` and `$paymentStatuses` variables together so combined filter triggers a single backend query (no separate re-query per filter)
- [x] T050 [US3] Add Vitest integration test for combined filter scenario in `page.tsx` — select one status AND one payment status; assert single query is issued with both variables; assert URL contains both params
- [x] T051 [US3] Verify pagination cursor reset logic in `page.tsx` handles both filter dimensions — any change to either filter resets to first page; add regression test
- [x] T052 [US3] Verify stale query discard behaviour — rapid successive filter changes result in only the latest query response being rendered; write Vitest test that fires two rapid selection changes and asserts only the second result is displayed

**Checkpoint**: Combined filter works. All three user story phases shippable together.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Observability, error handling, URL deep-link validation, and test coverage completion.

- [x] T053 [P] Add LOG-002 warning log unit test — assert that submitting an unrecognised status string to the resolver emits a structured warning log; use `ILogger` mock in `booking/apis/Booking.Api.UnitTests/`
- [x] T054 [P] Add Vitest test for pre-populated filter from URL — navigate to subscription page with `?statuses=ACTIVE&paymentStatuses=PENDING` in URL; assert filter controls show pre-selected values and initial query uses those filter variables (covers FR-014)
- [x] T055 [P] Add Vitest test for filter option load failure — mock relay error response for `marketplaceBookingSubscriptionStatuses`; assert combo box shows disabled/error state but subscription list still loads unfiltered (edge case from spec)
- [x] T056 [P] Add Vitest test for empty-state rendering — filter returns zero results; assert empty state UI shown without error or loading spinner
- [x] T057 Verify British spelling in all new user-facing labels: filter combo placeholder text, option names (e.g. "Renewal failed", "No payment required") — update any American spelling found
- [x] T058 Run full backend test suite `dotnet test` from repo root — confirm no regressions in existing subscription service or repository tests
- [x] T059 Run full frontend test suite `pnpm test` in `web/apps/webapp/` — confirm no regressions in existing subscription page tests
- [x] T060 Run `make generate` from repo root — confirm composed gateway schema, Relay artefacts, and all generated surfaces are in sync; commit generated file changes

---

## Dependencies

```
US4 (Phase 3) → must complete before US1 (Phase 4) and US2 (Phase 5)
US1 (Phase 4) → independent of US2; MVP can ship with only US1 + US4
US2 (Phase 5) → independent of US1; depends on US4
US3 (Phase 6) → depends on US1 + US2
Phase 7      → depends on US1 + US2 + US3
```

## Parallel Execution Examples

**Within Phase 3 (US4)**:

- T007, T008 (unit tests) can run in parallel
- T013, T014 (integration tests) can run in parallel after T012

**Within Phase 4 (US1)**:

- T015, T016 (backend unit tests) can run in parallel
- T022, T023 (frontend type helper + component) can run in parallel
- T029, T030 (frontend unit tests) can run in parallel

**Within Phase 5 (US2)**:

- T032, T033 (backend unit tests) can run in parallel
- T038, T039 (frontend type helper + component) can run in parallel
- T044, T045 (frontend unit tests) can run in parallel

**Within Phase 7**:

- T053–T056 can all run in parallel

## Implementation Strategy

**Recommended MVP scope**: Phase 1 + Phase 2 + Phase 3 (US4) + Phase 4 (US1)

This delivers backend-driven filter options plus subscription status filtering end-to-end. A space owner can filter by status with a multi-select combo, URL-synced, server-side. Payment status (US2) and combined filtering (US3) are additive on top.

**Incremental delivery order**:

1. Phases 1–3: Backend foundation + filter option queries (no UI change yet; backend shippable independently)
2. Phase 4: Subscription status filter (MVP — shippable to owners)
3. Phase 5: Payment status filter (additive)
4. Phase 6: Combined filter validation + stale-query handling
5. Phase 7: Polish, observability verification, full test run

## Format Summary

- Total tasks: **60**
- Phase 1 (Setup): 2
- Phase 2 (Foundational): 4
- Phase 3 — US4 (Filter option queries): 8
- Phase 4 — US1 (Subscription status filter): 17
- Phase 5 — US2 (Payment status filter): 15
- Phase 6 — US3 (Combined filter): 6
- Phase 7 (Polish): 8
- Parallelisable tasks [P]: 26
- Independent test criteria: Each story phase has its own checkpoint with independent test scenario
