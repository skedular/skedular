# Feature Specification: Marketplace Pricing Cadence Simplification

**Feature Branch**: `047-marketplace-pricing-cadence`
**Created**: 2026-08-31
**Status**: Draft
**Input**: User description: "Redesign marketplace pricing so `PurchaseCadence` is the only cadence field. Remove `BookingCadence` entirely, remove sub-day cadence values, make booking duration depend only on min/max duration, and make credit entitlements cadence-free."

## Clarifications

### Session 2026-08-31

- Q: How should existing persisted pricing records that contain removed sub-day cadence values or `BookingCadence` be handled during migration? → A: Production has no pricing records using values shorter than one day, so remove the obsolete representations without a legacy data-conversion path.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Configure clear marketplace offer terms (Priority: P1)

As an organization administrator, I want each marketplace pricing option to express one offer or contract term so customers and downstream processes use the same meaning for pricing, renewal, and billing.

**Why this priority**: A single authoritative cadence prevents contradictory pricing definitions and is the foundation for all other behavior.

**Independent Test**: Create and read pricing options using each supported term, verify that unsupported terms and the removed booking cadence cannot be stored or exposed, and verify that auto-renewal alone controls repetition.

**Acceptance Scenarios**:

1. **Given** a pricing option, **When** an administrator selects a cadence, **Then** the only available values are Daily, Weekly, Fortnightly, Monthly, TwoMonths, Quarterly, FourMonths, FiveMonths, SixMonths, and Yearly.
2. **Given** a Daily pricing option with auto-renewal disabled, **When** a customer purchases it, **Then** it grants one one-day offer and does not renew.
3. **Given** a Daily pricing option with auto-renewal enabled, **When** its term reaches renewal, **Then** the system renews it for the next daily term.
4. **Given** a longer purchase term, **When** billing or resource booking slices are created, **Then** the organization billing cycle may determine those slices without changing the selected purchase term.

### User Story 2 - Book any valid duration within the offer limits (Priority: P1)

As a customer, I want to choose a start and end date/time freely within the offer’s allowed duration so I am not forced into cadence-based booking increments.

**Why this priority**: Booking duration is a customer-facing behavior and must be predictable independently of the commercial purchase term.

**Independent Test**: Use a date-time picker to submit durations below, within, and above the configured limits and verify acceptance or rejection while existing availability and conflict rules still apply.

**Acceptance Scenarios**:

1. **Given** minimum and maximum booking durations, **When** the customer selects a start and end date/time, **Then** the system calculates `Until - From` and accepts any duration within the inclusive range.
2. **Given** a selected duration shorter than the minimum, **When** the customer submits the booking, **Then** the booking is rejected with a clear validation error.
3. **Given** a selected duration longer than the maximum, **When** the customer submits the booking, **Then** the booking is rejected with a clear validation error.
4. **Given** a duration within the range, **When** opening hours, resource availability, or conflict rules are violated, **Then** those existing rules still reject the booking.

### User Story 3 - Purchase cadence-free credit entitlements (Priority: P1)

As a customer purchasing credits, I want the entitlement to be defined by credits and validity rules rather than by a recurring purchase cadence.

**Why this priority**: Entitlements are not recurring subscriptions; removing their artificial cadence prevents them from entering renewal or recurring-purchase workflows.

**Independent Test**: Create, purchase, use, expire, and inspect an entitlement and verify that its cadence is NotSet/null, its credit and validity rules remain available, and no subscription renewal is scheduled.

**Acceptance Scenarios**:

1. **Given** a credit-entitlement offer, **When** it is created or serialized, **Then** cadence is represented using the project’s NotSet/null representation and no hardcoded cadence default is added.
2. **Given** a purchased entitlement, **When** renewal or recurring purchase processing runs, **Then** the entitlement is excluded from those processes.
3. **Given** an entitlement, **When** its rules are evaluated, **Then** credit quantity, validity period, available days, and minimum/maximum booking duration determine eligibility.

### Edge Cases

- The migration may remove obsolete cadence fields and values directly because production contains no pricing records using terms shorter than one day; any unexpected legacy value encountered outside production must fail explicitly rather than be silently reinterpreted.
- Unknown or invalid cadence values received from any contract must be rejected or mapped to the project’s explicit invalid/NotSet behavior according to the owning contract, never guessed as a supported term.
- A non-renewing term must not create renewal work even when its term is longer than one day.
- A renewing offer must renew by its purchase term, while generated invoice and resource-booking slices may follow the organization billing cycle.
- Equal start and end times, reversed times, and date-time values that cross daylight-saving boundaries must produce a deterministic duration validation result.
- A valid duration can still fail because the location is closed, resources are unavailable, or a booking conflict exists.
- Entitlements with no cadence must remain purchasable and usable when their credit quantity and validity rules are valid.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The marketplace pricing model MUST use `PurchaseCadence` as its only cadence field; `BookingCadence` MUST be removed from pricing data, persistence, serialization, contracts, projections, services, workflows, frontend models, and tests.
- **FR-002**: The supported `ProductPricingCadence` values MUST be exactly Daily, Weekly, Fortnightly, Monthly, TwoMonths, Quarterly, FourMonths, FiveMonths, SixMonths, and Yearly.
- **FR-003**: The system MUST remove OneTime, PerMinute, Per15Minutes, Per30Minutes, PerHour, and HalfDay from all supported pricing-cadence representations and choice surfaces.
- **FR-004**: `PurchaseCadence` MUST represent the marketplace offer or contract term and MUST NOT be renamed in this feature.
- **FR-005**: Auto-renewal MUST determine whether the purchase term repeats. A non-renewing offer MUST create one term only; a renewing offer MUST renew using its `PurchaseCadence`.
- **FR-006**: Organization billing cycle MUST continue to control invoice and resource-booking slices inside longer purchase terms and MUST NOT replace or reinterpret `PurchaseCadence`.
- **FR-007**: Individual booking duration MUST be determined exclusively by `MinDurationMinutes` and `MaxDurationMinutes`; no cadence-based duration increment or booking-duration cadence is permitted.
- **FR-008**: The customer-facing booking flow MUST accept a start and end date/time, calculate `Until - From`, and reject durations shorter than the minimum or longer than the maximum.
- **FR-009**: Any duration within the configured minimum and maximum range MUST be eligible for duration validation, subject to existing opening-hours, resource-availability, and conflict rules.
- **FR-010**: Credit-entitlement offers MUST use the project’s NotSet/null cadence representation and MUST NOT receive a hardcoded cadence default.
- **FR-011**: Credit entitlements MUST be excluded from subscription auto-renewal and recurring purchase-cadence processing.
- **FR-012**: Credit entitlement eligibility MUST continue to use credit quantity, validity period, available days, and minimum/maximum booking duration.
- **FR-013**: Every affected persistence and contract surface MUST be migrated or regenerated so no active read/write path depends on `BookingCadence` or removed sub-day cadence values.
- **FR-014**: Existing and new automated tests MUST cover supported cadence choices, removed values, renewal behavior, billing/resource slicing, duration boundaries, entitlement cadence absence, and exclusion from renewal processing.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs for pricing validation and purchase-term renewal decisions.
- **LOG-002**: The feature MUST emit structured logs when duration validation accepts or rejects a requested interval and when existing availability/conflict rules reject it.
- **LOG-003**: The feature MUST emit actionable warning/error logs for invalid legacy or unknown cadence data and migration/recovery paths.
- **LOG-004**: Logs MUST include correlation context and MUST NOT include sensitive customer, payment, or credential data.

### Key Entities

- **ProductPricing**: A marketplace offer containing price, purchase term, renewal setting, entitlement attributes, and booking duration limits.
- **ProductPricingCadence**: The finite set of supported offer/contract terms of one day or longer.
- **Marketplace subscription**: A renewable purchase whose repeated terms are controlled by the offer’s purchase cadence and auto-renewal setting.
- **Credit entitlement**: A non-cadenced purchase providing credits governed by quantity, validity, available days, and booking duration limits.
- **Organization billing cycle**: An organization-level rule used to slice invoicing and resource bookings inside a longer purchase term.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of newly created marketplace pricing options expose exactly one cadence field and only the ten supported cadence values.
- **SC-002**: 100% of booking-duration validation cases accept every interval within the configured inclusive range and reject every interval outside it, before availability and conflict checks are applied.
- **SC-003**: 100% of non-renewing offers create no renewal work, and 100% of renewing offers use the selected purchase term for renewal decisions in acceptance tests.
- **SC-004**: 100% of credit-entitlement purchase and lifecycle tests show NotSet/null cadence and no subscription auto-renewal or recurring purchase processing.
- **SC-005**: No supported production contract, persisted pricing record, generated client model, or test fixture references `BookingCadence` or a removed sub-day cadence value after migration and regeneration complete.
- **SC-006**: Customers can complete a valid date-time booking without selecting or satisfying a cadence-based duration increment, subject only to the existing business rules.

## Assumptions

- `MinDurationMinutes` and `MaxDurationMinutes` are inclusive bounds unless the existing domain contract explicitly defines another boundary; acceptance tests will make the chosen boundary behavior explicit.
- Existing auto-renewal, billing-cycle, opening-hours, availability, conflict, credit, and validity concepts remain in place; this feature changes their cadence relationships rather than replacing them.
- Production contains no pricing records using removed sub-day cadence values or `BookingCadence`, so no legacy conversion or backfill is required; unexpected legacy values must be surfaced explicitly during migration or validation.
- A NotSet/null representation already exists for cadence-free entitlements and will be used consistently across affected contracts.
- `PurchaseCadence` remains the public name for this feature; a future rename to `PurchaseTerm` is outside scope.
- The feature includes all repository-owned backend and frontend contract consumers, generated artifacts, projections, workflows, and tests required to keep the system consistent, but does not redesign unrelated marketplace pricing rules.
