# Quick Start: UI Test Automation

## Prerequisites

- Node.js 22 (matching `src/web/package.json`)
- pnpm through Corepack
- Chromium browser installed by Playwright
- ffmpeg for documentation media capture, used to convert Playwright WebM recordings to MP4/H.264

## Installation

```bash
cd src/web
pnpm install
pnpm --dir apps/webapp exec playwright install chromium
```

## Running Tests Locally

### All apps with mocked backend:

```bash
pnpm test:e2e all --run
```

### Single app:

```bash
pnpm test:e2e webapp --run
pnpm test:e2e webapp-spaces --run
pnpm test:e2e webapp-teams --run
```

### With video capture enabled:

```bash
PLAYWRIGHT_RECORD_VIDEO=true pnpm test:e2e webapp --run
PLAYWRIGHT_CAPTURE_SCREENSHOTS=true pnpm test:e2e webapp --run
```

## Writing New Tests

1. Create test file: `apps/<app-name>/tests/e2e/<feature>.spec.ts`
2. Use Playwright's assertions and shared mock helpers:
```typescript
import { test, expect } from '@playwright/test';
import { setupApiMocks } from '../../../../scripts/e2e-helpers';

test('should display app entry page', async ({ page }) => {
  await setupApiMocks(page, { appId: 'webapp-spaces' });
  await page.goto('/');
  await expect(page.getByRole('link', { name: 'Sign in' })).toBeVisible();
});
```

3. Add or update mock responses in `src/web/scripts/e2e-helpers.ts`

## Capturing Media for Documentation

```bash
pnpm capture:media webapp login-flow --output ./docs/images
pnpm capture:media webapp-spaces spaces-list --output ./docs/images
pnpm capture:media webapp-teams teams-list --output ./docs/images
```

This command:
- Runs the specified test scenario
- Records video of the execution at 1920x1080
- Converts Playwright WebM recordings to MP4/H.264
- Takes PNG screenshots at key steps at 1920x1080
- Saves outputs to the configured directory

## CI/CD Integration

The `ui-tests.yml` workflow runs on pull requests:

```yaml
- name: Run UI tests
  run: pnpm test:e2e webapp --run
```

Tests fail fast if any assertion fails, providing clear error messages.

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Browser not found | Run `pnpm --dir apps/webapp exec playwright install chromium` |
| Slow tests | Check route mocks before increasing Playwright timeouts |
| Mock not matching | Add the GraphQL operation to `src/web/scripts/e2e-helpers.ts` |
| Video not capturing | Use `pnpm capture:media <app> <scenario>` or set `PLAYWRIGHT_RECORD_VIDEO=true` |
| MP4 missing | Install ffmpeg and rerun the capture command |
