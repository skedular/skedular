# Current State Review: Skedular Spaces Pricing Implementation

**Date**: 2026-06-14  
**Feature**: [spec.md](./spec.md)  
**Graphify Context**: graph.json from `/graphify` execution

---

## Summary

The existing codebase has partial scaffolding for Spaces pricing:
- `SpacesPricingCatalogProvider.GetSpacesOffering()` returns a ProductOffering with EnterpriseCapacity plan (placeholder)
- `PrivateBookingService` handles private booking CRUD but does not check quota
- `BookPrivateRecurringResources` workflow manages recurring booking generation without Spaces quota awareness

This implementation adds the missing quota enforcement, subscription management, and frontend surfaces.

---

## Graphify Findings

### Nodes & Edges Analysis

```
SpacesPricingCatalogProvider [community=4721]
  --> .GetSpacesOffering() [method] [EXTRACTED]

PrivateBookingService [community=684]
  --> .AddAsync() [method] [EXTRACTED]
  --> .UpdateAsync() [method] [EXTRACTED]
  --> .DeleteAsync() [method] [EXTRACTED]
  --> IPrivateBookingService [implements]

BookPrivateRecurringResources [community=1097]
  --> .ExecuteAsync() [method] [EXTRACTED]
  --> .RecurringBookingUpdatedAsync() [method] [EXTRACTED]
  --> .RecurringBookingDeletedAsync() [method] [EXTRACTED]
```

### Community Insights

- **Community 4721**: Organization pricing catalog (SpacesPricingCatalogProvider)
- **Community 684**: Booking API services (PrivateBookingService)
- **Community 1097**: Booking workflows (BookPrivateRecurringResources)

No cross-community edges exist yet between pricing catalog and booking enforcement - this implementation creates those connections.

---

## Current State Assessment

### What Exists

| Component | Status | Notes |
|-----------|--------|-------|
| `PricingCatalogConstants.cs` | Complete | Defines enum codes for Spaces (Spaces = 2), plan codes, availability |
| `SpacesPricingCatalogProvider.cs` | Partial | Returns EnterpriseCapacity placeholder only; needs Free/Growth/Business plans |
| `PrivateBookingService.cs` (API layer) | Complete | Handles CRUD via shared service; no quota checks |
| `IPrivateBookingService.AddAsync()` | Present | Calls `sharedPrivateBookingService.AddAsync()` without quota validation |
| `BookPrivateRecurringResources` workflow | Complete | Generates recurring instances without quota enforcement |

### What Needs to be Added

| Component | Purpose |
|-----------|---------|
| Spaces plan mappings (Free/Growth/Business/Contact Us) | Catalog expansion |
| Existing offering model | Track Spaces subscription state alongside Teams offering state |
| Organization.Offering JSONB | Replicated Spaces plan, quota, custom capacity, and billing period boundaries |
| ISpacesBookingUsageRepository | Booking-row current-period usage count |
| SpacesBookingQuotaService | Evaluate quota decisions |
| SpacesBookingUsageRolloverService | First-day-of-month compatibility hook; usage is derived from booking rows |
| OrganizationSpacesSubscriptionMigrationService | Default-Free assignment |
| GraphQL surfaces (Organization + Booking) | API exposure |
| Frontend components | Quota status, upgrade prompts |

### Key Integration Points

1. **PrivateBookingService.AddAsync()** → Must call quota service before booking creation
2. **BookPrivateRecurringResources activity** → Must check quota per generated instance
3. **Organization GraphQL** → Subscription read/update mutations
4. **Booking GraphQL** → Quota status query, quota error payload

---

## Migration Path from Existing Teams Pattern

The implementation follows the existing Teams V1 catalog and Temporal rollover pattern:

| Team Feature | Spaces Equivalent |
|--------------|-------------------|
| `TeamsPricingCatalogProvider` | `SpacesPricingCatalogProvider` (extend) |
| Organization offering subscription | Existing offering model Spaces fields |
| Temporal monthly rollover activity | SpacesBookingUsageRolloverIntegrations compatibility hook |
| EF Core subscription state | Existing Organization offering plus Booking replicated Organization.Offering JSONB |

---

## Ready for Implementation

**Foundation Phase**: Can begin immediately after this review.

**Blocking**: None - all prerequisites are in place or being added in Phase 2.
