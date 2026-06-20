# Feature Specification: Weekly Price Day Selection

**Feature Branch**: `035-weekly-day-selection`
**Created**: 2026-07-21
**Status**: Draft
**Input**: User description: "Add weekly product pricing with an exact required customer-selected booking-day count per week, while keeping available days separate, preserving existing behavior, and updating public-web documentation."

## Scope Decision

The first iteration supports a **fixed weekly booking pattern**. A customer selects a valid set of weekdays once while purchasing an eligible weekly price, and that same set repeats throughout the resulting subscription or recurring booking period. This satisfies the requested behavior where a customer, for example, selects Tuesday and Thursday as their recurring days.

This iteration does **not** provide a flexible weekly entitlement that lets customers choose different weekdays in each individual week. That behavior would require separate customer-managed scheduling, entitlement tracking, and week-by-week booking allocation; it must be specified separately rather than implied by this feature. An administrator may make an explicit change only to an individual generated booking; that change does not rewrite the customer’s recurring weekday pattern.

The configuration introduced here is explicitly **weekly-specific**: it represents one exact required day count **per week**, not a generic number-of-days-per-period rule. It must remain separate from any future fortnightly, monthly, or other cadence-specific configuration. Those future cadences will receive their own parameters, behavior, validation, and implementation in separate specifications.

## Clarifications

### Session 2026-07-21

- Q: What happens when a resource cannot be booked on a customer-selected weekday? → A: The system creates a resource-less booking shell on that selected date; it never substitutes another weekday and is visible to both the customer and the space administrator.
- Q: How is payment handled while a booking shell has no resource? → A: Payment is retained while the system continues automatic resource repair or an administrator resolves the individual booking; it is not automatically refunded.
- Q: May an administrator change the customer-selected weekdays? → A: No. The administrator may change only the individual resource-less booking; the recurring weekday pattern remains unchanged.
- Q: What happens after an administrator edits an individual resource-less booking? → A: The edit applies immediately, notifies the customer where the schedule changed, and marks that booking as an override so the workflow no longer changes it.
- Q: Should the system continue automatic resource repair for a resource-less booking that has not been edited? → A: Yes. It retries assignment on the original selected date until fulfilled, expired, or overridden by an administrator.
- Q: What happens when an individual resource-less booking cannot be fulfilled? → A: The administrator cancels and refunds that individual booking only; the remaining subscription schedule stays active.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Configure Weekly Day-Selection Rules (Priority: P1)

An authorized administrator can configure a weekly product price in Skedular Host or Skedular Spaces to require customers to choose one exact number of recurring weekdays, independently of the price's available days.

**Why this priority**: The customer flow cannot enforce a meaningful selection until administrators can define the rule accurately.

**Independent Test**: Configure weekly prices with no rule, an exact-two-day rule, and a separate exact-three-day rule; review each saved price and verify that its rule and available days remain independent.

**Acceptance Scenarios**:

1. **Given** an administrator is editing a weekly price with Monday through Friday available, **When** they require two selected days and save, **Then** the price displays an exact two-days-per-week selection rule.
2. **Given** a weekly price requires two selected days, **When** an administrator reviews it, **Then** it clearly states that customers must select exactly two days per week.
3. **Given** an administrator creates or edits a non-weekly price, **When** they view price settings, **Then** weekly day-selection rules are not offered.
4. **Given** a weekly price has no day-selection rule, **When** it is saved, **Then** its existing purchase and booking behavior remains unchanged.
5. **Given** an administrator configures a fortnightly or monthly price, **When** they view its settings, **Then** it is not governed by or configured through the weekly required-day parameter.

---

### User Story 2 - Select Required Recurring Weekdays (Priority: P1)

A customer buying an eligible weekly price in the shared marketplace flow must select the exact required number of available weekdays before completing the purchase.

**Why this priority**: The business value of the feature is to turn a weekly price into a clear, enforceable recurring-day choice rather than an optional preference.

**Independent Test**: Attempt to buy a weekday-restricted weekly price with fewer than, exactly, and more than the required number of selected days, including an invalid direct submission.

**Acceptance Scenarios**:

1. **Given** a selected weekly price requires exactly two days and is available Monday through Friday, **When** the customer selects Tuesday and Thursday, **Then** they can continue with the purchase and their chosen days remain visible through the flow.
2. **Given** the same price, **When** the customer has selected only Tuesday, **Then** they receive clear feedback that exactly two days are required and cannot complete the purchase.
3. **Given** a price requires exactly two days, **When** the customer attempts to select a third available day, **Then** the experience prevents the additional selection and explains the exact requirement.
4. **Given** a weekly price is available only Monday through Thursday, **When** a customer attempts to submit Friday as one of the selected days, **Then** the purchase is rejected and no resulting purchase or booking schedule is created.

---

### User Story 3 - Generate the Fixed Schedule (Priority: P1)

A customer's valid weekday selection becomes the fixed recurring schedule for the resulting purchase, while all existing booking eligibility rules still apply.

**Why this priority**: A successful purchase must create the schedule the customer selected; otherwise the selection has no operational value.

**Independent Test**: Purchase a weekly price with a Tuesday-and-Thursday selection and verify that generated recurring booking instances are considered only on those weekdays for the covered period.

**Acceptance Scenarios**:

1. **Given** a customer buys an eligible weekly price and selects Tuesday and Thursday, **When** recurring bookings are generated for the covered period, **Then** only Tuesdays and Thursdays are considered for that purchase's recurring schedule.
2. **Given** a selected recurring weekday has no compatible resource or violates existing availability, opening-hours, product, or conflict rules, **When** generation runs, **Then** it creates a booking shell on that selected date without a resource and does not substitute a different weekday.
3. **Given** an active purchase was made with a fixed weekday selection, **When** future instances are generated within its current period, **Then** they retain the selection made at purchase rather than later price edits.
4. **Given** a price permits five weekdays, requires exactly two, and a customer selects Tuesday and Wednesday, **When** compatible resources are available only on unselected weekdays, **Then** the system creates resource-less booking shells on Tuesday and Wednesday as needed, does not allocate a resource on any unselected weekday, and both the customer and the space administrator can see the affected bookings.
5. **Given** an auto-renewable purchase has a Tuesday-and-Wednesday fixed pattern, **When** it renews, **Then** the renewed period validates and generates only against Tuesday and Wednesday; an unresourced selected-day booking remains a shell for automatic repair rather than substituting other weekdays.
6. **Given** an affected booking shell exists after payment, **When** the administrator has not yet changed it, **Then** payment is retained and the customer can see that resource assignment is still pending rather than being automatically refunded.
7. **Given** an administrator edits an individual resource-less booking, **When** the edit is saved, **Then** the edit applies to that booking only, the recurring weekday pattern remains unchanged, and future workflow repair does not alter the edited booking.
8. **Given** an untouched resource-less booking shell, **When** a later reconciliation finds a compatible resource on its original date, **Then** the system attaches that resource to the existing booking shell.
9. **Given** an administrator decides an individual resource-less booking cannot be fulfilled, **When** the administrator cancels it, **Then** only that booking is canceled, the established refund process begins for that booking, and the customer is notified.

---

### User Story 4 - Understand the Rule (Priority: P2)

Administrators, customers, and public-web visitors can understand when a weekly price requires recurring-day selection, what number of days is required, and that the initial experience creates a fixed weekly pattern.

**Why this priority**: Accurate wording prevents customers from expecting flexible week-by-week date selection and helps administrators configure prices correctly.

**Independent Test**: Review an eligible price in administration, the customer purchase flow, and public-web documentation; each communicates the exact required count, available days, and fixed-pattern behavior consistently.

**Acceptance Scenarios**:

1. **Given** a weekly price requires exactly three days, **When** it is shown to a customer, **Then** it clearly says "Choose exactly 3 days per week" and identifies the eligible weekdays.
2. **Given** a weekly price requires exactly two days, **When** it is shown to a customer, **Then** it clearly says "Choose exactly 2 days per week."
3. **Given** a visitor reads the public Skedular website documentation, **When** weekly price day selection is described, **Then** the documentation explains configuration, required selection, available-day restrictions, fixed recurring schedules, and the fact that normal booking availability still applies.

### Edge Cases

- A day-selection rule is enabled only when one exact required-day value is set; it remains optional for existing weekly behavior.
- The required-day value must be greater than zero and no greater than seven.
- When available days are specified, the required-day value may not exceed their count; when none are specified, all seven days are available.
- The customer's selection must contain unique weekdays and be a subset of the price's available days.
- Existing unrestricted weekly prices, non-weekly prices, active subscriptions, historical bookings, and recurring schedules remain unchanged.
- A fortnightly, monthly, or other non-weekly price must not inherit, interpret, or be validated against the weekly parameters.
- A direct or stale customer submission that bypasses interface limits must be rejected by the authoritative purchase validation.
- Resource availability on an unselected day must never compensate for unavailable resources on a customer-selected day.
- An auto-renewal must retain the customer's fixed weekday pattern; it must not silently choose a new pattern or allocate bookings on other available weekdays.
- A selected-day booking that cannot initially be fulfilled creates a visible resource-less booking shell; it must not fail silently after purchase or payment.
- Payment for a resource-less booking shell is retained while automatic repair remains eligible and is not automatically refunded.
- An administrator edit applies only to the individual booking and prevents recurring workflow repair from changing that booking again.
- An untouched booking shell may receive a resource later only on its original selected date.
- When an administrator decides an individual booking shell cannot be fulfilled, only that booking is canceled and follows the established refund process.
- Calendar-day matching uses UTC because the current product does not store or capture location time zones.
- Weekday determination and recurring generation use the UTC calendar. Location-local and daylight-saving conversion are out of scope because the product does not store or capture booking time zones.
- Editing a price after purchase must not silently change the selected weekdays for an active purchase or current subscription period.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Skedular Host and Skedular Spaces MUST let authorized administrators configure one exact required number of selected days per week on an eligible weekly product price.
- **FR-002**: The system MUST treat the exact required-day value as an optional rule; it is not required for existing weekly behavior when absent.
- **FR-003**: The system MUST accept only a required-day value greater than zero, no greater than seven, and no greater than the count of configured available days when that count is nonzero.
- **FR-004**: The system MUST define the configured value explicitly as an exact required number of selected days **per week** for weekly prices, rather than as a generic count applicable to all pricing periods.
- **FR-005**: The system MUST make the weekly parameters unavailable for non-weekly pricing cadences in this iteration; fortnightly, monthly, and other cadences MUST NOT inherit, interpret, or reuse them.
- **FR-006**: The system MUST keep the weekly day-selection rule, the price's available days, and a customer's selected weekdays as separate business information.
- **FR-007**: When a weekly price has no available days configured, the system MUST treat all seven weekdays as available for validating the customer's selection.
- **FR-008**: Any future fortnightly, monthly, or other cadence-specific day-selection feature MUST be specified and implemented with its own cadence-specific parameters and validation; it is outside the scope of this feature.
- **FR-009**: When an eligible weekly price has the rule enabled, the shared marketplace customer purchase flow MUST require the customer to explicitly select a unique set of available weekdays before completion.
- **FR-010**: The customer purchase flow MUST show only eligible weekdays, communicate the exact required count, prevent selection beyond that count, preserve valid selections while the customer progresses, and provide clear feedback for invalid or incomplete selections.
- **FR-011**: The system MUST reject any attempted purchase whose weekday selection is absent, contains an unavailable or duplicate day, or does not match the configured exact count; it MUST create no resulting purchase, subscription, or recurring schedule for that attempt.
- **FR-012**: For a valid purchase, the system MUST store the customer's selected weekdays with that resulting purchase or recurring configuration and MUST NOT modify the product price definition.
- **FR-013**: The resulting recurring schedule MUST use the customer's selected weekdays as a fixed weekly pattern throughout the applicable purchase or subscription period.
- **FR-014**: The system MUST continue to apply all existing location opening-hours, resource availability, product availability, conflict-detection, subscription-duration, and recurring-booking rules when generating bookings from the fixed pattern.
- **FR-015**: The authoritative booking and resource-matching process MUST evaluate only the customer's selected weekdays when validating and attempting to create the fixed recurring schedule; availability on an unselected weekday MUST NOT be used as a replacement.
- **FR-016**: If the required resources cannot be booked on a customer-selected weekday under existing rules, the system MUST create or retain an affected booking shell on that selected date without a resource, MUST NOT allocate a resource on a different weekday, and MUST NOT silently treat the attempt as successful.
- **FR-016a**: The shared marketplace customer flow MUST show the customer when an affected booking shell is awaiting resource assignment, with clear status and next-step guidance.
- **FR-016b**: Skedular Host MUST show the space administrator affected resource-less bookings and identify the selected weekday or weekdays that could not be fulfilled.
- **FR-016c**: When an affected booking shell has payment, the system MUST retain that payment while automatic repair remains eligible and MUST NOT automatically cancel or refund it solely because initial selected-day resource matching failed.
- **FR-016d**: Skedular Host MUST allow the space administrator to edit an affected individual booking; that edit MUST leave the subscription’s recurring weekday pattern unchanged and MUST prevent future recurring workflow repair from altering the edited booking.
- **FR-016e**: When an untouched booking shell later receives a resource or an administrator changes its schedule, the shared marketplace customer flow MUST notify the customer of the resulting booking update.
- **FR-016f**: Skedular Host MUST allow the space administrator to cancel an individual affected booking shell when it cannot be fulfilled; the system MUST start the established refund process for that booking without canceling the remaining subscription schedule.
- **FR-016g**: When an affected individual booking is canceled as unable to fulfill, the shared marketplace customer flow MUST notify the customer of the cancellation and refund outcome or next step.
- **FR-017**: For an auto-renewable purchase, the renewal process MUST retain the customer's fixed selected weekdays and apply the same selected-day-only validation and resource-matching rules to the renewed period; an unresourced selected-day booking MUST remain eligible for automatic repair rather than substituting a different weekday.
- **FR-018**: The implementation planning work MUST map the current price, purchase, subscription, booking-group, recurring-generation, resource-matching, and renewal flows and confirm the ownership point for the customer selection before implementation begins.
- **FR-019**: The system MUST preserve existing behavior for prices without the rule, including all existing weekly, non-weekly, one-time, recurring, subscription, and renewal behavior.
- **FR-020**: The system MUST preserve existing active subscriptions, historical purchases, bookings, and schedules without requiring data correction or administrator action.
- **FR-021**: The public Skedular website documentation MUST be updated to explain weekly price day-selection rules, their relationship to available days, the customer purchase requirement, the fixed recurring-pattern result, selected-day-only resource matching, and the scope exclusion for flexible week-by-week selection.
- **FR-022**: The feature MUST include automated coverage for configuration validation, no-rule backward compatibility, exact selections, unavailable-day submissions, direct invalid submissions, selected-day-only resource matching, resource-less booking shells when only unselected days have resources, automatic repair, individual booking overrides, fixed-pattern generation, auto-renewal, individual cancellation/refund, UTC calendar boundaries, and unaffected non-weekly pricing.
- **FR-023**: The feature MUST provide price configuration in both Skedular Host and Skedular Spaces. The shared marketplace customer flow owns weekday selection and purchase; Host and Spaces expose their respective authorized booking and status interactions without duplicating checkout.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs for the start and completion of recurring generation using a customer-selected weekly pattern.
- **LOG-002**: The feature MUST emit structured logs when purchase validation rejects a required weekday selection, resource matching creates a resource-less booking shell, or automatic repair attaches a resource, with relevant request or workflow correlation context.
- **LOG-003**: The feature MUST emit actionable warning or error logs when selected-weekday validation, booking-shell creation, or automatic repair cannot complete, including booking and workflow correlation context.
- **LOG-003a**: The feature MUST emit structured logs when an administrator overrides or cancels a resource-less booking, including the resulting resolution/refund path and relevant correlation context.
- **LOG-004**: Feature logs MUST include correlation context and MUST avoid sensitive customer data.

### Key Entities _(include if feature involves data)_

- **Weekly Price Day-Selection Rule**: An optional, weekly-specific exact count constraint owned by an eligible weekly product price; it is not a shared rule for other pricing cadences.
- **Available-Day Rule**: The existing price-level list of weekdays on which the price may be used; when absent, every weekday is eligible for selection.
- **Customer Weekday Selection**: The unique available weekdays chosen for one resulting purchase or recurring configuration; it establishes that customer's fixed weekly pattern.
- **Recurring Purchase or Subscription Period**: The customer-owned period that retains the selected weekly pattern and determines where recurring instances are considered.
- **Booking Instance**: A reservation on a customer-selected weekday; it may initially be a resource-less booking shell and later receive a compatible resource only on that same date.
- **Recurring Instance Override**: The existing individual-booking override state applied after an administrator edits a resource-less shell, preventing future recurring workflow repair from altering that booking.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In acceptance testing, administrators can configure every valid weekly rule in Skedular Host and Skedular Spaces, and 100% of matching weekly prices in the shared marketplace flow require a customer weekday selection before purchase completion, while 100% of equivalent prices without the rule retain their prior flow.
- **SC-002**: In acceptance testing, 100% of attempted selections with a count other than the exact required count, duplicated, or outside available days are rejected without creating a purchase, subscription, or recurring schedule.
- **SC-003**: In acceptance testing of an exact Tuesday-and-Thursday selection across a six-month covered period, 100% of considered recurring booking days follow that fixed pattern; no other weekdays are introduced by this feature.
- **SC-004**: In acceptance testing where a customer selects Tuesday and Wednesday but compatible resources exist only on other available weekdays, 100% of affected dates create visible resource-less booking shells without a resource allocation or booking on an unselected weekday.
- **SC-005**: In acceptance testing of auto-renewal, 100% of renewed periods retain the original selected weekdays; an unresourced selected-day booking remains eligible for automatic repair and never substitutes a different weekday.
- **SC-005a**: In acceptance testing of an affected paid booking shell, 100% retain payment while automatic repair remains eligible and none are automatically refunded solely because initial selected-day resource matching failed.
- **SC-005b**: In acceptance testing of an untouched booking shell, a compatible resource found later is attached to that original booking on its original selected date.
- **SC-005c**: In acceptance testing of an administrator edit, 100% of edited resource-less bookings become recurring-instance overrides, remain individual changes, and are not subsequently changed by the workflow.
- **SC-005d**: In acceptance testing where an administrator cancels an impossible resource-less booking, 100% cancel only that booking, enter the established refund process, and notify the customer.
- **SC-006**: Existing regression coverage for non-weekly pricing and weekly prices without a day-selection rule continues to pass with no customer-visible behavior change.
- **SC-007**: In usability review, administrators and customers can correctly state the required number of days and complete an eligible purchase without assistance in at least 9 of 10 representative attempts.
- **SC-008**: A reviewer can use the public-web documentation to distinguish available days, required selected-day counts, fixed recurring patterns, selected-day-only resource matching, and excluded flexible week-by-week selection without additional product guidance.

## Assumptions

- The existing available-days capability is the source of truth for which weekdays a price permits; an empty available-days setting permits all seven days.
- Existing product-price administration permissions govern who can configure the new weekly rule.
- "Weekday" includes every calendar day from Sunday through Saturday, not only workweek days.
- The first iteration stores a fixed weekly pattern chosen at purchase; customers cannot vary days from week to week after purchase through this feature.
- A price's available days identify the permitted candidate pool, but a customer's selected days are the exclusive schedule for their purchase; capacity on a different permitted day cannot fulfill that customer's selection.
- Auto-renewal retains the selected fixed pattern for the renewed period and creates/repairs resource-less bookings only on that pattern; it does not re-select weekdays for the customer.
- Current subscription, recurring-booking, availability, opening-hours, resource, conflict, and renewal behavior remains authoritative except for the added selection, resource-less booking shell, automatic repair, and individual override constraints.
- Skedular Host owns the administrative configuration experience and Skedular Spaces owns the customer purchase and selection experience; public-web documentation complements but does not replace either in-product experience.
- Fortnightly, monthly, and other cadences are intentionally out of scope. Each will need separately named cadence-specific parameters, behavior, and validation rather than reusing the weekly values.
