# Feature Specification: Rename PurchaseCadence to MembershipTerm

**Feature Branch**: `048-membership-term-rename`
**Created**: 2026-09-03
**Status**: Draft
**Input**: User description: "Use the Spec Kit workflow to specify a repository-wide terminology rename. Rename the customer-facing and domain-facing concept currently called `MembershipTerm` to a clearer term that communicates the contractual period a customer signs up for. The recommended term is `MembershipTerm`. The rename must cover the entire repository, including backend domain models, services, validators, workflows, APIs, GraphQL, OpenAPI, protobuf/event contracts, persistence, serialization, mappers, and tests; all web apps, generated Relay artifacts, and generated API clients; and public-facing copy, help content, product documentation, specifications, comments, logs, test names, and fixtures."

## Clarifications

### Session 2026-09-03

- Q: Should deployed clients and persisted records support a temporary compatibility window for the old field name while new clients adopt `MembershipTerm`? → A: Include persistence and existing records in this changeset, with a forward EF migration that renames storage fields while preserving values.
- Q: Should the application read and write the renamed `MembershipTerm` storage field while exposing `MembershipTerm` everywhere? → A: Yes; use the renamed field consistently across persistence, domain, contracts, APIs, UI, and documentation.
- Q: Should the later storage migration rename the field for all existing product and offering records, including inactive and historical records? → A: The storage migration is part of this changeset.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Understand and choose a membership term (Priority: P1)

As a customer or host, I want the product and booking experience to call the contractual signup period a “membership term,” so that I understand what period I am purchasing and do not confuse it with billing frequency or a single booking duration.

**Why this priority**: Clear terminology directly affects purchase comprehension and every user-facing surface.

**Independent Test**: Review each customer-facing and host-facing flow that displays, edits, validates, or explains the former concept and confirm it uses “membership term” consistently while retaining the same business meaning and behavior.

**Acceptance Scenarios**:

1. **Given** an offer with a defined contractual period, **when** a user views or edits its pricing, **then** the label, help text, validation, and displayed value identify that period as the membership term.
2. **Given** an offer that renews, **when** a user reviews its renewal or booking information, **then** the membership term is distinguishable from the organization billing cycle and individual booking duration.

### User Story 2 - Preserve domain behavior during the rename (Priority: P1)

As an operator, I want existing pricing, subscription, entitlement, booking, invoicing, renewal, and refund behavior to remain unchanged while the terminology is renamed, so that the migration does not alter customer commitments or financial outcomes.

**Why this priority**: A vocabulary change must not change contractual, scheduling, or financial semantics.

**Independent Test**: Exercise representative non-renewing, renewing, entitlement, billing-sliced, cancellation, refund, and invoice scenarios before and after the rename and compare decisions, dates, amounts, persisted meaning, and emitted state.

**Acceptance Scenarios**:

1. **Given** an existing pricing option, **when** it is read, written, renewed, serialized, projected, or mapped after the rename, **then** its membership-term value and all dependent decisions are equivalent to the former behavior.
2. **Given** a longer membership term with internal billing or booking slices, **when** invoices and bookings are generated, **then** those slices remain governed by their existing independent rules and are not relabeled or reinterpreted as the membership term.

### User Story 3 - Maintain repository-wide contract consistency (Priority: P1)

As a developer integrating with Skedular, I want every supported contract, generated artifact, and repository reference to use the new terminology, so that consumers do not encounter competing names or stale generated APIs.

**Why this priority**: Partial renames create integration failures, stale documentation, and ongoing ambiguity.

**Independent Test**: Search the complete repository, excluding only intentionally documented migration-history evidence if any is required, and verify that no active model, field, contract, generated artifact, test, fixture, comment, log, or documentation reference uses `MembershipTerm`.

**Acceptance Scenarios**:

1. **Given** every backend, contract, persistence, serialization, mapper, workflow, API, GraphQL, OpenAPI, protobuf/event, web, and generated surface, **when** artifacts are regenerated and checked, **then** the public and internal names consistently use `MembershipTerm`.
2. **Given** a client or event consumer using the renamed contract, **when** it builds and exchanges the updated representation, **then** it receives and maps the membership-term field without stale generated names or incompatible hand-maintained artifacts.

### Edge Cases

- Existing persisted and serialized records must continue to be readable and retain their original contractual meaning during the first rollout; storage renaming for existing products and offerings is a separate migration change.
- Unknown, unset, nullable, credit-entitlement, or non-renewing values must retain their existing semantics and must not be converted into a membership term merely because the name changed.
- Search-and-replace must not rename unrelated uses of “purchase,” organization billing cycles, invoice cadence, or individual booking duration.
- Generated schemas, clients, Relay artifacts, event classes, and API clients must be regenerated from their source definitions rather than edited independently.
- Logs, diagnostics, fixtures, test names, and documentation that describe the concept must use the new terminology without leaking stale labels to operators or customers.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The repository MUST rename the active domain and customer-facing concept `MembershipTerm` to `MembershipTerm` wherever it represents the contractual period a customer signs up for.
- **FR-002**: The rename MUST cover backend models, services, validators, workflows, persistence representations, serialization, mappers, APIs, GraphQL, OpenAPI, protobuf/event contracts, and their generated outputs.
- **FR-003**: The rename MUST cover every web app and web-facing surface, including the customer webapp, Host, Spaces, public web, legacy editors, generated Relay artifacts, and generated API clients.
- **FR-004**: The rename MUST cover public copy, help content, product documentation, specifications, comments, logs, test names, fixtures, and other active repository terminology.
- **FR-005**: The system MUST preserve all existing membership-term values, renewal decisions, booking-generation decisions, entitlement behavior, billing interactions, invoice behavior, cancellation behavior, refund behavior, and authorization behavior.
- **FR-006**: The system MUST continue to distinguish the membership term from organization billing cycles, invoice schedules, booking duration, availability cadence, and any other independently named period.
- **FR-007**: Contract source definitions MUST be updated before generated schemas, clients, event artifacts, and Relay artifacts are regenerated, and generated outputs MUST match those definitions.
- **FR-008**: The this changeset MUST rename active code, contracts, serialization mappings, generated artifacts, applications, tests, fixtures, logs, and documentation including persisted storage fields for products and offerings.
- **FR-009**: The this changeset MUST rename persisted fields and preserve existing values through the forward migration.
- **FR-010**: Persistence MUST read and write the renamed storage field, while the domain, contracts, APIs, UI, and documentation expose only `MembershipTerm`.
- **FR-011**: The database-field rename and migration for existing products and offerings MUST be included in this changeset.
- **FR-012**: Active repository searches and build/test checks MUST demonstrate that no stale `MembershipTerm` identifier or user-facing label remains outside the historical migration evidence.
- **FR-013**: The feature MUST provide updated tests and fixtures that verify the new terminology across contract boundaries, storage compatibility, and representative domain workflows without changing test intent.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Renamed workflows and meaningful state transitions MUST retain structured start, completion, decision, failure, and recovery logging with membership-term terminology.
- **LOG-002**: Logs and diagnostics MUST preserve correlation context and MUST NOT expose sensitive customer or payment data merely to describe the membership term.
- **LOG-003**: Compatibility, regeneration, or migration failures MUST produce actionable warnings or errors that identify the affected contract or data surface.

### Key Entities

- **Membership term**: The contractual period represented by an offer’s pricing and used by renewal and term-level purchasing behavior.
- **Pricing option**: The offer configuration that stores the membership term alongside independent renewal, duration, availability, entitlement, and pricing rules.
- **Subscription or entitlement**: A customer commitment or allocation whose lifecycle may use the membership term while retaining existing renewal and redemption semantics.
- **Generated contract surface**: Any API, schema, event, client, or UI artifact derived from an authoritative repository definition and required to expose the renamed concept consistently.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of active repository references to the former identifier across source, contracts, generated artifacts, applications, tests, fixtures, logs, and documentation are replaced or explicitly classified as historical migration evidence.
- **SC-002**: 100% of representative pricing, renewal, entitlement, booking, billing, invoice, cancellation, and refund scenarios retain the same business outcomes after the rename.
- **SC-003**: All supported applications and contract consumers build successfully with the renamed terminology and no stale generated field, type, or label remains.
- **SC-004**: A reviewer unfamiliar with the implementation can identify the contractual signup period as the membership term, and can distinguish it from billing cycle and booking duration, in every primary offer and subscription flow.
- **SC-005**: Existing persisted records and newly created records can be read and interpreted consistently, with zero unexplained loss or reinterpretation of membership-term values in compatibility verification.

## Assumptions

- `MembershipTerm` is the final replacement term for active code, contracts, UI copy, and documentation.
- This feature is a terminology migration, not a redesign of term values, renewal policy, billing cycles, booking duration, entitlement rules, or financial calculations.
- The repository’s authoritative contract definitions and established generation workflows determine the exact generated artifact updates.
- The database-field rename and migration for existing products and offerings are included in this changeset and are forward-only.
- Historical specifications or migration notes may mention `MembershipTerm` only when needed to explain prior behavior; active guidance and current terminology must use `MembershipTerm`.
- Existing authentication, authorization, persistence infrastructure, rollout controls, and deployment processes remain unchanged except where required to safely carry the renamed field.
