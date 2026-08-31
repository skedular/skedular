# Tasks: Required Days Across Longer Cadences

**Input**: Design documents from `/specs/046-required-days-cadence/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

## Phase 1: Setup

- [ ] T001 Review current ProductPricing, event, GraphQL, editor, booking, entitlement, and subscription paths in the files listed in `specs/046-required-days-cadence/research.md`.
- [ ] T002 [P] Add focused test fixtures for longer purchase cadences and UTC week boundaries in `src/booking/shared/Booking.Shared.UnitTests/`.

## Phase 2: Foundational

- [ ] T003 Implement a shared UTC calendar-week calculator and complete-boundary predicate in `src/booking/shared/Booking.Shared/Services/`.
- [ ] T004 [P] Add explicit longer-cadence and fulfillment validation rules in `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`.
- [ ] T005 [P] Add unit tests for UTC week keys, complete boundary weeks, available-day limits, and cadence validation in `src/booking/shared/Booking.Shared.UnitTests/` and `src/marketplace/apis/Marketplace.Api.UnitTests/Services/`.
- [ ] T006 Define structured logging properties for pricing validation and weekly eligibility decisions in the owning Booking/Marketplace services.

## Phase 3: User Story 1 - Preserve Weekly Offers (Priority: P1)

**Goal**: Keep existing weekly exact selected-day behavior unchanged.

**Independent Test**: Existing weekly selection, mapper, editor, and subscription tests pass unchanged, including duplicate/unavailable day rejection.

- [ ] T007 [P] [US1] Extend `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingWeeklyDaySelectionServiceTests/` with regression coverage for weekly exact selection.
- [ ] T008 [P] [US1] Extend `src/marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/ValidateWeeklyDaySelectionShould.cs` for weekly validation compatibility.
- [ ] T009 [US1] Ensure product-version matching and Marketplace/Booking event mappers preserve `RequiredDaysPerWeek` in `src/shared/Api.Shared.Services/Models/ProductPricing.cs`, `src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs`, and `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`.
- [ ] T010 [US1] Run focused weekly backend tests and verify no generated contract drift.

## Phase 4: User Story 2 - Enforce Longer-Cadence Schedules (Priority: P1)

**Goal**: Enforce exactly N bookings per complete UTC week for longer date-based reservation and subscription cadences.

**Independent Test**: A longer-cadence offer accepts exactly N eligible bookings in complete weeks, exempts boundary partial weeks, and rejects invalid weekday schedules.

- [ ] T011 [P] [US2] Add longer-cadence validator tests in `src/marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/`.
- [ ] T012 [P] [US2] Add booking validation tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/`.
- [ ] T013 [US2] Apply shared UTC-week schedule validation to `src/booking/shared/Booking.Shared/Services/MarketplaceBookingWeeklyDaySelectionService.cs` and related booking services.
- [ ] T014 [US2] Update recurring generation and renewal enforcement in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.
- [ ] T015 [P] [US2] Update Host editor cadence gating and guidance in `src/web/apps/webapp-host/src/components/product/product-editor-shared.ts`, `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`, and related add/edit pages.
- [ ] T016 [P] [US2] Update Spaces editor cadence gating and guidance in `src/web/apps/webapp-spaces/src/components/product/product-editor-shared.ts` and related add/edit pages.
- [ ] T017 [US2] Regenerate Relay artifacts with `pnpm --dir src/web relay` if operations changed; do not edit generated files.
- [ ] T018 [US2] Add subscription integration coverage for fortnightly, monthly, two-, three-, four-, five-, six-month, and yearly cadences in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`.

## Phase 5: User Story 3 - Limit Credit Entitlement Redemptions (Priority: P1)

**Goal**: Allow at most N confirmed credit redemptions per complete UTC week.

**Independent Test**: Concurrent redemptions cannot exceed N; fewer redemptions remain valid; `availableDays` and entitlement status rules still apply.

- [ ] T019 [P] [US3] Add repository query or durable usage model for confirmed entitlement redemptions by entitlement and UTC week in `src/booking/shared/Booking.Shared/Repositories/` and database entities/configuration.
- [ ] T020 [P] [US3] Add entitlement weekly-limit unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/EntitlementBookingServiceTests/`.
- [ ] T021 [US3] Enforce the at-most-N check atomically in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementBookingService.cs`.
- [ ] T022 [US3] Add repository-backed concurrency and persistence coverage in `src/booking/domain/Booking.Domain.IntegrationTests/Services/EntitlementConcurrentClaimShould.cs`.
- [ ] T023 [US3] Expose entitlement restriction data through shared models and Booking GraphQL mappings in `src/booking/apis/Booking.Api/GraphQL/Entitlement/EntitlementDetails.cs` if the UI needs it.

## Phase 6: Contracts, Documentation, and Verification

- [ ] T024 [P] Review source GraphQL definitions and protobuf event source in `api-definitions/`; update only if required, then run `scripts/generate-graphql.sh` and `api-definitions/events/generate.sh`.
- [ ] T025 [P] Update customer/operator documentation for longer-cadence weekly limits in the relevant public-web docs under `src/web/apps/public-web/`.
- [ ] T026 Run focused backend tests, relevant integration tests, and frontend Vitest tests; record zero-test or unrun outcomes accurately.
- [ ] T027 Run `git diff --check`, inspect generated diffs, and run `graphify update .` after code changes.

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
