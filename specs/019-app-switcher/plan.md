# Implementation Plan: App Switcher

**Branch**: `019-app-switcher` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/019-app-switcher/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add a shared Skedular app switcher that appears as a secondary shortcut inside existing signed-in navigation/menu surfaces for Skedular, Skedular Teams, and Skedular Spaces. Each product app supplies its current app identity and environment-provided base URLs for the three app destinations; the shared model filters missing or invalid URLs, shows every valid configured destination regardless of destination access, and navigates to the configured base URL without preserving page, organization, tenant, or workflow context. The switcher must not appear as a separate app bar, prominent page action, or customer-facing coworking-space subdomain control.

The implementation should extend existing frontend shared boundaries: `@skedular/shared` owns app identity, destination modeling, validation, and logging helpers; `@skedular/ui` owns the reusable visual switcher; each product app owns its configuration wiring and places the switcher in its authenticated left navigation content.

## Technical Context

**Language/Version**: TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router  
**Primary Dependencies**: `@skedular/shared`, `@skedular/ui`, MUI 9, Vitest, React Testing Library, pnpm/Turbo workspace  
**Storage**: N/A; app destinations are runtime configuration values, not persisted feature data  
**Testing**: Vitest and React Testing Library for shared model/UI behavior, plus product-app integration tests or focused component tests where navigation is wired  
**Target Platform**: Skedular web products: `src/web/apps/webapp`, `src/web/apps/webapp-teams`, `src/web/apps/webapp-spaces`  
**Project Type**: Frontend monorepo feature across three web applications and two shared packages  
**Performance Goals**: Switcher renders with authenticated navigation without perceptible delay; users can switch to another configured app in 10 seconds or less per SC-001  
**Constraints**: No backend contract changes; no generated artifacts edited; no Relay/OpenAPI regeneration expected; American spelling for user-facing copy; typography wrappers from `@skedular/ui` only  
**Scale/Scope**: Three fixed Skedular app identities and up to three configured destination URLs per product environment

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature does not touch `api-definitions/`, backend contracts, Relay schemas, or generated clients. No generator script is required unless later implementation discovers a backend contract dependency.
- [x] **II. Domain Boundaries** — This feature is frontend-only and does not cross backend domain ownership lines or read another domain's persistence.
- [x] **III. Testing** — Web UI/model changes require Vitest and React Testing Library. No persistence or backend integration tests are required because no database, Kafka, Temporal, or external HTTP boundary is introduced.
- [x] **IV. Frontend** — Web changes are planned. Shared runtime/model code belongs in `@skedular/shared`, visual primitives belong in `@skedular/ui`, product apps only wire product-specific configuration, generated artifacts are not hand-edited, and user-facing copy uses American spelling.
- [x] **V. Pattern Consistency** — This extends the existing split-product shared package model and app-shell pattern. No new framework or parallel shared abstraction is introduced.
- [x] **VI. Logging** — Structured client-side logging is planned for app-switcher rendering/configuration decisions and user switch selections, without logging sensitive data.

## Project Structure

### Documentation (this feature)

```text
specs/019-app-switcher/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── app-switcher-contract.md
├── checklists/
│   └── requirements.md
└── spec.md
```

### Source Code (repository root)

```text
src/web/
├── apps/
│   ├── webapp/
│   │   └── src/
│   │       ├── app/
│   │       └── types/
│   ├── webapp-teams/
│   │   └── src/
│   │       ├── app/
│   │       └── types/
│   └── webapp-spaces/
│       └── src/
│           ├── app/
│           └── types/
└── packages/
    ├── shared/
    │   └── src/
    │       ├── app-products/
    │       └── app-shell/
    └── ui/
        └── src/
            └── app-shell/
```

**Structure Decision**: Use the existing web workspace. Product identity and destination modeling extend `src/web/packages/shared/src/app-products` and `src/web/packages/shared/src/app-shell`. The visual switcher extends `src/web/packages/ui/src/app-shell`. Each app (`webapp`, `webapp-teams`, `webapp-spaces`) wires its current app id and environment-provided destination URLs into the shared app-shell model and renders it from authenticated navigation menu content only.

## Complexity Tracking

No constitution violations require justification.

## Phase 0: Research

See [research.md](./research.md). All technical context unknowns are resolved.

## Phase 1: Design and Contracts

See:

- [data-model.md](./data-model.md)
- [contracts/app-switcher-contract.md](./contracts/app-switcher-contract.md)
- [quickstart.md](./quickstart.md)

### Post-Design Constitution Check

- [x] **I. Contract-First** — Design remains frontend-only and does not require contract generation.
- [x] **II. Domain Boundaries** — Design uses no backend domain access.
- [x] **III. Testing** — Design includes shared model tests, shared UI tests, and product wiring tests.
- [x] **IV. Frontend** — Design keeps shared model/runtime in `@skedular/shared`, visual app-shell work in `@skedular/ui`, and product configuration in the product apps.
- [x] **V. Pattern Consistency** — Design extends existing app-products/app-shell modules rather than introducing a parallel navigation system.
- [x] **VI. Logging** — Design includes a structured app-switcher log event contract for configuration filtering and user selection.
