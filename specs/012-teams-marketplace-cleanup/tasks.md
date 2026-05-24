# Tasks: Remove Marketplace from Web App Teams

**Input**: Design documents from `specs/012-teams-marketplace-cleanup/`
**Prerequisites**: plan.md ✓, spec.md ✓, research.md ✓, data-model.md ✓, quickstart.md ✓

---

## Phase 1: Setup

**Purpose**: Remove stale generated artefacts before any source edits begin. The
Relay compiler will produce errors if removed fragment/mutation names still appear
in the `__generated__` folder during incremental compilation.

- [ ] T001 Delete stale Relay generated artefacts from `web/apps/webapp-teams/src/queries/__generated__/`: `multipleChoicesProductTags_query.graphql.ts`, `myBookingCard_deleteMarketplaceBookingMutation.graphql.ts`, `myBookingCard_deleteMarketplaceBookingSubscriptionMutation.graphql.ts`; also scan the same folder for any additional `productTag*` or `deleteProductTag*` artefact files and delete them if present (satisfies SC-006 for `deleteProductTags`)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Remove shared marketplace infrastructure — proxy config, root shell,
shared icons, links, more-actions menu entries, and the product tag component
folder. All user story phases depend on these shared removals being complete first.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [ ] T002 [P] Remove `/marketplace` and `/marketplace/:path*` proxy routes and the middleware redirect for `pathname === '/marketplace'` from `web/apps/webapp-teams/src/proxy.ts`
- [ ] T003 [P] Remove `marketplaceCustomerRecordSynced` from the GraphQL query and from the `areCustomerRecordsSync` boolean expression in `web/apps/webapp-teams/src/components/rootShell/root-shell.tsx`
- [ ] T004 [P] Remove `marketplaceCustomerRecordSynced` from the GraphQL query and from the `areCustomerRecordsSync` boolean expression in `web/apps/webapp-teams/src/components/rootShell/no-organization-root-shell.tsx`
- [ ] T005 [P] Remove all marketplace link helper functions (`getOrganizationLocationAddMarketplaceLink`, `getOrganizationMarketplaceSetupBaseLink`, `getOrganizationMarketplaceSetupProductTagsBaseLink`, `getOrganizationMarketplaceSetupMarketplaceListingBaseLink`, `getOrganizationMarketplaceSetupBillingCycleBaseLink`, `getOrganizationMarketplaceSetupXeroBaseLink`, `getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink`, `getOrganizationMarketplaceSetupBankAccountsBaseLink`) from `web/apps/webapp-teams/src/components/links/index.ts`
- [ ] T006 [P] Remove `MarketplaceIcon`, `SetupMarketplaceIcon`, and `ProductTagIcon` exports from `web/apps/webapp-teams/src/components/icons/index.tsx`
- [ ] T007 [P] Remove `EditProductTag` and `DeleteProductTag` from `MoreActionsMenuOptionType` enum and from the `moreActionsMenuAllOptions` map in `web/apps/webapp-teams/src/components/moreActionsMenu/more-actions-menu.tsx`
- [ ] T008 [P] Delete the entire `web/apps/webapp-teams/src/components/productTag/` folder (contains `index.ts`, `product-tag.tsx`, `product-tags.tsx`)
- [ ] T009 [P] Delete `web/apps/webapp-teams/src/components/organization/multiple-choices-product-tags.tsx` and remove the `MultipleChoicesProductTags` export line from `web/apps/webapp-teams/src/components/organization/index.ts`
- [ ] T010 Remove the marketplace location link branch (call to `getOrganizationLocationAddMarketplaceLink`) from `web/apps/webapp-teams/src/components/location/addLocation/new-location-button.tsx` (depends on T005)

**Checkpoint**: Shared marketplace infrastructure removed. All three user story phases can now begin.

---

## Phase 3: User Story 1 — Teams App Shows No Marketplace Concepts (Priority: P1) 🎯 MVP

**Goal**: Eliminate every marketplace-labelled UI element, navigation entry, and
data field from the organisation admin panel, organisation page, and root shell.
After this phase, an admin navigating webapp-teams sees no marketplace concepts
anywhere in the interface.

**Independent Test**: Navigate to Organisation Admin → verify no "Marketplace Setup"
nav entry, no marketplace listing form fields, and no marketplace subscription data
loaded. Inspect the organisation page GraphQL query → verify no
`marketplaceBookingSubscriptions` or `marketplaceBookingSubscriptionCancellationModes`
fields are fetched.

- [ ] T011 [P] [US1] Remove `marketplaceListingMetadata { title subTitle }` field from the `organizationAdminSetupSectionQuery` Relay fragment, remove `marketplaceListingMetadata` from the patch mutation variable construction and all related `submittedPatchValues` references in `web/apps/webapp-teams/src/components/organization/organizationAdmin/organization-admin-setup-section.tsx`
- [ ] T012 [P] [US1] Remove the `marketplaceListingMetadata` title fallback (the `organization?.marketplaceListingMetadata?.title` reference) from the organisation admin header label in `web/apps/webapp-teams/src/components/organization/organizationAdmin/organization-admin.tsx`
- [ ] T012a [P] [US1] Locate and remove the entire "Marketplace Setup" navigation entry (tab, section header, nav item, or route link — whatever renders the marketplace setup section) from `web/apps/webapp-teams/src/components/organization/organizationAdmin/organization-admin.tsx`; verify by inspection that no `marketplace` route or label remains visible in the admin panel navigation after this change (satisfies FR-002 and clarification Q5)
- [ ] T013 [P] [US1] Remove `marketplaceBookingSubscriptions` and `marketplaceBookingSubscriptionCancellationModes` fields from the Relay fragment in `web/apps/webapp-teams/src/components/organization/organizationPage/organization.tsx`
- [ ] T014 [P] [US1] Remove `marketplaceBookingSubscriptions` and `marketplaceBookingSubscriptionCancellationModes` fields from the Relay fragment in `web/apps/webapp-teams/src/components/organization/organizationPage/organization-bookings.tsx`
- [ ] T015 [US1] Update `web/apps/webapp-teams/src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx` — remove any test assertions that reference `marketplaceListingMetadata` fields or marketplace-specific form values (depends on T011)

**Checkpoint**: Organisation admin and organisation page have no marketplace concepts. User Story 1 is independently testable.

---

## Phase 4: User Story 2 — Resource Management Remains Fully Functional (Priority: P2)

**Goal**: Remove product tag fields from all resource forms and displays — add/edit/bulk-import
resource dialogs, resource card, resource management list, and floor plan editor —
without breaking any other resource management functionality.

**Independent Test**: Open the Add Resource form, Edit Resource form, and bulk
resource import dialog — verify no product tag input field appears in any of them.
Open a floor plan in edit mode — verify no product tag chips on resource canvas
items. Add and save a resource — verify it succeeds without errors.

- [ ] T016 [P] [US2] Remove the `productTagIds` form field, `MultipleChoicesProductTags` import and usage, `productTagIds` from the Yup schema, and the `productTags` connection from the Relay fragment and mutation input in `web/apps/webapp-teams/src/components/resource/addResource/add-resource-dialog.tsx`
- [ ] T017 [P] [US2] Remove the `productTagIds` form field, `MultipleChoicesProductTags` import and usage, `productTagIds` from the Yup schema, and all `productTags` connection references from the Relay fragment and mutation input in `web/apps/webapp-teams/src/components/resource/editResource/edit-resource.tsx`
- [ ] T018 [P] [US2] Remove the `showProductTags` prop, the `MultipleChoicesProductTags` import, and the conditional `showProductTags &&` render block from `web/apps/webapp-teams/src/components/resource/bulkAddResources/bulk-add-resources-row.tsx`
- [ ] T019 [P] [US2] Remove the `showProductTags` prop threading and any product tag column configuration from `web/apps/webapp-teams/src/components/resource/bulkAddResources/bulk-add-resources-dialog.tsx`
- [ ] T020 [P] [US2] Remove the `productTags { id name color }` field from the Relay fragment in `web/apps/webapp-teams/src/components/resource/resource-card.tsx`
- [ ] T021 [P] [US2] Remove the `ProductTags` import, the `productTags` array type from the list item type definition, the `productTags` field from the Relay fragment, the `item.productTags.length > 0` metadata check, and both `<ProductTags ...>` render usages from `web/apps/webapp-teams/src/components/organization/organizationLocation/organization-location-resource-management-list.tsx`
- [ ] T022 [P] [US2] Remove `multipleChoicesProductTagsSortingValues` as a prop (and any other product tag variable references) from `web/apps/webapp-teams/src/components/organization/organizationLocation/organization-location-manage-resources-section.tsx`; this variable is threaded in from `page.tsx` (removed in T025) so both ends of the prop chain must be cleaned up together
- [ ] T023 [P] [US2] Remove the `productTags { id name color }` field from the resource sub-fragment in the floor plan query in `web/apps/webapp-teams/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`
- [ ] T024 [P] [US2] Remove the `productTags { id name color }` field from the resource sub-fragment in the floor plan query in `web/apps/webapp-teams/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx`
- [ ] T025 [P] [US2] Remove the `$multipleChoicesProductTagsSortingValues` variable declaration and its value from the query and `loadQuery` call in `web/apps/webapp-teams/src/rootPages/organizations/organization/locations/location/resources/resource/page.tsx`
- [ ] T026 [US2] Update `web/apps/webapp-teams/src/components/organization/organizationLocation/organization-location-resource-management-list.test.tsx` — remove test assertions that check for product tag chips or product tag display in the resource list (depends on T021)

**Checkpoint**: All resource management forms and displays are product-tag-free. Resource creation, editing, and bulk import are fully functional without product tags. User Story 2 is independently testable.

---

## Phase 5: User Story 3 — Booking History Shows Only Private Bookings (Priority: P3)

**Goal**: Filter marketplace bookings out of the booking list at render time; remove
all marketplace subscription query logic and marketplace-specific booking card
actions and mutations.

**Independent Test**: Load the bookings list as a user who holds both private and
marketplace bookings — verify only private bookings appear. Inspect the booking card
for a private recurring booking — verify it shows only private cancellation actions
with no marketplace-labelled buttons.

- [x] T027 [US3] In `web/apps/webapp-teams/src/components/booking/myBookings/my-bookings.tsx`: remove the `marketplaceBookingSubscriptions(first: 100, ...)` and `marketplaceBookingSubscriptionCancellationModes` fields from the Relay fragment; remove the `MarketplaceSubscriptionLookup` type; remove the `recurringMarketplaceSubscriptionIds` memo; add `marketplaceBooking { __typename }` as a direct field on the `myBookings` list-level Relay fragment (so the filter predicate has a data source independent of the card fragment); add a render-time filter to skip booking edges where `node.marketplaceBooking` is non-null; remove the `recurringMarketplaceSubscriptionIds` prop from `<MyBookingCard />`
- [x] T028 [US3] In `web/apps/webapp-teams/src/components/booking/myBookings/my-booking-card.tsx`: remove the `recurringMarketplaceSubscriptionIds` prop from the component `Props` type; remove both `commitDeleteMarketplaceBooking` and `commitDeleteMarketplaceBookingSubscription` mutation definitions; remove the `isMarketplaceRecurringBooking` derived constant; remove all marketplace-conditional label branches and action button guards; remove the `marketplaceBooking` sub-field from the **card** fragment only — the list-level fragment in `my-bookings.tsx` retains a minimal `marketplaceBooking { __typename }` field for the render filter (depends on T027)
- [x] T029 [US3] Update `web/apps/webapp-teams/src/components/booking/myBookings/my-booking-card.test.tsx` — remove all test cases and mock data that exercise marketplace booking deletion, marketplace subscription cancellation, or `isMarketplaceRecurringBooking` branches (depends on T027, T028)
- [x] T029a [US3] Add a test in `web/apps/webapp-teams/src/components/booking/myBookings/my-bookings.test.tsx` (create the file if absent) that verifies: given a booking list containing one booking with `marketplaceBooking: { __typename: "MarketplaceBooking" }` and one with `marketplaceBooking: null`, only the private booking (null) is rendered (depends on T027)

**Checkpoint**: Booking list shows only private bookings. All marketplace mutations and subscription logic removed. User Story 3 is independently testable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Relay artefact regeneration and full build + test verification.

- [x] T030 Run `pnpm relay` from `web/apps/webapp-teams/` to regenerate the full Relay artefact set after all source edits are complete (depends on T001 through T029)
- [x] T031 [P] Run `pnpm test` from `web/apps/webapp-teams/` and verify all tests pass with no marketplace-related failures (depends on T030)
- [x] T032 [P] Run `pnpm build` from `web/apps/webapp-teams/` and verify a clean TypeScript build with no unresolved import errors (depends on T030)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — run immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — **BLOCKS all user stories**
- **US1 (Phase 3)**: Depends on Phase 2 completion — independent of US2 and US3
- **US2 (Phase 4)**: Depends on Phase 2 completion — independent of US1 and US3
- **US3 (Phase 5)**: Depends on Phase 2 completion — independent of US1 and US2
- **Polish (Phase 6)**: Depends on all desired user story phases being complete

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2 — no dependency on US2 or US3
- **US2 (P2)**: Can start after Phase 2 — no dependency on US1 or US3
- **US3 (P3)**: Can start after Phase 2 — no dependency on US1 or US2

### Within Each User Story

- Test updates (T015, T026, T029) depend on their corresponding implementation tasks
- All other tasks within each phase are independently parallelisable (marked [P])

---

## Parallel Opportunities

### Phase 2 Foundational (all run in parallel after T001)

```
T002 proxy.ts
T003 root-shell.tsx
T004 no-organization-root-shell.tsx
T005 links/index.ts
T006 icons/index.tsx
T007 more-actions-menu.tsx
T008 productTag/ folder
T009 multiple-choices-product-tags.tsx + organization/index.ts
→ then T010 (depends on T005)
```

### Phase 3 US1 (all run in parallel)

```
T011 organization-admin-setup-section.tsx
T012 organization-admin.tsx
T013 organization.tsx
T014 organization-bookings.tsx
→ then T015 (depends on T011)
```

### Phase 4 US2 (all run in parallel)

```
T016 add-resource-dialog.tsx
T017 edit-resource.tsx
T018 bulk-add-resources-row.tsx
T019 bulk-add-resources-dialog.tsx
T020 resource-card.tsx
T021 organization-location-resource-management-list.tsx
T022 organization-location-manage-resources-section.tsx
T023 add-floor-plan.tsx
T024 edit-floor-plan.tsx
T025 resource/page.tsx
→ then T026 (depends on T021)
```

### Phase 5 US3 (sequential within story)

```
T027 my-bookings.tsx
→ T028 my-booking-card.tsx (depends on T027)
→ T029 my-booking-card.test.tsx (depends on T027, T028)
```

### Phase 6 Polish (T031 and T032 in parallel after T030)

```
T030 pnpm relay
→ T031 pnpm test (parallel)
→ T032 pnpm build (parallel)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001)
2. Complete Phase 2: Foundational (T002–T010)
3. Complete Phase 3: User Story 1 (T011–T015)
4. Run Phase 6: T030 → T031 + T032

**Delivers**: No marketplace UI anywhere in the admin panel and organisation pages.
All product tag and marketplace listing metadata removed from the admin experience.

### Full Delivery (All Stories)

1. Phase 1 → Phase 2 → Phases 3, 4, 5 in parallel → Phase 6

---

## Task Count Summary

| Phase                  | Tasks  | Parallel |
| ---------------------- | ------ | -------- |
| Phase 1 — Setup        | 1      | 0        |
| Phase 2 — Foundational | 9      | 8        |
| Phase 3 — US1 (P1)     | 5      | 4        |
| Phase 4 — US2 (P2)     | 11     | 10       |
| Phase 5 — US3 (P3)     | 3      | 0        |
| Phase 6 — Polish       | 3      | 2        |
| **Total**              | **32** | **24**   |
