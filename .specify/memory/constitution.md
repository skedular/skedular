<!--
SYNC IMPACT REPORT
==================
Version change: 2.5.0 → 2.6.0

Modified principles:
  - IV. Frontend Consistency — added Relay mutation state-synchronization requirements

Added sections: none
Removed sections: none

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md — requires the Relay store/connection update strategy
  ✅ AGENTS.md — provides implementation and regeneration guidance

Deferred TODOs: none
-->
<!--
SYNC IMPACT REPORT
==================
Version change: 2.5.0 → 2.6.0

Modified principles:
  - II. Domain Ownership and Architecture Boundaries — API and repository methods MUST place all required
    parameters before CancellationToken; CancellationToken MUST be the final parameter, and trailing optional
    parameters are prohibited unless the contract explicitly requires optionality
  - IV. Frontend Consistency — added a mandatory public-documentation review/update for
    customer-facing or operator-facing behavior changes
  - V. Change Safety and Pattern Consistency — local persisted-entity references MUST use
    explicit EF Core foreign keys, with nullability defining relationship requiredness

Added sections: none
Removed sections: none

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md — added a documentation review gate for web behavior changes
  ✅ .specify/templates/spec-template.md — no direct structural change required
  ✅ .specify/templates/tasks-template.md — existing documentation task remains compatible

Deferred TODOs: none
-->
<!--
SYNC IMPACT REPORT
==================
Version change: 2.2.0 → 2.3.0

Modified principles:
  - II. Domain Ownership and Architecture Boundaries — added a mandatory service/API model boundary
    requiring services to return domain models rather than GraphQL or other adapter types
  - II. Domain Ownership and Architecture Boundaries — persisted enum-like values require explicit
    switch-based source-to-model mappings; direct Enum.Parse/TryParse mapping is prohibited

Added sections:
  - Service/API Model Boundary

Removed sections: none

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md — existing Constitution Check remains compatible; service/API
    model boundary is enforced by the amended principle
  ✅ .specify/templates/spec-template.md — no direct structural change required
  ✅ .specify/templates/tasks-template.md — no direct structural change required

Deferred TODOs: none
-->
<!--
SYNC IMPACT REPORT
==================
Version change: 2.0.0 → 2.1.0

Modified principles:
  - III. Proportionate Testing and Logging Verification — made unit tests the default for
    isolated behavior and limited integration tests to boundaries that unit tests cannot prove

Added sections: none
Removed sections: none

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md — updated testing review gate
  ✅ .specify/templates/tasks-template.md — added unit-first integration-test guidance

Deferred TODOs: none
-->
<!--
SYNC IMPACT REPORT
==================
Version change: 1.2.0 → 2.0.0

Modified principles:
  - IV. Frontend Consistency — switched user-facing/operator-facing copy standard from British
    spelling and grammar to American spelling and grammar

Added sections: none
Removed sections: none

Templates reviewed and alignment status:
  ✅ .specify/templates/plan-template.md — updated IV review gate copy standard to American spelling
  ✅ .specify/templates/spec-template.md — no direct localization rule; no change required
  ✅ .specify/templates/tasks-template.md — no direct localization rule; no change required
  ✅ AGENTS.md — updated repository agent guidance to American spelling
  ✅ specs/019-app-switcher/* — updated active feature artifacts to American spelling

Deferred TODOs: none
-->
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
Relay web artifacts, and OpenAPI TypeScript clients MUST stay in sync with their backend
definitions; stale generated files are treated as broken builds.

**Rationale**: Hand-edits to generated outputs are silently overwritten and introduce drift that
is difficult to trace. Keeping `api-definitions/` as the authoritative surface makes contract
changes explicit, reviewable, and reproducible.

**Review gate**: Any PR that touches `api-definitions/` or a generated file MUST demonstrate
that the appropriate generator was run and its outputs are committed.

### II. Domain Ownership and Architecture Boundaries

#### Service/API Model Boundary

Application and domain service interfaces MUST return shared domain models owned by the service/domain layer. They MUST
NOT return GraphQL detail types, GraphQL inputs, GraphQL payloads, HotChocolate connection/edge types, or any other API
adapter type. If a service needs to expose new data, first create or extend a model in the owning shared model namespace,
then map that model to GraphQL, REST, gRPC, or another API representation at the API boundary. This keeps service
contracts reusable when a second API layer is added and prevents API-specific dependencies from leaking inward.

#### Transport-to-Service Persistence Boundary

GraphQL resolvers, REST controllers, gRPC transport handlers, and other API/transport-layer adapters MUST NOT inject
`IRepositoryFactory` or any repository implementation. Transport code may call application/domain services only and
may map service-owned models into transport payloads. Repository access belongs inside the owning service layer (or a
lower domain layer), and service methods exposed to transport MUST return models rather than database entities.

**Rationale**: Keeping repository dependencies out of transport prevents persistence concerns from leaking into API
contracts and ensures authorization, mapping, transactions, and business rules are applied consistently by services.

**Review gate**: Reject new or changed GraphQL/API transport methods that inject `IRepositoryFactory`, query a
repository directly, or return database entities. Move the query behind an injected service and add a service-level
model/read method when necessary.

Every newly introduced enum-like model value MUST keep its enum, persisted constants, and conversion/name extensions
co-located in the owning model file/group. GraphQL choice/detail types may wrap those models, but must not become the
source of truth. Persisted strings MUST be mapped to model enums through explicit switch-based conversion extensions
that list each supported source value. Direct `Enum.Parse`, `Enum.TryParse`, or equivalent reflection-based parsing
MUST NOT be used for source-to-model mapping; unknown values MUST follow the owning mapping's explicit fallback or
error policy.

#### API Parameter Ordering

Public and internal API, service, and repository methods MUST declare all required parameters before the
`CancellationToken`. The `CancellationToken` MUST be the final parameter in the signature and in every call. Do not
add trailing optional parameters after it; make such parameters required, move them before the token, or introduce an
explicit request/options model when optionality is part of the contract.

Each domain (booking, organization, location, marketplace, etc.) owns its own data, services,
workflows, and Kafka event definitions. Cross-domain collaboration MUST go through public
service or event interfaces, never through direct database or internal-class access. GraphQL
federation is the primary client-facing integration surface; REST/OpenAPI is secondary; gRPC is
for internal inter-service communication only. Shared infrastructure and cross-cutting concerns
MUST live in `shared/` libraries rather than being duplicated per-domain. Workflow ID
construction MUST be centralized in a domain workflow-ID service, not scattered inline.

**Rationale**: Clear ownership boundaries keep the blast radius of change small, preserve
independent deployability, and prevent coupling that accumulates into architectural debt.

**Review gate**: New code that reads another domain's database, bypasses a service boundary, or
duplicates shared infrastructure logic requires explicit justification before merging.

### III. Unit-First, Proportionate Testing and Logging Verification

#### Unit-test construction and required collaborators

- Unit tests MUST prefer `[Theory]` with `AutoFakeItEasyData` (or the repository's equivalent auto-data attribute) when generated inputs are suitable.
- Constructor dependencies and the SUT MUST be injected through test-method parameters: dependencies first, `sut` next, then scenario inputs and expected values.
- Do not create `A.Fake<T>()` inside a test when auto-data can inject the dependency. Use `A.CallTo` only to configure scenario-specific behavior.
- Test doubles MUST be supplied through test-method parameters; do not construct or pass nullable service instances, null loggers, or null transaction builders to the SUT.
- Required collaborators MUST be non-nullable and supplied by dependency injection, including loggers, transaction builders, repository factories, mappers, clocks, and domain services.
- Production code MUST NOT add nullable collaborators, null-conditional calls such as `logger?.Log...`, null-forgiving workarounds, or fallback branches solely to make tests easier to construct. A logger and transaction builder are always provided.
- Optional collaborators are allowed only when optionality is part of the domain contract and both states are tested.

Every backend behavior change MUST be tested with unit tests when its behavior can be exercised
without real infrastructure. Existing unit coverage MUST be reused and extended rather than
duplicated in integration tests. Integration tests are required only for behavior that depends
on a real persistence, database-concurrency, migration, schema-wiring, Kafka, Temporal, external
HTTP, or other infrastructure boundary that a unit test cannot prove. An integration test MUST
have a specific boundary-focused reason; a service scenario that can be isolated is not a valid
reason by itself.

Changes that cross persistence or integration boundaries MUST include the smallest possible
integration test for that boundary, in addition to unit tests for business behavior. Tests MUST
be removed or reduced when they duplicate existing coverage. Integration tests MUST NOT become
end-to-end copies of unit-test scenarios.
System/end-to-end tests are reserved for true cross-domain or real-infrastructure scenarios.
Integration tests MUST NOT access `DbContext` or Entity Framework directly; all persistence
assertions MUST go through repository or query-layer methods. Web UI changes MUST use Vitest
and React Testing Library. Tests MUST be scenario-driven and assert observable behavior, not
internal implementation details. Any new or changed critical workflow MUST include tests that
verify expected logging side effects at the appropriate boundary (for example, warning/error
paths and key lifecycle transitions).

**Rationale**: Proportionate testing keeps the suite fast and meaningful without under-testing
behavior that is genuinely risky. Banning raw EF in integration tests keeps assertions
decoupled from persistence implementation choices.

**Review gate**: Reviewers MUST reject duplicated integration scenarios when equivalent unit
coverage exists. PRs that add infrastructure-dependent behavior without a focused boundary test,
or that assert state through `DbContext` directly, MUST be returned for revision.

### IV. Frontend Consistency

Web work MUST follow the Next.js App Router + Relay + generated-artifact model already
established in `web/apps/webapp`. Relay fragments MUST be collocated with the component that
consumes them. Generated Relay artifacts and OpenAPI TypeScript clients MUST NOT be hand-edited.

The Skedular web monorepo uses two centralized workspace packages that apply uniformly across
all three products (`webapp`, `webapp-teams`, `webapp-spaces`):

- **`@skedular/ui`** (`web/packages/ui`) — the Skedular design system. Owns all visual
  primitives: typography wrappers, layout building blocks, theme tokens, and color palette.
  A change to `@skedular/ui` propagates to every product automatically. Feature and page
  components in any product MUST import typography wrappers from `@skedular/ui` (for example
  `import { BodyIconTypography } from '@skedular/ui'`) rather than importing MUI `Typography`
  directly. The only permitted exception is inside `web/packages/ui/src/typography/` itself,
  where MUI `Typography` is the low-level primitive being wrapped.

- **`@skedular/shared`** (`web/packages/shared`) — the centralized shared application layer.
  Owns all cross-product runtime modules: React providers, hooks, utilities (date, name, Relay
  error helpers), MUI helpers, cookie consent, and image upload. All three products MUST import
  these modules from `@skedular/shared` rather than maintaining per-product copies.
  `@skedular/shared` may import from `@skedular/ui`; the reverse is forbidden.

Product apps (`webapp`, `webapp-teams`, `webapp-spaces`) own only product-specific feature
components, route trees, per-product configuration (logger name, analytics tag IDs), and
product-specific Relay queries. Auth entry points (sign-in, callback, account settings,
notifications) remain in `webapp` and are shared entry points for all products.

#### Relay Mutation State Synchronization

Successful GraphQL mutations MUST update the Relay store through normalized mutation payloads,
declarative connection updates, or targeted Relay query refetches. Mutation payloads MUST return
the stable record ID and every field rendered from the changed record. When a mutation changes a
connection, aggregate, or linked records that the payload does not return, the implementation MUST
update that connection declaratively or refetch only the affected query. Mutation-success handlers
MUST NOT call `window.location.reload()` as a cache-invalidation mechanism. A browser reload is
allowed only for an explicit user-initiated recovery action in the shared error boundary.

**Rationale**: Relay is the client-side source of truth for fetched GraphQL records. Reloading the
entire document discards application state, masks incomplete mutation contracts, and makes simple
updates slower and less reliable than an explicit store or query update.

All user-facing and operator-facing copy MUST use American spelling and grammar; technical
identifiers (API fields, routes, schema names) are exempt. Any change to customer-facing or
operator-facing behavior MUST include a review of the corresponding public-web documentation.
Documentation MUST be updated in the same change when the behavior, eligibility rules, visible
states, cancellation/refund semantics, or user/operator actions are changed. If no update is
needed, the plan or review notes MUST record why the existing documentation remains accurate.

**Rationale**: Centralizing the design system and shared runtime modules into packages means a
single change propagates to all products simultaneously, eliminating duplication and drift.
Clear package boundaries (`@skedular/ui` for visual primitives, `@skedular/shared` for runtime
infrastructure) make ownership decisions unambiguous and keep product apps focused on
product-specific work.

**Review gate**: PRs that hand-edit Relay artifacts, import MUI `Typography` directly in
feature or page components (outside `web/packages/ui/src/typography/`), import
typography wrappers from any path other than `@skedular/ui`, duplicate shared providers or
utilities inside a product app instead of using `@skedular/shared`, or introduce
non-American-English copy in user-facing strings MUST be corrected before merging. PRs that
change customer-facing or operator-facing behavior MUST also include the corresponding
public-web documentation update or an explicit rationale for why no documentation change is
required. PRs that use a mutation-success browser reload, omit rendered mutation fields from the
payload, or leave an affected connection/count stale without a declarative update or targeted
refetch MUST be corrected before merging.

### V. Change Safety and Pattern Consistency

Changes that affect contracts, schemas, event definitions, GraphQL types, or generated surfaces
MUST explicitly account for regeneration and downstream impact before implementation begins.
New work MUST favor consistency with existing patterns over introducing parallel abstractions.
Local persisted-entity references MUST be modeled as explicit EF Core foreign keys with a
navigation property. Nullable FK and navigation properties define optional relationships;
non-nullable properties define required relationships. FK IDs and navigations MUST be grouped
together in entity classes, with the repository's ReSharper suppression comment immediately
above string ID properties. Plain indexed IDs are reserved for external, polymorphic,
snapshot, or intentionally unresolved references. Entity configuration MUST be updated before
regenerating migrations, migration designers, and model snapshots.
Any deviation from established patterns — new frameworks, alternative persistence approaches,
alternative event serialization — requires explicit justification documented in the relevant
plan or ADR. Exceptions to any principle in this constitution MUST be rare, explicit, and
justified; undocumented exceptions are violations. Any feature design that omits operational
logging for its core flows is treated as a pattern violation unless an explicit exception is
approved and documented.

**Rationale**: A large monorepo with many domains becomes unmaintainable when each domain
independently reinvents patterns. Explicit justification for exceptions keeps the architecture
legible and deviations visible to reviewers.

**Review gate**: Plan and task artifacts for any feature that introduces a new shared pattern or
deviates from an existing one MUST include a brief justification note before tasks are accepted.

### VI. Mandatory Feature Logging and Observability

Every feature MUST include structured logging as a first-class deliverable. "Feature complete"
means business behavior, tests, and logs are all present. Logging MUST cover:

- start and completion of core feature workflows
- meaningful state transitions and branch decisions
- integration boundaries (external APIs, Kafka, Temporal, databases) with correlation context
- failure and recovery paths with actionable detail for operators

Logs MUST use consistent structured properties, avoid sensitive payload leakage, and follow
existing domain logging conventions in `shared/` and domain-specific libraries. Silent features
without operationally useful logs are non-compliant.

**Rationale**: Reliable operations and incident response depend on reconstructing feature
behavior from logs without source-level debugging in production.

**Review gate**: A feature PR MUST identify where logging was added or updated, and reviewers
MUST reject changes that add feature behavior without corresponding structured logs.

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
contributors. Spec, plan, and task artifacts produced by `/speckit.specify`, `/speckit.plan`,
and `/speckit.tasks` MUST reference the applicable review gates. Implementation PRs are expected
to have been checked against every applicable gate before review is requested.

**Logging and observability expectations**: For every feature, logging is mandatory rather than
optional polish. Planning artifacts MUST call out logging scope. Task breakdowns MUST include
explicit logging implementation work. Verification MUST include checks that logs are emitted for
successful and failure paths of the feature's primary workflows.

**Non-compliance**: A violation identified during review blocks merge until resolved or until an
explicit, documented exception is agreed and committed alongside the change.

---

**Version**: 2.6.0 | **Ratified**: 2026-04-14 | **Last Amended**: 2026-08-17
