# Design System Integration Policy

## Shared Package

- Package: `@skedular/ui`
- Version policy: all main apps must use identical value in package.json

## Verified Apps

- webapp: "@skedular/ui": "workspace:*"
- webapp-teams: "@skedular/ui": "workspace:*"
- webapp-spaces: "@skedular/ui": "workspace:*"

## Verification Commands

- `scripts/verify-ui-package-versions.sh webapp webapp-teams webapp-spaces`
- `grep -R "@skedular/ui" web/apps/webapp/src web/apps/webapp-teams/src web/apps/webapp-spaces/src`

## Typography Guardrail

- Prefer shared typography wrappers under `@/components/commons` in feature/page components.
