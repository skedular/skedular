import { expect, test } from '@playwright/test';
import { captureStepScreenshot, expectAppShell, setupApiMocks } from '../../../../scripts/e2e-helpers';

test.describe('webapp-teams core UI flows', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId: 'webapp-teams' });
  });

  test('teams-list opens the teams app shell without backend services', async ({ page }, testInfo) => {
    await page.goto('/');

    await expectAppShell(page, 'webapp-teams', 'private-organisation-entry');
    await expect(page.locator('body')).toBeVisible();

    await captureStepScreenshot(page, testInfo, 'webapp-teams', 'teams-list', 'entry');
  });

  test('login-flow keeps the entry shell local until explicitly clicked', async ({ page }, testInfo) => {
    await page.goto('/');

    await expectAppShell(page, 'webapp-teams', 'private-organisation-entry');
    await expect(page.getByRole('heading', { name: 'Welcome to Skedular Teams' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Create account' })).toHaveAttribute('href', '/signup');
    await expect(page.getByRole('link', { name: 'Sign in' })).toHaveAttribute('href', '/signin');
    await captureStepScreenshot(page, testInfo, 'webapp-teams', 'login-flow', 'entry-shell');
  });
});
