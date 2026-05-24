# Feature Specification: Remove Marketplace from Web App Teams

**Feature Branch**: `feature/012-teams-marketplace-cleanup`
**Created**: 2026-05-24
**Status**: Draft
**Input**: User description: "I need you to start looking into web app teams. The web app team is supposed to be for private organization. Anything that is related to marketplace needs to be cleaned up from this app and completely removed from this app. We stick it inside the web app until we completely do the proper migration, and then we get rid of it in the web app. We're focusing here only on web app teams, and anything marketplace related would be gone. Things like: marketplace product tags, product, anything that really has a direct meaning or direct definition based on the marketplace would be removed from the web app teams."

## Clarifications

### Session 2026-05-24

- Q: Should product tag fields and chips be removed from the floor plan editor (addFloorPlan, editFloorPlan) in webapp-teams? → A: Yes — remove from floor plan editor, consistent with FR-001's "anywhere in the interface" scope.
- Q: Should marketplace API proxy routes in `proxy.ts` also be removed from webapp-teams? → A: Yes — remove marketplace proxy routes from `proxy.ts` to enforce the private-organisation product boundary.
- Q: Should marketplace bookings appear in the webapp-teams booking list, with only their action buttons removed? → A: No — marketplace bookings are not shown in webapp-teams at all. The booking list displays only private organisation bookings. Users who need to see marketplace bookings must use webapp or webapp-spaces. Cross-product booking integration (private org ↔ co-working space marketplace) is deferred to a future feature.
- Q: After marketplace booking actions are removed, should private booking cancellation/deletion actions remain on the booking card? → A: Yes — private booking actions remain fully intact. Since marketplace bookings are filtered out entirely, the booking card in webapp-teams only ever renders private organisation bookings and their existing actions are unchanged.
- Q: Should only the "Product Tags" section be removed from the admin panel, or the entire marketplace setup navigation entry? → A: Remove the entire marketplace setup navigation entry from the admin panel, not just the product tags section within it.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Teams App Shows No Marketplace Concepts (Priority: P1)

An admin user logs into the `webapp-teams` application to manage their private organisation. They navigate through the interface — including the organisation admin panel, resource management, booking history, and team pages — and encounter no marketplace-specific UI elements, labels, filters, or actions that reference marketplace products, product tags, or public marketplace listings.

**Why this priority**: The core purpose of webapp-teams is private organisation management. The presence of marketplace UI elements creates confusion, incorrect user expectations, and leaks concerns that belong to a different product entirely. Removing them is the primary objective of this feature.

**Independent Test**: Can be fully tested by navigating each page and section of webapp-teams as an authenticated admin user and verifying no marketplace-labelled UI elements, product tag pickers, marketplace booking actions, or product listing widgets appear.

**Acceptance Scenarios**:

1. **Given** an authenticated admin user in webapp-teams, **When** they view the organisation admin panel, **Then** there is no "Product Tags" setup section, no marketplace listing section, and no marketplace-related navigation items.
2. **Given** an authenticated user viewing the resource management list, **When** they inspect a resource's details or the resource add/edit forms, **Then** there are no product tag pickers or product tag display chips present.
3. **Given** an authenticated admin user is in the floor plan editor (add or edit a floor plan), **When** the floor plan editor renders resource positions on the canvas, **Then** no product tag chips or product tag metadata are displayed on resources within the floor plan editor.
4. **Given** an authenticated user views the bookings list in webapp-teams, **When** the list loads, **Then** only private organisation bookings appear; marketplace bookings are not shown.
5. **Given** an authenticated admin user, **When** they inspect the organisation page navigation, **Then** there are no links or menu items pointing to marketplace setup, marketplace listing, or products pages.

---

### User Story 2 - Resource Management Remains Fully Functional (Priority: P2)

A team admin creates, edits, and bulk-imports resources after product tags have been removed from the resource forms. The resource management workflow remains complete and fully functional without the product tag field.

**Why this priority**: Resource management is a core teams workflow. The removal of product tags from resource forms must not break resource creation or editing.

**Independent Test**: Can be fully tested by creating a new resource, editing an existing resource, and using bulk import — verifying all fields except product tags work correctly and the forms submit successfully.

**Acceptance Scenarios**:

1. **Given** an admin user opens the "Add Resource" form, **When** the form renders, **Then** there is no "Product Tags" input field, and the form still contains all other resource fields (name, type, zone, capacity, etc.).
2. **Given** an admin user opens the "Edit Resource" form for an existing resource that previously had product tags assigned, **When** the form renders, **Then** product tags are absent from the form and the resource can be saved without error.
3. **Given** an admin user uses the bulk resource import dialog, **When** they complete the import flow, **Then** there is no product tag column or assignment step, and resources are imported successfully.

---

### User Story 3 - Booking History Shows Only Private Bookings (Priority: P3)

A user views their booking history in webapp-teams and sees only their private organisation bookings. Marketplace bookings — such as desk bookings made at external co-working spaces via the public marketplace — do not appear in webapp-teams. To view marketplace bookings, the user must go to webapp or webapp-spaces. Private booking management actions (cancel, edit) remain fully functional.

**Why this priority**: Booking history must reflect only the private-organisation context. Mixing marketplace and private bookings creates a confusing experience and implies a cross-product integration that does not yet exist. The two booking types will be integrated in a future feature once the proper migration is in place.

**Independent Test**: Can be fully tested by loading the bookings list in webapp-teams as a user who also holds marketplace bookings in webapp, and verifying that the webapp-teams list contains only private organisation bookings. All private booking actions (cancel, edit) work correctly.

**Acceptance Scenarios**:

1. **Given** a user has both private organisation bookings and marketplace bookings, **When** they view the booking list in webapp-teams, **Then** only their private organisation bookings are shown; no marketplace bookings appear.
2. **Given** a user views the booking list in webapp-teams, **When** they inspect the available actions on a booking card, **Then** only private booking management actions are present; there are no marketplace booking or subscription actions.
3. **Given** the bookings list renders, **When** it loads, **Then** there are no GraphQL mutations for `deleteMarketplaceBooking` or `deleteMarketplaceBookingSubscription` present in the component.

---

### Edge Cases

- What happens to resources that have existing product tag data stored in the backend? The backend data is not affected — only the frontend display and editing are changed. Tags remain stored but are simply not surfaced in the teams UI.
- What happens to a user who has marketplace bookings and opens webapp-teams? Marketplace bookings are not fetched or displayed in webapp-teams. The user must go to webapp or webapp-spaces to see those bookings. No in-app redirect or notification is required for this feature.
- What happens if a link helper function for marketplace routes is still referenced somewhere after cleanup? Any unresolved references to removed marketplace link helpers must result in a build error that is caught and resolved before the feature is complete.
- What happens to Relay-generated GraphQL artifacts that reference product tags or marketplace mutations? Generated artifacts that are no longer referenced by any component must be removed or left unused; the build must not fail due to orphaned generated types that are no longer imported.
- What if the marketplace OpenAPI client folder (`clients/openapi/skedular/v1/marketplace/`) is referenced by non-marketplace code? Any such references must be identified and removed before the client folder is considered clean.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The `webapp-teams` application MUST NOT display any product tag UI elements — including product tag pickers, product tag chips, product tag search fields, or product tag management lists — anywhere in the interface. This includes the floor plan editor (addFloorPlan and editFloorPlan components).
- **FR-002**: The `webapp-teams` organisation admin panel MUST NOT include any marketplace setup navigation entry. The entire marketplace setup section (including any tab, link, or navigation item pointing to marketplace listing, product tags, billing cycle, Xero, Stripe Connect, or bank account setup within the marketplace context) MUST be removed from the admin panel navigation.
- **FR-003**: The resource add form in `webapp-teams` MUST NOT contain a product tags input field.
- **FR-004**: The resource edit form in `webapp-teams` MUST NOT contain a product tags input field.
- **FR-005**: The bulk resource import dialog in `webapp-teams` MUST NOT include any product tag assignment step or column.
- **FR-006**: Marketplace bookings MUST NOT appear in the booking list in `webapp-teams`. The booking list query MUST filter results to show only private organisation bookings. Users who need to view marketplace bookings must use webapp or webapp-spaces.
- **FR-007**: The `webapp-teams` application MUST NOT include navigation links or menu items that route to marketplace setup, marketplace listing, or products pages. This applies to both the top-level application navigation and any admin panel section navigation.
- **FR-008**: The `productTag` component folder (`src/components/productTag/`) MUST be removed from `webapp-teams` if it is not used by any non-marketplace feature after cleanup.
- **FR-009**: The `multiple-choices-product-tags.tsx` component MUST be removed from `webapp-teams` if it is no longer referenced after other marketplace removals.
- **FR-010**: All marketplace-specific GraphQL fragment and mutation definitions (such as `multipleChoicesProductTags`, `deleteMarketplaceBooking`, and `deleteMarketplaceBookingSubscription`) MUST be removed from `webapp-teams` source files. Generated artefact cleanup is a two-step process: (1) stale `__generated__/*.graphql.ts` files that reference removed definitions MUST be manually deleted first, then (2) `pnpm relay` MUST be run from `web/apps/webapp-teams/` to regenerate a clean artefact set. No stale generated artefact that imports a removed fragment or mutation may remain active in the build.
- **FR-011**: Marketplace-specific entries in the `moreActionsMenu` options that apply only to marketplace contexts MUST be removed from the teams app's more-actions configuration.
- **FR-012**: Marketplace-specific link helper functions that exist solely for marketplace pages (e.g. marketplace listing, marketplace product pages, marketplace setup sections) MUST be removed from `web/apps/webapp-teams/src/components/links/index.ts` entirely, not merely left unused. No call sites to these helpers may remain in `webapp-teams` components or pages.
- **FR-013**: The `webapp-teams` application MUST continue to build and pass all existing tests after marketplace removals.
- **FR-014**: The `webapp-teams` resource card component MUST NOT display product tag badges or product tag information.
- **FR-015**: The `webapp-teams` floor plan editor (addFloorPlan and editFloorPlan) MUST NOT display product tag chips or product tag metadata on resource positions within the canvas.
- **FR-016**: The `webapp-teams` proxy configuration (`proxy.ts`) MUST NOT include marketplace API proxy routes after this cleanup. Any proxy forwarding rules that route requests to the marketplace API endpoints MUST be removed.
- **FR-017**: The `webapp-teams` booking list MUST request and render only private organisation bookings. Any GraphQL query fragments or variables that previously included marketplace booking types MUST be removed from the booking list query.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: No new structured logging is required for this removal feature; existing logging patterns for resource management and booking workflows are preserved.
- **LOG-002**: If any residual marketplace references are detected at build time (TypeScript errors, missing imports), those MUST be resolved and treated as blocking issues before the feature is considered complete.
- **LOG-003**: No new warning or error log paths are introduced; existing error handling for resource and booking operations is retained unchanged.
- **LOG-004**: No correlation context changes are required; this feature removes UI code only and does not alter backend API calls for private booking or resource workflows.

### Key Entities _(include if feature involves data)_

- **Product Tag**: A marketplace-defined label used to classify resources and products for customer discovery. In webapp-teams, all UI surfaces displaying, selecting, or managing product tags are removed. The underlying data remains in the backend and is unaffected.
- **Marketplace Booking**: A booking made via the public marketplace flow (e.g., booking a desk at an external co-working space). Marketplace bookings are not shown in webapp-teams at all. They remain accessible via webapp and webapp-spaces. Cross-product booking integration is deferred to a future feature.
- **Marketplace Booking Subscription**: A recurring marketplace subscription. Not shown in webapp-teams. Users access these via webapp or webapp-spaces.
- **Resource**: A physical or virtual asset (desk, room, parking) managed within an organisation. Resource management (add, edit, bulk import) remains fully functional; only the product tag field is removed from its forms.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Zero marketplace-labelled UI elements visible to a user navigating all pages of webapp-teams after the change.
- **SC-002**: The webapp-teams application builds without errors and all pre-existing tests pass after marketplace removals are complete.
- **SC-003**: Resource creation, editing, and bulk import complete successfully for 100% of tested flows with no product tag field present.
- **SC-004**: The booking list in webapp-teams shows only private organisation bookings. Users with marketplace bookings see zero marketplace booking entries in webapp-teams, and all private booking management actions function correctly.
- **SC-005**: The `productTag` component folder and `multiple-choices-product-tags.tsx` are fully absent from `webapp-teams` (verified by directory listing and build output).
- **SC-006**: No marketplace-specific GraphQL mutations (`deleteMarketplaceBooking`, `deleteMarketplaceBookingSubscription`, `deleteProductTags`) remain in webapp-teams source or generated artifacts that are imported by active components.

## Assumptions

- The backend APIs remain unchanged; only the frontend `webapp-teams` application is modified.
- Marketplace features will continue to exist in `webapp` and `webapp-spaces`; they are only removed from `webapp-teams`.
- Any product tag data already stored in the backend for resources is not affected; the removal is UI-only.
- The webapp-teams application is for private-organisation use and does not need marketplace listing, public product discovery, or customer-facing marketplace checkout flows.
- The generated marketplace OpenAPI client files under `clients/openapi/skedular/v1/marketplace/` are auto-generated; if they are no longer referenced after cleanup, they will be excluded from active imports but may remain on disk until the next generation cycle.
- Relay-generated GraphQL artifact files (`.graphql.ts`) for removed fragments and mutations will be removed as part of this cleanup; a Relay compiler run after removal will regenerate the artifact set without the deleted entries.
- The `billing-and-payment` page in webapp-teams is scoped to private invoicing and is not considered marketplace; it is out of scope for this removal.
- The Xero integration, Stripe Connect accounts, and bank account management features are not part of this cleanup — they belong to private billing and remain in scope only for `webapp-spaces`, not webapp-teams.
- Marketplace bookings are not shown in webapp-teams. Users who need to view or manage marketplace bookings (e.g., co-working space desk bookings made via the public marketplace) must use webapp or webapp-spaces. Cross-product booking integration — where private-org employees can see and manage marketplace bookings within webapp-teams — is explicitly out of scope and deferred to a future feature.
- The marketplace API proxy routes in `proxy.ts` are explicitly removed as part of this feature to enforce the webapp-teams product boundary at the network layer.
