# Client Boundary Findings

**Generated**: 2026-06-03  
**Tasks**: T029–T031  
**Scope**: `'use client'` directives across all 3 apps and shared packages

---

## Global Summary

| Scope                 | Total Components/Pages  | `'use client'` | Server-compatible     | Coverage                      |
| --------------------- | ----------------------- | -------------- | --------------------- | ----------------------------- |
| `@skedular/ui`        | 44 `.tsx` (excl. tests) | 41 (93%)       | 3 (tests only)        | All real components: client   |
| `@skedular/shared`    | 16 `.tsx`               | 15 (94%)       | 1                     | All providers: client         |
| `webapp` pages        | 28                      | 26 (93%)       | 2 (delegation shells) | 93% client                    |
| `webapp-teams` pages  | 64                      | 64 (100%)      | 0                     | 100% client                   |
| `webapp-spaces` pages | 74                      | 73 (99%)       | 1 (delegation shell)  | 99% client                    |
| `webapp` components   | ~190                    | ~4 (2%)        | ~186 (98%)            | Components are SC-compatible! |

---

## Key Findings

### CB-001: All Component Libraries Are Client-Only (`@skedular/ui` + `@skedular/shared`)

**Severity**: High  
**Impact**: Every consumer route inherits a client boundary from package imports

`@skedular/ui/index.ts` exports 41/44 components with `'use client'`. When a page imports ANY component from `@skedular/ui`, it pulls in a client boundary.

In practice, this means: even if a page removed its own `'use client'` directive, it would still be a client component because `@skedular/ui` components are client components.

**The problem**: The package-level `'use client'` is not just on the component itself — the barrel export (`export * from './commons'`) means importing `@skedular/ui` in a Server Component would force the entire barrel to be evaluated, pulling in all 41 client components.

**Fix for Server Component compatibility**: Import specific components from their direct path rather than the barrel:

```typescript
// ❌ Forces entire UI package client boundary
import { BodyIconTypography } from "@skedular/ui";

// ✅ Only imports the specific server-compatible component
import { BodyIconTypography } from "@skedular/ui/typography/body-icon-typography";
```

**Note**: This only works if the specific component itself doesn't have `'use client'`. Most UI typography components don't (verify before migrating).

---

### CB-002: Page-Level `'use client'` Is the Current Boundary

**Severity**: High  
**Impact**: Establishes the boundary at the worst possible level — top-level page

All 26 client pages in webapp declare `'use client'` at the top of the file. This means:

- React treats the entire page subtree as a client component tree
- No Server Components within the page
- All data fetching is client-side
- All rendering is client-side

The correct pattern for a page with interactive elements is:

```typescript
// ❌ Today: entire page is client
'use client';
export default function BookingPage() {
  const { user } = useAuth();   // needs client
  return <BookingList bookings={...} />;  // doesn't need client
}

// ✅ Better: only interactive parts are client islands
// booking-page.tsx (Server Component)
export default async function BookingPage() {
  const bookings = await loadBookings();  // server-side fetch
  return <BookingList bookings={bookings} />;  // server-rendered
}
// booking-list.tsx — Server Component, no 'use client'
// But: booking actions (cancel, modify) are client islands
```

---

### CB-003: App Root (`ClientRootLayout`) Forces Global Client Tree

**Severity**: Critical  
**Impact**: Root `'use client'` wraps ALL routes — no route can escape the client boundary

From `client-root-layout.tsx`:

```typescript
"use client";
// Wraps ALL routes:
// AuthKitProvider → AppAuthenticatedRelayProvider → ThemeProvider → children
```

This is the single biggest architectural blocker for SSR. See `relay-queries.md` → Option A for the fix path.

---

### CB-004: Hidden Interactive Components (Missing `'use client'`)

**Severity**: Medium  
**Risk**: Silent regression if pages are converted to Server Components

The following components are interactive (use state, effects, or event handlers) but lack `'use client'`:

| Component                      | Category | Why Client Needed           |
| ------------------------------ | -------- | --------------------------- |
| `datePickers/*` (3)            | forms    | MUI date pickers need state |
| `listGridToggle`               | utils    | View mode state             |
| `sorting`                      | utils    | Sort order state            |
| `weekOpeningHours`             | utils    | Multi-day schedule state    |
| `closedOpenAllDayCustomToggle` | utils    | Toggle state                |

**Risk**: If any parent page is converted to a Server Component, these components would error at runtime with "React hooks cannot be called in a Server Component."

**Recommendation**: Add `'use client'` to these 7 components NOW, before any phase 2 conversion work begins. This is a low-risk, defensive change.

---

### CB-005: Relay Query Components — All Client

All Relay query components (`useLazyLoadQuery`, `usePreloadedQuery`, `useFragment`) must run in client context — Relay for React hooks are client-only.

**Exception**: Relay 15+ has experimental server rendering support. Relay 21 (used in this repo) has improved SSR support via `loadQuery` + `readQuery` server pattern. This is a non-trivial migration but is architecturally possible.

**Assessment**: Low-priority for now. Relay SSR would require simultaneous changes to:

1. Route group auth scoping
2. Relay environment server initialization
3. `RelayProvider` accepting server-prefetched data

---

### CB-006: `InMsTeamsContext` — Teams-Specific Client Hook

**Severity**: Medium (webapp-teams, webapp-spaces only)  
**Impact**: All routes in teams/spaces inherit client boundary from context check

`AuthenticatedRelayProvider` calls `useContext(InMsTeamsContext)`. This forces `AuthenticatedRelayProvider` to be a client component even on non-Teams routes.

**Fix**: Move the Teams token check out of `AuthenticatedRelayProvider` and into a conditional wrapper that only applies in `webapp-teams` and `webapp-spaces`. The shared `RelayProvider` should accept an optional token without needing Teams context.

---

## Client Boundary Narrowing Roadmap

In order of impact and feasibility:

### Phase 2A — Quick Wins (Low Risk)

1. **Add `'use client'` to 7 interactive components** (CB-004) — prevents regression
2. **Fix `@skedular/ui` barrel contamination** (LL-001) — removes 521 KB from all routes
3. **Direct path imports for server-compatible UI components** — enables future SC migration

### Phase 2B — Medium Changes (Medium Risk)

4. **Extract `AuthKitProvider` to route group layouts** (CB-003, relay-queries → Option A)
   - Enables public routes to be server-rendered
   - ~5 public routes across 3 apps
5. **Move `InMsTeamsContext` check out of shared RelayProvider** (CB-006)
   - Reduces shared library coupling

### Phase 2C — Architectural Refactor (High Risk, High Value)

6. **Convert display-heavy pages to Server Components**
   - Target: Product detail, location detail, booking detail pages
   - Keep interactive parts (booking form, cancel button) as client islands
7. **Relay server-side prefetch** with Next.js Server Actions
   - Eliminates Relay data waterfall on page load

---

## Summary Table

| Finding                                      | Severity | Fix Effort | Phase |
| -------------------------------------------- | -------- | ---------- | ----- |
| CB-001: UI package barrel forces client      | High     | Medium     | 2A    |
| CB-002: Page-level `'use client'` everywhere | High     | Medium     | 2B/2C |
| CB-003: ClientRootLayout global boundary     | Critical | High       | 2B    |
| CB-004: 7 components missing `'use client'`  | Medium   | Low        | 2A    |
| CB-005: Relay query hooks are client-only    | Medium   | Very High  | 2C+   |
| CB-006: InMsTeamsContext in shared provider  | Medium   | Medium     | 2B    |
