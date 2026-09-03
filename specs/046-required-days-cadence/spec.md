# Feature Specification: Required Days Across Longer Cadences

**Feature Branch**: `046-required-days-cadence`
**Created**: 2026-08-31
**Status**: Clarified for implementation
**Input**: Research and implement support for `requiredDaysPerWeek` on offer cadences longer than one week, including credit-entitlement offers where appropriate.

## Decision

The existing field has coherent semantics for weekly offers: an exact number of distinct selected calendar weekdays, constrained by `availableDays`. The expanded rule uses complete UTC calendar weeks for every longer-than-weekly purchase cadence. Reservations and generated subscriptions require exactly N booking occurrences per complete week; credit entitlements enforce at most N successful redemptions per complete week because customers may legitimately use fewer credits.

The field name remains appropriate. No ProductPricing migration is required; enforcement needs a shared UTC-week rule and durable entitlement redemption counting.

## Clarifications

### Session 2026-08-31

- Q: Should `requiredDaysPerWeek` mean exactly N bookings per calendar week, with partial weeks at the beginning or end of a longer offer exempt? → A: Yes; use exactly N bookings per calendar week, constrained by `availableDays`, with boundary partial weeks exempt.
- Q: Should this rule apply to both reservation offers and credit-entitlement offers? → A: Yes; support both, with entitlement redemption counting confirmed redemptions in each complete calendar week.
- Q: Should the new rule apply to every purchase cadence longer than one week, including all existing fortnightly, monthly, multi-month, and yearly cadence values? → A: Yes; support every existing purchase cadence whose period is longer than one week.
- Q: What timezone should define calendar weeks for location-independent offers? → A: UTC; use UTC calendar weeks for v1 so reservations, subscriptions, and credit redemptions share one stable rule without storing location state.
- Q: For credit-entitlement offers, should `requiredDaysPerWeek` enforce a maximum of N redemptions per complete UTC week rather than require exactly N redemptions? → A: Yes; scheduled reservations/subscriptions use exactly N, while entitlements allow at most N redemptions.
- Q: Should scheduled offers derive bookings from the customer’s selected weekdays while entitlement offers simply block redemption after N weekly credits are spent? → A: Yes; selected weekdays define reservation/subscription bookings, while entitlement redemption is limited by the weekly count and does not ask the customer to select weekdays.
- Q: When should an entitlement redemption stop consuming the weekly allowance? → A: Count successful redemption bookings; release canceled or permanently failed redemptions.
- Q: Should the start and end of a longer offer’s eligible period be based on the purchase or subscription period timestamps? → A: Yes; use the actual period timestamps, enforce only full Monday-Sunday UTC weeks, and exempt first/last partial weeks.
- Q: Should `requiredDaysPerWeek` be enabled for every booking cadence when the purchase cadence is longer than one week, including hourly and other time-based bookings? → A: Yes; support every booking cadence and count booking occurrences, including time-based bookings. **Historical clarification superseded by Spec 047**; sub-day cadences and `BookingCadence` are out of scope for this feature.

### Session 2026-09-02

- Q: For cadence-free credit entitlements, should `requiredDaysPerWeek` apply across each complete UTC week within the entitlement’s validity period, independent of purchase cadence? → A: Yes; use the entitlement validity period as the active window and enforce at most N successful redemptions per complete UTC week.
- Q: Should `requiredDaysPerWeek` remain available for weekly offers and purchase cadences longer than one week, but be hidden for daily offers? → A: Yes; keep it for weekly and longer purchase cadences, hide it for daily offers, and use entitlement validity periods separately.
- Q: Should all active references to entitlement usage use “successful redemption”? → A: Yes; use successful redemption for a redemption that creates a booking and consumes one credit.
- Q: Should `BookingCadence` be removed from this feature’s data model and documentation? → A: Yes; booking duration follows Spec 047’s duration rules.
- Q: Should FR-006c say “Weekly and longer supported purchase cadences”? → A: Yes.
- Q: Should direct reservation creation enforce the same selected-weekday-to-booking-occurrence rule as subscription generation? → A: Yes; both flows create one occurrence per selected weekday per applicable complete UTC week.
- Q: Should refund-release behavior be covered primarily by unit tests, with focused repository integration only when necessary? → A: Yes.
- Q: Should all entitlement-limit references use the term “successful redemption” instead of “confirmed redemption”? → A: Yes; use “successful redemption” consistently, meaning a redemption that creates the booking and consumes one entitlement credit.
- Q: Should each customer-selected weekday create one booking occurrence in every applicable complete UTC week? → A: Yes; each selected weekday produces one booking occurrence per applicable complete UTC week, with existing duration rules unchanged.
- Q: Should the implementation include explicit tests for bookings exactly at the inclusive period start and exclusive period end? → A: Yes; cover `[start, end)` and complete/partial UTC weeks with unit tests wherever possible.
- Q: Should refunded entitlement redemptions be covered to verify that weekly allowance is restored? → A: Yes; cover refund release primarily with unit tests and add focused repository-backed integration coverage only when persistence cannot be proven by unit tests.

The implementation scope is therefore expanded to every supported cadence from Weekly onward, while cadence-free credit entitlements use their validity period as the active window. UTC calendar-week counting applies to both fulfillment types. The same nullable field and existing propagation paths remain the source of truth; no migration is required for existing data.

## User Scenarios & Testing

### User Story 1 - Preserve weekly offers (Priority: P1)

An operator can continue configuring an exact number of selected days for weekly offers, and customers can select exactly that number.

**Independent Test**: Create a weekly offer with two available weekdays and required days of two; verify valid and invalid selections.

**Acceptance Scenarios**:
1. Given a weekly offer with `requiredDaysPerWeek = 2`, when two distinct available weekdays are selected, then the booking is accepted.
2. Given the same offer, when fewer, more, duplicate, or unavailable weekdays are selected, then the booking is rejected.

### User Story 2 - Enforce longer-cadence schedules (Priority: P1)

An operator can configure longer calendar-based offers and customers receive the promised weekly booking behavior.

**Independent Test**: Configure a longer-cadence offer and verify complete UTC weeks enforce the rule.

**Acceptance Scenarios**:
1. Given a longer cadence with N required days, when a complete UTC week has exactly N eligible bookings, then the schedule is accepted.
2. Given a partial boundary week, when fewer than N bookings occur, then the schedule is not rejected for that reason.

### User Story 3 - Limit credit-entitlement redemptions (Priority: P1)

As a customer using a credit entitlement, I can redeem credits without selecting weekdays, while the system prevents more than N successful redemptions in a complete UTC week and restores the allowance when a redemption is released.

**Independent Test**: Redeem credits through a complete UTC week and verify success, weekly-limit rejection, cancellation/permanent-failure release, refund release, and idempotent retry behavior; verify the UI shows the remaining weekly allowance.

**Acceptance Scenarios**:
1. Given an active cadence-free entitlement with `requiredDaysPerWeek = N`, when a redemption creates a booking successfully, then it consumes one weekly allowance and is counted exactly once.
2. Given N successful redemptions in the same complete UTC week, when another redemption is attempted, then it is rejected without consuming a credit.
3. Given a successful redemption that is canceled, permanently fails, or is refunded, when the weekly allowance is recalculated, then that redemption no longer counts.
4. Given a retry of the same redemption request, when the original request already succeeded, then the retry is idempotent and does not count or consume another credit.
5. Given an entitlement with a weekly limit, when its details are displayed, then the customer can see the remaining allowance for the current applicable UTC week.

### Edge Cases

- Empty `availableDays` continues to mean every calendar day for weekly validation.
- Existing serialized records without the field continue to deserialize as null.
- Entitlement redemption validates the selected date and at-most-N weekly aggregate.
- Subscription renewal preserves and generates the longer-cadence weekly schedule.

## Requirements

### Functional Requirements

- **FR-001**: The system MUST preserve the existing exact-count semantics for weekly `requiredDaysPerWeek`.
- **FR-002**: The system MUST allow `requiredDaysPerWeek` on Weekly and longer supported purchase cadences and interpret it as exactly that many booking occurrences in each complete calendar week covered by the offer; cadence-free entitlements use their validity period instead.
- **FR-003**: The system MUST preserve the value through the existing model, JSON serialization, marketplace event projection, booking projection, and product-version matching.
- **FR-004**: The system MUST expose the existing field in product editors and read models for weekly and longer purchase cadences, hide it for daily offers, and support it for cadence-free entitlements through validity-period rules.
- **FR-005**: The system MUST preserve existing weekly validation for direct bookings, subscriptions, and entitlement redemption dates; longer-cadence behavior is defined by FR-005a through FR-006c.
- **FR-005a**: The system MUST enforce exactly N weekly booking occurrences for reservations and subscriptions, and at most N successful weekly redemptions for credit entitlements, using UTC history within each complete calendar week.
- **FR-005b**: The system MUST preserve the existing weekday-selection UI for scheduled offers and MUST NOT require weekday selection for credit-entitlement redemption.
- **FR-005c**: The system MUST count successful entitlement redemption bookings toward the weekly maximum and release the allowance when a redemption is canceled or permanently fails.
- **FR-005d**: The system MUST create one scheduled booking occurrence for each customer-selected weekday in every applicable complete UTC week.
- **FR-005e**: The system MUST treat offer periods as `[start, end)` and include unit coverage for exact boundaries and complete/partial UTC weeks.
- **FR-005f**: The system MUST release weekly entitlement allowance for refunded redemptions and cover the release behavior with unit tests, adding focused persistence integration coverage only where necessary.
- **FR-005g**: The system MUST expose the remaining weekly entitlement allowance in the entitlement read model and customer UI without requiring weekday selection.
- **FR-006b**: The system MUST derive complete-week eligibility from the actual purchase or subscription period timestamps, not calendar-month boundaries.
- **FR-006c**: The system MUST support Weekly and longer supported purchase cadences; daily offers do not expose the setting, and cadence-free entitlements are governed by validity rather than purchase cadence.
- **FR-006**: The system MUST exempt partial calendar weeks at the beginning and end of a longer offer from the exact weekly requirement.
- **FR-006a**: The system MUST use UTC calendar-week boundaries for all supported reservation bookings, subscriptions, and credit-entitlement redemptions without storing or consulting a booking or location timezone.

### Observability and Logging Requirements

- **LOG-001**: Validation warnings for invalid required-day values or blocked weekly eligibility MUST identify the pricing or entitlement identifier and UTC week without sensitive customer data.
- **LOG-003**: Unit tests for pricing validation, blocked redemption, allowance release, and idempotent retry MUST assert the relevant structured logging outcome.
- **LOG-002**: Existing booking and renewal transition logging MUST remain unchanged.

## Key Entities

- **ProductPricing**: The versioned offer pricing rule containing purchase cadence, booking duration limits, available weekdays, and the existing weekly exact-day requirement.
- **Entitlement**: A purchased credit balance whose redemption uses the offer’s date-availability rules and whose successful redemptions are counted against an at-most-N weekly limit.
- **Recurring subscription**: A recurring offer whose generated bookings inherit the pricing rule and selected weekly schedule.

## Success Criteria

### Measurable Outcomes

- **SC-001**: All existing weekly required-day tests continue to pass with no behavior change.
- **SC-002**: 100% of scheduled-offer configurations on supported cadences validate `requiredDaysPerWeek` against selected and available weekdays before persistence; cadence-free entitlements validate the value as a weekly successful-redemption limit.
- **SC-003**: No customer can be shown a longer-cadence setting that the system cannot enforce consistently.
- **SC-004**: Reservation, subscription, and entitlement flows enforce the same UTC weekly boundary without requiring location selection or entitlement timezone state.

## Assumptions

- “Required” means exact count for scheduled reservations/subscriptions; for credit entitlements it is an upper bound because redemption is optional.
- `availableDays` is a set of allowed calendar weekdays, not a quota.
- Weekly and longer supported purchase cadences are eligible, including weekly, fortnightly, monthly, two-, three-, four-, five-, and six-month, quarterly, and yearly values; Daily hides the setting and cadence-free entitlements use validity.
- UTC is the single calendar-week timezone for v1; location selection is not required.
- A Booking-owned durable query or usage record may be added to enforce entitlement redemption limits.

## Research Findings and Source-of-Truth Files

- Shared model: `src/shared/Api.Shared.Services/Models/ProductPricing.cs`.
- Marketplace validation: `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`.
- Marketplace event mapping: `src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs`.
- Booking projection: `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`.
- Weekly selection enforcement: `src/booking/shared/Booking.Shared/Services/MarketplaceBookingWeeklyDaySelectionService.cs`.
- Entitlement date enforcement: `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementBookingService.cs`.
- Subscription generation/renewal: `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.
- GraphQL source is the Marketplace API schema; exported schemas and Relay artifacts are generated.
- Host and Spaces editors currently clear and serialize the field only for `WEEKLY`.
