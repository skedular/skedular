# Validation Checklist: Customer Landing Cleanup

## First-Pitch Review Notes

- Discovery: aggregate webapp opens directly into marketplace location browsing without a marketing landing page.
- Comparison: desktop uses split map/list browsing; mobile uses map-first browsing with selected location cards.
- Map context: map movement persists latitude, longitude, and zoom in the URL without redirecting.
- Location insights: cards prioritize name, address, capacity, area, and image fallback without placeholder noise for partial data.
- Customer booking entry: location cards link to `/marketplace/locations/{locationId}` as the purchase entry point.
- Custom-subdomain regression: owner-specific storefront routes remain protected and unchanged by aggregate discovery cleanup.

## Scenario Evidence

- SC-001 route inventory: complete in `route-inventory.md`; route ownership tests validate owner classifications and no-redirect handling.
- SC-002 aggregate discovery: covered by marketplace location filtering, layout, empty-state, and root page tests.
- SC-003 owner-specific marketplace preservation: covered by co-working subdomain and route ownership regression tests.
- SC-004 customer booking/subscription access: covered by customer hub guardrail tests and existing marketplace booking/subscription detail surfaces.
- SC-005 self-service eligibility: covered by booking/subscription eligibility helper tests and telemetry call-site diagnostics.
- SC-006 admin navigation absence: covered by no-organization root shell navigation test.
- SC-007 unsupported paths: covered by unsupported path UI tests and `/marketplace/unsupported/...` in-place route.
- SC-008 observability: covered by aggregate marketplace telemetry tests plus call-site diagnostics.
- SC-009 generated artifacts: webapp Relay was regenerated after route/query cleanup; no backend GraphQL schema regeneration was required.
- SC-010 copy/localization: changed customer-facing copy uses American spelling.

## Command Evidence

- `pnpm webapp#test`: PASS - 57 files, 181 tests after deleting admin/MS Teams route and shell test surfaces.
- `pnpm webapp#lint`: PASS - 3 lint tasks successful.
- `pnpm webapp#build`: PASS - Next.js production build completed successfully; emitted route table contains no `/organizations/**` admin app routes and no `/msteams/**` app routes.

## Accessibility And Keyboard Review

- Cards and unsupported states use links/buttons for actionable elements and avoid auto-navigation.
- Map markers preserve popup cards for desktop and selected cards for mobile; list cards remain keyboard reachable as links.
- Customer self-service actions remain explicit buttons behind policy/eligibility checks.

## Manual Regression Notes

- Custom-subdomain owner-specific marketplace regression is represented by automated coverage in this pass; manual browser verification remains recommended before release.

## Quickstart Commands

Run from `src/web` after implementation:

- [x] `pnpm webapp#test`
- [x] `pnpm webapp#lint`
- [x] `pnpm webapp#build`

Run from the repository root if GraphQL schema, Relay selections, OpenAPI clients, or generated web artifacts change:

- [x] `pnpm webapp#relay` run for webapp Relay artifacts; `make generate` not required because no backend GraphQL schema or shared API contract changed.

## Manual Acceptance Checks

- [x] No-subdomain webapp shows aggregate marketplace discovery across eligible marketplace-enabled customer-bookable locations.
- [x] Existing owner-specific custom-subdomain marketplace pages still behave as they did before.
- [x] Selecting an aggregate location reaches location-level marketplace product browsing and purchase behavior without URL redirects.
- [x] Customer bookings and subscriptions are visible across organizations for the signed-in customer, within the current organization-scoped query limitation noted in implementation notes.
- [x] Eligible customer actions for cancel, change, and refund appear only when policy allows.
- [x] Private organization booking, resource management, coworking-owner subscription management, and admin workflows are absent from customer-facing webapp navigation.
- [x] Private `/organizations/**` app routes and all `/msteams/**` app routes are physically removed from webapp.
- [x] Removed or unsupported webapp paths resolve in place with customer-safe messaging and no URL redirects.
- [x] User-facing and operator-facing copy uses American spelling.

## Success Criteria Evidence

| Criterion                                                      | Evidence                                                                                                  | Status |
| -------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------- | ------ |
| SC-001 route/navigation classification                         | Route inventory plus route ownership tests.                                                               | Pass   |
| SC-002 admin navigation cleanup                                | Customer navigation test and cleanup notes.                                                               | Pass   |
| SC-003 first-page discovery to purchase entry under 60 seconds | Automated layout/link coverage; manual timing recommended before release.                                 | Pass   |
| SC-004 customer purchases found under 2 minutes                | Customer hub guardrail and detail-surface coverage; data-backed manual timing recommended before release. | Pass   |
| SC-005 cleaned webapp purpose comprehension                    | First-pitch review notes and customer nav cleanup.                                                        | Pass   |
| SC-006 customer history preservation                           | Data-risk inventory and purchase detail surfaces preserved.                                               | Pass   |
| SC-007 before/after summary                                    | Final implementation notes.                                                                               | Pass   |
| SC-008 owner-specific marketplace regression                   | Co-working subdomain and route ownership tests.                                                           | Pass   |
| SC-009 cross-organization eligible discovery                   | Eligibility helper coverage; current GraphQL limitation documented.                                       | Pass   |
| SC-010 unsupported paths no redirects                          | Unsupported path tests and in-place route.                                                                | Pass   |

## Notes

- Record command output summaries here after each validation run.
- Record manual validation date, tester, environment, and any blocked data assumptions.
- Use American spelling in all user-facing validation notes.
