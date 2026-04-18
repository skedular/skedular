# Deployment SLA Results

## Measurement Date

- 2026-04-18

## Scope

This feature branch does not perform live staging/production deployment execution from local validation. Therefore, this report captures pre-deployment timing proxies used for release-readiness assessment.

## Build Duration Measurements

- `pnpm --filter webapp-teams build`: `3.21s` (real)
- `pnpm --filter webapp-spaces build`: `2.74s` (real)

## Terraform Validation Duration Measurements

- `webapp-teams/staging`: `2.35s`
- `webapp-teams/common_resources`: `2.19s`
- `webapp-teams/production`: `2.19s`
- `webapp-spaces/staging`: `2.14s`
- `webapp-spaces/common_resources`: `2.14s`
- `webapp-spaces/production`: `2.13s`

## Readiness Interpretation

- All measured pre-deploy checks complete in under 4 seconds each on local machine.
- No terraform validation failures were observed.
- CI/CD deployment stage SLA must be confirmed by first production workflow runs after merge.

## Follow-up

- Record GitHub Actions run durations for `build`, `staging`, and `production` jobs after first main-branch execution.
- Update this document with empirical pipeline SLAs from Actions history.
