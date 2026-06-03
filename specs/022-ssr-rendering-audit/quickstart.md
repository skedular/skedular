# Quickstart: Bundle Analysis for Web Apps (022)

**Feature**: `022-ssr-rendering-audit`  
**Date**: 2026-06-03

---

## Overview

This guide explains how to install and run `@next/bundle-analyzer` against each of the three web apps to produce the bundle size measurements required by FR-016 and SC-006.

Bundle analysis is a prerequisite for producing the numeric KB estimates in audit findings.

---

## Install `@next/bundle-analyzer`

Run the following from the repo root (pnpm workspace):

```bash
# Add to webapp
pnpm --filter webapp add -D @next/bundle-analyzer

# Add to webapp-teams
pnpm --filter webapp-teams add -D @next/bundle-analyzer

# Add to webapp-spaces
pnpm --filter webapp-spaces add -D @next/bundle-analyzer
```

---

## Configure `next.config.ts` in Each App

Wrap the existing config with `withBundleAnalyzer` in each app's `next.config.ts`:

```ts
import type { NextConfig } from "next";
import withBundleAnalyzer from "@next/bundle-analyzer";
import relayConfig from "./relay.config";

const isVercel = process.env.VERCEL === "1";
const withAnalyzer = withBundleAnalyzer({
  enabled: process.env.ANALYZE === "true",
});

const nextConfig: NextConfig = {
  // ... existing config unchanged ...
};

export default withAnalyzer(nextConfig);
```

**Note**: Apply the same wrapper pattern to `webapp-teams/next.config.ts` and `webapp-spaces/next.config.ts`.

---

## Run the Analyzer

From each app directory:

```bash
# webapp
cd src/web/apps/webapp
ANALYZE=true pnpm build

# webapp-teams
cd src/web/apps/webapp-teams
ANALYZE=true pnpm build

# webapp-spaces
cd src/web/apps/webapp-spaces
ANALYZE=true pnpm build
```

Each run opens two browser tabs (client bundle and server bundle) showing interactive treemaps of all modules.

---

## Interpreting Results

1. **Find the client bundle treemap** — look for the largest blocks by parsed size (not gzip).
2. **Identify heavy modules** to lazy-load:
   - `react-leaflet` / `leaflet` — should only appear on location pages
   - `@mui/x-charts` — should only appear on analytics pages
   - `@mui/x-data-grid*` — should only appear on admin pages
   - `@stripe/*` — should only appear on payment pages
   - `logrocket` — should ideally be deferred
3. **Record KB sizes** (parsed and gzip) for each candidate — these become the numeric estimates in audit findings.
4. **Check for duplicate modules** — same package bundled multiple times from different pnpm hoisting paths.

---

## Recording Results

Record findings in the audit tables using the `estimatedBundleSavingKB` field on `LazyLoadCandidate` entries. Where a library currently appears in the main bundle but only belongs in a specific sub-tree, the saving estimate is the **parsed module size** from the treemap.

Annotate `specs/022-ssr-rendering-audit/audit/lazy-load-candidates.md` with the measured values from each app.
