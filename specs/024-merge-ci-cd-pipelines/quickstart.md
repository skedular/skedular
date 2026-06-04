# Quickstart: Merge CI/CD Pipelines

## Goal

Validate that Skedular has one consolidated CI/CD pipeline that uses the DSST CI pattern for change detection and conditional validation, then adds Skedular CD jobs after successful CI.

## Prerequisites

- Work on branch `024-merge-ci-cd-pipelines`.
- Keep DSST as a reference only; do not copy DSST versioning or package publishing logic.
- Preserve existing Skedular reusable actions:
  - `.github/actions/build-test-push`
  - `.github/actions/lint-validate-infrastructure`
  - `.github/actions/deploy-infrastructure`
- Preserve existing GitHub environment protections for `staging` and `production`.

## Expected Workflow Shape

1. One consolidated workflow exists at `.github/workflows/skedular-cicd-pipeline.yml`.
2. Existing application CI/CD workflows are retired or disabled after parity is verified.
3. `detect` always runs and emits trigger group outputs.
4. CI jobs run only for selected trigger groups.
5. CD jobs depend on matching CI jobs.
6. Pull requests never deploy.
7. Pushes to `main` may deploy selected staging surfaces after CI succeeds.
8. Production deploys only through existing production gates and approvals.
9. Manual runs force full CI validation; deployment jobs remain blocked unless the event is a push to `main`.

## Validation Matrix

### Documentation-only change

Change only a markdown/spec file, such as a file under `specs/`.

Expected result:

- Consolidated pipeline runs.
- `detect` marks docs-only.
- Build/test/deploy jobs skip.
- Required umbrella result succeeds.

Validation status: pending GitHub Actions run after workflow merge.

### Single web app change

Change a file under `src/web/apps/webapp-teams/`.

Expected pull request result:

- Webapp Teams CI runs.
- Web workspace dependencies run only if selected by changed paths or fan-out.
- Unrelated web app builds skip.
- Terraform validation for Webapp Teams staging/production workspaces runs if infrastructure paths are selected by the implementation's web app policy.
- No staging or production deploy job runs.

Expected `main` push result:

- Selected CI runs first.
- Matching staging CD job is eligible after CI succeeds.
- Production CD remains behind existing production gates.

Validation status: pending GitHub Actions run after workflow merge.

### Web package change

Change a file under `src/web/packages/`.

Expected result:

- All web app CI segments run because shared web packages fan out to every product app.
- Unrelated backend-only image jobs skip unless selected by another trigger.
- Web infrastructure validation/deploy eligibility follows current workflow dependency rules.

Validation status: pending GitHub Actions run after workflow merge.

### Backend shared change

Change a file under `src/shared/`.

Expected result:

- Backend/all-in-one CI segments run.
- Domain/shared infrastructure validation segments that currently include shared paths run.
- Web app CI runs only if the changed shared path is also part of a documented web dependency or pipeline fan-out rule.

Validation status: pending GitHub Actions run after workflow merge.

### API definitions change

Change a file under `api-definitions/`.

Expected result:

- Backend/all-in-one CI validation runs.
- Any generated client/schema validation affected by the contract change is selected.
- The plan/tasks identify required generator validation before implementation completes.
- No package versioning or tag-based publishing is introduced.

Validation status: pending GitHub Actions run after workflow merge.

### Pipeline/action change

Change a file under `.github/workflows/` or `.github/actions/`.

Expected result:

- All CI validation groups run to self-validate pipeline behavior.
- CD jobs still obey event policy: no PR deploys; `main` can deploy staging after CI; production stays gated.

Validation status: pending GitHub Actions run after workflow merge.

### Domain infrastructure change

Change a file under `src/organization/shared/infrastructure/`.

Expected pull request result:

- Organization shared infrastructure Terraform validation runs for staging and production workspaces.
- Deploy jobs skip because pull requests validate only.

Expected `main` push result:

- Organization shared infrastructure CI validation runs.
- Organization staging deployment is eligible after validation succeeds.
- Organization production deployment uses the existing production environment gate.

Validation status: pending GitHub Actions run after workflow merge.

### Manual run

Start the workflow with `workflow_dispatch`.

Expected result:

- Manual full-run mode is visible in the summary.
- All CI validation groups run.
- CD jobs are selected for summary/deployment-readiness visibility but remain skipped by event policy because manual runs are not `push` events.

Validation status: pending GitHub Actions run after workflow merge.

## Validation Commands

- Workflow lint: `actionlint .github/workflows/skedular-cicd-pipeline.yml`
- Repository lint: `make lint`
- Matrix validation: execute or simulate each scenario in this file and record the resulting selected/skipped CI/CD segments.

## Implementation Validation Log

| Check                              | Result  | Notes                                                                                                                                                               |
| ---------------------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Workflow inventory complete        | PASS    | All 19 retired workflow files are mapped in `workflow-inventory.md`, including `web-shared.yml`.                                                                    |
| Composite action inputs documented | PASS    | Existing build, Terraform validation, deploy, and build-test action inputs are listed in `workflow-inventory.md`.                                                   |
| Manual CD policy documented        | PASS    | Manual runs force full CI validation but CD jobs remain blocked unless the event is a push to `main`.                                                               |
| Workflow lint                      | PASS    | `.github/workflows/skedular-cicd-pipeline.yml` parsed as YAML and passed pinned `actionlint` v1.7.4.                                                                |
| `make lint`                        | BLOCKED | Local run failed before repository checks because `dotnet` is not available on this machine's PATH.                                                                 |
| Representative path matrix         | PASS    | Local simulation covered docs-only, single web app, web package, backend shared, API definitions, pipeline/action, domain infrastructure, and manual-run scenarios. |

## Local Path Matrix Simulation

| Scenario              | Simulated result                                                                                                          |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Documentation-only    | `docs_only=true`; no CI/CD matrices selected.                                                                             |
| Single web app        | `webapp-teams` web app build selected only.                                                                               |
| Web package           | All web app builds and shared web infrastructure validation selected.                                                     |
| Backend shared        | All-in-one image builds, shared infrastructure, shared Azure Entra, and domain shared infrastructure validation selected. |
| API definitions       | All-in-one image builds selected; DSST versioning/package publishing remains excluded.                                    |
| Pipeline/action       | All CI validation and all infrastructure validation selected for self-validation.                                         |
| Domain infrastructure | All-in-one image builds and the matching domain infrastructure validation selected, matching retired workflow coverage.   |
| Manual                | Full CI validation path selected; CD remains blocked unless the event is a push to `main`.                                |

## Review Checklist

- [ ] Exactly one application CI/CD workflow is active for the consolidated behavior.
- [ ] Non-CI/CD maintenance workflows are either documented as out of scope or intentionally retained.
- [ ] Every old workflow path filter maps to a trigger group or documented fan-out rule.
- [ ] Every old build/test job maps to a CI segment.
- [ ] Every old staging deploy maps to a post-CI staging CD segment.
- [ ] Every old production deploy maps to a post-CI production CD segment with existing gates.
- [ ] Pull request runs cannot deploy.
- [ ] Docs-only runs produce a passing umbrella result without builds.
- [ ] Pipeline summary explains run/skip decisions.
