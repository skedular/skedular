# Data Model: Marketplace Pricing Cadence Simplification

## ProductPricing

The shared marketplace pricing value remains the canonical offer model.

| Field | Rule |
|---|---|
| `PurchaseCadence` | Required for reservation offers; one supported term of one day or longer. Remains the existing public name. |
| `BookingCadence` | Removed entirely from the model, persistence, events, serializers, APIs, projections, workflows, frontend models, and tests. |
| `MinDurationMinutes` | Optional lower bound for an individual booking interval. |
| `MaxDurationMinutes` | Optional upper bound for an individual booking interval. |
| `SupportsSubscriptionAutoRenewal` | Determines whether the selected purchase term repeats. |
| `FulfillmentType` | Distinguishes reservation offers from credit entitlements. |
| `EntitlementCreditQuantity` | Credit quantity for entitlement offers. |
| `EntitlementValidityDays` | Validity period for entitlement offers. |
| `AvailableDays` | Allowed entitlement/booking days where applicable. |

## ProductPricingCadence

Supported values:

`NotSet`, `Daily`, `Weekly`, `Fortnightly`, `Monthly`, `TwoMonths`, `Quarterly`, `FourMonths`, `FiveMonths`, `SixMonths`, `Yearly`.

`NotSet` is the cadence-free representation for credit entitlements. `OneTime`, `PerMinute`, `Per15Minutes`, `Per30Minutes`, `PerHour`, and `HalfDay` are removed.

## Purchase and renewal lifecycle

- A non-renewing offer creates one purchase term, including a Daily one-day term.
- A renewing offer creates subsequent terms using `PurchaseCadence`.
- Organization billing cycle may slice invoices and resource bookings inside a longer purchase term but does not alter the purchase term.
- Credit entitlements have no cadence lifecycle and are excluded from subscription renewal and recurring purchase processing.

## Booking duration validation

The customer supplies `From` and `Until`. The system computes `Until - From` and validates it against `MinDurationMinutes` and `MaxDurationMinutes`. Any interval within the configured inclusive bounds passes duration validation; opening hours, resource availability, and conflicts remain subsequent independent checks.

## Persistence and migration

Remove the persisted `BookingCadence` representation wherever ProductPricing/ProductVersion data stores it. Remove obsolete cadence constants and mappings. No production data conversion/backfill is required based on the confirmed absence of removed terms in production; unexpected legacy values must fail explicitly.
