# Research: Customer Readiness Tracking

## Decision: Add a customer-owned public readiness topic

Add `customer_readiness` as a customer-owned public event topic with versioned protobuf key/value definitions under
`api-definitions/events/skedular`. The first version supports a generic event envelope with standard metadata, a type
discriminator, and a typed `CustomerIdentityProvisioned` payload.

**Rationale**: Readiness needs to cross domain boundaries without request-time service fan-out. The repository already
uses protobuf-defined Kafka events and checked-in metadata companions for public cross-domain collaboration.

**Alternatives considered**:

- Keep runtime fan-out to every domain: rejected because it keeps GraphQL access dependent on every participating
  domain during the hot path.
- Add a private customer-domain topic: rejected because booking, organisation, team, marketplace, and location are
  expected publishers.
- Add a dedicated `CustomerDomainIdentityProvisioned` topic: rejected by the spec and less extensible than an event-type
  discriminator.

## Decision: Domain enum is contract-owned and contains only current non-customer publishers

The readiness value contract owns a domain enum containing the current non-customer identity-provisioning domains:
booking, organisation, team, marketplace, and location. The enum must not include customer, unspecified, unknown, or
none values. If code cannot map a publisher to a known enum value, it must not publish.

**Rationale**: The enum is part of the public readiness contract and must match the required-domain set used by the
customer domain. Omitting unknown-style enum values forces unmappable publishers to fail closed.

**Alternatives considered**:

- String domain names: rejected because typed enum values are easier to validate and test across generated event code.
- Include an unknown value: rejected because it would allow ambiguous readiness reports.

## Decision: Publish after durable local customer provisioning in existing subscribers

Extend existing customer source-event subscribers in:

- `booking/processors/Booking.Processors/Subscribers/CustomerSubscriber.cs`
- `organization/processors/Organization.Processors/Subscribers/CustomerSubscriber.cs`
- `team/processors/Team.Processors/Subscribers/CustomerSubscriber.cs`
- `marketplace/processors/Marketplace.Processors/Subscribers/CustomerSubscriber.cs`
- `location/processors/Location.Processors/Subscribers/CustomerSubscriber.cs`

Publish `CustomerIdentityProvisioned` only after local upsert/merge, identity rebuild, save, and relevant cache
invalidation are complete enough that federated execution can recognise the customer in that domain.

**Rationale**: Existing subscribers are already the durable local provisioning boundary. Publishing before persistence
would let customer readiness become active while a participating domain may still fail customer recognition.

**Alternatives considered**:

- Publish at event receipt: rejected because it confirms only delivery, not provisioning.
- Publish from a separate polling job: rejected because it introduces lag and duplicate coordination without improving
  correctness over the durable subscriber boundary.

## Decision: Customer persists readiness as an aggregate with per-domain child states

Create customer-owned readiness persistence with an aggregate keyed by `customerId`, an overall status, timestamps, and
a collection/table of domain states keyed by the same domain concept. Missing domain state is interpreted as pending.
Duplicate domain reports update the existing child state rather than creating duplicates.

**Rationale**: This satisfies the no per-domain fields requirement and allows future participating domains without
adding new columns such as `bookingProvisioned` or `teamProvisioned`.

**Alternatives considered**:

- Add boolean fields per domain: rejected by the spec and brittle for future domain additions.
- Derive readiness only from events without persistence: rejected because the hot path needs a durable single lookup.

## Decision: Required-domain list is centralised in the customer domain

Add a single customer-domain service or configuration object that returns the required non-customer domains. Readiness
derivation, event handling, and auth/readiness checks must use that service rather than duplicating the list.

**Rationale**: Scattered lists are the most likely source of drift when adding a new publisher or updating rollout
requirements.

**Alternatives considered**:

- Copy the list into handlers, middleware, and tests: rejected because it makes correctness depend on manual sync.

## Decision: Missing central readiness blocks access after rollout

No backward compatibility fallback is required. Existing customers without central readiness state are activating or
pending once the central gate is enabled. Operators will manually trigger customer synchronisation/backfill using the
customer republish/sync path.

**Rationale**: The clarified rollout accepts downtime and avoids building a temporary fan-out fallback that would
undermine the single-source-of-truth goal.

**Alternatives considered**:

- Backfill before switching the gate: valid but rejected by clarification.
- Grandfather existing active customers: rejected because it preserves a second readiness rule during rollout.

## Decision: Reuse and extend the existing customer workaround republish flow

Use the existing customer workaround republish surface as the operator path for manual synchronisation/backfill. The
flow republishes customer source events, participating domains idempotently reprocess them and republish readiness, and
customer processors update central readiness.

**Rationale**: The repository already has a customer republish API and tests. Extending or reusing it keeps rollout
aligned with the expected backfill flow.

**Alternatives considered**:

- Directly write readiness state during backfill: rejected by the spec because backfill should exercise the same
  participating-domain provisioning path as production events.

## Decision: Auth/readiness hot path reads one customer-domain readiness service

Replace backend readiness/auth fan-out with a single customer-domain readiness lookup. A missing readiness aggregate or
missing required domain state returns activating/pending and blocks normal authenticated/federated access.

**Rationale**: The feature's core value is moving cross-domain checks off the request hot path while preserving the
same readiness guarantee.

**Alternatives considered**:

- Cache fan-out results at the gateway: rejected because it still depends on distributed domain checks and cache
  invalidation rules outside the customer source of truth.

## Decision: Verification spans event contracts, processors, persistence, and access gating

Use unit tests for deterministic derivation, enum mapping, publisher conditions, duplicate handling, and logging.
Use integration tests where Kafka event handling or customer-domain persistence is involved, with repository-layer
assertions only. Regenerate event code with `api-definitions/events/generate.sh` and build affected projects.

**Rationale**: The change crosses generated contracts, Kafka, persistence, and auth gating. Unit-only coverage would
miss the integration risks called out by the constitution.

**Alternatives considered**:

- End-to-end-only verification: rejected because it would be slow and less precise for idempotency and derivation
  rules.
