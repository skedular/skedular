# Contract: ProductPricing Available Days

## Public Price Shape

`ProductPricing` gains an additive `availableDays` collection in both query output and product create/update inputs.

| Field | Values | Compatibility |
| --- | --- | --- |
| `availableDays` | Existing calendar codes: `MON`, `TUE`, `WED`, `THU`, `FRI`, `SAT`, `SUN` | Omitted or empty is interpreted as every day. |

The values use the existing shared day mapping and are calendar days, not workweek-only weekdays.

## Mutation Semantics

- Product creation and whole-pricing-options updates accept `availableDays` per price.
- A submitted list must contain only supported codes and no duplicates.
- An empty list is valid and must be preserved as the unrestricted representation.
- Query responses return the saved collection so Host, Spaces, and customer views can show the rule.

## Event and Projection Semantics

- Marketplace product-version events include the new field for every pricing option.
- Booking and Location projections retain it in their existing replicated ProductVersion data.
- Existing events or saved product versions without the field deserialize as an empty, unrestricted collection.

## Booking Error Semantics

When a nonempty rule excludes the booking's local start day, booking and subscription requests fail before resource allocation with a customer-safe “not available on this day” outcome. The response must not claim that the resource itself is unavailable.
