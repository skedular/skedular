<!--
SYNC IMPACT REPORT
==================
Version change: (none) → 1.0.0  (initial ratification)

Modified principles: N/A — first version

Added sections:
  - Core Principles (I–V)
  - Stack Reference
  - Governance

Removed sections: N/A

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md  — Constitution Check section present; gates below now apply
  ✅ .specify/templates/spec-template.md  — Requirements and testing sections align with Principle III
  ✅ .specify/templates/tasks-template.md — Task phases and test-optionality wording align with Principle III
  ✅ .specify/templates/constitution-template.md — source template; no update required

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

Each domain (booking, organisation, location, marketplace, etc.) owns its own data, services,
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

### III. Proportionate and Correct Testing

Every backend change MUST include unit tests. Changes that cross persistence or integration
boundaries (database, Kafka, Temporal, external HTTP) MUST also include integration tests.
System/end-to-end tests are reserved for true cross-domain or real-infrastructure scenarios.
Integration tests MUST NOT access `DbContext` or Entity Framework directly; all persistence
assertions MUST go through repository or query-layer methods. Web UI changes MUST use Vitest
and React Testing Library. Tests MUST be scenario-driven and assert observable behaviour, not
internal implementation details.

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
Feature and page components MUST use the project typography wrappers exported from
`@/components/commons` rather than importing MUI `Typography` directly. All user-facing and
operator-facing copy MUST use British spelling and grammar; technical identifiers (API fields,
routes, schema names) are exempt.

**Rationale**: Colocation and generated-artefact discipline keep the web layer coherent with the
backend schema. Centralised typography wrappers enforce visual consistency without per-component
overrides.

**Review gate**: PRs that hand-edit Relay artefacts, import MUI `Typography` directly in
feature components, or introduce American-English copy in user-facing strings MUST be corrected
before merging.

### V. Change Safety and Pattern Consistency

Changes that affect contracts, schemas, event definitions, GraphQL types, or generated surfaces
MUST explicitly account for regeneration and downstream impact before implementation begins.
New work MUST favour consistency with existing patterns over introducing parallel abstractions.
Any deviation from established patterns — new frameworks, alternative persistence approaches,
alternative event serialisation — requires explicit justification documented in the relevant
plan or ADR. Exceptions to any principle in this constitution MUST be rare, explicit, and
justified; undocumented exceptions are violations.

**Rationale**: A large monorepo with many domains becomes unmaintainable when each domain
independently reinvents patterns. Explicit justification for exceptions keeps the architecture
legible and deviations visible to reviewers.

**Review gate**: Plan and task artefacts for any feature that introduces a new shared pattern or
deviates from an existing one MUST include a brief justification note before tasks are accepted.

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

**Non-compliance**: A violation identified during review blocks merge until resolved or until an
explicit, documented exception is agreed and committed alongside the change.

---

**Version**: 1.0.0 | **Ratified**: 2026-04-14 | **Last Amended**: 2026-04-14
