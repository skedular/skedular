import { expect, test } from '@playwright/test';
import { captureStepScreenshot, expectAppShell, setupApiMocks } from '../../../../scripts/e2e-helpers';

const appId = 'webapp-host';

test.describe('webapp-host core UI flows', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId });
  });

  test('landing page uses the authenticated product entry pattern', async ({ page }, testInfo) => {
    await page.goto('/');

    await expectAppShell(page, appId, 'host-entry');
    await expect(page.getByText('Welcome to Skedular Host')).toBeVisible();
    await expect(page.getByRole('link', { name: 'Sign in', exact: true })).toBeVisible();
    await expect(page.getByRole('link', { name: /create account/i })).toBeVisible();

    await captureStepScreenshot(page, testInfo, appId, 'landing', 'authenticated-entry');
  });

  test('locations page loads canonical Host locations', async ({ page }, testInfo) => {
    await page.goto('/locations');

    await expect(page.getByText('My locations')).toBeVisible();
    await expect(page.getByText('Garden Studio')).toBeVisible();
    await expect(page.getByText('1 Garden Lane, Auckland')).toBeVisible();

    await captureStepScreenshot(page, testInfo, appId, 'locations', 'relay-list');
  });

  test('location and product details use canonical Relay queries', async ({ page }) => {
    await page.goto('/locations/location-1');
    await expect(page.getByText('Garden Studio')).toBeVisible();
    await expect(page.getByText('Full-Day Studio Workshop')).toBeVisible();

    await page.goto('/locations/location-1/products/product-1');
    await expect(page.getByText('Entire place')).toBeVisible();
    await expect(page.getByText('120 USD')).toBeVisible();
    await expect(page.getByText('Payment: CARD')).toBeVisible();
  });

  test('location edit submits canonical patch mutations', async ({ page }) => {
    await page.goto('/locations/location-1/edit');
    await page.getByLabel('Name').fill('Updated Garden Studio');
    await page.getByRole('button', { name: 'Save changes' }).click();
    await expect(page).toHaveURL(/\/locations\/location-1/);
  });

  test('navigate to create location, fill form, submit', async ({ page }, testInfo) => {
    await page.goto('/locations/create');

    await expect(page.getByRole('heading', { name: 'Create Location' })).toBeVisible();
    await expect(page.getByText(/Add the physical venue your customers will book/i)).toBeVisible();

    await page.getByLabel('Name').fill('Garden Studio');
    await page.getByLabel('Street address').fill('1 Garden Lane');
    await page.getByRole('textbox', { name: 'City', exact: true }).fill('Auckland');
    await page.getByLabel('Country').fill('New Zealand');
    await page.getByRole('button', { name: 'Create location' }).click();
    await expect(page).toHaveURL(/\/locations\/location-created/);

    await captureStepScreenshot(page, testInfo, appId, 'create-location', 'empty-form');
  });

  test('navigate to create product at a location, fill form, submit', async ({ page }, testInfo) => {
    await page.goto('/locations/location-1/products/create');

    await expect(page.getByRole('heading', { name: 'Create Product' })).toBeVisible();
    await expect(
      page.getByText(/Products define how customers can book your entire place/i),
    ).toBeVisible();

    await page.getByLabel('Name').fill('Full-Day Studio Workshop');
    await page.getByLabel('Price').fill('120');
    await page.getByLabel('Currency').fill('USD');
    await page.getByLabel('Purchase cadence').selectOption('DAILY');

    await captureStepScreenshot(page, testInfo, appId, 'create-product', 'filled-form');

    await page.getByRole('button', { name: 'Create product' }).click();
    await expect(page).toHaveURL(/\/locations\/location-1\/products\/product-created/);

    await captureStepScreenshot(page, testInfo, appId, 'create-product', 'after-submit');
  });

  test('dashboard shows Host booking, commission, and payout history', async ({ page }, testInfo) => {
    await page.goto('/dashboard');

    await expect(page.getByText('Garden Studio dashboard')).toBeVisible();
    await expect(page.getByText('Recent bookings')).toBeVisible();
    await expect(page.getByText('Commission history')).toBeVisible();
    await expect(page.getByText('$95.00')).toBeVisible();

    await captureStepScreenshot(page, testInfo, appId, 'dashboard', 'stats-placeholder');
  });
});
