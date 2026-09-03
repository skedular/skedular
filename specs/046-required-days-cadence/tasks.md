# Tasks: Required Days Across Longer Cadences

**Input**: Design documents from `/specs/046-required-days-cadence/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

## Phase 1: Setup

- [X] T001 Review current ProductPricing, event, GraphQL, editor, booking, entitlement, and subscription paths in the files listed in `specs/046-required-days-cadence/research.md`.
- [X] T002 [P] Add focused test fixtures for longer purchase cadences and UTC week boundaries in `src/booking/shared/Booking.Shared.UnitTests/`.

## Phase 2: Foundational

- [X] T003 Implement a shared UTC-only calendar-week calculator and `[start, end)` complete-boundary predicate, without timezone persistence or location-timezone lookup, in `src/booking/shared/Booking.Shared/Services/`.
- [X] T004 [P] Add validation and editor gating for Weekly and longer supported purchase cadences, hide the field for Daily, and keep cadence-free entitlements governed by validity in `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`.
- [X] T005 [P] Add unit tests for UTC week keys, complete boundary weeks, available-day limits, and cadence validation in `src/booking/shared/Booking.Shared.UnitTests/` and `src/marketplace/apis/Marketplace.Api.UnitTests/Services/`.
- [X] T006 Define structured logging properties for pricing validation and weekly eligibility decisions in the owning Booking/Marketplace services, and add runtime log assertions to the relevant unit tests.

## Phase 3: User Story 1 - Preserve Weekly Offers (Priority: P1)

**Goal**: Keep existing weekly exact selected-day behavior unchanged.

**Independent Test**: Existing weekly selection, mapper, editor, and subscription tests pass unchanged, including duplicate/unavailable day rejection.

- [X] T007 [P] [US1] Extend `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingWeeklyDaySelectionServiceTests/` with regression coverage for weekly exact selection.
- [X] T008 [P] [US1] Extend `src/marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/ValidateWeeklyDaySelectionShould.cs` for weekly validation compatibility.
- [X] T009 [US1] Ensure product-version matching and Marketplace/Booking event mappers preserve `RequiredDaysPerWeek` in `src/shared/Api.Shared.Services/Models/ProductPricing.cs`, `src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs`, and `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`.
- [X] T010 [US1] Run focused weekly backend tests and verify no generated contract drift.

## Phase 4: User Story 2 - Enforce Longer-Cadence Schedules (Priority: P1)

**Goal**: Enforce exactly N booking occurrences per complete UTC week for Weekly and longer reservation/subscription cadences.

**Independent Test**: A longer-cadence offer accepts exactly N eligible bookings in complete weeks, exempts boundary partial weeks, and rejects invalid weekday schedules.

- [X] T011 [P] [US2] Add longer-cadence validator tests in `src/marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/`.
- [X] T012 [P] [US2] Add direct reservation booking validation tests, including selected-weekday occurrence generation, in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/`.
- [X] T013 [US2] Apply shared UTC-week schedule validation to `src/booking/shared/Booking.Shared/Services/MarketplaceBookingWeeklyDaySelectionService.cs` and related booking services.
- [X] T014 [US2] Update recurring generation and renewal enforcement so each selected weekday creates one booking occurrence per applicable complete UTC week in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.
- [X] T015 [P] [US2] Update Host editor cadence gating and guidance in `src/web/apps/webapp-host/src/components/product/product-editor-shared.ts`, `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`, and related add/edit pages.
- [X] T016 [P] [US2] Update Spaces editor cadence gating and guidance in `src/web/apps/webapp-spaces/src/components/product/product-editor-shared.ts` and related add/edit pages.
- [X] T017 [US2] Regenerate Relay artifacts with `pnpm --dir src/web relay` if operations changed; do not edit generated files.
- [X] T018 [US2] Add subscription integration coverage for fortnightly, monthly, two-, three-, four-, five-, six-month, and yearly cadences in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`.

## Phase 5: User Story 3 - Limit Credit Entitlement Redemptions (Priority: P1)

**Goal**: Allow at most N successful credit redemptions per complete UTC week within a cadence-free entitlement validity period.

**Independent Test**: Concurrent redemptions cannot exceed N; fewer redemptions remain valid; `availableDays` and entitlement status rules still apply.

- [X] T019 [P] [US3] Reuse the existing durable credit-ledger entries for successful entitlement redemption counts, with repository querying by entitlement and UTC week; no new table or migration is required.
- [X] T020 [P] [US3] Add entitlement weekly-limit unit coverage in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/EntitlementBookingServiceTests/`, with consumption/allowance and runtime logging assertions; verify release, forfeiture, refund, and concurrency lifecycle behavior through the existing entitlement lifecycle and integration suites because those transitions are ledger-owned rather than implemented by the weekly-limit check.
- [X] T021 [US3] Enforce the weekly limit atomically through the existing serializable repository transaction and ledger lifecycle in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementBookingService.cs`.
- [X] T022 [US3] Add repository-backed concurrency and persistence coverage in `src/booking/domain/Booking.Domain.IntegrationTests/Services/EntitlementConcurrentClaimShould.cs`.
- [X] T023 [US3] Expose the remaining weekly entitlement allowance through shared models and Booking GraphQL mappings in `src/booking/apis/Booking.Api/GraphQL/Entitlement/EntitlementDetails.cs`, then update the customer entitlement UI and Relay operations to display it without weekday selection.

## Phase 6: Contracts, Documentation, and Verification

- [X] T024 [P] Review source GraphQL definitions and protobuf event source in `api-definitions/`; update only if required, then run `scripts/generate-graphql.sh`; the repository has no separate `api-definitions/events/generate.sh`, and no protobuf source change is required.
- [X] T025 [P] Update customer/operator documentation for longer-cadence weekly limits in the relevant public-web docs under `src/web/apps/public-web/`.
- [X] T026 Run focused backend tests, relevant integration tests, and frontend Vitest tests; record zero-test or unrun outcomes accurately.
- [X] T027 Run `git diff --check`, inspect generated diffs, and run the Graphify extraction refresh after code changes; full clustering remains environment-timeout limited.

## Dependencies & Execution Order

- Phase 1 precedes Phase 2; Phase 2 blocks all stories.
- US1 can proceed independently after Phase 2.
- US2 depends on the shared calculator and validation from Phase 2.
- US3 depends on the shared calculator and repository/concurrency foundation from Phase 2, but can run in parallel with US2.
- Phase 6 follows all stories and generation-sensitive changes.

## Parallel Opportunities

- T004/T005/T006 can proceed in parallel after T003 interfaces are agreed.
- T011/T012/T015/T016 can proceed in parallel.
- T019/T020 can proceed in parallel before T021.
- US2 and US3 can proceed in parallel after Phase 2.

## Implementation Strategy

MVP is US1 plus the shared foundation, then deliver US2 for reservations/subscriptions and US3 for entitlements as independently testable increments. Finish with source-contract regeneration, documentation, and full verification.
