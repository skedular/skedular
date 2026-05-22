# Data Model: Cross-Domain Patch Updates

## In-Scope Editable Record

Represents an existing record owned by a remaining domain after organisation whose editable state is already changed
through an update surface.

### Record families from current inventory

- **Booking**: private booking, marketplace booking, and private recurring booking update state.
- **Customer**: self/profile details, admin customer details, billing details, and admin identity update state.
- **Location**: location details, opening hours, physical address, restricted information, floor plans, resource
  positions, resources, and resource available hours.
- **Marketplace**: product/version details edited through product update flows.
- **Team**: team details, team member membership, and combined team/member update state.

### Validation rules

- The owning domain continues to resolve record identity and permissions before applying selected changes.
- Existing validation, events, cache updates, workflow interactions, and cross-domain service boundaries stay owned by
  that domain.
- Omitted fields and related values outside the selected change set remain unchanged.
- Organisation records already migrated by the preceding feature are reference behaviour, not new work here.

## Patch Field Selection

Represents the explicit allowlist-backed caller intent for a migrated update surface.

### Fields

- `fieldsToUpdate`: Required typed selection of patch fields for the surface. GraphQL surfaces use an input field and
  patch-field enum; gRPC update inputs use repeated typed field selection in the protobuf contract.
- Surface field enum values: the fields or aggregate edit units that the owning update surface permits.

### Validation rules

- Unknown, unsupported, deprecated, or unauthorised fields are rejected before any changes are persisted.
- Field selection must state clear intent independently from nullable, empty, or default-like submitted values.
- A grouped edit unit can map several submitted values to one aggregate selected field where those values must validate
  together.

## Patch Update Request

Represents a migrated GraphQL or gRPC update request carrying record identity, selected values, and explicit field
selection.

### Fields

- Existing target identifier values already required by the update surface.
- Existing mutation or RPC correlation values where present.
- `fieldsToUpdate` field selection.
- Optional submitted values for each allowlisted field or aggregate edit unit.

### State transitions

```text
Submitted
├── Rejected: record not found
├── Rejected: unauthorised selection or operation
├── Rejected: unsupported or invalid field selection
├── Rejected: invalid selected value or grouped edit unit
├── Retried: concurrency conflict detected, latest state reloaded, selected fields reapplied
├── Accepted: valid no-op, latest details returned
└── Applied: selected changes persisted, latest details returned
```

### Validation rules

- Validate the record and authorisation before applying changes.
- Validate the whole selected field set before persisting any selected values.
- Apply the selected changes atomically inside the owning update operation.
- If existing concurrency detection reports a conflict, reload the latest record state and retry only the selected
  fields.

## Autosave Edit Unit

Represents the client-side save boundary for a migrated user-facing edit screen.

### Fields

- `recordTarget`: Existing record identifier or route context for the owning screen.
- `selectedFields`: Field selection emitted by the edit unit.
- `values`: Submitted field or grouped values.
- `saveState`: idle, saving, saved, or failed state visible to the affected edit area.

### Validation rules

- Independent fields can autosave as individual edit units.
- Related fields that must validate together autosave as one grouped edit unit.
- Redundant update buttons disappear only for values covered by autosave; explicit actions for creation, deletion,
  workflow submission, or confirmation can remain.

## Update Result

Represents the successful or rejected result returned to a migrated caller.

### Validation rules

- Successful updates and valid no-op updates return the latest details needed by the current surface to reconcile
  displayed state.
- Rejections identify the failed selected field or edit area where the existing surface supports field-level feedback.
- Logs capture selected-field processing branches and correlation context without sensitive submitted payload values.
