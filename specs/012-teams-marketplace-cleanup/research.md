# Research: Remove Marketplace from Web App Teams

**Feature**: 012-teams-marketplace-cleanup
**Date**: 2026-05-24
**Phase**: 0 — Codebase exploration and scope resolution

---

## Decision: Relay artefact regeneration approach

**Decision**: After all source file edits are complete, run `pnpm relay` from
`web/apps/webapp-teams/` to regenerate the Relay compiler output. The command is
listed in `web/apps/webapp-teams/package.json` as the `relay` script.

**Rationale**: The three generated artefacts that reference removed fragments and
mutations (`multipleChoicesProductTags_query.graphql.ts`,
`myBookingCard_deleteMarketplaceBookingMutation.graphql.ts`,
`myBookingCard_deleteMarketplaceBookingSubscriptionMutation.graphql.ts`) must be
deleted by hand, then the compiler re-run to produce a clean artefact set. The
broader regenerated query artefacts for `myBookings_query.graphql.ts`,
`organization_rootQuery.graphql.ts`, and `organizationBookings_rootQuery.graphql.ts`
will be refreshed automatically as the compiler processes the updated source files.

**Alternatives considered**: Running `make generate` (umbrella). Rejected because
the umbrella script regenerates backend GraphQL schemas and OpenAPI clients — none
of which change here. The scoped `pnpm relay` within the webapp-teams package is
sufficient and faster.

---

## Decision: Marketplace booking filtering strategy

**Decision**: Filter marketplace bookings at the React component render level in
`my-bookings.tsx`. A booking card is skipped from rendering if the underlying
booking node has a non-null `marketplaceBooking` field. The `marketplaceBooking`
field is already fetched in the booking card fragment; it just needs to be used as
a filter predicate at the list level rather than a branch within the card.

**Rationale**: The backend GraphQL schema is unchanged (per spec assumption).
There is no server-side filter parameter for `privateOnly` bookings in the current
query surface. Adding a component-level guard in the booking list is the lowest-risk
approach: it requires no backend contract change, no new query variable, and is
fully reversible when the cross-product integration is built in a future feature.

**Alternatives considered**:

- Adding a backend filter parameter (e.g., `channel: PRIVATE`). Rejected — spec
  states backend is unchanged.
- Removing all `marketplaceBooking` references from the fragment entirely. Rejected —
  the fragment field is needed at the list level to make the filter predicate work.
  It will be removed from the card's internal logic but kept only as a boolean flag
  for the list-level filter, then removed entirely once the query-level filter is
  available.

---

## Decision: `marketplaceCustomerRecordSynced` in root-shell startup check

**Decision**: Remove `marketplaceCustomerRecordSynced` from the
`areCustomerRecordsSync` boolean expression in both `root-shell.tsx` and
`no-organization-root-shell.tsx`.

**Rationale**: This flag gates the app's readiness state (it delays rendering until
all customer records are synced). Since webapp-teams is a private-org-only product
that no longer uses marketplace, waiting for marketplace customer record sync on
every page load is unnecessary. Removing it eliminates a potential startup delay and
removes the last implicit marketplace dependency from the app shell layer.

**Alternatives considered**: Leaving it in place (no visible UI impact). Rejected —
it introduces an invisible coupling to the marketplace service that conflicts with the
product boundary goal.

---

## Decision: `marketplaceListingMetadata` in organisation admin setup section

**Decision**: Remove the `marketplaceListingMetadata` field from the
`organizationAdminSetupSectionQuery` fragment and from the patch mutation input in
`organization-admin-setup-section.tsx`. Remove the title fallback reference in
`organization-admin.tsx`.

**Rationale**: `marketplaceListingMetadata` is the organisation's public marketplace
listing title and subtitle — content whose sole purpose is to describe the org to
marketplace customers browsing a public co-working directory. This has no function in
a private-org-only context. Keeping it in the admin form misleads admins into
believing their organisations have a public marketplace presence managed from this
app.

**Alternatives considered**: Keeping it as a read-only display field. Rejected —
displaying marketplace data without context or action creates confusion in a
private-org product.

---

## Decision: `getOrganizationLocationAddMarketplaceLink` and associated links

**Decision**: Remove all marketplace-specific link helper functions from
`web/apps/webapp-teams/src/components/links/index.ts`. The functions to remove are:

- `getOrganizationLocationAddMarketplaceLink`
- `getOrganizationMarketplaceSetupBaseLink`
- `getOrganizationMarketplaceSetupProductTagsBaseLink`
- `getOrganizationMarketplaceSetupMarketplaceListingBaseLink`
- `getOrganizationMarketplaceSetupBillingCycleBaseLink`
- `getOrganizationMarketplaceSetupXeroBaseLink`
- `getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink`
- `getOrganizationMarketplaceSetupBankAccountsBaseLink` (if present)

Also remove the "add marketplace location" branch from `new-location-button.tsx`
which calls `getOrganizationLocationAddMarketplaceLink`.

**Rationale**: These link helpers generate URLs to pages that do not exist in
webapp-teams (no `/setup-marketplace` route, no `/locations/add-marketplace` route).
Removing them eliminates dead navigation targets and prevents future accidental use.

**Alternatives considered**: Keeping unused exports. Rejected — TypeScript build
would flag unreferenced imports after callers are removed, and leaving them creates
maintenance confusion.

---

## Decision: Proxy route removal scope

**Decision**: Remove `/marketplace` and `/marketplace/:path*` from the proxy
configuration in `web/apps/webapp-teams/src/proxy.ts`. The middleware logic that
redirects `pathname === '/marketplace'` must also be cleaned up.

**Rationale**: webapp-teams has no marketplace routes. The proxy entries currently
forward requests to paths that don't exist in this app, creating unnecessary surface
area and implying a marketplace presence that conflicts with the private-org boundary.

---

## Complete File Inventory

The following 33 source files require edits. Three generated artefacts require
manual deletion before the Relay compiler re-run.

### Source files to edit

| File                                                                                                       | Change type                                                                                    |
| ---------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| `src/proxy.ts`                                                                                             | Remove marketplace proxy routes                                                                |
| `src/components/rootShell/root-shell.tsx`                                                                  | Remove `marketplaceCustomerRecordSynced`                                                       |
| `src/components/rootShell/no-organization-root-shell.tsx`                                                  | Remove `marketplaceCustomerRecordSynced`                                                       |
| `src/components/links/index.ts`                                                                            | Remove marketplace link helpers                                                                |
| `src/components/icons/index.tsx`                                                                           | Remove `MarketplaceIcon`, `SetupMarketplaceIcon`, `ProductTagIcon`                             |
| `src/components/moreActionsMenu/more-actions-menu.tsx`                                                     | Remove `EditProductTag`, `DeleteProductTag` options                                            |
| `src/components/organization/index.ts`                                                                     | Remove `MultipleChoicesProductTags` export                                                     |
| `src/components/organization/multiple-choices-product-tags.tsx`                                            | DELETE                                                                                         |
| `src/components/organization/organizationAdmin/organization-admin-setup-section.tsx`                       | Remove `marketplaceListingMetadata` field and patch logic                                      |
| `src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx`                  | Remove marketplace test assertions                                                             |
| `src/components/organization/organizationAdmin/organization-admin.tsx`                                     | Remove `marketplaceListingMetadata` title fallback                                             |
| `src/components/organization/organizationLocation/organization-location-resource-management-list.tsx`      | Remove `ProductTags` import and usage                                                          |
| `src/components/organization/organizationLocation/organization-location-resource-management-list.test.tsx` | Remove product tag test assertions                                                             |
| `src/components/organization/organizationLocation/organization-location-manage-resources-section.tsx`      | Remove product tag references                                                                  |
| `src/components/organization/organizationPage/organization.tsx`                                            | Remove `marketplaceBookingSubscriptions` and `marketplaceBookingSubscriptionCancellationModes` |
| `src/components/organization/organizationPage/organization-bookings.tsx`                                   | Remove same fields                                                                             |
| `src/components/booking/myBookings/my-bookings.tsx`                                                        | Remove marketplace subscription query, filter marketplace bookings from render                 |
| `src/components/booking/myBookings/my-booking-card.tsx`                                                    | Remove marketplace mutations and conditional logic                                             |
| `src/components/booking/myBookings/my-booking-card.test.tsx`                                               | Remove marketplace test cases                                                                  |
| `src/components/productTag/index.ts`                                                                       | DELETE                                                                                         |
| `src/components/productTag/product-tag.tsx`                                                                | DELETE                                                                                         |
| `src/components/productTag/product-tags.tsx`                                                               | DELETE                                                                                         |
| `src/components/resource/resource-card.tsx`                                                                | Remove `productTags` from fragment                                                             |
| `src/components/resource/addResource/add-resource-dialog.tsx`                                              | Remove `productTagIds` field and `MultipleChoicesProductTags`                                  |
| `src/components/resource/editResource/edit-resource.tsx`                                                   | Remove `productTagIds` field and `MultipleChoicesProductTags`                                  |
| `src/components/resource/bulkAddResources/bulk-add-resources-row.tsx`                                      | Remove `showProductTags` prop and `MultipleChoicesProductTags`                                 |
| `src/components/resource/bulkAddResources/bulk-add-resources-dialog.tsx`                                   | Remove `showProductTags` usage                                                                 |
| `src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`                                                 | Remove `productTags` from fragment                                                             |
| `src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx`                                               | Remove `productTags` from fragment                                                             |
| `src/components/location/addLocation/new-location-button.tsx`                                              | Remove marketplace location link branch                                                        |
| `src/rootPages/organizations/organization/locations/location/resources/resource/page.tsx`                  | Remove `multipleChoicesProductTagsSortingValues` variable                                      |

### Generated artefacts to delete then regenerate

| File                                                                                              | Action                  |
| ------------------------------------------------------------------------------------------------- | ----------------------- |
| `src/queries/__generated__/multipleChoicesProductTags_query.graphql.ts`                           | Delete (source removed) |
| `src/queries/__generated__/myBookingCard_deleteMarketplaceBookingMutation.graphql.ts`             | Delete (source removed) |
| `src/queries/__generated__/myBookingCard_deleteMarketplaceBookingSubscriptionMutation.graphql.ts` | Delete (source removed) |

After deleting the above: run `cd web/apps/webapp-teams && pnpm relay` to regenerate
the full artefact set. The regenerated `myBookings_query.graphql.ts`,
`organization_rootQuery.graphql.ts`, and `organizationBookings_rootQuery.graphql.ts`
will automatically reflect removed fields.

### Test command

```bash
cd web/apps/webapp-teams && pnpm test
```
