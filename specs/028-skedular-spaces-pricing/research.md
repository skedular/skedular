# Research: Skedular Spaces Pricing Implementation

## Decision: Use Product-Specific Catalog Versions on the Existing Pricing Catalog

**Rationale**: Spaces should use the same Organization-owned pricing catalog and offering pattern as Skedular Teams, but Teams and Spaces can evolve independently. Teams offerings use `TEAMS_V1`; Spaces/co-working marketplace offerings use `SPACES_V1`. Product filtering remains consistent while catalog-version state correctly reflects the product offering assigned to the organization.

**Alternatives considered**:

- Reuse `TEAMS_V1` for Spaces. Rejected because it mislabels Spaces/co-working marketplace offerings and prevents independent Spaces catalog evolution.
- Create a separate Spaces pricing domain. Rejected because existing Organization catalog ownership and product filtering are sufficient.
- Hardcode Spaces pricing in `webapp-spaces`. Rejected because the spec requires backend-driven pricing and the Teams feature already moved toward server-driven catalog data.

## Decision: Organization Owns Spaces Subscription and Catalog Assignment

**Rationale**: Organization already owns organization offering/subscription state and the product-aware pricing catalog. Spaces plan assignment, Enterprise/admin overrides, default Free migration, and catalog versioning are organization-level business state, not booking-instance data.

**Alternatives considered**:

- Put subscription state in Booking. Rejected because Booking owns booking workflows, not organization commercial subscription assignment.
- Add a new pricing domain. Deferred because existing Organization ownership and Teams catalog implementation are sufficient for this feature.

## Decision: Booking Owns Booking-Instance Usage and Quota Enforcement

**Rationale**: The usage metric is booking-instance creation. Booking owns one-off booking creation, multi-slot expansion, recurring private booking generation, subscription-generated bookings, cancellation semantics, and rollback when instance creation fails. Enforcing quotas at booking creation boundaries prevents drift and avoids leaking Booking persistence details into Organization.

**Alternatives considered**:

- Count booking usage in Organization. Rejected because Organization does not own booking instance creation or failure rollback.
- Enforce only at frontend/API gateway. Rejected because recurring and background generation paths must be enforced consistently.

## Decision: Current Usage Is Counted From Booking Rows

**Rationale**: The user clarified that Booking should not maintain a booking-count counter inside the replicated `Organization.Offering` JSONB object and should not use raw SQL/jsonb updates for quota usage. The Booking domain already owns the booking rows, so current-period usage is derived by counting persisted bookings whose scheduled start falls inside the billing period. The replicated offering keeps only plan code, quota limit, custom capacity, and period boundaries.

**Alternatives considered**:

- Add a separate Booking usage-period table. Rejected because the feature does not need a second usage persistence model when booking rows can be counted directly.
- Store and increment a current usage counter in `Organization.Offering` JSONB with raw SQL. Rejected because it is more complex than needed and couples booking usage mutation to a replicated organization JSON object.
- Calculate usage only from cached/frontend state. Rejected because quota checks must remain server-authoritative and close to realtime.
- Use local time zones. Rejected because the spec defines UTC day boundaries.

## Decision: Quota Counts Created Instances Scheduled Inside the Billing Period

**Rationale**: The spec defines a booking instance as each individual stored booking record created by a request, and canceled bookings remain counted for the monthly period. The current-period quota counts only booking instances whose scheduled start falls inside the current UTC billing period. Failed creation is naturally excluded because there is no booking row to count.

**Alternatives considered**:

- Count only non-canceled bookings. Rejected by the spec's canceled-booking edge case.
- Count attempted bookings before persistence. Rejected because failed creation must not consume quota.
- Count bookings scheduled outside the current billing period. Rejected because future/out-of-period instances must not consume the current period's quota.

## Decision: Quota Enforcement Uses Close-to-Real-Time Booking Counts

**Rationale**: Booking creation should be blocked when the organization is at quota, but the user explicitly accepted small overages under concurrency in exchange for simpler code. The quota service therefore performs an async database count over persisted Booking rows for the current period and compares that count plus the attempted in-period instances to the effective limit. This avoids raw SQL/jsonb counter updates and keeps booking usage in the Booking table.

**Alternatives considered**:

- Atomic current-period usage counters. Rejected as too complex for the business value of this quota.
- Frontend-only quota checks. Rejected because background and concurrent server-side creation paths must enforce the same limit.
- Distributed locks. Rejected because occasional concurrent overage is acceptable and locks would add operational complexity.

## Decision: Spaces Frontend Shows Server-Driven Pricing, Quota Status, and Upgrade/Contact Prompts

**Rationale**: The frontend must display backend catalog-driven plan data and upgrade options on quota errors. Full new checkout/subscription mutation UI is out of scope unless existing flows already support it.

**Alternatives considered**:

- Backend-only enforcement. Rejected because users need clear upgrade guidance and the spec requires frontend pricing data from the backend.
- Build complete checkout flows now. Rejected by clarification; this feature should reuse existing subscription flows only.

## Decision: Contract Changes Require Regeneration

**Rationale**: The primary client-facing surface is GraphQL/Fusion. New pricing, quota status, or mutation fields must update GraphQL schema outputs through `scripts/generate-graphql.sh`. If OpenAPI organization or booking endpoints change, update the YAML source and run the OpenAPI generator and consumed web client generation.

**Alternatives considered**:

- Hand-edit generated schemas or Relay/OpenAPI files. Rejected by repository constitution and AGENTS rules.
