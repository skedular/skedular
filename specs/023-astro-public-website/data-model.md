# Data Model: Astro Public Website

## Public Website App

Represents the new Astro app in the web monorepo.

**Fields**

- `name`: `public-web`
- `path`: `src/web/apps/public-web`
- `packageName`: `public-web`
- `framework`: Astro
- `siteMode`: static
- `outputDirectory`: `dist`
- `localPort`: app-local Astro dev/preview port chosen to avoid existing app ports
- `scripts`: `dev`, `build`, `preview`, `start`, `check`, `lint`, `lint-fix`, `format`
- `workspaceParticipation`: pnpm workspace package discovered by `src/web/pnpm-workspace.yaml`
- `deploymentTargets`: Cloudflare Pages/Workers static assets, Vercel static Astro project

**Relationships**

- Owns one Home Page for v1.
- May later own additional public website pages after content migration.
- Belongs to the `src/web` pnpm workspace and Turborepo task graph.

**Validation Rules**

- Must not require a server runtime for v1.
- Must not depend on product app authentication, Relay, or generated GraphQL artifacts.
- Must not change existing apps except shared workspace/root script registration when needed.
- Must expose standard scripts so `turbo run build`, `turbo run lint`, and `turbo run format` can include it.
- Must keep user-facing copy in American English.

## Home Page

Represents the single public website page delivered by this feature.

**Fields**

- `route`: `/`
- `title`: concise browser/page title including Skedular
- `metaDescription`: short summary of Skedular's workspace-management purpose
- `heroBrand`: Skedular
- `heroHeadline`: high-level workspace-management value proposition
- `heroSummary`: minimal description of desks, rooms, hybrid teams, and co-working/business audiences
- `primaryCta`: link to the Skedular app or sign-up flow
- `secondaryCta`: optional support/contact or learn-more link if useful
- `featureHighlights`: small set of high-level value statements, not a full feature migration
- `integrationMention`: Slack and Microsoft Teams support, if included in minimal copy
- `footerLinks`: minimal support/contact and copyright links needed for credibility

**Relationships**

- Belongs to Public Website App.
- Uses current public site research as source context, without copying or migrating full WordPress content.
- Links externally to the Skedular app sign-up and optional support/contact destinations.

**Validation Rules**

- Must avoid placeholder content.
- Must not include placeholder pages or navigation to unavailable pages.
- Must remain meaningful with JavaScript disabled.
- Must be readable and usable on mobile without horizontal scrolling.
- Must include accessible link text and sufficient color contrast.
- Must not claim features beyond the high-level positioning visible on the current public site.

## Deployment Configuration

Represents the documented hosting approach for the static output.

**Fields**

- `buildCommand`: package manager command that runs the app build from the monorepo or app directory
- `outputDirectory`: `src/web/apps/public-web/dist` when deploying from repository root, or `dist` when deploying from the app root
- `primaryTarget`: Cloudflare Pages or Cloudflare Workers static assets
- `fallbackTarget`: Vercel static Astro project
- `runtimeAdapter`: none for v1
- `environmentVariables`: none required for v1
- `analyticsHook`: documented future location for Cloudflare Web Analytics or Vercel Analytics

**Validation Rules**

- Must be deployable without server-side rendering.
- Must not require secrets for the initial static build.
- Must document Cloudflare and Vercel settings in README.
- Must keep adapters out until server-side functionality is required.

## Build Diagnostics

Represents observable validation during local and CI builds.

**Fields**

- `astroCheck`: static Astro diagnostics and type/content checks
- `astroBuild`: static production build output and warnings
- `outputSummary`: page count and output size information surfaced by Astro/build tooling
- `lintResult`: app-local lint command result
- `formatResult`: app-local format command behavior

**Validation Rules**

- Build warnings and errors must not be suppressed.
- Build output must not include sensitive environment values.
- README must identify where analytics can be added later.

## State Transitions

### Public Website App

```text
Planned -> Scaffolded -> Integrated in workspace -> Validated -> Deployable
```

### Home Page

```text
Drafted -> Reviewed for brand accuracy -> Built -> Previewed -> Accepted
```

### Deployment Configuration

```text
Documented -> Build output verified -> Cloudflare-ready
                              -> Vercel-ready
```
