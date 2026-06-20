# Web Apps Topology

This directory contains all web products in the monorepo.

## Main Products

- `webapp` - baseline main web app.
- `webapp-teams` - teams variant (scaffolded, landing-page-only route).
- `webapp-spaces` - spaces variant (scaffolded, landing-page-only route).

## Public Website

- `public-web` - static Astro public website for Skedular. It is a marketing and public-information surface, not an authenticated product app.

## Shared Package

- `../packages/ui` - shared UI primitives used by main apps.

## Workspace Notes

- Each app keeps its own infrastructure workspaces under `infrastructure/workspaces/`.
- Main app workspace environments: `staging`, `common_resources`, `production`.
- Help app workspace environments: `staging`, `common_resources`, `production`.
