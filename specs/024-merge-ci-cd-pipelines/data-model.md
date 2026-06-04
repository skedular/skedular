# Data Model: Merge CI/CD Pipelines

## Change Set

Represents the files changed for a pipeline run.

**Fields**:

- `eventName`: GitHub event that started the run (`pull_request`, `push`, or `workflow_dispatch`).
- `baseSha`: SHA used as the diff base for pull requests and pushes.
- `headSha`: SHA used as the diff head.
- `changedFiles`: Ordered list of changed repository-relative paths.
- `isManualFullRun`: True when `workflow_dispatch` forces all CI trigger groups active.
- `isDocsOnly`: True when all changed files are documentation, Spec Kit, markdown, or agent-instruction paths.

**Validation Rules**:

- Pull request runs use the PR base SHA as the diff base.
- Push runs use `github.event.before` as the diff base and handle first-push empty-tree cases deterministically.
- Manual runs do not depend on a diff and activate all CI trigger groups.
- Failed change-set resolution fails the required pipeline result.

## Trigger Group

Named boolean decision produced by the detection job.

**Fields**:

- `name`: Stable output name consumed by downstream jobs.
- `pathPatterns`: Repository-relative path patterns that activate the group.
- `dependsOnGroups`: Trigger groups that cause this group to become active through dependency fan-out.
- `selectedSegments`: Pipeline segments that should run when the group is active.
- `reason`: Human-readable reason emitted to logs and the run summary.

**Core Trigger Groups**:

- `pipeline_changed`: `.github/workflows/**`, `.github/actions/**`, `.github/prompts/**`, relevant Spec Kit pipeline docs.
- `shared_backend_changed`: `src/shared/**`, backend contract/generation inputs that affect backend consumers.
- `api_definitions_changed`: `api-definitions/**`, `scripts/generate-graphql.sh`, generation scripts.
- `all_in_one_changed`: `src/all-in-one/**`, backend image build surfaces.
- `backend_domain_changed`: domain folders such as `src/booking/**`, `src/customer/**`, `src/gateway/**`, `src/location/**`, `src/marketplace/**`, `src/msteams/**`, `src/organization/**`, `src/slack/**`, `src/team/**`, `src/core/**`, `src/system/**`.
- `shared_infrastructure_changed`: `src/shared/infrastructure/**`, `src/shared/infrastructure-azure-entra/**`.
- `domain_infrastructure_changed`: `src/{domain}/shared/infrastructure/**` for deployed domain infrastructure.
- `web_workspace_changed`: `src/web/package.json`, `src/web/pnpm-lock.yaml`, `src/web/pnpm-workspace.yaml`, `src/web/packages/**`.
- `web_app_changed`: individual web apps under `src/web/apps/{app}/**`.
- `web_infrastructure_changed`: `src/web/infrastructure/**` and app-specific `src/web/apps/{app}/infrastructure/**`.
- `docs_event_catalog_changed`: `docs/event-catalog/**`.
- `docs_only_changed`: `.specify/**`, `specs/**`, `docs/**` except deployable docs app paths, `*.md`, agent instruction files.

**Fan-Out Rules**:

- `pipeline_changed` activates all CI validation groups.
- `shared_backend_changed` activates backend/all-in-one and domain infrastructure validation groups that currently include shared paths.
- `api_definitions_changed` activates all backend/all-in-one CI validation and any generated-client/web validation documented in implementation.
- `web_workspace_changed` activates every web app CI segment and web infrastructure validation.
- `shared_infrastructure_changed` activates all infrastructure validation segments that currently depend on shared infrastructure.
- `docs_only_changed` deactivates all build/deploy groups after detection unless mixed with non-doc changes.

## Pipeline Segment

A build, validation, or deployment unit in the consolidated workflow.

**Fields**:

- `id`: Stable job identifier.
- `kind`: `ci-build`, `ci-validation`, `cd-staging`, `cd-production`, or `summary`.
- `triggerGroups`: Trigger groups that select the segment.
- `needs`: Upstream jobs that must complete before the segment can run.
- `eventPolicy`: Events on which the segment may run.
- `environment`: Optional GitHub environment (`staging`, `production`).
- `usesAction`: Existing reusable action or inline validation step.
- `statusBehavior`: How the segment contributes to the umbrella result.

**Validation Rules**:

- CI segments may run on pull requests, pushes to `main`, and manual runs.
- CD staging segments may run only on pushes to `main` or approved manual contexts after relevant CI succeeds.
- CD production segments may run only through existing production gates, environment protections, and approvals after relevant CI succeeds.
- Pull request runs never deploy.
- Selected segment failure fails the umbrella result.
- Skipped unselected segments do not fail the umbrella result.

## CD Segment

A deployment segment that runs after CI.

**Fields**:

- `target`: Deployed surface such as `webapp`, `webapp-teams`, `docs-event-catalog`, `shared-infrastructure`, or a domain shared infrastructure area.
- `environment`: `staging` or `production`.
- `workingDirectory`: Terraform workspace path reused from existing workflows.
- `sourceValidationSegment`: CI segment that must pass before deployment.
- `deploymentGate`: Branch, environment, and approval policy.

**State Transitions**:

1. `NotSelected`: Trigger groups do not match.
2. `WaitingForCI`: Trigger groups match, but required CI is still running.
3. `Eligible`: Required CI succeeded and event/environment policy permits deployment.
4. `WaitingForApproval`: Production environment protection requires approval.
5. `Succeeded`: Deployment action completed.
6. `Failed`: Deployment action failed and umbrella result fails.
7. `SkippedByPolicy`: Trigger groups match, but event policy prevents deployment, such as pull request validation.

## Pipeline Summary

Audit output produced by the consolidated workflow.

**Fields**:

- `changedFileCount`
- `activeTriggerGroups`
- `inactiveTriggerGroups`
- `fanOutReasons`
- `selectedCiSegments`
- `selectedCdSegments`
- `skippedSegments`
- `docsOnlyDecision`
- `manualFullRunDecision`
- `overallResult`

**Validation Rules**:

- Summary is emitted for every run, including docs-only and failed detection runs.
- Summary avoids secrets, token values, Terraform variable values, and private payloads.
