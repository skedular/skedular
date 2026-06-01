# Data Model: Customer Landing Cleanup

## Webapp Capability

Represents a current webapp route, navigation item, page, or major workflow reviewed for ownership.

**Fields**:

- `id`: Stable inventory identifier.
- `pathPattern`: Route or workflow identifier, for example `/marketplace/bookings` or `/msteams/organizations/[organizationCustomDomain]/bookings`.
- `label`: Human-readable capability name.
- `currentSurface`: Current location in webapp navigation or route tree.
- `audience`: `customer`, `visitor`, `organization-admin`, `coworking-owner`, `shared-account`, or `unknown`.
- `workflowType`: `discovery`, `marketplace-booking`, `marketplace-subscription`, `customer-self-service`, `private-booking`, `resource-management`, `admin-settings`, `account`, `notifications`, or `other`.
- `hasCustomerOwnedData`: Whether removal could hide customer bookings, subscriptions, invoices, refunds, or account state.
- `currentCustomDomainBehavior`: `no-subdomain`, `owner-specific-custom-subdomain`, `both`, or `not-applicable`.

**Relationships**:

- Has one `Capability Classification`.
- May reference one or more `Cleanup Decision Records`.

**Validation Rules**:

- Every current webapp route and major navigation entry must have a capability record before cleanup starts.
- Every capability must declare whether it affects customer-owned data.
- Capabilities that affect owner-specific custom-subdomain marketplace behavior must be marked as protected for this phase.

## Capability Classification

Represents the ownership and cleanup decision for a webapp capability.

**Fields**:

- `capabilityId`: The reviewed capability.
- `ownerApp`: `webapp`, `webapp-teams`, `webapp-spaces`, `shared-entry-point`, or `undecided`.
- `disposition`: `keep`, `move`, `remove-from-navigation`, `preserve-shared`, `protect-unchanged`, or `defer`.
- `rationale`: Plain-language reason for the decision.
- `customerImpact`: Expected impact on visitors and signed-in customers.
- `adminImpact`: Expected impact on administrators or coworking-space owners.
- `urlHandling`: `serve-in-place`, `unavailable-in-place`, `preserve-existing`, or `not-applicable`.
- `approvalStatus`: `draft`, `reviewed`, `approved`, or `blocked`.

**Relationships**:

- Belongs to one `Webapp Capability`.
- Produces one or more `Cleanup Decision Records` when implemented.

**Validation Rules**:

- No classification may choose URL redirect behavior in this phase.
- `webapp-teams` ownership is required for private organization booking creation, coworking owner booking management, subscription management, and resource management.
- Owner-specific custom-subdomain marketplace capabilities must use `protect-unchanged` unless the feature explicitly scopes a non-functional validation-only task.

## Location Listing

Represents an eligible customer-bookable location in aggregate marketplace discovery.

**Fields**:

- `locationId`: Stable location identifier.
- `organizationId`: Owning organization identifier.
- `organizationName`: Customer-visible organization name.
- `organizationCustomDomain`: Owner custom domain or organization custom domain used for context.
- `locationName`: Customer-visible location name.
- `locationUrlKey`: Shareable aggregate webapp location path segment or equivalent URL identifier.
- `addressSummary`: Customer-readable address summary.
- `mapPosition`: Latitude/longitude or displayable map position when available.
- `featureImage`: Optional customer-visible image.
- `marketplaceEnabled`: Whether the location is explicitly marketplace-enabled.
- `customerBookable`: Whether customers can buy products from this location.
- `availabilityCue`: Optional summary of current customer-facing availability.
- `insights`: Collection of `Location Insight` items.

**Relationships**:

- Belongs to an organization.
- Has zero or more customer-facing products.
- Has zero or more `Location Insight` items.

**Validation Rules**:

- Aggregate discovery may show only listings where `marketplaceEnabled` and `customerBookable` are true.
- Private or non-customer-bookable locations must not appear in aggregate discovery.
- Missing images, map position, or insight data must not block a listing from being usable if the location is otherwise eligible.

## Aggregate Marketplace

Represents the no-subdomain webapp experience that exposes eligible marketplace locations across organizations.

**Fields**:

- `entryPoint`: Always no-subdomain webapp for this feature.
- `discoveryMode`: `map-and-list` for browse, compare, and selection.
- `locationFilters`: Customer-facing filters such as map boundary, search, and product/resource category when available.
- `selectedLocation`: Optional `Location Listing` selected by the customer.
- `unsupportedPathState`: Customer-safe in-place state for paths that cannot be served.

**Relationships**:

- Lists many `Location Listing` records.
- Leads to location-level marketplace pages and product purchase flows.

**Validation Rules**:

- Must not redirect URLs during this phase.
- Must not expose private organization booking controls.
- Must distinguish selected locations in URLs clearly enough for sharing and revisiting.

## Owner-Specific Marketplace

Represents the existing custom-subdomain customer-facing marketplace for a coworking-space owner.

**Fields**:

- `customSubdomain`: Owner-specific custom-subdomain context.
- `organizationContext`: Organization represented by the custom subdomain.
- `existingBrowseBehavior`: Current location/product browsing behavior.
- `existingPurchaseBehavior`: Current booking/subscription purchase behavior.

**Relationships**:

- Uses existing marketplace components and purchase flows.
- Is protected by regression validation in this feature.

**Validation Rules**:

- Must remain functionally unchanged by aggregate marketplace cleanup.
- Must continue to pass current browse and purchase validation.

## Location Insight

Represents customer-relevant information that helps compare or understand a location.

**Fields**:

- `type`: `amenity`, `availability`, `capacity`, `address`, `image`, `map`, `policy`, or `other`.
- `label`: Customer-facing label.
- `value`: Customer-facing value.
- `confidence`: `complete`, `partial`, or `missing`.

**Relationships**:

- Belongs to one `Location Listing`.

**Validation Rules**:

- Insights must be customer-facing and must not reveal private administration data.
- Missing insights must produce graceful fallback UI.

## Customer Booking

Represents a marketplace-style booking belonging to a signed-in customer.

**Fields**:

- `bookingId`: Stable booking identifier.
- `organizationId`: Owning organization identifier.
- `organizationName`: Customer-visible organization name.
- `locationId`: Booked location identifier.
- `locationName`: Customer-visible location name.
- `productId`: Purchased product identifier.
- `productName`: Customer-visible product name.
- `scheduleSummary`: Customer-readable date/time or usage window.
- `paymentState`: Current customer-facing payment state.
- `bookingStatus`: Current customer-facing booking state.
- `eligibleActions`: Policy-bound customer actions: `cancel`, `change`, `refund-request`, or none.

**Relationships**:

- Belongs to a signed-in customer.
- May be associated with a product and location.
- May produce refund or cancellation states according to policy.

**State Transitions**:

- `upcoming` → `changed` when an eligible customer change succeeds.
- `upcoming` → `cancelled` when an eligible customer cancellation succeeds.
- `paid` → `refund-requested` → `refunded` or `refund-unavailable` according to refund policy and payment state.

**Validation Rules**:

- Only the owning customer may view or act on the booking in webapp.
- Actions must be shown only when policy allows.
- Unavailable actions must be explained without exposing internal policy mechanics.

## Customer Subscription

Represents a marketplace-style recurring product purchase belonging to a signed-in customer.

**Fields**:

- `subscriptionId`: Stable subscription identifier.
- `organizationId`: Owning organization identifier.
- `organizationName`: Customer-visible organization name.
- `locationId`: Associated location identifier.
- `locationName`: Customer-visible location name.
- `productId`: Purchased recurring product identifier.
- `productName`: Customer-visible product name.
- `renewalSummary`: Customer-readable renewal or cycle summary.
- `paymentState`: Current customer-facing payment state.
- `subscriptionStatus`: Current customer-facing subscription state.
- `eligibleActions`: Policy-bound customer actions: `cancel`, `change`, `refund-request`, or none.

**Relationships**:

- Belongs to a signed-in customer.
- May materialize one or more customer bookings over time.

**State Transitions**:

- `active` → `cancel-at-period-end` when policy allows scheduled cancellation.
- `active` → `cancelled` when policy allows immediate cancellation.
- `active` → `changed` when policy allows customer plan or product changes.
- `paid` → `refund-requested` → `refunded` or `refund-unavailable` according to refund policy and payment state.

**Validation Rules**:

- Only the owning customer may view or act on the subscription in webapp.
- Cancellation, change, and refund options must be policy-bound.
- Private coworking-owner subscription management must remain out of webapp.

## Shared Entry Point

Represents a cross-product account workflow that remains reachable from webapp.

**Fields**:

- `pathPattern`: Shared route or workflow identifier.
- `purpose`: Authentication, callback, account settings, notifications, or similar cross-product purpose.
- `requiredByProducts`: Product apps that depend on the entry point.
- `customerVisible`: Whether it is visible to visitors or signed-in customers.

**Validation Rules**:

- Must be identified separately from customer marketplace and admin cleanup decisions.
- Must not become a back door to private organization administration.

## Cleanup Decision Record

Represents an approved change made to simplify webapp.

**Fields**:

- `capabilityId`: Capability being changed.
- `decision`: `kept`, `removed-from-navigation`, `served-in-place`, `unavailable-in-place`, `protected-unchanged`, or `deferred`.
- `approvedBy`: Stakeholder approval record or review reference.
- `implementedIn`: Change set or task reference.
- `beforeState`: Summary of existing behavior.
- `afterState`: Summary of resulting behavior.
- `verification`: Evidence that success criteria were checked.

**Validation Rules**:

- Must exist for each implemented keep/move/remove/defer decision.
- Must include no-redirect verification when path handling changes.
- Must include custom-subdomain regression verification when adjacent marketplace behavior is touched.
