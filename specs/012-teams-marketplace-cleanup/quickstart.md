# Quickstart: Remove Marketplace from Web App Teams

**Feature**: 012-teams-marketplace-cleanup
**Date**: 2026-05-24

---

## What This Feature Does

Removes all marketplace-related UI and data from `web/apps/webapp-teams`. After
this change, webapp-teams is a strictly private-organisation product with no
marketplace concepts surfaced anywhere in its interface.

---

## Working on This Feature

### Prerequisites

```bash
# From the repo root — ensure dependencies are installed
pnpm install
```

### Running the app (development)

```bash
cd web/apps/webapp-teams
pnpm dev
```

### Running tests

```bash
cd web/apps/webapp-teams
pnpm test
```

### Regenerating Relay artefacts

After making changes to any GraphQL fragment or mutation definition in source files,
run the Relay compiler to regenerate the artefact set:

```bash
cd web/apps/webapp-teams
pnpm relay
```

**Important**: Before running `pnpm relay`, manually delete the three stale
artefacts that correspond to removed fragment/mutation definitions:

```bash
rm src/queries/__generated__/multipleChoicesProductTags_query.graphql.ts
rm src/queries/__generated__/myBookingCard_deleteMarketplaceBookingMutation.graphql.ts
rm src/queries/__generated__/myBookingCard_deleteMarketplaceBookingSubscriptionMutation.graphql.ts
```

Then run `pnpm relay` to produce the clean updated artefact set.

---

## Key Areas Changed

### 1. Proxy (`src/proxy.ts`)

- `/marketplace` and `/marketplace/:path*` routes removed from the middleware
  matcher and redirect logic.

### 2. Root shell (`src/components/rootShell/`)

- `marketplaceCustomerRecordSynced` removed from the `areCustomerRecordsSync`
  startup gate in both `root-shell.tsx` and `no-organization-root-shell.tsx`.

### 3. Booking list (`src/components/booking/myBookings/`)

- `my-bookings.tsx`: marketplace subscription query and lookup map removed;
  booking card render now skips any booking where `marketplaceBooking` is non-null.
- `my-booking-card.tsx`: marketplace mutation definitions, `isMarketplaceRecurringBooking`
  logic, and all marketplace-conditional UI branches removed.

### 4. Organisation admin (`src/components/organization/organizationAdmin/`)

- `organization-admin-setup-section.tsx`: `marketplaceListingMetadata` field removed
  from the GraphQL fragment and from the patch mutation variable construction.
- `organization-admin.tsx`: `marketplaceListingMetadata` title fallback removed.

### 5. Product tags (deleted entirely from webapp-teams)

- `src/components/productTag/` — entire folder deleted (3 files)
- `src/components/organization/multiple-choices-product-tags.tsx` — deleted
- All `productTagIds` fields removed from resource add / edit / bulk-import forms
- All `productTags` fields removed from resource card, resource management list,
  and floor plan editor fragments

### 6. Links (`src/components/links/index.ts`)

- All `getOrganizationMarketplaceSetup*` and `getOrganizationLocationAddMarketplace*`
  helper functions removed.

### 7. Icons (`src/components/icons/index.tsx`)

- `MarketplaceIcon`, `SetupMarketplaceIcon`, `ProductTagIcon` exports removed.

### 8. More-actions menu (`src/components/moreActionsMenu/more-actions-menu.tsx`)

- `EditProductTag` and `DeleteProductTag` entries removed from
  `MoreActionsMenuOptionType` and `moreActionsMenuAllOptions`.

---

## Verifying the Change

After completing the implementation:

1. **Build check**:

   ```bash
   cd web/apps/webapp-teams && pnpm build
   ```

2. **Test suite**:

   ```bash
   cd web/apps/webapp-teams && pnpm test
   ```

3. **Manual verification checklist**:
   - Navigate to Organisation Admin → confirm no "Marketplace Setup" nav entry
   - Navigate to a resource → confirm no product tag field in edit form
   - Navigate to Add Resource → confirm no product tag picker
   - Navigate to Bookings → confirm no marketplace bookings appear
   - Navigate to a floor plan → confirm no product tag chips on resource canvas items
   - Inspect `src/proxy.ts` → confirm no `/marketplace` entries

---

## Future Work (Out of Scope)

- **Cross-product booking integration**: Private-org employees booking co-working
  space desks via the marketplace, with those bookings appearing in webapp-teams.
  Deferred to a future feature once the proper migration path is designed.
- **Marketplace OpenAPI client cleanup**: The generated files under
  `src/clients/openapi/skedular/v1/marketplace/` are auto-generated. If they are
  not imported by any remaining source file after this cleanup, they can be removed
  in the next `web/apps/webapp/scripts/generate.sh` regeneration cycle.
