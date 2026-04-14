# Implementation Plan: Team Domain Structured Logging

**Branch**: `001-team-domain-logging` | **Date**: 2026-04-14 | **Spec**: `/specs/001-team-domain-logging/spec.md`
**Input**: Feature specification from `/specs/001-team-domain-logging/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Introduce structured, secret-safe `ILogger<T>` coverage across team-domain service,
authorisation, subscriber, cache, publisher, activity, temporal-outbox, and gRPC components,
aligned to the existing Enterprise.Shared Serilog pipeline and logging safety rules.
Implementation keeps existing architecture boundaries intact and focuses on predictable,
observable behaviour with proportionate unit and integration test updates.

## Technical Context

**Language/Version**: C# on .NET 10  
**Primary Dependencies**: `Microsoft.Extensions.Logging`, Enterprise.Shared logging/hosting extensions,
Serilog pipeline via `UseSerilogCustom`, HotChocolate (existing GraphQL stack), Temporal client
(existing usage), Kafka outbox/event subscribers (existing usage)  
**Storage**: PostgreSQL via Entity Framework Core (existing Team shared repositories)  
**Testing**: xUnit + AutoFixture + FakeItEasy (`[AutoFakeItEasyData]` pattern), existing Team
unit/integration test projects  
**Target Platform**: Linux containerised services hosted by ASP.NET Core/.NET Generic Host
(Team.Api, Team.Jobs, Team.Processors)  
**Project Type**: Multi-project backend domain in monorepo (`team/apis`, `team/shared`,
`team/processors`, `team/jobs`)  
**Performance Goals**: No material throughput regression; logging overhead remains bounded by
using `LogDebug` for high-frequency cache diagnostics and structured templates for efficient
serialization  
**Constraints**: No secret/credential/PII values in structured log properties; no changes to
contract surfaces or generated outputs; preserve existing subscriber behaviour for
`LocationSubscriber` and `OrganizationSubscriber`  
**Scale/Scope**: Team domain service-layer and related shared/processors components only;
approximately 15-25 production classes plus dependent test files

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — No `api-definitions/` or generated contract surfaces are changed.
      No regeneration required.
- [x] **II. Domain Boundaries** — Scope is contained to Team domain components and existing shared
      Team libraries. No cross-domain DB or internal-class bypass.
- [x] **III. Testing** — Unit tests required for updated services/components. Integration tests
      only where behaviour crosses persistence/event boundaries. No direct `DbContext` assertions
      in integration tests.
- [x] **IV. Frontend** — No frontend scope.
- [x] **V. Pattern Consistency** — Reuses existing primary-constructor `ILogger<T>` injection
      pattern and existing Enterprise.Shared Serilog pipeline. No new pattern introduced.

### Post-Design Constitution Re-check

- [x] **I. Contract-First** — Design artefacts keep all changes implementation-local; no API/
      schema/generation impact identified.
- [x] **II. Domain Boundaries** — Data model and contracts are component-observability contracts
      inside Team domain only.
- [x] **III. Testing** — Quickstart includes required unit/integration validation commands and
      logger-safe assertions.
- [x] **IV. Frontend** — Not applicable; unchanged.
- [x] **V. Pattern Consistency** — Contract and data model explicitly codify existing patterns
      (`ILogger<T>`, structured templates, safe-log property set).

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

```text
team/
├── apis/
│   ├── Team.Api/
│   │   ├── Services/
│   │   ├── Services/Authorization/
│   │   └── Grpc/
│   ├── Team.Api.UnitTests/
│   └── Team.Api.IntegrationTests/
├── processors/
│   ├── Team.Processors/Subscribers/
│   ├── Team.Processors.UnitTests/
│   └── Team.Processors.IntegrationTests/
├── jobs/
│   ├── Team.Jobs/
│   ├── Team.Jobs.UnitTests/
│   └── Team.Jobs.IntegrationTests/
└── shared/
      ├── Team.Shared/
      │   ├── Services/
      │   ├── Services/Cache/
      │   ├── Activities/
      │   └── Publishers/
      ├── Team.Shared.UnitTests/
      └── Team.Infrastructure/
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

**Structure Decision**: Use the existing Team-domain multi-project structure and apply
logging updates in-place within current service/component boundaries rather than introducing
new projects or shared abstractions.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --------- | ---------- | ------------------------------------ |
| None      | N/A        | N/A                                  |
