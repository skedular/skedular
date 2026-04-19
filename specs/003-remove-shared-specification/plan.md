# Implementation Plan: Remove Shared Specification

**Branch**: `003-remove-shared-specification` | **Date**: 2026-04-19 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/003-remove-shared-specification/spec.md`

## Summary

Replace active `Enterprise.Shared` specification-based query composition with explicit repository methods owned by the data-owning domain shared repositories, migrate consumers domain by domain, preserve existing query behaviour, and retire the shared specification/evaluator path once no active production callers remain. The implementation will extend existing repository interfaces and repository extension helpers in organization, location, team, and related domains rather than introducing any new generic query abstraction.

## Technical Context

**Language/Version**: C# on .NET 10  
**Primary Dependencies**: Entity Framework Core, `Enterprise.Shared.Database` repository bases, domain shared repository factories, HotChocolate pagination helpers, `Microsoft.Extensions.Logging`  
**Storage**: PostgreSQL via EF Core domain DbContexts  
**Testing**: xUnit-based unit tests plus domain integration tests using repository-layer assertions  
**Target Platform**: Linux/macOS-hosted backend services, jobs, and processors on .NET 10
**Project Type**: Backend monorepo refactor across domain APIs, shared libraries, jobs, processors, and repository layers  
**Performance Goals**: Preserve current query performance characteristics and avoid additional database round trips or broader eager-loading than existing behaviour  
**Constraints**: No direct `DbContext` assertions in integration tests; no cross-domain direct DB ownership violations; no new generic query abstraction; preserve filters, includes, ordering, grouping, paging, and soft-delete semantics; keep logs secret-safe  
**Scale/Scope**: Active production migration spans at least 16 inline specification usages across organization, location/team-related, booking, marketplace, customer, and Slack consumers, followed by Enterprise.Shared database-layer cleanup

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — No `api-definitions/` or generated surfaces are intended to change. This is an internal repository/refactor feature.
- [x] **II. Domain Boundaries** — The feature crosses consumer domains, but query ownership will stay with the data-owning domain repositories. No new direct cross-domain DB access is planned.
- [x] **III. Testing** — Unit tests are required for new repository-method behaviour where practical, and integration tests are required for persistence-sensitive services/workflows. Integration assertions will use repository methods, not raw `DbContext`.
- [x] **IV. Frontend** — No web frontend changes are in scope.
- [x] **V. Pattern Consistency** — No new pattern is being introduced; this feature standardises on the repository-owned query pattern already used in domain shared layers.
- [x] **VI. Logging** — This is behaviour-preserving refactor work. Existing workflow logging must remain intact, and any service/job/processor branches that change due to repository-method adoption must keep structured warning/error logging and correlation context at the service boundary rather than adding noisy repository-level logs.

## Project Structure

### Documentation (this feature)

```text
specs/003-remove-shared-specification/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/
│   └── repository-ownership-contract.md
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
shared/
├── Enterprise.Shared/
│   └── Database/
│       ├── IRepository.cs
│       ├── Specification.cs
│       ├── SpecificationEvaluator.cs
│       ├── PostgreSql/RepositoryBase.cs
│       └── SqlServer/RepositoryBase.cs
├── Enterprise.Shared.UnitTests/
└── Testing.Shared/

organization/
├── apis/Organization.Api/Services/
└── shared/Organization.Shared/Repositories/

location/
├── apis/Location.Api/Services/
├── shared/Location.Shared/Repositories/
└── shared/Location.Shared.UnitTests/

team/
├── shared/Team.Shared/Repositories/
└── shared/Team.Shared.UnitTests/

booking/
├── apis/Booking.Api/Services/
└── shared/Booking.Shared/

marketplace/
└── apis/Marketplace.Api/Services/

slack/
├── apis/Slack.Api/Handlers/
├── apis/Slack.Api/Pages/
└── jobs/Slack.Jobs/Jobs/

customer/
└── processors/Customer.Processors/
```

**Structure Decision**: This feature spans the existing backend monorepo layout. Query ownership changes will be implemented in the domain shared repository directories for organization, location, and team, with consumer updates in booking, marketplace, Slack, customer, and organization/location API layers. Shared cleanup will be limited to `shared/Enterprise.Shared/Database` after consumer migration is complete.

## Post-Design Constitution Check

- [x] **I. Contract-First** — Still passes; design changes remain internal and avoid generated/API contract work.
- [x] **II. Domain Boundaries** — Still passes; repository-ownership contract keeps query definitions in data-owning domains.
- [x] **III. Testing** — Still passes; design explicitly requires repository-focused unit tests and repository-based integration assertions.
- [x] **IV. Frontend** — Not applicable.
- [x] **V. Pattern Consistency** — Still passes; design formalises the repo's existing domain repository pattern rather than inventing a new one.
- [x] **VI. Logging** — Still passes; design scope records that logging remains at workflow/service boundaries, with targeted updates only if refactor paths alter failure or branch behaviour.

## Complexity Tracking

No constitution violations identified.
