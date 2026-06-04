# Contract: Consolidated Skedular CI/CD Pipeline

## Workflow Contract

The repository exposes one consolidated CI/CD workflow for application and infrastructure validation/deployment.

**Workflow file**: `.github/workflows/skedular-cicd-pipeline.yml`

**Supported events**:

- `workflow_dispatch`
- `pull_request` targeting `main`
- `push` to `main`

**Out of scope**:

- DSST-style package versioning
- Tag-driven package publish flows
- NuGet/npm package release calculation

## Required Jobs

### `detect`

Always runs first.

**Inputs**:

- GitHub event payload
- Repository checkout with enough history for diffing

**Outputs**:

- `pipeline_changed`
- `shared_backend_changed`
- `api_definitions_changed`
- `all_in_one_changed`
- `backend_domain_changed`
- `shared_infrastructure_changed`
- `domain_infrastructure_changed`
- `web_workspace_changed`
- `web_app_changed`
- `web_infrastructure_changed`
- `docs_event_catalog_changed`
- `docs_only_changed`
- one output per concrete deployable web app and infrastructure target when implementation needs finer job selection

**Behavior**:

- Resolves changed files for pull request and push runs.
- Forces all CI trigger groups active for manual runs.
- Applies docs-only override only when every changed file is documentation/specification/agent-instruction content.
- Applies dependency fan-out before emitting outputs.
- Emits a GitHub Actions summary with changed files, trigger groups, fan-out reasons, selected CI segments, selected CD segments, and skipped segments.

### CI Build and Validation Jobs

Run only when selected by `detect` outputs.

**Required segment families**:

- Global lint validation equivalent to current `lint.yml` behavior.
- All-in-one image build/test jobs for existing `allapis`, `allprocessors`, `alljobs`, and `allinfra` repositories.
- Web app image build/test jobs for `webapp`, `webapp-help`, `webapp-teams`, `webapp-teams-help`, `webapp-spaces`, `webapp-spaces-help`, and any current public web workflow if present during implementation.
- Docs event catalog image build/test validation.
- Terraform format/init/validate/plan validation for staging and production workspaces currently covered by the individual workflows.

**Event policy**:

- Pull requests: validation only, no deployment.
- Pushes to `main`: validation before eligible CD.
- Manual runs: full CI validation path.

**Failure policy**:

- Any selected CI job failure fails the umbrella pipeline result.
- Unselected skipped jobs do not fail the umbrella pipeline result.

### CD Staging Jobs

Run after matching CI succeeds.

**Selection policy**:

- Selected only for changed deployable surfaces or dependency fan-out surfaces.
- Never runs on pull request events.
- May run on pushes to `main` after matching CI succeeds.

**Behavior**:

- Reuses `.github/actions/deploy-infrastructure` for Terraform apply.
- Uses the existing staging Terraform working directory for each selected deployable surface.
- Uses GitHub `staging` environment semantics already present in the repository workflows.

### CD Production Jobs

Run after matching CI succeeds and only through existing production gates.

**Selection policy**:

- Selected only for changed deployable surfaces or dependency fan-out surfaces.
- Never runs on pull request events.
- Must preserve existing production environment protections and approval conditions.

**Behavior**:

- Reuses `.github/actions/deploy-infrastructure` for Terraform apply.
- Uses the existing production Terraform working directory for each selected deployable surface.
- Uses GitHub `production` environment semantics already present in the repository workflows.

## Trigger Group Contract

### Global fan-out groups

| Trigger group                   | Activating paths                                                           | Required fan-out                                                                                              |
| ------------------------------- | -------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- |
| `pipeline_changed`              | `.github/workflows/**`, `.github/actions/**`, `.github/prompts/**`         | All CI validation segments                                                                                    |
| `shared_backend_changed`        | `src/shared/**` excluding docs-only cases                                  | Backend/all-in-one and domain/shared infrastructure validation segments currently triggered by shared changes |
| `api_definitions_changed`       | `api-definitions/**`, generation scripts                                   | Backend/all-in-one and any generated client/schema validation affected by contracts                           |
| `web_workspace_changed`         | `src/web/package.json`, lockfile, workspace file, `src/web/packages/**`    | All web app CI validation and web infrastructure validation                                                   |
| `shared_infrastructure_changed` | `src/shared/infrastructure/**`, `src/shared/infrastructure-azure-entra/**` | All infrastructure validation/deploy segments that currently include shared infrastructure paths              |

### Deployable app groups

| Trigger group                | Primary path                         | CI segment                                        | CD segment                                           |
| ---------------------------- | ------------------------------------ | ------------------------------------------------- | ---------------------------------------------------- |
| `webapp_changed`             | `src/web/apps/webapp/**`             | Build/test webapp image and validate webapp infra | Staging on `main`, production through existing gates |
| `webapp_help_changed`        | `src/web/apps/webapp-help/**`        | Build/test help image and validate infra          | Staging on `main`, production through existing gates |
| `webapp_teams_changed`       | `src/web/apps/webapp-teams/**`       | Build/test Teams image and validate infra         | Staging on `main`, production through existing gates |
| `webapp_teams_help_changed`  | `src/web/apps/webapp-teams-help/**`  | Build/test Teams help image and validate infra    | Staging on `main`, production through existing gates |
| `webapp_spaces_changed`      | `src/web/apps/webapp-spaces/**`      | Build/test Spaces image and validate infra        | Staging on `main`, production through existing gates |
| `webapp_spaces_help_changed` | `src/web/apps/webapp-spaces-help/**` | Build/test Spaces help image and validate infra   | Staging on `main`, production through existing gates |
| `docs_event_catalog_changed` | `docs/event-catalog/**`              | Build/test docs catalog image and validate infra  | Staging on `main`, production through existing gates |

### Backend and infrastructure groups

| Trigger group                       | Primary path                                   | CI segment                                                  | CD segment                                                 |
| ----------------------------------- | ---------------------------------------------- | ----------------------------------------------------------- | ---------------------------------------------------------- |
| `all_in_one_changed`                | `src/all-in-one/**` plus backend fan-out paths | Build/test allapis, allprocessors, alljobs, allinfra images | No Terraform CD unless existing workflow target defines it |
| `booking_shared_infra_changed`      | `src/booking/shared/infrastructure/**`         | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `customer_shared_infra_changed`     | `src/customer/shared/infrastructure/**`        | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `location_shared_infra_changed`     | `src/location/shared/infrastructure/**`        | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `msteams_shared_infra_changed`      | `src/msteams/shared/infrastructure/**`         | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `organization_shared_infra_changed` | `src/organization/shared/infrastructure/**`    | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `slack_shared_infra_changed`        | `src/slack/shared/infrastructure/**`           | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `team_shared_infra_changed`         | `src/team/shared/infrastructure/**`            | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `shared_infra_changed`              | `src/shared/infrastructure/**`                 | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |
| `shared_azure_entra_changed`        | `src/shared/infrastructure-azure-entra/**`     | Validate staging/production Terraform                       | Staging on `main`, production through existing gates       |

## Summary Contract

Every workflow run writes a summary containing:

- Event name and branch/ref.
- Base SHA and head SHA or manual-run marker.
- Changed file count and sample/list of changed files.
- Active trigger groups with reasons.
- Inactive trigger groups.
- Docs-only decision.
- Manual full-run decision.
- CI jobs selected and skipped.
- CD jobs selected, blocked by event policy, waiting for approval, or skipped.

The summary must not include secrets, tokens, Terraform state values, or sensitive payload data.
