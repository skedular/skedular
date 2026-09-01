# Research: Marketplace Pricing Cadence Simplification

## Decision: Preserve `PurchaseCadence` as the sole offer-term field

**Rationale**: The existing shared `ProductPricing` record contains both `PurchaseCadence` and `BookingCadence`, while subscription and billing code already uses `PurchaseCadence` for term-level behavior. Removing the duplicate field makes one value authoritative without introducing the later `PurchaseTerm` rename.

**Alternatives considered**: Retain both fields with validation; rejected because the feature explicitly removes `BookingCadence`. Rename to `PurchaseTerm`; deferred to a later migration as required by the specification.

## Decision: Retain only day-or-longer cadence values

**Rationale**: The supported set is exactly Daily, Weekly, Fortnightly, Monthly, TwoMonths, Quarterly, FourMonths, FiveMonths, SixMonths, and Yearly. `NotSet` remains available for cadence-free entitlements; OneTime and all sub-day values are removed from supported product-pricing choices and mappings.

**Alternatives considered**: Keep `OneTime` for non-renewing offers; rejected because auto-renewal already distinguishes one term from repeating terms. Keep sub-day values for booking duration; rejected because date-time interval plus min/max duration is now authoritative.

## Decision: Use date-time interval and min/max bounds for individual bookings

**Rationale**: This preserves existing `MinDurationMinutes` and `MaxDurationMinutes` while removing cadence-derived duration steps. Opening hours, resource availability, and conflict checks remain independent validation stages.

**Alternatives considered**: Infer duration from purchase cadence or retain a duration step; rejected because purchase terms and individual booking duration represent different business concepts.

## Decision: Remove obsolete persisted/contract representations directly

**Rationale**: The product owner confirmed there are no production pricing records using terms shorter than one day. The implementation can remove obsolete fields and enum values without conversion or backfill, while unexpected values in non-production/imported data must fail explicitly.

**Alternatives considered**: Automatic nearest-term conversion; rejected because it could change commercial meaning. Disable-and-correct or historical-only compatibility; unnecessary for the confirmed production dataset.

## Decision: Keep entitlements out of cadence and renewal processing

**Rationale**: Entitlements are governed by credit quantity, validity, available days, and duration limits. They use `NotSet`/null cadence and must not enter subscription auto-renewal or recurring purchase-cadence logic.

**Alternatives considered**: Continue using a hardcoded one-time cadence; rejected because it falsely models entitlements as cadence-based offers.

## Decision: Regenerate all derived surfaces from source definitions

**Rationale**: Repository instructions require source-first changes for protobuf, GraphQL, OpenAPI, and Relay surfaces. Event protobufs, GraphQL schemas, generated clients, and Relay artifacts must be regenerated after source changes.

**Alternatives considered**: Hand-edit generated schema/client files; rejected because it creates drift and will be overwritten.
