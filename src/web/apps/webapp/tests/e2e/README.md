# UI Test Automation

This directory contains Playwright end-to-end tests for the webapp.

## Running Tests

### Local Development (without backend)
```bash
pnpm test:e2e --run
```

### With Video Capture
```bash
PLAYWRIGHT_RECORD_VIDEO=true pnpm test:e2e --run
VIDEO_OUTPUT_DIR=./my-videos pnpm test:e2e --run
```

### Headed Mode (for debugging)
```bash
npx playwright test tests/e2e --project=chromium-headed --headful
```

## Test Organization

- `auth/` - Login, logout, and authentication flows
- `spaces/` - Space management and listing
- `media/` - Media capture integration tests
- `ci/` - CI-specific test scenarios

## Writing New Tests

1. Create a new file in the appropriate subdirectory (e.g., `auth/login.spec.ts`)
2. Use Playwright's testing API:
   ```typescript
   import { test, expect } from '@playwright/test';
   
   test('should do something', async ({ page }) => {
     await page.goto('/path');
     await expect(page.locator('.element')).toBeVisible();
   });
   ```
3. Mock API responses using Playwright's route mocking

## Media Capture

For capturing videos and screenshots for documentation:

```bash
pnpm capture:media webapp <scenario-name>
```

Output is saved to `.test-artifacts/videos/` by default.

## CI Integration

Tests run automatically on pull requests via `.github/workflows/ui-tests.yml`.
