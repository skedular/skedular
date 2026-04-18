# Implementation Plan: Split UI into Three Products

**Branch**: `002-split-ui-products` | **Date**: 2026-04-18 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-split-ui-products/spec.md`

## Summary

Scaffold two new web products (`webapp-teams`, `webapp-spaces`) and two health companions
using existing `webapp` and `webapphelp` patterns. Keep deployment independent per product,
keep Terraform state isolated per workspace path, keep shared UI package version aligned across
all products, and include mandatory structured logging verification tasks.

## Technical Context

**Language/Version**: TypeScript (Next.js web apps), Terraform HCL  
**Primary Dependencies**: `next`, `react`, `@skedular/ui`, Terraform AWS/Vercel/Google providers  
**Storage**: S3 Terraform backend + DynamoDB locking (per workspace state key)  
**Testing**: `vitest` for web app tests, `terraform validate`, workflow lint/validation checks  
**Target Platform**: Web apps deployed via Vercel and infrastructure managed by Terraform  
**Project Type**: Monorepo web application scaffolding with infra-as-code  
**Performance Goals**: Single workspace deploy target < 5 minutes end-to-end  
**Constraints**: No generated contract changes, no backend domain boundary changes, independent deployments  
**Scale/Scope**: 3 main web products total (`webapp` existing + 2 new), 3 workspaces per main app,
health companion per new app

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — No `api-definitions/` updates; no generated surface changes in this phase.
- [x] **II. Domain Boundaries** — UI/infrastructure scaffolding only; no cross-domain DB/internal access.
- [x] **III. Testing** — Plan includes app build/lint/test + Terraform validation + workflow validation tasks.
- [x] **IV. Frontend** — Web-only changes; preserve existing webapp patterns and British copy rules.
- [x] **V. Pattern Consistency** — Reuse existing `web/apps/webapp` and `web/apps/webapphelp` structure.
- [x] **VI. Logging** — Structured logging requirements included for build/deploy/startup/failure paths.

## Project Structure

### Documentation (this feature)

```text
specs/002-split-ui-products/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
web/apps/
├── webapp/                 # source template for main app scaffolding
├── webapphelp/             # source template for health app scaffolding
├── webapp-teams/         # to scaffold
├── webapp-spaces/     # to scaffold
├── webapp-teams-help/    # to scaffold
└── webapp-spaces-help/# to scaffold

.github/workflows/
├── webapp.yml
├── webapphelp.yml
└── web-shared.yml
```

**Structure Decision**: Use existing monorepo web app pattern under `web/apps/`, with repo-level
workflow patterns in `.github/workflows/` and per-app Terraform workspace config under each app.

## Phase 0 Output

`research.md` updated with concrete decisions and no open `NEEDS CLARIFICATION` items.

## Phase 1 Output

- `data-model.md` retained and aligned with current source-of-truth paths.
- `contracts/` retained as implementation contracts for Terraform, workflows, and app structure.
- `quickstart.md` retained as execution guide.

## Re-Check Gates (Post-Design)

- [x] **I. Contract-First** — Still no contract/generator changes.
- [x] **II. Domain Boundaries** — Still scoped to web/infra scaffolding.
- [x] **III. Testing** — Test + validation paths documented.
- [x] **IV. Frontend** — Existing frontend conventions maintained.
- [x] **V. Pattern Consistency** — Existing patterns reused; no deviation justification needed.
- [x] **VI. Logging** — Logging requirements and checklist coverage present.

## Complexity Tracking

No constitution violations requiring exception handling.
