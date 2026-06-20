# Data Model: Skedualr Host App

**Feature**: #026-scheduler-host-app  
**Branch**: `026-scheduler-host-app`  
**Date**: 2026-06-28

## Overview

The Host app reuses existing entities from the Location and Customer domains. No new database tables are created; instead, we introduce:
1. A new enum value for `OrganizationType`
2. A computed field on `Location` to detect Host-type organization
3. Business logic for auto-Resource creation

---

## Existing Entities (Reused)

### Organization

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| Type | OrganizationType | **EXTENDED**: Added `Host` value |
| IsOwnershipVerified | bool? | Admin verification flag (existing) |
| Name | string | Display name |
| CreatedAt | DateTimeOffset | Creation timestamp |

**Extension**: New enum value `OrganizationType.Host`

```csharp
// Api.Shared.Services/Models/OrganizationType.cs
public enum OrganizationType
{
    Private,
    Marketplace,
    Host  // replaces Individual while preserving wire value 2
}

public static class OrganizationTypeConstants
{
    public const string Host = "HOST";
    // ... existing constants
}
```

### Location

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| Name | string | Display name (e.g., "My Downtown Apartment") |
| OrganizationId | Guid | Foreign key to Organization |
| Type | LocationType | Existing value from enum |

**Computed Properties**:
- `IsHostLocation`: `Organization.Type == OrganizationType.Host`
- `HasVerifiedOwner`: `Organization.IsOwnershipVerified == true`

### Product

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| OrganizationId | Guid | Foreign key to Organization |
| Inactive | bool | Soft-delete flag |

**For Host Products**:
- `ProductType` is always `Event` (enforced by business logic)
- Uses the Product Tag provisioned for its Host Location

### Resource

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | Primary key |
| LocationId | Guid | Foreign key to Location |
| Name | string | For Host: "Host: {Location.Name}" |

**Auto-Created Fields for Host Locations**:
- `Name`: Derived from Location name
- `Tags`: Contains the Entire Location resource type and Location-specific Product Tag

---

## Entity Relationships

```
Organization (1) ──< (N) Location
Organization (1) ──< (N) Product
Location (1) ──< (N) Resource
Product (N) ──> Product Tag <── (1) hidden Resource ──> Location
```

**Note**: For Host Locations, the Resource and Product Tag are asynchronously provisioned when the Location is created. The relationship is:
- One Host Location → exactly one hidden Resource, one hidden Product Tag, and one inactive provisioned Product for the MVP

---

## State Transitions

### Organization: Private → Verified Host

1. User creates Organization with `Type = Host`
2. Admin reviews and sets `IsOwnershipVerified = true`
3. Host can now create Locations/Products
4. **Log event**: `HostVerificationChanged { OrgId, IsVerified }`

### Location Provisioning (Host Flow)

```
User Creates Location → Start Temporal workflow → Create Product Tag in Organization
                                               → Create hidden Entire Location Resource
                                               → Assign Product Tag to Location and Resource
                                               → Create inactive Marketplace Product using Product Tag
Host Completes Draft → Configure location-specific pricing and policies → Explicitly activate Product
```

**Validation Rules**:
1. Organization may create and edit draft Products before verification, but cannot activate them
2. Product must use the Product Tag provisioned for the selected Location
3. For Host Products: `ProductType` is implicitly `Event`

### Booking Flow (Host)

```
Guest Books Host Product → Check Product.Organization.Type == Host
                         → Book ENTIRE Location (all Resources)
                         → Calculate Commission
                         → Save Booking with InvolvedLocations = [Location]
```

---

## Validation Rules

| Rule | Entity | Description |
|------|--------|-------------|
| V-01 | Organization | Type must be one of: Private, Marketplace, Host |
| V-02 | Organization | If Type is Host and not verified, cannot activate or publicly expose Products |
| V-03 | Location | OrganizationId must reference existing Organization |
| V-04 | Product | Product Tag must belong to the same Organization and Host Location |
| V-05 | Product (Host) | ProductType must be Event (enforced on save) |

---

## Indexes

Existing indexes support the new flow:

| Table | Index Columns | Purpose |
|-------|--------------|---------|
| Organization | Type, IsOwnershipVerified | Filter verified Hosts |
| Location | OrganizationId | Get all Locations for an Org |
| Product/ProductVersion tag join | ProductTagId | Find Products matching the Location Resource |
| Resource | LocationId, Name | Auto-Resource lookup |

**New Index Recommendation**:
```sql
CREATE INDEX IX_Product_Location_Organization_Type
ON Product (LocationId)
WHERE OrganizationId IN (
    SELECT Id FROM Organization WHERE Type = 'HOST'
);
```

---

## Migration Plan

### 1. Add Host enum value (API layer)

```csharp
// Api.Shared.Services/Models/OrganizationType.cs
public enum OrganizationType
{
    Private,
    Marketplace,
    Host  // replaces Individual while preserving wire value 2
}
```

**Rollback**: Remove `Host` from enum (no data migration needed)

### 2. Create Auto-Resource Service (Location domain)

No database changes required; uses existing foreign keys.

---

## Data Volume Assumptions

| Entity | Estimate | Notes |
|--------|----------|-------|
| Host Organizations | 10,000+ | One per host |
| Locations per Host | 1-50 | Avg 5 |
| Products per Location | 1-3 | Multiple pricing tiers |

**Total**: ~50k Locations, ~100k Products at scale
