# Tasks: Product Price Available Days

**Input**: Design documents from `/specs/034-price-available-days/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contract](./contracts/product-pricing-available-days.md), [quickstart.md](./quickstart.md)

**Tests**: Automated coverage is explicitly required by FR-017; each story includes its relevant unit, integration, and web tests.

**Organization**: Tasks are grouped by user story. Shared contract, replication, and time-zone prerequisites precede story work.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel after its listed dependencies are complete.
- **[US#]**: User story label.

## Phase 1: Setup and Current-Flow Baseline

**Purpose**: Confirm the active code paths and document the baseline before changing behavior.

- [X] T001 Document the price-to-booking-to-renewal flow and affected component inventory in `specs/034-price-available-days/research.md`
- [X] T002 [P] Verify all active Host price-editor routes and their Relay operations in `src/web/apps/webapp-host/src/components/product/` and `src/web/apps/webapp-host/src/rootPages/organizations/organization/locations/location/pricing/`
- [X] T003 [P] Verify all customer booking/subscription and Spaces price-consumer routes in `src/web/apps/webapp/src/components/marketplaceProductBooking/`, `src/web/apps/webapp/src/components/marketplaceProductSubscription/`, and `src/web/apps/webapp-spaces/src/components/product/`

---

## Phase 2: Foundational Contracts, Projections, and Shared Eligibility

**Purpose**: Add the backward-compatible price rule and the shared backend behavior required by every story.

**⚠️ CRITICAL**: Complete this phase before implementing any user-story surface.

- [X] T004 Add the optional all-calendar-day `AvailableDays` collection and empty-list default to `src/shared/Api.Shared.Services/Models/ProductPricing.cs`
- [X] T005 [P] Add supported-value and duplicate validation for `AvailableDays` to `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`
- [X] T006 [P] Extend ProductPricing protobuf serialization with available days in `api-definitions/events/skedular/marketplace_v1_value.proto`
- [X] T007 [P] Propagate available days through Marketplace event publication in `src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs`
- [X] T008 [P] Propagate available days through Booking event consumption in `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`
- [ ] T009 [P] Propagate available days through the Location product-version event mapper in `src/location/processors/Location.Processors/Mappers/EventMapper.cs`
- [ ] T010 Add backward-compatible JSONB persistence/projection mapping for ProductPricing available days in `src/marketplace/shared/Marketplace.Shared/Database/Entities/ProductVersion.cs`, `src/booking/shared/Booking.Shared/Database/Entities/ProductVersion.cs`, and `src/location/shared/Location.Shared/Database/Entities/ProductVersion.cs`
- [X] T011 Add a shared Booking available-day eligibility service that resolves the applicable location-local booking start day using `src/shared/Api.Shared.Services/Models/DayOfWeek.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingAvailableDaysService.cs`
- [X] T012 Add customer-safe unavailable-day exception/error mapping and structured rejection logging in `src/shared/Api.Shared.Services/Exceptions.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`
- [ ] T013 Add unit coverage for ProductPricing available-day validation and empty-list compatibility in `src/marketplace/apis/Marketplace.Api.UnitTests/Services/ProductServiceTests/`
- [ ] T014 Add integration coverage for GraphQL product create/update persistence and projection of available days in `src/marketplace/domain/Marketplace.Domain.IntegrationTests/Api/GraphQL/UpdateProductPatchSaveShould.cs`
- [ ] T015 Run `api-definitions/events/generate.sh` to regenerate protobuf event outputs after T006
- [X] T016 Run `scripts/generate-graphql.sh` to regenerate API schemas and GraphQL test surfaces after T004–T012

**Checkpoint**: Product prices carry an unrestricted-by-default calendar-day rule through all required domain projections, and backend callers have one local-calendar eligibility rule.

---

## Phase 3: User Story 1 - Configure a Price's Available Days (Priority: P1) 🎯 MVP

**Goal**: An administrator can set, clear, and review all-seven-day available-day rules independently for each price in Skedular Host.

**Independent Test**: Save unrestricted, Saturday-only, and Wednesday-plus-Thursday prices in every active Host editor and reload them with their distinct selections intact.

### Tests for User Story 1

- [ ] T017 [P] [US1] Add Host form default/schema tests for empty, single-day, and multi-day price selections in `src/web/apps/webapp-host/src/components/product/product-editor-shared.ts`
- [ ] T018 [P] [US1] Extend Host product autosave query/mutation mapping coverage in `src/web/apps/webapp-host/src/components/product/editProduct/edit-product-autosave.test.ts`
- [ ] T019 [P] [US1] Add location-pricing editor mapping coverage in `src/web/apps/webapp-host/src/rootPages/organizations/organization/locations/location/pricing/`

### Implementation for User Story 1

- [X] T020 [US1] Add `availableDays` form state, defaults, and validation to `src/web/apps/webapp-host/src/components/product/product-editor-shared.ts`
- [X] T021 [US1] Add an equal-weight Sunday-through-Saturday multiselect with “Every day” empty-state help to `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`
- [X] T022 [US1] Include available days in standard Host add/edit fragments, initial values, and mutation normalization in `src/web/apps/webapp-host/src/components/product/addProduct/add-product.tsx` and `src/web/apps/webapp-host/src/components/product/editProduct/edit-product.tsx`
- [X] T023 [US1] Add the same available-days form state and selector to the Host location-pricing flow in `src/web/apps/webapp-host/src/rootPages/organizations/organization/locations/location/pricing/page.tsx` and `src/web/apps/webapp-host/src/components/unified-listing-form/HostListingProductSettings.tsx`
- [X] T024 [US1] Regenerate Relay types for Host price-editor operations with `src/web/apps/webapp/scripts/generate.sh`

**Checkpoint**: Host administrators can configure or clear per-price available days without affecting other prices.

---

## Phase 4: User Story 2 - Find and Buy a Valid Date (Priority: P1)

**Goal**: Customers can see a price's allowed calendar days, are guided away from invalid dates, and cannot bypass server-side validation.

**Independent Test**: For a Saturday-only price, valid Saturday booking succeeds when resources are available; a direct non-Saturday booking is rejected before resource allocation; an unrestricted price retains existing behavior.

### Tests for User Story 2

- [ ] T025 [P] [US2] Add one-time marketplace booking service tests for unrestricted, allowed, disallowed, part-day, and location-timezone boundary cases in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/MarketplaceBookingServiceShould.cs`
- [ ] T026 [P] [US2] Add subscription-checkout available-day validation tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/MarketplaceBookingSubscriptionServiceShould.cs`
- [ ] T027 [P] [US2] Add Booking GraphQL integration tests using repository assertions for disallowed direct booking and empty-list compatibility in `src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/`
- [ ] T028 [P] [US2] Add customer booking and subscription UI tests for available-day display and invalid-date disabling in `src/web/apps/webapp/src/components/marketplaceProductBooking/` and `src/web/apps/webapp/src/components/marketplaceProductSubscription/`

### Implementation for User Story 2

- [X] T029 [US2] Enforce the shared location-local available-day rule after price resolution and before resource allocation in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`
- [ ] T030 [US2] Enforce the same rule after price resolution and before requested-resource or checkout processing in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs`
- [X] T031 [US2] Add structured available-day rejection logs with booking/request correlation and local-date context in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs`
- [X] T032 [US2] Select and present available days, disable disallowed dates where practical, and preserve selected start times in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-form.tsx`
- [X] T033 [US2] Select and present available days, disable disallowed subscription start dates where practical, and preserve existing cadence selection in `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscribe-form.tsx`
- [X] T034 [US2] Update the mirrored Skedular Spaces product editor form, add/edit mapping, and Relay selections in `src/web/apps/webapp-spaces/src/components/product/`
- [X] T035 [US2] Regenerate Relay types for customer and Spaces operations with `src/web/apps/webapp/scripts/generate.sh`

**Checkpoint**: Customer-facing paths make the rule understandable and the backend rejects every invalid direct request.

---

## Phase 5: User Story 3 - Generate Restricted Recurring Entitlement (Priority: P1)

**Goal**: Recurring and multi-day prices generate instances only on their permitted local calendar days, preserve purchased-period rules, and adopt edits on renewal.

**Independent Test**: A six-month Saturday-only subscription creates only Saturday instances; Wednesday-plus-Thursday creates only those days; resource-unavailable eligible days remain unbooked; renewal applies the new rule only to the new period.

### Tests for User Story 3

- [ ] T036 [P] [US3] Add reconciliation tests for empty, single-day, and multi-day available-day rules and unavailable resources in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`
- [ ] T037 [P] [US3] Add six-month Saturday-only, timezone-boundary, and current-period snapshot/renewal tests in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`
- [X] T038 [P] [US3] Add ProductPricing matcher regression tests ensuring available days do not alter fallback identity matching in `src/booking/shared/Booking.Shared.UnitTests/Services/ProductVersionHelperServiceTests/`

### Implementation for User Story 3

- [X] T039 [US3] Filter disallowed local calendar candidates before opening-hours/resource planning and booking materialization in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`
- [ ] T040 [US3] Apply the shared local-day resolver consistently to subscription scheduling/reconciliation paths in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs` and `src/booking/shared/Booking.Shared/Services/RecurringBookingScheduleService.cs`
- [ ] T041 [US3] Preserve the copied current-period price rule while ensuring renewal reloads the latest available-days value in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs` and `src/booking/shared/Booking.Shared/Services/ProductVersionHelperService.cs`
- [X] T042 [US3] Add structured logs for skipped recurring candidates and renewal rule selection in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`

**Checkpoint**: Restricted recurring booking generation and renewal meet the six-month Saturday-only and multi-day acceptance scenarios without changing resource-availability behavior.

---

## Phase 6: User Story 4 - Understand Day-Restricted Products (Priority: P2)

**Goal**: Administrators and customers can understand the optional available-day rule across product and public documentation surfaces.

**Independent Test**: Review Host, Spaces/customer views, and public documentation for a multi-day rule; each says Sunday–Saturday are equal, empty means every day, resources still govern actual availability, and renewal applies current pricing only to the new period.

### Tests for User Story 4

- [ ] T043 [P] [US4] Add rendered Host and Spaces form assertions for the “Available days” and “Every day” explanatory copy in `src/web/apps/webapp-host/src/components/product/` and `src/web/apps/webapp-spaces/src/components/product/`
- [ ] T044 [P] [US4] Add customer product-detail and checkout display assertions in `src/web/apps/webapp/src/components/marketplaceProductGuest/` and `src/web/apps/webapp/src/components/marketplaceProductCard/`

### Implementation for User Story 4

- [X] T045 [US4] Display the selected available days in customer product cards and booking detail surfaces in `src/web/apps/webapp/src/components/marketplaceProductGuest/marketplace-product-detail-booking-card.tsx` and `src/web/apps/webapp/src/components/marketplaceProductCard/marketplace-product-card.tsx`
- [X] T046 [P] [US4] Update Host pricing documentation in `src/web/apps/public-web/src/content/docs/host/core-features/pricing.md`
- [X] T047 [P] [US4] Update Spaces product, booking, and subscription documentation in `src/web/apps/public-web/src/content/docs/spaces/core-features/products-and-pricing.md`, `src/web/apps/public-web/src/content/docs/spaces/bookings/bookings.md`, and `src/web/apps/public-web/src/content/docs/spaces/bookings/subscriptions.md`
- [X] T048 [P] [US4] Update shared subscription and availability documentation in `src/web/apps/public-web/src/content/docs/shared/marketplace/subscriptions.md` and `src/web/apps/public-web/src/content/docs/shared/core-concepts/availability.md`

**Checkpoint**: All user-facing surfaces consistently explain calendar-day availability without conflating it with resource availability.

---

## Phase 7: Polish, Regeneration, and Full Validation

**Purpose**: Validate contract consistency, generated artifacts, logs, and end-to-end behavior across all stories.

- [ ] T049 Run the generation workflow defined by `./Makefile` and review all generated event, GraphQL schema, OpenAPI, and Relay changes
- [X] T050 [P] Run affected Marketplace and Booking unit tests from `src/marketplace/apis/Marketplace.Api.UnitTests/` and `src/booking/shared/Booking.Shared.UnitTests/`
- [ ] T051 [P] Run affected Marketplace and Booking integration tests from `src/marketplace/domain/Marketplace.Domain.IntegrationTests/` and `src/booking/domain/Booking.Domain.IntegrationTests/`
- [X] T052 [P] Run affected Host, Spaces, and customer web tests from `src/web/apps/webapp-host/`, `src/web/apps/webapp-spaces/`, and `src/web/apps/webapp/`
- [ ] T053 Verify the quickstart scenarios and generated-artifact consistency in `specs/034-price-available-days/quickstart.md`
- [X] T054 Run `graphify update .` and verify the refreshed knowledge graph in `graphify-out/graph.json`
- [X] T055 Run `git diff --check` and review user-facing copy against `specs/034-price-available-days/spec.md`
- [ ] T056 Add Booking integration coverage for restricted recurring generation and renewal, asserting persisted outcomes through repositories in `src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/ResourceAvailabilitySubscriptionShould.cs`
- [ ] T057 Add runtime logging assertions for disallowed one-time booking and subscription rejection in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/MarketplaceBookingServiceShould.cs` and `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/MarketplaceBookingSubscriptionServiceShould.cs`
- [ ] T058 Add runtime logging assertions for skipped recurring candidates and renewal-rule selection in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 establishes the verified path inventory.
- Phase 2 blocks all user stories because ProductPricing, projection, validation, and generated-contract work are shared.
- US1, US2, and US3 depend on Phase 2. US1 can start independently; US2 and US3 consume the same contract but can proceed in parallel after its completion.
- US4 depends on generated contract selections from Phase 2 and customer display work from US2.
- Phase 7 runs after the selected stories are complete.

### User Story Dependencies

- **US1 (P1)**: Complete after Phase 2; no dependency on other stories.
- **US2 (P1)**: Complete after Phase 2; may proceed alongside US1 after shared Relay regeneration.
- **US3 (P1)**: Complete after Phase 2; may proceed alongside US1 and US2 after shared eligibility helper is available.
- **US4 (P2)**: Depends on the published field from Phase 2 and customer display from US2.

### Parallel Opportunities

- T005–T009 and T013–T014 can run in parallel once T004 is agreed.
- T017–T019, T025–T028, T036–T038, and T043–T044 can run in parallel within their stories.
- T046–T048 can run in parallel after terminology and behavior are finalized.
- T050–T052 can run in parallel after generation and implementation are complete.

## Parallel Example: User Story 3

```text
T036 Reconciliation coverage for allowed and unavailable days
T037 Six-month, timezone, snapshot, and renewal coverage
T038 ProductPricing matcher regression coverage
```

## Implementation Strategy

### MVP First

1. Complete Phases 1 and 2.
2. Deliver US1: Host configuration and persisted/retrievable rule.
3. Validate unrestricted, Saturday-only, and two-day price configuration independently.

### Incremental Delivery

1. Add US2 to make the rule visible and authoritative during purchase.
2. Add US3 to apply it to recurring generation and renewal.
3. Add US4 documentation and explanatory polish.
4. Finish with Phase 7 regeneration and end-to-end validation.
