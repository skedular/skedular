# Research: Split UI into Three Products

**Date**: 2026-04-18  
**Related**: [plan.md](plan.md), [spec.md](spec.md)

## Decisions

### R1 Current webapp structure source

**Decision**: Scaffold from `web/apps/webapp` and `web/apps/webapphelp`, excluding transient dirs (`node_modules`, `.next`, `.turbo`).

**Rationale**: Existing app already has required infra + src + docs pattern; excluding transient output avoids dirty scaffolds.

**Alternatives considered**:

- Scaffold from docs/contracts only: rejected, higher risk of mismatch with real app layout.
- Build new skeleton manually: rejected, slower and more drift-prone.

### R2 Shared design system package

**Decision**: Use `@skedular/ui` from workspace dependency (`workspace:*`) as shared UI package baseline.

**Rationale**: `web/apps/webapp/package.json` currently imports `@skedular/ui`; this is source-of-truth in repo.

**Alternatives considered**:

- `@skedular/design-system`: rejected; not present in current app dependency list.

### R3 Workflow template source

**Decision**: Reuse repo-level workflow patterns from `.github/workflows/webapp.yml`, `.github/workflows/webapphelp.yml`, and shared workflow files.

**Rationale**: Current web apps do not carry per-app `.github/workflows/`; CI pattern is centralised at repo root.

**Alternatives considered**:

- Create per-app workflow dirs under each app: rejected for now; would diverge from current repo pattern.

### R4 Terraform workspace/backend pattern

**Decision**: Use workspace dirs `staging`, `common_resources`, `production` per app; keep isolated backend keys per `{project_id}/{environment}/terraform.tfstate`.

**Rationale**: Matches current workspace naming and stated isolation strategy in contracts.

**Alternatives considered**:

- Collapse environments into single workspace: rejected due to weaker environment isolation.

### R5 Vercel/deployment config shape

**Decision**: Keep build/deploy settings aligned with existing webapp build scripts and central workflow orchestration; app-specific environment values remain per project/environment.

**Rationale**: Reuses operational model already used by `webapp` and `webapphelp`.

**Alternatives considered**:

- Dedicated new deployment stack for each app immediately: rejected for phase-1 scaffolding scope.

### R6 UI version alignment rule

**Decision**: Enforce same `@skedular/ui` version reference across all three main web products.

**Rationale**: Directly implements specification clarification requiring version sync.

**Alternatives considered**:

- Independent version drift per app: rejected by clarified requirement.

### R7 Provider compatibility baseline

**Decision**: Follow existing provider constraints from current webapp workspace files and shared Terraform modules.

**Rationale**: Avoid speculative upgrades during scaffolding; preserve working baseline.

**Alternatives considered**:

- Upgrade providers during scaffold creation: rejected; adds unrelated migration risk.

### R8 Local developer baseline

**Decision**: Keep local toolchain baseline already used by webapp and quickstart: Node, Terraform, AWS CLI, plus existing package manager scripts.

**Rationale**: Minimises onboarding variance between current and new apps.

**Alternatives considered**:

- New toolchain constraints for only new apps: rejected; unnecessary split.

### R9 Health project source pattern

**Decision**: Mirror `web/apps/webapphelp` structure for `webapp-teams-help` and `webapp-spaces-help`.

**Rationale**: Aligns with FR-008 and existing health app implementation model.

**Alternatives considered**:

- Lightweight ping-only health apps: rejected; would not mirror current help app contract.

### R10 Logging/observability pattern

**Decision**: Treat structured logs as mandatory for infra validation/build/deploy/startup/failure paths, aligned with constitution v1.1.0.

**Rationale**: Constitution principle VI requires feature-level observability parity.

**Alternatives considered**:

- Documentation-only logging notes: rejected; insufficient for constitutional gate.

## Summary

All phase-0 unknowns resolved with concrete repo-grounded decisions. No `NEEDS CLARIFICATION` items remain.
