# Quickstart: Floor Plan Setup Page Redesign

**Feature**: `018-floor-plan-setup-redesign`
**Date**: 2026-05-30

## Prerequisites

- Node.js / pnpm installed
- All three webapps' dependencies installed (`pnpm install` from `src/web/`)
- Docker Compose services running for local API access (optional for visual validation)

## Running the Webapps Locally

```bash
# From repo root — start all three webapps simultaneously
cd src/web
pnpm dev
```

Or start each individually:

```bash
# Main webapp (typically http://localhost:3000)
cd src/web/apps/webapp && pnpm dev

# Teams webapp (typically http://localhost:3001)
cd src/web/apps/webapp-teams && pnpm dev

# Spaces webapp (typically http://localhost:3002)
cd src/web/apps/webapp-spaces && pnpm dev
```

## Navigating to the Redesigned Pages

Once a webapp is running and you are logged in as a location manager:

**Add Floor Plan**:
`/organizations/{customDomain}/locations/{locationId}/floor-plans/add`

**Edit Floor Plan**:
`/organizations/{customDomain}/locations/{locationId}/floor-plans/admin/{floorPlanId}`

## Running Tests

```bash
# All web tests (Vitest + React Testing Library)
cd src/web && pnpm test

# Tests for a specific webapp
cd src/web/apps/webapp && pnpm test
```

## Regenerating Relay Types (after fragment changes)

```bash
# From repo root — runs relay compiler for all webapps
src/web/apps/webapp/scripts/generate.sh
```

## Visual Acceptance Checklist

After the redesign is implemented, verify each of these manually or via screenshot comparison:

- [ ] Add Floor Plan page in **webapp**: standard page background (no dark app bar), centered content card
- [ ] Edit Floor Plan page in **webapp**: same layout as Add; canvas inside `SettingsSectionCard`
- [ ] Add Floor Plan page in **webapp-teams**: matches Teams-embedded design conventions
- [ ] Edit Floor Plan page in **webapp-teams**: matches Teams-embedded design conventions
- [ ] Add Floor Plan page in **webapp-spaces**: matches Spaces design conventions
- [ ] Edit Floor Plan page in **webapp-spaces**: matches Spaces design conventions
- [ ] Page title reads "Add Floor Plan" (not "Add Location" — bug fix included)
- [ ] Canvas drag-and-drop still works: resources can be positioned on the floor plan image
- [ ] Auto-save on Edit page triggers after name change and after resource drag (check network tab)
- [ ] All three webapps use aligned app-local components — verify the same layout and behaviour in each copy

## Key Files After Implementation

```text
src/web/apps/webapp/src/components/floorPlan/
├── addFloorPlan/
│   ├── add-floor-plan.tsx           ← App-local redesigned Add component
│   └── index.ts
└── editFloorPlan/
    ├── edit-floor-plan.tsx          ← App-local redesigned Edit component
    └── index.ts

# Same app-local implementation pattern for webapp-teams and webapp-spaces
```
