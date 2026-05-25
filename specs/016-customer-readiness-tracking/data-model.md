# Data Model: Customer Readiness Tracking

## CustomerIdentityProvisioningState

Customer-owned aggregate representing central readiness for one customer.

### Fields

- `customerId`: Customer identifier. Required. Unique across readiness states.
- `overallStatus`: Current readiness status. Required.
- `domainStates`: Collection of `CustomerIdentityProvisioningDomainState`.
- `lastUpdatedAt`: Last time any readiness state for the customer changed.
- `activatedAt`: Time the customer first became active. Null while activating.

### Status values

- `Activating`: Not every required non-customer domain has reported provisioned. This includes missing aggregate state
  and missing per-domain rows.
- `Active`: Every required non-customer domain has reported provisioned.
- `Failed`: Reserved only for existing customer-domain failure mechanisms, not set by the initial
  `CustomerIdentityProvisioned` event.
- `ActionRequired`: Reserved only for existing customer-domain manual intervention mechanisms, not set by the initial
  `CustomerIdentityProvisioned` event.

### Validation and invariants

- One readiness state per customer.
- Missing readiness state is treated as `Activating` by access checks.
- `overallStatus` becomes `Active` only when all configured required domains have a provisioned domain state.
- Duplicate successful reports never regress `Active` to `Activating`.
- `activatedAt` is set only on the first transition to `Active`.
- No customer profile, preference, metadata, organisation, tenant, failure, correlation, or causation data is stored as
  part of the readiness event payload.

## CustomerIdentityProvisioningDomainState

Child state recording readiness for one participating non-customer domain.

### Fields

- `customerId`: Parent customer identifier. Required.
- `domain`: Participating-domain enum value. Required.
- `status`: Domain readiness status. Required.
- `lastUpdatedAt`: Last time the domain state changed or was reconfirmed.

### Status values

- `Pending`: Default derived state when no provisioned state exists for a required domain.
- `Provisioned`: The participating domain has durably provisioned the minimum customer identity needed for
  authenticated/federated execution.

### Validation and invariants

- Unique key is `(customerId, domain)`.
- Duplicate `CustomerIdentityProvisioned` reports update or confirm the same row; they do not insert duplicates.
- Unknown or unmappable domain values are not published by participating domains and should not create readiness state.
- Missing domain state is interpreted as `Pending` without requiring a stored row.

## RequiredCustomerReadinessDomainSet

Customer-domain configuration/service that returns the required non-customer domains for readiness derivation.

### Fields

- `domains`: Ordered or unordered set of required participating domains.

### Current required domains

- Booking
- Organisation
- Team
- Marketplace
- Location
- Core
- Slack
- MsTeams

### Validation and invariants

- The set is owned centrally in the customer domain.
- The set excludes customer and any unknown/unspecified/none domain values.
- Event handling, readiness derivation, auth/readiness checks, and tests use this single source instead of local copies.

## CustomerReadinessEvent

Generic public topic event value for `customer_readiness`.

### Fields

- `metadata`: Standard event metadata with id, source, type discriminator, time, and correlation context following the
  repository event envelope pattern.
- `data`: Typed event data container.

### Type values

- `CustomerIdentityProvisioned`: Initial and only supported type for this feature.

### Validation and invariants

- Unknown future event types are ignored or logged according to existing subscriber conventions without failing known
  event processing.
- The key contains `customerId` so readiness events for the same customer partition together.

## CustomerIdentityProvisionedPayload

Typed payload for the initial readiness event.

### Fields

- `customerId`: Customer identifier that the participating domain has provisioned.
- `domain`: Participating-domain enum value for the publishing domain.

### Validation and invariants

- Payload contains only `customerId` and `domain`.
- `domain` is one of the contract-defined non-customer participating domains.
- A publisher that cannot map itself to a known domain enum value does not publish.

## State transitions

```text
No central state or missing required domain
  -> Activating

Activating + CustomerIdentityProvisioned(domain)
  -> Activating when any required domain remains pending or missing

Activating + CustomerIdentityProvisioned(last missing required domain)
  -> Active and set activatedAt

Active + duplicate CustomerIdentityProvisioned(domain)
  -> Active
```

`Failed` and `ActionRequired` are not introduced by the initial readiness event because the payload has no failure
fields. They may only be used if an existing customer-domain mechanism already defines those states.
