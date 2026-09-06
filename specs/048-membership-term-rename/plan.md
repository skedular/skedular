# Implementation Plan: MembershipTerm Terminology Rename

**Branch**: `048-membership-term-rename` | **Date**: 2026-09-03 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/048-membership-term-rename/spec.md`

## Summary

Rename the active contractual-period concept from `PurchaseCadence` to `MembershipTerm` across domain models, services, persistence, contracts, generated artifacts, applications, tests, and documentation. Add a forward EF migration that renames existing storage columns without changing values.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C#/.NET 10; TypeScript 6; React 19; Next.js 16
**Primary Dependencies**: EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, OpenAPI, protobuf/Kafka events, Temporal, Relay 21, pnpm workspace tooling
**Storage**: EF Core/PostgreSQL product and offering persistence, including forward column and serialized-property migration
**Testing**: .NET unit/integration/system tests; Vitest/React Testing Library; contract-generation and absence checks
**Target Platform**: Skedular backend services and all supported web applications
**Project Type**: Multi-domain backend and multi-app web monorepository
**Performance Goals**: Preserve existing behavior and performance
**Constraints**: Source definitions precede regeneration; persistence migration is forward-only; no direct transport-to-repository access; American English copy
**Scale/Scope**: Repository-wide across backend, contracts, generated outputs, web apps, tests, fixtures, logs, and documentation

**Persistence Migration Boundary**: This changeset renames persisted `PurchaseCadence`/`purchaseCadence` properties and database columns to `MembershipTerm`/`membershipTerm`. The EF migration is forward-only and preserves stored values; no compatibility alias or dual-write path is retained.

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — Update authoritative definitions first and regenerate event, GraphQL, OpenAPI-client, and Relay outputs; never hand-edit generated output.
      If yes, confirm the correct generator script is identified and will be run.
- [x] **II. Domain Boundaries** — Cross-domain updates use existing service/event interfaces and explicit mappings; no direct database access or reflection-based enum parsing is introduced.
      If yes, confirm the cross-domain path uses a public service or event interface, not direct DB/internal access.
      For persisted enum-like values, confirm source strings use explicit switch-based mappings to model enums;
      direct `Enum.Parse`/`Enum.TryParse` mapping is not permitted.
- [x] **III. Testing** — Unit tests cover mappings and behavior preservation first; integration/system tests cover persistence compatibility and contract boundaries; frontend tests cover affected apps.
      Confirm unit tests are planned first. Add integration tests only for persistence, database-concurrency,
      migration, schema-wiring, or external-infrastructure behavior that unit tests cannot prove; do not duplicate
      service scenarios already covered by unit tests.
- [x] **IV. Frontend** — Update all web apps and public documentation, regenerate Relay/API artifacts, use typography wrappers, preserve American English, and retain Relay mutation state updates without browser reloads.
      If yes, confirm Relay colocation, no hand-edited generated artifacts, typography wrappers used,
      American spelling in user-facing copy, and review/update of corresponding public-web documentation
      for any customer-facing or operator-facing behavior changes. Record why existing documentation remains
      accurate when no documentation update is needed. For every mutation, document the Relay store-update
      strategy: return the rendered fields and stable ID in the payload; use a declarative connection update or
      targeted refetch for affected lists/counts; never use `window.location.reload()` after mutation success.
- [x] **V. Pattern Consistency** — No new pattern; follow existing source-definition generation and persistence-boundary mapping patterns while deferring storage migration.
      If yes, a brief justification MUST be documented here before tasks are accepted.
- [x] **VI. Logging** — Rename active structured log terminology and verify workflow, decision, integration, failure, and compatibility diagnostics retain correlation context and avoid sensitive data.
      If yes, confirm structured logging scope is explicitly planned for core workflows,
      state transitions, integration boundaries, and failure paths.

## Project Structure

### Documentation (this feature)

```text
specs/048-membership-term-rename/
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
api-definitions/{events,graphql,openapi}/
src/{marketplace,booking,shared,system,web}/
specs/048-membership-term-rename/{research.md,data-model.md,contracts/,quickstart.md}
```

**Structure Decision**: Use the existing multi-domain backend and multi-app web structure. Contract definitions remain under `api-definitions/`; shared models and mappings remain in owning projects; generated outputs are regenerated in place; design artifacts remain under `specs/048-membership-term-rename/`.
