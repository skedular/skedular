# Data Model: Split UI into Three Products

**Date**: 2026-04-18  
**Related**: [plan.md](plan.md), [research.md](research.md)

## Entity Definitions

### Entity 1: Web Application Project (Webapp)

**Description**: A complete, independently deployable web application with infrastructure-as-code, application source code, CI/CD pipelines, and configuration for a single product variant (current, private, or marketplace).

**Attributes**:

- `id`: String (project identifier: `webapp`, `webapp-teams`, `webapp-spaces`)
- `display_name`: String (human-readable name: "Current Web App", "teams web app", "spaces web app")
- `description`: String (purpose of the product variant)
- `created_date`: Date (when project was scaffolded)
- `status`: Enum (active, archived, maintenance)

**Key Fields**:

- `root_path`: String (absolute path to project root, e.g., `web/apps/webapp-teams/`)
- `infrastructure_path`: String (e.g., `web/apps/webapp-teams/infrastructure/`)
- `source_path`: String (e.g., `web/apps/webapp-teams/src/`)
- `design_system_version`: String (pinned design system version; all three apps must use same version)
- `terraform_backend_bucket`: String (S3 bucket for state)
- `terraform_backend_key_prefix`: String (key prefix in S3, e.g., `webapp-teams/`)

**Relationships**:

- Has Many: `TerraformWorkspace` (3 workspaces per app: staging, common_resources, production)
- Has One: `HealthProject` (paired monitoring project)
- Depends On: `DesignSystem` (shared package dependency, always latest version)
- Depends On: `GitHubActionsPipeline` (CI/CD workflows)

**State Transitions**:

- `scaffolded` → `building` (first local build)
- `building` → `ready_for_deployment` (build successful, Terraform validated)
- `ready_for_deployment` → `deployed` (infrastructure created in staging/production)
- `deployed` → `active` (health checks passing)
- All states → `archived` (end-of-life)

**Validation Rules**:

- `id` must match pattern: `^webapp(-private|-marketplace)?$`
- `design_system_version` must match version of other two webapps (constraint enforced)
- All three workspaces (staging, common_resources, production) must exist and validate
- S3 backend bucket must be accessible and have appropriate IAM permissions
- GitHub Actions workflows must exist and pass linting

---

### Entity 2: Terraform Workspace

**Description**: An isolated Terraform execution environment within a web application project, representing a specific stage of infrastructure (staging, common_resources, or production).

**Attributes**:

- `id`: String (composite: `{webapp_id}/{environment}`)
- `environment`: Enum (staging, common_resources, production)
- `description`: String (purpose of this environment)
- `created_date`: Date (when created)
- `last_validated`: Date (last successful terraform validate)
- `terraform_version`: String (e.g., "1.6.0")

**Key Fields**:

- `workspace_path`: String (path to workspace directory, e.g., `infrastructure/workspaces/staging/`)
- `terraform_config_file`: String (usually `terraform.tf` in workspace directory)
- `backend_key`: String (S3 object key for state file, e.g., `webapp-teams/staging/terraform.tfstate`)
- `backend_lock_table`: String (DynamoDB table for state locking)
- `terraform_var_file`: String (path to `.tfvars` or equivalent, if exists)

**Relationships**:

- Belongs To: `WebApplication`
- Contains: `TerraformModule` (collection of modules used in this workspace)
- Produces: `TerraformState` (managed state file in S3)

**State Transitions**:

- `not_initialized` → `init_pending` (ready to run terraform init)
- `init_pending` → `initialized` (terraform init successful)
- `initialized` → `plan_pending` (ready to run terraform plan)
- `plan_pending` → `planned` (terraform plan successful, awaiting apply)
- `planned` → `apply_pending` (human approval to apply)
- `apply_pending` → `applied` (terraform apply successful, infrastructure created)
- `applied` → `drift_detected` (terraform plan shows changes, drift needs fixing)
- All states → `destroyed` (infrastructure torn down)

**Validation Rules**:

- `environment` must be one of: staging, common_resources, production (fixed set)
- `backend_key` must follow pattern: `{webapp_id}/{environment}/terraform.tfstate`
- All providers specified in `terraform.tf` must be available (e.g., hashicorp/aws ~> 6.0)
- No references to hardcoded paths (all paths must be relative or interpolated)
- `terraform validate` must pass before state is considered valid

---

### Entity 3: Health Project

**Description**: A companion monitoring/health check application paired with each main web application project. Monitors uptime, performance, and basic functionality of the main web app.

**Attributes**:

- `id`: String (project identifier: `webapp-help`, `webapp-teams-help`, `webapp-spaces-help`)
- `display_name`: String (human-readable name)
- `description`: String (monitoring purpose and scope)
- `created_date`: Date
- `status`: Enum (active, archived, maintenance)

**Key Fields**:

- `root_path`: String (path to health project root)
- `infrastructure_path`: String (Terraform config for monitoring infrastructure)
- `source_path`: String (health check endpoint implementations)
- `main_app_id`: String (reference to the associated main webapp)
- `health_check_interval`: Integer (seconds between health checks, e.g., 300 for 5-minute intervals)
- `alert_threshold`: Integer (consecutive failures before alert)

**Relationships**:

- Belongs To: `WebApplication` (main app it monitors)
- Depends On: `DesignSystem` (if UI components used, same version as main app)
- Depends On: `GitHubActionsPipeline` (CI/CD for health app)

**Health Check Types**:

- `ping`: Simple HTTP GET to main app endpoint
- `component_render`: Verify shared design system components render
- `api_connectivity`: Verify backend API connectivity
- `database_connectivity`: Verify database access (if applicable)

**Validation Rules**:

- `id` must follow pattern: `^webapp(-private|-marketplace)?-help$`
- `main_app_id` must reference a valid existing WebApplication
- All health check endpoints must respond within SLA (e.g., < 2 seconds)
- Health project must deploy successfully before main app is considered "active"

---

### Entity 4: Design System

**Description**: Shared package of reusable UI components, styles, and design tokens. All three web applications depend on the same version to ensure consistent UX.

**Attributes**:

- `package_name`: String (npm package ID, e.g., `@skedular/design-system`)
- `current_version`: String (semver, e.g., `1.0.0`)
- `latest_available_version`: String (current latest published version)
- `status`: Enum (stable, beta, rc, deprecated)
- `last_updated`: Date (publication date of current version)

**Key Fields**:

- `npm_registry_url`: String (registry where package is published)
- `documentation_url`: String (link to design system documentation)
- `github_repo`: String (source repository)
- `components`: Array of Strings (list of exported component names, e.g., `["Button", "TextField", "Card"]`)
- `breaking_changes`: Array (breaking changes in recent releases)

**Relationships**:

- Used By: `WebApplication` (all three apps depend on this)
- Has Many: `Component` (individual UI components exported by the package)

**Versioning Strategy**:

- All three web applications (`webapp`, `webapp-teams`, `webapp-spaces`) MUST use the same design system version
- Version updates are coordinated across all three apps to prevent divergence
- When new version is released, all three apps must update together

**Validation Rules**:

- Package must be available on npm registry
- All three web apps must reference the same version in package.json
- Components must be importable and render without errors in all three apps

---

### Entity 5: GitHub Actions Pipeline

**Description**: Continuous Integration and Continuous Deployment (CI/CD) workflows that automate testing, building, and deploying the web applications and infrastructure.

**Attributes**:

- `id`: String (composite: `{webapp_id}/{workflow_name}`)
- `workflow_name`: String (e.g., "lint-validate-infrastructure", "build-deploy")
- `description`: String (purpose of this workflow)
- `status`: Enum (active, disabled, deprecated)

**Key Fields**:

- `workflow_file_path`: String (path in repo, e.g., `.github/workflows/lint-validate-infrastructure.yml`)
- `trigger_events`: Array of Strings (what triggers the workflow, e.g., ["push", "pull_request"])
- `trigger_branches`: Array of Strings (which branches trigger the workflow, e.g., `["main"]`)
- `concurrent_jobs`: Integer (number of jobs that run in parallel)
- `estimated_duration_seconds`: Integer (typical execution time)
- `failure_notification_channel`: String (where failures are reported, e.g., Slack channel)

**Relationships**:

- Belongs To: `WebApplication` (workflow for this specific app)
- Contains: `WorkflowJob` (collection of jobs in the workflow)
- Produces: `DeploymentArtifact` (build outputs, logs, etc.)

**Workflow Types**:

- `lint-validate-infrastructure`: Terraform linting, validation
- `build-deploy`: Application build, testing, deployment to Vercel
- `health-check`: Verification of health project endpoints

**State Transitions**:

- `configured` → `running` (triggered by event)
- `running` → `success` (all jobs pass)
- `running` → `failure` (one or more jobs fail)
- `failure` → `retry_pending` (awaiting manual retry or auto-retry)
- `retry_pending` → `running` (workflow retried)
- `success` / `failure` → `archived` (old runs archived)

**Validation Rules**:

- YAML syntax must be valid
- All referenced secrets must exist in GitHub repo
- Referenced actions must be publicly available or internal
- Workflow must not expose sensitive credentials in logs

---

### Entity 6: Deployment Artifact

**Description**: Output artifacts produced by CI/CD workflows, including built applications, logs, and deployment records.

**Attributes**:

- `id`: String (composite: `{run_id}/{artifact_name}`)
- `artifact_type`: Enum (application_build, terraform_plan, deployment_log, test_report)
- `created_date`: Date (when artifact was produced)
- `size_bytes`: Integer (artifact size on disk/storage)
- `status`: Enum (available, archived, expired)

**Key Fields**:

- `storage_location`: String (S3 path or artifact store reference)
- `checksum`: String (SHA256 hash for integrity verification)
- `workflow_run_id`: String (GitHub Actions run that produced this artifact)
- `deployment_target`: String (where deployed: staging, production, etc.)

**Relationships**:

- Produced By: `GitHubActionsPipeline`
- Associated With: `WebApplication`
- Associated With: `TerraformWorkspace` (if infrastructure artifact)

**Retention Policy**:

- Build artifacts: 90 days
- Logs: 30 days
- Terraform plans: 7 days (after apply)

---

## Relationships & State Machine Diagram

```
WebApplication
  ├─ Has Many (1:M) ──> TerraformWorkspace
  ├─ Has One (1:1) ──> HealthProject
  ├─ Depends On (N:1) ──> DesignSystem [ALL THREE APPS → SAME VERSION]
  ├─ Has Many (1:M) ──> GitHubActionsPipeline
  └─ Produces (1:M) ──> DeploymentArtifact

TerraformWorkspace
  ├─ Belongs To (M:1) ──> WebApplication
  ├─ Contains (1:M) ──> TerraformModule
  └─ Produces (1:1) ──> TerraformState [in S3]

HealthProject
  ├─ Belongs To (M:1) ──> WebApplication
  └─ Depends On (N:1) ──> DesignSystem [SAME VERSION AS MAIN APP]

GitHubActionsPipeline
  ├─ Belongs To (M:1) ──> WebApplication
  ├─ Contains (1:M) ──> WorkflowJob
  └─ Produces (1:M) ──> DeploymentArtifact

DesignSystem
  └─ Used By (1:N) ──> WebApplication [CONSTRAINT: ALL USE SAME VERSION]
```

---

## Key Constraints

1. **Design System Version Alignment**: All three webapps MUST use the same design system version at all times. This is enforced in package-lock files and verified during builds.

2. **Terraform State Isolation**: Each workspace has its own state file in S3 with distinct key paths:
   - `s3://bucket/webapp/staging/terraform.tfstate`
   - `s3://bucket/webapp-teams/staging/terraform.tfstate`
   - `s3://bucket/webapp-spaces/staging/terraform.tfstate`

3. **Independent Deployment Pipelines**: Each webapp has its own GitHub Actions workflows with no dependencies on other webapps. Deployments can run in parallel.

4. **Health Project Dependency**: Each health project MUST verify the main app before reporting "healthy". Health project failures do not block main app deployment but are logged and alerted.

5. **Workspace Consistency**: All three webapps MUST have the same three environments (staging, common_resources, production) for parity and operational consistency.

---

## Success Criteria (Data Model)

- [x] All entities defined with clear attributes and relationships
- [x] State transitions documented for key entities (Webapp, Workspace, Pipeline)
- [x] Validation rules specified for each entity
- [x] Relationships modeled correctly (1:1, 1:M, N:1, M:N as applicable)
- [x] Key constraints documented (design system version pinning, state isolation, etc.)
- [x] Three webapp instances can be instantiated from this model with no ambiguity
- [x] Health projects correctly modeled as companions to webapps
- [x] GitHub Actions pipelines correctly modeled as infrastructure
