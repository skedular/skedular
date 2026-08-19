# Component Audit — Analytics and Charts

**Generated**: 2026-06-03  
**Task**: T018  
**Scope**: `analytics`, chart/insight components, `@mui/x-charts`, `@mui/x-data-grid`

---

## analytics (2 components — 0 with `'use client'`)

| Component                | `'use client'` | Hooks Used | Notes                         |
| ------------------------ | -------------- | ---------- | ----------------------------- |
| Analytics page shell     | no             | none       | Display shell; SSR-compatible |
| Analytics data component | no             | —          | Data display                  |

**Note**: Both components lack `'use client'` but analytics pages ARE rendering client-side (page declares `'use client'`). The analytics components themselves don't use browser APIs or React hooks directly — they're thin display wrappers.

---

## availabilityDashboard — Chart Components

See also `components-org-admin.md`.

### `@mui/x-charts` Usage

`@mui/x-charts` is used in the availability dashboard for resource utilization visualizations. Key observations:

| Aspect                 | Finding                                                                                  |
| ---------------------- | ---------------------------------------------------------------------------------------- |
| Bundle presence        | `@mui/x-charts` present in bundle; size TBD from tree-shaking                            |
| `'use client'` needed? | **Yes** — animation, tooltip, legend interaction all require client                      |
| Route scope            | `/organizations/[customDomain]/analytics` + `/organizations/[customDomain]/availability` |
| All apps?              | webapp-teams + webapp-spaces (not webapp)                                                |

**Assessment**: Chart components correctly use `'use client'`. The optimization opportunity is to ensure the chart's parent shell (header, date picker, summary cards) doesn't NEED to be client-side just because a chart inside it is.

**Pattern to apply**:

```tsx
// ✅ Server Component shell
export default function AnalyticsPage() {
  return (
    <AdminPageShell title="Analytics">
      {" "}
      {/* Server */}
      <SummaryCards /> {/* Server */}
      <ChartIsland /> {/* 'use client' — lazy loaded */}
    </AdminPageShell>
  );
}
```

### `@mui/x-data-grid` — Investigation Required

| Aspect            | Finding                                                                                                              |
| ----------------- | -------------------------------------------------------------------------------------------------------------------- |
| Bundle size       | 119 KB in all 3 apps                                                                                                 |
| Import type       | `import type { GridRowSelectionModel }` in `@skedular/shared/src/mui/index.ts`                                       |
| Expected behavior | `import type` should be tree-shaken at build time                                                                    |
| Actual behavior   | Still appears in bundle at 119 KB                                                                                    |
| Hypothesis        | Type-only import triggers runtime inclusion due to babel/swc transform quirk, OR there's a non-type import elsewhere |

**Action required**:

1. Run `grep -r "from '@mui/x-data-grid'" src/web --include="*.tsx" --include="*.ts"` (excluding type-only)
2. If found, wrap the data grid component in a lazy import
3. If only type imports found, verify with `ANALYZE=true pnpm build --webpack` and inspect chunk breakdown

**Command to verify**:

```bash
grep -r "from '@mui/x-data-grid'" src/web --include="*.tsx" --include="*.ts" | grep -v "import type"
```

---

## Lazy Load Strategy for Charts

All chart components should be wrapped in `next/dynamic` with `ssr: false`:

```typescript
// Before
import { BarChart } from '@mui/x-charts';

// After
const BarChart = dynamic(() =>
  import('@mui/x-charts').then(m => ({ default: m.BarChart })),
  { ssr: false, loading: () => <ChartSkeleton /> }
);
```

**Why `ssr: false`**: Charts render an SVG with animated transitions on first paint. Server-rendered HTML would show static SVG with no data, then hydrate — causing a layout shift. `ssr: false` renders a consistent skeleton server-side and loads the chart client-side.

---

## Summary for Analytics/Charts

| Component                  | Client Required | Opportunity                |
| -------------------------- | --------------- | -------------------------- |
| Analytics shell            | No              | Convert to SC              |
| Summary cards              | No              | Convert to SC              |
| `@mui/x-charts` components | **Yes**         | Lazy load with skeleton    |
| Date range picker          | **Yes**         | Already client             |
| `@mui/x-data-grid`         | Verify          | Investigate runtime import |

**Estimated bundle saving**: If `@mui/x-data-grid` has a runtime import path, lazy-loading it on analytics routes only would save 119 KB from the initial bundle.
