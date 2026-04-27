# Quickstart: Modularize Webapp Products

**Feature**: 004-modularize-webapp-products
**Phase**: 1 — Design
**Date**: 2026-04-27

This guide explains how to work within the modularized web codebase — where code lives, how to decide where to put new code, and how to perform a modularization extraction slice correctly.

---

## Package Map

| Package                  | Import name        | Purpose                                                                                                                |
| ------------------------ | ------------------ | ---------------------------------------------------------------------------------------------------------------------- |
| `web/packages/ui`        | `@skedular/ui`     | Visual primitives: layout components, typography wrappers, theme tokens                                                |
| `web/packages/shared`    | `@skedular/shared` | Centralised shared modules: providers, hooks, utilities (date, name, Relay), MUI helpers, cookie consent, image upload |
| `web/apps/webapp`        | internal (`@/`)    | Core Skedular scheduler — feature components, route tree, Relay queries                                                |
| `web/apps/webapp-teams`  | internal (`@/`)    | Skedular for Teams — product feature components and route tree                                                         |
| `web/apps/webapp-spaces` | internal (`@/`)    | Skedular for Spaces — product feature components and route tree                                                        |

---

## Where Does New Code Go?

Ask these questions in order:

1. **Is it a visual primitive with no state, no business logic, no product copy?**
   → `@skedular/ui`

2. **Is it runtime infrastructure (a provider, context, hook, utility, or any other module) shared by two or more products?**
   → `@skedular/shared` — this is the single central home for all shared cross-product code

3. **Is it specific to the Skedular scheduler?**
   → `web/apps/webapp/src/`

4. **Is it specific to Skedular for Teams?**
   → `web/apps/webapp-teams/src/`

5. **Is it specific to Skedular for Spaces?**
   → `web/apps/webapp-spaces/src/`

6. **Is it product-specific configuration (logger name, analytics tag ID, environment bindings)?**
   → Each app owns it under `src/libs/` — do not extract to a shared package.

---

## Typography Rule

Never import `@mui/material/Typography` directly in feature or page components. Always use the Skedular wrappers:

```tsx
// ✅ Correct
import { BodyIconTypography, SmallIconTypography } from "@skedular/ui";

// ❌ Wrong
import Typography from "@mui/material/Typography";
```

The wrappers available in `@skedular/ui`:

- `BodyIconTypography`
- `CaptionIconTypography`
- `SmallIconTypography`
- `SmallSubtitleIconTypography`
- `SmallHeadingIconTypography`
- `MediumHeadingIconTypography`
- `LargeHeadingIconTypography`
- `ExtraLargeHeadingIconTypography`
- `LeadIconTypography`
- `SectionIconTypography`
- `SubtitleIconTypography`
- `IconTypography`
- `ErrorTypography`

---

## Using Shared Utilities (Canonical Example)

Utilities are the clearest example of what centralises in `@skedular/shared`. Instead of each product maintaining its own date helpers or name formatters, they all import from the single shared package:

```ts
// ✅ Correct — shared utility imported from the central package
import { formatName, isToday, isInSameWeek } from "@skedular/shared";

// ❌ Wrong — do not copy utils into each product app
import { formatName } from "@/libs/utils";
```

The same rule applies to every shared module: providers, hooks, MUI helpers, cookie consent, image upload. All shared code has one home.

---

## Using Shared Providers

All three products wire up their client root layout using `@skedular/shared`:

```tsx
// web/apps/webapp-teams/src/app/layout.tsx (example)
import { RelayProvider, ThemeProvider, PaletteModeProvider, DatePickerLocalizationProvider } from "@skedular/shared";
```

Each product's root layout is still product-owned — it mounts the shared providers but controls its own metadata, font variables, and analytics config.

---

## Using the Theme

Theme tokens and the MUI theme factory live in `@skedular/ui`:

```tsx
import { createTheme, coal, emerald } from "@skedular/ui/theme";
```

Do not re-declare colour tokens in product apps.

---

## Performing a Modularization Slice

A "slice" is the unit of modularization work: one module (or logical group) moved from its current location to its target package.

### Slice checklist

1. **Identify the source module** using `data-model.md`.
2. **Create the target file** in the destination package.
3. **Update internal imports** inside the moved file — replace `@/libs/...` or `@/components/...` with the correct package name (`@skedular/ui` or `@skedular/shared`).
4. **Update all consumers** in `webapp` (and any other app) — replace the old `@/...` import path with the new package import.
5. **Delete the source file** from its original location.
6. **Update `index.ts` exports** in the destination package.
7. **Run the build** — `pnpm build` from `web/` or use Turborepo: `pnpm turbo build`.
8. **Run the tests** — `pnpm test` in the affected app and package.
9. **If any Relay fragment moved**, run `web/apps/webapp/scripts/generate.sh` to regenerate artefacts.
10. **Verify no `@/libs/` or `@/components/commons/` import remains** for the moved module.

> A slice is not complete until the source location is deleted and all consumers compile and test clean.

---

## Setting Up `@skedular/shared` (New Package)

The package scaffolding is created as part of the first implementation task. Structure:

```text
web/packages/shared/
  package.json          # name: "@skedular/shared"
  tsconfig.json         # extends web root tsconfig
  src/
    providers/
    hooks/
    utils/
    mui/
    cookie-consent/
    image-file-uploader/
    index.ts
```

The `package.json` uses the same `peerDependencies` pattern as `@skedular/ui`: React, MUI, and Next.js are peers, not bundled dependencies.

To add `@skedular/shared` to an app:

```json
// In the app's package.json "dependencies":
"@skedular/shared": "workspace:*"
```

Then run `pnpm install` from `web/`.

---

## Running Builds and Tests

From `web/`:

```sh
# Build all apps and packages
pnpm turbo build

# Test all
pnpm turbo test

# Lint all
pnpm turbo lint

# Build a specific app
pnpm turbo build --filter=webapp
pnpm turbo build --filter=webapp-teams
pnpm turbo build --filter=webapp-spaces

# Test a specific package
pnpm turbo test --filter=@skedular/ui
pnpm turbo test --filter=@skedular/shared
```

---

## Auth and Account Surfaces

Sign-in, callback, account settings, and notifications remain in `webapp`. Users of `webapp-teams` and `webapp-spaces` are redirected to `webapp` for these journeys and returned to their product domain post-authentication.

**Do not** create product-specific sign-in or account pages in `webapp-teams` or `webapp-spaces` during this initiative.

---

## Logging

Each product app configures its own logger. Do not share the logger instance across products.

```ts
// web/apps/webapp/src/libs/logging/index.ts
import pino from "pino";
export default pino({ name: "webapp", base: { app: "webapp" } });

// web/apps/webapp-teams/src/libs/logging/index.ts
import pino from "pino";
export default pino({ name: "webapp-teams", base: { app: "webapp-teams" } });

// web/apps/webapp-spaces/src/libs/logging/index.ts
import pino from "pino";
export default pino({ name: "webapp-spaces", base: { app: "webapp-spaces" } });
```

---

## Ownership Boundary Disputes

When uncertain about ownership, consult `data-model.md`. The boundary rules (FR-003) are:

- **Design system** (`@skedular/ui`): visual only, no state, no business logic.
- **Shared application** (`@skedular/shared`): runtime, cross-product, no feature logic.
- **Product app**: everything else.

If a module contains both visual and non-visual concerns, split it. Extract the visual shell to `@skedular/ui` and the orchestration to the owning product app or `@skedular/shared`.
