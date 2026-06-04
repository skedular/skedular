# Implementation Plan: Merge CI/CD Pipelines

**Branch**: `024-merge-ci-cd-pipelines` | **Date**: 2026-06-04 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/024-merge-ci-cd-pipelines/spec.md`

## Summary

Merge Skedular's current CI/CD workflow set into one folder-aware GitHub Actions workflow. The plan uses the DSST single-pipeline CI pattern as the reference for change detection, conditional fan-out, docs-only skipping, manual full runs, and one umbrella status check. Skedular must add the CD layer after selected CI checks pass: pull requests validate only, pushes to `main` may deploy affected staging surfaces, and production deploys only through the existing production gates and environment protections.

## Technical Context

**Language/Version**: GitHub Actions YAML on `ubuntu-latest`; Bash shell in existing Skedular composite actions; Terraform HCL for infrastructure workspaces; Dockerfile-based app builds  
**Primary Dependencies**: GitHub Actions, `actions/checkout@v6`, Docker BuildKit, `docker/metadata-action`, `docker/login-action`, `docker/build-push-action`, `hashicorp/setup-terraform@v4`, `actions/github-script`, existing `.github/actions/build-test-push`, `.github/actions/lint-validate-infrastructure`, `.github/actions/deploy-infrastructure`  
**Storage**: N/A for application data; workflow-local changed-file lists, coverage outputs, Docker image tags, and Terraform state backends already configured by current workspaces  
**Testing**: Workflow validation through representative change matrix, existing `make lint`, Terraform `fmt/init/validate/plan`, Docker target `test` builds, and GitHub Actions dry-run/review validation where available  
**Target Platform**: GitHub Actions hosted Linux runners for the Skedular monorepo  
**Project Type**: Monorepo CI/CD pipeline consolidation  
**Performance Goals**: Documentation/spec-only changes complete without build/deploy jobs; isolated product/domain changes avoid unrelated build jobs; manual runs execute full CI validation intentionally  
**Constraints**: Do not introduce DSST versioning or package publishing; preserve existing staging and production environment protections; pull requests must never deploy; selected CD must run only after relevant CI succeeds  
**Scale/Scope**: Consolidates 19 existing Skedular workflow files into one application CI/CD workflow, while allowing documented non-CI/CD maintenance workflows to remain separate if needed

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature does not change `api-definitions/` contracts or generated application surfaces. It must classify `api-definitions/**` changes as broad CI triggers and must not hand-edit generated files.
- [x] **II. Domain Boundaries** — The feature crosses domain folders only for CI/CD orchestration. It does not read domain databases or internal runtime classes. Existing public build/deploy surfaces and infrastructure workspaces remain the boundary.
- [x] **III. Testing** — Pipeline behavior requires workflow-level validation through representative path-change cases, action linting/`make lint`, Terraform validation, and Docker test-target builds. No persistence tests or repository-layer assertions are required because no application persistence behavior changes.
- [x] **IV. Frontend** — The feature builds and deploys web apps but does not change frontend source behavior, Relay artifacts, typography usage, or UI copy. Existing web build/test behavior must remain covered by the selected CI jobs.
- [x] **V. Pattern Consistency** — This introduces a new repository-level orchestration pattern by consolidating many workflow files into one. Justification: the user requested a single folder-aware pipeline, and the DSST workspace provides a proven local reference for single-workflow CI orchestration. Existing Skedular composite actions remain the implementation pattern for build/test and deploy steps.
- [x] **VI. Logging** — Pipeline observability is explicitly planned through a `detect` summary and job logs covering changed files, trigger groups, fan-out decisions, selected/skipped CI segments, selected/skipped CD segments, docs-only decisions, manual full-run decisions, and failure paths without leaking secrets.

## Project Structure

### Documentation (this feature)

```text
specs/024-merge-ci-cd-pipelines/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── consolidated-pipeline-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
.github/
├── actions/
│   ├── build-test-push/
│   ├── deploy-infrastructure/
│   └── lint-validate-infrastructure/
└── workflows/
    ├── skedular-cicd-pipeline.yml
    └── [retired or documented non-CI/CD workflows]

api-definitions/
├── events/
├── graphql/
├── grpc/
└── openapi/

docs/
└── event-catalog/

src/
├── all-in-one/
├── booking/
├── core/
├── customer/
├── gateway/
├── location/
├── marketplace/
├── msteams/
├── organization/
├── shared/
├── slack/
├── system/
├── team/
└── web/
    ├── apps/
    │   ├── public-web/
    │   ├── webapp/
    │   ├── webapp-help/
    │   ├── webapp-spaces/
    │   ├── webapp-spaces-help/
    │   ├── webapp-teams/
    │   └── webapp-teams-help/
    ├── infrastructure/
    └── packages/
```

**Structure Decision**: Use a single GitHub Actions workflow as the orchestration boundary and keep existing composite actions as reusable implementation units. The consolidated workflow owns change detection, trigger outputs, CI job selection, and post-CI CD job selection. App, docs, and infrastructure directories remain unchanged.

## Phase 0: Research Summary

Research decisions are captured in [research.md](research.md):

- Use DSST as the CI orchestration reference, then add Skedular CD after CI succeeds.
- Model folder awareness through named trigger groups.
- Preserve existing reusable action contracts.
- Exclude versioning and package release semantics.
- Implement Option A deployment behavior as post-CI CD jobs.
- Treat documentation/spec-only changes as successful no-build runs.
- Trigger broad validation for pipeline/action changes.
- Use workflow-run summaries as the primary audit surface.

## Phase 1: Design Summary

Design artifacts are captured in:

- [data-model.md](data-model.md) — change sets, trigger groups, pipeline segments, CD segments, and summaries.
- [contracts/consolidated-pipeline-contract.md](contracts/consolidated-pipeline-contract.md) — workflow events, detect outputs, CI segment families, CD segment policies, trigger group contract, and summary contract.
- [quickstart.md](quickstart.md) — representative validation matrix for docs-only, web app, web package, backend shared, API definitions, pipeline/action, infrastructure, and manual runs.

## Implementation Approach

1. Inventory every current Skedular CI/CD workflow and map each path filter, build job, validation job, staging deployment, and production deployment to a trigger group and segment.
2. Create `.github/workflows/skedular-cicd-pipeline.yml` with global triggers for `workflow_dispatch`, pull requests to `main`, and pushes to `main`.
3. Add `detect` as the first job, modeled after DSST's CI detection behavior but without DSST tag/version paths.
4. Emit boolean outputs for global groups and concrete deployable targets.
5. Apply fan-out rules for shared backend, API definitions, shared web packages, shared infrastructure, and pipeline/action changes.
6. Add CI jobs that reuse existing Docker and Terraform validation actions and run only when selected by `detect`.
7. Add CD staging jobs that depend on matching CI jobs, run only for pushes to `main`, and reuse existing staging Terraform workspaces.
8. Add CD production jobs that depend on matching CI jobs and preserve existing production environments, approvals, and gates.
9. Add a required umbrella/summary job that reports selected/skipped segments and fails if any selected CI/CD segment fails.
10. Retire or disable old application CI/CD workflows after parity validation, documenting any retained non-CI/CD maintenance workflows.

## Complexity Tracking

No constitution violations require exception tracking.

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --------- | ---------- | ------------------------------------ |
| N/A       | N/A        | N/A                                  |

## Post-Design Constitution Check

- [x] **I. Contract-First** — No contract files are changed by the plan. Contract path changes are represented as trigger inputs only.
- [x] **II. Domain Boundaries** — Domain boundaries remain intact; CI/CD orchestrates existing folders and actions only.
- [x] **III. Testing** — Quickstart defines representative workflow validation cases; tasks must include action/workflow linting plus path matrix validation.
- [x] **IV. Frontend** — No frontend code changes are planned; web app CI/CD remains folder-aware through existing Docker/Terraform paths.
- [x] **V. Pattern Consistency** — The new single-workflow orchestration pattern is justified by the feature request and DSST CI reference; existing composite actions are reused.
- [x] **VI. Logging** — The detect and summary contracts include structured workflow audit output for run/skip/fan-out/CD decisions.
