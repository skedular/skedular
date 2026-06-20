# Data Model: Product Price Available Days

## ProductPricing

| Field | Shape | Rules |
| --- | --- | --- |
| `availableDays` | Ordered unique collection of existing calendar-day codes | Values are only `MON`, `TUE`, `WED`, `THU`, `FRI`, `SAT`, or `SUN`; no duplicates. Empty means every day. |

The field is owned by an individual price. It is persisted within the existing ProductVersion pricing-options JSON document, replicated with product-version events, and copied with the purchased price snapshot used by booking and subscriptions.

## Available-Day Eligibility

| Input | Result |
| --- | --- |
| Empty `availableDays` | Eligible on every local calendar day; normal availability checks remain required. |
| Nonempty `availableDays` containing the local booking start day | Eligible to continue to opening-hours, resource, and conflict checks. |
| Nonempty `availableDays` excluding the local booking start day | Reject direct booking/subscription checkout or skip recurring candidate generation. |

## Subscription Lifecycle

1. Purchase stores the selected ProductPricing with its available-day rule for the current period.
2. Daily reconciliation considers only the days permitted by that stored rule, then applies existing availability rules.
3. Editing a price does not rewrite current-period instances or its stored rule.
4. Renewal resolves the latest matching ProductPricing and uses its available-day rule for the next period.

## Validation and Identity

- All seven calendar days have equal meaning; “weekday” is not a business term for this feature.
- A booking uses its selected local start date; the rule does not change selected start time or duration.
- Price identity and current matching behavior remain unchanged; available days must not create a new fallback matching key.
