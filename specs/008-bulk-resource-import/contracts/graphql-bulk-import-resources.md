# GraphQL Contract: Bulk Resource Import

**Domain**: `location`
**Feature**: 008-bulk-resource-import
**Date**: 2026-05-13

> **Note**: This contract describes the new types and mutation to be added to the
> `location` API's GraphQL schema. After implementation, run `scripts/generate-graphql.sh`
> to regenerate the composed gateway schema and Relay artefacts.

---

## New Mutation

```graphql
type Mutation {
  """
  Imports multiple resources into a location in a single operation.
  Supports partial success: valid rows are persisted even when other rows in the same
  batch fail validation.
  """
  bulkImportResources(input: BulkImportResourcesInput!): BulkImportResourcesPayload! @cost(weight: "100")
}
```

---

## Input Types

```graphql
input BulkImportResourcesInput {
  """
  Relay client mutation identifier, echoed back in the payload.
  """
  clientMutationId: String

  """
  ID of the location to which all resources in this batch will be added.
  """
  locationId: String!

  """
  One to 100 import rows. The sum of all row quantities must not exceed 100.
  Each row represents a homogeneous group of resources sharing the same type and tags.
  """
  rows: [BulkImportResourceRowInput!]!
}

input BulkImportResourceRowInput {
  """
  Tag ID of the resource-type OrganizationTag for all resources in this row.
  Must match a tag whose Type is in OrganizationTagTypeConstants.ResourceTypes.
  """
  organizationResourceTypeTagId: String!

  """
  Optional base name. The server appends a numeric suffix ("-1", "-2", ...) to generate
  individual resource names. When omitted or empty, the resource type name is used as the
  base name.
  """
  baseName: String

  """
  Number of resources to create from this row. Must be >= 1.
  """
  quantity: Int!

  """
  IDs of custom (non-type) organization tags to assign to all resources in this row.
  """
  customTagIds: [String!]!

  """
  IDs of zone tags to assign to all resources in this row.
  """
  zoneIds: [String!]!

  """
  IDs of product tags to assign to all resources in this row (marketplace only).
  """
  productTagIds: [String!]!
}
```

---

## Payload / Output Types

```graphql
type BulkImportResourcesPayload {
  """
  Relay client mutation identifier.
  """
  clientMutationId: String

  """
  Per-row results in the same order as the input rows.
  Successful rows contain the created resources; failed rows contain a failure reason.
  """
  results: [BulkImportResourceRowResult!]!
}

type BulkImportResourceRowResult {
  """
  Zero-based index matching the input row position.
  """
  rowIndex: Int!

  """
  Resources successfully created for this row.
  Empty when the row failed validation.
  Each resource includes its auto-generated name and assigned tag details.
  """
  createdResources: [Resource!]!

  """
  Human-readable reason why this row was rejected.
  Null when the row was processed successfully.
  """
  failureReason: String
}
```

---

## Validation Rules (enforced server-side, returned as `failureReason`)

| Rule                                                                 | Failure reason                                                                |
| -------------------------------------------------------------------- | ----------------------------------------------------------------------------- |
| `quantity < 1`                                                       | `"Quantity must be at least 1."`                                              |
| `organizationResourceTypeTagId` not found or not a resource-type tag | `"Resource type not found or invalid."`                                       |
| More than one resource-type tag included in the merged tag list      | `"Only a single resource type is allowed per row."`                           |
| Any tag ID not found or not active for the organization              | `"One or more tag identifiers are invalid."`                                  |
| `sum(quantities)` across all rows > 100                              | Top-level input validation — mutation returns error before processing any row |
| `rows` list is empty                                                 | Top-level input validation — mutation returns error                           |

---

## Existing Types Referenced (no changes)

The `Resource` type returned inside `BulkImportResourceRowResult.createdResources` is the
existing `Resource` object type already defined in the location schema:

```graphql
# Existing — no changes required
type Resource {
  id: ID!
  name: String!
  inactive: Boolean!
  requireBookingApproval: Boolean!
  color: String
  capacity: Int!
  customTags: [OrganizationTag!]!
  zones: [OrganizationTag!]!
  productTags: [OrganizationTag!]!
  resourceType: OrganizationTag!
  # ... (other existing fields unchanged)
}
```

---

## C# HotChocolate Input/Output Classes

New files to create:

```
location/apis/Location.Api/GraphQL/Resource/BulkImportResourcesInput.cs
location/apis/Location.Api/GraphQL/Resource/BulkImportResourceRowInput.cs
location/apis/Location.Api/GraphQL/Resource/BulkImportResourcesPayload.cs
location/apis/Location.Api/GraphQL/Resource/BulkImportResourceRowResult.cs
```

The existing `RootMutation.cs` gets one new method:

```csharp
[UseResolverScope]
public async Task<BulkImportResourcesPayload> BulkImportResourcesAsync(
    BulkImportResourcesInput input,
    [Service] IBulkImportResourcesService bulkImportResourcesService,
    CancellationToken cancellationToken)
{
    var result = await bulkImportResourcesService.ImportAsync(
        graphQlMapper.MapTo(input), cancellationToken);
    return new BulkImportResourcesPayload
    {
        ClientMutationId = input.ClientMutationId,
        Results = result.Select(graphQlMapper.MapTo)
    };
}
```
