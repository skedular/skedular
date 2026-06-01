# Tasks: Customer Landing Cleanup

**Input**: Design documents from `specs/020-customer-landing-cleanup/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/), [quickstart.md](quickstart.md)

**Tests**: Included because the implementation plan and constitution require Vitest/React Testing Library coverage, regression validation, and logging verification for changed behavior.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files and has no dependency on incomplete tasks.
- **[Story]**: User story label from [spec.md](spec.md).
- All tasks include concrete repository paths.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the working inventory, test surfaces, and validation scaffolding shared by all stories.

- [x] T001 Create the initial route inventory document in `specs/020-customer-landing-cleanup/route-inventory.md` from `src/web/apps/webapp/src/app/**/page.tsx`, `src/web/apps/webapp/src/app/**/route.ts`, and `src/web/apps/webapp/src/rootPages/marketplace/**`.
- [x] T002 [P] Create the implementation notes document in `specs/020-customer-landing-cleanup/implementation-notes.md` with sections for GraphQL gaps, no-redirect decisions, owner-specific marketplace regression, and customer-owned data preservation.
- [x] T003 [P] Add a feature test checklist in `specs/020-customer-landing-cleanup/validation-checklist.md` covering the quickstart manual checks from `specs/020-customer-landing-cleanup/quickstart.md`.
- [x] T004 [P] Audit existing marketplace link helpers and record aggregate URL assumptions in `specs/020-customer-landing-cleanup/implementation-notes.md` using `src/web/apps/webapp/src/components/links/index.ts`.
- [x] T005 [P] Audit existing marketplace GraphQL/Relay coverage and record missing fields or no-op findings in `specs/020-customer-landing-cleanup/implementation-notes.md` using `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx`, `src/web/apps/webapp/src/components/location/marketplaceLocation/marketplace-location.tsx`, `src/web/apps/webapp/src/components/marketplaceProductBooking/`, and `src/web/apps/webapp/src/components/marketplaceProductSubscription/`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Shared code and verification utilities that MUST be complete before user story implementation.

**CRITICAL**: No user story work can begin until this phase is complete.

- [x] T006 Create aggregate marketplace telemetry helpers in `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.ts` for discovery, location selection, purchase hub load, self-service decisions, unsupported paths, and owner-specific entry resolution.
- [x] T007 [P] Add telemetry helper tests in `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.test.ts` verifying structured event names and safe properties from `specs/020-customer-landing-cleanup/contracts/observability.md`.
- [x] T008 Create customer-safe unsupported path UI in `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/marketplace-unsupported-path.tsx`.
- [x] T009 [P] Add unsupported path UI tests in `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/marketplace-unsupported-path.test.tsx` verifying customer-safe copy, no admin controls, and no redirect calls.
- [x] T010 Export unsupported path UI from `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/index.ts`.
- [x] T011 Create aggregate marketplace route ownership constants in `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.ts` using the keep/protect/remove categories from `specs/020-customer-landing-cleanup/contracts/route-map.md`.
- [x] T012 [P] Add route ownership tests in `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.test.ts` verifying marketplace/customer routes are kept, owner-specific behavior is protected, and private/admin routes are classified out of customer navigation.
- [x] T013 Review the webapp root entry behavior in `src/web/apps/webapp/src/app/page.tsx` and remove any feature-internal `window.location.replace` use that conflicts with the no-redirect rule, preserving sign-out return behavior only if it is classified in `specs/020-customer-landing-cleanup/implementation-notes.md` as a shared account exception.
- [x] T014 Update root entry resolver tests in `src/web/apps/webapp/src/app/customer-facing-subdomain/customer-facing-subdomain-resolver.test.ts` for no-subdomain aggregate marketplace and unchanged custom-subdomain owner-specific marketplace entry points.
- [x] T015 Update root page tests in `src/web/apps/webapp/src/app/page.test.tsx` to verify no-subdomain aggregate marketplace rendering, custom-subdomain owner-specific rendering, and no feature URL redirects.

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel.

---

## Phase 3: User Story 1 - Identify Webapp Responsibilities (Priority: P1) - MVP

**Goal**: Produce and approve a complete webapp capability inventory with owner app, disposition, rationale, customer impact, admin impact, customer-owned data risk, and no-redirect handling.

**Independent Test**: Review `specs/020-customer-landing-cleanup/route-inventory.md` and confirm every current webapp route or major workflow has exactly one disposition and one owner app.

### Tests for User Story 1

- [x] T016 [P] [US1] Add route inventory validation tests in `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.test.ts` asserting every route inventory owner is one of `webapp`, `webapp-teams`, `webapp-spaces`, `shared-entry-point`, or `undecided`.
- [x] T017 [P] [US1] Add no-redirect classification tests in `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.test.ts` asserting unsupported or removed paths use in-place handling and never redirect.

### Implementation for User Story 1

- [x] T018 [US1] Complete route and workflow inventory records in `specs/020-customer-landing-cleanup/route-inventory.md` for root, marketplace, custom-subdomain, private organization, MS Teams, resource, booking-management, subscription-management, admin, shared account, auth, callback, notification, upload, and integration routes under `src/web/apps/webapp/src/app/`.
- [x] T019 [US1] Classify webapp capability owner and disposition in `specs/020-customer-landing-cleanup/route-inventory.md` according to `specs/020-customer-landing-cleanup/contracts/capability-inventory.md`.
- [x] T020 [US1] Mark all customer-owned data risks in `specs/020-customer-landing-cleanup/route-inventory.md` for marketplace bookings, subscriptions, invoices, refunds, account state, and historical purchase links.
- [x] T021 [US1] Add no-redirect URL handling decisions for each removed, relocated, owner-specific, or unsupported route in `specs/020-customer-landing-cleanup/route-inventory.md`.
- [x] T022 [US1] Add stakeholder approval status and blocked/deferred decisions in `specs/020-customer-landing-cleanup/route-inventory.md` before any route removal implementation.
- [x] T023 [US1] Wire route ownership constants in `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.ts` to match the approved inventory records in `specs/020-customer-landing-cleanup/route-inventory.md`.
- [x] T024 [US1] Add `UnsupportedWebappPathHandled` telemetry usage in `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/marketplace-unsupported-path.tsx` through `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.ts`.

**Checkpoint**: User Story 1 is independently testable as the approved cleanup inventory and no-redirect route ownership baseline.

---

## Phase 4: User Story 2 - Simplify Webapp To Customer Discovery And Booking (Priority: P1)

**Goal**: Make no-subdomain webapp the aggregate marketplace discovery experience for marketplace-enabled customer-bookable locations while preserving owner-specific custom-subdomain marketplace behavior.

**Independent Test**: Open webapp with no custom subdomain and verify the first screen shows customer-facing location discovery, map/list browsing, and marketplace purchase entry points without private administration navigation.

### Tests for User Story 2

- [x] T025 [P] [US2] Add aggregate discovery filtering tests in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.test.tsx` for marketplace-enabled customer-bookable locations and empty states.
- [x] T026 [P] [US2] Add location card organization-context tests in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-location-card.test.tsx` for location name, organization context, address, image fallback, capacity, and map popup parity.
- [x] T027 [P] [US2] Add custom-subdomain regression tests in `src/web/apps/webapp/src/app/customer-facing-subdomain/co-working-subdomain.test.tsx` verifying owner-specific marketplace pages remain unchanged.
- [x] T028 [P] [US2] Add root aggregate marketplace tests in `src/web/apps/webapp/src/app/page.test.tsx` verifying no-subdomain webapp renders aggregate discovery and does not expose admin navigation.

### Implementation for User Story 2

- [x] T029 [US2] Update no-subdomain root behavior in `src/web/apps/webapp/src/app/page.tsx` so `public-discovery` renders the aggregate marketplace surface and logs `AggregateMarketplaceDiscoveryStarted` and `AggregateMarketplaceDiscoveryCompleted` through `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.ts`.
- [x] T030 [US2] Update aggregate marketplace query variables in `src/web/apps/webapp/src/rootPages/page.tsx` to request only marketplace-enabled customer-bookable locations using existing GraphQL filters where available.
- [x] T031 [US2] Update the Relay fragment in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx` to include only customer-facing fields needed for organization context, eligibility, map/list display, and practical insights.
- [x] T032 [US2] Update `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx` to render customer-safe empty state content when no eligible marketplace locations are available.
- [x] T033 [US2] Update `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-location-card.tsx` so aggregate list cards and map popup cards share compact card anatomy, organization context, stable media dimensions, and customer-facing purchase entry links.
- [x] T034 [US2] Update marketplace location detail entry in `src/web/apps/webapp/src/rootPages/marketplace/locations/location/page.tsx` to log `AggregateMarketplaceLocationSelected` when opened from aggregate webapp context.
- [x] T035 [US2] Preserve custom-subdomain owner-specific marketplace wrapping in `src/web/apps/webapp/src/app/page.tsx` and `src/web/apps/webapp/src/app/customer-facing-subdomain/co-working-subdomain.tsx` without changing existing owner-specific browse or purchase behavior.
- [x] T036 [US2] If Relay selections changed, regenerate web Relay artifacts from `src/web/apps/webapp` using `pnpm webapp#relay` or the repo generation path documented in `specs/020-customer-landing-cleanup/quickstart.md`.

**Checkpoint**: User Story 2 is independently testable as aggregate marketplace discovery plus unchanged custom-subdomain marketplace behavior.

---

## Phase 5: User Story 3 - View Customer Bookings Across Organizations (Priority: P2)

**Goal**: Provide a signed-in customer hub for marketplace bookings and subscriptions across organizations, including policy-bound cancel/change/refund actions.

**Independent Test**: Sign in as a customer with bookings or subscriptions in multiple organizations and confirm all relevant purchases are visible with eligible customer actions.

### Tests for User Story 3

- [x] T037 [P] [US3] Add bookings hub tests in `src/web/apps/webapp/src/components/organizationStoreFrontGuest/guest-store-front-bookings.test.tsx` for cross-organization booking context, empty state, and unauthenticated prompt.
- [x] T038 [P] [US3] Add subscriptions hub tests in `src/web/apps/webapp/src/components/organizationStoreFrontGuest/guest-store-front-subscriptions.test.tsx` for cross-organization subscription context, empty state, and unauthenticated prompt.
- [x] T039 [P] [US3] Add booking action eligibility tests in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.test.tsx` for cancel, change, refund, and unavailable action messaging.
- [x] T040 [P] [US3] Add subscription action eligibility tests in `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscription-details.test.tsx` for cancel, change, refund, and unavailable action messaging.

### Implementation for User Story 3

- [x] T041 [US3] Update customer bookings route wrapper in `src/web/apps/webapp/src/rootPages/marketplace/bookings/page.tsx` to present the no-subdomain cross-organization booking hub and log `CustomerPurchaseHubLoaded`.
- [x] T042 [US3] Update `src/web/apps/webapp/src/components/organizationStoreFrontGuest/guest-store-front-bookings.tsx` to show customer bookings across organizations with organization, location, product, schedule, payment state, booking status, and eligible actions.
- [x] T043 [US3] Update customer subscriptions route wrapper in `src/web/apps/webapp/src/rootPages/marketplace/subscriptions/page.tsx` to present the no-subdomain cross-organization subscription hub and log `CustomerPurchaseHubLoaded`.
- [x] T044 [US3] Update `src/web/apps/webapp/src/components/organizationStoreFrontGuest/guest-store-front-subscriptions.tsx` to show customer subscriptions across organizations with organization, location, product, renewal summary, payment state, subscription status, and eligible actions.
- [x] T045 [US3] Update `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.tsx` to expose customer cancel/change/refund actions only when marketplace booking policy allows them.
- [x] T046 [US3] Update `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscription-details.tsx` to expose customer cancel/change/refund actions only when marketplace subscription policy allows them.
- [x] T047 [US3] Add `CustomerSelfServiceActionStarted` and `CustomerSelfServiceActionRejected` logging through `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.ts` in booking and subscription detail action handlers.
- [x] T048 [US3] If GraphQL action eligibility fields or mutations are missing, update the owning GraphQL/domain source and document generation impact in `specs/020-customer-landing-cleanup/implementation-notes.md` before regenerating artifacts.
- [x] T049 [US3] If Relay selections changed, regenerate web Relay artifacts from `src/web/apps/webapp` using `pnpm webapp#relay` or the repo generation path documented in `specs/020-customer-landing-cleanup/quickstart.md`.

**Checkpoint**: User Story 3 is independently testable as the signed-in customer bookings/subscriptions self-service hub.

---

## Phase 6: User Story 4 - Remove Or Hide Obsolete Administration Functionality (Priority: P2)

**Goal**: Remove private/admin workflows from customer-facing webapp navigation and handle old paths in place without URL redirects.

**Independent Test**: Verify admin workflows classified as removable are absent from normal webapp customer navigation, and old links show customer-safe in-place states without redirects.

### Tests for User Story 4

- [x] T050 [P] [US4] Add customer navigation tests in `src/web/apps/webapp/src/components/rootShell/no-organization-root-shell.test.tsx` verifying private/admin navigation items are absent from customer webapp surfaces.
- [x] T051 [P] [US4] Add unsupported route tests in `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/marketplace-unsupported-path.test.tsx` for old admin paths, owner-specific paths opened in webapp, and no redirect calls.
- [x] T052 [P] [US4] Add route ownership regression tests in `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.test.ts` for webapp-teams and webapp-spaces owner classifications.

### Implementation for User Story 4

- [x] T053 [US4] Remove private organization and coworking-owner admin links from customer-facing shell/navigation in `src/web/apps/webapp/src/components/rootShell/no-organization-root-shell.tsx`.
- [x] T054 [US4] Remove or hide private organization booking/admin entry points from the no-subdomain customer path in `src/web/apps/webapp/src/app/page.tsx` using `src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.ts`.
- [x] T055 [US4] Implement in-place unsupported handling for removed webapp admin paths in `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/marketplace-unsupported-path.tsx`.
- [x] T056 [US4] Add route-level usage of unsupported path UI for removed or unsupported customer-facing webapp paths under `src/web/apps/webapp/src/app/marketplace/` without redirecting.
- [x] T057 [US4] Record each removed or hidden admin surface and preservation decision in `specs/020-customer-landing-cleanup/route-inventory.md` and `specs/020-customer-landing-cleanup/implementation-notes.md`.
- [x] T058 [US4] Add `UnsupportedWebappPathHandled` logging coverage in `src/web/apps/webapp/src/libs/logging/aggregate-marketplace-telemetry.test.ts` for removed admin paths and unsupported marketplace paths.

**Checkpoint**: User Story 4 is independently testable as cleaned customer navigation plus in-place no-redirect handling.

---

## Phase 7: User Story 5 - Establish A Product Direction For The New Landing Experience (Priority: P3)

**Goal**: Shape the no-subdomain webapp into a first-pitch aggregate marketplace experience inspired by Airbnb/gabel.to-style discovery while staying Skedular-specific and preserving owner-specific marketplaces.

**Independent Test**: Review the first-pitch experience against the product-direction checklist: discover locations, compare options, understand map context, see useful insights, and start marketplace booking.

### Tests for User Story 5

- [x] T059 [P] [US5] Add responsive aggregate discovery tests in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.test.tsx` for desktop and mobile layout states.
- [x] T060 [P] [US5] Add partial-data card tests in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-location-card.test.tsx` for missing image, missing insight, and missing map detail fallbacks.

### Implementation for User Story 5

- [x] T061 [US5] Refine aggregate discovery layout in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx` to prioritize map/list browsing, comparison, practical insights, and fast purchase entry without marketing-page hero content.
- [x] T062 [US5] Refine marketplace location card visuals in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-location-card.tsx` with stable media dimensions, compact details, organization context, and graceful fallbacks.
- [x] T063 [US5] Update location-level marketplace details in `src/web/apps/webapp/src/components/location/marketplaceLocation/marketplace-location.tsx` so aggregate-selected locations show product browsing and purchase entry comparable to owner-specific marketplace pages.
- [x] T064 [US5] Add first-pitch review notes in `specs/020-customer-landing-cleanup/validation-checklist.md` for discovery, comparison, map context, location insights, customer booking entry, and custom-subdomain regression.
- [x] T065 [US5] Verify all new or changed customer-facing copy uses American spelling in `src/web/apps/webapp/src/components/location/marketplaceLocations/`, `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/`, and `src/web/apps/webapp/src/rootPages/marketplace/`.

**Checkpoint**: User Story 5 is independently testable as the first-pitch product direction on top of the cleaned aggregate marketplace.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Final validation, generated artifact checks, documentation cleanup, and performance/accessibility review across stories.

- [x] T066 [P] Update `specs/020-customer-landing-cleanup/implementation-notes.md` with final before-and-after summary for removed navigation areas, relocated workflows, preserved custom-subdomain behavior, and remaining customer-facing responsibilities.
- [x] T067 [P] Update `specs/020-customer-landing-cleanup/validation-checklist.md` with completed evidence for SC-001 through SC-010 from `specs/020-customer-landing-cleanup/spec.md`.
- [x] T068 Run web tests from `src/web` with `pnpm webapp#test` and record results in `specs/020-customer-landing-cleanup/validation-checklist.md`.
- [x] T069 Run web lint from `src/web` with `pnpm webapp#lint` and record results in `specs/020-customer-landing-cleanup/validation-checklist.md`.
- [x] T070 Run web build from `src/web` with `pnpm webapp#build` and record results in `specs/020-customer-landing-cleanup/validation-checklist.md`.
- [x] T071 If backend GraphQL or generated artifacts changed, run `make generate` from the repo root and record generated outputs in `specs/020-customer-landing-cleanup/implementation-notes.md`.
- [x] T072 [P] Perform accessibility and keyboard review for aggregate marketplace cards, map/list interaction, unsupported path state, and customer self-service actions in `src/web/apps/webapp/src/components/location/marketplaceLocations/`, `src/web/apps/webapp/src/components/marketplaceUnsupportedPath/`, `src/web/apps/webapp/src/components/marketplaceProductBooking/`, and `src/web/apps/webapp/src/components/marketplaceProductSubscription/`.
- [x] T073 [P] Verify custom-subdomain owner-specific marketplace regression manually using `specs/020-customer-landing-cleanup/quickstart.md` and record the result in `specs/020-customer-landing-cleanup/validation-checklist.md`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion - blocks all user stories.
- **User Stories (Phase 3+)**: Depend on Foundational completion.
- **Polish (Phase 8)**: Depends on the desired user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Start after Foundational; this is the MVP because cleanup safety depends on the approved inventory.
- **User Story 2 (P1)**: Start after Foundational; can run alongside US1 only after route ownership constants and test scaffolding exist, but should consume US1 inventory before hiding/removing paths.
- **User Story 3 (P2)**: Start after Foundational; can proceed independently using existing marketplace booking/subscription surfaces.
- **User Story 4 (P2)**: Start after US1 approval because it removes or hides admin surfaces based on inventory decisions.
- **User Story 5 (P3)**: Start after US2 because it refines the aggregate marketplace surface.

### Within Each User Story

- Tests should be written before implementation tasks in that story.
- Inventory/model/contract updates precede code changes that depend on them.
- Relay query changes precede generated artifact regeneration.
- Logging hooks are implemented with the workflow they observe.
- Each story checkpoint should be validated before moving to lower-priority work.

### Parallel Opportunities

- Setup documentation tasks T002-T005 can run in parallel.
- Foundational test tasks T007, T009, T012, T014 can run in parallel with their paired implementation owners once file contracts are agreed.
- US1 test tasks T016-T017 can run in parallel.
- US2 tests T025-T028 can run in parallel.
- US3 tests T037-T040 can run in parallel.
- US4 tests T050-T052 can run in parallel.
- US5 tests T059-T060 can run in parallel.
- Polish evidence tasks T066-T067 and review tasks T072-T073 can run in parallel after implementation validation.

---

## Parallel Example: User Story 2

```text
Task: "T025 [P] [US2] Add aggregate discovery filtering tests in src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.test.tsx"
Task: "T026 [P] [US2] Add location card organization-context tests in src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-location-card.test.tsx"
Task: "T027 [P] [US2] Add custom-subdomain regression tests in src/web/apps/webapp/src/app/customer-facing-subdomain/co-working-subdomain.test.tsx"
Task: "T028 [P] [US2] Add root aggregate marketplace tests in src/web/apps/webapp/src/app/page.test.tsx"
```

## Parallel Example: User Story 3

```text
Task: "T037 [P] [US3] Add bookings hub tests in src/web/apps/webapp/src/components/organizationStoreFrontGuest/guest-store-front-bookings.test.tsx"
Task: "T038 [P] [US3] Add subscriptions hub tests in src/web/apps/webapp/src/components/organizationStoreFrontGuest/guest-store-front-subscriptions.test.tsx"
Task: "T039 [P] [US3] Add booking action eligibility tests in src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.test.tsx"
Task: "T040 [P] [US3] Add subscription action eligibility tests in src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscription-details.test.tsx"
```

## Parallel Example: User Story 4

```text
Task: "T050 [P] [US4] Add customer navigation tests in src/web/apps/webapp/src/components/rootShell/no-organization-root-shell.test.tsx"
Task: "T051 [P] [US4] Add unsupported route tests in src/web/apps/webapp/src/components/marketplaceUnsupportedPath/marketplace-unsupported-path.test.tsx"
Task: "T052 [P] [US4] Add route ownership regression tests in src/web/apps/webapp/src/app/aggregate-marketplace-route-ownership.test.ts"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 setup tasks T001-T005.
2. Complete Phase 2 foundational tasks T006-T015.
3. Complete Phase 3 User Story 1 tasks T016-T024.
4. Stop and validate the approved route/workflow inventory independently.
5. Use the approved inventory to decide which US2-US5 tasks can proceed safely.

### Incremental Delivery

1. Setup + Foundational -> shared telemetry, route ownership, unsupported UI, and root tests ready.
2. US1 -> route/workflow responsibility inventory approved.
3. US2 -> no-subdomain aggregate marketplace discovery and unchanged owner-specific marketplace behavior.
4. US3 -> customer booking/subscription hub and policy-bound self-service.
5. US4 -> private/admin navigation cleanup and in-place unsupported path handling.
6. US5 -> first-pitch product polish for aggregate discovery.
7. Polish -> validation, build/lint/test evidence, accessibility, regression notes.

### Parallel Team Strategy

- Developer A: US1 inventory and route ownership constants.
- Developer B: US2 aggregate marketplace discovery and card behavior.
- Developer C: US3 customer booking/subscription self-service.
- Developer D: US4 unsupported path handling and admin navigation cleanup after US1 approval.
- Designer/QA: US5 product-direction review and quickstart validation.

## Notes

- `[P]` tasks touch separate files or can be done independently after their phase starts.
- `[USx]` labels map directly to user stories in [spec.md](spec.md).
- Every task includes a concrete file path.
- Tests are included before implementation tasks for each story.
- Do not hand-edit generated Relay artifacts under `src/web/apps/webapp/src/queries/__generated__/`.
- Do not implement URL redirects from webapp in this phase.
- Preserve existing owner-specific custom-subdomain marketplace behavior.
- Use American spelling for user-facing and operator-facing copy.
