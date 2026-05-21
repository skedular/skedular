# Research: Organization Patch Updates

## Decision: Use explicit field selection instead of nullable-value inference

**Rationale**: The core requirement is to distinguish omitted values from values intentionally set to empty, default, or cleared. Explicit selection makes caller intent testable and avoids overloading `null` as both "not provided" and "clear this field".

**Alternatives considered**:

- Infer changed fields from non-null values: rejected because clear/default values become ambiguous.
- Compare incoming values against stored values only: rejected because callers still cannot express "I intended to clear this field" independently of omission.
- JSON Patch-style operations: rejected because the organisation API uses typed GraphQL inputs and an enum mask is clearer for Relay clients.

## Decision: Keep one public `Update*` contract per migrated organisation surface

**Rationale**: The behaviour change is partial update semantics, not a second public API family. GraphQL setup editing uses `updateOrganization` with `fieldsToUpdate`, specialised organisation GraphQL update surfaces use the same field-mask pattern, and migrated gRPC update endpoints keep their normal `Update*` names while their inputs state caller intent explicitly.

**Alternatives considered**:

- Keep temporary `*Patch` GraphQL and gRPC aliases after every update surface is migrated: rejected because it duplicates contract names after full-replacement paths are removed.
- Create a separate REST/OpenAPI patch endpoint: rejected because REST/OpenAPI is not the target first-release surface.
- Keep full-replacement inputs beside field-masked inputs: rejected because it preserves the overwrite risk this feature removes.

## Decision: Initial allowlist is all editable organisation setup fields only

**Rationale**: Name and description verified the pattern. The same patch semantics now cover the editable organisation setup fields previously submitted by the full GraphQL update path so the web apps no longer need the old mutation.

**Alternatives considered**:

- Patch every editable organisation field except a blocklist: rejected because it expands security and regression risk.
- Patch name only: rejected because it does not validate multi-field patch selection.
- Patch all setup fields: accepted after the initial name and description path was verified.

## Decision: Rely on existing entity concurrency and retry selected patch fields

**Rationale**: The organisation persistence layer already has concurrency protection. The GraphQL patch API does not need a caller-supplied version token. If the save fails for a concurrency reason, the service reloads the latest organisation and reapplies only the selected patch fields, preserving all omitted values.

**Alternatives considered**:

- Caller-supplied `expectedVersion`: rejected as unnecessary API surface because concurrency is already handled at the .NET entity layer.
- Reject stale updates and require caller refresh: rejected as heavier than needed for first-release inline editing.
- Last full object wins: rejected because it can overwrite omitted fields and defeats patch semantics.

## Decision: Accept no-op partial updates and return latest organisation details

**Rationale**: Autosave and blur-save flows may submit unchanged values. Treating this as a successful no-op keeps the UI stable, and returning the latest organisation details lets Relay update the rest of the fields from the server response.

**Alternatives considered**:

- Reject no-op updates: rejected because it creates avoidable validation noise.
- Silently ignore no-op updates: rejected because callers need the latest organisation details for reconciliation and logs need a clear processing branch.

## Decision: No database migration for first release unless code discovery proves description is not already represented

**Rationale**: The current organisation model already has a `Name` field and profile/listing metadata surfaces that include descriptive copy. Planning assumes the first implementation maps all editable organisation setup fields to existing persisted fields rather than introducing a new persistence column.

**Alternatives considered**:

- Add a dedicated description column immediately: deferred until implementation verifies that no existing organisation description field is suitable.

## Decision: Regenerate GraphQL outputs through the repository script

**Rationale**: The constitution and repository notes require `scripts/generate-graphql.sh` for backend GraphQL schema changes. Generated GraphQL schema files and Relay artifacts must not be hand-edited.

**Alternatives considered**:

- Hand-edit `schema.graphql` or per-API schema exports: rejected by repository policy.
- Run per-API schema export commands directly: rejected by repository policy; use the script.
