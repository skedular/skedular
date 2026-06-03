# Relay Queries and SSR Architecture Assessment

**Generated**: 2026-06-03  
**Tasks**: T023–T026  
**Scope**: Relay environment, AuthKitProvider, AuthenticatedRelayProvider, SSR fetch path

---

## Current Architecture

### Provider Stack (all 3 apps)

```
layout.tsx (Server Component)
  └── ClientRootLayout (client-root-layout.tsx) → 'use client'
        └── PaletteModeProvider
              └── ThemeProvider
                    └── CssBaseline
                          └── DatePickerLocalizationProvider
                                └── AuthKitProvider        ← WorkOS auth
                                      └── AppAuthenticatedRelayProvider  ← uses useAuth()
                                            └── RelayProvider
                                                  └── {children}  ← All routes
```

**Key observation**: `AuthKitProvider` wraps **all** children including public routes. This is the root cause of the global client boundary — every page is inside a client component that manages auth state.

### AuthenticatedRelayProvider

```typescript
// @skedular/shared/src/providers/authenticated-relay-provider.tsx
'use client';

const AuthenticatedRelayProvider = ({ authLoading, children, teamsToken }) => {
  const inMsTeams = useContext(InMsTeamsContext);

  if (!inMsTeams && authLoading) return null;  // Holds rendering until auth resolves

  return <RelayProvider token={inMsTeams ? teamsToken : undefined}>
    {children}
  </RelayProvider>;
};
```

**Critical issue**: `authLoading` check blocks rendering of all children until auth state resolves. Combined with the global client boundary, this means:

1. Page HTML arrives empty (React shell)
2. Client hydrates
3. Auth state resolves (`authLoading = false`)
4. Relay queries fire
5. Data arrives
6. Page renders with content

This is a **4-waterfall** sequence before users see content.

---

## Relay Environment (SSR Support)

```typescript
// @skedular/shared/src/utils/relay-environment.ts
import { isServer } from "./constants";
// ...
export function createNetwork(endpoint: string, token?: string | null | undefined) {
  // network/fetch setup
}
```

The relay environment checks `isServer` — which means it has awareness of server-side rendering. This is the foundation for potential Relay SSR with `loadQuery` + Suspense on the server.

**Positive finding**: The relay environment is architected for SSR — it just isn't being used that way because `AuthKitProvider` forces a client boundary before any data fetching.

---

## Relay Query Patterns in Routes

### webapp — Relay Query Roots

| Route/Component                       | Query Type          | Preloaded?                | SSR Opportunity                        |
| ------------------------------------- | ------------------- | ------------------------- | -------------------------------------- |
| Home (`/`)                            | `useLazyLoadQuery`  | No                        | Yes — if auth extracted to route group |
| `/auth/signin`                        | `usePreloadedQuery` | Yes (preloaded on server) | High — public route                    |
| `/auth/signup`                        | `usePreloadedQuery` | Yes (preloaded on server) | High — public route                    |
| `/marketplace/locations/[locationId]` | `usePreloadedQuery` | Yes                       | High — public location data            |
| Organization storefront               | `useLazyLoadQuery`  | No                        | Possible — org data is cacheable       |

### webapp-teams / webapp-spaces

All routes use `useLazyLoadQuery` (no preloaded queries). All routes require authentication. SSR opportunity is low without the auth architecture refactor.

---

## SSR Architecture Refactor Path

### Option A: Route Group AuthKit Scoping (Recommended)

Move `AuthKitProvider` from root layout to route group layouts:

```
app/
  (public)/           ← No auth provider
    layout.tsx        ← Server Component ✓
    auth/signin/
    install-slack/
  (authenticated)/    ← Auth provider here
    layout.tsx        ← Client Component with AuthKitProvider
    marketplace/
    organizations/
  layout.tsx          ← Root: ThemeProvider, CssBaseline only (Server Component)
```

**Impact**:

- `(public)` routes: Full server rendering, no auth overhead
- `(authenticated)` routes: Same as today, no regression
- Est. bundle reduction for public routes: ~180 KB (WorkOS AuthKit + Relay provider overhead)
- LCP improvement for auth pages: 300–600ms (eliminating auth waterfall)

**Effort**: High — requires restructuring all 3 apps' route layouts.  
**Risk**: Medium — auth flow changes need thorough testing.

### Option B: Suspense-Based Deferred Auth

Keep global `AuthKitProvider` but wrap auth-dependent children in Suspense:

```typescript
const InnerRootLayout = () => (
  <ThemeProvider>
    <AuthKitProvider>
      <Suspense fallback={<AppShellSkeleton />}>
        <AuthGatedContent />
      </Suspense>
    </AuthKitProvider>
  </ThemeProvider>
);
```

**Impact**: Renders app shell skeleton on server, defers auth check to Suspense boundary.  
**Limitation**: Still client-rendered; Suspense here doesn't enable SSR without `renderToPipeableStream` server-side integration.

### Option C: Auth-Aware Server Actions + Relay Server Prefetch

Use Next.js Server Actions for auth token resolution + Relay `loadQuery` on server:

```typescript
// layout.tsx (Server Component)
import { withAuth } from "@workos-inc/authkit-nextjs";
const { user, accessToken } = await withAuth({ ensureSignedIn: false });
// Pass token to Relay Environment for server-side prefetch
```

**Impact**: Data prefetched on server, page renders with content, auth state hydrated client-side.  
**Effort**: Very high — requires significant Relay + AuthKit integration work.

---

## Relay Query SSR Feasibility by Route

| Route                                 | Query Type | Auth Required | SSR Feasible Now? | Path                    |
| ------------------------------------- | ---------- | ------------- | ----------------- | ----------------------- |
| `/auth/signin`                        | preloaded  | No            | **Yes**           | Option A (route group)  |
| `/auth/signup`                        | preloaded  | No            | **Yes**           | Option A                |
| `/install-slack`                      | none       | No            | **Yes**           | Option A                |
| `/marketplace/locations/[locationId]` | preloaded  | No (public)   | **Yes**           | Option A                |
| `/` (home)                            | lazy       | No (public)   | Possible          | Option A + C            |
| Any authenticated route               | lazy       | Yes           | No (today)        | Option A + C (phase 2+) |

---

## Streaming / Partial Prerendering (PPR)

Next.js 16 supports Partial Prerendering (PPR) — render static shells on server, stream dynamic parts.

**Applicability**:

- PPR requires the static shell to be identifiable at build time
- All three apps' routes are fully dynamic (auth-conditional) — PPR cannot precompute the static shell
- Exception: `/auth/signin` and `/auth/signup` pages have a completely static shell (form container, logo, branding) with only the org-specific content being dynamic

**PPR candidate**: Auth pages (`/auth/signin`, `/auth/signup`) could use PPR with `experimental_ppr = true` per route — static form shell streamed immediately, org branding loaded after org resolution.

---

## Summary

| Finding                                        | Impact   | Action                                 |
| ---------------------------------------------- | -------- | -------------------------------------- |
| `AuthKitProvider` at root forces global client | High     | Option A: route group scoping          |
| `authLoading` blocks children render           | High     | Suspense-based deferred render         |
| Relay environment has `isServer` support       | Positive | Ready for SSR when auth unblocks       |
| Auth routes use preloaded queries              | Positive | SSR-ready if in `(public)` route group |
| All admin routes use lazy queries              | Neutral  | Need Option C for SSR benefit          |
