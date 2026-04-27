# Feature Specification: Modularize Webapp Products

**Feature Branch**: `004-modularize-webapp-products`  
**Created**: 2026-04-27  
**Status**: Ready for Planning  
**Input**: User description: "Review the entire web application and create a specification for modularizing the codebase in preparation for splitting Skedular into Skedular, Skedular for Teams, and Skedular for Spaces. Identify repeated patterns, shared components, duplicated logic, layout structures, hooks, utilities, and UI behaviours that can be extracted into reusable modules. Move design related components, shared UI primitives, styling conventions, layout building blocks, and reusable visual patterns into the design system or UI framework layer. Separate product specific modules so future work can clearly place functionality under the correct product area: core Skedular, Teams, or Spaces. Do not change product behaviour unless the change is required to support modularization or remove duplication. Refactor the web application toward clear boundaries, better naming, reusable abstractions, and reduced coupling between features. Update imports, folder structure, routing references, and documentation to reflect the new modularized structure. Implement or update unit tests for all affected web application modules, components, hooks, and utilities. Fix any broken tests caused by the refactor and ensure the web application still builds successfully. Challenge assumptions, identify unclear ownership boundaries, and ask follow up questions before finalizing the implementation plan."

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Establish Shared Module Boundaries (Priority: P1)

A web developer needs a reliable way to identify repeated UI structures, hooks, utilities, and behavioural patterns across the current products so they can extract those patterns once and reuse them without changing the experience for end users.

**Why this priority**: This is the foundation for every later split. If shared surfaces are not identified and extracted first, product separation will duplicate logic and increase coupling.

**Independent Test**: A developer can take one repeated slice, such as a shared layout pattern or hook family, move it into the approved shared module boundary, update all affected imports, and verify that the affected products still behave the same.

**Acceptance Scenarios**:

1. **Given** repeated modules exist across the web products, **When** the modularization audit is completed, **Then** each repeated module is classified as shared design system, shared application module, core Skedular, Teams, or Spaces.
2. **Given** a repeated component or hook is extracted into a shared module, **When** the affected products consume the shared module, **Then** behaviour and visual output remain unchanged unless a documented modularization fix requires otherwise.
3. **Given** a shared abstraction replaces duplicated logic, **When** a developer reviews the resulting module boundaries, **Then** the shared module owns the reusable logic and no product keeps an undocumented duplicate copy.

---

### User Story 2 - Separate Product Ownership Clearly (Priority: P1)

A web developer needs each feature area, route, and supporting module to have a clear product owner so future work can be placed under core Skedular, Teams, or Spaces without guessing where code belongs.

**Why this priority**: The main goal of this initiative is product separation. Without explicit ownership boundaries, the codebase will continue to drift back into a single mixed product.

**Independent Test**: A developer can inspect any affected route or feature module, determine its product ownership from the structure and documentation alone, and place new work in the correct area without needing tribal knowledge.

**Acceptance Scenarios**:

1. **Given** a feature contains product-specific behaviour, **When** the feature is modularized, **Then** the product-specific code resides under the owning product area and shared dependencies remain outside that area.
2. **Given** an area currently mixes core, Teams, and Spaces concerns, **When** the modularization work is applied, **Then** the resulting structure exposes clear ownership boundaries and any temporary adapters are explicitly documented.
3. **Given** updated routing and folder references, **When** a developer follows them from entry point to feature implementation, **Then** the path leads through the correct ownership boundary without ambiguous cross-product placement.

---

### User Story 3 - Consolidate Shared Design Patterns (Priority: P2)

A design system maintainer needs reusable visual primitives, layout building blocks, and repeated styling conventions to live in the shared UI layer so visual consistency can be maintained once and used across all products.

**Why this priority**: Shared visual patterns are a major source of repetition, but they only deliver value after the primary ownership boundaries are defined.

**Independent Test**: A developer can replace repeated visual structures in more than one product with shared design system building blocks and confirm the products still render consistently.

**Acceptance Scenarios**:

1. **Given** repeated visual structures exist across the products, **When** they are reviewed for extraction, **Then** reusable visual primitives and layout patterns are moved to the shared UI layer.
2. **Given** a product needs the shared visual pattern, **When** it imports the shared UI module, **Then** the product no longer needs a product-local copy of the same pattern unless a documented exception exists.
3. **Given** shared styling conventions are updated, **When** they are applied across products, **Then** the same design rule is expressed through shared modules rather than repeated local definitions.

---

### User Story 4 - Preserve Delivery Safety During Refactor (Priority: P2)

A maintainer needs the modularization effort to keep builds, tests, and documentation accurate so the refactor can proceed without silently breaking the web products.

**Why this priority**: The refactor is only safe if the codebase remains verifiable. This work reduces risk and makes the split sustainable.

**Independent Test**: After modularizing an affected slice, a maintainer can run the documented verification steps, confirm the affected automated tests pass, and see updated documentation and references that match the new structure.

**Acceptance Scenarios**:

1. **Given** modules are moved or renamed, **When** imports, folder references, and routing references are updated, **Then** the affected products still build successfully.
2. **Given** affected components, hooks, and utilities are refactored, **When** the verification suite runs, **Then** unit tests for the affected modules pass and any broken tests caused by the refactor are fixed.
3. **Given** the modularized structure is documented, **When** a developer reads the updated guidance, **Then** they can understand where shared, core, Teams, and Spaces code belongs.

---

### Edge Cases

- What happens when a repeated module is shared by only two of the three products and not the third?
- How does the system handle routes or layouts that currently combine shared concerns with product-specific behaviour in the same entry point?
- What happens when a repeated visual pattern also contains product-specific copy, permissions, or business rules?
- How are circular dependencies handled when a shared abstraction is extracted from mixed product code?
- What happens when moving a utility or hook breaks existing unit tests because older path assumptions are embedded in the test setup?
- How are intentionally duplicated modules handled when duplication remains necessary for product divergence?

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: System MUST produce and maintain an ownership map for affected web application modules that classifies each targeted module as shared design system, shared application module, core Skedular, Teams, or Spaces.
- **FR-002**: System MUST identify repeated components, hooks, utilities, layout structures, styling conventions, and UI behaviours that are candidates for extraction into reusable modules.
- **FR-003**: System MUST define the boundary between the design system or UI framework layer and non-visual shared application modules. The design system is limited to visual primitives and layout building blocks only; composite UI patterns that include product copy, local state handling, or behavioural orchestration must live in shared application modules outside the design system.
- **FR-004**: System MUST move reusable visual primitives, styling conventions, layout building blocks, and repeated visual patterns into the shared UI layer when they do not require product-specific ownership.
- **FR-005**: System MUST separate product-specific modules so future work can be placed unambiguously under core Skedular, Teams, or Spaces.
- **FR-006**: System MUST define the ownership boundary for all modules currently shared across the three web products. Because each product is deployed on its own domain, route-level separation is enforced by deployment; this initiative focuses on module and component ownership boundaries. Transitional adapters are permitted only for non-route module extraction in progress, must be explicitly documented with an owner and removal condition, and must be removed before the relevant slice is considered complete.
- **FR-007**: System MUST preserve existing product behaviour unless a change is required to support modularization or remove duplication.
- **FR-008**: System MUST reduce coupling between feature areas by replacing direct cross-product dependencies with approved shared abstractions or documented ownership hand-offs.
- **FR-009**: System MUST update imports, folder structure references, routing references, and supporting documentation to match the modularized structure.
- **FR-010**: System MUST implement or update unit tests for all affected web application modules, components, hooks, and utilities touched by the modularization work.
- **FR-011**: System MUST fix any broken tests caused by the refactor before the modularized slice is considered complete.
- **FR-012**: System MUST keep the web application buildable after each completed modularization slice.
- **FR-013**: System MUST define ownership for cross-product authenticated journeys and account-management surfaces. Sign-in, callback handling, account settings, and notifications remain shared entry points owned by core Skedular; each product owns its own post-authentication journeys and product-specific experiences from that point forward.
- **FR-014**: System MUST document any allowed temporary exceptions where duplication or transitional adapters remain necessary, including the reason and intended removal condition.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Modularization work MUST preserve existing user-facing logging and telemetry behaviour for any affected user journeys unless a documented modularization change requires otherwise.
- **LOG-002**: Verification workflows for modularized slices MUST emit actionable build and test output that identifies the affected module boundary when a refactor introduces a failure.
- **LOG-003**: Any newly introduced shared modules for cross-product behaviour MUST continue to expose the correlation context already relied on by affected user journeys, test harnesses, and diagnostics.
- **LOG-004**: Logs and diagnostics produced during modularization MUST avoid leaking sensitive data while still identifying the product area and module boundary involved in a failure.

### Key Entities _(include if feature involves data)_

- **Ownership Map**: The maintained classification of modules, routes, layouts, hooks, utilities, and UI patterns into shared design system, shared application, core Skedular, Teams, or Spaces.
- **Shared Design Module**: A reusable visual primitive, styling rule, or layout building block that can be consumed across products without product-specific ownership.
- **Shared Application Module**: A reusable non-visual abstraction such as a hook, utility, provider, or behavioural helper that is intentionally shared across product areas.
- **Product Module**: A feature or supporting module owned by one product area only: core Skedular, Teams, or Spaces.
- **Migration Adapter**: A temporary boundary used to keep behaviour stable while a mixed-ownership module is being separated into clearer product or shared modules.
- **Verification Suite**: The set of unit tests, build checks, and documentation checks used to confirm that each modularized slice remains stable.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of modules touched by this initiative are assigned to exactly one documented ownership category or a documented transitional exception.
- **SC-002**: For every modularized slice, no undocumented duplicate implementation of the extracted pattern remains in the affected products after the slice is completed.
- **SC-003**: 100% of affected unit tests for touched modules, components, hooks, and utilities pass before each modularized slice is considered complete.
- **SC-004**: 100% of affected web application build checks pass after each completed modularized slice.
- **SC-005**: Developers can determine the correct ownership area for a touched route, component, hook, or utility in under 5 minutes using the updated structure and documentation alone.
- **SC-006**: Shared visual patterns extracted into the shared UI layer replace repeated local implementations in every targeted product area unless a documented exception remains.
- **SC-007**: Behavioural regressions in the affected user journeys are limited to explicitly approved modularization changes and zero unapproved user-facing behaviour changes are introduced.

## Assumptions

- Each web product is deployed on its own domain: `webapp` (core Skedular scheduler), `webapp-teams` (Skedular for Teams), `webapp-spaces` (Skedular for Spaces). Route-level separation between products is therefore enforced by deployment boundaries, not by this initiative.
- Each product has its own distinct look, feel, and feature set. The landing page and primary journeys of one product are not shared with the others.
- This initiative focuses on modularization and ownership boundaries inside the existing web codebase rather than changing product scope, pricing, permissions, or customer-facing behaviour.
- Reusable visual assets, layout structures, and styling conventions should be centralised when they do not require product-specific ownership.
- Shared non-visual abstractions may be created when they reduce duplication and do not blur product ownership boundaries.
- Incremental delivery is acceptable so long as each completed slice leaves the affected products buildable, tested, and documented.
- Temporary adapters or transitional duplication are allowed only when they are explicitly documented with ownership and removal conditions.
- Existing automated verification can be expanded or repaired as part of this initiative, but the initiative is not intended to broaden into unrelated feature work.
- Documentation updates are part of the deliverable because developers will need explicit guidance on where future work belongs.
