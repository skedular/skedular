# Implementation Plan: Product Price Available Days

**Branch**: `034-price-available-days` | **Date**: 2026-07-18 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/034-price-available-days/spec.md`

## Summary

Add an optional, price-level `availableDays` collection that accepts any of the seven calendar days. An empty collection remains backward compatible and means every day. Carry this rule through the shared price contract, Marketplace product-version projection, Booking and Location projections, Host and Spaces editors, and customer-facing purchase views. Enforce it server-side against the booking location's local start date before resource checks; filter recurring instance generation before opening-hours and resource allocation; retain the purchased rule for an active subscription period and reload the current price rule only on renewal.

## Technical Context

**Language/Version**: C#/.NET 10; TypeScript 6, React 19, Next.js 16  
**Primary Dependencies**: HotChocolate/Fusion GraphQL, EF Core/PostgreSQL, Kafka protobuf events, Temporal, Relay, MUI, `@skedular/ui`  
**Storage**: Existing Marketplace, Booking, and Location PostgreSQL JSONB product-version/pricing projections  
**Testing**: xUnit unit and integration tests; Vitest and React Testing Library; generated-contract validation  
**Target Platform**: Backend services, Skedular Host, Skedular Spaces, public Astro documentation site  
**Project Type**: Distributed web application and documentation site  
**Performance Goals**: Add constant-time day membership checks without increasing booking-generation scan scope; preserve current daily workflow cadence  
**Constraints**: Empty selection means every calendar day; all seven days are equal; local location calendar is authoritative; no generated artifact may be hand-edited  
**Scale/Scope**: One additive ProductPricing field propagated to Marketplace, Booking, Location, Host, Spaces, customer purchase views, generated contracts, and public documentation

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — ProductPricing is exposed through GraphQL and replicated by Marketplace protobuf events. Update `api-definitions/events/skedular/marketplace_v1_value.proto`, run `api-definitions/events/generate.sh`, then run `scripts/generate-graphql.sh` and `src/web/apps/webapp/scripts/generate.sh`; commit resulting schemas and Relay artifacts.
- [x] **II. Domain Boundaries** — Marketplace owns price definition and publishes it through its existing event; Booking and Location consume their own product-version projections. Booking uses its public projection/service boundaries, not another domain's database.
- [x] **III. Testing** — Add unit tests for price validation, day eligibility, direct booking, subscription generation and renewal; add Marketplace/Booking integration tests with repository-based assertions; add Host/Spaces web tests for editor mapping and customer display.
- [x] **IV. Frontend** — Add Relay selections next to consuming components, regenerate Relay artifacts, use `@skedular/ui` typography and American English, and keep the Host and Spaces editor paths consistent.
- [x] **V. Pattern Consistency** — Reuse the existing `DayOfWeekConstants` (`MON`–`SUN`), JSONB pricing collections, ProductPricing replacement patch, and price snapshot/renewal behavior. No parallel recurrence model is introduced.
- [x] **VI. Logging** — Add structured, correlation-aware logs for available-day rejection and recurring-candidate skips, and preserve existing workflow start/completion and failure logs.

## Project Structure

### Documentation (this feature)

```text
specs/034-price-available-days/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
api-definitions/events/skedular/marketplace_v1_value.proto
src/shared/Api.Shared.Services/Models/ProductPricing.cs
src/marketplace/{apis,shared,processors}/
src/booking/{apis,shared,processors,domain}/
src/location/{shared,processors}/
src/web/apps/{webapp-host,webapp-spaces,webapp}/src/
src/web/apps/public-web/src/content/docs/
```

**Structure Decision**: Extend the existing shared ProductPricing contract; Marketplace remains the definition owner, Booking owns eligibility and subscription generation, Location consumes the replicated catalog, Host owns price administration, Spaces/customer surfaces consume the published price, and the public website documents behavior.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --- | --- | --- |
| None | Existing domain and contract patterns cover the feature. | N/A |
