# Research: Modularize Webapp Products

**Feature**: 004-modularize-webapp-products  
**Phase**: 0 — Research  
**Date**: 2026-04-27

---

## Current State Assessment

### Three Products, One Real Codebase

| App                      | Domain                  | State                                              |
| ------------------------ | ----------------------- | -------------------------------------------------- |
| `web/apps/webapp`        | Core Skedular scheduler | Full implementation — all shared code lives here   |
| `web/apps/webapp-teams`  | Skedular for Teams      | Stub — placeholder `page.tsx` + empty `layout.tsx` |
| `web/apps/webapp-spaces` | Skedular for Spaces     | Stub — placeholder `page.tsx` + empty `layout.tsx` |

Both Teams and Spaces already have their own Vercel deployments, `package.json`, `next.config.ts`, `relay.config.js`, `vitest.config.ts`, and `Dockerfile` — the deployment shell is ready. What is missing is the shared infrastructure that allows them to be built as real products.

### What Currently Lives Only in `webapp`

| Module                                    | Location                                                   | Needed by Teams/Spaces?                               |
| ----------------------------------------- | ---------------------------------------------------------- | ----------------------------------------------------- |
| Typography wrappers                       | `src/components/commons/`                                  | Yes — all products need them per the Typography Rule  |
| Theme primitives (colour tokens, shadows) | `src/libs/theme/theme-primitives.ts`                       | Yes — same visual identity                            |
| Theme construction                        | `src/libs/theme/theme.ts`                                  | Yes — all products use the same MUI theme             |
| RelayProvider                             | `src/libs/providers/relay-provider.tsx`                    | Yes — all products use Relay                          |
| ThemeProvider                             | `src/libs/providers/theme-provider.tsx`                    | Yes                                                   |
| PaletteModeProvider                       | `src/libs/providers/palette-mode-provider.tsx`             | Yes                                                   |
| DatePickerLocalizationProvider            | `src/libs/providers/date-picker-localization-provider.tsx` | Yes                                                   |
| `useKnownParams` hook                     | `src/libs/providers/known-params-hook.tsx`                 | Yes — cross-product URL param convention              |
| `useIntegratedPlatform` hook              | `src/libs/providers/integrated-platform-hook.tsx`          | Yes                                                   |
| Shared date/name utilities                | `src/libs/utils/index.ts`                                  | Yes — common date helpers, name formatting            |
| MUI X license wrapper                     | `src/libs/mui/muix-license.tsx`                            | Yes — each product needs the license mounted          |
| Cookie consent                            | `src/libs/cookie-consent/`                                 | Yes — all public-facing products                      |
| Image file uploader                       | `src/libs/image-file-uploader/`                            | Yes — used across products                            |
| LogRocket provider                        | `src/libs/providers/logrocket-provider.tsx`                | Likely yes — observability cross-product              |
| GoogleAnalytics provider                  | `src/libs/providers/google-analytics-provider.tsx`         | Each product needs its own config; provider is shared |
| MS Teams provider                         | `src/libs/providers/in-msteams-provider.tsx`               | `webapp` and `webapp-teams` only — not Spaces         |

### What Stays in `webapp` Only

| Module                                                                                  | Why                                                                        |
| --------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| All feature components (`booking/`, `marketplace/`, `organization/`, `location/`, etc.) | Skedular scheduler domain — not Teams or Spaces                            |
| `app/` route tree (billing, marketplace, organizations, settings, etc.)                 | Core Skedular product journeys                                             |
| `client-root-layout.tsx`                                                                | Contains Teams credential auth logic specific to `webapp`                  |
| Logger (`libs/logging/index.ts`)                                                        | Product-named (`app: 'webapp'`) — each product needs its own logger config |
| Analytics config                                                                        | Product-specific GA/GTM tag IDs                                            |

### `web/packages/ui` Current State

Already has layout building blocks: `stack-row`, `stack-column`, `page-header-panel`, `page-section-card`, `settings-section-card`, `setup-feature-card`, `setup-split-layout`, `sticky-review-rail`, `guided-editor-progress`, `editor-action-bar`.

**Gap**: Typography wrappers (`BodyIconTypography`, `SmallIconTypography`, etc.) currently live in `webapp/src/components/commons/` but are needed by all products. They belong in `@skedular/ui`.

---

## Decision 1: Where Should Shared Non-Visual Modules Live?

**Decision**: Create a new workspace package `web/packages/shared` (`@skedular/shared`). This is the single centralised home for everything shared across products that is not a visual primitive: providers, hooks, utilities, MUI helpers, and generic UI behaviours. Utilities (`date-utils`, `name-utils`, `relay-utils`, etc.) are the canonical example — they are extracted from `webapp/src/libs/utils/` and placed here so every product imports from `@skedular/shared` rather than maintaining its own copy.

**Rationale**:

- `@skedular/ui` is a visual-primitives package with no runtime state or React context — mixing providers into it would violate its design boundary (FR-003: composites with state/orchestration belong outside the design system).
- A separate `@skedular/shared` package centralises all shared runtime infrastructure — providers, hooks, **utilities**, MUI helpers — in one place that all three products consume.
- Both `webapp-teams` and `webapp-spaces` can declare `@skedular/shared` as a dependency and immediately use shared providers, hooks, and utilities without copying files.

**Worked example — utilities**:

```
web/packages/shared/src/utils/
  date-utils.ts        ← dayjs timezone/formatting/comparison helpers
  name-utils.ts        ← NameDetails type + name formatting
  relay-utils.ts       ← RelayErrorLike + Relay error formatting
  index.ts
```

Consumers import: `import { formatName, isToday } from '@skedular/shared';`

**Alternatives considered**:

- _Copy files into each app_: Creates three diverging copies; ruled out immediately.
- _Expand `@skedular/ui`_: Would combine visual primitives with stateful providers; violates FR-003.
- _Use a `src/shared/` folder inside each app_: Does not scale across three separate Next.js processes; pnpm workspaces is the correct tool.

---

## Decision 2: Should Typography Wrappers Move to `@skedular/ui`?

**Decision**: Yes — move `src/components/commons/` typography wrappers into `web/packages/ui`.

**Rationale**:

- Typography wrappers are pure visual primitives (they wrap MUI `Typography` with Skedular-specific props). They contain no state, no business logic, no product copy.
- All three products will need them; keeping them in `webapp` would require Teams and Spaces to copy or depend on the wrong package.
- `@skedular/ui` already has `stack-row`, `stack-column`, etc. — typography wrappers fit naturally alongside them.
- The Typography Rule in `webapp/AGENTS.md` will remain valid; the import path changes from `@/components/commons` to `@skedular/ui` in feature components.

**Alternatives considered**:

- _Keep in `webapp`, re-export from `@skedular/shared`_: Double-indirection with no benefit.
- _Keep in each product app_: Duplicates the wrappers; any change requires three updates.

---

## Decision 3: What is the Boundary for `@skedular/shared`?

**Decision**: `@skedular/shared` is the single centralised package for all cross-product runtime modules — React context providers, shared hooks, **utilities**, MUI helpers, generic UI components, cookie consent, and image upload. It does NOT own feature components, product-specific analytics config, or auth integration.

| Module                                | Destination                                                                              |
| ------------------------------------- | ---------------------------------------------------------------------------------------- |
| RelayProvider                         | `@skedular/shared`                                                                       |
| ThemeProvider                         | `@skedular/shared`                                                                       |
| PaletteModeProvider + context         | `@skedular/shared`                                                                       |
| DatePickerLocalizationProvider        | `@skedular/shared`                                                                       |
| `useKnownParams`                      | `@skedular/shared`                                                                       |
| `useIntegratedPlatform`               | `@skedular/shared`                                                                       |
| Shared date/name/error utilities      | `@skedular/shared` ← **canonical example of centralised sharing**                        |
| MUI X license wrapper                 | `@skedular/shared`                                                                       |
| Cookie consent                        | `@skedular/shared`                                                                       |
| Image file uploader                   | `@skedular/shared`                                                                       |
| LogRocket provider                    | `@skedular/shared`                                                                       |
| GoogleAnalytics/GTM component         | `@skedular/shared` (component only; tag ID config stays per-product)                     |
| InMsTeams provider + context          | `@skedular/shared` (both webapp and webapp-teams use it)                                 |
| Typography wrappers                   | `@skedular/ui` (visual primitive, not runtime)                                           |
| Theme primitives + theme construction | `@skedular/ui` (design tokens — visual layer)                                            |
| Feature components                    | Stay in `webapp` (Skedular-specific)                                                     |
| Product logger config                 | Each app owns its own (`app: 'webapp'` / `app: 'webapp-teams'` / `app: 'webapp-spaces'`) |

---

## Decision 4: How Should Auth / Shared Authenticated Journeys Work Across Products?

**Decision**: Auth entry points (sign-in, callback, account settings, notifications) remain owned by core Skedular (`webapp`). Teams and Spaces products redirect to `webapp` for auth and are returned to their respective domains post-authentication. (Per FR-013 as clarified.)

**Rationale**:

- WorkOS AuthKit is already configured per product via `NEXT_PUBLIC_SITE_URL`. The provider-level wiring belongs in `@skedular/shared`; the actual sign-in pages stay in `webapp`.
- Duplicating auth pages into each product adds maintenance overhead with no product-differentiation benefit at this stage.
- Teams and Spaces stubs currently have no auth pages; this decision does not create rework.

**Alternatives considered**:

- _Per-product auth pages_: Higher effort, no value now.
- _Shared auth micro-frontend_: Over-engineering for the current scale.

---

## Decision 5: Path Alias Strategy When Moving Modules to Packages

**Decision**: Packages use relative imports internally. Apps that consume packages use the package name (`@skedular/ui`, `@skedular/shared`). The `@/` alias in app code is updated to import from the package rather than the local `src/libs/` path.

**Rationale**:

- `@/` aliases only work within a single Next.js app's `tsconfig.json`. They cannot be used inside workspace packages.
- Migrating callers in `webapp` from `@/libs/providers/relay-provider` to `@skedular/shared` (or `@/libs/utils/` to `@skedular/shared`) is a mechanical import-path update with no logic change.
- TypeScript `paths` config in each app's `tsconfig.json` may optionally map short aliases for the shared packages, but the canonical import is the package name.

---

## Decision 6: Do `webapp-teams` and `webapp-spaces` Need New Packages Added?

**Decision**: Yes — both apps add `"@skedular/shared": "workspace:*"` to their `package.json`. `@skedular/ui` is already a dependency of both.

**Rationale**: `webapp-teams` and `webapp-spaces` both already list `@skedular/ui` as a dependency. Extending to `@skedular/shared` follows the same pattern.

---

## Resolved Unknowns

| Unknown                                                                | Resolution                                                                                               |
| ---------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------- |
| Where do shared providers and utilities live?                          | `web/packages/shared` (`@skedular/shared`) — the single centralised shared package                       |
| Where do typography wrappers and theme tokens live?                    | Expand `web/packages/ui` (`@skedular/ui`)                                                                |
| How do Teams/Spaces access shared code?                                | Declare workspace package dependency; import by package name                                             |
| Are routes split?                                                      | No — each product is already on its own domain; route separation is inherent                             |
| Does this touch `api-definitions/`?                                    | No — purely frontend restructure; Relay artifacts regenerated if fragments move                          |
| What happens to the MS Teams credential logic?                         | Stays in `webapp/client-root-layout.tsx`; `in-msteams-provider` moves to `@skedular/shared`              |
| What about `libs/utils` dependency on `@/libs/theme/theme-primitives`? | Theme primitives move to `@skedular/ui`; utils move to `@skedular/shared` and import from `@skedular/ui` |

---

## Risks and Mitigations

| Risk                                                                 | Likelihood                            | Mitigation                                                                                        |
| -------------------------------------------------------------------- | ------------------------------------- | ------------------------------------------------------------------------------------------------- |
| Relay artifact regeneration breaks after fragment moves              | Medium                                | Run `scripts/generate-graphql.sh` + `web/apps/webapp/scripts/generate.sh` after any fragment move |
| `@/` alias references in moved files                                 | High (many files)                     | Systematic import-path update; automated search for `@/libs/` imports after each extraction       |
| MUI X license not mounted in Teams/Spaces stubs                      | Low (stubs don't use MUI X grids yet) | Add `MuiXLicense` to each product's client root layout as part of this initiative                 |
| Circular dependency between `@skedular/shared` and `@skedular/ui`    | Medium                                | `@skedular/shared` imports from `@skedular/ui` (one-way); `@skedular/ui` has no shared dependency |
| Build size increase if packages bundle peer dependencies incorrectly | Low                                   | Both packages use `peerDependencies` for React and MUI; confirmed in `ui/package.json`            |
