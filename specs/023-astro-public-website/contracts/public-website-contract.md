# Public Website Contract

## Scope

This contract defines the externally visible behavior for the first minimal Skedular public website. It is a UI, content, and deployment contract rather than an API contract.

## App Contract

The app MUST live at:

```text
src/web/apps/public-web
```

The app package MUST be discoverable by the existing `src/web` pnpm workspace, which already includes `apps/*`.

The app MUST expose these package scripts:

```json
{
  "dev": "start Astro development server",
  "build": "run Astro validation/build for static output",
  "preview": "preview the built static site locally",
  "start": "alias preview or production preview command",
  "check": "run Astro project diagnostics",
  "test": "run Vitest page, accessibility, and build-diagnostics tests",
  "lint": "run relevant validation without modifying files",
  "lint-fix": "run fixable validation/formatting if applicable",
  "format": "format source files according to workspace style"
}
```

## Page Contract

The app MUST expose exactly one public route for v1:

```text
/
```

The home page MUST include:

- Skedular product name as a first-viewport signal.
- A headline that positions Skedular as workspace management for hybrid teams, businesses, and co-working spaces.
- A short product description mentioning desks, rooms, and workspace usage at a high level.
- A primary call-to-action linking to the Skedular app or sign-up flow configured through `PUBLIC_SKEDULAR_SIGNUP_URL`.
- Minimal supporting content that can mention team availability, Slack/Microsoft Teams integrations, privacy/flexibility, or analytics without attempting a full WordPress migration.
- Accessible page metadata: title, description, language, and viewport.
- A minimal footer with credible support/contact and copyright information.

The home page MUST NOT include:

- Placeholder pages or links to pages that do not exist in this app.
- Placeholder copy such as lorem ipsum or TODO text.
- A full migration of WordPress pages, pricing tables, blog posts, company pages, terms, or privacy copy.
- Product-authenticated UI flows or product dashboard content.
- Server-only behavior required for first render.

## Brand and Content Contract

The first-page copy MUST align with current public-site positioning:

- Skedular helps businesses and co-working spaces manage desks, rooms, and workspace usage.
- Skedular supports hybrid teams and modern workspace coordination.
- Skedular integrates with Slack and Microsoft Teams.
- Skedular should feel professional, clear, and modern.

The first-page copy MUST remain minimal. Detailed content strategy, final marketing copy, full feature taxonomy, pricing, blog content, and WordPress migration are deferred.

## Accessibility Contract

The page MUST:

- Use semantic landmarks (`header`, `main`, `footer`) or equivalent Astro/HTML structure.
- Have one clear page-level heading.
- Provide descriptive link text for CTAs.
- Maintain sufficient color contrast for text and interactive elements.
- Avoid horizontal scrolling on mobile widths.
- Render useful content without client-side JavaScript.

## Deployment Contract

The app MUST build to a static output directory:

```text
dist
```

Cloudflare deployment settings SHOULD be documented as:

```text
Root directory: src/web/apps/public-web (or repo root with equivalent command)
Build command: pnpm build
Output directory: dist
```

If deploying from the repository root, equivalent settings SHOULD be documented as:

```text
Build command: pnpm --dir src/web/apps/public-web build
Output directory: src/web/apps/public-web/dist
```

Vercel deployment settings SHOULD be documented as a static Astro project with no adapter required for v1. If importing from the monorepo root, the Vercel project root SHOULD be set to `src/web/apps/public-web`.

No deployment adapter is required for v1. Adding `@astrojs/cloudflare` or `@astrojs/vercel` is reserved for future on-demand rendering, middleware, or platform-runtime APIs.

`PUBLIC_SKEDULAR_SIGNUP_URL` MUST be configured as a public build-time environment variable. The production build MUST fail clearly when it is missing or empty.

## Validation Contract

Implementation is acceptable when these checks pass or any environment-specific limitation is recorded:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web test
pnpm --dir src/web/apps/public-web check
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web build
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web preview
pnpm --dir src/web/apps/public-web lint
pnpm --dir src/web/apps/public-web format
```

Root-level validation SHOULD include the app through Turborepo:

```bash
pnpm --dir src/web check:workspace-version-sync
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web build
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web test
pnpm --dir src/web lint
pnpm --dir src/web format
```

The implementation MUST not require changes to backend contracts, generated GraphQL/Relay artifacts, OpenAPI clients, or existing product app routes.
