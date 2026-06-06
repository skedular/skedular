# Local UI Test Automation

The Playwright UI suites cover `webapp`, `webapp-spaces`, and `webapp-teams`. They run against local Next.js dev servers and use Playwright route mocks for API calls, so backend services are not required.

## Prerequisites

- Node.js 22
- pnpm through Corepack
- Playwright Chromium installed for the app being tested
- ffmpeg for documentation media capture because Playwright records WebM and the capture script converts it to MP4/H.264

## Run Tests

From `src/web`:

```bash
pnpm test:e2e all --run
pnpm test:e2e webapp --run
pnpm test:e2e webapp-spaces --run
pnpm test:e2e webapp-teams --run
```

From an app directory:

```bash
pnpm test:e2e --run
```

## Capture Documentation Media

From `src/web`:

```bash
pnpm capture:media webapp login-flow --output ./docs-media
pnpm capture:media webapp-spaces spaces-list --output ./docs-media
pnpm capture:media webapp-teams teams-list --output ./docs-media
```

Capture runs use a 1920x1080 viewport, record video, capture PNG screenshots at test-defined steps, and convert recorded videos to MP4/H.264 when the test run completes.

## Output

Default artifacts are written under `src/web/.test-artifacts/`. Media capture output is grouped by app, scenario, and run timestamp.

## Mocking

Shared API route mocks live in `src/web/scripts/e2e-helpers.ts`. If a GraphQL operation is not registered, the helper returns a clear mock error so stale or missing mock coverage is visible during test runs.
