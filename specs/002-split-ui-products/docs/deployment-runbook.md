# Deployment Runbook

## Prerequisites

- Correct AWS role variables configured for staging and production.
- Required secrets available in repository settings.
- Terraform state backends configured per app/workspace.

## Workflows

- Main apps:
  - `.github/workflows/webapp.yml`
  - `.github/workflows/webapp-teams.yml`
  - `.github/workflows/webapp-spaces.yml`
- Help apps:
  - `.github/workflows/webapphelp.yml`
  - `.github/workflows/webapp-teams-help.yml`
  - `.github/workflows/webapp-spaces-help.yml`

## Standard Deployment Sequence

1. Trigger workflow via push to `main` (or manual dispatch).
2. Confirm build stage success.
3. Confirm terraform lint/validate/plan success for staging and production.
4. Confirm staging deployment success.
5. Confirm production deployment success.

## Rollback Guidance

1. Stop further production promotions.
2. Re-run previous known-good workflow revision.
3. For terraform drift or bad plan, revert offending infrastructure change and re-run validation.
4. Document incident summary with run links and root cause.

## Post-Deployment Checks

- Landing route reachable for main apps.
- Help docs route reachable for help apps.
- No terraform validation regressions in any workspace.
