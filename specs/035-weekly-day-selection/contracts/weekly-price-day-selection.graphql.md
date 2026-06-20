# Contract: Weekly Price Day Selection

This is a source-level GraphQL contract. Final schema and Relay outputs are generated; they must not be edited directly.

## Price Configuration

`ProductPricing` and the corresponding product create/update price input gain additive nullable fields:

| Field | Type | Semantics |
| --- | --- | --- |
| `requiredDaysPerWeek` | `Int` | Optional exact selected-day count; valid only for weekly purchase pricing. |

Both values are omitted or null for all non-weekly prices and for weekly prices that preserve existing behavior.

## Customer Subscription Purchase

`AddMarketplaceBookingSubscriptionInput` gains an additive `weeklySelectedDays` collection using the existing calendar-day enum.

| Input state | Server result |
| --- | --- |
| Weekly price has no exact required count | `weeklySelectedDays` is optional and no weekly-selection behavior is introduced. |
| Weekly price has an exact required count | A unique selection with that exact count is required. |
| Non-weekly price | The weekly exact count and weekly selection are rejected/not applicable. |
| Invalid count, duplicate, or unavailable day | Customer-safe validation error; no subscription or schedule is created. |

## Subscription and Booking Details

`MarketplaceBookingSubscriptionDetails` exposes the current `weeklySelectedDays`. Existing Booking detail/query shapes expose an affected Booking with no resources, its scheduled selected date, payment status, and its individual recurring-instance override state where appropriate. Payment and refund information continue to use their existing contract surfaces.

## Host Individual-Booking Operations

Reuse the existing operator-authorized individual Booking update and cancellation operations:

| Action | Input | Outcome |
| --- | --- | --- |
| Edit an individual shell | Existing booking update input | Updates only that Booking, marks its recurring instance as overridden, leaves `weeklySelectedDays` and `ByWeekDays` unchanged, and notifies the customer if its schedule changes. |
| Cancel an impossible individual shell | Existing booking cancellation input | Cancels only that Booking and starts the existing refund flow; the subscription and remaining recurring schedule continue. |

Exact type and mutation names follow current Booking GraphQL naming conventions during implementation. Operations must remain idempotent and preserve the existing authorization and refund boundaries.

## Error and Update Semantics

- A resource available on another price-available day is not a valid substitute for a selected day.
- Selected-day allocation failure creates or retains a resource-less Booking shell rather than silently succeeding.
- Shell creation, repair, individual update, and cancellation publish existing Booking/subscription GraphQL change topics so Spaces and Host refresh their views.
- Pricing and GraphQL source changes require event, GraphQL, and web-client generation.
