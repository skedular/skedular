import { expect, type Page, type TestInfo } from '@playwright/test';
import fs from 'node:fs';
import path from 'node:path';

export type WebAppId = 'webapp' | 'webapp-spaces' | 'webapp-teams';

type MockOptions = {
  appId: WebAppId;
};

type GraphqlPayload = {
  query?: string;
  operationName?: string;
};

const screenshotTimestamp = () => new Date().toISOString().replace(/[:.]/g, '-');

const logTestEvent = (event: string, context: Record<string, unknown>) => {
  console.info(
    JSON.stringify({
      event,
      timestamp: new Date().toISOString(),
      ...context,
    }),
  );
};

const getOperationName = ({ operationName, query }: GraphqlPayload) => {
  if (operationName) {
    return operationName;
  }

  const match = query?.match(/\b(query|mutation)\s+([A-Za-z0-9_]+)/);
  return match?.[2];
};

const getGraphqlResponse = (operationName: string | undefined, appId: WebAppId) => {
  if (!operationName) {
    return {
      errors: [{ message: `Missing GraphQL operationName for ${appId} UI test mock.` }],
    };
  }

  if (operationName.includes('pageAuthSignIn')) {
    return {
      data: {
        organizationPublic: null,
      },
    };
  }

  if (operationName.includes('pageHome')) {
    return {
      data: {
        marketplaceLocations: {
          totalCount: 0,
          edges: [],
          pageInfo: {
            hasNextPage: false,
            endCursor: null,
          },
        },
      },
    };
  }

  if (operationName.includes('noOrganizationLandingPage')) {
    return {
      data: {
        me: {
          id: `${appId}-user-1`,
          isOnboardingDone: true,
        },
        myOrganizations:
          appId === 'webapp-teams'
            ? [
                {
                  name: 'Design Team',
                  uniqueId: 'design-team',
                  customDomain: 'design-team',
                  logoUrl: null,
                },
              ]
            : [
                {
                  name: 'Central Workspace',
                  uniqueId: 'central-workspace',
                  customDomain: 'central-workspace',
                  logoUrl: null,
                },
              ],
      },
    };
  }

  if (operationName.includes('noOrganizationRootShell')) {
    return {
      data: {
        me: {
          id: `${appId}-user-1`,
          email: 'test@example.com',
          emails: ['test@example.com'],
          title: null,
          givenName: 'Test',
          middleName: null,
          familyName: 'User',
          name: 'Test User',
          photoUrl: null,
          isOnboardingDone: true,
        },
        customerReadinessSynced: true,
        isAzureTenantInstalled: false,
        azureTenantOrganization: null,
        myOrganizations:
          appId === 'webapp-teams'
            ? [
                {
                  name: 'Design Team',
                  uniqueId: 'design-team',
                  customDomain: 'design-team',
                  logoUrl: null,
                },
              ]
            : [
                {
                  name: 'Central Workspace',
                  uniqueId: 'central-workspace',
                  customDomain: 'central-workspace',
                  logoUrl: null,
                },
              ],
        pendingOrganizationInvitationsCount: 0,
        pendingTeamInvitationsCount: 0,
      },
    };
  }

  return {
    errors: [
      {
        message: `No UI test mock response registered for GraphQL operation "${operationName}" in ${appId}. Add it in src/web/scripts/e2e-helpers.ts.`,
      },
    ],
  };
};

export const setupApiMocks = async (page: Page, { appId }: MockOptions) => {
  logTestEvent('ui_test_mock_registration_started', { appId, testId: appId });

  await page.route('**/api/v1/graphql', async (route) => {
    const request = route.request();
    const payload = (request.postDataJSON() ?? {}) as GraphqlPayload;
    const operationName = getOperationName(payload);
    const response = getGraphqlResponse(operationName, appId);

    logTestEvent('ui_test_graphql_mock_used', {
      appId,
      testId: appId,
      operationName: operationName ?? 'unknown',
      hasErrors: 'errors' in response,
    });

    await route.fulfill({
      status: 'errors' in response ? 500 : 200,
      contentType: 'application/json',
      body: JSON.stringify(response),
    });
  });

  await page.route('**/graphql', async (route) => {
    const request = route.request();
    const payload = (request.postDataJSON() ?? {}) as GraphqlPayload;
    const operationName = getOperationName(payload);
    const response = getGraphqlResponse(operationName, appId);

    await route.fulfill({
      status: 'errors' in response ? 500 : 200,
      contentType: 'application/json',
      body: JSON.stringify(response),
    });
  });

  await page.route('**/api/users/**', async (route) => {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify({
        id: `${appId}-user-1`,
        name: 'Test User',
        email: 'test@example.com',
      }),
    });
  });

  logTestEvent('ui_test_mock_registration_completed', { appId, testId: appId });
};

export const captureStepScreenshot = async (page: Page, testInfo: TestInfo, appId: WebAppId, scenario: string, step: string) => {
  if (process.env.PLAYWRIGHT_CAPTURE_SCREENSHOTS !== 'true' && process.env.PLAYWRIGHT_RECORD_VIDEO !== 'true') {
    return;
  }

  const baseDir = process.env.SCREENSHOT_OUTPUT_DIR ?? testInfo.outputPath('screenshots');
  const scenarioDir = path.join(baseDir, appId, scenario);
  fs.mkdirSync(scenarioDir, { recursive: true });

  const filePath = path.join(scenarioDir, `${appId}-${scenario}-${step}-${screenshotTimestamp()}.png`);
  await page.setViewportSize({ width: 1920, height: 1080 });
  await page.screenshot({ path: filePath, type: 'png' });
  await testInfo.attach(`${appId}-${scenario}-${step}`, { path: filePath, contentType: 'image/png' });

  logTestEvent('ui_test_screenshot_captured', {
    appId,
    testId: testInfo.title,
    scenario,
    step,
    filePath,
    width: 1920,
    height: 1080,
  });
};

export const expectAppShell = async (page: Page, appId: WebAppId, reviewScope: string) => {
  await expect(page.locator(`html[data-product-app="${appId}"]`)).toBeVisible();
  await expect(page.locator(`[data-product-app="${appId}"][data-review-scope="${reviewScope}"]`)).toBeVisible();
};
