# Research: Unified Host Listing Experience

**Feature**: 032-unified-host-listing  
**Date**: 2026-07-07

## Unknowns from Technical Context

### 1. GraphQL Query Strategy for Unified Listing Data

**Unknown**: What is the optimal approach to retrieve unified Location + Product data?

**Context**: The spec requires showing both Location properties and Product (listing) configuration on the same page. We need to determine how to efficiently fetch this combined data.

**Research Findings**:

1. **Option A: Separate GraphQL queries with client-side union**
   - Query `LocationDetails` from LOCATION_API
   - Query `ProductDetails` for each location via PrecomputedLocationProduct relationship
   - Merge on the client
   - Pros: Simple, uses existing types
   - Cons: Requires 2+ queries per listing, more complex client logic

2. **Option B: Add a new unified backend query**
   - Create a new query that fetches Location + Product in single request
   - Pros: Single round-trip, cleaner client code
   - Cons: Adds backend/API surface that the feature does not actually require

3. **Option C: Use existing PrecomputedLocationProduct as lookup**
   - Query locations with PrecomputedLocationProducts collection
   - Then batch load Products by ID using the existing `productById` field
   - Pros: Leverages existing relationships, minimal new code
   - Cons: Still requires 2 queries

**Decision**: Prefer existing host queries and existing `location { products { ... } }` access where already available. If a page needs one additional frontend query, keep it in the web app only and still target existing GraphQL fields. Do not add a backend aggregation API.

### 2. Backend Service Layer Changes

**Unknown**: Do we need a new service layer or can we extend existing services?

**Research Findings**:

1. **Current Services**:
   - `ProductService.cs` in Marketplace API handles Product CRUD operations
   - Location API has existing GraphQL mutations for Location operations
   - Product API has existing GraphQL mutations for Product operations
   - PrecomputedLocationProducts are managed by the location domain

2. **Recommended Approach**: Use existing GraphQL APIs directly
   - Frontend calls existing Location GraphQL mutations for location updates
   - Frontend calls existing Product GraphQL mutations for product updates
   - No new backend services required
   - Frontend coordinates the two API calls as needed

3. **No Breaking Changes**: All existing services remain unchanged

**Decision**: Do not introduce `IListingService`, a backend unified listing orchestrator, or any equivalent cross-domain service unless a concrete blocker appears that cannot be solved in the host frontend.

### 3. Frontend Form Architecture

**Unknown**: How should the unified form be structured?

**Research Findings**:

1. **Current Implementation**:
   - Host already has a location create/edit flow
   - Host already has a mature product add/edit form stack in `components/product/*`
   - Host still exposes legacy product index/detail/edit routes and navigation

2. **Proposed Structure**:

```
locations/[id]/
├── page.tsx                  # Unified edit page (replaces separate Location/Product pages)
└── products/                 # Keep for backward compat but redirect
    ├── page.tsx              # Redirect to /locations/[id]
    └── create/page.tsx       # Redirect to /locations/new or /locations/[id]
```

3. **Form Components**:
   - Reuse the existing location create/edit pieces for location fields
   - Reuse the existing product editor form stack for pricing, policies, amenities, media, and booking rules
   - Add only a thin host-specific composition layer that places those sections on one screen

**Decision**: The real implementation should be a frontend composition/refactor, not a greenfield form rewrite. New lightweight wrapper components are acceptable, but they should sit on top of the existing host location/product editors.

### 4. Navigation Changes for Skedular Host Only

**Unknown**: How to ensure Skedular Spaces/Teams are unaffected?

**Research Findings**:

1. **Application Structure**:
   - `webapp-host`: Skedular Host product
   - `webapp-spaces`: Skedular Spaces product
   - `webapp-teams`: Skedular Teams product

2. **Product Page Location** (from graph):
   - `/products/page.tsx` exists in webapp-host
   - Should not exist or redirect in other products

3. **Permissions Check**: The existing permission system should already distinguish Host vs Spaces/Teams users based on organization type.

**Decision**:

- Remove Products navigation from webapp-host sidebar
- Add 404 or redirect for direct access to old Product pages in webapp-host only
- No changes needed for other products

### 5. Subscription Pricing Implementation Details

**Unknown**: How should subscription (longer-term >1 day) pricing be displayed?

**Context**: Clarification: Subscription bookings are for longer-term stays (>1 day) grouped under a subscription that can be auto-renewed or non-auto-renewed.

**Research Findings**:

1. **Current GraphQL Types**:
   - `ProductPricing` has `billingMode` (likely includes subscription mode)
   - `ProductPricingCadenceDetails` handles billing cadence

2. **Display Strategy**:
   - Show both per-booking and subscription options
   - Group subscriptions together in pricing UI
   - Include auto-renewal toggle for subscription-based listings

**Decision**: Reuse the existing product pricing editor behavior, including cadence, billing mode, cancellation rules, and subscription auto-renewal. Avoid inventing a parallel simplified pricing model in the unified page.

## Summary of Resolved Unknowns

| Unknown                 | Decision                                                                              |
| ----------------------- | ------------------------------------------------------------------------------------- |
| GraphQL query strategy  | Reuse existing host GraphQL fields and only add minimal frontend query glue if needed |
| Backend service layer   | Use existing Location and Product GraphQL APIs directly; no new service               |
| Frontend form structure | Compose existing host location and product editors into a unified host flow           |
| Navigation scope        | Only modify webapp-host; no changes to other products                                 |
| Subscription pricing    | Reuse existing product pricing editor behavior                                        |

## Dependencies Identified

1. **PrecomputedLocationProduct entity** - Already exists, links Location to Product
2. **Existing Temporal workflow** - Continues to create hidden Resource/Product/ProductTag
3. **Existing host location/product components** - Already implement most required validation and form behavior
