# Skedular Host (`webapp-host`)

Skedular Host is the host-facing web application for individuals who rent out spaces. It lets space owners manage their locations and products, track bookings, and monitor commission — all under a dedicated **HOST** organization type.

This app is part of the **026-scheduler-host-app** feature. See the full spec at [`specs/026-scheduler-host-app/`](../../specs/026-scheduler-host-app/).

## Key features

- **Host org type** — Organizations are created with `type: HOST`, distinguishing space-renting individuals from `PRIVATE` and `MARKETPLACE` orgs.
- **Location & Product CRUD** — Create, read, update, and delete locations and the products (spaces) available at each location.
- **Auto-Resource creation** — Products automatically provision backing resources so they are bookable without manual backend setup.
- **Full-place booking** — Supports renting an entire place, not just individual resources within it.
- **5% commission** — A default 5% commission is charged on host bookings (configurable via `commissionPercentage`).

## Running locally

This is a Next.js 16 (App Router) + TypeScript 6 app in a pnpm monorepo. It runs on **port 15005**.

```bash
# from the repo root
pnpm install                # install workspace dependencies
pnpm --filter webapp-host dev   # starts the dev server on http://localhost:15005
```

Environment variables are documented in `.env.example`.

## Running tests

```bash
# Unit / component tests (Vitest)
pnpm --filter webapp-host test

# Watch mode
pnpm --filter webapp-host test:watch

# End-to-end tests (Playwright)
pnpm --filter webapp-host test:e2e
```

Linting and formatting:

```bash
pnpm --filter webapp-host lint
pnpm --filter webapp-host format
```

## Project structure

```
webapp-host/
├── Dockerfile                    # Multi-stage Docker build (lint → test → build → final)
├── package.json                  # name: webapp-host, scripts, deps
├── src/
│   ├── app/                      # Next.js App Router pages
│   │   ├── bookings/             # Booking management views
│   │   ├── dashboard/            # Host dashboard
│   │   ├── locations/            # Location CRUD views
│   │   ├── map/                  # Location map view (Leaflet)
│   │   └── products/             # Product CRUD views
│   ├── components/               # Shared UI components
│   │   ├── commission-history/
│   │   ├── dashboard-layout/
│   │   ├── location-card/
│   │   ├── locationMap/
│   │   ├── product-form/
│   │   └── product-table/
│   ├── queries/                  # GraphQL (Relay) queries
│   ├── rootPages/                # Top-level page composition
│   └── test/                     # Test setup/helpers
├── infrastructure/               # App instrumentation / config
└── tests/                        # Playwright e2e tests
```

The app shares UI and domain packages from the workspace (`@skedular/ui`, `@skedular/shared`) and talks to the backend over GraphQL via Relay.

## Spec & documentation

- Feature spec: [`specs/026-scheduler-host-app/spec.md`](../../specs/026-scheduler-host-app/spec.md)
- Plan: [`specs/026-scheduler-host-app/plan.md`](../../specs/026-scheduler-host-app/plan.md)
- Data model: [`specs/026-scheduler-host-app/data-model.md`](../../specs/026-scheduler-host-app/data-model.md)
- Tasks: [`specs/026-scheduler-host-app/tasks.md`](../../specs/026-scheduler-host-app/tasks.md)
