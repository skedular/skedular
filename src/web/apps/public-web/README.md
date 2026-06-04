# Skedular Public Website

`public-web` is the static Astro public website for Skedular. It contains one route, `/`, and does not depend on product authentication, Relay, backend APIs, or a server runtime.

## Prerequisites

- Node.js 22
- pnpm 11.5.1
- A public build-time CTA URL provided through `PUBLIC_SKEDULAR_SIGNUP_URL`

The build intentionally fails when `PUBLIC_SKEDULAR_SIGNUP_URL` is missing or empty. There is no fallback URL.

## Install

From `src/web`:

```bash
pnpm install
```

The existing `apps/*` workspace pattern discovers this package automatically.

## Run Locally

From the repository root:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web dev
```

Or from `src/web`:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm public-web#dev
```

Astro serves the site at `http://localhost:15006`. Changes to `src/pages/index.astro` and `src/styles/global.css` should hot reload without restarting the process.

## Validate

Run app-local validation:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web test
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web check
pnpm --dir src/web/apps/public-web lint
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web build
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web preview
```

The production build writes static output to `dist/` and emits a JSON build summary with the page count and output size. Build warnings and errors are not suppressed.

Run workspace-level validation from the repository root:

```bash
pnpm --dir src/web check:workspace-version-sync
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web build
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web test
pnpm --dir src/web lint
pnpm --dir src/web format
```

## Cloudflare Pages

No Astro adapter is required for v1. The repository provisions separate direct-upload Cloudflare Pages projects and custom domains:

| Environment | Pages project           | Custom domain                   | Sign-up URL                           |
| ----------- | ----------------------- | ------------------------------- | ------------------------------------- |
| Staging     | `staging-public-web`    | `stagingpublic.getskedular.com` | `https://staging.skedular.app/signup` |
| Production  | `production-public-web` | `public.getskedular.com`        | `https://skedular.app/signup`         |

Infrastructure lives under `infrastructure/workspaces/staging` and `infrastructure/workspaces/production`. The main CI/CD pipeline applies each workspace, builds the static site with the environment-specific `PUBLIC_SKEDULAR_SIGNUP_URL`, and uploads `dist/` with Wrangler.

The `CLOUDFLARE_API_KEY` GitHub secret is used as an API token. It must grant the target account `Pages Read`, `Pages Write`, and permission to edit DNS records in the `getskedular.com` zone.

For a manual direct upload after the Pages project exists:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://staging.skedular.app/signup pnpm build
CLOUDFLARE_API_TOKEN=... CLOUDFLARE_ACCOUNT_ID=... pnpm exec wrangler pages deploy dist \
  --project-name=staging-public-web \
  --branch=main
```

After a Cloudflare URL is available, measure the home page on a standard broadband profile and record whether it loads in under two seconds. Do not substitute a local preview result for the deployed measurement.

## Vercel

Import the site as a static Astro project and configure `PUBLIC_SKEDULAR_SIGNUP_URL` as a public build-time environment variable.

```text
Root directory: src/web/apps/public-web
Build command: pnpm build
Output directory: dist
```

No Vercel adapter is required for v1. Add an adapter only if future work introduces on-demand rendering, middleware, or platform runtime APIs.

## Future Analytics

Once a provider is chosen, add its static page integration in `src/pages/index.astro`. Keep analytics optional and avoid exposing sensitive environment values in static output or build logs.
