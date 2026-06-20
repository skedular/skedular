# Tasks: Unified Host Listing Experience

**Input**: Design documents from `/specs/032-unified-host-listing/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Vitest + React Testing Library for frontend; focused route and mutation-coordination validation

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and cleanup of speculative work before the real unified flow is built

- [x] T001 Audit and consolidate speculative unified listing scaffolding in `src/web/apps/webapp-host/src/components/unified-listing-form/index.ts`
- [x] T002 [P] Review and align speculative unified listing query work in `src/web/apps/webapp-host/src/queries/getListing.graphql.ts`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

**ARCHITECTURE NOTE**: Use existing Location and Product GraphQL APIs directly. Keep orchestration in the frontend. Add only the minimal readiness surface required for asynchronous hidden Product availability.

- [x] T003 Create the unified listing location-plus-product query surface in `src/web/apps/webapp-host/src/queries/hostListingQuery.ts`
- [x] T004 [P] Create the linked-product readiness watcher for a location in `src/web/apps/webapp-host/src/queries/hostListingProductReadiness.ts`
- [x] T005 [P] Create shared unified listing orchestration state in `src/web/apps/webapp-host/src/components/unified-listing-form/useHostListingCoordinator.ts`
- [x] T006 [P] Create the shared pending/ready shell for unified listing screens in `src/web/apps/webapp-host/src/components/unified-listing-form/HostListingShell.tsx`
- [x] T007 [P] Create product-to-location lookup support for legacy route redirects in `src/web/apps/webapp-host/src/queries/hostProductLocationLookup.ts`
- [x] T008 Map and prepare legacy Host product routes for redirect into the listing flow in `src/web/apps/webapp-host/src/app/products/page.tsx`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Create a New Listing as a Host (Priority: P1) 🎯 MVP

**Goal**: Hosts create a listing from one flow while hidden Product creation happens asynchronously behind the scenes

**Independent Test**: Sign in as a host, click "Create Listing", submit the unified form, verify the Location is created immediately, product-related controls remain pending until the hidden Product becomes available, and the screen transitions automatically when it is ready.

### Implementation for User Story 1

- [x] T009 [P] [US1] Add unified create-flow coverage in `src/web/apps/webapp-host/src/app/locations/create/page.test.tsx`
- [x] T010 [US1] Replace the current create flow with a unified listing flow in `src/web/apps/webapp-host/src/app/locations/create/page.tsx`
- [x] T011 [US1] Reuse existing marketplace location creation behavior from `src/web/apps/webapp-host/src/components/location/addLocation/add-marketplace-location.tsx`
- [x] T012 [P] [US1] Reuse product editor sections for title, pricing, policies, amenities, media, and rules from `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`
- [x] T013 [US1] Implement post-location-create hidden Product readiness waiting and automatic transition in `src/web/apps/webapp-host/src/app/locations/create/page.tsx`
- [x] T014 [US1] Persist pending listing configuration after linked Product readiness in `src/web/apps/webapp-host/src/components/unified-listing-form/useHostListingCoordinator.ts`
- [x] T015 [US1] Update unified create success navigation to land in listing management in `src/web/apps/webapp-host/src/app/locations/create/page.tsx`

**Checkpoint**: User can create a new listing with one flow and continue once the hidden Product becomes available

---

## Phase 4: User Story 2 - Edit an Existing Listing's Configuration (Priority: P1)

**Goal**: Hosts edit location and listing configuration from one page

**Independent Test**: Open an existing listing, edit location and product-backed settings from the same screen, save successfully, and verify unsaved-changes protection works.

### Implementation for User Story 2

- [x] T016 [P] [US2] Add unified edit-flow coverage in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.test.tsx`
- [x] T017 [US2] Turn the location edit page into the unified listing edit entry in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`
- [x] T018 [P] [US2] Reuse existing product edit behavior from `src/web/apps/webapp-host/src/components/product/editProduct/edit-product.tsx`
- [x] T019 [P] [US2] Reuse existing location edit mutations and validation in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`
- [x] T020 [US2] Add unsaved-changes handling across location and product sections in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`
- [x] T021 [US2] Keep product-related controls pending until linked Product readiness in `src/web/apps/webapp-host/src/components/unified-listing-form/HostListingShell.tsx`

**Checkpoint**: Host can edit any location or product-backed listing setting from one page

---

## Phase 5: User Story 3 - View Listings Summary with Key Product Information (Priority: P1)

**Goal**: Hosts see listing summary information directly from the locations experience

**Independent Test**: Open the locations list and a listing detail page and verify pricing, publish state, and booking summary are visible without using a separate Product page.

### Implementation for User Story 3

- [x] T022 [P] [US3] Rework the host locations list query for listing summary needs in `src/web/apps/webapp-host/src/app/locations/page.tsx`
- [x] T023 [P] [US3] Update listing summary presentation in `src/web/apps/webapp-host/src/components/location-card/LocationCard.tsx`
- [x] T024 [US3] Turn the location detail page into a listing summary and management entry screen in `src/web/apps/webapp-host/src/app/locations/[id]/page.tsx`

**Checkpoint**: Host can view key listing information without clicking through to product management

---

## Phase 6: User Story 4 - Configure Pricing and Cancellation Policies (Priority: P2)

**Goal**: Hosts manage pricing models, tax settings, and cancellation policies from the unified listing flow

**Independent Test**: Configure per-booking and subscription pricing, tax settings, and cancellation policies from the unified create/edit flows and verify the underlying Product is updated correctly.

### Implementation for User Story 4

- [x] T025 [P] [US4] Expose pricing and cancellation controls through the unified flow using `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`
- [x] T026 [US4] Persist subscription pricing, tax settings, and cancellation rules from `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`

**Checkpoint**: Host can manage pricing and cancellation policy settings from the unified listing experience

---

## Phase 7: User Story 5 - Manage Booking Rules and Restrictions (Priority: P2)

**Goal**: Hosts manage duration rules, booking increments, and related restrictions from the unified listing flow

**Independent Test**: Configure booking rule settings in the unified flow and verify invalid product-rule combinations surface coherent validation to the host.

### Implementation for User Story 5

- [x] T027 [P] [US5] Surface booking rule controls through the unified flow using `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`
- [x] T028 [US5] Surface product-rule validation in a unified error summary in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`

**Checkpoint**: Host can manage booking restrictions without leaving the unified listing experience

---

## Phase 8: User Story 6 - Navigate from Location Management Without Seeing Product Page (Priority: P2)

**Goal**: Hosts no longer see or use separate Product management routes

**Independent Test**: Verify the Host sidebar has no Products entry, `/products` redirects away, and legacy product routes resolve back into the correct listing flow.

### Implementation for User Story 6

- [x] T029 [US6] Remove the Host Products navigation entry in `src/web/apps/webapp-host/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [x] T030 [P] [US6] Redirect the Host products index route in `src/web/apps/webapp-host/src/app/products/page.tsx`
- [x] T031 [P] [US6] Redirect the Host product edit route in `src/web/apps/webapp-host/src/app/products/[id]/edit/page.tsx`
- [x] T032 [P] [US6] Redirect the location-scoped products index route in `src/web/apps/webapp-host/src/app/locations/[id]/products/page.tsx`
- [x] T033 [P] [US6] Redirect the location-scoped product create route in `src/web/apps/webapp-host/src/app/locations/[id]/products/create/page.tsx`
- [x] T034 [P] [US6] Redirect the location-scoped product detail route in `src/web/apps/webapp-host/src/app/locations/[id]/products/[productId]/page.tsx`

**Checkpoint**: Host no longer has a separate Product management experience

---

## Phase 9: User Story 7 - Manage Images and Amenities (Priority: P2)

**Goal**: Hosts manage images and amenities from the unified listing screens

**Independent Test**: Upload, reorder, and remove images, and edit amenities from the unified create/edit flows without leaving the listing experience.

### Implementation for User Story 7

- [x] T035 [P] [US7] Surface feature images and amenities in the unified create flow using `src/web/apps/webapp-host/src/components/product/product-editor-form.tsx`
- [x] T036 [US7] Keep feature images, amenities, and product tags editable in the unified edit flow in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`

**Checkpoint**: Host can manage listing media and amenities from the unified flow

---

## Phase 10: User Story 8 - View and Manage Availability Settings (Priority: P3)

**Goal**: Hosts manage availability-related settings from the unified listing experience

**Independent Test**: Open the unified listing flow, find the availability-related controls currently supported by the existing host surfaces, and verify those settings can be edited without using a separate Product page.

### Implementation for User Story 8

- [x] T037 [US8] Surface existing availability-related controls in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`
- [x] T038 [P] [US8] Wire newly created listings into the availability-ready state transition in `src/web/apps/webapp-host/src/queries/hostListingProductReadiness.ts`

**Checkpoint**: Host can manage supported availability settings from the unified listing flow

---

## Phase 11: User Story 9 - Edit Location Information Only (Priority: P3)

**Goal**: Hosts can update only location information without friction from product-backed sections

**Independent Test**: Change only location fields on the unified edit page and verify the save succeeds without forcing unrelated product edits.

### Implementation for User Story 9

- [x] T039 [US9] Allow location-only save when product-backed fields are untouched in `src/web/apps/webapp-host/src/app/locations/[id]/edit/page.tsx`

**Checkpoint**: Host can complete fast location-only maintenance from the unified edit flow

---

## Phase 12: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [x] T040 [P] Add legacy-route redirect coverage in `src/web/apps/webapp-host/src/app/products/page.test.tsx`
- [x] T041 [P] Align GraphQL contract notes with the actual readiness/query design in `specs/032-unified-host-listing/contracts/graphql.md`
- [x] T042 Run the quickstart scenarios in `specs/032-unified-host-listing/quickstart.md`
- [x] T043 Verify permission behavior across unified create/edit flows in `src/web/apps/webapp-host/src/app/locations/create/page.tsx`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel or sequentially in priority order
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1 - MVP)**: Starts after Foundational and establishes the async create flow
- **User Story 2 (P1)**: Depends on the shared create/edit orchestration from US1
- **User Story 3 (P1)**: Depends on foundational query/orchestration work but can proceed alongside US2
- **User Story 4 (P2)**: Depends on US1/US2 reusing the product editor inside unified screens
- **User Story 5 (P2)**: Depends on US1/US2 reusing the product editor inside unified screens
- **User Story 6 (P2)**: Depends on US2/US3 defining the canonical listing entry routes
- **User Story 7 (P2)**: Depends on US1/US2 product editor reuse
- **User Story 8 (P3)**: Depends on US1 async readiness handling and US2 unified edit entry
- **User Story 9 (P3)**: Depends on US2 unified edit entry

### Within Each User Story

- Shared query and readiness infrastructure before route wiring
- Route/page composition before redirects that rely on canonical listing entry points
- Validation and pending-state behavior before final quickstart verification

### Parallel Opportunities

- `T002`, `T004`, `T005`, `T006`, and `T007` can run in parallel after `T001`
- `T009` and `T010` can proceed alongside `T011` and `T012` once the foundational work is done
- `T018` and `T019` can run in parallel during unified edit implementation
- `T022` and `T023` can run in parallel for listing summary work
- `T030` through `T033` can run in parallel once the canonical listing routes are stable
- `T040` and `T041` can run in parallel during polish

---

## Parallel Example: User Story 1

```bash
# Launch parallel work for the unified create flow:
Task: "Add unified create-flow coverage in src/web/apps/webapp-host/src/app/locations/create/page.test.tsx"
Task: "Reuse product editor sections in src/web/apps/webapp-host/src/components/product/product-editor-form.tsx"
Task: "Implement hidden Product readiness waiting in src/web/apps/webapp-host/src/app/locations/create/page.tsx"
```

---

## Implementation Strategy

### MVP First (User Stories 1-3 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Complete Phase 4: User Story 2
5. Complete Phase 5: User Story 3
6. Validate the unified create/edit/view workflow before moving to lower-priority enhancements

### Incremental Delivery

1. Setup + Foundational → unified listing infrastructure ready
2. US1 (Create) → validate async hidden Product readiness flow
3. US2 (Edit) → validate one-page editing
4. US3 (Summary) → validate listing-centric browsing
5. US4-US9 → layer in deeper configuration and cleanup without reopening the product split

### Parallel Team Strategy

1. One stream handles query/readiness/orchestration (`T003`-`T007`)
2. One stream handles create/edit route composition (`T010`-`T021`)
3. One stream handles navigation cleanup and summary surfaces (`T022`-`T034`)
