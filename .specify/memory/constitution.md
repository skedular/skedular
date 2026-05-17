<!--
SYNC IMPACT REPORT
==================
Version change: 1.1.0 → 1.2.0

Modified principles:
  - IV. Frontend Consistency — updated typography import source and added @skedular/ui / @skedular/shared package model

Added sections: none
Removed sections: none

Templates reviewed and alignment status:
  ⚠ pending: .specify/templates/plan-template.md — IV review gate wording unchanged but now refers to @skedular/ui
  ⚠ pending: .specify/templates/spec-template.md — no direct reference to @/components/commons; no change required
  ⚠ pending: .specify/templates/tasks-template.md — no direct reference; no change required

Deferred TODOs: none
-->
<!--
SYNC IMPACT REPORT
==================
Version change: 1.0.0 → 1.1.0

Modified principles:
  - III. Proportionate and Correct Testing → III. Proportionate Testing and Logging Verification
  - V. Change Safety and Pattern Consistency (expanded for observability checks)
  - Added: VI. Mandatory Feature Logging and Observability

Added sections:
  - Logging and Observability Expectations (under Governance)

Removed sections: none

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md  — added mandatory logging constitution gate
  ✅ .specify/templates/spec-template.md  — added mandatory observability/logging requirement section
  ✅ .specify/templates/tasks-template.md — added required logging tasks in foundational and per-story phases
  ✅ README.md — reviewed; no direct constitution-reference text required changes
  ⚠ pending: .specify/templates/commands/*.md (directory not present in this repository)

Deferred TODOs: none
-->

# Skedular Constitution

## Core Principles

### I. Contract-First and Generated-Code Discipline

`api-definitions/` is the single source of truth for all OpenAPI, protobuf event, gRPC, and
GraphQL-related contract surfaces. Generated outputs MUST NOT be hand-edited. When any contract
changes, the correct generator script MUST be run before any downstream code change is made.
`make generate` is the preferred umbrella regeneration entry point. GraphQL schema files,
Relay web artefacts, and OpenAPI TypeScript clients MUST stay in sync with their backend
definitions; stale generated files are treated as broken builds.

**Rationale**: Hand-edits to generated outputs are silently overwritten and introduce drift that
is difficult to trace. Keeping `api-definitions/` as the authoritative surface makes contract
changes explicit, reviewable, and reproducible.

**Review gate**: Any PR that touches `api-definitions/` or a generated file MUST demonstrate
that the appropriate generator was run and its outputs are committed.

### II. Domain Ownership and Architecture Boundaries

Each domain (booking, organization, location, marketplace, etc.) owns its own data, services,
workflows, and Kafka event definitions. Cross-domain collaboration MUST go through public
service or event interfaces, never through direct database or internal-class access. GraphQL
federation is the primary client-facing integration surface; REST/OpenAPI is secondary; gRPC is
for internal inter-service communication only. Shared infrastructure and cross-cutting concerns
MUST live in `shared/` libraries rather than being duplicated per-domain. Workflow ID
construction MUST be centralised in a domain workflow-ID service, not scattered inline.

**Rationale**: Clear ownership boundaries keep the blast radius of change small, preserve
independent deployability, and prevent coupling that accumulates into architectural debt.

**Review gate**: New code that reads another domain's database, bypasses a service boundary, or
duplicates shared infrastructure logic requires explicit justification before merging.

### III. Proportionate Testing and Logging Verification

Every backend change MUST include unit tests. Changes that cross persistence or integration
boundaries (database, Kafka, Temporal, external HTTP) MUST also include integration tests.
System/end-to-end tests are reserved for true cross-domain or real-infrastructure scenarios.
Integration tests MUST NOT access `DbContext` or Entity Framework directly; all persistence
assertions MUST go through repository or query-layer methods. Web UI changes MUST use Vitest
and React Testing Library. Tests MUST be scenario-driven and assert observable behaviour, not
internal implementation details. Any new or changed critical workflow MUST include tests that
verify expected logging side effects at the appropriate boundary (for example, warning/error
paths and key lifecycle transitions).

**Rationale**: Proportionate testing keeps the suite fast and meaningful without under-testing
behaviour that is genuinely risky. Banning raw EF in integration tests keeps assertions
decoupled from persistence implementation choices.

**Review gate**: PRs that touch persistence, event, or workflow code without accompanying
integration tests, or that assert state through `DbContext` directly, MUST be returned for
revision.

### IV. Frontend Consistency

Web work MUST follow the Next.js App Router + Relay + generated-artefact model already
established in `web/apps/webapp`. Relay fragments MUST be collocated with the component that
consumes them. Generated Relay artefacts and OpenAPI TypeScript clients MUST NOT be hand-edited.

The Skedular web monorepo uses two centralised workspace packages that apply uniformly across
all three products (`webapp`, `webapp-teams`, `webapp-spaces`):

- **`@skedular/ui`** (`web/packages/ui`) — the Skedular design system. Owns all visual
  primitives: typography wrappers, layout building blocks, theme tokens, and colour palette.
  A change to `@skedular/ui` propagates to every product automatically. Feature and page
  components in any product MUST import typography wrappers from `@skedular/ui` (for example
  `import { BodyIconTypography } from '@skedular/ui'`) rather than importing MUI `Typography`
  directly. The only permitted exception is inside `web/packages/ui/src/typography/` itself,
  where MUI `Typography` is the low-level primitive being wrapped.

- **`@skedular/shared`** (`web/packages/shared`) — the centralised shared application layer.
  Owns all cross-product runtime modules: React providers, hooks, utilities (date, name, Relay
  error helpers), MUI helpers, cookie consent, and image upload. All three products MUST import
  these modules from `@skedular/shared` rather than maintaining per-product copies.
  `@skedular/shared` may import from `@skedular/ui`; the reverse is forbidden.

Product apps (`webapp`, `webapp-teams`, `webapp-spaces`) own only product-specific feature
components, route trees, per-product configuration (logger name, analytics tag IDs), and
product-specific Relay queries. Auth entry points (sign-in, callback, account settings,
notifications) remain in `webapp` and are shared entry points for all products.

All user-facing and operator-facing copy MUST use British spelling and grammar; technical
identifiers (API fields, routes, schema names) are exempt.

**Rationale**: Centralising the design system and shared runtime modules into packages means a
single change propagates to all products simultaneously, eliminating duplication and drift.
Clear package boundaries (`@skedular/ui` for visual primitives, `@skedular/shared` for runtime
infrastructure) make ownership decisions unambiguous and keep product apps focused on
product-specific work.

**Review gate**: PRs that hand-edit Relay artefacts, import MUI `Typography` directly in
feature or page components (outside `web/packages/ui/src/typography/`), import
typography wrappers from any path other than `@skedular/ui`, duplicate shared providers or
utilities inside a product app instead of using `@skedular/shared`, or introduce
American-English copy in user-facing strings MUST be corrected before merging.

### V. Change Safety and Pattern Consistency

Changes that affect contracts, schemas, event definitions, GraphQL types, or generated surfaces
MUST explicitly account for regeneration and downstream impact before implementation begins.
New work MUST favour consistency with existing patterns over introducing parallel abstractions.
Any deviation from established patterns — new frameworks, alternative persistence approaches,
alternative event serialisation — requires explicit justification documented in the relevant
plan or ADR. Exceptions to any principle in this constitution MUST be rare, explicit, and
justified; undocumented exceptions are violations. Any feature design that omits operational
logging for its core flows is treated as a pattern violation unless an explicit exception is
approved and documented.

**Rationale**: A large monorepo with many domains becomes unmaintainable when each domain
independently reinvents patterns. Explicit justification for exceptions keeps the architecture
legible and deviations visible to reviewers.

**Review gate**: Plan and task artefacts for any feature that introduces a new shared pattern or
deviates from an existing one MUST include a brief justification note before tasks are accepted.

### VI. Mandatory Feature Logging and Observability

Every feature MUST include structured logging as a first-class deliverable. "Feature complete"
means business behaviour, tests, and logs are all present. Logging MUST cover:

- start and completion of core feature workflows
- meaningful state transitions and branch decisions
- integration boundaries (external APIs, Kafka, Temporal, databases) with correlation context
- failure and recovery paths with actionable detail for operators

Logs MUST use consistent structured properties, avoid sensitive payload leakage, and follow
existing domain logging conventions in `shared/` and domain-specific libraries. Silent features
without operationally useful logs are non-compliant.

**Rationale**: Reliable operations and incident response depend on reconstructing feature
behaviour from logs without source-level debugging in production.

**Review gate**: A feature PR MUST identify where logging was added or updated, and reviewers
MUST reject changes that add feature behaviour without corresponding structured logs.

## Stack Reference

The following stack choices meaningfully constrain architecture and implementation decisions.
These are not aspirational — they describe the current, enforced state:

- **Backend**: C# on .NET 10; domain services follow the microservice layout under each domain directory.
- **API surfaces**: Federated GraphQL (primary client-facing), REST/OpenAPI (secondary), gRPC (internal only).
- **Events**: Kafka with protobuf-defined events; event classes generated into `shared/Api.Shared.Clients/obj`.
- **Workflows**: Temporal for long-running, stateful processes; workflow IDs owned by domain ID services.
- **Persistence**: PostgreSQL via Entity Framework Core; migrations owned per domain.
- **Frontend**: Next.js App Router, React, Relay, TypeScript, MUI.
- **Local orchestration**: Aspire (for service graph and dependency readiness) and Docker Compose.

## Governance

**Amendment procedure**: Any change to this constitution MUST be proposed as a pull request to
`main` that updates this file. The proposer MUST document the change rationale in the PR
description. MAJOR version bumps (principle removals or redefinitions) require reviewer
consensus. MINOR bumps (new sections or material expansions) require at least one additional
reviewer beyond the author. PATCH bumps (wording, clarifications) may be merged by the author
after self-review.

**Versioning policy** (semantic):

- `MAJOR` — backward-incompatible governance change: removing or fundamentally redefining a principle.
- `MINOR` — additive: new principle, section, or materially expanded guidance.
- `PATCH` — non-semantic: wording, typo fixes, formatting, clarifications.

**Compliance expectations**: The constitution applies to all feature branches and all
contributors. Spec, plan, and task artefacts produced by `/speckit.specify`, `/speckit.plan`,
and `/speckit.tasks` MUST reference the applicable review gates. Implementation PRs are expected
to have been checked against every applicable gate before review is requested.

**Logging and observability expectations**: For every feature, logging is mandatory rather than
optional polish. Planning artefacts MUST call out logging scope. Task breakdowns MUST include
explicit logging implementation work. Verification MUST include checks that logs are emitted for
successful and failure paths of the feature's primary workflows.

**Non-compliance**: A violation identified during review blocks merge until resolved or until an
explicit, documented exception is agreed and committed alongside the change.

---

**Version**: 1.2.0 | **Ratified**: 2026-04-14 | **Last Amended**: 2026-04-27
