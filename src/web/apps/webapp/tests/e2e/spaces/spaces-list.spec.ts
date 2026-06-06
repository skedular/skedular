import { expect, test } from '@playwright/test';
import { captureStepScreenshot, expectAppShell, setupApiMocks } from '../../../../../scripts/e2e-helpers';

test.describe('webapp spaces discovery surface', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId: 'webapp' });
  });

  test('spaces-list opens the marketplace discovery app shell without backend services', async ({ page }, testInfo) => {
    await page.goto('/');

    await expectAppShell(page, 'webapp', 'public-discovery');
    await expect(page.locator('body')).toContainText(/Skedular|Find|Book|workspace|location/i);

    await captureStepScreenshot(page, testInfo, 'webapp', 'spaces-list', 'marketplace-shell');
  });
});
