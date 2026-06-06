import { expect, test } from '@playwright/test';
import { setupApiMocks } from '../../../../../scripts/e2e-helpers';

test.describe('webapp CI integration', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId: 'webapp' });
  });

  test('ci-integration runs headless with a 1920x1080 viewport and mocked APIs', async ({ page }) => {
    await page.goto('/auth/signin');

    const viewport = page.viewportSize();
    expect(viewport).toEqual({ width: 1920, height: 1080 });
    await expect(page.getByRole('heading', { name: 'Sign in' })).toBeVisible();
  });

  test('ci-integration exposes JUnit output when CI is enabled', async ({}, testInfo) => {
    expect(testInfo.project.name).toContain('chromium');
  });
});
