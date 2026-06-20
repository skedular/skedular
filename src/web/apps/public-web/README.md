# Skedular Public Website

`public-web` is the static Astro public website for Skedular. It covers public workspace discovery, Teams, Spaces, pricing, company/about content, terms, privacy, resources, support, feature pages, comparison pages, redirects, and launch review inventories. It does not perform direct public-site booking or checkout.

## Required Public URLs

The build requires the public site URL plus public destination URL variables:

| Variable                            | Purpose                                          |
| ----------------------------------- | ------------------------------------------------ |
| `PUBLIC_WEB_SITE_URL`               | Canonical public site URL for SEO files          |
| `PUBLIC_SKEDULAR_APP_URL`           | Search, booking, and app-forwarding actions      |
| `PUBLIC_SKEDULAR_SIGNUP_URL`        | Login, sign-up, and get-started actions          |
| `PUBLIC_SKEDULAR_TEAMS_APP_URL`     | Try Teams actions                                |
| `PUBLIC_SKEDULAR_SPACES_APP_URL`    | Try Spaces actions                               |
| `PUBLIC_SKEDULAR_DEMO_URL`          | Demo, contact sales, and contact support actions |
| `PUBLIC_SKEDULAR_BECOME_HOST_URL`   | Become-a-host actions                            |
| `PUBLIC_SKEDULAR_SLACK_INSTALL_URL` | Slack install actions for Skedular Teams         |

The build intentionally fails when any variable is missing or empty. Staging and production values must be supplied by environment-specific deployment configuration. Do not hardcode those destination domains in source content, tests, or deployment scripts.

## Install

From `src/web`:

```bash
pnpm install
```

## Run Locally

From the repository root:

```bash
PUBLIC_WEB_SITE_URL=https://www.example.test \
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_TEAMS_APP_URL=https://teams.example.test \
PUBLIC_SKEDULAR_SPACES_APP_URL=https://spaces.example.test \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://host.example.test/start \
PUBLIC_SKEDULAR_SLACK_INSTALL_URL=https://slack.example.test/install \
pnpm --dir src/web/apps/public-web dev
```

Astro serves the site at `http://localhost:15008`.

## Validate

Run app-local validation:

```bash
PUBLIC_WEB_SITE_URL=https://www.example.test \
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_TEAMS_APP_URL=https://teams.example.test \
PUBLIC_SKEDULAR_SPACES_APP_URL=https://spaces.example.test \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://host.example.test/start \
PUBLIC_SKEDULAR_SLACK_INSTALL_URL=https://slack.example.test/install \
pnpm --dir src/web/apps/public-web test
```

```bash
PUBLIC_WEB_SITE_URL=https://www.example.test \
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_TEAMS_APP_URL=https://teams.example.test \
PUBLIC_SKEDULAR_SPACES_APP_URL=https://spaces.example.test \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://host.example.test/start \
PUBLIC_SKEDULAR_SLACK_INSTALL_URL=https://slack.example.test/install \
pnpm --dir src/web/apps/public-web check
```

```bash
pnpm --dir src/web/apps/public-web lint
```

```bash
PUBLIC_WEB_SITE_URL=https://www.example.test \
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_TEAMS_APP_URL=https://teams.example.test \
PUBLIC_SKEDULAR_SPACES_APP_URL=https://spaces.example.test \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://host.example.test/start \
PUBLIC_SKEDULAR_SLACK_INSTALL_URL=https://slack.example.test/install \
pnpm --dir src/web/apps/public-web build
```

The production build writes static output to `dist/` and emits a JSON build summary with page count and output size. Build output must not print full public destination URL values.

## Route Families

- `/`
- `/teams`
- `/spaces`
- `/pricing`
- `/about`
- `/company` redirects to `/about`
- `/terms-of-service`
- `/privacy-policy`
- `/resources`
- `/resources/<resource-slug>`
- `/compare/<comparison-slug>`

Current public blog URLs are represented in `src/data/current-public-content.ts` and redirected through `astro.config.mjs`. Current Skedular company, terms, and privacy URLs are represented in `src/data/company-page.ts`, `src/data/legal-pages.ts`, and `src/data/source-audit.ts`.

## Content Inventory Workflow

Review these data files before launch:

- `src/data/current-public-content.ts` for migrated blog content.
- `src/data/company-page.ts` for the migrated About Skedular page.
- `src/data/legal-pages.ts` for Terms of Service and Privacy Policy summary content awaiting owner review.
- `src/data/draft-coverage.ts` for every major draft heading and bullet group.
- `src/data/claim-review.ts` for capability, pricing, and competitor claims.
- `src/data/launch-review.ts` for human-quality copy, accessibility, SEO, pricing, and participant review evidence.
- `src/data/source-audit.ts` for source/reference inputs and review dates.

## Cloudflare Pages

Infrastructure lives under `infrastructure/workspaces/staging` and `infrastructure/workspaces/production`. The CI/CD pipeline applies each workspace, builds the static site with environment-specific public URL values, and uploads `dist/` with Wrangler.

## Cloudflare Web Analytics

Public-site measurement uses Cloudflare Web Analytics instead of Google Analytics. Web Analytics is enabled manually in Cloudflare, not through Terraform, because Cloudflare's RUM/Web Analytics API endpoint can reject the existing deployment token even when normal Pages/DNS permissions work. Do not add Google Analytics, Google Tag Manager, or a manual Cloudflare beacon snippet to the Astro layout unless the analytics provider decision changes.

To enable it manually:

1. In Cloudflare, open Workers & Pages.
2. Select the public-web Pages project for staging or production.
3. Open Metrics.
4. Select Enable under Web Analytics.
5. Redeploy the site so Cloudflare injects the beacon.

Review consent, regional privacy rules, and legal-page wording before adding advertising pixels or a second analytics provider.

For a manual direct upload after the Pages project exists:

```bash
PUBLIC_WEB_SITE_URL=https://www.example.test \
PUBLIC_SKEDULAR_APP_URL=https://app.example.test \
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up \
PUBLIC_SKEDULAR_TEAMS_APP_URL=https://teams.example.test \
PUBLIC_SKEDULAR_SPACES_APP_URL=https://spaces.example.test \
PUBLIC_SKEDULAR_DEMO_URL=https://demo.example.test/book \
PUBLIC_SKEDULAR_BECOME_HOST_URL=https://host.example.test/start \
PUBLIC_SKEDULAR_SLACK_INSTALL_URL=https://slack.example.test/install \
pnpm build
```

```bash
CLOUDFLARE_API_TOKEN=... CLOUDFLARE_ACCOUNT_ID=... pnpm exec wrangler pages deploy dist \
  --project-name=staging-public-web \
  --branch=main
```

## Performance Review

Run Lighthouse/Core Web Vitals review against staging when a staging URL is available. If staging is unavailable, run the review against a local static preview and record the limitation here.

Target measurements:

- Lighthouse Performance 95+
- Lighthouse Accessibility 95+
- Lighthouse SEO 100
- Lighthouse Best Practices 100
- LCP under 1.5s
- INP under 200ms
- CLS under 0.05

## Vercel

Import the site as a static Astro project and configure all four public URL variables as build-time environment variables.

```text
Root directory: src/web/apps/public-web
Build command: pnpm build
Output directory: dist
```
