# GitHub Actions Secrets and Variables Matrix

This matrix documents required CI/CD inputs for all web product workflows introduced by this feature.

## Shared Variables

- `AWS_REGION`
- `STAGING_AWS_GITHUB_ACTIONS_AWS_ASSUME_ROLE_ARN`
- `PRODUCTION_AWS_GITHUB_ACTIONS_AWS_ASSUME_ROLE_ARN`

## Shared Secrets

- `CLOUDFLARE_API_KEY`
- `VERCEL_API_TOKEN`
- `GITHUB_TOKEN` (GitHub-provided)

## Main App Secrets

Used by `webapp`, `webapp-teams`, and `webapp-spaces` workflows:

- `STAGING_GCP_WEB_CREDENTIALS_CLIENT_ID`
- `STAGING_GCP_WEB_CREDENTIALS_CLIENT_SECRET`
- `STAGING_GOOGLE_MAP_API_KEY`
- `STAGING_WORKOS_API_KEY`
- `STAGING_SLACK_CLIENT_SECRET`
- `PRODUCTION_GCP_WEB_CREDENTIALS_CLIENT_ID`
- `PRODUCTION_GCP_WEB_CREDENTIALS_CLIENT_SECRET`
- `PRODUCTION_GOOGLE_MAP_API_KEY`
- `PRODUCTION_WORKOS_API_KEY`
- `PRODUCTION_SLACK_CLIENT_SECRET`

## Help App Secrets

Used by `webapphelp`, `webapp-teams-help`, and `webapp-spaces-help` workflows:

- `CLOUDFLARE_API_KEY`
- `VERCEL_API_TOKEN`

## Notes

- New workflows mirror existing secret naming conventions.
- No new secret names were introduced by scaffold-only implementation.
