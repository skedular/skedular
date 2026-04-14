# Phase 0 Research: Team Domain Structured Logging

## Decision 1: Keep existing Enterprise.Shared Serilog pipeline and add component-level `ILogger<T>` usage only

- Decision: Reuse existing host-level `UseSerilogCustom(appName)` configuration and add
  structured `ILogger<T>` calls within Team domain components.
- Rationale: The Serilog pipeline is already standardised, secret-masking is already configured,
  and this feature focuses on missing component-level observability rather than pipeline redesign.
- Alternatives considered:
  - Replace or extend Serilog pipeline for this feature: rejected because it broadens scope and
    risks cross-domain side effects.
  - Add logging wrappers/abstractions per component: rejected as unnecessary complexity versus
    direct `ILogger<T>` pattern already used.

## Decision 2: Use primary-constructor logger injection consistently

- Decision: Add `ILogger<TComponent>` to primary constructor parameters across in-scope
  Team services/components.
- Rationale: Matches existing code style in Team subscribers and broader monorepo usage;
  keeps DI wiring implicit and testable.
- Alternatives considered:
  - Static logger instances: rejected because they reduce testability and break DI patterns.
  - Service-locator based logger resolution: rejected because it hides dependencies.

## Decision 3: Secret-safe structured logging policy

- Decision: Restrict structured properties to IDs, counts, booleans, enums, and outcomes.
  Do not log emails, names, tokens, claims, credentials, or payload content.
- Rationale: Aligns with constitution and existing security guidance; avoids accidental
  exposure even before sink-level masking.
- Alternatives considered:
  - Rely only on Serilog masking: rejected because sensitive values should not enter log
    properties in the first place.

## Decision 4: Log-level policy by outcome and component type

- Decision:
  - Authorization: denied at `LogWarning`, granted at `LogInformation`.
  - Cache miss/eviction: `LogDebug`.
  - Read methods: log only denial/failure/empty-result outcomes.
  - Mutation, publish, workflow-activity outcomes: `LogInformation` (and `LogError` for failures).
- Rationale: Balances operability signal quality against production log volume.
- Alternatives considered:
  - `LogInformation` for everything: rejected due to noise.
  - `LogDebug` for all operations: rejected due to insufficient production visibility.

## Decision 5: Testing approach remains proportionate and pattern-aligned

- Decision: Update existing unit tests for constructor dependency changes and critical logging
  assertions; add integration tests only where behavioural boundaries (persistence/events/workflows)
  are affected.
- Rationale: Meets constitution requirements and avoids over-testing logging internals.
- Alternatives considered:
  - Add system tests for logging output: rejected as disproportionate for this feature.
  - Skip logging assertions entirely: rejected because it weakens acceptance confidence.

## Decision 6: No contract regeneration required

- Decision: Do not touch `api-definitions/`, GraphQL schemas, OpenAPI contracts, or generated
  clients/artefacts.
- Rationale: Feature is implementation-only observability enhancement.
- Alternatives considered:
  - Add API-level log toggles in contracts: rejected as out of scope and unnecessary.
