# Feature Specification: Skedular Spaces Pricing Implementation

**Feature Branch**: `028-skedular-spaces-pricing`  
**Created**: 2026-06-14  
**Status**: Draft  
**Input**: User description: "# Skedular Spaces Pricing, Subscription, Entitlement, Backend, and Web App Implementation"

## Clarifications

### Session 2026-06-14

- Q: Should automated billing-period rollover be explicitly in scope for this feature? → A: In scope, using the same first-day-of-month Temporal activity pattern as Skedular Teams.
- Q: Which frontend scope should this feature explicitly cover for Spaces pricing and upgrade behavior? → A: Spaces app must show server-driven pricing, quota status, and upgrade/contact prompts; checkout/subscription mutation uses existing flows only.
- Q: What pricing catalogue versioning strategy should Spaces use? → A: Use the same pricing catalogue infrastructure as Teams, but keep product versions independent: Teams offerings use `TEAMS_V1`, and Spaces/co-working marketplace offerings use `SPACES_V1`.

## User Scenarios & Testing

### User Story 1 - Free Plan with Monthly Booking Instance Quota (Priority: P1)

An organization signs up for the free tier of Skedular Spaces. They can create bookings freely as long as they stay within their monthly booking instance quota. When they exceed the quota, new booking attempts are rejected with a clear upgrade message.

**Why this priority**: This is the foundation of the pricing model - every Spaces customer starts on Free and must be able to understand when they've hit their limit.

**Independent Test**: Can be fully tested by creating bookings until quota is exceeded and verifying rejection with proper error code. Delivers value as a functional free tier.

**Acceptance Scenarios**:

1. **Given** an organization on the Free plan with a 100 booking instance monthly quota, **When** they create 50 bookings, **Then** all are successful
2. **Given** an organization on the Free plan at 99 booking instances, **When** they attempt to create 2 more bookings, **Then** only the first succeeds and the second is rejected with a quota exceeded error
3. **Given** quota exceeded error response, **When** displayed in the frontend, **Then** it shows upgrade options from the backend pricing catalogue

---

### User Story 2 - Paid Plan (Growth/Business) with Usage-Based Billing (Priority: P1)

An organization upgrades to a paid plan (Growth or Business). They are charged based on their monthly booking instance volume. The system correctly tracks usage and allows bookings within their quota.

**Why this priority**: Revenue generation is critical - paid plans must work correctly from day one.

**Independent Test**: Can be tested by upgrading an organization and verifying usage tracking and quota enforcement. Delivers immediate commercial value.

**Acceptance Scenarios**:

1. **Given** an organization on the Growth plan with a 500 booking instance monthly quota, **When** they create 400 bookings, **Then** all are successful
2. **Given** usage has reached the quota limit, **When** attempting to create another booking, **Then** the system rejects it and returns available upgrade plans
3. **Given** a billing period rollover, **When** the new month starts, **Then** current usage is calculated from booking instances scheduled in the new billing period only

---

### User Story 3 - Rebooking Within Quota (Priority: P2)

A user needs to modify an existing booking that doesn't create additional instances. This should not count against the quota since no new instance is created.

**Why this priority**: Common user workflow - users frequently need to reschedule or modify bookings without consuming additional capacity.

**Independent Test**: Can be tested by updating a booking and verifying the current-period booking count does not increase. Delivers value as an improved user experience.

**Acceptance Scenarios**:

1. **Given** an organization with 80/100 quota used, **When** they update an existing booking (rebook), **Then** the operation succeeds without consuming additional quota
2. **Given** a recurring booking instance override, **When** modifying an individual instance, **Then** only actual new instance creation consumes quota

---

### User Story 4 - Recurring Booking Instance Generation (Priority: P1)

A recurring booking is set up that will generate multiple instances over time. Each generated instance counts toward the monthly quota.

**Why this priority**: Recurring bookings are a primary use case; ensuring they don't unexpectedly exceed quotas is critical for customer satisfaction.

**Independent Test**: Can be tested by creating a recurring booking and verifying each generated instance counts against quota. Delivers value with automated scheduling.

**Acceptance Scenarios**:

1. **Given** an organization with 95/100 quota used, **When** a recurring booking generates 3 new instances, **Then** only the first 2 succeed and the third is rejected
2. **Given** a recurring booking with monthly occurrence count, **When** reviewing billing period usage, **Then** all generated instances are counted

---

### User Story 5 - Admin Override for Enterprise Customers (Priority: P3)

An organization has negotiated an Enterprise plan with custom capacity. An admin can manually adjust their subscription without being blocked by standard quota checks.

**Why this priority**: Required for enterprise sales team to manage custom contracts, but not needed for standard self-serve flows.

**Independent Test**: Can be tested by admin manually updating a customer's offering and verifying quota override. Delivers value via direct sales support.

**Acceptance Scenarios**:

1. **Given** an organization on Enterprise plan with negotiated 2000 capacity, **When** admin updates their offering, **Then** the system respects the custom limit
2. **Given** migration/default assignment has not produced a valid Spaces subscription state, **When** a booking is attempted, **Then** it is rejected with an operator-facing subscription state missing error

---

### Edge Cases

- **No active subscription**: Organizations without a Spaces subscription are assigned to the Free plan during migration/setup. If booking creation still finds missing Spaces subscription state after that prerequisite, the booking is rejected with an operator-facing subscription state missing error.
- **Timezone handling**: Billing periods use UTC day boundaries (midnight to midnight UTC), with monthly rollover on the first day of each month.
- **Out-of-period booking instances**: Booking instances scheduled outside the current billing period are not included in the current period's usage calculation, even when they are created during the current period.
- **Booking instance generation failure**: If quota check passes but instance creation fails, usage is not counted because no booking row was persisted, and the error is logged for investigation.
- **Downgrade below current usage**: Downgrading a plan that results in a lower quota than current usage is allowed; the organization continues with their current usage until billing period rollover.
- **Cancelled bookings**: Once created, booking instances are counted regardless of cancellation status. The monthly usage count reflects total instances ever created for that period.

## Requirements

### Functional Requirements

**FR-001**: System MUST define four Skedular Spaces pricing plans: Free, Growth, Business, and Contact Us.

**FR-002**: System MUST configure each plan with a monthly booking instance quota through the backend pricing catalogue.

**FR-003**: System MUST track the number of booking instances scheduled within the current billing period for each organization.

**FR-004**: Before creating any new booking for a Spaces organization (marketplace organization), system MUST calculate: (current usage for booking instances scheduled within the current billing period + new booking instances scheduled within the current billing period that will be created) and compare against plan quota. A "new booking instance" is defined as each individual booking record that will be stored in the database for a single API request.

**FR-004a**: Quota checks MUST be close to real time by asynchronously counting Booking-owned persisted booking rows scheduled within the current billing period before creation. Minor concurrent overage is acceptable; the feature MUST NOT require a separate atomic usage counter or raw SQL update against replicated organization offering JSON.

**FR-005**: If a booking would exceed the quota, system MUST reject the request with an error response containing:
  - Error code indicating quota exceeded
  - Current usage count
  - Quota limit
  - Available upgrade plans from the pricing catalogue

**FR-006**: System MUST only count actual booking instance creation toward the monthly quota. Updating an existing booking record (changing dates, participants, resources) does not create a new instance.

**FR-006a**: System MUST NOT count booking instances whose scheduled start falls outside the current billing period when evaluating the current period's quota.

**FR-007**: A booking request creates multiple instances when:
  - Multi-slot bookings span multiple days or time periods
  - Recurring booking instance generation creates new records
  The system MUST count each distinct booking record scheduled within the current billing period toward quota.

**FR-008**: Recurring booking instance generation MUST be validated against the quota before creating each instance.

**FR-009**: System MUST support billing period boundaries (monthly start/end dates) for usage tracking.

**FR-009a**: System MUST roll over Spaces usage on the first day of each month using the same Temporal activity behavior as Skedular Teams.

**FR-010**: All booking creation paths MUST enforce quota:
  - One-off bookings
  - Multi-slot bookings
  - Recurring bookings
  - Subscription-generated bookings
  - Admin-created bookings

**FR-011**: The pricing catalogue API MUST return all plan details without hardcoding in the frontend:
  - Plan names, descriptions, display order
  - Monthly booking instance quotas
  - Prices and billing cadences
  - Contact Us thresholds
  - Recommended plan indicator
  - Visibility rules

**FR-011a**: Spaces pricing MUST use the existing Organization pricing catalogue infrastructure with an independent `SPACES_V1` catalogue version for Spaces product and offering data. Teams offerings continue to use `TEAMS_V1`.

**FR-011d**: Organization admin subscription surfaces MUST return available offerings for the organization's product family only: private/Teams organizations receive Teams offerings, and marketplace/Spaces organizations receive Spaces offerings.

**FR-011b**: Organization offerings MUST support a persisted discount percentage from 0 through 100. The discount defaults to 0, is applied when calculating the amount charged for the offering, and is copied to renewed offering periods until an admin updates or resets it.

**FR-011c**: A 100% discount MUST result in a zero charge for the billing period, but it MUST NOT change the underlying offering code, catalog price, fixed price, unit price, capacity, or quota.

**FR-012**: System MUST assign existing organizations to a valid Spaces pricing state during migration.

**FR-012a**: If booking creation finds no valid Spaces subscription state after migration/default assignment should have completed, system MUST reject the booking instead of lazily assigning Free during booking creation.

**FR-013**: The entitlement service MUST answer for any organization:
  - Current plan ID
  - Monthly booking instance quota
  - Usage in current billing period
  - Remaining quota
  - Whether a specific booking can be created

**FR-013a**: Current Spaces quota usage/status MUST be exposed through Booking-owned API/GraphQL surfaces because Booking owns booking-instance usage.

**FR-014**: System MUST NOT enforce quotas on:
  - Locations count
  - Resources count
  - Desks count
  - Rooms count
  - Equipment count
  - Products count
  - Customers count
  - Subscriptions count
  - Memberships count

**FR-015**: Frontend applications MUST load all pricing data from the backend API; no pricing values are hardcoded.

**FR-016**: The Spaces frontend MUST show server-driven pricing, current quota status, and upgrade or contact prompts when quota enforcement blocks booking creation. New checkout or subscription mutation flows are out of scope except where existing subscription flows already support them.

---

### Observability and Logging Requirements

**LOG-001**: Feature MUST emit structured logs when a booking is rejected due to quota exceeded, including organization ID, current usage, quota limit, and plan code.

**LOG-002**: Feature MUST emit structured logs for billing period boundary transitions (start of new month).

**LOG-003**: Feature MUST emit structured logs for pricing catalogue retrieval with product offering code filtering.

**LOG-004**: Feature logs MUST include correlation context (organization ID, booking IDs where applicable) and avoid sensitive data leakage.

---

### Key Entities

- **Booking Instance**: A single booking record created by the Skedular Spaces booking engine. Counts toward monthly quota.
- **Current-Period Booking Instance**: A booking instance whose scheduled start falls within the current UTC billing period and is included in that period's quota usage.
- **Billing Period**: Monthly window for usage tracking with defined start and end dates.
- **Organization Subscription/Offering**: The current plan assignment and capacity configuration for an organization.
- **Offering Discount Percentage**: Persisted percentage discount on an organization offering. Defaults to 0, applies during billing amount calculation, and carries forward on offering renewal until changed.
- **Pricing Catalogue**: Server-driven configuration of all available plans, quotas, and prices.
- **Entitlement Record**: Current state of an organization's quota usage and remaining capacity.

---

## Success Criteria

**SC-001**: Users see immediate feedback when attempting to create a booking that would exceed their monthly quota.

**SC-002**: Frontend displays upgrade options that match backend pricing catalogue without manual synchronization.

**SC-003**: All organizations have a valid Spaces subscription state within 24 hours of migration completion.

**SC-004**: Recurring booking instance generation creates no more instances than the organization's remaining monthly quota allows and returns a quota-exceeded outcome for each blocked instance.

**SC-005**: Close-to-real-time quota checks block normal booking creation once persisted current-period booking counts reach quota; minor concurrent overage is accepted by design.

---

## Assumptions

- Existing organization offering/subscription infrastructure can be extended to support Spaces pricing
- Spaces pricing uses an independent `SPACES_V1` catalogue version without breaking Teams `TEAMS_V1` pricing behavior.
- Discounted trials or customer-specific discounts can be represented as an offering-level percentage discount without introducing a separate discount table or changing catalog prices.
- Billing period tracking uses existing monthly calendar boundaries and the same first-day-of-month Temporal activity pattern as Skedular Teams
- Booking instance counting aligns with existing booking creation workflow in PrivateBookingService and recurring booking workflows
- Usage counting is based on booking instance scheduled start within the current UTC billing period, not merely the request creation time.
- No separate subscription product is needed - spaces are offered as a plan within the existing offering system
- Frontend can consume the same PricingCatalog API used by Teams pricing
- Spaces frontend scope is limited to server-driven pricing display, quota status, and upgrade/contact prompts; checkout and subscription changes reuse existing supported flows.

---

## Dependencies

- Existing Teams pricing V1 implementation and catalogue model
- Existing offering/subscription database tables and models
- Booking creation workflow in PrivateBookingService
- Recurring booking instance generation workflow (BookPrivateRecurringResources)
- Organization authorization for subscription management
