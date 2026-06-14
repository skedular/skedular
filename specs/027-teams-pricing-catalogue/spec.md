# Feature Specification: Skedular Teams Pricing Catalog Redesign

**Feature Branch**: `027-teams-pricing-catalogue`  
**Created**: 2026-06-12  
**Status**: Draft  
**Input**: User description: "Skedular Teams pricing, subscription, entitlement, and pricing catalog redesign. Build an end-to-end product implementation that reviews the existing pricing model first, decides whether to extend the current pricing version or introduce a new one, moves pricing display to a server-driven product-aware catalog, simplifies Teams offerings to Free, Pay As You Go, and Enterprise Contact Us, centralizes entitlement enforcement, leaves existing subscriptions untouched, and leaves a reusable foundation for Skedular Spaces."

## Clarifications

### Session 2026-06-12

- Q: How should downgrades behave when the target plan or capacity is lower than current usage? → A: No active paid users
- Q: What should happen to existing Early Bird organizations? → A: Leave unchanged
- Q: Should existing subscriptions be migrated, repaired, or otherwise changed? → A: No; existing subscriptions stay as-is and are used only for read-only compatibility checks.
- Q: How should other domains enforce pricing limits? → A: Organization publishes pricing/subscription state through existing events; other domains store that projected state locally, currently as a JSON block, and enforce limits through shared `Api.Shared.Services` models/code without runtime calls back to Organization.
- Q: Should performance goals be measurable? → A: Yes; catalog reads and entitlement checks need explicit validation targets.
- Q: How should Enterprise pricing be exposed and configured? → A: Public pricing shows Enterprise as Contact Us only; Skedular admins negotiate a per-active-user unit price and active-user cap, then set those terms through the Organization workaround REST API.
- Q: Where should plan limits come from after offering creation? → A: `Api.Shared.Services.Offering` remains the template used when creating/renewing an `OrganizationOffering`; the persisted offering row stores active-user, location, and team capacities used by downstream enforcement.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Review and Evolve Existing Pricing Safely (Priority: P1)

Product and engineering stakeholders need the existing pricing, subscription, enterprise, entitlement, customer-facing contract, versioning, and compatibility behavior reviewed before any redesign decision is finalized, so the redesign preserves working customer behavior instead of replacing it blindly.

**Why this priority**: This is the foundation for every later pricing and subscription change. Without a documented current-state review and versioning decision, the feature risks breaking existing customers and contracts.

**Independent Test**: Can be tested by reviewing the completed design decision record and confirming it covers existing models, subscriptions, enterprise behavior, entitlement enforcement, customer-facing dependencies, pricing version concepts, compatibility risks, and a justified decision to extend the current version or introduce a new catalog version.

**Acceptance Scenarios**:

1. **Given** the existing pricing implementation and active subscriptions, **When** the redesign is assessed, **Then** stakeholders can see a documented inventory of current pricing behavior and dependencies.
2. **Given** the current pricing version can be safely extended, **When** the versioning decision is recorded, **Then** the decision prefers extension and explains the compatibility guarantees.
3. **Given** extension is not safe, **When** a new pricing version is selected, **Then** the decision explains why, how versions coexist, how subscriptions are selected, and how existing customers remain supported.

---

### User Story 2 - Render Pricing From a Server-Driven Product Catalog (Priority: P1)

Prospective and existing customers need Teams pricing pages to reflect the catalog configured by the business, so plan names, descriptions, ordering, visibility, features, prices, capacities, recommendations, and Contact Us thresholds can change without a web redesign.

**Why this priority**: Server-driven pricing is the primary customer-facing outcome and removes pricing drift between business configuration and web presentation.

**Independent Test**: Can be tested by changing catalog data and verifying the Teams pricing experience renders the updated plans, capacities, order, prices, feature lists, recommendations, and Contact Us behavior without changing page-specific pricing copy or constants.

**Acceptance Scenarios**:

1. **Given** the Teams pricing catalog includes Free, Pay As You Go, and Enterprise offerings, **When** a customer views Teams pricing, **Then** the page presents those offerings in the configured order with configured copy, prices, features, limits, and availability.
2. **Given** Enterprise is a negotiated offering, **When** a customer views enterprise options, **Then** the page directs them to Contact Us and does not expose public capacity package pricing.
3. **Given** the negotiated Enterprise terms are changed by Skedular, **When** the organization offering is updated through the admin REST API, **Then** the stored unit price and active-user cap drive billing and entitlement behavior.

---

### User Story 3 - Subscribe Organizations to the Correct Teams Offering (Priority: P2)

Organization decision makers need to choose or remain on a valid Teams subscription that clearly records product, plan, capacity, catalog version, effective date, and status, so billing and entitlement decisions are consistent over time.

**Why this priority**: The catalog only becomes useful when organizations can be assigned to durable subscriptions that drive billing and access rules.

**Independent Test**: Can be tested by creating Free and Pay As You Go offerings and by setting Enterprise negotiated terms through the Skedular-admin Organization workaround REST API, then verifying the resulting organization offering stores the expected plan, unit price, currency, active-user cap, team/location caps, catalog version, effective date, and status.

**Acceptance Scenarios**:

1. **Given** a new organization selects Free, **When** the subscription is created, **Then** the organization receives Free limits for active users, teams, and locations.
2. **Given** a new organization selects Pay As You Go, **When** the subscription is created, **Then** the organization has unlimited teams and locations and is billed from actual monthly active usage.
3. **Given** Skedular negotiates Enterprise terms with an organization, **When** an admin sets the Enterprise offering with a unit price and active-user cap, **Then** the organization is billed monthly using that per-active-user unit price up to the purchased cap.
4. **Given** an existing organization has a legacy subscription, **When** compatibility checks run, **Then** the subscription remains unchanged and is honored through the existing compatibility path.
5. **Given** an existing organization has an Early Bird subscription, **When** compatibility checks run, **Then** the Early Bird subscription remains unchanged and continues to be honored.

---

### User Story 4 - Enforce User Capacity and Plan Entitlements Consistently (Priority: P2)

Organizations need plan restrictions enforced consistently across organization, booking, team, and location workflows, so customers receive the access they paid for and are prevented from exceeding active-user or Free-plan limits.

**Why this priority**: Correct entitlement enforcement protects revenue, prevents overuse, and avoids inconsistent behavior across product areas.

**Independent Test**: Can be tested by exercising organization, booking, team, and location workflows under each Teams offering and verifying the same entitlement outcome is applied consistently.

**Acceptance Scenarios**:

1. **Given** a Free organization already has 10 active users, **When** an eleventh user attempts a qualifying active-user action, **Then** the new active user is blocked and existing active users continue normally.
2. **Given** a Free organization already has one team or one location, **When** another team or location is created, **Then** the action is blocked with a clear entitlement outcome.
3. **Given** a Pay As You Go organization, **When** additional teams or locations are created, **Then** the actions are allowed without team or location limits.
4. **Given** an Enterprise Capacity organization has a negotiated purchased capacity, **When** users become active up to that capacity in the billing period, **Then** they are allowed; **When** one more user attempts to become active, **Then** that new activation is blocked.

---

### User Story 5 - Prepare the Pricing Framework for Spaces Reuse (Priority: P3)

Product owners need the pricing, subscription, entitlement, and catalog framework to support both Teams and Spaces, so future Spaces pricing can reuse the same product-aware foundation without another redesign.

**Why this priority**: Spaces implementation is not the first deliverable, but the data and behavior model must not be Teams-only.

**Independent Test**: Can be tested by confirming the catalog can represent multiple product offerings and can return Teams-only or Spaces-only catalog views while the first production pricing behavior is limited to Teams.

**Acceptance Scenarios**:

1. **Given** the catalog contains product offerings, **When** a caller requests Teams pricing, **Then** only Teams pricing is returned.
2. **Given** the catalog contains product offerings, **When** a caller requests Spaces pricing before Spaces plans are commercially launched, **Then** the response remains valid and does not expose incomplete Teams-specific assumptions.
3. **Given** future Spaces plans require different capacities, prices, or thresholds, **When** those catalog entries are added later, **Then** the existing catalog structure can represent them without changing the Teams pricing model.

### Edge Cases

- Existing organizations must not have current subscription state mutated by this feature.
- Existing subscriptions, including incomplete, missing, unknown, Early Bird, Free, or enterprise-style states, must be treated as read-only compatibility inputs.
- Existing Early Bird subscriptions must not be modified.
- Current compatibility planning may assume existing organizations are on Free or Early Bird rather than active paid usage plans; active paid-plan downgrade behavior is out of scope.
- Existing active users in an Enterprise Capacity organization must continue normal work after the organization reaches capacity; only newly active users are blocked.
- Free-plan team and location limits must continue to apply even if user activity is below the active-user limit.
- Paid plans must not accidentally inherit Free-plan team or location restrictions.
- Catalog versions must remain stable for existing subscriptions when future catalog prices, capacities, thresholds, or visibility rules change.
- Pricing pages must handle hidden, unavailable, deprecated, Contact Us, and recommended plan states using catalog data.
- Usage events that could qualify a user as monthly active must be consistently counted once per organization and billing period.
- Compatibility checks must be read-only so partial execution cannot create duplicate or conflicting subscription state.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The redesign MUST begin with a documented review of the existing pricing, subscription, enterprise, entitlement, customer-facing dependency, contract, versioning, and compatibility behavior.
- **FR-002**: The redesign MUST record a versioning decision that either extends the current pricing version or introduces a new pricing catalog version with documented reasons.
- **FR-003**: If the current pricing version is extended, the redesign MUST preserve backwards compatibility for existing customers, subscriptions, and frontend contracts.
- **FR-004**: If a new pricing version is introduced, the redesign MUST define coexistence rules, version selection rules, and backwards compatibility guarantees without mutating existing subscriptions.
- **FR-005**: The pricing catalog MUST be server-driven and product-aware.
- **FR-006**: The pricing catalog MUST support at least Skedular Teams and Skedular Spaces as product offerings, even though this feature implements Teams commercial behavior first.
- **FR-007**: Teams pricing MUST be represented as three commercial offerings: Free, Pay As You Go, and Enterprise Capacity.
- **FR-008**: Historical Enterprise package labels MUST NOT be represented as separate Teams plan types or public self-service packages; Enterprise MUST be represented as Contact Us behavior.
- **FR-009**: The Free offering MUST enforce a maximum of 10 active users, one team, and one location.
- **FR-010**: Pay As You Go MUST allow unlimited teams and locations, track monthly active users, and support billing from actual usage.
- **FR-011**: Enterprise Capacity MUST store and enforce the negotiated purchased active-user capacity as the primary commercial limit while keeping team and location capacities stored on the organization offering.
- **FR-012**: Enterprise Capacity MUST bill monthly from the negotiated per-active-user unit price and purchased active-user cap.
- **FR-013**: Enterprise Capacity MUST allow existing active users to continue normal work during the billing period after capacity is reached and MUST block only new users attempting to become active beyond purchased capacity.
- **FR-014**: The catalog MUST expose Enterprise as Contact Us and MUST NOT expose negotiated Enterprise unit prices or capacities publicly.
- **FR-015**: Enterprise unit price, currency, and purchased capacity MUST be set only through the Skedular-admin Organization workaround REST API.
- **FR-016**: Web pricing experiences MUST NOT hardcode plan names, plan descriptions, plan ordering, plan visibility, feature lists, pricing values, capacity options, Contact Us thresholds, recommended plans, or product offerings.
- **FR-017**: Web pricing experiences MUST render pricing and subscription choices from catalog responses.
- **FR-018**: Organizations MUST have offering/subscription state that records product offering, plan, unit price, currency, active-user capacity, team capacity, location capacity, pricing catalog version, effective date, and status.
- **FR-019**: Existing Free subscriptions MUST remain unchanged and compatible so every organization keeps its current subscription outcome.
- **FR-019a**: Existing Early Bird subscriptions MUST remain unchanged and continue to be honored for existing organizations.
- **FR-020**: Existing Free subscriptions MUST retain Free restrictions.
- **FR-021**: Existing Pay As You Go subscriptions MUST remain unchanged and supported with usage-based billing behavior preserved where present.
- **FR-022**: Existing enterprise-style subscriptions MUST remain unchanged and be honored through a documented compatibility path.
- **FR-023**: The active-user definition MUST include unique users within an organization who perform qualifying actions during a billing period.
- **FR-024**: Qualifying active-user actions MUST include creating bookings, updating bookings, owning bookings, participating in confirmed bookings, and any other meaningful usage events identified during the current-state review.
- **FR-025**: Entitlement enforcement MUST be centralized in shared `Api.Shared.Services` pricing models/code so organization, booking, team, and location workflows use the same outcome for the same projected subscription state.
- **FR-026**: The entitlement model MUST include a first-class User Capacity Quota concept with finite values and unlimited where applicable.
- **FR-027**: The catalog MUST allow callers to retrieve the full pricing catalog, Teams-only pricing, and Spaces-only pricing.
- **FR-028**: Catalog responses MUST include enough information for web experiences to render product offerings, version, plans, capacity options, prices, features, limits, visibility, availability, and display ordering.
- **FR-029**: New subscription lifecycle behavior MUST cover creation, upgrade, version compatibility, and future-ready change handling; downgrade of existing subscriptions is out of scope.
- **FR-030**: Automated tests MUST cover Free limits, Pay As You Go usage and unlimited team/location behavior, negotiated Enterprise Capacity enforcement, pricing catalog retrieval, version selection, Contact Us rendering, admin Enterprise offering updates, read-only existing-subscription compatibility, and cross-domain entitlement consistency.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers) and MUST avoid sensitive data leakage.
- **LOG-005**: Pricing version selection, read-only compatibility decisions, entitlement allow/block outcomes, and Contact Us threshold decisions MUST be logged with operator-useful context.

### Key Entities

- **Pricing Catalog**: The business-owned collection of pricing information available to customers and product experiences.
- **Pricing Catalog Version**: A stable version of catalog rules, offerings, prices, thresholds, and compatibility behavior.
- **Product Offering**: A sellable product area such as Skedular Teams or Skedular Spaces.
- **Subscription Plan**: A commercial plan within a product offering, such as Free, Pay As You Go, or Enterprise Capacity.
- **Capacity Option**: A contact-only Enterprise capacity placeholder in the public catalog; negotiated Enterprise capacity is stored per organization offering.
- **Plan Feature**: A customer-facing capability or benefit shown in pricing experiences.
- **Plan Limit**: A plan restriction such as active users, teams, locations, or unlimited capacity.
- **Plan Price**: The price and billing cadence associated with a plan or capacity option.
- **Plan Availability**: Visibility, self-service, recommended, unavailable, deprecated, or Contact Us state for a plan or capacity.
- **Organization Subscription**: The durable subscription assigned to an organization, including product offering, plan, purchased capacity, catalog version, effective date, and status.
- **User Capacity Quota**: The entitlement representing how many active users an organization may have during the relevant billing period.
- **Monthly Active User**: A unique organization user who performs qualifying activity during a billing period.
- **Entitlement Decision**: The allow, block, or compatibility outcome used by product workflows when enforcing subscription limits.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of Teams pricing page plan names, plan descriptions, display order, features, prices, availability, recommendations, and Contact Us behavior are sourced from catalog responses.
- **SC-002**: 100% of existing organizations retain their current subscription state after compatibility evaluation.
- **SC-003**: Free-plan enforcement blocks the eleventh active user, second team, and second location in automated acceptance coverage.
- **SC-004**: Pay As You Go acceptance coverage confirms unlimited team and location creation and monthly active-user usage tracking.
- **SC-005**: Enterprise Capacity acceptance coverage confirms a negotiated purchased capacity allows users up to capacity and blocks only newly active users beyond capacity.
- **SC-006**: At least 95% of pricing-page rendering checks pass using catalog-only data with no fixed page-specific pricing values.
- **SC-007**: New subscription creation, upgrade, and version compatibility workflows each have passing acceptance coverage before rollout.
- **SC-008**: Operators can identify pricing version selection, read-only compatibility decisions, and entitlement block reasons from structured logs for 100% of tested primary workflows.
- **SC-011**: Pricing catalog reads complete within 500 ms p95 under normal product-page load in automated validation.
- **SC-012**: Entitlement checks complete within 100 ms p95 for create/update workflows in automated validation.
- **SC-009**: The catalog can return both all-product pricing and product-specific pricing for Teams and Spaces without Teams-specific assumptions leaking into Spaces responses.
- **SC-010**: Existing customer pricing and entitlement behavior that is intentionally preserved by the versioning decision remains unchanged in regression testing.

## Assumptions

- Skedular Teams is the first product to receive full commercial behavior in this feature; Skedular Spaces support is framework-level unless separately scoped later.
- Existing customer subscriptions and billing behavior must remain authoritative and must not be changed by this feature.
- Existing organizations are currently on Free or Early Bird subscriptions; there are no active paid usage or capacity subscriptions that require downgrade handling.
- Early Bird subscriptions are out of scope for changes and should remain as-is for existing organizations.
- The business default is to extend the existing pricing version if the current-state review proves that is safe.
- Enterprise Capacity is negotiated by Skedular; the public catalog routes customers to Contact Us and the admin REST API stores the negotiated unit price and active-user cap.
- Paid Teams offerings do not restrict team or location counts.
- Monthly active user counting is scoped to one organization and one billing period.
- Active-user qualification may be expanded after reviewing existing usage events, but booking creation, booking updates, booking ownership, and confirmed-booking participation are mandatory minimum signals.
- The user-facing pricing experience uses American spelling and grammar, including "pricing catalog" in displayed copy.
