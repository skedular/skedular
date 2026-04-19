# Research: Remove Shared Specification

## Decision 1: Migrate inline specifications into explicit domain repository methods

**Decision**: Replace each active `Specification<T>` usage with a named repository method owned by the domain shared repository for the entity being queried.

**Rationale**: The codebase already uses this pattern widely in domain shared repositories such as location, team, and organization. It makes query intent explicit, keeps ownership with the data-owning domain, and removes the generic Enterprise.Shared query composition dependency that has caused maintenance issues.

**Alternatives considered**:

- Keep `Specification<T>` and only reduce new usages: rejected because the problematic abstraction and maintenance burden remain in the shared layer.
- Replace `Specification<T>` with another generic query object: rejected because it preserves the same architectural smell under a new name.
- Inline LINQ into every consumer without repository methods: rejected because it scatters ownership and duplicates domain query rules.

## Decision 2: Sequence migration by data-owning domain and consumer risk

**Decision**: Migrate in this order: organization, location/team-related reads, booking/marketplace consumers, customer/slack consumers, then Enterprise.Shared cleanup.

**Rationale**: Organization and location/team repositories provide foundational query surfaces used by other domains. Slack and booking consume team/location data, so data-owner repositories need the new methods first. Enterprise.Shared cleanup must be last because `IRepository.Query(ISpecification)` cannot be removed until all active consumers are migrated.

**Alternatives considered**:

- Remove shared infrastructure first: rejected because production code would still depend on it.
- Migrate consumers in arbitrary file order: rejected because it increases churn and breaks ownership-driven sequencing.

## Decision 3: Preserve includes, paging, ordering, and soft-delete rules inside repository methods

**Decision**: New repository methods will preserve existing filtering, include chains, ordering, grouping, paging, and soft-delete behaviour by using the same EF query rules currently expressed either inline or in existing repository extension helpers.

**Rationale**: This feature is a refactor, not a behaviour change. Repository methods must match current business-visible results exactly. Existing repository extension helpers like `AddDependentObjects(...)` and `AddSearchCriteria(...)` provide an established place to centralise those rules.

**Alternatives considered**:

- Simplify queries during migration: rejected because it risks behavioural regressions.
- Keep advanced cases on the generic specification path: rejected because it leaves the core problem unresolved.

## Decision 4: Keep cross-domain reads owned by the data-owning domain repository

**Decision**: When a consumer in one domain reads another domain's data, the query definition will move to the repository layer of the data-owning domain, not the consuming service.

**Rationale**: The constitution and repository notes emphasise domain ownership boundaries. Slack reading team/location data does not make Slack the owner of team/location query logic.

**Alternatives considered**:

- Let consuming domains create their own helper queries: rejected because it duplicates business rules and weakens ownership boundaries.
- Introduce a new cross-domain shared repository library: rejected because the repo already uses domain shared layers for this purpose.

## Decision 5: Replace specification-focused tests with repository-owned query tests

**Decision**: Validation will focus on unit tests for new repository methods and integration tests for affected services/workflows, with persistence assertions performed through repository methods rather than `DbContext`.

**Rationale**: Repository memory and AGENTS guidance require integration tests to assert through repository-layer queries. The removed shared abstraction should no longer be the object under test; the repository-owned behaviour should be.

**Alternatives considered**:

- Keep Enterprise.Shared specification evaluator tests as the main safety net: rejected because they test the mechanism being removed.
- Assert persistence directly through EF in integration tests: rejected by repository policy.

## Decision 6: Treat this as an internal refactor with internal repository contracts, not an API contract change

**Decision**: Document repository ownership and migration contracts in planning artefacts, but do not model this as a public API/OpenAPI/GraphQL contract change.

**Rationale**: No external schema or protocol surface is meant to change. The impacted contracts are internal repository method contracts between domain shared layers and their consumers.

**Alternatives considered**:

- Produce public interface contracts: rejected because the feature is internal.
- Skip documenting repository contracts entirely: rejected because implementation will span many domains and needs a shared planning reference.

## Decision 7: Logging scope is preservation-oriented, with targeted updates only where service boundary behaviour changes

**Decision**: Preserve existing service/workflow logging and add or adjust logs only where query-path refactors introduce new service-level branches, failures, or migration-sensitive lookups.

**Rationale**: Repositories in this codebase are generally not the place for broad workflow logging. Logging obligations are better satisfied at the service, job, processor, and activity boundaries that already own workflow context and correlation data.

**Alternatives considered**:

- Add pervasive repository-level logging to every new method: rejected because it would create noisy logs and break existing layering conventions.
- Ignore logging because this is a refactor: rejected because the constitution requires explicit logging scope consideration for every feature.
