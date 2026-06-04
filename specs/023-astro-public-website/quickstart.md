# Quickstart: Astro Public Website

## 1. Confirm Feature Context

From the repository root:

```bash
git status --short --branch
cat .specify/feature.json
```

Expected feature directory:

```text
specs/023-astro-public-website
```

## 2. Review Planning Artifacts

```bash
sed -n '1,260p' specs/023-astro-public-website/spec.md
sed -n '1,220p' specs/023-astro-public-website/research.md
sed -n '1,220p' specs/023-astro-public-website/contracts/public-website-contract.md
```

## 3. Implement App Structure

Create the app at:

```text
src/web/apps/public-web/
├── README.md
├── astro.config.mjs
├── package.json
├── vitest.config.ts
├── public/
├── scripts/
│   └── report-build.mjs
├── src/
    ├── env.d.ts
    ├── pages/
    │   └── index.astro
    └── styles/
        └── global.css
└── tests/
    ├── build-diagnostics.test.ts
    └── home-page.test.ts
```

Keep v1 to one route only: `/`.

## 4. Install Dependencies

From `src/web`:

```bash
pnpm install
```

The app should be discovered automatically through the existing `apps/*` workspace pattern.

## 5. Run Locally

From the repository root:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web dev
```

Or from `src/web` after app-specific root scripts are added:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm public-web#dev
```

Open the local URL printed by Astro.

## 6. Validate

Run app-local checks:

```bash
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web test
pnpm --dir src/web/apps/public-web check
pnpm --dir src/web/apps/public-web lint
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web build
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web/apps/public-web preview
```

Run workspace-level checks when practical:

```bash
pnpm --dir src/web check:workspace-version-sync
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web build
PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.com/sign-up pnpm --dir src/web test
pnpm --dir src/web lint
pnpm --dir src/web format
```

Record any environment-specific failure separately; do not suppress build or lint warnings.

Run app-local `build` and `test` sequentially. The build-diagnostics tests intentionally create and remove `dist/` to verify successful and failed builds, so running them concurrently against the same app output directory is unsupported.

## 7. Cloudflare Deployment Notes

For Cloudflare Pages or Workers static assets, use a static build with no Astro adapter for v1.

If deploying with app root set to `src/web/apps/public-web`:

```text
Build command: pnpm build
Output directory: dist
Environment variable: PUBLIC_SKEDULAR_SIGNUP_URL
```

If deploying from repository root:

```text
Build command: pnpm --dir src/web/apps/public-web build
Output directory: src/web/apps/public-web/dist
Environment variable: PUBLIC_SKEDULAR_SIGNUP_URL
```

Add a Cloudflare adapter only if future work introduces server-side rendering, middleware, bindings, or Cloudflare runtime APIs.

## 8. Vercel Deployment Notes

For Vercel, import the project as an Astro static site.

Recommended monorepo settings:

```text
Root directory: src/web/apps/public-web
Build command: pnpm build
Output directory: dist
Environment variable: PUBLIC_SKEDULAR_SIGNUP_URL
```

No Vercel adapter is required for the v1 static site. Add `@astrojs/vercel` only if future work needs on-demand rendering or Vercel runtime APIs.

## 9. Manual Acceptance Review

Confirm:

- the page clearly identifies Skedular in the first viewport
- the page explains hybrid workspace management at a high level
- the CTA links to the Skedular app or sign-up flow
- no placeholder pages or placeholder copy exist
- the page remains useful with JavaScript disabled
- mobile widths do not introduce horizontal scrolling
- README explains local development and Cloudflare/Vercel deployment
- existing apps are unchanged except shared workspace/root script registration if needed

## 10. Implementation Validation Record

Validated on 2026-06-04:

- `ncu -u` was run in `src/web/apps/public-web`; it upgraded `axe-core` to `^4.12.0`, and all other public-web package ranges were already current.
- `PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up pnpm --dir src/web/apps/public-web check` passed with zero errors, warnings, or hints.
- `PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up pnpm --dir src/web/apps/public-web build` passed and emitted `{"event":"public-web.build.complete","pageCount":1,"outputBytes":16983}`.
- `PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up pnpm --dir src/web/apps/public-web test` passed 4 tests, including structured build metadata, missing CTA configuration failure, configured CTA links, semantic landmarks, and critical axe violations.
- `pnpm --dir src/web/apps/public-web lint` passed.
- Production preview served the static page successfully at `http://localhost:15006/`.
- Astro dev-server hot reload was verified by changing home-page copy, observing the updated browser DOM without restarting the process, and reverting the temporary change.
- Browser review confirmed one `h1`, semantic header/main/footer landmarks, useful content without client JavaScript, descriptive CTA links, and no horizontal overflow at a 375px client width.
- `pnpm --dir src/web check:workspace-version-sync` passed. The existing version-sync tool reports clear failures when shared dependency versions or required lockfile versions diverge.
- `PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up pnpm --dir src/web build` passed across the web workspace.
- `PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up pnpm --dir src/web test` passed across all test-bearing web packages.
- `pnpm --dir src/web lint` passed across the web workspace.
- `pnpm --dir src/web format` passed across the web workspace without changing existing app files.
- No Cloudflare URL is available yet, so the deployed under-two-second performance measurement is recorded as an environment limitation. A local preview result was not substituted.
- Scope review confirmed no backend contracts, generated artifacts, existing product app routes, or existing app dependencies were modified.

Validated on 2026-06-05 for Cloudflare deployment infrastructure:

- Added direct-upload Cloudflare Pages projects `staging-public-web` and `production-public-web`.
- Added custom domains `stagingpublic.getskedular.com` and `public.getskedular.com`.
- `terraform validate -no-color` passed for both public web Terraform workspaces with Cloudflare provider `5.19.1`.
- `PUBLIC_SKEDULAR_SIGNUP_URL=https://app.example.test/sign-up pnpm --dir src/web/apps/public-web build` passed and emitted the structured build summary.
- `pnpm --dir src/web/apps/public-web exec wrangler --version` reported Wrangler `4.98.0`.
- Local Terraform apply could not use the existing S3 state backend because AWS credentials were unavailable in the local environment.
- Local Wrangler deployment authenticated to the Cloudflare account, but the configured token was rejected for Pages project access. Grant the `CLOUDFLARE_API_KEY` token `Pages Read` and `Pages Write` before running the pipeline or a manual upload.
- Live checks confirmed `stagingpublic.getskedular.com` and `public.getskedular.com` do not resolve yet, so no partial deployment was reported as complete.
