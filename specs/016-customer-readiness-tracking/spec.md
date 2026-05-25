# Feature Specification: Customer Readiness Tracking

**Feature Branch**: `016-customer-readiness-tracking`  
**Created**: 2026-05-24  
**Status**: Draft  
**Input**: User description: "Implement customer-owned cross-domain readiness tracking using a generic public topic."

## Clarifications

### Session 2026-05-24

- Q: How should existing customers without central readiness state be treated when the new hot-path gate is enabled? → A: No backward compatibility is required; missing central readiness means Activating/Pending, downtime is acceptable, and operators will manually trigger customer synchronisation/backfill.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Gate access from central readiness (Priority: P1)

As an authenticated customer, I need normal federated access to begin only after my identity is recognised by every required participating domain, so that requests do not fail unpredictably depending on which domain handles part of the federated operation.

**Why this priority**: This is the primary safety requirement. It removes readiness fan-out from the access path while preserving the guarantee that federated access is blocked until provisioning is complete everywhere it is required.

**Independent Test**: Can be tested by creating a customer readiness state with only some required domains reported and verifying normal access remains blocked, then reporting every required domain and verifying access is allowed from the single customer readiness source.

**Acceptance Scenarios**:

1. **Given** a customer has no reported participating-domain readiness, **When** the customer attempts normal authenticated federated access, **Then** access remains blocked because the customer is still activating.
2. **Given** a customer has readiness reported for some but not all required participating domains, **When** normal authenticated federated access is checked, **Then** access remains blocked and missing domains are treated as pending.
3. **Given** every required participating domain has reported customer identity provisioning, **When** normal authenticated federated access is checked, **Then** access is allowed based on the customer-owned readiness state.

---

### User Story 2 - Participating domains report durable provisioning (Priority: P2)

As a participating non-customer domain, I need to report that customer identity provisioning is complete only after the local identity is durably available, so that the customer domain can rely on the report as a readiness signal.

**Why this priority**: Central readiness is only trustworthy if readiness events are published after successful local provisioning, not merely after receiving a source customer event.

**Independent Test**: Can be tested by replaying a customer source event through a participating domain and verifying the domain idempotently ensures the local customer identity exists before publishing the readiness event.

**Acceptance Scenarios**:

1. **Given** a participating domain receives a customer source event for a customer it has not provisioned, **When** local provisioning succeeds, **Then** the domain publishes a `CustomerIdentityProvisioned` readiness report.
2. **Given** a participating domain receives a duplicate or replayed customer source event for an already provisioned customer, **When** it revalidates the local identity, **Then** it may publish the same readiness report again without creating duplicate local state.
3. **Given** a domain cannot map itself to a known readiness domain value, **When** it processes a customer source event, **Then** it does not publish a readiness report.

---

### User Story 3 - Customer domain tracks per-domain progress (Priority: P3)

As the customer domain, I need to record readiness progress by participating domain and derive one overall customer readiness status, so that backfills, replays, and future domains can be handled without adding per-domain status fields.

**Why this priority**: This provides the durable source of truth for readiness and makes production backfill safe.

**Independent Test**: Can be tested by delivering readiness reports in any order, including duplicates, and verifying only the reported domains become provisioned while the overall status becomes active only after all required domains report.

**Acceptance Scenarios**:

1. **Given** a customer is activating, **When** a `CustomerIdentityProvisioned` readiness report is received from one required domain, **Then** only that domain is marked provisioned and all unreported required domains remain pending.
2. **Given** a customer is already active, **When** a duplicate successful readiness report is received, **Then** the customer remains active and no duplicate domain state is created.
3. **Given** an unknown future readiness event type is received, **When** the customer domain processes the topic, **Then** known event processing continues and the unknown event does not break readiness handling.

---

### User Story 4 - Backfill historical customer provisioning (Priority: P4)

As an operator, I need historical customer source events to be safely replayable, so that existing customers can build central readiness state without manual per-domain repair.

**Why this priority**: Production rollout depends on backfilling existing customers safely and repeatably.

**Independent Test**: Can be tested by replaying historical customer source events for active, partially provisioned, and missing customers and verifying the resulting readiness state is correct and duplicate-safe.

**Acceptance Scenarios**:

1. **Given** historical customer source events are republished, **When** participating domains reprocess them, **Then** each domain ensures local identity provisioning and republishes readiness safely.
2. **Given** a customer already has active readiness, **When** the same readiness reports arrive during backfill, **Then** the customer remains active without duplicate transitions.
3. **Given** a customer is partially provisioned during backfill, **When** additional required-domain reports arrive, **Then** the customer progresses toward active while missing domains remain pending.

### Edge Cases

- Duplicate readiness reports for the same customer and domain must not create duplicate domain readiness entries or repeated meaningful transitions.
- Missing domain readiness is treated as pending until a report arrives.
- A customer that has already become active must not regress to activating because a duplicate successful report is replayed.
- Existing customers without central readiness state are treated as activating or pending after the new gate is enabled until manual synchronisation/backfill creates the required readiness reports.
- Unknown future readiness event types must not prevent known readiness reports from being processed.
- Participating domains must not publish readiness when their domain identity cannot be represented by the known participating-domain list.
- A readiness report must not imply full customer profile synchronisation; names, profile fields, preferences, metadata, and business data remain outside this feature.
- Readiness reports must not include organisation, tenant, failure, status, timing, correlation, or causation details beyond standard metadata already carried by the shared event envelope.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST provide a customer-owned public cross-domain readiness topic named `customer_readiness`.
- **FR-002**: The `customer_readiness` topic MUST be generic enough to support multiple readiness event types over time.
- **FR-003**: The first supported readiness event type MUST be `CustomerIdentityProvisioned`.
- **FR-004**: Each readiness event MUST include standard event metadata, an event type discriminator, and a typed payload.
- **FR-005**: The `CustomerIdentityProvisioned` payload MUST contain only `customerId` and `domain`, excluding any additional business, tenant, status, failure, timing, correlation, or causation fields unless those are part of the standard event metadata.
- **FR-006**: The `domain` value MUST come from a defined participating-domain enumeration owned by this readiness contract.
- **FR-007**: The participating-domain enumeration MUST include only known non-customer domains that can report customer identity provisioning and MUST exclude unspecified, unknown, none, and customer values.
- **FR-008**: The system MUST NOT create a dedicated topic named `CustomerDomainIdentityProvisioned`.
- **FR-009**: Participating non-customer domains MUST publish `CustomerIdentityProvisioned` only after their local customer identity or authentication projection is durably provisioned enough for federated authenticated execution to recognise the customer.
- **FR-010**: Participating domains MUST NOT publish readiness merely because they received a source customer event.
- **FR-011**: Participating domains MUST NOT publish readiness if they cannot map themselves to a known participating-domain enumeration value.
- **FR-012**: Participating-domain customer provisioning handlers MUST be idempotent and replay-safe.
- **FR-013**: Replaying original customer source events MUST cause participating domains to safely ensure local identity provisioning and republish readiness when represented by the participating-domain enumeration.
- **FR-014**: The customer domain MUST consume `customer_readiness` reports and process `CustomerIdentityProvisioned`.
- **FR-015**: The customer domain MUST ignore or otherwise safely handle unknown future readiness event types without breaking known event processing.
- **FR-016**: The customer domain MUST persist readiness state centrally per customer, including one overall readiness status and per-domain readiness states.
- **FR-017**: Per-domain readiness MUST be stored as a collection keyed by the participating-domain concept, not as separate fields or columns for each domain.
- **FR-018**: Missing per-domain readiness MUST count as pending.
- **FR-019**: Because the initial readiness payload contains no failure details, the customer domain MUST NOT invent failure details from this event.
- **FR-020**: The customer domain MUST maintain one configured set of required non-customer domains for readiness decisions.
- **FR-021**: The required-domain list MUST NOT be scattered across access checks, event handlers, query resolvers, or user interface code.
- **FR-022**: The customer domain MUST mark a customer active or ready only when every required non-customer domain has reported provisioned.
- **FR-023**: The customer domain MUST keep a customer activating while any required domain is pending or missing.
- **FR-024**: A customer that is already active MUST NOT regress because of duplicate or replayed successful readiness events.
- **FR-025**: The backend authenticated/federated readiness check MUST read customer-domain readiness state as the single backend source of truth.
- **FR-026**: The backend authenticated/federated readiness check MUST NOT fan out to every participating domain during the request hot path.
- **FR-027**: Normal authenticated/federated access MUST remain blocked until every required non-customer domain has provisioned the customer identity.
- **FR-028**: Production backfill by republishing historical customer source events MUST be safe for existing active, partially provisioned, and missing readiness states.
- **FR-029**: Duplicate readiness events MUST NOT create duplicate per-domain readiness records.
- **FR-030**: Duplicate readiness events MUST NOT create duplicate meaningful state transitions.
- **FR-031**: The feature MUST preserve the existing event envelope, metadata, topic ownership, schema, key/value, protobuf, and versioning conventions used by the repository.
- **FR-032**: Generated event outputs MUST be produced from source definitions and MUST NOT be manually edited.
- **FR-033**: Handwritten event metadata companions MUST exist for the new readiness topic and version.
- **FR-034**: Existing customers without central readiness state MUST be treated as activating or pending when the central gate is enabled; no backward-compatible legacy readiness fallback is required.
- **FR-035**: Operators MUST be able to manually trigger customer synchronisation or backfill so existing customers can generate central readiness state after rollout.
- **FR-036**: Tests MUST cover partial activation, successful activation, duplicate readiness reports, missing-domain pending behaviour, backfill replay, active-customer non-regression, participating-domain publication timing, no-publication for unmappable domains, existing-customer missing-state blocking, and the single-source backend readiness check.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers)
  and MUST avoid sensitive data leakage.
- **LOG-005**: Readiness processing logs MUST identify customer and participating-domain context without logging sensitive customer profile data.
- **LOG-006**: Duplicate and replayed readiness events MUST be logged as idempotent outcomes when useful for diagnosis, without implying an error.

### Key Entities _(include if feature involves data)_

- **Customer Readiness State**: The customer-owned central state for a customer, including customer identity, overall readiness status, per-domain states, last update time, and activation time when applicable.
- **Customer Readiness Domain State**: A per-domain readiness entry keyed by participating domain, with status such as pending or provisioned and the last time that domain state changed.
- **Participating Domain**: A known non-customer domain that can durably provision minimum customer identity and report readiness.
- **Customer Identity Provisioned Report**: A readiness report stating that one participating domain has provisioned the minimum customer identity required for authenticated federated execution.
- **Required Domain Set**: The central customer-domain configuration describing which participating domains are required before a customer is considered ready.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of normal authenticated/federated readiness decisions use the customer-owned central readiness state rather than querying multiple participating domains during the request path.
- **SC-002**: A customer with any missing required participating-domain report remains blocked from normal access in 100% of readiness checks.
- **SC-003**: A customer with all required participating-domain reports is recognised as ready in 100% of readiness checks.
- **SC-004**: Replaying the same readiness report 10 or more times for the same customer and domain leaves exactly one effective per-domain readiness state and no repeated meaningful activation transition.
- **SC-005**: Replaying historical customer source events for active customers leaves them active in 100% of cases.
- **SC-006**: Replaying historical customer source events for partially provisioned customers progresses only the domains that successfully report readiness and leaves all missing domains pending.
- **SC-007**: Participating domains publish readiness only after successful local identity provisioning in 100% of tested success and failure paths.
- **SC-008**: The readiness contract remains bounded to the initial payload: `CustomerIdentityProvisioned` contains only customer identity and participating domain information apart from standard metadata.
- **SC-009**: After the central gate is enabled, 100% of customers without central readiness state remain blocked until manual synchronisation/backfill produces all required readiness reports.

## Assumptions

- The required participating domains are the existing non-customer domains that currently need local customer identity for authenticated or federated execution, such as organisation, booking, team, marketplace, location, and any other current domain with the same provisioning responsibility.
- Existing customer source events remain the trigger used by participating domains to create or ensure local customer identity projections.
- "Ready", "active", and "synced" should map to the repository's existing customer-domain terminology during planning and implementation.
- The initial feature does not introduce customer profile synchronisation, failure reporting, or operator remediation flows beyond preserving any existing mechanisms.
- Backfill will be performed by manually triggering customer synchronisation that republishes or reprocesses customer source events rather than by writing readiness records directly.
- Backward compatibility for customers without central readiness state is not required during rollout; temporary downtime is acceptable until manual synchronisation/backfill completes.
