# Feature Specification: Remove Shared Specification

**Feature Branch**: `003-remove-shared-specification`  
**Created**: 2026-04-19  
**Status**: Draft  
**Input**: User description: "I currently have a specification and a specification evaluator sitting inside the Enterprise.Shared database and are currently using this in a few places in the entire project. This has caused me really issues in the past.

What I want you to do is come up with a set of stories that go through every domain one by one. Everywhere that this specification is used, replace that one by adding a method into the repository function inside the shared object for that domain, for the repository layer. We are trying to get rid of the specification and specification evaluator and everything that comes with that from the entire Enterprise.Shared and keep that whole layer really sitting inside the repository layer."

## Clarifications

### Session 2026-04-19

- Q: After migration, should the shared specification path be removed entirely from production code and the public Enterprise.Shared repository contract, or kept as a deprecated transitional API? → A: Remove it entirely from production code and the public Enterprise.Shared repository contract.
- Q: Should domain repositories expose only explicit business lookups and concrete result shapes, or still expose IQueryable after the shared specification path is removed? → A: Domain repositories expose only explicit methods with concrete return shapes such as entity, nullable entity, collection, or paginated result.
- Q: When multiple former specification-based lookups overlap, should the public repository surface stay narrowly business-oriented and reuse shared query-building only internally? → A: Public repository interfaces use narrow business methods, with shared query-building reused only inside repository implementation/helpers.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Migrate Organization Queries (Priority: P1)

As a developer maintaining the organization-facing domains, I need every organization-related use of the shared specification abstraction replaced with explicit repository-layer methods owned by the relevant domain shared layer so that business services no longer depend on Enterprise.Shared query composition.

**Why this priority**: Organization-owned queries are reused by several flows and are a representative proving ground for the migration approach, including filtering, active-state lookups, analytics reads, and offering/tag lookups.

**Independent Test**: Can be fully tested by replacing all organization-related specification-based reads with named repository methods, running the affected organization flows, and confirming the public behaviour and returned records remain unchanged.

**Acceptance Scenarios**:

1. **Given** an organization service currently composes a shared specification to fetch domain data, **When** the migration is applied, **Then** the service uses a domain-owned repository method with equivalent business filtering and returns the same functional result.
2. **Given** organization repository methods need domain-specific filters such as active records, current terms, category membership, or offering state, **When** those methods are introduced, **Then** the filtering logic resides in the domain repository layer rather than a shared generic evaluator.
3. **Given** organization-related consumers in APIs, jobs, or processors execute their existing workflows, **When** the migrated repository methods are used, **Then** those workflows continue to succeed without depending on the shared specification abstraction.

---

### User Story 2 - Migrate Location And Team Queries (Priority: P1)

As a developer maintaining location and team workflows, I need location-, resource-, and team-related reads to move from shared specifications to domain repository methods so that the query contract is explicit, easier to reason about, and owned by the domains that use it.

**Why this priority**: Location and team data are used across booking, Slack, and location services, so they are a major source of hidden coupling caused by the current generic specification approach.

**Independent Test**: Can be fully tested by migrating the location and team consumers to named repository methods, then validating that booking filters, Slack pages, edit flows, and analytics reads still return the expected location, resource, and team records.

**Acceptance Scenarios**:

1. **Given** a workflow fetches locations, resources, tags, or teams through a shared specification, **When** the migration is completed, **Then** it calls a domain-owned repository method that clearly expresses the supported lookup.
2. **Given** a location or team query is reused across multiple consumers, **When** repository methods are introduced, **Then** the shared method is owned by the appropriate domain shared layer and reused instead of recreating ad hoc query definitions in calling services.
3. **Given** location and team workflows rely on non-deleted filtering, membership-by-id lookups, or analytics time-slice retrieval, **When** the repository methods are executed, **Then** those business rules are preserved exactly.

---

### User Story 3 - Migrate Booking And Marketplace Queries (Priority: P1)

As a developer maintaining booking and marketplace flows, I need booking- and marketplace-related services to stop constructing shared specifications and instead use repository methods owned by their domains so that recurring booking, subscription, and product workflows remain stable without depending on Enterprise.Shared query composition.

**Why this priority**: Booking and marketplace flows are operationally sensitive and rely on cross-domain reads for teams, tags, offerings, and subscription state; replacing the abstraction here removes a large portion of the current risk surface.

**Independent Test**: Can be fully tested by running representative booking creation, recurring booking, subscription, and marketplace product flows after migrating their lookups to domain repository methods and confirming the same domain decisions are made.

**Acceptance Scenarios**:

1. **Given** booking services currently build shared specifications for location, team, or related domain reads, **When** those services are migrated, **Then** they use named repository methods and the booking decisions remain unchanged.
2. **Given** marketplace product and tag lookups currently rely on shared specification objects, **When** repository methods replace them, **Then** marketplace services continue to resolve the same products, tags, and related records.
3. **Given** recurring booking and subscription workflows execute after the migration, **When** they perform repository reads, **Then** they continue to respect the same domain constraints and state filters as before.

---

### User Story 4 - Migrate Customer And Slack Queries (Priority: P2)

As a developer maintaining customer and Slack integrations, I need customer- and Slack-facing consumers to use domain-owned repository methods instead of shared specifications so that messaging, page rendering, and subscriber workflows no longer depend on generic shared query machinery.

**Why this priority**: These areas use several small, targeted lookups that are straightforward to migrate but important for eliminating the abstraction from outward-facing integration flows.

**Independent Test**: Can be fully tested by migrating customer and Slack consumers, then validating that subscriber handling, daily update jobs, page rendering, and action handlers still locate the same organization members, workspaces, teams, and locations.

**Acceptance Scenarios**:

1. **Given** a Slack job, page, or action handler currently supplies a shared specification to find teams, locations, or workspace members, **When** it is migrated, **Then** it uses a repository method owned by the relevant domain shared layer.
2. **Given** a customer workflow currently resolves organization-linked records through the shared specification abstraction, **When** the migration is applied, **Then** the workflow still resolves the same records through explicit repository methods.
3. **Given** customer and Slack workflows are executed after the change, **When** they query their domain data, **Then** behaviour remains unchanged from the user and operator perspective.

---

### User Story 5 - Remove Shared Specification Infrastructure (Priority: P2)

As a platform maintainer, I need Enterprise.Shared to stop owning the specification abstraction, evaluator, and repository entry points that depend on them so that data access responsibilities are fully contained within domain repository layers and the old abstraction cannot be reused.

**Why this priority**: The migration is incomplete until the shared abstraction is removed or reduced to the point that domains no longer rely on it, otherwise the old coupling and regression risk remain in place.

**Independent Test**: Can be fully tested by confirming all identified usages have been migrated, the shared specification/evaluator path has been removed from active production code and the public shared repository contract, and the solution still builds and passes affected tests.

**Acceptance Scenarios**:

1. **Given** all known domain usages have been migrated, **When** the shared database layer is reviewed, **Then** the specification abstraction and evaluator are removed from active production use and the public shared repository contract.
2. **Given** repository query responsibilities are meant to sit with each domain, **When** the cleanup is complete, **Then** Enterprise.Shared no longer exposes a generic query composition mechanism that new domain code can adopt.
3. **Given** a developer adds a new domain query after this feature, **When** they follow the resulting pattern, **Then** they do so by extending the domain repository layer rather than introducing a new shared specification.

### Edge Cases

- A single workflow may depend on lookups owned by a different domain; the migration must still move the query definition into the owning domain's repository layer rather than duplicating logic in the caller.
- Some current specifications may combine filtering, ordering, grouping, includes, or paging; repository methods must preserve those business-visible outcomes even when the generic abstraction is removed.
- Domains with no active specification usage must not receive unnecessary repository changes solely for consistency.
- Shared queries used by APIs, jobs, processors, and activities must continue to return consistent results across all call sites after consolidation.
- If two consumers appear to need similar lookups but with materially different business rules, the migration must keep them as distinct repository methods instead of over-generalising them.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST identify every active use of the shared specification abstraction in domain code and map each use to an owning domain repository method.
- **FR-002**: The system MUST replace each identified specification-based lookup with an explicit repository-layer method owned by the relevant domain shared layer.
- **FR-003**: The system MUST preserve the existing business-visible behaviour of each migrated query, including filtering, ordering, grouping, paging, inclusion of related data, and soft-delete handling where applicable.
- **FR-004**: The system MUST provide a domain-by-domain migration path covering, at minimum, organization, location, team-related reads, booking, marketplace, customer, and Slack consumers where active usages exist.
- **FR-005**: The system MUST keep cross-domain reads owned by the domain that owns the underlying data instead of moving query logic into consuming services.
- **FR-006**: The system MUST ensure APIs, jobs, processors, activities, and other consumers use repository methods rather than constructing shared specification objects directly.
- **FR-007**: The system MUST remove the shared specification abstraction, evaluator, and `IRepository.Query(ISpecification<T>)` path from Enterprise.Shared once all active consumers have been migrated.
- **FR-008**: The system MUST prevent any deprecated or fallback generic shared query abstraction from remaining available for future domain production query work.
- **FR-009**: The system MUST require public domain repository interfaces to expose explicit business-oriented methods with concrete return shapes rather than `IQueryable`.
- **FR-010**: The system MUST keep public repository contracts narrowly business-oriented, while allowing internal repository helpers or extension methods to reuse shared query-building logic where behaviour overlaps.
- **FR-011**: The system MUST update or replace affected automated tests so they validate repository-owned query behaviour rather than the removed shared specification mechanism.
- **FR-012**: The system MUST document the new ownership rule clearly enough that future developers know domain query composition belongs in domain repository layers.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for start/completion of core workflows.
- **LOG-002**: Feature MUST emit structured logs for meaningful state transitions and branch decisions.
- **LOG-003**: Feature MUST emit actionable warning/error logs for failure and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context (for example request/workflow identifiers) and MUST avoid sensitive data leakage.

### Key Entities _(include if feature involves data)_

- **Domain Repository Method**: A named repository lookup owned by a domain shared layer that encapsulates a specific business query contract.
- **Shared Specification Usage**: Any existing consumer that currently defines query criteria through the generic shared specification abstraction.
- **Domain Query Consumer**: An API, job, processor, subscriber, activity, or service that needs domain data to execute a workflow.
- **Repository Ownership Boundary**: The rule that query composition belongs in the repository layer of the domain that owns the data being queried.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of identified active shared specification usages in the targeted domains are replaced by domain-owned repository methods.
- **SC-002**: 100% of migrated workflows in the affected domains complete with no behaviour regressions in their acceptance and regression checks.
- **SC-003**: Developers can determine the owning repository method for each migrated query in under 5 minutes during code review or maintenance.
- **SC-004**: Enterprise.Shared no longer exposes the specification abstraction, evaluator, or `IRepository.Query(ISpecification<T>)` path for domain production code.
- **SC-005**: New domain query work can be added without introducing any new dependency on the removed shared specification abstraction.

## Assumptions

- Current active usages are concentrated in organization, location, team-related consumers, booking, marketplace, customer, and Slack, based on present repository findings.
- Domains with no active specification usage are only in scope for verification that they do not depend on the shared abstraction, not for artificial repository changes.
- Existing repository abstractions in each domain shared layer are sufficient to host the new named query methods without redefining domain ownership boundaries.
- Behaviour preservation matters more than minimising the number of repository methods; explicit, domain-owned lookups are preferred over a new generic query helper.
- Public repository contracts remain intentionally narrow even when repository implementations internally share query-building helpers.
- The resulting work will be planned and delivered domain by domain so each migration slice can be tested independently before shared abstraction removal is finalised.
