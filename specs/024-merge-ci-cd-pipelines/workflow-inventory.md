# Workflow Inventory: Merge CI/CD Pipelines

## Scope

This inventory maps the active Skedular application CI/CD workflows into the consolidated workflow at `.github/workflows/skedular-cicd-pipeline.yml`.

DSST was used only as the CI orchestration reference: detect-first execution, folder classification, docs-only skipping, manual full CI, conditional fan-out, and an umbrella required status. DSST tag/version/package publishing behavior was not copied.

## Reusable Action Contracts

| Action                                         | Purpose                                               | Inputs Used By Consolidated Workflow                                                                                                                                                                         |
| ---------------------------------------------- | ----------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `.github/actions/build-test-push`              | Docker test target, final image build, optional push  | `dockerFilePath`, `dockerBuildContextPath`, `dockerRegistry`, `dockerRegistryUsername`, `dockerRegistryPassword`, `dockerNamespace`, `dockerRepository`, `pushDockerImage`, `scanForSecurityVulnerabilities` |
| `.github/actions/lint-validate-infrastructure` | Terraform setup, fmt, init, validate, PR plan/comment | `githubToken`, `workingDirectory`, `componentName`, `environment`                                                                                                                                            |
| `.github/actions/deploy-infrastructure`        | Terraform init/apply                                  | `workingDirectory`                                                                                                                                                                                           |
| `.github/actions/build-test`                   | Docker test target only                               | Retained action, not required by the consolidated workflow because existing CI/CD workflows use `build-test-push`                                                                                            |

## Retired Workflow Mapping

| Old workflow                                | Consolidated segment(s)                                                                   | Trigger group(s)                                                                                 | CD coverage                                                                                        |
| ------------------------------------------- | ----------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------- |
| `.github/workflows/lint.yml`                | `lint`                                                                                    | all non-doc changes, manual runs                                                                 | N/A                                                                                                |
| `.github/workflows/workarounds.yml`         | `build-all-in-one` matrix for `allapis`, `allprocessors`, `alljobs`, `allinfra`           | `all_in_one_changed`, backend domain, shared backend, API definitions, pipeline, manual          | Existing workflow had no Terraform CD                                                              |
| `.github/workflows/webapp.yml`              | `build-web-apps`; `validate-infrastructure`; staging/production deploy matrices           | `webapp`, web workspace, web infrastructure, shared infrastructure, pipeline, manual             | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/webapp-help.yml`         | `build-web-apps`; `validate-infrastructure`; staging/production deploy matrices           | `webapp-help`, web workspace, web infrastructure, shared infrastructure, pipeline, manual        | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/webapp-teams.yml`        | `build-web-apps`; `validate-infrastructure`; staging/production deploy matrices           | `webapp-teams`, web workspace, web infrastructure, shared infrastructure, pipeline, manual       | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/webapp-teams-help.yml`   | `build-web-apps`; `validate-infrastructure`; staging/production deploy matrices           | `webapp-teams-help`, web workspace, web infrastructure, shared infrastructure, pipeline, manual  | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/webapp-spaces.yml`       | `build-web-apps`; `validate-infrastructure`; staging/production deploy matrices           | `webapp-spaces`, web workspace, web infrastructure, shared infrastructure, pipeline, manual      | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/webapp-spaces-help.yml`  | `build-web-apps`; `validate-infrastructure`; staging/production deploy matrices           | `webapp-spaces-help`, web workspace, web infrastructure, shared infrastructure, pipeline, manual | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/web-shared.yml`          | `validate-infrastructure`; staging/production deploy matrices                             | `web_infrastructure_changed`, shared infrastructure, pipeline, manual                            | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/docs-event-catalog.yml`  | `build-docs-event-catalog`; `validate-infrastructure`; staging/production deploy matrices | `docs_event_catalog_changed`, shared infrastructure, pipeline, manual                            | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/shared.yml`              | `validate-infrastructure`; staging/production deploy matrices                             | shared backend, shared infrastructure, pipeline, manual                                          | Preserved with AWS and Azure login steps through `staging` and `production` environments           |
| `.github/workflows/shared-azure-entra.yml`  | `validate-infrastructure`; staging/production deploy matrices                             | shared backend, shared Azure Entra, pipeline, manual                                             | Preserved with AWS and Azure no-subscription login through `staging` and `production` environments |
| `.github/workflows/booking-shared.yml`      | `validate-infrastructure`; staging/production deploy matrices                             | `src/booking/shared/**`, shared backend, pipeline, manual                                        | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/customer-shared.yml`     | `validate-infrastructure`; staging/production deploy matrices                             | `src/customer/shared/**`, shared backend, pipeline, manual                                       | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/location-shared.yml`     | `validate-infrastructure`; staging/production deploy matrices                             | `src/location/shared/**`, shared backend, pipeline, manual                                       | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/msteams-shared.yml`      | `validate-infrastructure`; staging/production deploy matrices                             | `src/msteams/shared/**`, shared backend, pipeline, manual                                        | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/organization-shared.yml` | `validate-infrastructure`; staging/production deploy matrices                             | `src/organization/shared/**`, shared backend, pipeline, manual                                   | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/slack-shared.yml`        | `validate-infrastructure`; staging/production deploy matrices                             | `src/slack/shared/**`, shared backend, pipeline, manual                                          | Preserved through `staging` and `production` environments                                          |
| `.github/workflows/team-shared.yml`         | `validate-infrastructure`; staging/production deploy matrices                             | `src/team/shared/**`, shared backend, pipeline, manual                                           | Preserved through `staging` and `production` environments                                          |

## Trigger Group Notes

- `pipeline_changed` activates all CI and infrastructure validation matrices.
- Documentation/specification/agent-instruction-only changes produce a passing required result without build or deploy jobs.
- `src/web/packages/**`, `src/web/package.json`, `src/web/pnpm-lock.yaml`, and `src/web/pnpm-workspace.yaml` fan out to every web app build and web infrastructure validation.
- `api-definitions/**` and generation scripts fan out to all all-in-one backend image validation.
- `src/shared/**` fans out to all-in-one backend image validation and shared/domain infrastructure validation.
- `src/shared/infrastructure/**` additionally fans out to web app and docs event catalog infrastructure validation because the retired workflows selected those jobs from that path.

## Deployment Policy

- Pull requests validate only and cannot deploy.
- Pushes to `main` may run selected staging CD after the matching selected infrastructure validation succeeds.
- Production CD depends on selected staging CD and uses the existing `production` GitHub environment gate.
- Manual runs intentionally execute the full CI validation path. CD jobs remain blocked unless the run is a push to `main`; this keeps manual runs as deployment-readiness checks without bypassing the clarified deployment policy.

## Retained Workflows

No application CI/CD workflows are intentionally retained outside `.github/workflows/skedular-cicd-pipeline.yml` for this feature. Future non-CI/CD maintenance workflows may remain separate if they are documented here before merge.

## Parity Validation Notes

- All 19 old workflow files are represented in the consolidated workflow inventory.
- The `web-shared.yml` workflow was included in retirement mapping to close the workflow count gap identified during analysis.
- DSST versioning, tag-based package publishing, and release version calculation remain excluded.
