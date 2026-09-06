# Research: MembershipTerm Terminology Rename

## Decision: Separate active terminology rename from storage migration

**Rationale**: Phase one renames active domain and contract terminology while the existing product/offering storage field remains readable and writable. A later changeset may rename the storage field and migrate data.

**Alternatives considered**: Renaming storage together was rejected because it expands deployment and rollback scope. Renaming only display labels was rejected because the requested change covers domain, contracts, generated artifacts, tests, and documentation.

## Decision: Use an explicit persistence compatibility mapping

**Rationale**: The domain model, external contracts, and persistence use `MembershipTerm`, with a forward migration preserving stored values. This preserves records without dual storage fields or a backfill.

**Alternatives considered**: Reading/writing both storage names was rejected because the migration is forward-only and dual-write behavior is unnecessary.

## Decision: Treat source contracts as authoritative and regenerate derivatives

**Rationale**: Repository governance requires source-first changes for protobuf events, GraphQL, OpenAPI, and Relay artifacts. Update definitions and run established generators; do not hand-edit generated output.

**Alternatives considered**: Hand-editing generated schemas and clients was rejected because it creates drift and is overwritten by generation.

## Decision: Preserve semantics, not names

**Rationale**: `MembershipTerm` keeps the existing values and contractual-period behavior. Billing cycles, invoice schedules, booking duration, availability, entitlement validity, and renewal decisions remain separate concepts.
