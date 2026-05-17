# Quickstart: Bulk Resource Import

**Feature**: 008-bulk-resource-import
**Date**: 2026-05-13

## Overview

This guide describes how to validate the bulk resource import end-to-end after implementation.

---

## Prerequisites

- Local dev stack running (`docker-compose up` or Aspire host).
- A seeded organization with at least one location and at least one resource-type tag.
- Admin credentials for that organization.
- `gh` CLI authenticated (optional, for schema inspection).

---

## 1. Verify the GraphQL Mutation Is Registered

After running `scripts/generate-graphql.sh`, inspect the location schema:

```bash
cat location/domain/Location.Domain.IntegrationTests/schema.graphql | grep bulkImportResources
```

Expected output includes:

```
bulkImportResources(input: BulkImportResourcesInput!): BulkImportResourcesPayload! @cost(weight: "100")
```

---

## 2. Call the Mutation Directly (via GraphQL playground or curl)

```graphql
mutation BulkImportResources {
  bulkImportResources(
    input: {
      locationId: "<your-location-id>"
      rows: [{ organizationResourceTypeTagId: "<desk-type-tag-id>", baseName: "Desk", quantity: 3, customTagIds: [], zoneIds: [], productTagIds: [] }]
    }
  ) {
    results {
      rowIndex
      createdResources {
        id
        name
      }
      failureReason
    }
  }
}
```

**Expected response** — three resources created with names `Desk-1`, `Desk-2`, `Desk-3`
(or `Desk-4` onwards if `Desk-1` through `Desk-3` already exist):

```json
{
  "data": {
    "bulkImportResources": {
      "results": [
        {
          "rowIndex": 0,
          "createdResources": [
            { "id": "...", "name": "Desk-1" },
            { "id": "...", "name": "Desk-2" },
            { "id": "...", "name": "Desk-3" }
          ],
          "failureReason": null
        }
      ]
    }
  }
}
```

---

## 3. Verify Partial Success

Submit one valid row and one invalid row (quantity = 0):

```graphql
mutation BulkImportPartialSuccess {
  bulkImportResources(
    input: {
      locationId: "<your-location-id>"
      rows: [
        { organizationResourceTypeTagId: "<desk-type-tag-id>", baseName: "GoodDesk", quantity: 2, customTagIds: [], zoneIds: [], productTagIds: [] }
        { organizationResourceTypeTagId: "<desk-type-tag-id>", baseName: "BadDesk", quantity: 0, customTagIds: [], zoneIds: [], productTagIds: [] }
      ]
    }
  ) {
    results {
      rowIndex
      createdResources {
        id
        name
      }
      failureReason
    }
  }
}
```

**Expected**: row 0 creates `GoodDesk-1`, `GoodDesk-2`; row 1 returns `failureReason: "Quantity must be at least 1."`.

---

## 4. Verify Empty Base Name Fallback

Submit a row with no `baseName` and confirm names are derived from the resource type:

```graphql
rows: [
  {
    organizationResourceTypeTagId: "<room-type-tag-id>"
    quantity: 2
    customTagIds: []
    zoneIds: []
    productTagIds: []
  }
]
```

**Expected**: resources named `Room-1`, `Room-2` (using the resource-type tag name as the base).

---

## 5. Verify the UI Bulk Add Button

1. Navigate to **Locations → [Location] → Resources**.
2. Click **"Bulk add"** button (next to the existing "Add resource" button).
3. The bulk import dialogue opens.
4. Click **"Add row"**, fill in resource type and quantity.
5. Click **"Import"**.
6. The results view shows the created resources and any per-row errors.
7. Verify the new resources appear in the resource list after the dialogue closes.

---

## 6. Run the Integration Tests

```bash
dotnet test location/domain/Location.Domain.IntegrationTests \
  --filter "FullyQualifiedName~BulkImportResources"
```

Expected: all `BulkImportResourcesShould` tests pass.

---

## 7. Run the Unit Tests

```bash
dotnet test location/apis/Location.Api.UnitTests \
  --filter "FullyQualifiedName~BulkImportResources"
```

Expected: all unit tests for `BulkImportResourcesService` pass.
