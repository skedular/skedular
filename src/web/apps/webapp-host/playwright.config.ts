import { defineConfig, devices } from '@playwright/test';

const port = process.env.PORT ?? '15006';
const baseURL = process.env.PLAYWRIGHT_BASE_URL ?? `http://localhost:${port}`;
const recordVideo = process.env.PLAYWRIGHT_RECORD_VIDEO === 'true';
const captureScreenshots = process.env.PLAYWRIGHT_CAPTURE_SCREENSHOTS === 'true';

export default defineConfig({
  testDir: './tests/e2e',
  fullyParallel: true,
  timeout: 60_000,
  expect: {
    timeout: 10_000,
  },
  retries: process.env.CI ? 2 : 0,
  outputDir: process.env.VIDEO_OUTPUT_DIR ?? '.test-artifacts/playwright',
  reporter: process.env.CI ? [['list'], ['junit', { outputFile: 'test-results/e2e-junit.xml' }]] : 'list',
  use: {
    baseURL,
    trace: 'on-first-retry',
    viewport: { width: 1920, height: 1080 },
    screenshot: captureScreenshots ? 'on' : 'only-on-failure',
    video: recordVideo ? { mode: 'on', size: { width: 1920, height: 1080 } } : 'off',
  },
  webServer: {
    command: `pnpm exec next dev --turbopack --hostname 127.0.0.1 --port ${port}`,
    url: baseURL,
    reuseExistingServer: false,
    timeout: 120_000,
    env: {
      ...process.env,
      SKEDULAR_UI_TEST_BYPASS_AUTH: 'true',
      NEXT_PUBLIC_SKEDULAR_UI_TEST_BYPASS_AUTH: 'true',
      NEXT_DIST_DIR: '.next-e2e',
    },
  },
  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        viewport: { width: 1920, height: 1080 },
      },
    },
    {
      name: 'chromium-headed',
      use: {
        ...devices['Desktop Chrome'],
        headless: false,
        viewport: { width: 1920, height: 1080 },
      },
    },
  ],
});
