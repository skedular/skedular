# Research: Skedualr Host App

**Feature**: #026-scheduler-host-app  
**Branch**: `026-scheduler-host-app`  
**Date**: 2026-06-28

## Research Questions

This research addresses the following unknowns from the implementation plan:

1. **How to implement auto-Resource creation for Host Products?**
2. **What event type configuration ensures "full place" booking for Location-level products?**
3. **How should Host commission calculation integrate with existing payment flows?**
4. **How to render Host Locations on the Spaces map with a distinct badge?**

---

## Decision 1: Auto-Resource Creation Pattern

**Decision**: Create a new service `AutoResourceService` in `Location.Shared.Services` that:
1. Watches for Product creation where `Organization.Type == OrganizationType.Host`
2. Creates a single Resource named after the Location (e.g., "Main Space" or custom)
3. Links the Resource to the Location via existing foreign key relationship
4. Tags the Resource with `ProductType.Event` so booking books entire Location

**Rationale**:
- Separation of concerns: Location domain owns Resources; auto-creation is a business rule
- Reuses existing foreign keys and relationships (no schema changes)
- Event-based pattern matches Kafka event architecture (already in use)

**Alternatives considered**:
1. *Trigger-based* (database trigger) — Rejected: Hard to test, bypasses business logic validation
2. *API middleware/gateway* — Rejected: Would require intercepting all Product creation endpoints across domains
3. *Domain service in Location domain* — **CHOSEN**: Clean separation, existing patterns exist

**Implementation sketch**:
```csharp
// In Location.Shared.Services/AutoResourceService.cs
public class AutoResourceService : IAutoResourceService
{
    public async Task HandleProductCreatedAsync(Product product)
    {
        if (product.Organization.Type != OrganizationType.Host) return;
        
        // Check if Resource already exists for this Product-Location pair
        var existing = await _context.Resources
            .Where(r => r.LocationId == product.LocationId && r.Name == $"Host: {product.Location.Name}")
            .FirstOrDefaultAsync();
            
        if (existing != null) return; // Idempotent
        
        var resource = new Resource
        {
            Name = $"Host: {product.Location.Name}",
            LocationId = product.LocationId,
            Capacity = product.Location.Capacity, // or default to 10
            Tags = new List<OrganizationTag> 
            { 
                new OrganizationTag { Type = OrganizationTagType.ProductType, Value = ProductType.Event }
            }
        };
        
        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();
    }
}
```

---

## Decision 2: Full-Place Booking via ProductType.Event

**Decision**: For Host Products, always use `ProductType.Event` instead of `ProductType.Resource`. The Event type:
1. Books all matching Resources in the Location for the chosen time
2. Prevents partial bookings (entire Location unavailable when any Resource booked)
3. Uses existing `ProductType.Event` semantics from Spaces

**Rationale**:
- `ProductType.Event` already exists and implements full-place booking logic
- No new event type configuration needed — just default to Event for Host Products
- Consistent with existing booking engine behavior

**Alternatives considered**:
1. *New "FullPlace" product type* — Rejected: Unnecessary complexity, `Event` already does this
2. *Custom Resource-based logic* — Rejected: Would duplicate booking engine functionality
3. *Database constraint* — Rejected: Would require schema changes, violates thin-abstraction goal

**Implementation sketch** (Product creation):
```csharp
// In Product service or API endpoint for Host orgs
var product = new Product
{
    OrganizationId = hostOrg.Id,
    LocationId = location.Id,
    // ... other fields
};

// Auto-set ProductType.Event for Host Products
if (hostOrg.Type == OrganizationType.Host)
{
    // Ensure at least one ProductVersion with Event type
    var version = new ProductVersion
    {
        ProductType = ProductType.Event,
        // ... pricing, tags, etc.
    };
    product.ProductVersions.Add(version);
}
```

---

## Decision 3: Commission Calculation Integration

**Decision**: Integrate Host commission through Organization offerings and Booking-owned payment accounting:
1. Define `HostStandardV1` with a 5% commission in the Organization pricing catalog.
2. Persist the selected rate on `OrganizationOffering` and propagate it through Organization contracts.
3. Snapshot commission, Host payout, and rate on each Marketplace booking.
4. Apply the commission as a Stripe Connect Checkout application fee.

**Rationale**:
- Reuses existing Organization offering and Booking payment flows
- Commission is versioned per offering, not configured per process or environment
- Booking retains the applied financial snapshot for auditability

**Alternatives considered**:
1. *Separate commission microservice* — Rejected: Overkill for simple percentage calculation
2. *Stripe webhook hooks* — Rejected: Would require new webhook endpoints and state tracking
3. *Post-payment deduction* — Rejected: Would require holding funds, complex refund handling

**Implementation shape**: Organization exposes the active offering's commission rate through its event and gRPC contracts. Booking calculates and persists the applied rate, commission amount, and Host payout when totals are finalized, then supplies the commission amount as the Stripe Connect Checkout application fee.

---

## Decision 4: Host Badge on Spaces Map

**Decision**: Extend existing map rendering to:
1. Add `IsHost` flag derived from `Organization.Type == Host`
2. Render Host pins with a new icon (e.g., `HomeRounded`) and "HOST" badge
3. Add filter option for organization type (`filter=host`)

**Rationale**:
- Reuses existing map infrastructure (GraphQL queries, component structure)
- Minimal changes to public-web `/public-web/src/app/` pages
- Filter extension follows same pattern as Spaces filtering

**Alternatives considered**:
1. *New dedicated Host map* — Rejected: Fragmented discovery experience
2. *CSS-only badge* — Rejected: Would require changing map component source (Space app), creating coupling
3. *Backend-driven badge* — **CHOSEN**: Simple, follows existing pattern for Space badges

**Implementation sketch** (GraphQL fragment extension):
```graphql
# In src/web/apps/webapp/queries/LocationCard.graphql
fragment LocationCard_location on Location {
  id
  name
  organization {
    type  # New: include org type in query
    isOwnershipVerified
  }
  # ... existing fields
}
```

```typescript
// In components/locationMap/locationMarker.tsx
const LocationMarker = ({ location }: Props) => {
  const orgType = location.organization?.type;
  const isHost = orgType === OrganizationType.Host;
  
  return (
    <div className="location-marker">
      {isHost && <span className="host-badge">HOST</span>}
      {/* ... existing marker content */}
    </div>
  );
};
```

---

## Research Summary

| Question | Decision | Impact |
|----------|----------|--------|
| Auto-Resource creation | Domain service pattern (no schema changes) | Low risk, testable |
| Full-place booking | Use existing `ProductType.Event` | Zero new code needed |
| Commission calculation | Offering-owned rate + Booking payment snapshot | Reuses canonical Organization and Booking ownership |
| Map badge | Frontend extension with filter | Minimal UI changes |

---

## Unresolved / Deferred

- **Exact map styling for Host badge**: Specified in Phase 1 design (UI/UX review)
- **Performance testing for large Location lists**: Deferred to Phase 2 (load tests)
- **Audit logging for commission calculations**: May require additional schema fields
