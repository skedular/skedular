# Research: Product Price Available Days

## Decisions

### Reuse the shared ProductPricing contract and existing calendar constants

**Decision**: Add an `availableDays` collection to `ProductPricing`, using the existing `MON` through `SUN` day constants. The collection is optional; an empty collection means every calendar day.

**Rationale**: ProductPricing is already the contract used by Marketplace, Booking, GraphQL, and web clients. The existing constants provide all seven days and display names without inventing a workweek-only type.

**Alternatives considered**:

- Product-wide day rules — rejected because each price can have distinct booking rules.
- A new recurrence model — rejected because existing pricing cadence and subscription scheduling remain authoritative.
- A separate database table — rejected because price options are already versioned and replicated as JSON collections.

### Preserve current-period price snapshots; refresh on renewal

**Decision**: Generation within an active subscription period uses the ProductPricing copied at purchase. Renewal reloads and matches the latest ProductPricing, including `availableDays`.

**Rationale**: This matches existing price-change behavior, avoids rewriting paid entitlement, and is already compatible with the renewal path that reloads ProductVersion.

**Alternatives considered**:

- Immediately apply edits to future instances in the active period — rejected because it changes a paid period after purchase.
- Forbid price changes while subscriptions exist — rejected as unnecessarily restrictive.

### Gate before availability and filter before recurring generation

**Decision**: Validate the allowed local start date immediately after resolving the selected price for a one-time booking or subscription checkout. In the daily subscription reconciliation, skip disallowed candidate days before opening-hours, resource allocation, and booking materialization.

**Rationale**: The price rule is a necessary condition, while opening hours, tag matching, conflicts, and resource availability remain independently necessary conditions. This prevents invalid direct calls and prevents the workflow from creating or repairing prohibited dates.

**Alternatives considered**:

- Rely on disabled UI dates — rejected because server-side validation is required.
- Embed the rule into resource availability — rejected because resources remain independently available and may be used by other prices.

### Establish one local calendar-day resolver

**Decision**: Introduce or extend a shared Booking helper that resolves the booking start date in the applicable location timezone, then use it for day eligibility at checkout and recurring generation.

**Rationale**: current paths derive dates from UTC; this can select the wrong calendar day at timezone boundaries. One shared rule prevents one-time and recurring behavior from diverging.

**Alternatives considered**:

- Use the client timezone — rejected because backend location rules must be authoritative.
- Continue using UTC dates — rejected because it violates the specified location-calendar semantics.

### Update all live price editors and consumer documentation

**Decision**: Update the standard Host and Spaces product editors, the Host location-pricing editor, customer booking/subscription views, and public documentation for pricing, bookings, subscriptions, and availability.

**Rationale**: the repository contains more than one active price editing path. Missing any path would produce a price that is invisible or uneditable in a supported surface.

**Alternatives considered**:

- Update a single editor — rejected because it leaves product administration inconsistent.
