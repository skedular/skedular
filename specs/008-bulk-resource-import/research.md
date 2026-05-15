# Research: Bulk Resource Import

**Feature**: 008-bulk-resource-import
**Date**: 2026-05-13

## 1. Domain Ownership

**Decision**: `location` domain (`location/apis/Location.Api/`) owns the new mutation.

**Rationale**: Resources are location-owned entities. The existing `IResourceService` and
`ResourceRepository` in `location/` already own all resource CRUD. The new bulk mutation is a
batch wrapper around the existing single-resource add flow.

**Alternatives considered**: Dedicated shared service — rejected; no cross-domain state needed.

---

## 2. How Resource Types Are Modelled

**Decision**: Resource type is **not** a separate entity. It is an `OrganizationTag` with a
`Type` value matching one of `OrganizationTagTypeConstants.ResourceTypes` (e.g. `ResourceDesk`,
`ResourceRoom`). The `organizationResourceTypeId` field on `AddResourceInput` is the tag ID of
the resource-type tag.

**Evidence**:

- `RemoveOrganizationResourceType` migration dropped the `OrganizationResourceType` table.
- `ResourceService.AddAsync` validates: `resource.Tags` must contain exactly one tag whose
  `Type` is in `OrganizationTagTypeConstants.ResourceTypes`.
- `AddResourceInput.organizationResourceTypeId` is passed through the GraphQL mapper which
  merges it into the tags list.

**Impact on design**: `BulkImportResourceRowInput` must include an
`organizationResourceTypeTagId` field (the resource-type tag ID), alongside `customTagIds`,
`zoneIds`, and `productTagIds`. The service layer merges these to build the `Resource.Tags`
list before calling the internal add logic.

---

## 3. Naming Strategy

**Decision**: Server auto-generates resource names using `{baseName}-{n}` suffix where `n`
starts at 1 and increments past the highest existing suffix for that base name at the location.
Gap-filling is **not** used; the system always appends at the end (e.g. if "Desk-1" and
"Desk-3" exist, the next generated name is "Desk-4").

**Rationale**: Gap-filling requires a full sequential scan and complicates the algorithm
without meaningful user benefit. Always-append is predictable, fast, and easy to reason about.

**Algorithm**:

1. For each row, resolve `effectiveBaseName = baseName ?? resourceTypeName`.
2. Before processing any row, fetch all existing active resource names for the location in a
   single query (one DB round trip for the whole batch).
3. Maintain an in-memory set of names already allocated within the current batch.
4. For each resource to generate (up to `quantity` per row):
   - Scan existing + in-batch names matching `{effectiveBaseName}-{n}` pattern.
   - `nextSuffix = max(matchingSuffixes) + 1`, starting from 1 if none found.
   - Allocate `{effectiveBaseName}-{nextSuffix}`, add to in-batch set.
5. All allocations in a row are committed together inside a transaction. If tag validation or
   any other row-level check fails before name generation, the row is rejected and its slot
   is freed.

**New repository method needed**: `GetActiveNamesByLocationIdAsync(string locationId, CancellationToken ct)`
returns `IReadOnlyList<string>` — all active (non-deleted) resource names for a location.

---

## 4. Batch Size Enforcement

**Decision**: The mutation validates that the **sum of all row quantities** ≤ 100.
Individual rows may not have a quantity of zero or negative.

**Rationale**: The spec says "100 resources per import request", meaning total created
resources, not total rows.

---

## 5. Partial Success Processing

**Decision**: Rows are processed independently. A failure in one row (tag validation error,
quantity zero, resource type missing) does not roll back already-created rows.

**Implementation**: Iterate rows; wrap each row's add logic in its own try/catch. Collect
`BulkImportRowResult` per row. All successful rows share a single database transaction (for
efficiency), but individual row validation failures are caught before the DB write and excluded
from the transaction.

**Alternative considered**: One transaction per row — rejected as too slow at 100 resources;
also risks partial commits if row-level errors only surface mid-transaction.

**Refined approach**: Pre-validate all rows before any DB write. Rows that fail pre-validation
are marked as failed immediately. Rows that pass pre-validation are written together in a
single transaction. This avoids partial-transaction complexity while still supporting partial
success.

---

## 6. Existing Service Reuse

**Decision**: Do **not** call the existing `ResourceService.AddAsync` per resource in a loop.
Instead, create `IBulkImportResourcesService` that inlines the same validation logic in a
batch-aware way (single location fetch, single tag fetch, single name-set fetch, one
transaction for all successful rows).

**Rationale**: Calling `AddAsync` N times would fire N location queries, N tag queries, N
authorization checks, and N Temporal outbox entries, which is wasteful. The bulk service
fetches shared data once and reuses it.

**Shared logic reused**: Authorization check, organization offering check, tag validation
shape, `IRandomHelper` for ID generation, `ILocationOutboxPublisher`, `ITemporalOutboxService`.

---

## 7. Frontend Approach

**Decision**: New "Bulk add" button added alongside the existing "Add resource" button in
`organization-location-manage-resources-section.tsx`. Clicking opens a new
`BulkImportResourcesDialog` (same pattern as `AddResourceDialog`).

**Dialog structure**:

- Dynamic row table: each row = ResourceType (single choice), BaseName (optional text),
  Quantity (number, min 1), CustomTags (multi-select), Zones (multi-select), ProductTags
  (multi-select, marketplace only).
- Add/Remove row controls.
- Submit sends `bulkImportResources` Relay mutation.
- After submission: results view shows per-row status (success count / failure reason).
  Failed rows can be retried (pre-populated).

**Relay pattern**: `usePreloadedQuery` + `useQueryLoader` (same as `AddResourceDialogWithRelay`),
collocated fragments. `useMutation` for the bulk mutation.

**Tag components reused**: `SingleChoiceResourceType`, `MultipleChoicesCustomTags`,
`MultipleChoicesZones`, `MultipleChoicesProductTags` — same Relay fragment approach already
used in `EditResource`.

---

## 8. Logging

**Decision**: Structured logging on `IBulkImportResourcesService`:

- `Information` on batch received (locationId, rowCount, totalQuantity).
- `Warning` per failed row (rowIndex, reason — no tag content).
- `Information` on completion (created, rejected counts).
- All entries include request correlation context via standard middleware.

---

## 9. No New Migration Required

**Decision**: No EF Core migration needed. The bulk import reuses the existing `Resource`
table and `OrganizationTagResource` join table. The only new repository method
(`GetActiveNamesByLocationIdAsync`) is a read-only query, not a schema change.

---

## 10. GraphQL Schema Regeneration

After the new mutation and types are added to the backend, run:

```sh
scripts/generate-graphql.sh
web/apps/webapp/scripts/generate.sh
```

This keeps the per-API `schema.graphql`, composed gateway schema, and Relay web artefacts
in sync.
