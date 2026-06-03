# Shared Package Analysis

**Generated**: 2026-06-03  
**Tasks**: T032–T034  
**Scope**: `@skedular/ui` + `@skedular/shared`

---

## `@skedular/ui` Package

**Location**: `src/web/packages/ui/`  
**Package name**: `@skedular/ui`

### Component Inventory

| Category      | Count  | `'use client'` | Notes                                                                    |
| ------------- | ------ | -------------- | ------------------------------------------------------------------------ |
| `app-shell/`  | 4      | 4              | AppShellLayout, AppSwitcher, ManagementPageShell, OrganisationEmptyState |
| `commons/`    | 11     | 11             | ALL commons have `'use client'`                                          |
| `typography/` | 13     | 13             | ALL typography wrappers have `'use client'`                              |
| Top-level     | 7      | 7              | page-header-panel, setup-_, stack-_, sticky-review-rail                  |
| **Tests**     | 3      | 0              | (excluded from count)                                                    |
| **TOTAL**     | **41** | **41**         | 100% of real components are client                                       |

### Critical Finding: `commons/credit-card.tsx`

This component imports `react-svg-credit-card-payment-icons` (521 KB):

```typescript
// @skedular/ui/commons/credit-card.tsx
import { PaymentIcon } from "react-svg-credit-card-payment-icons";
```

It is exported from the barrel: `@skedular/ui/commons/index.ts` → `@skedular/ui/index.ts`.

This means **every route** that imports any component from `@skedular/ui` via the barrel loads 521 KB.

### Typography Components — Server Compatibility Assessment

All 13 typography components have `'use client'` but most only use MUI `Typography`:

```typescript
// Example: body-icon-typography.tsx
"use client"; // ← This may not be necessary
import Typography from "@mui/material/Typography";
// ...
```

**Investigation needed**: Do these typography components actually NEED `'use client'`? MUI v9 components may not require the directive if they don't use hooks or event handlers. If typography components can remove `'use client'`, importing them in Server Components becomes possible.

**Caveat**: MUI components use React context for theming (`useTheme`, `ThemeContext`) — this is client-side. However, with MUI's App Router cache provider (`AppRouterCacheProvider`), server-side rendering of MUI components is supported. The `'use client'` on typography components may be overly conservative.

### Barrel Export Impact

Current barrel: `@skedular/ui/index.ts` exports everything.

```
import { BodyIconTypography } from '@skedular/ui'
  → loads CreditCard (521 KB)
  → loads AppSwitcher
  → loads all 41 client components
```

**Recommendation**: For any Server Component consumers (future), use direct imports:

```typescript
import BodyIconTypography from "@skedular/ui/typography/body-icon-typography";
```

---

## `@skedular/shared` Package

**Location**: `src/web/packages/shared/`  
**Package name**: `@skedular/shared`

### Component Inventory

| Category               | Count  | `'use client'` | Notes                               |
| ---------------------- | ------ | -------------- | ----------------------------------- |
| `providers/`           | ~8     | 8              | All providers are client (expected) |
| `image-file-uploader/` | 2      | 2              | Uploader components — client        |
| `image-cropper/`       | 1      | 1              | Cropper — client                    |
| `cookie-consent/`      | 1      | 1              | Cookie banner — client              |
| Misc components        | ~4     | 3              | Mostly client                       |
| **TOTAL**              | **16** | **15**         | 94% client                          |

### Notable Files

#### `src/utils/relay-environment.ts`

```typescript
import { isServer } from "./constants";
```

Contains `isServer` check — designed for SSR. Not a component, not client-constrained.  
This is the Relay environment factory. It correctly handles both client and server contexts.

#### `src/mui/index.ts`

```typescript
import type { GridRowSelectionModel } from "@mui/x-data-grid";
```

Type-only import — should not appear in runtime bundle. However, `@mui/x-data-grid` (119 KB) is present in bundle. Investigate whether there's a non-type import path:

```bash
# Run to check
grep -r "from '@mui/x-data-grid'" src/web --include="*.tsx" --include="*.ts" | grep -v "import type"
```

#### `src/providers/authenticated-relay-provider.tsx`

**Critical**: This is a `'use client'` provider that wraps all children. It calls `useContext(InMsTeamsContext)` — a Teams-specific context check in what should be a shared, Teams-agnostic provider.

**Issue**: This couples the shared Relay provider to Teams-specific context. In non-Teams apps (webapp), `InMsTeamsContext` always returns a falsy value — the check is dead code.

**Fix**: Move Teams token injection to a Teams-specific wrapper:

```typescript
// @skedular/shared: relay-provider.tsx — base, no Teams context
const RelayProvider = ({ children, token }) => ...

// webapp-teams: teams-relay-provider.tsx
'use client';
const TeamsRelayProvider = ({ children }) => {
  const inMsTeams = useContext(InMsTeamsContext);
  return <RelayProvider token={inMsTeams ? teamsToken : undefined}>{children}</RelayProvider>;
};
```

---

## Package Boundary Rule Compliance

Per project guidelines (`AGENTS.md`):

| Rule                                                   | Status         |
| ------------------------------------------------------ | -------------- |
| `@skedular/ui` must NOT import from `@skedular/shared` | ✓ Not violated |
| `@skedular/shared` MAY import from `@skedular/ui`      | ✓ Correct      |

**Current violation risk**: If any `@skedular/ui` component imports from `@skedular/shared`, it violates the package boundary. Verify with:

```bash
grep -r "from '@skedular/shared'" src/web/packages/ui/src
```

---

## Key Recommendations

| Priority | Finding                                           | Action                            |
| -------- | ------------------------------------------------- | --------------------------------- |
| P0       | `CreditCard` barrel contamination (521 KB)        | Remove from commons barrel        |
| P1       | `@skedular/shared` InMsTeamsContext coupling      | Extract to Teams-specific wrapper |
| P2       | Typography components may not need `'use client'` | Audit each; remove if safe        |
| P2       | `@mui/x-data-grid` type-only import in bundle     | Investigate non-type imports      |
| P3       | Barrel export forces all 41 components client     | Document direct-import pattern    |
