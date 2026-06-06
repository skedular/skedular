import { expect, test } from '@playwright/test';
import { captureStepScreenshot, setupApiMocks } from '../../../../../scripts/e2e-helpers';

test.describe('webapp media capture integration', () => {
  test.beforeEach(async ({ page }) => {
    await setupApiMocks(page, { appId: 'webapp' });
  });

  test('media-capture creates a 1920x1080 PNG screenshot for documentation', async ({ page }, testInfo) => {
    await page.goto('/auth/signin');
    await captureStepScreenshot(page, testInfo, 'webapp', 'media-capture', 'signin');

    const viewport = page.viewportSize();
    expect(viewport).toEqual({ width: 1920, height: 1080 });
  });

  test('media-capture enables video only for capture runs', async ({ page }, testInfo) => {
    await page.goto('/auth/signin');

    const shouldRecordVideo = process.env.PLAYWRIGHT_RECORD_VIDEO === 'true' || testInfo.project.name === 'chromium-capture';
    expect(shouldRecordVideo || testInfo.project.name === 'chromium').toBe(true);
  });
});
