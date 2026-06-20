# Data Model: Unified Host Listing Experience

**Feature**: 032-unified-host-listing  
**Date**: 2026-07-07

## Entities

### Location (Unchanged)

**Purpose**: Represents the physical space being listed.

| Field                       | Type                         | Description                  |
| --------------------------- | ---------------------------- | ---------------------------- |
| id                          | string                       | Primary key                  |
| name                        | string                       | Location name                |
| timezone                    | string?                      | Timezone identifier          |
| type                        | string                       | Private/Public location type |
| openingHours                | OpeningHours?                | JSONB opening hours config   |
| featureImages               | CdnImageFile[]?              | JSONB feature images array   |
| extraMetadata               | LocationExtraMetadata?       | JSONB extra metadata         |
| uniqueClaimCode             | string?                      | Unique identifier code       |
| contactedViaEmail           | bool                         | Email contact status         |
| contactedViaSms             | bool                         | SMS contact status           |
| contactedViaCall            | bool                         | Call contact status          |
| contactedViaWhatsapp        | bool                         | WhatsApp contact status      |
| listingMetadata             | ListingMetadata?             | JSONB marketplace metadata   |
| organizationId              | string                       | Organization foreign key     |
| organization                | Organization                 | Navigation property          |
| resources                   | Resource[]                   | Resources at this location   |
| floorPlans                  | FloorPlan[]                  | Floor plan images            |
| precomputedLocationProducts | PrecomputedLocationProduct[] | Link to Product data         |

### Product (Unchanged)

**Purpose**: Hidden backend entity storing listing configuration.

| Field                       | Type                         | Description              |
| --------------------------- | ---------------------------- | ------------------------ |
| id                          | string                       | Primary key              |
| inactive                    | bool                         | Product visibility flag  |
| organizationId              | string                       | Organization foreign key |
| organization                | Organization                 | Navigation property      |
| productVersions             | ProductVersion[]             | Version history          |
| precomputedLocationProducts | PrecomputedLocationProduct[] | Link to Location data    |

### PrecomputedLocationProduct (Unchanged)

**Purpose**: Junction table linking Location to Product.

| Field            | Type              | Description                |
| ---------------- | ----------------- | -------------------------- |
| id               | string            | Primary key                |
| organizationId   | string            | Organization foreign key   |
| locationId       | string            | Location foreign key       |
| productId        | string            | Product foreign key        |
| organizationTags | OrganizationTag[] | Tags for this relationship |

### Resource (Unchanged - auto-created)

**Purpose**: Bookable item associated with a location.

| Field                  | Type    | Description                     |
| ---------------------- | ------- | ------------------------------- |
| id                     | string  | Primary key                     |
| locationId             | string  | Location foreign key            |
| productConfigurationId | string? | Product configuration reference |

## Relationships

```
Organization (1) <---> (*) Locations
Organization (1) <---> (*) Products
Location (1) <---> (1) PrecomputedLocationProduct
Product (1) <---> (1) PrecomputedLocationProduct
PrecomputedLocationProduct (1) <---> (*) OrganizationTags
Location (1) <---> (*) Resources
```

## State Transitions

### Listing Creation Flow

1. User submits unified host form
2. Frontend creates the Location using the existing Location mutation
3. Existing backend behavior creates or prepares the hidden Resource/Product/Product Tag relationship without any new backend orchestration service for this feature
4. Frontend then coordinates existing Product-domain mutation(s) to persist listing configuration
5. PrecomputedLocationProduct links Location → Product

### Open Verification Point

- The current host implementation and public docs disagree on whether Host location creation already produces an immediately editable hidden product draft.
- The implementation must verify the existing behavior and then choose one frontend sequence:
  - patch the auto-created hidden product draft after location creation, or
  - coordinate the existing product creation flow without exposing Product as a separate host management concept.

### Listing Edit Flow

1. User modifies unified form → Frontend coordinates separate GraphQL mutations
2. Frontend calls:
   - Location GraphQL mutation for location fields (name, address, hours)
   - Product GraphQL mutation for product fields (pricing, policies, availability)
3. Changes validated by existing Product domain rules

## Validation Rules

**Location-level**:

- Name: Required, max 255 chars
- Type: Must be valid location type enum
- OrganizationId: Must belong to current organization

**Product-level** (unchanged from existing):

- Currency: Valid ISO currency code
- Pricing: At least one pricing option required
- Cancellation policy: Valid policy type with refund rules
- Duration limits: min <= max duration in minutes

## New Fields Added (None)

This feature does not add new database fields. It exposes existing Product data through the Host listing flow.
