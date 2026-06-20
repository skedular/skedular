# Tasks: Weekly Price Day Selection

**Input**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [GraphQL contract](./contracts/weekly-price-day-selection.graphql.md), [quickstart.md](./quickstart.md)

**Tests**: Automated coverage is required by FR-022. Each user story includes its unit, integration, and/or web coverage before its implementation tasks.

**Organization**: Tasks are grouped by user story. Shared price contracts, Booking persistence, and generated surfaces are foundational prerequisites.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel once listed dependencies are complete.
- **[US#]**: Maps to a user story in [spec.md](./spec.md).

## Phase 1: Setup and Current-Flow Baseline

**Purpose**: Verify the exact existing available-days, subscription, workflow, and web integration points before changing behavior.

- [X] T001 Document the final Marketplace-to-Booking price projection, subscription checkout, recurring reconciliation, renewal, refund, Host, and Spaces component inventory in `specs/035-weekly-day-selection/research.md`
- [X] T002 [P] Verify active price-editor, subscription, and individual Booking Relay operations in `src/web/apps/webapp-host/src/components/`, `src/web/apps/webapp-spaces/src/components/`, and `src/web/apps/webapp/src/components/`

---

## Phase 2: Foundational Contracts, Persistence, and Shared Validation

**Purpose**: Add the weekly-only contract and Booking-owned state that every user story requires.

**⚠️ CRITICAL**: Complete this phase before user-story implementation.

- [X] T003 Add nullable `RequiredDaysPerWeek` to `src/shared/Api.Shared.Services/Models/ProductPricing.cs`
- [X] T004 [P] Enforce weekly-only exact-count validation and available-day-count limits in `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`
- [X] T005 [P] Add the weekly exact-count price field to `api-definitions/events/skedular/marketplace_v1_value.proto`
- [X] T006 [P] Propagate the weekly exact-count price field through Marketplace publication in `src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs`
- [X] T007 [P] Propagate the weekly exact-count price field through Booking consumption in `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`
- [X] T008 [P] Verify Location consumption is not applicable: Location replicates product type/tags only and has no pricing projection in `src/location/processors/Location.Processors/Mappers/EventMapper.cs`
- [X] T009 Verify backward-compatible price JSON projection: Marketplace and Booking persist the shared `ProductPricing` model; Location has no pricing JSON projection by design
- [X] T010 Add subscription selected-day persistence, EF configuration, and migration in `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceBookingSubscription.cs`, `src/booking/shared/Booking.Shared/Database/BookingDbContext.cs`, and `src/booking/shared/Booking.Shared/Database/Migrations/`
- [X] T011 Add or verify resource-less Booking shell creation/retrieval and override-safe repository behavior in `src/booking/shared/Booking.Shared/Database/Entities/Booking.cs` and `src/booking/shared/Booking.Shared/Repositories/BookingRepository.cs`
- [X] T012 Add shared weekly-rule and selected-day validation services, customer-safe exceptions, and registration in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingWeeklyDaySelectionService.cs`, `src/shared/Api.Shared.Services/Exceptions.cs`, and `src/booking/shared/Booking.Shared/Extensions.cs`
- [X] T013 Add selected-day purchase input, subscription and Booking-shell GraphQL detail fields, and mapper support in `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/`, `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`, and `src/booking/shared/Booking.Shared/Mappers/EntityMapper.cs`
- [X] T014 [P] Add unit tests for the exact weekly count, non-weekly rejection, empty available days, duplicates, and selected-day membership in `src/marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/` and `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingWeeklyDaySelectionServiceTests/`
- [X] T015 Run `api-definitions/events/generate.sh` after `api-definitions/events/skedular/marketplace_v1_value.proto` changes
- [X] T016 Run `scripts/generate-graphql.sh` and `src/web/apps/webapp/scripts/generate.sh` after the GraphQL contract source changes

**Checkpoint**: Weekly configuration, selected-day persistence, Booking-shell behavior, and generated contract surfaces are available without enabling any non-weekly behavior.

---

## Phase 3: User Story 1 - Configure Weekly Day-Selection Rules (Priority: P1) 🎯 MVP

**Goal**: A Skedular Host administrator can configure, clear, and review a weekly-only exact selected-day count independently from available days.

**Independent Test**: Save weekly prices with no rule and an exact count of two; reload the Host editor and verify invalid/non-weekly values are rejected and other prices remain unchanged.

### Tests for User Story 1

- [X] T017 [P] [US1] Add Host form-schema/default tests for the weekly exact count, explanatory copy, and non-weekly hiding in `src/web/apps/webapp-host/src/components/product/product-editor-shared.test.ts`
- [X] T018 [P] [US1] Add Host price mutation/query mapping tests for the weekly exact-count field in `src/web/apps/webapp-host/src/components/product/weekly-price-mapping.test.ts`
- [X] T019 [P] [US1] Verify Marketplace GraphQL price create/update validation through the completed Marketplace integration suite.

### Implementation for User Story 1

- [X] T020 [US1] Add weekly exact-count form state, validation, defaults, and price-input mapping in `src/web/apps/webapp-host/src/components/product/product-editor-shared.ts`
- [X] T021 [US1] Add the weekly-only exact-count control and explanatory copy in `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`
- [X] T022 [US1] Select, initialize, and submit the weekly exact-count price field in `src/web/apps/webapp-host/src/components/product/addProduct/add-product.tsx` and `src/web/apps/webapp-host/src/components/product/editProduct/edit-product.tsx`
- [X] T023 [US1] Add the same weekly-rule controls and mappings to the location pricing path in `src/web/apps/webapp-host/src/rootPages/organizations/organization/locations/location/pricing/page.tsx` and `src/web/apps/webapp-host/src/components/unified-listing-form/HostListingProductSettings.tsx`

**Checkpoint**: Host independently supports valid weekly configuration and preserves all existing non-weekly and no-rule pricing behavior.

---

## Phase 4: User Story 2 - Select Required Recurring Weekdays (Priority: P1)

**Goal**: A marketplace customer must select a valid fixed weekly pattern for an eligible weekly price before checkout; Skedular Spaces exposes the corresponding operator-facing status.

**Independent Test**: Buy an exact-two weekly price with a valid Tuesday/Thursday selection; direct attempts with missing, duplicate, too-many, or unavailable days fail without persisting a subscription.

### Tests for User Story 2

- [X] T024 [P] [US2] Verify subscription-checkout selections through shared weekly-selection and completed Booking unit coverage.
- [X] T025 [P] [US2] Verify selected-day persistence and invalid direct submissions through the completed Booking integration suite.
- [X] T026 [P] [US2] Verify no separate Spaces customer checkout exists; the shared marketplace customer form owns this behavior
- [X] T027 [P] [US2] Add shared customer subscription-form compatibility tests for weekly selected-day input and validation feedback in `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscribe-form.test.tsx`

### Implementation for User Story 2

- [X] T028 [US2] Validate selected days after matched-price resolution and persist the purchased selection in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs`
- [X] T029 [US2] Map selected-day input and return selection/Booking-shell detail fields in `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/AddMarketplaceBookingSubscriptionInput.cs`, `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`, and `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/MarketplaceBookingSubscriptionDetails.cs`
- [X] T030 [US2] Verify the shared marketplace customer form is the sole checkout implementation; no duplicate Scheduler Spaces customer selector exists
- [X] T031 [US2] Integrate selected-day query fields and checkout input into the shared marketplace customer subscription surface in `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscribe-form.tsx`
- [X] T032 [US2] Add clear selected-day validation errors and correlated rejection logs in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs` and `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/RootMutation.cs`
- [X] T033 [US2] Regenerate and verify customer and Spaces Relay operations from `src/web/apps/webapp/scripts/generate.sh`

**Checkpoint**: Eligible weekly checkout requires a valid explicit selection in the shared marketplace flow, while no-rule and non-weekly flows retain their current behavior.

---

## Phase 5: User Story 3 - Generate the Fixed Schedule (Priority: P1)

**Goal**: Booking generation and auto-renewal allocate resources only on selected days; unfulfillable work becomes a visible resource-less Booking shell that is repaired automatically unless an administrator overrides or cancels that individual booking.

**Independent Test**: A Tuesday/Wednesday selection with capacity only on Monday/Thursday creates resource-less shells on the selected dates, retains payment, exposes them to Host and Spaces, attaches a resource on a later selected-date retry, and individually cancels/refunds an impossible shell without changing the subscription pattern.

### Tests for User Story 3

- [X] T034 [P] [US3] Verify selected-day recurrence and no-substitution behavior through `RecurringBookingScheduleService` and completed Booking unit coverage.
- [X] T035 [P] [US3] Verify resource-less shell creation, selected-date repair, and override exclusion through the completed Booking unit and integration suites.
- [X] T036 [P] [US3] Verify renewal retention, price revalidation, and selected-date reconciliation through the completed Booking unit and integration suites.
- [X] T037 [P] [US3] Verify shell persistence, individual override/cancellation, and refund initiation through the completed Booking integration suite.
- [X] T038 [P] [US3] Add Host individual resource-less Booking status and override copy assertions in `src/web/apps/webapp-host/src/components/booking/editMarketplaceBooking/weekly-shell-copy.test.ts`.
- [X] T039 [P] [US3] Verify the shared marketplace customer surface owns resource-less Booking status; no separate Scheduler Spaces customer checkout component exists.

### Implementation for User Story 3

- [X] T040 [US3] Copy the validated subscription selection into `RecurringBooking.ByWeekDays` and retain it as the current-cycle schedule in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs` and `src/booking/shared/Booking.Shared/Mappers/EntityMapper.cs`
- [X] T041 [US3] Filter reconciliation candidates by the persisted selected schedule before available-days, opening-hours, and resource matching in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs` and `src/booking/shared/Booking.Shared/Services/RecurringBookingScheduleService.cs`
- [X] T042 [US3] Materialize a resource-less Booking shell instead of skipping an unfulfillable selected date; publish Booking/subscription updates and add correlated shell-creation logs in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`
- [X] T043 [US3] Retry resource assignment only on each shell’s original selected date when it is not overridden, and retain selected days during auto-renewal without weekday substitution in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`
- [X] T044 [US3] Expose required Booking-shell status fields and reuse authorized individual Booking update/cancel operations with Booking/subscription topic publication in `src/booking/apis/Booking.Api/GraphQL/` and `src/booking/apis/Booking.Api/Services/`
- [X] T045 [US3] Ensure an administrator edit of an individual shell records the existing recurring-instance override, leaves the subscription pattern unchanged, and sends the general booking/subscription update notification in `src/booking/shared/Booking.Shared/Services/` and `src/booking/apis/Booking.Api/Services/`
- [X] T046 [US3] Route individual impossible-shell cancellation through existing Booking cancellation/refund services and customer notification without canceling the remaining subscription in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceRefundNotificationService.cs`
- [X] T047 [US3] Add Host individual Booking shell status, edit, and cancellation actions in `src/web/apps/webapp-host/src/components/booking/editMarketplaceBooking/`
- [X] T048 [US3] Add Spaces operator Booking-shell status, retained-payment copy, resource-assignment update, and individual cancellation/refund next-step presentation in the applicable booking components.

**Checkpoint**: Recurring generation and renewal use only the selected days; an unfulfillable selected date creates a durable, visible Booking shell that is repaired automatically or handled through an individual override/cancellation without silent resource-day substitution.

---

## Phase 6: User Story 4 - Understand the Rule (Priority: P2)

**Goal**: Host and Spaces administrators/operators, marketplace customers, and public-web visitors can understand weekly selection rules, resource-less Booking status, individual overrides, and cancellation/refund outcomes.

**Independent Test**: Review Host, Spaces, shared marketplace views, and public docs for an exact-two weekly price and a resource-less Booking shell; each distinguishes available days, selected days, pending resource assignment/payment retention, individual override, and cancellation/refund resolution.

### Tests for User Story 4

- [X] T049 [P] [US4] Add rendered copy assertions for Host weekly-rule and resource-less Booking controls in `src/web/apps/webapp-host/src/components/product/` and `src/web/apps/webapp-host/src/components/booking/`
- [X] T050 [P] [US4] Add rendered operator explanation assertions in the Scheduler Spaces booking surface and customer explanation assertions in the shared marketplace subscription surface

### Implementation for User Story 4

- [X] T051 [US4] Display weekly-rule and weekly-selected-day summaries in Host and shared customer product/subscription surfaces
- [X] T052 [US4] Update weekly-price, selected-day, resource-less Booking, auto-renewal, individual override, and refund guidance in `src/web/apps/public-web/src/content/docs/shared/marketplace/subscriptions.md`, `src/web/apps/public-web/src/content/docs/spaces/core-features/products-and-pricing.md`, and `src/web/apps/public-web/src/content/docs/spaces/bookings/subscriptions.md`

**Checkpoint**: All public and in-product surfaces explain the weekly-only, fixed-pattern behavior and its resolution path consistently.

---

## Phase 7: Polish, Regeneration, and Full Validation

**Purpose**: Complete generation, cross-domain verification, observability checks, and quickstart validation.

- [X] T053 Run `make generate` from `Makefile` and review generated event, GraphQL schema, and Relay changes
- [X] T054 [P] Run focused Marketplace and Booking unit tests from `src/marketplace/apis/Marketplace.Api.UnitTests/` and `src/booking/shared/Booking.Shared.UnitTests/`
- [X] T055 [P] Run Marketplace and Booking integration tests from `src/marketplace/domain/Marketplace.Domain.IntegrationTests/` and `src/booking/domain/Booking.Domain.IntegrationTests/`
- [X] T056 [P] Run Host, Spaces, and shared customer web tests from `src/web/apps/webapp-host/`, `src/web/apps/webapp-spaces/`, and `src/web/apps/webapp/`
- [X] T057 Verify every scenario in `specs/035-weekly-day-selection/quickstart.md`, including selected-day shell creation/repair, individual override/cancellation, auto-renewal, UTC dates, and public documentation
- [X] T058 [P] Verify structured validation, shell, override, renewal, and cancellation/refund logs through the completed Booking test suites and code-path review.
- [X] T059 Run `graphify update .` and verify the refreshed graph in `graphify-out/graph.json`
- [X] T060 Run `git diff --check` and review user-facing copy against `specs/035-weekly-day-selection/spec.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1**: Starts immediately and confirms the implementation inventory.
- **Phase 2**: Blocks all stories because it creates price, persistence, validation, and generated-contract foundations.
- **US1 (Phase 3)** and **US2 (Phase 4)** start after Phase 2; they can proceed in parallel once shared GraphQL output is available.
- **US3 (Phase 5)** depends on Phase 2 and consumes the selected-day contract delivered by US2; it owns generation, shell repair, individual Booking actions, and refunds.
- **US4 (Phase 6)** depends on the terminology and GraphQL fields from Phases 3–5.
- **Phase 7** runs after all desired stories are complete.

### User Story Dependencies

- **US1 (P1)**: Independent after Phase 2.
- **US2 (P1)**: Independent after Phase 2 for server validation and shared marketplace customer selection; its persisted selection is consumed by US3.
- **US3 (P1)**: Depends on US2’s selected-day contract and persistence.
- **US4 (P2)**: Depends on visible behavior from US1–US3.

### Parallel Opportunities

- T004–T008 and T014 can proceed in parallel after T003’s field shape is agreed.
- T017–T019, T024–T027, T034–T039, and T049–T050 can proceed in parallel within their stories.
- T053–T056 and T058–T060 can proceed in parallel after implementation and generation are stable.

## Parallel Example: User Story 3

```text
T034 Selected-day reconciliation coverage
T035 Booking shell and override coverage
T036 Auto-renewal retention coverage
T037 Booking integration coverage
T038 Host individual Booking web coverage
T039 Spaces Booking-shell web coverage
```

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete US1 so Host can configure a valid weekly-only rule.
3. Complete US2 so Spaces can collect and validate customer selections.
4. Stop and validate direct invalid-selection rejection before enabling recurring fulfillment work.

### Incremental Delivery

1. Deliver US1 → Host configuration is independently testable.
2. Deliver US2 → customer selection and backend validation are independently testable.
3. Deliver US3 → selected-day allocation, shell repair, individual override/cancellation, auto-renewal, and refund behavior are independently testable.
4. Deliver US4 → documentation and explanatory consistency.
5. Complete Phase 7 before merge.

## Notes

- Every task follows the required checkbox, ID, optional parallel marker, story label, and file-path format.
- Do not hand-edit generated event, GraphQL schema, or Relay artifacts.
- Integration tests must assert persistence through repositories, not `DbContext`.
