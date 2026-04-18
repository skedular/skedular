# Feature Specification: Split UI into Three Products

**Feature Branch**: `002-split-ui-products`  
**Created**: 2026-04-18  
**Status**: Draft  
**Input**: Split web app into three separate products: current web app, teams web app, and spaces web app with shared infrastructure patterns and design system.

## Clarifications

### Session 2026-04-18

- Q: How should the three web app products be named in the codebase? → A: `webapp` (current), `webapp-teams`, `webapp-spaces`
- Q: Should all three web apps always sync to the same design system version, or can they diverge? → A: Always sync to latest design system version (all three use same version)
- Q: Should teams and spaces app deployments depend on current web app deployment, or deploy independently? → A: Independent parallel deployments (no inter-product deployment dependencies)
- Q: When should teams and spaces functionality be extracted from the current web app? → A: Deferred to Phase 2 after scaffolding is production-ready (extraction is a separate subsequent phase)
- Q: What is the target deployment time for a single web app workspace? → A: < 5 minutes (aggressive but achievable target for streamlined CI/CD)

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Scaffold teams web app Project Structure (Priority: P1)

A developer needs to create an empty teams web app project that mirrors the current web app's infrastructure, Terraform, and application structure so that private business logic can eventually be separated from the public web app.

**Why this priority**: This is the foundational scaffolding required for the product split. Without it, no private functionality can be extracted or implemented.

**Independent Test**: The teams web app project is created with all infrastructure-as-code, Terraform workspaces, and project skeleton in place. A developer can run Terraform init/validate on each workspace and verify it resolves correctly. The project structure can be browsed and should match the current web app pattern.

**Acceptance Scenarios**:

1. **Given** a new teams web app project folder exists, **When** examining the directory structure, **Then** it contains subdirectories mirroring the current web app: `infrastructure/`, `workspaces/`, `src/`, `docs/`, etc.
2. **Given** Terraform workspaces exist in the teams web app, **When** running `terraform init -backend=false -upgrade` and `terraform validate` on each workspace (staging, common_resources, production), **Then** all workspaces initialize and validate without errors.
3. **Given** the teams web app project, **When** building and running it locally, **Then** it loads the shared design system and renders a basic placeholder page without build errors.
4. **Given** Terraform state backend configuration, **When** inspecting the state backend references, **Then** they point to the same genesis S3 backend as the current web app with appropriate workspace-specific paths.

---

### User Story 2 - Scaffold spaces web app Project Structure (Priority: P1)

A developer needs to create an empty spaces web app project that mirrors the current web app's infrastructure, Terraform, and application structure so that marketplace admin and listing logic can eventually be separated from the public web app.

**Why this priority**: This is equally foundational as the teams web app and required for the complete product split strategy.

**Independent Test**: The spaces web app project is created with all infrastructure-as-code, Terraform workspaces, and project skeleton in place. A developer can run Terraform init/validate on each workspace and verify it resolves correctly. The project structure matches the current web app pattern.

**Acceptance Scenarios**:

1. **Given** a new spaces web app project folder exists, **When** examining the directory structure, **Then** it contains subdirectories mirroring the current web app: `infrastructure/`, `workspaces/`, `src/`, `docs/`, etc.
2. **Given** Terraform workspaces exist in the spaces web app, **When** running `terraform init -backend=false -upgrade` and `terraform validate` on each workspace (staging, common_resources, production), **Then** all workspaces initialize and validate without errors.
3. **Given** the spaces web app project, **When** building and running it locally, **Then** it loads the shared design system and renders a basic placeholder page without build errors.
4. **Given** Terraform state backend configuration, **When** inspecting the state backend references, **Then** they point to the same genesis S3 backend as the current web app with appropriate workspace-specific paths.

---

### User Story 3 - Create Health/Monitoring Projects for teams web app (Priority: P2)

A developer needs to create a health/monitoring project for the teams web app that follows the same patterns and naming conventions as the current web app help project.

**Why this priority**: Health/monitoring infrastructure is important for observability but can be set up after the core teams web app scaffolding is in place.

**Independent Test**: The health project is created with the same structure as the existing web app help project. It can be deployed independently and includes monitoring/health check endpoints.

**Acceptance Scenarios**:

1. **Given** the teams health project exists, **When** examining its structure, **Then** it mirrors the naming and layout of the current web app help project.
2. **Given** the teams health project, **When** building and deploying it, **Then** it successfully deploys to the same Vercel infrastructure used by other health projects.

---

### User Story 4 - Create Health/Monitoring Projects for spaces web app (Priority: P2)

A developer needs to create a health/monitoring project for the spaces web app that follows the same patterns and naming conventions as the current web app help project.

**Why this priority**: Health/monitoring infrastructure is important for observability but can be set up after the core spaces web app scaffolding is in place.

**Independent Test**: The health project is created with the same structure as the existing web app help project. It can be deployed independently and includes monitoring/health check endpoints.

**Acceptance Scenarios**:

1. **Given** the spaces health project exists, **When** examining its structure, **Then** it mirrors the naming and layout of the current web app help project.
2. **Given** the spaces health project, **When** building and deploying it, **Then** it successfully deploys to the same Vercel infrastructure used by other health projects.

---

### User Story 5 - Configure Shared Design System Integration (Priority: P1)

A developer needs to ensure both new web apps (teams and spaces) correctly import and use the shared design system so they have consistent UI/UX without duplicating design code.

**Why this priority**: Shared design system integration is critical from the start to maintain design consistency across all three products and prevent duplication.

**Independent Test**: Both private and spaces web apps have design system dependencies configured correctly. A developer can verify they build successfully and render UI components from the shared design system.

**Acceptance Scenarios**:

1. **Given** both new web apps are created, **When** checking their package dependencies and import statements, **Then** they reference the shared design system package correctly.
2. **Given** the teams web app builds, **When** rendering a page, **Then** shared design system components are available and render correctly.
3. **Given** the spaces web app builds, **When** rendering a page, **Then** shared design system components are available and render correctly.

---

### Edge Cases

- What happens if Terraform init fails for a new workspace (missing provider/module)?
- How are DNS and domain configurations managed for three separate products?
- What if the shared design system releases a breaking change after private/spaces apps are scaffolded?
- How should GitHub Actions CI/CD pipelines be configured for three separate web apps?
- What about shared authentication/authorization across the three products?

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: System MUST provide a complete project scaffold for the teams web app (`webapp-teams/` folder) that includes infrastructure-as-code (Terraform), web application structure, and build/deployment configurations.
- **FR-002**: System MUST provide a complete project scaffold for the spaces web app (`webapp-spaces/` folder) that includes infrastructure-as-code (Terraform), web application structure, and build/deployment configurations.
- **FR-003**: Both new web apps MUST use the same directory structure and naming conventions as the current web app (`webapp/` folder) to ensure consistency and reduce cognitive load.
- **FR-004**: Both new web apps MUST reference and utilize the shared design system for UI components and styling.
- **FR-005**: Both new web apps MUST configure Terraform workspaces (staging, common_resources, production) that validate without errors.
- **FR-006**: Both new web apps MUST configure Terraform backend state pointing to the same genesis S3 backend as the current web app, with workspace-scoped state files.
- **FR-007**: Both new web apps MUST support local development environment (ability to build, run, and test locally).
- **FR-008**: Health/monitoring projects for both new web apps MUST be created with structure mirroring the current web app help project.
- **FR-009**: GitHub Actions CI/CD workflows MUST be configured for both new web apps following existing patterns (lint, validate, build, deploy).
- **FR-010**: Both new web apps MUST include placeholder/empty implementations that can serve as starting points for feature development.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Terraform deployments for new web apps MUST emit structured logs showing workspace initialization, planning, and apply operations.
- **LOG-002**: Web app build and deployment processes MUST emit structured logs showing build stages, test execution, and deployment steps.
- **LOG-003**: Application startup MUST log initialization steps including design system loading and configuration validation.
- **LOG-004**: All logs MUST avoid leaking sensitive infrastructure details or API keys and MUST include correlation identifiers.

### Key Entities _(include if feature involves data)_

- **Web App Project**: Represents a complete web application deployment unit with infrastructure, source code, configuration, and CI/CD pipelines. Each project (current, private, marketplace) is an independent instance.
- **Terraform Workspace**: Represents an environment (staging, common_resources, production) within a web app project with independent state and configuration.
- **Health Project**: Represents a monitoring/health check application paired with a main web app project, used for observability and uptime monitoring.
- **Design System**: Shared package containing reusable UI components, styles, and design tokens used by all three web app projects.
- **Terraform Backend**: Shared S3 state storage (genesis backend) that stores state files for all three products with workspace-scoped isolation.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: Both private and spaces web apps have complete project scaffolding in place (infrastructure, Terraform, application structure) matching the current web app layout.
- **SC-002**: All Terraform workspaces (staging, common_resources, production) for both new web apps initialize and validate without errors.
- **SC-003**: Both new web apps build successfully without errors and can be run locally.
- **SC-004**: Both new web apps correctly load and render components from the shared design system.
- **SC-005**: Terraform backend state for both new web apps is properly configured and isolated using the same genesis S3 backend.
- **SC-006**: Health/monitoring projects for both new web apps are created and deployable.
- **SC-007**: GitHub Actions CI/CD workflows exist for both new web apps and pass all lint/validation checks.
- **SC-008**: A developer can navigate the three web app projects and immediately understand the structure due to consistency across all three.
- **SC-009**: Deployment of a single workspace (infrastructure + app + health project) completes in < 5 minutes, measured end-to-end from CI trigger to production availability.

## Assumptions

- **Org. Structure**: The organization is ready to manage three separate web app products with independent development, deployment, and operational cycles.
- **Design System**: The shared design system package is stable, versioned, and accessible to all three web apps via the existing package management setup. All three apps MUST always use the same design system version to ensure consistent UX across products.
- **Deployment Independence**: Each web app product (`webapp`, `webapp-teams`, `webapp-spaces`) deploys independently on its own release cycle with no inter-product deployment dependencies or deployment ordering requirements.
- **Deployment Performance**: CI/CD pipeline infrastructure (GitHub Actions runners, Terraform execution environment, build cache, Vercel resources) can support < 5 minute end-to-end deployments for workspace infrastructure and app.
- **Terraform Backend**: The existing genesis S3 backend can support workspace isolation for three separate products without configuration changes.
- **Authentication**: A shared authentication/authorization mechanism already exists or will be implemented separately (out of scope for scaffolding).
- **Domain/DNS**: Domain configuration and DNS routing for three separate products will be handled by ops/DevOps (out of scope for scaffolding).
- **CI/CD Patterns**: Existing GitHub Actions patterns (lint-validate-infrastructure, build, deploy) can be replicated for the new web apps.
- **Local Development**: All developers have the same local development setup tools (Node.js, Terraform, etc.) as the current web app requires.
- **Future Separation**: This specification covers Phase 1 (scaffolding only). Scaffolding creates the foundation for eventual separation of teams and spaces functionality, but actual feature extraction and refactoring is deferred to Phase 2 after the scaffolding is production-ready and validated.
- **No Breaking Changes**: The shared design system and current web app will remain stable during the scaffolding phase so new apps can inherit patterns reliably.
