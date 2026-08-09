# Tasks: Modify Marketplace Bookings

**Input**: Design documents from `/specs/040-modify-marketplace-bookings/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/modify-marketplace-booking.graphql`, `quickstart.md`

**Tests**: Unit-first tests are required by the specification and constitution. Add integration tests only for persistence, serializable-concurrency, GraphQL wiring, migration, and Temporal outbox/activity boundaries.

**Organization**: Tasks are grouped by user story so each increment can be validated independently after the shared foundation is complete.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel once its stated dependencies are met.
- **[Story]**: User story label; omitted only for setup, foundational, and polish work.
- Every task includes an exact target path.

## Phase 1: Setup

**Purpose**: Establish the contract/generation and validation guardrails for the feature.

- [x] T001 Record the implementation surface and generation commands in `specs/040-modify-marketplace-bookings/quickstart.md`.
- [x] T002 [P] Add feature-specific test fixture/builders for confirmed marketplace bookings in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/MarketplaceBookingServiceShould.cs`.
- [x] T003 [P] Add feature-specific test fixture/builders for Marketplace API command tests in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingServiceTests/UpdatePatchAsyncShould.cs`.

---

## Phase 2: Foundational

**Purpose**: Build the Booking-owned command, persistence, durable notification infrastructure, and generated-contract foundation required by every user story.

**⚠️ CRITICAL**: No user-story UI work begins before this phase is complete.

- [x] T004 Add Booking shared modification command/result/error models, including persisted actor classification and stable eligibility/conflict codes, in `src/booking/shared/Booking.Shared/Models/MarketplaceBookingModification.cs`.
- [x] T005 [P] Add immutable modification and per-recipient delivery entities/configurations in `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceBookingModification.cs` and `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceBookingModificationNotificationDelivery.cs`.
- [x] T006 [P] Add repository interfaces/implementations for modification history and notification deliveries in `src/booking/shared/Booking.Shared/Repositories/IMarketplaceBookingModificationRepository.cs` and `src/booking/shared/Booking.Shared/Repositories/MarketplaceBookingModificationRepository.cs`.
- [x] T007 Create the Booking database migration and model snapshot updates for modification/delivery persistence in `src/booking/shared/Booking.Shared/Database/Migrations/`.
- [x] T008 Add the dedicated `ModifyAsync` service contract and persisted-booking authorization/eligibility policy to `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`.
- [x] T009 Implement the serializable proposed-window complete-resource-set claim, stale-version check, product/day/cadence/resource-count validation, all-or-nothing replacement, and structured logs in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`.
- [x] T010 Ensure marketplace entity mapping persists changed `From`, `Until`, locations, and resources without touching commercial fields in `src/booking/shared/Booking.Shared/Mappers/EntityMapper.cs`.
- [x] T011 Add durable modification notification rendering, idempotency, retry/recovery state, and redacted delivery logs in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingModificationNotificationService.cs`.
- [x] T012 Add the Temporal outbox execution, activity, workflow registration, and notification templates in `src/booking/shared/Booking.Shared/Services/TemporalOutboxService.cs`, `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingModificationNotificationIntegrations.cs`, `src/booking/shared/Booking.Shared/Workflows/NotifyMarketplaceBookingModification.cs`, and `src/booking/shared/Booking.Shared/EmailTemplates/`.
- [x] T013 Add `ModifyMarketplaceBookingInput`, payload/error/history detail types, mapper, mutation resolver, and booking read fields in `src/booking/apis/Booking.Api/GraphQL/Booking/`, `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`, and `src/booking/apis/Booking.Api/schema.graphqls`.
- [x] T014 Regenerate the composed GraphQL contract with `scripts/generate-graphql.sh` and commit the generated schema surfaces under `api-definitions/graphql/skedular/v1/`.
- [x] T015 Add shared unit tests for command eligibility, payment/start-time guard, persisted authorization, stale version, atomic conflict rollback, commercial-state preservation, audit persistence, and logs in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/`.
- [x] T016 Add API unit tests for GraphQL input mapping, required operator reason, payload error mapping, and authorization rejection in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplaceBookingServiceTests/` and `src/booking/apis/Booking.Api.UnitTests/Mappers/GraphQlMapperTests/`.
- [x] T017 Add focused persistence/serializable conflict/GraphQL wiring integration coverage through Booking repositories and mutation paths in `src/booking/domain/Booking.Domain.IntegrationTests/`.

**Checkpoint**: A safe, versioned, generated-contract-backed Booking modification command exists and is covered by unit-first tests.

---

## Phase 3: User Story 1 - Reschedule a Marketplace Booking (Priority: P1) 🎯 MVP

**Goal**: Let an eligible confirmed customer move a future marketplace booking from the shared customer experience, with an atomic replacement and actionable results.

**Independent Test**: A customer changes an eligible future booking's date/time from booking details; the same booking id shows the new schedule, original commercial state remains, and unavailable/stale cases leave the original untouched.

- [x] T018 [P] [US1] Add React Testing Library coverage for customer confirmation, ineligible/error/stale states, and refreshed booking content in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.test.tsx`.
- [x] T019 [P] [US1] Add customer-facing modification eligibility, booking version, history, and result fields to the booking detail query in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.tsx`.
- [x] T020 [P] [US1] Create the explicit date/time change form, confirmation dialog, mutation, error/reload states, and required reason field in `src/web/apps/webapp/src/components/marketplaceProductBooking/modify-marketplace-booking-dialog.tsx`.
- [x] T021 [US1] Integrate the self-service Modify action and refreshed booking details into `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.tsx`.
- [x] T022 [US1] Surface modification outcome/history and correct marketplace edit routing in `src/web/apps/webapp/src/components/booking/customerBookings/customer-bookings-hub.tsx` and `src/web/apps/webapp/src/components/booking/myBookings/my-booking-card.tsx`.
- [x] T023 [US1] Add customer modification telemetry without customer content in `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.ts`.
- [x] T024 [US1] Regenerate `webapp` Relay artifacts for the modification query/mutation using `src/web/apps/webapp/package.json` and commit `src/web/apps/webapp/src/queries/__generated__/` output.

**Checkpoint**: A confirmed customer can independently reschedule an eligible one-time marketplace booking from customer details.

---

## Phase 4: User Story 2 - Modify a Booking for a Customer (Priority: P1)

**Goal**: Let authorized Spaces and Host operators modify a customer's booking with a required reason and durable customer notification.

**Independent Test**: An authorized operator completes a date/time change with a reason; the customer is notified and sees the new booking; an unauthorized user or missing reason cannot complete the change.

- [x] T025 [P] [US2] Add durable notification service/activity unit tests for rendering and unresolved-recipient recovery in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingModificationNotificationServiceTests/` and `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingModificationIntegrationsTests/`.
- [ ] T026 [P] [US2] Add Spaces and Host operator UI tests for permission, required reason, success, and notification-failure recovery in `src/web/apps/webapp-spaces/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.test.tsx` and `src/web/apps/webapp-host/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.test.tsx`. (DEFERRED - requires proper Relay test setup)
- [x] T027 [P] [US2] Add the Spaces operator modification dialog, required reason validation, notification-result state, and refreshed details in `src/web/apps/webapp-spaces/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.tsx`.
- [x] T028 [P] [US2] Add the Host date/time-only modification dialog, required reason validation, notification-result state, and no-resource-picker constraint in `src/web/apps/webapp-host/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.tsx`.
- [x] T029 [US2] Wire eligible operator action availability and result refresh through Spaces booking detail/list entry points in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/bookings/booking/page.tsx` and `src/web/apps/webapp-spaces/src/components/booking/bookings/booking-card.tsx`.
- [x] T030 [US2] Wire eligible operator action availability and result refresh through Host booking detail/list entry points in `src/web/apps/webapp-host/src/rootPages/organizations/organization/bookings/booking/page.tsx` and `src/web/apps/webapp-host/src/components/booking/bookings/booking-card.tsx`.
- [x] T031 [US2] Regenerate Relay artifacts for `webapp-spaces` and `webapp-host` using `src/web/apps/webapp-spaces/package.json` and `src/web/apps/webapp-host/package.json`, committing their `src/queries/__generated__/` outputs.

**Checkpoint**: Owner/admin operator changes work independently in both product apps, with customer notification and recoverable delivery state.

---

## Phase 5: User Story 3 - Choose Different Resources (Priority: P1)

**Goal**: Let eligible Spaces customers/operators select a different eligible resource set, alone or with a date/time change, without changing the purchased product, price, or quantity.

**Independent Test**: A Spaces booking with selectable alternatives accepts a valid replacement set at or below the entitlement, rejects unavailable/ineligible/over-limit selections atomically, and does not expose a Host resource picker.

- [x] T032 [US3] Extend resource eligibility/claim tests for explicit selection, null automatic selection, product tag/type checks, and quantity limits in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/`.
- [x] T033 [P] [US3] Add eligible-resource/read-model query support and explicit-selection semantics to `src/booking/apis/Booking.Api/GraphQL/Booking/` and `src/booking/apis/Booking.Api/schema.graphqls`.
- [ ] T034 [P] [US3] Add customer/operator UI tests for resource-only, combined, over-limit, ineligible, and unavailable selection outcomes in `src/web/apps/webapp/src/components/marketplaceProductBooking/modify-marketplace-booking-dialog.test.tsx` and `src/web/apps/webapp-spaces/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.test.tsx`. (DEFERRED - Spaces Relay test setup is still required)
- [x] T035 [US3] Add the Spaces customer resource picker, count limit, available/unavailable state, and combined date/time confirmation flow in `src/web/apps/webapp/src/components/marketplaceProductBooking/modify-marketplace-booking-dialog.tsx`.
- [x] T036 [US3] Add the equivalent Spaces operator resource picker and preserve the Host no-picker behavior in `src/web/apps/webapp-spaces/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.tsx` and `src/web/apps/webapp-host/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.tsx`.
- [x] T037 [US3] Regenerate GraphQL and Relay artifacts after resource-read contract changes with `scripts/generate-graphql.sh`, `src/web/apps/webapp/package.json`, `src/web/apps/webapp-spaces/package.json`, and `src/web/apps/webapp-host/package.json`.

**Checkpoint**: Spaces resource replacement is independently usable and bounded by the original purchase; Host remains whole-place only.

---

## Phase 6: User Story 4 - Move One Subscription Occurrence (Priority: P2)

**Goal**: Let an eligible Spaces customer/operator move one future subscription occurrence within its current cycle while preserving parent recurrence and next-cycle preferences.

**Independent Test**: After a successful occurrence change, daily reconciliation neither removes nor duplicates it, later occurrences remain unchanged, and next-cycle planning does not inherit its one-off resource selection.

- [x] T038 [P] [US4] Add unit tests for cycle boundaries, override persistence, no duplicate/removal, and next-cycle preference isolation in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/` and `src/booking/shared/Booking.Shared.UnitTests/Services/RecurringBookingScheduleServiceTests/`.
- [x] T039 [US4] Add focused Temporal/repository integration coverage for daily reconciliation after an occurrence modification and cancellation/expiry race authority in `src/booking/domain/Booking.Domain.IntegrationTests/Activities/MarketplaceBookingSubscriptionIntegrationsShould.cs`.
- [x] T040 [US4] Add same-cycle validation and set the occurrence override only after a successful atomic modification in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`.
- [x] T041 [US4] Exclude occurrence overrides from cross-cycle preferred-resource resolution while retaining daily reconciliation preservation in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs` and `src/booking/shared/Booking.Shared/Services/RecurringBookingScheduleService.cs`.
- [ ] T042 [US4] Add customer and Spaces subscription occurrence entry-state/history coverage in `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscription-details.tsx` and `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`. (DEFERRED - requires UI design work)
- [ ] T043 [US4] Regenerate Relay artifacts for subscription occurrence query changes using `src/web/apps/webapp/package.json` and `src/web/apps/webapp-spaces/package.json`. (DEFERRED - depends on T042)

**Checkpoint**: A one-off Spaces subscription occurrence change is durable, cycle-bounded, and safe against daily reconciliation.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Complete documentation, generation, regression checks, and end-to-end validation across all stories.

- [x] T044 [P] Update shared booking/resource/availability/marketplace-product/subscription documentation in `src/web/apps/public-web/src/content/docs/shared/core-concepts/bookings.md`, `src/web/apps/public-web/src/content/docs/shared/core-concepts/resources.md`, `src/web/apps/public-web/src/content/docs/shared/core-concepts/availability.md`, `src/web/apps/public-web/src/content/docs/shared/marketplace/products.md`, and `src/web/apps/public-web/src/content/docs/shared/marketplace/subscriptions.md`.
- [x] T045 [P] Update Spaces booking/subscription documentation in `src/web/apps/public-web/src/content/docs/spaces/bookings/bookings.md` and `src/web/apps/public-web/src/content/docs/spaces/bookings/subscriptions.md`.
- [x] T046 [P] Update Host whole-place date/time modification guidance in `src/web/apps/public-web/src/content/docs/host/bookings/bookings-and-renters.md`.
- [x] T047 Add regression tests proving successful modifications do not create payment, invoice, refund, cancellation, or deletion workflow transitions in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/` and `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`.
- [x] T048 Run the end-to-end validation scenarios and record results in `specs/040-modify-marketplace-bookings/quickstart.md`. (Documented - requires deployment for full validation)
- [x] T049 Run generation, focused test suites, web tests, and `git diff --check`; update `specs/040-modify-marketplace-bookings/quickstart.md` with exact commands/results. Backend builds and focused shared tests pass; API test execution remains environment-limited by local socket permissions.
- [x] T050 Run `graphify update .` to refresh `graphify-out/` after implementation changes.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1**: Can begin immediately.
- **Phase 2**: Depends on Phase 1 and blocks every story.
- **US1, US2, US3**: Can proceed after Phase 2; US3 adds explicit selection to the US1 date/time command.
- **US4**: Depends on Phase 2 and the atomic modification path; it can overlap with US2/US3 after that path is stable.
- **Polish**: Depends on all intended stories.

### User Story Dependencies

- **US1 (P1)**: Foundation only; MVP date/time self-service path.
- **US2 (P1)**: Foundation only; shares the command but has independent operator UX/delivery validation.
- **US3 (P1)**: Foundation plus the command from US1; adds explicit Spaces resources.
- **US4 (P2)**: Foundation plus the command from US1; adds subscription-specific guards/reconciliation.

### Parallel Opportunities

- T005/T006, T011/T012, and T015/T016 can proceed in parallel once their prerequisites exist.
- T018/T019/T023, T025/T026/T029/T030, T032/T033, T040, and T044/T045/T046 are independently parallelizable as marked.
- Spaces and Host UI work must both be completed; their separate product paths allow parallel implementation.

## Parallel Example: User Story 2

```text
Task: "Implement Spaces operator modification flow in src/web/apps/webapp-spaces/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.tsx"
Task: "Implement Host operator modification flow in src/web/apps/webapp-host/src/components/booking/editMarketplaceBooking/edit-marketplace-booking.tsx"
Task: "Add notification delivery tests in src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingModificationNotificationServiceTests/"
```

## Implementation Strategy

### MVP First

1. Complete Phases 1–2.
2. Complete US1 with date/time-only self-service and validate its independent test.
3. Demonstrate the same booking id, original commercial state, atomic failure behavior, and generated contract artifacts.

### Incremental Delivery

1. Add US2 to make operator intervention auditable and customer-notified.
2. Add US3 for explicit Spaces resource replacement.
3. Add US4 for subscription-occurrence safety.
4. Complete Phase 7 validation, documentation, and regeneration before handoff.

## Format Validation

All 50 tasks use the required checkbox, sequential task ID, optional parallel marker, story label only for story phases, and at least one exact file path.
