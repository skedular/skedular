import { describe, expect, it, vi } from 'vitest';
import { createWebAppSwitcherModel } from './app-switcher-config';

describe('createWebAppSwitcherModel', () => {
  it('creates Skedular switcher destinations from environment configuration', () => {
    const logger = { info: vi.fn(), warn: vi.fn() };

    const model = createWebAppSwitcherModel({
      logger,
      env: {
        NEXT_PUBLIC_SITE_URL: 'https://app.skedular.test/',
        NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL: 'https://teams.skedular.test/',
        NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL: 'https://spaces.skedular.test/',
      } as NodeJS.ProcessEnv,
    });

    expect(model.currentAppId).toBe('webapp');
    expect(model.destinations.map((destination) => [destination.appId, destination.availability])).toEqual([
      ['webapp', 'current'],
      ['webapp-teams', 'available'],
      ['webapp-spaces', 'available'],
    ]);
    expect(logger.info).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'web_app_switcher_configuration', availableDestinationCount: 2 }),
      'Web app switcher configuration resolved',
    );
  });
});
