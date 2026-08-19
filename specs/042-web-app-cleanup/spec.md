# Feature Specification: Web App Component Cleanup

**Feature Branch**: `042-web-app-cleanup`  
**Created**: 2026-08-19  
**Status**: Draft  
**Input**: User description: "Audit and remove unused components across webapp, webapp-spaces, webapp-teams, and webapp-hosts based on Next.js route usage, including related tests"

## Clarifications

### Session 2026-08-19

- Q: Should the cleanup remove only unreachable components and their component-specific tests, while retaining all route files and route-level tests unless a separate route removal is explicitly approved? → A: Preserve all routes and route-level tests; remove only unreachable components and component-only tests.
- Q: Should orphaned application-owned helpers, hooks, styles, fixtures, and configuration files also be removed when they are used only by components approved for deletion? → A: Remove orphaned app-owned support files transitively; retain shared or ambiguous files.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Establish the four-app inventory (Priority: P1)

As a maintainer, I want a complete inventory of the application-owned routes, components, helpers, and tests in the four web applications so that cleanup decisions are based on current evidence.

**Why this priority**: A reliable inventory is the prerequisite for safe deletion and prevents active behavior from being removed during cleanup.

**Independent Test**: Review the inventory and confirm it names every in-scope application directory, every route entry point, every candidate component/test, and each candidate’s reachability status.

**Acceptance Scenarios**:

1. **Given** the repository contains the four web applications, **When** the inventory is produced, **Then** it covers `webapp`, `webapp-spaces`, `webapp-teams`, and the repository’s actual host application directory, currently `webapp-host`.
2. **Given** shared component and UI packages exist, **When** the inventory is produced, **Then** those packages are excluded from cleanup decisions unless an in-scope application file is required to remove a now-unused import.
3. **Given** a file is reachable through a route, layout, middleware/proxy, API route, dynamic import, or another reachable application entry point, **When** it is classified, **Then** it is marked used or conditionally used with the evidence recorded.

### User Story 2 - Remove unreachable application code (Priority: P1)

As a maintainer, I want unreachable components and their dedicated tests removed from each web application so that the codebase is smaller and easier to maintain without changing reachable product behavior.

**Why this priority**: Removing obsolete code is the primary business outcome and reduces maintenance cost across all migrated applications.

**Independent Test**: Starting from the approved inventory, remove one app’s candidates and verify that its remaining routes, builds, lint checks, and relevant tests still pass.

**Acceptance Scenarios**:

1. **Given** a component has no reachable reference from an in-scope application entry point, **When** cleanup is applied, **Then** the component and tests that exist only for that component are removed.
2. **Given** a test covers multiple components or protects a route-level behavior, **When** an individual component is removed, **Then** the shared test is retained or updated rather than deleted wholesale.
3. **Given** a candidate is referenced only by another candidate already approved for removal, **When** cleanup is applied, **Then** the dependent removal is included and no broken import remains.
4. **Given** a component is imported through a barrel, alias, dynamic import, generated route convention, or test setup, **When** reachability is assessed, **Then** those references are considered before deletion.

### User Story 3 - Prove the cleanup is behavior-preserving (Priority: P2)

As a maintainer, I want each application checked after cleanup so that obsolete code is gone while active routes and expected user journeys continue to work.

**Why this priority**: The cleanup is only valuable if it does not regress the four deployed applications.

**Independent Test**: Run the agreed validation for each application and compare the route inventory before and after cleanup; all retained routes must remain available.

**Acceptance Scenarios**:

1. **Given** the cleanup is complete for an application, **When** its route/build/test validation runs, **Then** it completes without unresolved imports, type errors, lint failures, or failures caused by removed code.
2. **Given** a route was present before cleanup, **When** the post-cleanup route comparison runs, **Then** it is either still present or has an explicitly documented product decision explaining its removal.
3. **Given** an apparently unused file has an unresolved reference or ambiguous runtime loading path, **When** it is reviewed, **Then** it is retained and recorded as a follow-up rather than deleted speculatively.

### Edge Cases

- A component may be used by a route in one application but appear duplicated and unused in another; classification is application-specific.
- Dynamic route segments, route groups, parallel/intercepting routes, layouts, middleware/proxy files, API routes, and custom-domain entry points must be included in the route surface.
- Barrel exports may make a file appear used even when no consumer imports the specific symbol; unused exports may be removed only after checking all consumers.
- Tests may be the only reference to a component while still documenting a shared utility or route contract; those tests are not automatically removable.
- Route files and route-level tests are protected from deletion by default; they remain part of the inventory and validation surface.
- Orphaned application-owned helpers, hooks, styles, fixtures, and configuration may be removed transitively when no retained consumer uses them; shared or ambiguous files must be retained.
- Generated files, build output, dependencies, shared packages, and UI packages are not cleanup targets.
- The user’s “webapp-hosts” wording maps to the existing `webapp-host` application directory; the audit must document that naming discrepancy.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The cleanup MUST audit exactly four application scopes: `webapp`, `webapp-spaces`, `webapp-teams`, and `webapp-host`.
- **FR-002**: The audit MUST enumerate route entry points and route-supporting files for each application, including layouts, middleware/proxy behavior, API routes, dynamic routes, and custom-domain entry points where present.
- **FR-003**: The audit MUST classify each in-scope component and related test as used, conditionally used, unused, or unresolved, and MUST record the evidence supporting the classification.
- **FR-004**: A component MUST be eligible for deletion only when no reachable route, application entry point, runtime import, test contract, or retained file depends on it.
- **FR-005**: The cleanup MUST exclude shared component libraries, UI packages, generated artifacts, dependencies, build output, and unrelated applications from deletion.
- **FR-006**: When an unused component is removed, the cleanup MUST remove or update tests, fixtures, mocks, imports, exports, and documentation that exist only for that component.
- **FR-007**: The cleanup MUST preserve all retained route behavior and MUST NOT remove a route solely because its implementation file is not directly linked by a visible navigation component.
- **FR-008**: The cleanup MUST process each application independently while also checking cross-application package and workspace references before deleting shared application-owned code.
- **FR-009**: The final result MUST include an auditable summary of removed files, retained ambiguous files, affected routes, and validation results for each application.
- **FR-010**: Any route or component whose usage cannot be proven or disproven from repository evidence MUST be retained and listed as unresolved with a recommended follow-up.
- **FR-011**: The cleanup MUST preserve every existing route file and route-level test by default; removal of a route or route-level test requires separately recorded approval and is outside the default component-cleanup deletion set.
- **FR-012**: After a component is approved for removal, the cleanup MUST follow its application-owned dependency chain and remove supporting helpers, hooks, styles, fixtures, configuration, and tests that have no remaining consumer; shared or ambiguous dependencies MUST be retained and documented.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The cleanup workflow MUST provide structured or machine-readable progress output for each application audit and cleanup phase.
- **LOG-002**: The workflow MUST report each deletion decision with the candidate path, classification, and evidence category without exposing secrets or customer data.
- **LOG-003**: The workflow MUST report validation failures with the application, route or test area, and actionable failure context.
- **LOG-004**: The final audit record MUST distinguish confirmed unused files from files retained because their usage was ambiguous.

### Key Entities

- **Application Scope**: One of the four in-scope Next.js applications and its route, source, and test boundaries.
- **Route Entry Point**: A page, layout, route handler, middleware/proxy, or equivalent application entry that can make code reachable.
- **Reachability Classification**: The evidence-based status of a file or symbol: used, conditionally used, unused, or unresolved.
- **Cleanup Candidate**: An application-owned component or related test approved for removal after reachability analysis.
- **Validation Record**: The before/after route and verification result for one application.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of the four in-scope applications have a completed route and candidate inventory before deletion begins.
- **SC-002**: 100% of deleted components have a recorded unreachable classification and a reviewable evidence trail.
- **SC-003**: 100% of retained pre-cleanup routes remain available after cleanup unless an explicitly approved route removal is recorded.
- **SC-004**: Each in-scope application completes its agreed build, lint, type, and relevant test validation with zero cleanup-introduced failures.
- **SC-005**: Every ambiguous candidate is documented rather than silently deleted, achieving 0 speculative deletions.
- **SC-006**: Maintainers can identify the owner, reason, and validation result for every deletion from the final cleanup record in under 5 minutes.
- **SC-007**: The final candidate inventory contains no confirmed-unused application-owned components or component-only tests within the four audited applications.

## Assumptions

- The existing Next.js applications and their checked-in source/test files are the system of record for this cleanup.
- “Web app hosts” refers to the existing `webapp-host` directory; no separate `webapp-hosts` application exists today.
- Route reachability includes direct and indirect runtime references, not only visible navigation links.
- Shared and UI component packages may be depended on by the applications but are not being cleaned up in this feature.
- Generated artifacts should be regenerated by their normal repository workflows when source changes require it; generated output is not hand-edited as part of the audit.
- Validation uses the repository’s existing per-application quality checks and relevant automated tests; adding new product behavior is out of scope.
- Deleting files is acceptable only within the in-scope application directories and their application-specific tests/support files.
