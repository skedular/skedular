# Data Model: Bulk Resource Import

**Feature**: 008-bulk-resource-import
**Date**: 2026-05-13

## No New Database Entities

No new EF Core entities or migrations are required. All data is persisted in the existing
`Resource` and `OrganizationTagResource` tables.

---

## New Service-Layer Models

These models live in `location/apis/Location.Api/Services/` (or a dedicated sub-namespace
`BulkImportResources/`) and are used purely as service input/output.

### `BulkImportResourceRow`

Input row submitted to `IBulkImportResourcesService`.

| Field                           | Type                    | Notes                                                             |
| ------------------------------- | ----------------------- | ----------------------------------------------------------------- |
| `OrganizationResourceTypeTagId` | `string`                | ID of the resource-type `OrganizationTag`. Required.              |
| `BaseName`                      | `string?`               | Optional base name. Null/empty → derived from resource type name. |
| `Quantity`                      | `int`                   | Number of resources to create. Must be ≥ 1.                       |
| `CustomTagIds`                  | `IReadOnlyList<string>` | IDs of custom (non-type) organization tags.                       |
| `ZoneIds`                       | `IReadOnlyList<string>` | IDs of zone tags.                                                 |
| `ProductTagIds`                 | `IReadOnlyList<string>` | IDs of product tags.                                              |

### `BulkImportResources` (top-level input)

| Field        | Type                                   | Notes                                |
| ------------ | -------------------------------------- | ------------------------------------ |
| `LocationId` | `string`                               | ID of the target location. Required. |
| `Rows`       | `IReadOnlyList<BulkImportResourceRow>` | 1–100 rows; sum of quantities ≤ 100. |

### `BulkImportRowResult`

Per-row output from `IBulkImportResourcesService`.

| Field              | Type                      | Notes                                                       |
| ------------------ | ------------------------- | ----------------------------------------------------------- |
| `RowIndex`         | `int`                     | Zero-based index matching the input row.                    |
| `CreatedResources` | `IReadOnlyList<Resource>` | Created resources (populated on success; empty on failure). |
| `FailureReason`    | `string?`                 | Human-readable reason for failure. Null on success.         |

---

## New Repository Method

Added to `IResourceRepository` / `ResourceRepository`:

```csharp
/// <summary>
/// Returns the names of all active (non-deleted) resources for the given location.
/// Used by the bulk import service to pre-load existing names for conflict-free name generation.
/// </summary>
Task<IReadOnlyList<string>> GetActiveNamesByLocationIdAsync(
    string locationId,
    CancellationToken cancellationToken);
```

---

## Existing Entities Involved (read-only)

### `Resource` (existing, `location/shared/Location.Shared/Database/Entities/`)

Key fields used by the bulk import flow:

| Field                    | Type       | Notes                                                            |
| ------------------------ | ---------- | ---------------------------------------------------------------- |
| `Id`                     | `string`   | Random ID generated via `IRandomHelper`.                         |
| `Name`                   | `string`   | Auto-generated: `{effectiveBaseName}-{n}`.                       |
| `Location`               | navigation | Foreign key to `Location`.                                       |
| `OrganizationTags`       | collection | Includes resource-type tag + custom/zone/product tags.           |
| `Inactive`               | `bool`     | Always `false` on bulk import.                                   |
| `RequireBookingApproval` | `bool`     | Always `false` on bulk import (default).                         |
| `Capacity`               | `int`      | Always `1` on bulk import (default; can be changed post-import). |

### `OrganizationTag` (existing)

| Field  | Type      | Notes                                                                                                              |
| ------ | --------- | ------------------------------------------------------------------------------------------------------------------ |
| `Id`   | `string`  | Tag identifier.                                                                                                    |
| `Type` | `string?` | Non-null for resource-type tags (`OrganizationTagTypeConstants.ResourceTypes`). Null for custom/zone/product tags. |
| `Name` | `string?` | Used as default `effectiveBaseName` when the row's `BaseName` is empty.                                            |

---

## Naming Convention State Machine

```
Input baseName = "Desk", quantity = 3
Existing names at location = ["Desk-1", "Desk-3"]

Step 1: maxExistingSuffix("Desk") = 3
Step 2: allocate Desk-4, Desk-5, Desk-6

Input baseName = null, resourceTypeName = "Room", quantity = 2
Existing names at location = []

Step 1: effectiveBaseName = "Room"
Step 2: maxExistingSuffix("Room") = 0
Step 3: allocate Room-1, Room-2
```

---

## GraphQL Input/Output Types (new)

See `contracts/graphql/bulk-import-resources.graphql` for the full schema contract.

Summary:

| Type                          | Kind   | Purpose                                                    |
| ----------------------------- | ------ | ---------------------------------------------------------- |
| `BulkImportResourcesInput`    | Input  | Top-level mutation input: locationId + rows                |
| `BulkImportResourceRowInput`  | Input  | Per-row input: typeTagId, baseName?, quantity, tag IDs     |
| `BulkImportResourcesPayload`  | Object | Mutation output: clientMutationId + results                |
| `BulkImportResourceRowResult` | Object | Per-row result: rowIndex, createdResources, failureReason? |
