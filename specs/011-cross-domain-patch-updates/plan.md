# Implementation Plan: Cross-Domain Patch Updates

**Branch**: `011-cross-domain-patch-updates` | **Date**: 2026-05-21 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/011-cross-domain-patch-updates/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Roll the organisation field-masked update pattern through every remaining owned update surface for editable domain
state. Existing booking, customer, location, marketplace, and team GraphQL mutations plus booking, customer, location,
and team gRPC update contracts keep one normal `Update*` contract per surface while adding explicit allowlisted field
selection and selected-field-only application. Public web edit screens migrate to autosave by coherent edit unit,
remove redundant update buttons for autosaved values, preserve grouped validation boundaries such as booking details or
product editors, and keep returned details and structured logs useful for reconciliation and operations.

## Technical Context

**Language/Version**: C# .NET 10 for booking/customer/location/marketplace/team API and domain services; HotChocolate/Fusion GraphQL schema generation; TypeScript 6 / React 19 / Next.js 16 / Relay 20 for web consumers  
**Primary Dependencies**: Existing GraphQL mutation roots and typed inputs, gRPC protobuf contracts under `api-definitions/grpc/skedular`, domain repositories/models/services, generated GraphQL schemas and Relay artifacts, structured logging through existing Microsoft/Serilog conventions  
**Storage**: PostgreSQL via EF Core in the owning domains; no migration is planned because the rollout changes update semantics for existing editable state rather than adding new persisted business fields  
**Testing**: xUnit unit tests per API/service/mapper branch; domain integration tests for GraphQL and gRPC contracts with repository/query-layer persistence assertions; Vitest/React Testing Library for migrated edit screens across `webapp`, `webapp-teams`, and `webapp-spaces`
**Target Platform**: Skedular domain APIs, internal gRPC clients, federated GraphQL gateway/schema consumers, and the three Skedular web apps  
**Project Type**: Cross-domain backend GraphQL/gRPC contract and service migration with frontend autosave work on existing web edit surfaces  
**Performance Goals**: Field or grouped-edit autosaves stay within the existing update latency envelope and do not require a client-side full-object fetch before submission  
**Constraints**: Preserve omitted values, distinguish omitted from explicit clear/default values, require explicit allowlisted field selection, reload latest state and retry selected fields after detected concurrency conflicts, accept valid no-op updates, keep one public `Update*` contract per migrated surface, and regenerate generated contract surfaces rather than hand-editing them  
**Scale/Scope**: Remaining owned editable update surfaces after organisation. Current inventory identifies booking booking/recurring-booking GraphQL plus private-booking gRPC, customer details/billing GraphQL plus admin identity gRPC, location location/opening-hours/physical-address/restricted-information/floor-plan/resource GraphQL plus location/resource gRPC, marketplace product GraphQL, and team/team-member GraphQL plus team gRPC. Slack currently consumes several affected gRPC contracts; core and Microsoft Teams require re-checking for affected owning update surfaces or consumers during implementation.

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — This feature changes gRPC inputs in `api-definitions/grpc/skedular` and backend
      GraphQL mutation schemas. Build-time gRPC generation follows the edited protobuf definitions. Backend GraphQL
      schemas must be regenerated with `scripts/generate-graphql.sh`, and changed Relay callers must be regenerated
      through the web generation flow instead of hand-editing generated artifacts.
- [x] **II. Domain Boundaries** — Each owning domain applies patch semantics inside its own API/service/repository
      layer. Cross-domain consumers such as Slack update only public gRPC inputs and never read another domain's data
      store or internal classes directly.
- [x] **III. Testing** — Unit tests are required for field selection, allowlists, no-op handling, selected-field
      retry, validation, authorisation, and logging. GraphQL and gRPC integration tests are required for mutated
      public surfaces and must assert persistence through repository/query-layer APIs rather than raw `DbContext`.
- [x] **IV. Frontend** — Web UI changes are in scope for edit surfaces in the three apps. Relay operations remain
      collocated, generated artifacts are regenerated, typography comes from `@skedular/ui`, shared runtime helpers
      come from `@skedular/shared`, and any new user-facing copy uses British spelling.
- [x] **V. Pattern Consistency** — This extends the completed organisation partial-update pattern rather than
      introducing a parallel contract family. Explicit field selection, one `Update*` contract per migrated surface,
      selected-field-only application, and coherent autosave units are the reuse rules.
- [x] **VI. Logging** — Structured logs are planned for patch start/completion, selected field or edit-unit
      decisions, no-op branches, validation and authorisation rejection, concurrency retry, changed gRPC integration
      boundaries, and persistence failures without sensitive payload leakage.

## Project Structure

### Documentation (this feature)

```text
specs/011-cross-domain-patch-updates/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── cross-domain-patch-contracts.md
└── tasks.md
```

### Source Code (repository root)

```text
api-definitions/grpc/skedular/
├── booking/
├── customer/
├── location/
└── team/

booking/
├── apis/Booking.Api/{GraphQL,Grpc,Mappers,Models,Services}
├── apis/Booking.Api.UnitTests/
└── domain/Booking.Domain.IntegrationTests/

customer/
├── apis/Customer.Api/{GraphQL,Grpc,Mappers,Models,Services}
├── apis/Customer.Api.UnitTests/
└── domain/Customer.Domain.IntegrationTests/

location/
├── apis/Location.Api/{GraphQL,Grpc,Mappers,Models,Services}
├── apis/Location.Api.UnitTests/
└── domain/Location.Domain.IntegrationTests/

marketplace/
├── apis/Marketplace.Api/{GraphQL,Mappers,Models,Services}
├── apis/Marketplace.Api.UnitTests/
└── domain/Marketplace.Domain.IntegrationTests/

team/
├── apis/Team.Api/{GraphQL,Grpc,Mappers,Models,Services}
├── apis/Team.Api.UnitTests/
└── domain/Team.Domain.IntegrationTests/

slack/shared/Slack.Shared/Services/CrossDomains/

web/apps/
├── webapp/src/components/
├── webapp-teams/src/components/
└── webapp-spaces/src/components/
```

**Structure Decision**: Change typed update contracts and services in each owning domain, using the organisation API
patch models and mappers as the local precedent rather than creating a shared cross-domain patch engine. Update gRPC
protobufs where internal update RPCs exist, then migrate public gRPC consumers such as Slack. Update web edit screens
in place for affected booking, customer, location, marketplace product, resource, and team flows, grouping autosaves
where the current editor validates related values together.

## Complexity Tracking

No constitution violations requiring justification. The plan deliberately repeats the organisation field-mask pattern
inside the owning domains so each domain preserves its validation, permissions, events, caches, and workflow rules.

## Phase 0: Research

See [research.md](./research.md). Code inventory and clarified spec decisions resolve the contract, surface, autosave,
concurrency, generation, and verification questions needed for design.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/cross-domain-patch-contracts.md](./contracts/cross-domain-patch-contracts.md),
and [quickstart.md](./quickstart.md).

## Post-Design Constitution Check

- [x] **I. Contract-First** — The contract artifact separates GraphQL and gRPC update families and names the required
      GraphQL and Relay regeneration steps for changed generated surfaces.
- [x] **II. Domain Boundaries** — Design keeps patch application in booking/customer/location/marketplace/team owners
      and updates internal consumers through gRPC contracts only.
- [x] **III. Testing** — Data-model state transitions and quickstart verification include unit, GraphQL integration,
      gRPC integration, repository-layer persistence assertions, and web UI tests.
- [x] **IV. Frontend** — Design uses coherent edit units for autosave, preserves explicit non-save actions, and keeps
      frontend work inside existing web app components with generated Relay discipline.
- [x] **V. Pattern Consistency** — Research confirms the organisation implementation is the baseline and rejects
      implicit nullable inference or parallel patch contract families.
- [x] **VI. Logging** — Research, data model, contract expectations, and quickstart all name the required structured
      logging branches for migrated patch flows.
