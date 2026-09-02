# Implementation Plan: Marketplace Pricing Cadence Simplification

**Branch**: `047-marketplace-pricing-cadence` | **Date**: 2026-08-31 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/047-marketplace-pricing-cadence/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Remove `BookingCadence` and all sub-day/one-time `ProductPricingCadence` values from the shared pricing model and every persisted, serialized, generated, projected, workflow, API, frontend, and test surface. Retain `PurchaseCadence` as the offer term, use auto-renewal to determine repetition, validate booking duration solely from the selected date-time interval against min/max limits, and represent credit entitlements as cadence-free. Existing production data requires no legacy conversion because no production pricing uses removed terms.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# .NET 10; TypeScript 6.0.3; React 19.2.6; Next.js 16.2.6
**Primary Dependencies**: HotChocolate/Fusion GraphQL, EF Core/PostgreSQL, Kafka/protobuf event contracts, Temporal workflows, Relay 21, React Testing Library/Vitest
**Storage**: Marketplace-owned PostgreSQL product/product-version persistence; replicated pricing JSON/event state in consuming domains
**Testing**: xUnit with AutoFixture/FakeItEasy; repository-layer integration tests; Vitest and React Testing Library; generated schema/client validation
**Target Platform**: Cloud-hosted ASP.NET Core APIs and Next.js web applications
**Project Type**: Full-stack marketplace pricing and booking platform
**Performance Goals**: Preserve existing pricing, booking, renewal, and billing response/workflow performance; no new network round trips required by the model change
**Constraints**: Contract-first generation; no direct EF from transport/application layers; explicit enum mappings; `CancellationToken` last; no `window.location.reload()` after successful Relay mutations; American English; production has no removed-term data
**Scale/Scope**: All marketplace pricing options, product versions, subscriptions, entitlements, booking consumers, billing projections, APIs, web apps, generated artifacts, and affected tests

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — Yes. Update `api-definitions/events/skedular/marketplace_v1_value.proto` and GraphQL source schemas, then run `api-definitions/events/generate.sh`, `scripts/generate-graphql.sh`, and `src/web/apps/webapp/scripts/generate.sh`/Relay generation as applicable. Generated schemas and clients will not be hand-edited.
- [x] **II. Domain Boundaries** — Yes. Marketplace owns product pricing and its event projection; Booking owns booking duration, subscription, billing, and resource-booking behavior. Cross-domain propagation remains through shared models/events and service boundaries. Persisted cadence strings use explicit switch mappings; no reflection parsing.
      If yes, confirm the cross-domain path uses a public service or event interface, not direct DB/internal access.
      For persisted enum-like values, confirm source strings use explicit switch-based mappings to model enums;
      direct `Enum.Parse`/`Enum.TryParse` mapping is not permitted.
- [x] **III. Testing** — Unit tests cover enum/mapping rules, duration validation, renewal branching, entitlement exclusion, billing slices, and workflow decisions. Integration tests cover persistence/event/schema wiring and end-to-end subscription/entitlement boundaries. Frontend tests cover editor choices and date-time duration behavior; generated artifacts are validated after regeneration.
- [x] **IV. Frontend** — Yes. Update marketplace/customer, host, and spaces consumers with collocated Relay operations and typography wrappers. Regenerate Relay artifacts. No successful mutation reloads the browser; mutation payloads/refetches must update affected records and connections. Review affected customer/operator documentation and update it if behavior is documented.
      If yes, confirm Relay colocation, no hand-edited generated artifacts, typography wrappers used,
      American spelling in user-facing copy, and review/update of corresponding public-web documentation
      for any customer-facing or operator-facing behavior changes. Record why existing documentation remains
      accurate when no documentation update is needed. For every mutation, document the Relay store-update
      strategy: return the rendered fields and stable ID in the payload; use a declarative connection update or
      targeted refetch for affected lists/counts; never use `window.location.reload()` after mutation success.
- [x] **V. Pattern Consistency** — No new architectural pattern. The change consolidates existing pricing-term semantics into `PurchaseCadence`, reuses existing billing-cycle slicing, existing duration bounds, existing entitlement lifecycle, and existing generated-contract workflows.
- [x] **VI. Logging** — Plan structured logs for pricing validation, renewal/no-renewal decisions, duration acceptance/rejection, legacy/unknown cadence failures, entitlement exclusion, and migration/recovery outcomes with correlation context and no sensitive data.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
api-definitions/
├── events/skedular/marketplace_v1_value.proto
└── graphql/skedular/v1/schema.graphql        # regenerated
src/shared/Api.Shared.Services/Models/
├── ProductPricing.cs
└── ProductPricingCadence.cs
src/marketplace/
├── shared/Marketplace.Shared/Mappers/EventMapper.cs
├── shared/Marketplace.Shared/Database/       # pricing persistence/migrations
└── apis/Marketplace.Api/                     # GraphQL choices and product service
src/booking/
├── shared/Booking.Shared/                    # duration, subscription, invoice/billing rules
├── processors/Booking.Processors/Mappers/    # event projection
└── domain/*IntegrationTests/                 # persistence/workflow/schema boundaries
src/web/apps/
├── webapp/                                   # customer marketplace booking/entitlement flow
├── webapp-host/                              # host pricing editor
└── webapp-spaces/                            # spaces pricing editor
```
```

**Structure Decision**: Extend the existing shared-model → marketplace contract/event → booking projection/workflow → web Relay structure. Keep ownership in Marketplace for product pricing and in Booking for booking, subscription, invoice, and resource behavior; regenerate all derived contract and Relay surfaces from their sources.

## Complexity Tracking

No constitution violations requiring justification.
