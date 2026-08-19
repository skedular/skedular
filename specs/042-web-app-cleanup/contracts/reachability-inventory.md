# Reachability Inventory Contract

This is a review artifact, not a runtime API. Every deletion decision must be reproducible from it.

## Baseline Inventory — 2026-08-19

| Application | Source root | Route-related files | Component-path files | Test files | Validation scripts |
|---|---|---:|---:|---:|---|
| `webapp` | `src/web/apps/webapp` | 66 | 443 | 83 | `lint`, `test`, `build`, `test:e2e`, `relay` |
| `webapp-spaces` | `src/web/apps/webapp-spaces` | 138 | 367 | 56 | `lint`, `test`, `build`, `test:e2e`, `relay` |
| `webapp-teams` | `src/web/apps/webapp-teams` | 110 | 294 | 36 | `lint`, `test`, `build`, `test:e2e`, `relay` |
| `webapp-host` | `src/web/apps/webapp-host` | 91 | 377 | 49 | `lint`, `test`, `build`, `test:e2e`, `relay` |

Route-related files were counted from each app’s `src/app` and `src/rootPages` trees using page, layout, route, and proxy filename conventions. Component-path files were counted under each app’s `src/components` path. These are inventory baselines, not deletion decisions.

## Scope and Exclusions

- In scope: `src/web/apps/webapp`, `src/web/apps/webapp-spaces`, `src/web/apps/webapp-teams`, and `src/web/apps/webapp-host`.
- Protected: route files, route-level tests, unresolved candidates, shared/UI packages, generated artifacts, dependencies, `.next`, and unrelated applications.
- The repository contains `webapp-host`, not a separate `webapp-hosts` directory.
- Existing ignore files were verified at repository scope: `.gitignore`, `.dockerignore`, `.npmignore`, `.prettierignore`, and `.terraformignore`.

## Confirmed Cleanup Candidates

| Application | Path | Classification | Evidence | Action |
|---|---|---|---|---|
| `webapp` | `src/web/apps/webapp/src/components/refund/ModificationRefundPreview.tsx` | `unused` | No file-path or exported-symbol consumers under `src/web/apps` or app tests; no route, barrel, dynamic-load, workspace, or generated-operation dependency | Removed |
| `webapp` | `src/web/apps/webapp/src/components/availabilityDashboard/` | `unused` | No route or retained application import; references were limited to the directory’s own barrel, sibling components, component-only tests, and their Relay documents | Removed |
| `webapp` | `src/web/apps/webapp/src/components/organization/organizationAdmin/` | `unused` | No reachable Next.js route or retained application import; references were limited to the directory, its component tests, and generated Relay documents | Removed |
| `webapp` | `src/web/apps/webapp/src/components/organization/organizationMarketplaceSetup/` | `unused` | No reachable Next.js route or retained application import; references were limited to the directory, its component tests, and generated Relay documents | Removed |
| `webapp-host` | `src/web/apps/webapp-host/src/components/locationMap/hostMarker.tsx` | `unused` | No route, import, export, or dynamic-load consumer under `src/web/apps` | Removed |
| `webapp-host` | `src/web/apps/webapp-host/src/components/locationMap/locationMarker.tsx` | `unused` | No route, import, export, or dynamic-load consumer under `src/web/apps` | Removed |
| `webapp-host` | `src/web/apps/webapp-host/src/components/product-form/ProductForm.tsx` | `unused` | No route, import, export, or dynamic-load consumer under `src/web/apps` | Removed |
| `webapp-host` | `src/web/apps/webapp-host/src/components/product-table/ProductTable.tsx` | `unused` | No route, import, export, or dynamic-load consumer under `src/web/apps` | Removed |
| `webapp` | `src/web/apps/webapp/src/components/bankAccount/` | `unused` | No `webapp` route or retained application import; active equivalents exist only in `webapp-spaces` and `webapp-host`; related tests and Relay documents were component-local | Removed |
| `webapp` | `src/web/apps/webapp/src/components/listGridToggle/` | `unused` | No `webapp` route or retained application import; references were limited to the component’s own test and barrel | Removed |
| `webapp` | `src/web/apps/webapp/src/components/contactEmail/` | `unused` | No `webapp` route or retained application import | Removed |
| `webapp` | `src/web/apps/webapp/src/components/contactPeople/` | `unused` | No `webapp` route or retained application import | Removed |
| `webapp` | `src/web/apps/webapp/src/components/contactPhone/` | `unused` | No `webapp` route or retained application import | Removed |
| `webapp` | `src/web/apps/webapp/src/components/stripeConnectAccount/` | `unused` | No reachable `webapp` route or retained application import; remaining references were internal to the component chain or stale link helpers | Removed |
| `webapp` | `src/web/apps/webapp/src/components/analytics/` and related location/organization insight directories | `unused` | The insight chain had no reachable `webapp` route; `OrganizationAnalytics` had no retained consumer, and all remaining references were internal to this chain or generated Relay documents | Removed |
| `webapp` | `src/web/apps/webapp/src/components/location/addLocation/` | `unused` | No reachable `webapp` add-location route or retained consumer; `NewLocationButton` was only used by the separately retained but currently unreachable organization locations chain | Removed |
| `webapp` | `src/web/apps/webapp/src/components/organization/organizationLocation/`, `weekOpeningHours/`, and `closedOpenAllDayCustomToggle/` | `unused` | No reachable `webapp` route or retained consumer; the toggle and opening-hours components were only reachable through the deleted organization-location component chain | Removed |
| `webapp` | Remaining organization management chains (`organizationLocations/`, `organizationProducts/`, `organizationTeams/`, `organizationUsers/`, `organizationTeam/`, `addOrganizationPaymentMethod/`, `editOrganizationCustomTag/`, `editOrganizationProductTag/`, `editOrganizationZone/`) | `unused` | No `webapp` route or retained consumer outside the deleted organization-management surface; shared selectors used by active team creation were preserved | Removed |
| `webapp` | `src/web/apps/webapp/src/components/team/` | `unused` | No `webapp` route or retained import; active booking flows handle team selection inline | Removed |
| `webapp` | `src/web/apps/webapp/src/components/booking/editPrivateBooking/` and `editPrivateRecurringBooking/` | `unused` | No `webapp` route or retained import; the only external reference was an orphaned source-inspection test | Removed |
| `webapp` | `src/web/apps/webapp/src/components/booking/addBooking/new-booking-dialog.tsx` and `add-private-booking.tsx` | `unused` | No `webapp` route or retained import; `NewBookingButton` was independently retained because `resource-card` uses it | Removed |
| `webapp` | `src/web/apps/webapp/src/components/resource/resource-card.tsx`, `booking/addBooking/new-booking-button.tsx`, and `floorPlan/floorPlans/` | `unused` | No active `webapp` route reaches the floor-plan page component; the resource card and booking button were only reachable through that orphaned chain | Removed |
| `webapp` | `src/web/apps/webapp/src/components/resourceType/` | `unused` | No `webapp` route or retained component import; resource-type handling is inline elsewhere | Removed |
| `webapp` | `src/web/apps/webapp/src/components/booking/bookings/` | `unused` | No active `webapp` route or retained import reaches the organization bookings chain; `myBookings` remains active separately | Removed |
| `webapp` | `src/web/apps/webapp/src/components/zone/` and legacy `booking/myBookings/` presentation files | `unused` | The active `/marketplace/bookings` route reaches `CustomerBookingsHub`; the legacy zone/my-bookings components have no active webapp route or retained consumer | Removed |
| `webapp` | `src/web/apps/webapp/src/components/address/`, `customTag/`, `datePickers/`, `productTag/`, `resource/`, and `search/` | `unused` | No active webapp route or retained component import reaches these directories | Removed |
| `webapp` | `src/web/apps/webapp/src/components/floorPlan/` and `generics/` | `unused` | No active webapp route or retained component import reaches these directories | Removed |
| `webapp` | `src/web/apps/webapp/src/components/moreActionsMenu/` | `unused` | No active webapp route or retained component import reaches this directory | Removed |
| `webapp` | `/welcome`, `rootPages/welcome/`, and `components/setupFlow/` | `unused` | Webapp no longer has onboarding; removed the stale route, flow, and no-organization-shell redirect | Removed |
| `webapp` | `src/web/apps/webapp/src/components/organization/addOrganization/` and onboarding organization link helpers | `unused` | These components were reachable only from the removed welcome onboarding flow; no active webapp route imports them | Removed |

## Retained for Further Review

Files that appear unreferenced by a simple filename scan but are coupled to generated Relay artifacts, route conventions, or indirect runtime behavior remain retained until those paths are explicitly proven unused. They are not speculative deletion candidates. Relay artifacts for the removed availability dashboard were regenerated through the app’s Relay compiler.

## Validation Results

| Application | Lint | Tests | Build/TypeScript | Route result |
|---|---|---|---|---|
| `webapp` | Passed | 78 files, 245 tests passed | Passed; TypeScript and 24 static pages generated | Protected route inventory retained |
| `webapp-spaces` | Passed | 55 files, 201 tests passed | Passed; TypeScript and 21 static pages generated | Protected route inventory retained |
| `webapp-teams` | Passed | 35 files, 102 tests passed | Passed; TypeScript and 25 static pages generated | Protected route inventory retained |
| `webapp-host` | Passed | 48 files, 186 tests passed | Passed; TypeScript and 25 static pages generated | Protected route inventory retained |

## E2E and Route Comparison Results

| Application | Playwright result | Route comparison |
|---|---|---|
| `webapp` | 8 passed | No route, layout, middleware, proxy, or API route files changed |
| `webapp-spaces` | 1 passed, 1 failed because the existing `Sign in` locator matched two links under strict mode | No route, layout, middleware, proxy, or API route files changed |
| `webapp-teams` | 1 passed, 1 failed because the existing `Sign in` locator matched two links under strict mode | No route, layout, middleware, proxy, or API route files changed |
| `webapp-host` | Blocked before test start because `http://localhost:15006` was already in use; no process was terminated | No route, layout, middleware, proxy, or API route files changed |

## Required record

```text
application: <webapp | webapp-spaces | webapp-teams | webapp-host>
path: <repository-relative path>
symbol: <optional exported symbol>
classification: <used | conditionally-used | unused | unresolved>
evidence: <route/import/export/dynamic-load/test/workspace/unresolved explanation>
consumers: <zero or more retained consumer paths>
deletionEligible: <true | false>
```

## Rules

1. Record route roots for every app before deletion.
2. Route files and route-level tests are protected by default.
3. Shared/UI, generated, dependency, or ambiguous references make a candidate ineligible.
4. Delete only `unused` app-owned candidates whose transitive dependencies are also confirmed unused.
5. The final inventory lists deleted candidates, retained ambiguous candidates, affected routes, and validation results.
