# Quickstart: Skedular Competitor Comparison Hub

## Prerequisites

- Node.js 22
- pnpm 11.x
- Public-web dependencies installed under `src/web`

If dependencies are missing:

```bash
pnpm --dir src/web install
```

## Run Locally

From the repository root:

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://spaces.example.test/signup \
pnpm --dir src/web/apps/public-web dev
```

Expected local URL:

```text
http://localhost:15008
```

## App-Local Validation

Run the public-web test suite:

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://spaces.example.test/signup \
pnpm --dir src/web/apps/public-web test
```

Run Astro type/content checks:

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://spaces.example.test/signup \
pnpm --dir src/web/apps/public-web check
```

Run formatting check:

```bash
pnpm --dir src/web/apps/public-web lint
```

Build static output:

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://spaces.example.test/signup \
pnpm --dir src/web/apps/public-web build
```

## Required Route Validation

After build, validate:

- `/compare/index.html` exists.
- These generated comparison pages exist:
  - `/compare/skedular-vs-skedda`
  - `/compare/skedular-vs-officernd`
  - `/compare/skedular-vs-nexudus`
  - `/compare/skedular-vs-gable`
  - `/compare/skedular-vs-robin`
  - `/compare/skedular-vs-officely`
  - `/compare/skedular-vs-envoy`
  - `/compare/skedular-vs-kadence`
  - `/compare/skedular-vs-archie`
  - `/compare/skedular-vs-deskbird`
- These supporting pages exist:
  - `/compare/best-coworking-software`
  - `/compare/best-workspace-management-software`
  - `/compare/best-desk-booking-software`
  - `/compare/skedda-alternatives`
  - `/compare/officernd-alternatives`
  - `/compare/nexudus-alternatives`

Expected results:

- `/compare` lists every generated page above.
- Clicking each `/compare` entry navigates to the correct page path.
- Each generated page links back to `/compare`.
- No removed legacy one-off comparison route is emitted, redirected, aliased, linked, or listed in sitemap data.

## Required Content Validation

Review generated pages for:

- One H1 per page.
- Overview, Feature Matrix, Pricing Comparison, Integration Comparison, Best For, Limitations, Why Teams Choose Skedular, FAQ, and CTA sections on every individual comparison page.
- Supporting pages generated from the same competitor dataset and feature matrix.
- Skedular claims backed by current source references.
- Competitor claims backed by evidence notes or explicit approved review status.
- Unknown states used instead of unsupported competitor claims.
- American spelling and grammar.
- No hardcoded page-specific comparison claims outside shared comparison data.

## Required SEO and Structured Data Validation

Review built output for:

- Unique page titles.
- Unique meta descriptions.
- `/compare` canonical paths for all comparison and supporting pages.
- Open Graph metadata.
- FAQ schema only when matching FAQ content is visible.
- Breadcrumb structured data including `/compare`.
- Sitemap inclusion for every published comparison page.
- No duplicate canonical paths.

## Workspace Validation

From repository root:

```bash
pnpm --dir src/web check:workspace-version-sync
```

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://spaces.example.test/signup \
pnpm --dir src/web build
```

```bash
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://spaces.example.test/signup \
pnpm --dir src/web test
```

```bash
pnpm --dir src/web lint
```

## Manual Launch Review

Before publication, confirm:

- The existing one-off comparison page is removed first.
- The full required page set is complete before publication.
- `/compare` lists all generated pages.
- Competitor claim evidence/review inventory is complete.
- Skedular capability evidence references current specs, help content, current public-web data, routes, pricing data, or implemented contracts.
- Structured data matches visible page content.
- Content inventory maps every generated page back to source records.
