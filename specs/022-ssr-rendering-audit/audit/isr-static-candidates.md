# ISR and Static Pre-rendering Candidates

**Generated**: 2026-06-03  
**Summary**: No ISR/Static candidates across all three apps (confirmed).

---

## Assessment

After auditing all 166 routes (28 webapp + 64 webapp-teams + 74 webapp-spaces):

### Static (`output: 'export'` / `generateStaticParams`)

**Result: 0 routes suitable for full static export.**

Reasons:

1. All meaningful content is user-specific or organization-specific
2. All product/location pages require an authenticated customer or org context to render
3. Custom domain resolution at request time prevents static pregeneration
4. `useAuth()` in the root layout means the first HTML is always user-aware

### ISR (Incremental Static Regeneration — `revalidate`)

**Result: 0 routes suitable for ISR today.**

Reasons:

1. Route content is user-session-dependent (subscriptions, bookings, admin dashboards)
2. The marketplace storefront pages (`/marketplace/organizations/[customDomain]`) are the closest ISR candidates — but they resolve org context from `window.location.hostname` at render time, not from URL params, so `generateStaticParams` cannot enumerate them
3. Public routes (`/auth/signin`, `/install-slack`) are static HTML shells but are wired through the same global `ClientRootLayout` which requires `'use client'` — Next.js cannot split these into a separate static rendering path without architectural changes

### Theoretical ISR Path (Phase 2 investigation)

If custom domain resolution were moved to Next.js middleware (rewrites), the marketplace storefront pages could potentially become ISR'd per organization. This would require:

1. Middleware to rewrite `[customDomain].skedular.com/` → `/marketplace/organizations/[customDomain]`
2. `generateStaticParams` to enumerate all active organization custom domains
3. `revalidate` set to an appropriate cache window (e.g., `3600` for 1 hour)
4. Auth handled as client-side enhancement rather than SSR requirement

**Effort**: High architectural change. Not recommended for the current audit phase. Tracked as theoretical direction in `summary.md`.

---

## Root Cause

The fundamental blocker for all ISR/Static opportunities is the global client root layout:

```typescript
// src/web/apps/webapp/src/app/client-root-layout.tsx
'use client';
// ...
const InnerRootLayout = ({ children }: PropsWithChildren) => (
  <ThemeProvider>
    <CssBaseline />
    <DatePickerLocalizationProvider>
      <AuthKitProvider>          {/* ← Forces client boundary at root */}
        <AppAuthenticatedRelayProvider>
          {children}
        </AppAuthenticatedRelayProvider>
      </AuthKitProvider>
    </DatePickerLocalizationProvider>
  </ThemeProvider>
);
```

`AuthKitProvider` wraps the entire app at the root. Moving it to route-group layouts (one for public routes, one for authenticated routes) would enable static/ISR rendering for public routes.

See `relay-queries.md` for the architectural assessment of this refactor.
