import { describe, expect, it, vi } from 'vitest';
import { createSpacesAppSwitcherModel } from './app-switcher-config';

describe('createSpacesAppSwitcherModel', () => {
  it('creates Skedular Spaces switcher destinations from environment configuration', () => {
    const logger = { info: vi.fn(), warn: vi.fn() };

    const model = createSpacesAppSwitcherModel({
      logger,
      env: {
        NEXT_PUBLIC_SKEDULAR_APP_URL: 'https://app.skedular.test/',
        NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL: 'https://teams.skedular.test/',
        NEXT_PUBLIC_SITE_URL: 'https://spaces.skedular.test/',
      } as NodeJS.ProcessEnv,
    });

    expect(model.currentAppId).toBe('webapp-spaces');
    expect(model.destinations.find((destination) => destination.appId === 'webapp-spaces')).toMatchObject({ availability: 'current' });
    expect(model.availableDestinationCount).toBe(2);
    expect(logger.info).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'web_app_switcher_configuration', currentAppId: 'webapp-spaces' }),
      'Web app switcher configuration resolved',
    );
  });
});
