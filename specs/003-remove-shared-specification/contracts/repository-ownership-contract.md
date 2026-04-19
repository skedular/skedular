# Repository Ownership Contract

## Purpose

This feature is an internal refactor. The relevant contract is not OpenAPI, GraphQL, or gRPC; it is the internal ownership contract for how domain code performs data access.

## Contract Rules

### Rule 1: Query composition belongs to the data-owning domain repository

- A consumer may request data from another domain.
- The query definition itself must live in the repository layer of the domain that owns that data.
- Consumers must not recreate business filters using shared specification objects or ad hoc LINQ in service code when a repository method is the intended contract.

### Rule 2: Repository methods must be explicit and behaviour-oriented

- Method names should express the supported business lookup.
- Inputs should represent business filters directly.
- Return shapes should match actual usage: single entity, nullable entity, collection, or paginated result.
- Tracking, include, soft-delete, and ordering rules are part of the method contract.
- Public repository contracts must not expose `IQueryable`, `Specification<T>`, or any replacement generic query abstraction.

### Rule 3: Cross-domain reads use supported repository interfaces

- Cross-domain consumers continue to access data through the existing repository factory and domain shared repository abstractions.
- This feature does not authorise direct database access across domains.

### Rule 4: Validation happens through repository behaviour

- Unit tests validate repository method semantics where practical.
- Integration tests validate workflow behaviour and assert persisted outcomes through repository methods.
- The removed shared specification abstraction is not the long-term behavioural contract.

## Migration Contract Map

- Organization-owned entities: methods live under `organization/shared/Organization.Shared/Repositories/`.
- Location-owned entities and analytics: methods live under `location/shared/Location.Shared/Repositories/`.
- Team-owned entities: methods live under `team/shared/Team.Shared/Repositories/`.
- Consumer-only domains such as Slack and customer call those methods; they do not become owners of the underlying query rules.

## Non-Goals

- No public API surface is intentionally changed by this feature.
- No new generic shared query abstraction is introduced to replace the old one.
- No transitional fallback production path remains for `Specification<T>`, `ISpecification<T>`, `SpecificationEvaluator<TEntity>`, or `IRepository.Query(ISpecification<T>)` once migration is complete.
