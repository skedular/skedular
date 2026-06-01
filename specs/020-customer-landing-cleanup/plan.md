# Implementation Plan: Customer Landing Cleanup

**Branch**: `020-customer-landing-cleanup` | **Date**: 2026-06-01 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/020-customer-landing-cleanup/spec.md`

## Summary

Simplify `webapp` into the no-subdomain aggregate marketplace and customer self-service surface. Existing owner-specific custom-subdomain marketplaces stay unchanged, while aggregate webapp discovery lists only marketplace-enabled customer-bookable locations across organizations. Private organization booking, coworking-owner booking/subscription/resource management, and admin workflows are inventoried, classified, and removed from customer navigation or handled in place without URL redirects. The implementation should reuse existing marketplace location, product, booking, and subscription flows where possible, adding contract/schema work only when the current GraphQL/Relay surfaces cannot express eligibility, location insight, or policy-bound customer self-service needs.

## Technical Context

**Language/Version**: TypeScript 6.0.3; React 19.2.6; Next.js 16.2.6 App Router; backend C# .NET 10 only if GraphQL/domain contract changes are needed  
**Primary Dependencies**: Relay 21, `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, WorkOS AuthKit, Leaflet/react-leaflet for map browsing, existing marketplace GraphQL schema and generated Relay artifacts  
**Storage**: No new persistence planned for the first cleanup/design slice; uses existing marketplace booking, subscription, location, organization, and customer data via GraphQL. Any new durable cleanup inventory can start as feature documentation/task artifact unless implementation requires product-owned persistence.  
**Testing**: Vitest + React Testing Library for web behavior; Relay/component tests for route and marketplace surfaces; backend unit/integration tests only if GraphQL/domain behavior changes; generated artifact validation when schema/query selections change  
**Target Platform**: Webapp in `src/web/apps/webapp`, deployed as a Next.js web application  
**Project Type**: Frontend-led web application cleanup and product-boundary refactor with possible GraphQL contract additions  
**Performance Goals**: Aggregate marketplace first-page discovery should load without regressing current marketplace page responsiveness; location browse interactions should remain usable on mobile and desktop; customer can reach discovery result and purchase entry within SC-003 60-second usability target  
**Constraints**: No URL redirects from webapp in this phase; custom-subdomain owner-specific marketplace behavior must remain unchanged; aggregate discovery may show only marketplace-enabled customer-bookable locations; no private organization booking/admin controls in customer navigation; generated files must not be hand-edited; UI copy uses American spelling  
**Scale/Scope**: Current webapp route tree, existing marketplace pages, aggregate discovery across eligible locations and organizations, customer booking/subscription hub, route/workflow responsibility inventory, and no-redirect unsupported path handling

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature may touch GraphQL/Relay surfaces if existing marketplace fields cannot express aggregate eligibility, insights, or self-service actions. If so, update source schema/contracts first, run `scripts/generate-graphql.sh` and web Relay generation through `make generate` or the established web script, and do not hand-edit generated artifacts.
- [x] **II. Domain Boundaries** — The feature crosses location, marketplace, booking, organization, and customer concepts, but must do so through existing public GraphQL/domain surfaces. No direct cross-domain database or internal class access is allowed.
- [x] **III. Testing** — Web UI changes require Vitest + React Testing Library. Backend unit and integration tests are required only for changed domain/query behavior. Integration assertions must use repository/query-layer paths, not raw `DbContext`.
- [x] **IV. Frontend** — Web changes are in scope. Relay selections must be colocated, generated Relay artifacts must be regenerated instead of edited, typography wrappers from `@skedular/ui` must be used, and user-facing/operator-facing copy must use American spelling per the current constitution.
- [x] **V. Pattern Consistency** — No new framework or app is introduced. The plan reuses existing `MarketplaceLocations`, location detail, product, booking, subscription, route shell, `@skedular/ui`, and `@skedular/shared` patterns. Any new shared primitive must be extracted only when stable and reusable.
- [x] **VI. Logging** — Structured logging is planned for aggregate discovery, location selection, customer hub loads, self-service action decisions, unsupported path handling, and owner-specific entry resolution. Logs must include correlation context and avoid sensitive payloads.

**Post-Design Re-Check**: PASS. Research and contracts preserve generated-code discipline, no direct cross-domain persistence, current package boundaries, no URL redirects, custom-subdomain regression protection, and explicit observability/test contracts.

## Project Structure

### Documentation (this feature)

```text
specs/020-customer-landing-cleanup/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── aggregate-marketplace.md
│   ├── capability-inventory.md
│   ├── graphql-relay.md
│   ├── observability.md
│   ├── route-map.md
│   └── test-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/web/
├── apps/
│   ├── webapp/
│   │   ├── src/app/                         # Next.js App Router routes and entry-point resolution
│   │   │   ├── page.tsx                     # Root public/custom-domain entry branching
│   │   │   ├── customer-facing-subdomain/   # Existing custom-domain entry-point resolver
│   │   │   └── marketplace/                 # Existing customer marketplace route tree
│   │   ├── src/rootPages/marketplace/       # Existing marketplace page wrappers for locations/products/bookings/subscriptions
│   │   ├── src/components/location/         # Marketplace location discovery/detail components
│   │   ├── src/components/marketplaceProductBooking/
│   │   ├── src/components/marketplaceProductSubscription/
│   │   ├── src/components/organizationStoreFrontGuest/
│   │   ├── src/components/rootShell/
│   │   ├── src/queries/                     # Relay source queries/fragments and generated artifacts
│   │   └── scripts/generate.sh              # Web OpenAPI/Relay generation entry point
│   ├── webapp-teams/                        # Owner for private organization/coworking booking admin workflows
│   └── webapp-spaces/                       # Owner for space administration workflows
├── packages/
│   ├── ui/                                  # Shared visual primitives and typography wrappers
│   └── shared/                              # Shared runtime providers, hooks, utilities
└── package.json                             # pnpm/turbo scripts

api-definitions/
├── graphql/                                 # Source/composed GraphQL artifacts when schema changes are needed
└── openapi/                                 # OpenAPI contracts if web API clients change

src/booking/                                 # Booking/subscription domain behavior if self-service policy contracts change
src/location/                                # Location eligibility/insight behavior if new domain fields are required
src/organization/                            # Organization/custom-domain behavior if owner-specific contracts change
```

**Structure Decision**: Use the existing web monorepo and keep implementation centered in `src/web/apps/webapp`. Reuse current marketplace route and component surfaces first. Add backend/domain or `api-definitions/` work only when existing GraphQL/Relay contracts cannot support aggregate marketplace eligibility, insight, or policy-bound customer self-service requirements.

## Phase 0: Research Output

Completed in [research.md](research.md). Key decisions:

- No-subdomain webapp is the aggregate marketplace layer.
- Existing marketplace location/product/booking/subscription surfaces are the foundation.
- Owner-specific custom-subdomain marketplace behavior remains unchanged.
- Cleanup starts with a route/workflow responsibility inventory.
- Unsupported or removed paths resolve in place; URL redirects are out of scope.
- GraphQL/Relay/generated-code discipline applies if contract changes are required.
- Web component tests are primary; backend/integration tests are added only for changed domain contracts.

## Phase 1: Design Output

Completed artifacts:

- [data-model.md](data-model.md) — entities, relationships, validation rules, and state transitions.
- [contracts/capability-inventory.md](contracts/capability-inventory.md) — inventory record and approval contract.
- [contracts/aggregate-marketplace.md](contracts/aggregate-marketplace.md) — customer-facing aggregate marketplace behavior.
- [contracts/graphql-relay.md](contracts/graphql-relay.md) — GraphQL/Relay boundaries and generation rules.
- [contracts/route-map.md](contracts/route-map.md) — planned route ownership categories.
- [contracts/test-contract.md](contracts/test-contract.md) — required verification and regression coverage.
- [contracts/observability.md](contracts/observability.md) — structured logging requirements.
- [quickstart.md](quickstart.md) — validation flow and commands.

## Complexity Tracking

No constitution violations or justified complexity exceptions are required at plan time.

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --------- | ---------- | ------------------------------------ |
| None      | N/A        | N/A                                  |
