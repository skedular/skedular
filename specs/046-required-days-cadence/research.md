# Research

## Decisions

- Reuse `RequiredDaysPerWeek`; it already exists in the shared model, GraphQL input, protobuf event, JSON serialization, Marketplace/Booking mappers, and product editors.
- Preserve exact selected-day semantics for scheduled reservations and subscriptions.
- Use an at-most limit for credit entitlements because customers may redeem fewer credits.
- Apply the rule to every existing purchase cadence longer than one week.
- Use complete UTC calendar weeks; partial boundary weeks are exempt.
- Spec 047 is authoritative: sub-day values and `BookingCadence` are removed; Daily is supported, and entitlements are cadence-free.
- For scheduled offers, count every booking occurrence, including multiple time intervals on one day.
- For entitlements, count successful redemptions; canceled, permanently failed, and refunded redemptions release capacity, and retries are idempotent.
- Keep `availableDays` as the allowed weekday set; empty means every day.

## Rationale

UTC avoids location-independent offer ambiguity and avoids adding entitlement timezone state. Durable Booking-owned redemption history is required because individual date validation cannot enforce an aggregate weekly limit.

## Sources Reviewed

`ProductPricing.cs`, Marketplace `ProductService.Validate`, Marketplace and Booking event mappers, `MarketplaceBookingWeeklyDaySelectionService`, `EntitlementBookingService`, subscription integrations, GraphQL schema, protobuf event definition, and Host/Spaces product editors/tests.
