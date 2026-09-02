# Pricing Contracts: Marketplace Pricing Cadence Simplification

## Shared and event contract

The marketplace pricing value contains `purchaseCadence` and does not contain `bookingCadence`. The event enum contains `NOT_SET` plus the ten supported offer terms. Removed enum values are absent from the source contract and regenerated outputs.

For an entitlement, `fulfillmentType` is `ENTITLEMENT`, cadence is `NOT_SET`/null according to the owning representation, and credit quantity, validity, available days, and min/max duration remain available.

## GraphQL contract

- `ProductPricing` exposes `purchaseCadence` only; `bookingCadence` is removed.
- `ProductPricingCadence` choice/detail queries expose only the supported values, with `NOT_SET` available where the existing choice contract permits cadence-free entitlements.
- Product create/update inputs remove `bookingCadence` and reject removed enum values through regenerated schema validation.
- Booking mutations accept `from` and `until`; duration validation is based on their difference and pricing min/max bounds. No cadence-step input is exposed.

## Projection and service contract

- Marketplace event mapping maps only purchase cadence.
- Booking projection and shared services consume purchase cadence for term/renewal behavior and organization billing cycle for internal slices.
- Entitlement services bypass subscription renewal and recurring purchase-cadence paths.
- Transport layers continue calling services and mapping shared models; persistence remains behind repositories/services.

## Frontend contract

Pricing editors render one cadence choice field. Customer booking forms render start/end date-time selection and validation feedback for min/max duration. Generated Relay types and operations are regenerated from the updated schema; successful mutations return rendered fields and stable IDs or use targeted refetch/connection updates.
