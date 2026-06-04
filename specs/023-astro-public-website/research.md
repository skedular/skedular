# Research: Astro Public Website

## Decision: Use `public-web` as the app name

**Rationale**: The user explicitly requested `public-web` unless the repository has a stronger naming convention. Existing app names use `webapp-*` for authenticated product apps and help/documentation companions. The public marketing site is a distinct public-facing website, so `public-web` is clear and avoids implying it is another product shell.

**Alternatives considered**:

- `webapp-public`: rejected because it suggests another product app variant rather than a marketing website.
- `webapp-marketing`: rejected because the requested scope is broader public website ownership and may later include pages beyond marketing content.
- `website`: rejected because it is less specific inside a monorepo that already has multiple web apps.

## Decision: Create an Astro static site under `src/web/apps/public-web`

**Rationale**: The stakeholder explicitly requires Astro, and the feature is a mostly static public website with one minimal page. Astro's default static output matches the scope without adding a server runtime, React/Relay, or product-app authentication concerns.

**Alternatives considered**:

- Use the existing Next.js app pattern: rejected because the user explicitly requested Astro, and the page does not need App Router, Relay, or server-side product data.
- Put static HTML under `assets/` or `docs/`: rejected because the site must be a first-class app with local dev, build, preview, lint, and formatting scripts.
- Add pages to `webapp`: rejected because the public website should not couple marketing content to the authenticated product app.

## Decision: Keep the app static with no Astro adapter for v1

**Rationale**: The first version has one static page and no server-side functionality. Astro builds static sites to `dist/` by default. Vercel deploys static Astro projects with no extra adapter, and Cloudflare can host static assets from `dist/` through Pages or Workers static assets.

**Alternatives considered**:

- Add `@astrojs/cloudflare`: rejected for v1 because the feature does not need on-demand rendering or Cloudflare runtime APIs.
- Add `@astrojs/vercel`: rejected for v1 because Vercel does not require an adapter for static Astro sites.
- Configure SSR now for future flexibility: rejected because it adds deployment/runtime complexity before a real server-side requirement exists.

## Decision: Integrate with the existing pnpm workspace and Turborepo tasks

**Rationale**: `src/web/package.json` already defines `apps/*` workspaces and `turbo run` scripts. Adding `src/web/apps/public-web/package.json` is enough for workspace discovery. The app should expose the standard scripts expected by Turbo: `dev`, `build`, `start`, `lint`, `lint-fix`, `format`, plus `preview` and `check` for Astro validation.

**Alternatives considered**:

- Add a separate workspace root: rejected because it fragments dependency installation and task orchestration.
- Run Astro only through app-local scripts: rejected because the app must participate in monorepo build, lint, and format conventions.
- Add only `dev` and `build`: rejected because the spec requires preview and relevant validation.

## Decision: Use minimal app-local styling and no shared React UI packages for v1

**Rationale**: The public site uses Astro and has only one static page. `@skedular/ui` and `@skedular/shared` are designed for the React/MUI/Relay product apps. Pulling them into Astro would add unnecessary React runtime and package coupling for the first minimal site.

**Alternatives considered**:

- Reuse `@skedular/ui`: rejected for v1 because the static Astro page can express brand tone with local CSS and avoids React hydration.
- Create a new shared marketing design package: rejected because one page does not justify a new abstraction.
- Copy product app components: rejected because it would violate shared-package ownership and create drift.

## Decision: Model the public page as a single content contract, not API contracts

**Rationale**: The feature exposes a public web page, not a backend API. The useful contract is therefore a UI/content/deployment contract defining required page sections, links, accessibility expectations, and deployment outputs.

**Alternatives considered**:

- OpenAPI or GraphQL contract: rejected because no API surface is introduced.
- No contract artifact: rejected because the feature still has externally visible behavior and deployment expectations.

## Decision: Use current website research only for high-level positioning

**Rationale**: The current site positions Skedular as smart workspace management for hybrid teams, businesses, and co-working spaces, with desk and room booking, team availability, Slack/MS Teams integrations, analytics/reporting, privacy controls, and a free trial CTA. The implementation should represent those themes minimally without migrating or rewriting WordPress content.

**Alternatives considered**:

- Full WordPress content migration: rejected because it is explicitly out of scope.
- Deep content rewrite: rejected because a separate research task will handle proper website content.
- Placeholder content: rejected because the page must represent Skedular accurately.

## Decision: Validate with Astro check/build/preview and monorepo lint/format

**Rationale**: The app is a static frontend. The right verification is `astro check`, `astro build`, local preview, lint/format scripts, and optionally the root Turbo build/lint if time permits. No backend unit or integration tests are needed because there is no executable business workflow, persistence, or cross-domain integration.

**Alternatives considered**:

- Backend-style unit tests: rejected because no backend behavior is added.
- End-to-end browser automation: deferred because the first implementation is a single static page; manual preview plus build validation is proportionate.
- No validation: rejected because the app must be a first-class workspace package.
