# Feature Specification: Customer Landing Cleanup

**Feature Branch**: `020-customer-landing-cleanup`  
**Created**: 2026-06-01  
**Status**: Draft  
**Input**: User description: "Build a spec to identify all functionality now settled in webapp-teams or webapp-spaces and no longer needed in webapp. The webapp should become the public/customer landing and booking experience: location discovery, maps, insights, booking flows, and a minimum view of a customer's bookings across organizations, with an experience direction inspired by Airbnb and gabel.to. Private organization and co-working space administration should live in the other two apps, and webapp should be cleaned up and simplified to form the foundation for the new customer-facing product."

## Clarifications

### Session 2026-06-01

- Q: For this first cleanup/customer landing phase, how far should booking go? → A: Marketplace-style customer product booking across available locations; private organization booking interfaces stay out of webapp.
- Q: Which locations should appear in cross-location discovery? → A: Only marketplace-enabled customer-bookable locations.
- Q: What should the customer bookings/subscriptions hub support? → A: Full customer self-service, including cancel/change/refund where policy allows.
- Q: How should this relate to existing coworking-space owner marketplace sites? → A: Existing custom-subdomain marketplace sites stay unchanged; the no-subdomain webapp is an aggregate layer over the same marketplace functionality across locations.
- Q: How should webapp handle removed or owner-specific URLs in this phase? → A: No URL redirects from webapp for now, including marketplace customer-facing paths.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Identify Webapp Responsibilities (Priority: P1)

Product and engineering stakeholders review current webapp functionality and classify each customer, organization, team, and space workflow by whether it belongs in webapp, webapp-teams, webapp-spaces, or shared entry points.

**Why this priority**: Cleanup cannot start safely until the team agrees which responsibilities remain in webapp and which have already moved to the private administration apps.

**Independent Test**: Can be fully tested by reviewing the produced responsibility inventory and confirming every existing webapp route or major workflow has exactly one disposition: keep, move, remove, or preserve as shared entry point.

**Acceptance Scenarios**:

1. **Given** a complete list of existing webapp routes and major workflows, **When** stakeholders review the inventory, **Then** each item has an owner app, disposition, customer impact, and rationale.
2. **Given** functionality already available in webapp-teams or webapp-spaces, **When** it is classified for cleanup, **Then** the inventory marks it as removable from webapp unless it is required for the customer-facing booking journey or shared account access.
3. **Given** a workflow with unclear ownership, **When** the team reviews it, **Then** the workflow remains explicitly tracked as a decision item before any removal occurs.

---

### User Story 2 - Simplify Webapp To Customer Discovery And Booking (Priority: P1)

A customer visits webapp without a coworking-space owner custom subdomain and experiences it as the main public entry point for discovering marketplace-enabled locations, understanding available spaces through maps and useful insights, and buying customer-facing marketplace products without seeing private organization or space administration booking tools.

**Why this priority**: This defines the future product identity of webapp and provides the core value that replaces the old mixed administration surface.

**Independent Test**: Can be fully tested by opening webapp as a customer and confirming the first screen focuses on location discovery, maps, and marketplace-style product booking entry points while administration-only navigation is absent.

**Acceptance Scenarios**:

1. **Given** a visitor opens webapp, **When** the landing experience loads, **Then** they can browse available locations without being presented with private administration workflows.
2. **Given** marketplace-enabled customer-bookable locations exist, **When** a visitor explores the first page, **Then** the page presents location cards or listings, map context, and practical insights that support booking decisions.
3. **Given** a visitor selects a location, **When** they continue the journey, **Then** they can reach the marketplace-style product purchase path for that location from the customer-facing experience.
4. **Given** a visitor is on an existing coworking-space owner custom subdomain, **When** they use the current customer-facing marketplace pages, **Then** those pages continue to behave as the owner-specific marketplace and are not changed by the aggregate webapp cleanup.

---

### User Story 3 - View Customer Bookings Across Organizations (Priority: P2)

A signed-in customer can view and manage their own marketplace bookings and subscriptions across different organizations from webapp, giving them a personal self-service hub without exposing organization administration tools.

**Why this priority**: Customers need a self-service area to review and act on their relationship with locations across organizations without entering private organization interfaces.

**Independent Test**: Can be fully tested by signing in as a customer with bookings or subscriptions in more than one organization and confirming all relevant personal purchases are visible and eligible customer actions are available in one place.

**Acceptance Scenarios**:

1. **Given** a signed-in customer has marketplace bookings or subscriptions with multiple organizations, **When** they open their bookings view, **Then** they see their personal purchases grouped or labeled clearly enough to understand location and organization context.
2. **Given** a signed-in customer has no bookings or subscriptions, **When** they open their bookings view, **Then** they see a useful empty state that points them back to location discovery.
3. **Given** a user attempts to view bookings or subscriptions, **When** they are not signed in, **Then** the system prompts them to sign in before showing customer-specific purchase data.
4. **Given** a signed-in customer has a booking or subscription with an available customer action, **When** they view the purchase details, **Then** they can cancel, change, or request a refund only when the applicable policy allows that action.

---

### User Story 4 - Remove Or Hide Obsolete Administration Functionality (Priority: P2)

Administrators and internal users stop relying on webapp for private organization, team, and co-working space administration because those workflows are now owned by webapp-teams or webapp-spaces.

**Why this priority**: The cleanup reduces confusion, maintenance cost, and product overlap once the destination apps own the private administration experience.

**Independent Test**: Can be fully tested by checking the cleaned webapp navigation and route inventory to verify administration workflows marked removable are no longer reachable by normal users from webapp.

**Acceptance Scenarios**:

1. **Given** a workflow is classified as owned by webapp-teams, **When** a user looks for it in webapp, **Then** it is no longer presented as a webapp feature.
2. **Given** a workflow is classified as owned by webapp-spaces, **When** a user looks for it in webapp, **Then** it is no longer presented as a webapp feature.
3. **Given** a removed workflow may still be accessed through an old link, **When** the link is used in webapp, **Then** webapp handles it in place with an appropriate customer-safe explanation or unavailable state and does not redirect the URL in this phase.

---

### User Story 5 - Establish A Product Direction For The New Landing Experience (Priority: P3)

Product stakeholders can evaluate a first-pitch version of webapp that feels like a location marketplace and booking discovery product, taking directional inspiration from Airbnb and gabel.to while remaining specific to Skedular's locations and customer booking model.

**Why this priority**: The team needs a clear north star for future design and product work after cleanup, but it depends on first defining and simplifying the webapp responsibility boundary.

**Independent Test**: Can be fully tested by reviewing the first-pitch experience against a short product-direction checklist: discover locations, compare options, understand map context, see useful location insights, and start a booking.

**Acceptance Scenarios**:

1. **Given** the first-pitch landing experience is available, **When** stakeholders review it, **Then** they can identify the customer discovery, comparison, and booking value within the first page.
2. **Given** a visitor has not yet chosen a location, **When** they browse the landing page, **Then** they can compare location options without needing administration knowledge.
3. **Given** a location lacks enough insight data, **When** it appears in discovery, **Then** the experience remains usable and does not block the customer from learning basic location details or starting the booking path.
4. **Given** a visitor chooses a location from the aggregate webapp experience, **When** they open that location, **Then** they see location-level marketplace details and products comparable to the existing owner-specific customer-facing marketplace page.

### Edge Cases

- A capability exists in both webapp and another product app but behaves differently for customers and administrators.
- A route is used as a shared account, authentication, notification, or profile entry point by more than one product app.
- A customer has bookings across multiple organizations with different branding, policies, or booking states.
- A customer has subscriptions across multiple organizations with different renewal, cancellation, or payment states.
- A customer attempts to cancel, change, or request a refund for a booking or subscription that is not eligible under the applicable policy.
- A location has incomplete map, image, availability, or insight data.
- An old administration link into webapp is still bookmarked or indexed externally after cleanup.
- An owner-specific marketplace path is opened in webapp while URL redirects are intentionally disabled for this phase.
- A user has both customer bookings and administration permissions in another product app.
- No marketplace-enabled customer-bookable locations are currently available for browsing.
- A removed feature still has historical customer data that must remain accessible through the correct owner app or customer view.
- A request uses a coworking-space owner custom subdomain and must continue to resolve to the existing owner-specific marketplace experience.
- A request uses the main webapp without a custom subdomain and should expose cross-location marketplace discovery.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The feature MUST produce an inventory of current webapp routes, navigation items, and major user workflows that are in scope for responsibility review.
- **FR-002**: Each inventoried item MUST be classified as one of: keep in webapp, move to webapp-teams, move to webapp-spaces, preserve as a shared entry point, remove from customer-facing navigation, or defer pending decision.
- **FR-003**: Each classification MUST include a plain-language rationale, expected user impact, and any known dependency on customer booking, account access, organization administration, or space administration.
- **FR-004**: Webapp MUST be defined as the public and customer-facing discovery, marketplace-style product booking, subscription, and personal booking hub, not the primary home for private organization or co-working space administration.
- **FR-005**: Webapp MUST present a customer-facing first page that helps visitors discover marketplace-enabled customer-bookable locations, understand their map context, review useful location insights, and continue toward booking.
- **FR-006**: Users MUST be able to browse marketplace-enabled customer-bookable locations from webapp without being required to understand private organization or administration concepts.
- **FR-007**: Users MUST be able to buy customer-facing products from a selected location when that location supports marketplace booking.
- **FR-008**: Signed-in customers MUST be able to view and manage their own marketplace bookings and subscriptions across organizations from webapp, with enough context to distinguish location, organization, date or time, purchase type, payment state, and booking or subscription status.
- **FR-009**: Webapp MUST avoid exposing private administration workflows and private organization booking interfaces that are owned by webapp-teams or webapp-spaces in its primary customer navigation.
- **FR-010**: Removed or relocated workflows MUST have a documented in-webapp handling decision for existing links, such as showing an explanatory unavailable state or preserving access as a shared entry point, without redirecting the URL in this phase.
- **FR-011**: Shared account-level entry points that remain necessary across product apps MUST be identified separately from customer discovery and booking features.
- **FR-012**: The cleanup scope MUST protect customer-owned data, historical marketplace bookings, and subscriptions from accidental loss or invisibility during feature removal.
- **FR-013**: The first-pitch product direction MUST support comparison between locations using customer-relevant signals such as location details, map position, availability cues, imagery, and practical insights when available.
- **FR-014**: The feature MUST define a review checkpoint where product and engineering stakeholders approve the keep, move, remove, and defer decisions before implementation cleanup proceeds.
- **FR-015**: The feature MUST provide a measurable before-and-after summary of webapp simplification, including removed navigation areas, relocated workflows, and remaining customer-facing responsibilities.
- **FR-016**: Private organization booking creation, coworking space owner booking management, subscription management, and resource management MUST be owned by webapp-teams, not webapp.
- **FR-017**: Customer-facing purchases in webapp MUST follow the existing marketplace booking mental model: customers browse places, choose a location, buy available products, and pay for them without using private organization booking controls.
- **FR-018**: Cross-location discovery MUST exclude private, non-marketplace, or non-customer-bookable locations from the customer-facing browse experience.
- **FR-019**: Webapp MUST expose customer self-service actions for marketplace bookings and subscriptions, including cancel, change, and refund actions only when the relevant booking, subscription, payment, and cancellation policies allow them.
- **FR-020**: Webapp MUST communicate unavailable customer self-service actions without exposing private administration controls or internal policy mechanics.
- **FR-021**: Existing customer-facing marketplace experiences served through coworking-space owner custom subdomains MUST remain unchanged by this feature.
- **FR-022**: When no coworking-space owner custom subdomain is provided, webapp MUST provide aggregate marketplace discovery across eligible locations and organizations.
- **FR-023**: Selecting a location from the aggregate marketplace MUST lead to a location-level marketplace experience that preserves the customer-facing product browsing and purchase behavior of the existing owner-specific marketplace model.
- **FR-024**: Aggregate webapp URLs MUST distinguish selected locations clearly enough for customers to share, revisit, and understand location context without depending on private organization administration routes.
- **FR-025**: Webapp MUST NOT redirect URLs as part of this phase, including owner-specific marketplace customer-facing paths, old administration paths, or removed workflow paths.
- **FR-026**: When a path cannot be served in webapp during this phase, webapp MUST resolve it in place with a customer-safe unavailable or explanatory state that avoids exposing private administration controls.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers) and MUST avoid sensitive data leakage.

### Key Entities _(include if feature involves data)_

- **Webapp Capability**: A route, navigation item, page, workflow, or major customer/admin interaction currently present in webapp and reviewed for ownership.
- **Capability Classification**: The cleanup decision for a webapp capability, including owner app, disposition, rationale, user impact, and decision status.
- **Location Listing**: A marketplace-enabled customer-bookable place or venue that can appear in discovery and supports customer purchase entry points.
- **Aggregate Marketplace**: The no-subdomain webapp experience that lets customers discover and buy products across marketplace-enabled locations and organizations.
- **Owner-Specific Marketplace**: The existing customer-facing marketplace experience served through a coworking-space owner custom subdomain; this remains unchanged by the cleanup.
- **Location Insight**: Customer-relevant information that helps compare or understand a location, such as amenities, availability cues, imagery, map context, or practical details.
- **Customer Booking**: A marketplace-style booking belonging to the signed-in customer, including its location, organization context, schedule, payment state, current status, and eligible customer self-service actions.
- **Customer Subscription**: A marketplace-style recurring product purchase belonging to the signed-in customer, including its location, organization context, renewal context, payment state, current status, and eligible customer self-service actions.
- **Shared Entry Point**: A cross-product account or access workflow that remains reachable from webapp because it supports customers across product boundaries.
- **Cleanup Decision Record**: A durable record of what changed, what moved, what was removed, and how old access paths are handled.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of current webapp routes and major navigation entries are classified with an owner, disposition, rationale, and customer impact before cleanup starts.
- **SC-002**: At least 90% of administration-only navigation items currently visible in webapp are removed, relocated, or hidden from the customer-facing experience after approved cleanup.
- **SC-003**: A first-time visitor can reach a marketplace-enabled location discovery result and a marketplace-style product purchase entry point from the first page in under 60 seconds during usability testing.
- **SC-004**: A signed-in customer with bookings or subscriptions in multiple organizations can find all current and upcoming personal purchases and identify eligible self-service actions from webapp in under 2 minutes.
- **SC-005**: At least 85% of tested users describe the cleaned webapp purpose as location discovery, marketplace booking, subscription review, or personal booking management without prompting.
- **SC-006**: No customer-owned booking or subscription history is lost or made unreachable as a result of removing or relocating administration workflows.
- **SC-007**: Stakeholders can review a before-and-after simplification summary that accounts for every removed, relocated, preserved, and deferred capability.
- **SC-008**: Existing owner-specific marketplace pages on custom subdomains continue to pass their current customer-facing browse and purchase validation after the aggregate webapp changes.
- **SC-009**: A visitor using the no-subdomain webapp can discover at least two eligible locations from different organizations, when such data exists, without entering an owner-specific marketplace first.
- **SC-010**: Removed, relocated, and owner-specific paths tested in webapp produce in-place customer-safe states without URL redirects during this phase.

## Assumptions

- Webapp is intended to become the public and customer-facing app for discovery, booking, and personal booking management.
- Webapp-teams owns team, private organization, coworking space owner, booking management, subscription management, and resource management workflows that are not part of the customer marketplace purchase journey.
- Webapp-spaces owns co-working space administration workflows that are not part of the customer marketplace purchase journey.
- Existing marketplace booking behavior is the reference model for customer-facing purchase flows; the webapp difference is cross-organization and cross-location discovery before purchase.
- Cross-location discovery includes only locations explicitly enabled for customer-facing marketplace booking.
- Customer self-service actions are governed by the applicable marketplace booking, subscription, payment, cancellation, and refund policies.
- Coworking-space owner custom subdomains continue to represent owner-specific marketplace experiences and are not redesigned or functionally changed by this feature.
- The main webapp experience without an owner-specific custom subdomain is the aggregate marketplace surface.
- URL redirect behavior is out of scope for this phase; webapp handles unsupported or removed paths in place.
- Shared account entry points may remain in webapp when they are necessary for customers or cross-product access.
- The first cleanup phase focuses on responsibility discovery, route/workflow classification, safe removal decisions, and the product foundation rather than completing every future marketplace feature.
- The Airbnb and gabel.to references are directional product inspiration for discovery, comparison, and booking flow quality; the feature should not copy their branding, content, or protected expression.
- Existing customer authentication and booking records remain available to support personal booking views.
- American spelling and grammar are used for user-facing and operator-facing copy.
