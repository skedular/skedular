# Research: Weekly Price Day Selection

## Decision: Extend `ProductPricing` with a weekly-specific nullable exact count

Add `requiredDaysPerWeek` as an optional exact selected-day count on the existing shared `ProductPricing` model and product-version event shape. It is valid only when the price purchase cadence is weekly; no generic period-count field is introduced.

**Rationale**: `ProductPricing` already owns `availableDays`, cadence, resource count, and recurring commerce settings. Explicit weekly names protect future fortnightly and monthly work from accidental reuse.

**Alternatives considered**:

- A generic days-per-period field — rejected because its period semantics and validation would differ by cadence.
- A separate weekly-pricing entity — rejected because it would duplicate price identity, projection, and editor flows.

## Implementation inventory

- Marketplace validates `ProductPricing` in `Marketplace.Api/Services/ProductService.cs` and publishes it from `Marketplace.Shared/Mappers/EventMapper.cs` through `marketplace_v1_value.proto`.
- Booking consumes the event in `Booking.Processors/Mappers/EventMapper.cs`, validates subscription purchase input in `MarketplaceBookingSubscriptionService`, and stores customer selected days on `MarketplaceBookingSubscription`.
- `MarketplaceBookingSubscriptionIntegrations` runs the daily Temporal reconciliation. `RecurringBookingScheduleService` uses UTC calendar dates and `HasRecurringInstanceOverrides` already excludes administrator-edited individual bookings from automatic repair.
- When a missing selected date cannot resolve an opening-hours/resource plan, reconciliation creates the normal Booking without resources. Existing unoverridden bookings continue through `MarketplaceBookingService.AdjustRequiredResourcesAsync` on later daily runs.

## Decision: Persist the purchased selection in Booking and materialize it through `ByWeekDays`

Add a Booking-owned selected-day snapshot to the marketplace subscription/current recurring configuration. Validate the customer’s selection at subscription creation, then write the validated days into the existing recurring booking `ByWeekDays` schedule used by reconciliation. The snapshot is authoritative for the current period; later price edits do not rewrite it.

**Rationale**: `RecurringBooking.ByWeekDays` is the existing execution schedule. A subscription-level snapshot preserves what was purchased and gives auto-renewal a stable value to validate without mutating the price.

**Alternatives considered**:

- Store selected days only on `ProductPricing` — rejected because that changes the product for every customer.
- Store selected days only on generated bookings — rejected because there is no durable source for future generation or renewal.

## Decision: Match resources only on selected calendar days

In `MarketplaceBookingSubscriptionService`, validate the selected-day count and membership before checkout. In `MarketplaceBookingSubscriptionIntegrations`, filter recurring candidates by the purchased selected days before opening-hours and resource matching. A resource available only on another price-available day must never replace an unfulfillable selected day.

**Rationale**: Existing available-days logic already acts as an early candidate filter, while `TryResolveDailyPlanAsync` performs resource allocation. Applying the purchased schedule first keeps availability and entitlement distinct.

**Alternatives considered**:

- Treat any `AvailableDays` resource as interchangeable — rejected because it breaks the customer-selected weekly pattern.
- Fail only in client UI — rejected because direct GraphQL calls and workflow generation remain authoritative server paths.

## Decision: Create resource-less booking shells and reuse recurring-instance overrides

When selected-day resource matching fails, create the ordinary Booking for the original selected date without attached resources. Daily reconciliation continues to attempt resource assignment on that same date for a shell whose `HasRecurringInstanceOverrides` is false. An administrator edits only that individual Booking; the existing override prevents future automatic repair from changing it. If the shell is impossible to fulfill, the administrator cancels that individual booking and invokes the existing refund process while the remaining subscription schedule continues.

**Rationale**: A Booking already provides the durable customer/operator-visible record for the date, payment relationship, audit trail, and cancellation path. Existing reconciliation already excludes overridden recurring instances, so it supplies the needed automation boundary without a second aggregate.

**Alternatives considered**:

- A separate intervention aggregate — rejected because it duplicates the booking, visibility, and cancellation/refund lifecycle.
- Automatically refund every failure — rejected by clarification; payment stays retained while automatic repair remains eligible.
- Substitute another available weekday — rejected because it breaks the purchased fixed pattern.

## Decision: Keep auto-renewal on the same selected pattern

At renewal, retain the persisted selected days for the renewed period, revalidate them against the renewed weekly price’s available days and exact required count, and run the same selected-day-only allocation. If allocation cannot succeed, create resource-less shells on the selected dates and continue automatic repair instead of silently selecting other days.

**Rationale**: The existing workflow already reloads the current price and moves to renewal failure if no compatible price exists. The selected pattern is a purchased schedule, not a new price-matching key.

**Alternatives considered**:

- Re-select days automatically on renewal — rejected because it changes the customer’s purchased schedule.
- Treat a failed allocation as a normal resource skip — rejected because the customer and operator need a durable visible booking shell.

## Decision: Deliver additive GraphQL and synchronized web/documentation surfaces

Expose the weekly exact count on price administration/query shapes, selected-day input and subscription/booking status details on purchase/query shapes, and reuse individual Booking update/cancel operations for Host actions. Keep the customer selection/status experience in the shared marketplace flow, configure prices in Host and Spaces, and update public subscriptions and products/pricing documentation. Regenerate GraphQL schemas and Relay artifacts from their sources.

**Rationale**: Host and Spaces cannot rely on one another’s UI, and existing GraphQL topics/subscriptions can refresh state after workflow transitions.

**Alternatives considered**:

- Hide an unresourced booking behind server logs — rejected because both actors require actionable status.
- Hand-edit Relay or schema output — rejected by the repository’s generated-code rules.
