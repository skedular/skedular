import { expect, test } from '@playwright/test';
import { captureStepScreenshot, setupApiMocks } from '../../../../../scripts/e2e-helpers';

test.describe('webapp login flow', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId: 'webapp' });
  });

  test('login-flow renders the password and social sign-in options without backend services', async ({ page }, testInfo) => {
    await page.goto('/auth/signin?returnTo=/marketplace/bookings');

    await expect(page.getByRole('heading', { name: 'Sign in' })).toBeVisible();
    await expect(page.getByLabel('Email')).toBeVisible();
    await expect(page.getByLabel('Password')).toBeVisible();
    await expect(page.getByRole('button', { name: 'Sign in' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Continue with Google' })).toBeVisible();
    await expect(page.getByRole('link', { name: 'Continue with Microsoft' })).toBeVisible();
    await expect(page.locator('input[name="returnTo"]')).toHaveValue('/marketplace/bookings');

    await captureStepScreenshot(page, testInfo, 'webapp', 'login-flow', 'signin-form');
  });

  test('login-flow shows clear validation copy for invalid credentials', async ({ page }, testInfo) => {
    await page.goto('/auth/signin?error=invalid_credentials');

    await expect(page.getByText('The email or password is incorrect.')).toBeVisible();
    await captureStepScreenshot(page, testInfo, 'webapp', 'login-flow', 'invalid-credentials');
  });
});
