import { expect, test } from '@playwright/test';
import { captureStepScreenshot, setupApiMocks } from '../../../../../scripts/e2e-helpers';

test.describe('webapp session exit flow', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId: 'webapp' });
  });

  test('logout-flow returns a signed-out user to the sign-in surface with preserved return path', async ({ page }, testInfo) => {
    await page.goto('/auth/signin?returnTo=/settings');

    await expect(page.getByRole('heading', { name: 'Sign in' })).toBeVisible();
    await expect(page.locator('input[name="returnTo"]')).toHaveValue('/settings');
    await expect(page.getByRole('link', { name: 'Create account' })).toHaveAttribute('href', '/auth/signup?returnTo=%2Fsettings');

    await captureStepScreenshot(page, testInfo, 'webapp', 'logout-flow', 'signed-out-return');
  });
});
