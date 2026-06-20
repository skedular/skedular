# Feature Specification: Product Price Available Days

**Feature Branch**: `034-price-available-days`  
**Created**: 2026-07-18  
**Status**: Draft  
**Input**: User description: "Add optional product-price day-of-week restrictions that govern customer availability, booking validation, recurring booking generation, renewals, Skedular Host, Skedular Spaces, and public-web documentation while preserving unrestricted-price behavior."

## Clarifications

### Session 2026-07-18

- Q: How should a day-of-the-week rule edit affect already-purchased recurring subscriptions? → A: Preserve the available-day rule active at purchase for the current subscription period; apply the latest rule on renewal.
- Q: How should an available-day rule apply to a booking made partway through an allowed day? → A: It authorizes the chosen local booking date; the booking starts at the customer's selected time and retains its normal duration.
- Q: Does "weekday" mean workweek days only? → A: No. The rule applies equally to any day of the week, from Sunday through Saturday.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Configure a Price's Available Days (Priority: P1)

An administrator using Skedular Host can optionally select one or more days of the week for an individual product price, review the saved selection later, or leave the selection empty to allow every valid day.

**Why this priority**: Administrators need a clear, price-specific way to express when a product can be consumed without changing the rules of other prices for the same product.

**Independent Test**: Create and edit a product with unrestricted, Saturday-only, and Wednesday-and-Thursday prices; verify each price retains exactly its own day selection.

**Acceptance Scenarios**:

1. **Given** an administrator creates or edits a price, **When** they select Saturday only and save, **Then** that price shows Saturday as its only available day when reviewed.
2. **Given** a product has multiple prices, **When** the administrator configures available days for one price, **Then** the other prices' available-day rules remain unchanged.
3. **Given** an administrator leaves a price's day selection empty, **When** they save it, **Then** that price remains eligible on all days permitted by existing rules.

---

### User Story 2 - Find and Buy a Valid Date (Priority: P1)

A customer using Skedular Spaces can see a selected price's applicable available days and can choose only a date that satisfies the price rule as well as the existing availability rules.

**Why this priority**: Customers must receive accurate availability guidance and must not be able to purchase an invalid date through any client path.

**Independent Test**: For a Saturday-only price, attempt purchase flows for a Saturday and a non-Saturday, including a direct invalid submission; confirm only the Saturday can proceed when all other availability rules allow it.

**Acceptance Scenarios**:

1. **Given** a customer selects a Saturday-only price, **When** they view date availability in Skedular Spaces, **Then** the experience identifies Saturday as the available day and prevents practical selection of other days of the week.
2. **Given** a customer submits a booking request for a date outside the selected price's days, **When** the request reaches booking validation, **Then** it is rejected with a clear explanation and no booking is created.
3. **Given** a date is allowed by the price but cannot satisfy resource, opening-hours, conflict, or matching rules, **When** the customer attempts to book it, **Then** the booking remains unavailable under the existing rule that prevents it.
4. **Given** a customer chooses a time partway through an allowed local day, **When** the booking is created, **Then** it starts at the selected time and keeps its normal duration rather than being moved to the start of the day.

---

### User Story 3 - Generate Restricted Recurring Entitlement (Priority: P1)

A customer purchasing a recurring or multi-day price receives booking instances only on that price's allowed days throughout the applicable period; future generation and automatic renewal continue to honor the same rule.

**Why this priority**: The feature would be incomplete and financially misleading if a recurring purchase created bookings on disallowed days.

**Independent Test**: Purchase a six-month Saturday-only price and verify that every generated booking within the covered period is on a Saturday, including bookings created by later generation and renewal activity.

**Acceptance Scenarios**:

1. **Given** a customer purchases a six-month Saturday-only price, **When** booking instances are generated for the subscription period, **Then** an instance is considered only for Saturdays in the location's calendar and none are created for other days of the week.
2. **Given** a price allows Wednesday and Thursday, **When** instances are generated for a recurring period, **Then** only qualifying Wednesdays and Thursdays are considered for creation.
3. **Given** an eligible day of the week lacks a compatible available resource, **When** instances are generated, **Then** no instance is created for that date and existing resource-repair behavior remains intact.
4. **Given** an automatically renewable restricted price reaches renewal, **When** the next period is created, **Then** its generated instances continue to observe the price's selected days.

---

### User Story 4 - Understand Day-Restricted Products (Priority: P2)

Customers and administrators can understand what an available-day restriction means from Skedular Spaces, Skedular Host, and the public Skedular website documentation.

**Why this priority**: Clear explanation reduces mistaken purchases and helps operators configure prices consistently.

**Independent Test**: Review the saved Host price, the Spaces purchase flow, and public documentation for a multi-day restriction; each accurately states the allowed days and explains that resource availability still applies.

**Acceptance Scenarios**:

1. **Given** a price is restricted to particular days of the week, **When** it is shown in Skedular Spaces, **Then** customers can understand the applicable days before selecting a date or purchasing.
2. **Given** an administrator is editing a price in Skedular Host, **When** they review its settings, **Then** they can clearly identify and change the currently selected days.
3. **Given** a visitor reads public Skedular website documentation about subscriptions or booking generation, **When** day-restricted prices are relevant, **Then** the documentation explains their effect on bookable dates and recurring instances.

### Edge Cases

- A price with no selected available days must preserve its current behavior for one-time, recurring, renewal, and availability flows.
- The day of the week must be determined in the booking location's timezone, including dates around a UTC-day boundary and daylight-saving transitions where applicable.
- All seven days, Sunday through Saturday, are equally eligible for selection; the feature is not limited to workweek days.
- A product may have prices with overlapping, distinct, or unrestricted day selections; selection of one price must not inherit rules from another.
- Existing active subscriptions and historical bookings must remain valid and unchanged; each active subscription keeps the available-day rule that applied when its current period was purchased.
- If an administrator changes a price's selected days, the change must not silently rewrite already-created or future generated bookings in an active period; the latest rule applies when that subscription renews.
- A restricted date that is otherwise eligible for the price but lacks a resource must not create a booking merely because its day of the week is allowed.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST allow authorized administrators in Skedular Host to set an optional collection of one or more days of the week, selected equally from Sunday through Saturday, on each individual product price.
- **FR-002**: The system MUST allow an administrator to clear the available-day collection; an empty collection MUST mean that the price has no day-of-the-week restriction.
- **FR-003**: The system MUST preserve available-day selections independently for each price, including prices belonging to the same product.
- **FR-004**: The system MUST expose a price's available-day rule wherever Skedular Host and Skedular Spaces need it to display, select, or validate that price.
- **FR-005**: Skedular Spaces MUST clearly communicate a selected price's available-day restriction and prevent selection of disallowed dates wherever the experience can do so before submission.
- **FR-006**: The system MUST reject every booking attempt whose local booking date falls outside the selected price's available days, regardless of the requesting client.
- **FR-007**: The system MUST determine a booking date's day of the week using the applicable location timezone.
- **FR-007a**: For a booking made partway through an allowed local day, the available-day rule MUST authorize the selected local booking date without changing the customer's selected start time or the price's normal duration.
- **FR-008**: For a restricted recurring or multi-day price, the system MUST consider booking-instance creation only on allowed days of the week within the covered subscription period.
- **FR-009**: The system MUST apply the same available-day rule to future instance generation, repair or extension activities, and automatic renewal for restricted prices.
- **FR-009a**: The system MUST retain the available-day rule active when a subscription period was purchased for all generation within that period, and MUST use the latest price rule only when the subscription enters a renewal period.
- **FR-010**: The system MUST require both the price's available-day rule and all pre-existing resource availability, opening-hours, conflict detection, and product-resource matching rules before a booking can be created.
- **FR-011**: The system MUST retain existing pricing cadence, subscription, booking, renewal, validation, and availability behavior when a price has no available-day restriction.
- **FR-012**: The system MUST preserve existing active subscriptions, historical bookings, and other previously persisted data without requiring administrators to update unrestricted prices.
- **FR-013**: The system MUST make the available-day rule available to appropriate administrative and customer-facing views without exposing internal-only information.
- **FR-014**: The public Skedular website documentation MUST explain optional price-level day-of-the-week restrictions, their relationship to resource availability, and their effect on recurring booking generation.
- **FR-015**: The implementation work MUST document the current end-to-end price, purchase, availability, recurring-generation, and renewal flow, and identify all affected components before behavior changes are implemented.
- **FR-016**: The system MUST provide clear, actionable feedback when a requested date is unavailable because it violates the selected price's available-day rule.
- **FR-017**: The feature MUST include automated coverage for unrestricted prices, single and multiple day-of-the-week restrictions, invalid direct booking attempts, recurring generation, automatic renewal, timezone boundaries, unavailable resources, and a six-month Saturday-only price.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for the start and completion of restricted recurring booking generation and renewal workflows.
- **LOG-002**: Feature MUST emit structured logs when a candidate date is skipped because it is outside a price's available days, including booking or workflow correlation context and the evaluated local date.
- **LOG-003**: Feature MUST emit actionable warning or error logs when restricted-price validation or generation cannot complete, while preserving existing recovery behavior.
- **LOG-004**: Feature logs MUST include correlation context and MUST avoid sensitive customer data.

### Key Entities _(include if feature involves data)_

- **Product Price**: A purchasable pricing option for a product; it owns an optional collection of allowed days of the week independently of the product and other prices.
- **Available-Day Rule**: The selected days of the week associated with one price; when empty, it imposes no day-of-the-week constraint.
- **Booking Request**: A customer's requested date and selected price, evaluated against the available-day rule and existing booking rules.
- **Subscription Period**: The period covered by a recurring or multi-day purchase; it determines the date range in which allowed-day instances may be considered.
- **Booking Instance**: A resource reservation created only when its local date is allowed by the selected price and satisfies all existing availability requirements.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In acceptance testing, 100% of direct booking attempts for a date outside a selected price's allowed days of the week are rejected without creating a booking.
- **SC-002**: In acceptance testing of a six-month Saturday-only purchase, 100% of generated booking instances fall on Saturdays in the applicable location timezone and 0 are generated on other days of the week.
- **SC-003**: In acceptance testing of a Wednesday-and-Thursday price, 100% of generated instances are limited to those two days of the week, subject to existing resource availability.
- **SC-004**: Existing acceptance and regression coverage for an unrestricted price continues to pass with no changed customer-visible availability or generated-booking behavior.
- **SC-005**: In usability review, administrators can configure or clear a price's available-day rule and customers can identify a selected price's allowed days before booking without assistance.
- **SC-006**: Public documentation presents the available-day rule, its resource-availability dependency, and its recurring-generation effect accurately enough for a reviewer to complete the documented examples without additional product guidance.

## Assumptions

- Days of the week follow the local calendar of the relevant booking location, not a server or client UTC calendar.
- The existing product-price editing permissions continue to govern who may configure available days.
- The existing purchase, subscription, availability, conflict, opening-hours, resource-matching, and renewal behavior remains the source of truth except for the additional day-of-the-week eligibility check.
- Day restrictions are a property of a price, not a product-wide setting and not a separate recurrence model.
- Existing unrestricted prices are treated as having an empty available-day rule, which permits the price on every day while existing resource availability rules still apply, and require no administrator action or data correction.
- The affected user interfaces are Skedular Host for administration and Skedular Spaces for customer purchase; public Skedular website documentation complements rather than replaces in-product guidance.
- Documentation scope includes the existing public-site guidance for subscriptions and booking generation plus any cross-links needed for customers and administrators to discover the behavior.
