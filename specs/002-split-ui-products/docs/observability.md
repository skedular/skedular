# Observability Runbook

## Scope

This runbook covers scaffold validation and deployment visibility for:

- `webapp`
- `webapp-teams`
- `webapp-spaces`
- `webapphelp`
- `webapp-teams-help`
- `webapp-spaces-help`

## Build and Validation Signals

- GitHub Actions workflow status per app workflow file.
- Terraform validation pass/fail for each workspace:
  - `staging`
  - `common_resources`
  - `production`
- Build output for each app package via `pnpm --filter <app> build`.

## Minimum Structured Events to Capture

- `build_started`
- `build_completed`
- `terraform_validate_started`
- `terraform_validate_completed`
- `deploy_started`
- `deploy_completed`
- `deploy_failed`

## Required Fields

- `service` (example: `webapp-teams`)
- `environment` (staging/production)
- `workflow`
- `workspace`
- `correlation_id` (GitHub run ID or pipeline execution ID)
- `status`
- `duration_ms`
- `error_message` (when failed)

## Incident Triage

1. Identify failing workflow and run ID.
2. Confirm whether failure is in build, lint, terraform validate, or deploy stage.
3. Re-run stage locally using app-local commands.
4. If terraform-related, run `terraform init -backend=false` and `terraform validate` in failing workspace.
5. Raise issue with run ID, failing stage, and error snippet.
