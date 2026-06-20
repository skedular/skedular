# Feature Specification: Unified Host Listing Experience

**Feature Branch**: `032-unified-host-listing`  
**Created**: 2026-07-07  
**Status**: Draft  
**Input**: User description: "Skedular Host – Merge Location and Product into a Single End to End Experience"

## Clarifications

### Session 2026-07-07

- Q: How should the frontend retrieve unified listing data for editing? What is the expected backend API pattern? → A: Use existing Location and Product domain GraphQL APIs where possible. Do not introduce a new GraphQL API unless a concrete blocker is proven. No cross-domain API calls unless it's async (from Kafka subscriber or Temporal workflow).
- Q: What does 'subscription pricing' mean in the context of this feature? → A: Subscription bookings are for longer-term stays (>1 day) grouped under a subscription that can be auto-renewed or non-auto-renewed.
- Q: Where should validation occur for Listing configuration settings like pricing and booking rules? → A: Backend validation remains unchanged (existing rules still apply). Frontend validation is merged into one unified page. Add backend-side validation where needed.

### Session 2026-07-08

- Q: After a host creates a location, how should the unified flow handle the hidden Product becoming available? → A: Treat hidden Product creation as asynchronous. Keep location-related editing available immediately, keep product-related editing pending, and transition the screen automatically when the linked Product for that location becomes available.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Create a New Listing as a Host (Priority: P1)

A host wants to list their space for rent. Currently, they must navigate to separate Location and Product pages, understanding that two different system entities are being created behind the scenes. After this change, the host creates a single listing through one cohesive workflow.

**Why this priority**: This is the fundamental onboarding flow for new hosts. If hosts cannot easily create listings, the product fails at its core purpose.

**Independent Test**: Can be fully tested by signing in as a host, navigating to the location creation flow, and verifying that both Location and hidden Product entities are created with all configuration saved correctly. Delivers value as soon as the listing appears in the dashboard.

**Acceptance Scenarios**:

1. **Given** a host is on the locations page, **When** they click "Create Listing", **Then** they see a unified form containing both location information and listing configuration (pricing, availability, rules) in logical sections.
2. **Given** the host fills out the unified form with location details and pricing settings, **When** they submit, **Then** the system creates the Location entity and starts the existing asynchronous flow that creates the hidden Resource, Product Tag, and Product entities.
3. **Given** the Location has been created but the hidden Product is not yet available, **When** the host remains on the unified listing screen, **Then** location-related fields remain editable, product-related fields show a pending state, and the screen updates automatically when the linked Product becomes available.
4. **Given** the listing is created, **When** the host views their locations list, **Then** they see their new listing with key information like pricing, booking status, and visibility displayed directly without navigating to a separate page.

---

### User Story 2 - Edit an Existing Listing's Configuration (Priority: P1)

A host needs to update their listing details or change settings like pricing, cancellation policy, or availability. Currently, this requires navigating between Location and Product pages. After this change, hosts land on a location-centric grouped card entry screen and then open focused edit pages for each setup area.

**Why this priority**: Hosts need to frequently adjust their listings for seasonality, promotions, or personal changes. A fragmented workflow reduces efficiency and increases errors.

**Independent Test**: Can be fully tested by opening an existing listing's edit flow and verifying that all previously separate Product settings (pricing, policies, availability) are now accessible on the location page. Value is delivered when changes can be saved without leaving the page.

**Acceptance Scenarios**:

1. **Given** a host opens their listing for editing, **When** they use the grouped entry cards, **Then** they can open focused edit pages for Location properties (name, address, description) and Listing properties (pricing, cancellation policy, availability rules) without navigating to a separate Product management area.
2. **Given** the host modifies settings across different sections (e.g., changes price AND updates cancellation policy), **When** they save, **Then** all changes are persisted using existing Location and Product domain GraphQL APIs without requiring separate client-side API calls.
3. **Given** a host is editing a listing, **When** they navigate away without saving and have made changes, **Then** they see an appropriate unsaved changes warning.

---

### User Story 3 - View Listings Summary with Key Product Information (Priority: P1)

A host wants to quickly scan their listings to understand pricing, availability status, and booking information. Currently, this requires opening each listing individually or switching between Location and Product views.

**Why this priority**: Hosts need at-a-glance visibility into their portfolio performance. Fragmented information delays decision-making.

**Independent Test**: Can be fully tested by viewing the locations list page and verifying that each listing card shows pricing, booking status, visibility, and marketplace status without clicking through to a product page.

**Acceptance Scenarios**:

1. **Given** a host is on the locations listing page, **When** they view their list of locations, **Then** each listing card displays key product information including base price, currency, current availability status (available/unavailable), and booking confirmation settings.
2. **Given** a host views a listing card, **When** they look at marketplace-related information, **Then** they see whether the listing is published to the marketplace and visible status without needing to open another page.

---

### User Story 4 - Configure Pricing and Cancellation Policies (Priority: P2)

A host wants to set up complex pricing models (subscription, per-booking) and configure cancellation policies. Currently these are accessed through the Product page but must become accessible from the Location page.

**Why this priority**: Pricing configuration is critical for revenue optimization. Hosts need flexible options without navigating between pages.

**Independent Test**: Can be fully tested by setting up different pricing models (per-booking, subscription) and cancellation policies within the location edit flow. Value is delivered when all pricing scenarios can be configured in one place.

**Acceptance Scenarios**:

1. **Given** a host is editing a listing's pricing section, **When** they select a pricing model (per-booking or subscription), **Then** the appropriate pricing fields are displayed and they can configure rates.
2. **Given** a host has configured pricing, **When** they set up cancellation policies (flexible, moderate, strict), **Then** they can define the policy details including any free cancellation period.
3. **Given** a subscription-based listing is configured, **When** the host saves, **Then** the underlying Product entity receives the correct subscription pricing configuration based on existing backend validation rules.

---

### User Story 5 - Manage Booking Rules and Restrictions (Priority: P2)

A host wants to control who can book their space by setting minimum/maximum booking duration, booking increments, and other restrictions. Currently these are Product-level settings but must be accessible from the Location page.

**Why this priority**: Booking rules protect hosts' interests and prevent incompatible bookings. Fragmenting these controls reduces usability.

**Independent Test**: Can be fully tested by setting various booking restrictions (minimum hours, maximum guests, advance notice requirements) within the location edit flow. Value is delivered when all rule configurations can be applied in one session.

**Acceptance Scenarios**:

1. **Given** a host is editing a listing's booking rules section, **When** they configure minimum/maximum booking duration, **Then** they can specify time units (hours/days) and values.
2. **Given** a host sets booking increments (e.g., bookings must be in 30-minute blocks), **When** they save, **Then** the system enforces these constraints for new bookings using existing Product domain validation.
3. **Given** a host has set booking restrictions, **When** another user attempts to book outside those rules, **Then** the booking is rejected with an appropriate error message (existing behavior preserved).

---

### User Story 6 - Navigate from Location Management Without Seeing Product Page (Priority: P2)

After this change, the Product page should no longer be accessible from Skedular Host navigation. Hosts must not see or navigate to a separate product management interface.

**Why this priority**: The entire goal of this feature is to hide implementation complexity. Any path that leads to the old separation breaks the unified experience.

**Independent Test**: Can be fully tested by verifying that (a) no Product-related navigation items exist in Skedular Host menus, (b) direct URLs to product pages result in either redirect or 404 for hosts, and (c) breadcrumbs never reference "Products" as a standalone management area.

**Acceptance Scenarios**:

1. **Given** a host is logged into Skedular Host, **When** they view the main navigation menu, **Then** there is no "Products" or equivalent menu item visible.
2. **Given** a user attempts to access an old Product page URL directly, **When** they navigate to it as a Skedular Host, **Then** they receive an appropriate response (redirect or 404/not-found).
3. **Given** a host is on any location-related page, **When** they check breadcrumbs or navigation history, **Then** there are no references to "Product" as a separate management concept.

---

### User Story 7 - Manage Images and Amenities (Priority: P2)

A host wants to upload listing photos and manage amenities. Currently these may be scattered between Location and Product pages but must be consolidated.

**Why this priority**: Visual presentation and amenities significantly impact booking decisions. Fragmented media management reduces efficiency.

**Independent Test**: Can be fully tested by uploading multiple images, adding/removing amenities, and verifying they appear correctly in the listing preview without switching pages.

**Acceptance Scenarios**:

1. **Given** a host is editing a listing, **When** they navigate to the media section, **Then** they can upload, reorder, and delete listing photos all within the location edit page.
2. **Given** a host is configuring amenities, **When** they select or deselect amenities from the available list, **Then** their selections are saved as part of the location configuration.

---

### User Story 8 - View and Manage Availability Settings (Priority: P3)

A host wants to block out dates or set up recurring availability patterns. Currently these are Product-level settings but must be accessible through the Location page.

**Why this priority**: Availability management is important for preventing double-bookings during personal use or maintenance periods, but can be addressed after core creation/edit workflows work smoothly.

**Independent Test**: Can be fully tested by blocking dates or setting up weekly availability patterns within the location edit flow. Value is delivered when hosts can manage all availability from one place.

**Acceptance Scenarios**:

1. **Given** a host wants to block specific dates, **When** they navigate to the availability section of their listing, **Then** they can select dates and mark them as unavailable.
2. **Given** a host wants to set up recurring patterns, **When** they configure weekly availability (e.g., available Mon-Fri only), **Then** these patterns are applied consistently for future bookings.

---

### User Story 9 - Edit Location Information Only (Priority: P3)

Some hosts may only need to update location details (name, description, address) without touching listing settings. The edit flow should support this focused workflow.

**Why this priority**: This is a common maintenance task that should be quick and simple, even though it's lower priority than core configuration changes.

**Independent Test**: Can be fully tested by modifying only location fields and verifying they save correctly without requiring Product-related form sections to be completed.

**Acceptance Scenarios**:

1. **Given** a host wants to update only their listing name, **When** they modify the location information section and save, **Then** the change is applied without requiring them to fill out product/booking settings.
2. **Given** a host is viewing a saved location, **When** they check details like name, description, address, and property type, **Then** these are all visible on the same page.

---

### Edge Cases

- What happens when a user who previously accessed both Location and Product pages (e.g., Skedular Spaces operator) tries to access the Product page after this change?
- How does the system handle existing hosts who may have bookmarks or deep links to old Product URLs?
- What happens when a listing is created with missing product configuration data during migration?
- How are validation errors that span both Location and Product properties presented to the user?
- What happens if an API call to create the hidden Product fails after Location creation succeeds?
- What happens if the hidden Product does not become available within the expected wait window after Location creation?
- How does the system handle concurrent edits where one session modifies location info while another modifies product settings?

## Requirements _(mandatory)_

### Functional Requirements

**Location Management (Core)**

- **FR-001**: System MUST allow hosts to create a new listing through a unified form that includes both Location and Listing information.
- **FR-002**: System MUST persist all Location properties (name, description, address, property type, capacity) when a host creates or edits a listing.
- **FR-003**: System MUST display all Location properties on the location detail page without requiring navigation to another page.

**Listing Configuration (Formerly Product)**

- **FR-004**: System MUST expose pricing configuration on the location edit page, including pricing model selection (per-booking or subscription), base rates, and currency. Subscription pricing supports longer-term bookings (>1 day) grouped under a subscription that can be auto-renewed or non-auto-renewed.
- **FR-005**: System MUST allow hosts to configure cancellation policies (flexible, moderate, strict) with free cancellation period settings from the location edit page.
- **FR-006**: System MUST expose tax configuration options on the location edit page as they are relevant to Skedular Host.
- **FR-007**: System MUST allow hosts to manage availability settings including date blocking and recurring patterns from the location edit page.
- **FR-008**: System MUST allow hosts to configure booking rules including minimum/maximum booking duration, booking increments, and advance notice requirements.
- **FR-009**: System MUST expose visibility settings (published/unpublished, marketplace visibility) on the location edit page.
- **FR-010**: System MUST allow hosts to configure booking confirmation settings (instant book or request-to-book) from the location edit page.
- **FR-011**: System MUST allow hosts to upload, reorder, and delete listing images from the location edit page.
- **FR-012**: System MUST allow hosts to select and manage amenities for their listing from the location edit page.

**Unified Display**

- **FR-013**: System MUST display key Listing information (pricing, availability status, booking confirmation settings) on each listing card in the locations list view.
- **FR-014**: System MUST not require navigation to a separate Product page for any listing management task available to Skedular Hosts.

**Navigation and Routing**

- **FR-015**: System MUST remove all Product-related navigation items from the Skedular Host sidebar menu.
- **FR-016**: System MUST handle requests to old Product management URLs appropriately (redirect or return 404/not-found for Skedular Hosts).
- **FR-017**: System MUST ensure deep links that previously navigated to Product pages either redirect to Location pages or return an appropriate response for hosts.

**Backend Integration**

- **FR-018**: System MUST continue using the existing Temporal workflow to automatically create hidden Resource, Product Tag, and Product entities when a host creates a listing.
- **FR-019**: System MUST preserve all existing relationships between Location, Resource, Product, and Product Tag in the database without breaking changes to those entities.
- **FR-020**: System MUST use existing Location and Product domain GraphQL APIs directly. No new backend services or APIs should be created for this feature unless a concrete blocker is proven. Frontend coordinates calls to existing GraphQL mutations as needed. No cross-domain API calls unless it's async (from Kafka subscriber or Temporal workflow).

**User Interface**

- **FR-021**: System MUST use a location-centric grouped card entry page that routes to focused edit pages for each setup area (for example location details, pricing/rules, media/amenities, and visibility) without exposing Product as a host concept.
- **FR-022**: System MUST preserve all existing validation rules that applied to Product entities when those settings are now edited on the location page. Frontend validation is merged into one unified page; backend validation remains unchanged and existing rules still apply.
- **FR-023**: System MUST apply existing permission checks consistently—location editors must have appropriate access to modify all listing configuration.

**Error Handling**

- **FR-024**: If automatic creation of hidden Product entities fails after Location creation, system MUST roll back or handle the failure gracefully with clear error messaging.
- **FR-025**: System MUST display validation errors that reference both Location and Listing properties on a single unified error summary when multiple sections have issues. Validation rules are enforced server-side using existing Product domain validation.
- **FR-026**: After a Location is created, system MUST keep location-related actions available immediately even if the linked hidden Product is still being materialized asynchronously.
- **FR-027**: System MUST keep product-related actions in a pending state until the linked hidden Product for the created Location becomes available, then transition the unified screen automatically without requiring the host to restart the workflow manually.

### Observability and Logging Requirements

- **LOG-001**: Feature MUST emit structured logs for listing creation start/completion, including whether hidden entities were successfully created.
- **LOG-002**: Feature MUST emit structured logs for listing edits that modify Product-related settings (pricing, policies, availability).
- **LOG-003**: Feature MUST emit warning/error logs when automatic Product/Resource/Product Tag creation fails.
- **LOG-004**: Feature logs MUST include correlation context (request ID, organization ID, location ID) and avoid sensitive data leakage.

### Key Entities

- **Location**: Represents the physical space being listed. Key attributes include name, description, address, property type, capacity, and ownership. Remains as the primary entity for Skedular Hosts to manage.
- **Product**: The hidden backend entity that stores pricing, booking rules, availability settings, and marketplace configuration. Continues to exist but is no longer directly editable by hosts. Uses existing Product domain GraphQL API for data access.
- **Resource**: The backend entity representing the bookable item, automatically created alongside Location with a one-to-one relationship.
- **Product Tag**: A tag linking Product to Organization for marketplace categorization, automatically created and managed.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Hosts can create a new listing (including location details, pricing, and basic configuration) in under 5 minutes, measured from clicking "Create Listing" to seeing the listing appear in their dashboard.
- **SC-002**: 90% of hosts can complete their first edit session (modifying both location and listing settings) on their own without external assistance or documentation review.
- **SC-003**: The average time to load a host's listings page with summary information must not increase by more than 500ms compared to the current implementation.
- **SC-004**: Zero Product-related navigation items remain visible in Skedular Host UI for hosts (navigation cleanup verification).
- **SC-005**: 100% of existing listings migrate without data loss or requiring manual intervention.

### Business Metrics

- **SC-B1**: Support tickets related to "finding product settings" or "where is the product page" decrease by at least 80% within 3 months of release.
- **SC-B2**: Hosts with completed listings (both location and listing configuration) increase by at least 25% within 6 months of release.

## Assumptions

- **A1**: The existing Temporal workflow for automatic Product/Resource/Product Tag creation continues to function correctly without modification—this feature only changes the frontend UI.
- **A2**: All Skedular Host organizations will have completed migration by the time the old Product page is fully removed (no partial-migration states need special handling).
- **A3**: Hosts do not require access to the raw Product entity IDs or backend technical details—these remain hidden implementation concerns.
- **A4**: The existing API contracts for Location and Product domain GraphQL APIs can be used directly without requiring new cross-domain endpoints in the expected implementation.
- **A5**: All permissions previously applied to Location management (edit, delete, view) also apply appropriately to the listing configuration—no additional permission logic needs to be introduced.
- **A6**: Skedular Spaces operators retain access to separate Location/Resource/Product management (this change only affects Skedular Host).
- **A7**: The existing booking system's validation and business rules for products remain unchanged—the frontend just exposes different settings. Backend validation (not frontend) continues to enforce all Product-level constraints.

## Unsupported (Out of Scope)

- **U1**: Changes to the Skedular Spaces or Skedular Teams product interfaces—these continue with separate Location/Resource/Product management.
- **U2**: Modifications to backend domain models, database schemas, or Temporal workflows—these remain unchanged as per requirements.
- **U3**: New pricing models or business logic—only exposure/access of existing capabilities is changed.
- **U4**: Migration scripts for old Bookmarks or Deep Links—redirects will handle common cases but individual user bookmarks may need manual update.
