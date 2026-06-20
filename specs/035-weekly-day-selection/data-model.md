# Data Model: Weekly Price Day Selection

## ProductPricing

| Field | Shape | Rules |
| --- | --- | --- |
| `availableDays` | Existing unique collection of calendar-day codes | Empty means all seven days are eligible. It remains a price availability rule. |
| `requiredDaysPerWeek` | Nullable positive integer | Only weekly purchase pricing may set it; it requires exactly that many selected days and must not exceed the available-day count when restricted. |

This weekly-specific value is not a generic cadence field. Fortnightly, monthly, and other pricing cadences receive no new behavior from it.

## MarketplaceBookingSubscription / Current Recurring Schedule

| Field | Shape | Rules |
| --- | --- | --- |
| `weeklySelectedDays` | Unique collection of calendar-day codes | Customer-selected fixed weekly pattern. Required exactly when the matched weekly price has `requiredDaysPerWeek`; must be within `availableDays` and match that exact count. |
| `RecurringBooking.ByWeekDays` | Existing JSON collection | Execution projection of `weeklySelectedDays` for reconciliation and resource generation. It is updated only by a validated customer purchase. An administrator’s individual booking edit does not rewrite this fixed pattern. |
| Product-price snapshot | Existing subscription/marketplace booking relationship | Retains the current period’s price and weekly rule; later price edits do not rewrite the current selection. |

## Resource-less Booking Shell

| Field/state | Shape | Rules |
| --- | --- | --- |
| Original selected date and schedule | Existing Booking date/time and recurring relationship | The shell is created only for the customer-selected UTC calendar date. It never changes the subscription’s `weeklySelectedDays` or `RecurringBooking.ByWeekDays`. |
| Resources | Existing Booking resource collection | Empty when no compatible selected-date resource can be assigned. A later automatic repair may attach a compatible resource to this same Booking. |
| `HasRecurringInstanceOverrides` | Existing individual Booking state | False permits recurring reconciliation to repair the shell. An administrator edit sets it true, so the workflow must not change that booking again. |
| Payment/refund relationship | Existing Booking payment and refund state | Payment remains retained while automatic repair is eligible. An administrator can cancel only this shell and use the established Booking refund process. |

## State Transitions

```text
Valid selected schedule
  -> selected-day resource match succeeds -> Active generation
  -> selected-day resource match fails -> Resource-less booking shell

Resource-less booking shell (not overridden)
  -> later selected-date match succeeds -> attach resource to the same Booking
  -> Host edits individual booking -> recurring-instance override; no further workflow changes
  -> Host cancels individual shell -> cancellation + existing refund process

Auto-renewal
  -> retain selected schedule -> selected-day validation/allocation
  -> allocation failure -> resource-less shell and selected-date repair (not silent day substitution)
```

## Validation and Ownership

- Marketplace owns weekly price-rule configuration and emits the price projection.
- Booking owns selected schedules, resource fulfillment, individual-booking override state, cancellation, and refunds.
- Customer selected days must be unique and entirely within the weekly price’s `availableDays`; when `availableDays` is empty, all seven days are eligible.
- UTC calendar interpretation governs candidate dates because the product does not store booking time zones.
