# Developer Quickstart: Smart Organization Landing Page

**Feature**: 015-smart-org-landing-page  
**Branch**: `015-smart-org-landing-page`

---

## Prerequisites

- pnpm 11+ installed
- Docker running (for local dev dependencies)
- Organization API and Customer API reachable (via `docker-compose-min.yml` or Aspire)
- An authenticated user account in the local environment

---

## Local Setup

```bash
# From repo root — start minimum local services (Postgres, Kafka, Redis, etc.)
docker compose -f docker-compose-min.yml up -d

# Install web dependencies (from repo root)
cd web && pnpm install

# Run webapp-teams dev server
cd web/apps/webapp-teams && pnpm dev

# Or run webapp-spaces dev server (separate terminal)
cd web/apps/webapp-spaces && pnpm dev

# Or run webapp dev server (separate terminal)
cd web/apps/webapp && pnpm dev
```

---

## Testing the Three Landing Page States

### State 1: No organizations (0 orgs)

**Setup**: Use a freshly created user account that has not yet joined or created any
organization.

1. Sign in to webapp-teams (e.g. `http://localhost:3000`)
2. Navigate to the root landing page (`/`)
3. **Expected**: A centered panel with a heading such as "Get started" or "No organizations
   found", a brief description, and a "Create an organization" button
4. **Expected**: No left navigation sidebar in the DOM (inspect with browser DevTools —
   `<NoOrganizationLeftSideNavigationMenu>` should not be present)
5. Click the "Create an organization" button → should navigate to `/organizations/add-private`

### State 2: One organization (1 org)

**Setup**: Use a user account that belongs to exactly one private organization.

1. Sign in to webapp-teams at `/`
2. **Expected**: A centered panel showing a single org card with the org name and logo/avatar
3. **Expected**: No left navigation sidebar
4. Click the org card → should navigate to the org home (e.g.
   `/{integratedPlatform}/organizations/{orgId}`)

### State 3: Multiple organizations (N > 1 orgs)

**Setup**: Use a user account that belongs to two or more private organizations.

1. Sign in to webapp-teams at `/`
2. **Expected**: A centered panel listing all orgs, each as a card with name and logo/avatar
3. **Expected**: No left navigation sidebar
4. Clicking any card navigates to that org's home

### Repeat for webapp-spaces

- Use a user with Marketplace organizations
- Create org link should navigate to `/organizations/add-marketplace`

### Verify left nav on non-landing pages

1. Navigate to any non-root page (e.g. `/organizations/add-private`)
2. **Expected**: Left nav IS present (either collapsed or expanded) — `hideSideNav` is only
   passed on the root landing page

---

## After Fragment Changes — Regenerate Relay Artifacts

```bash
# webapp-teams
cd web/apps/webapp-teams && pnpm relay

# webapp-spaces
cd web/apps/webapp-spaces && pnpm relay
```

Commit the updated `src/queries/__generated__/` files alongside the fragment source changes.

---

## Running Unit Tests

```bash
# webapp-teams
cd web/apps/webapp-teams && pnpm test

# webapp-spaces
cd web/apps/webapp-spaces && pnpm test
```

Unit tests for `NoOrganizationLandingContent` should cover all three states (0, 1, N orgs)
with mocked Relay data and verify:

- Correct heading and body text rendered
- Correct CTA button and link href for each state
- No left nav rendered when `hideSideNav` is passed
- Org cards show name and avatar

---

## Smoke Test Checklist

- [ ] `/` in webapp-teams with 0 orgs → no-org prompt + no left nav
- [ ] `/` in webapp-teams with 1 org → single org card + no left nav
- [ ] `/` in webapp-teams with 2+ orgs → multi-org list + no left nav
- [ ] `/` in webapp-spaces with 0 orgs → no-org prompt + no left nav
- [ ] `/` in webapp-spaces with 1 org → org card visible + no left nav
- [ ] `/` in webapp with any state → no left nav
- [ ] Non-root page in any app → left nav still present
- [ ] Org card click navigates to correct org home URL
- [ ] Create org CTA in teams → `/organizations/add-private`
- [ ] Create org CTA in spaces → `/organizations/add-marketplace`
- [ ] App bar profile menu accessible (notifications + settings reachable) on landing page with no left nav (FR-007)
