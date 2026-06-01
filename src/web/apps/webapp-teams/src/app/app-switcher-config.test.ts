import { describe, expect, it, vi } from 'vitest';
import { createTeamsAppSwitcherModel } from './app-switcher-config';

describe('createTeamsAppSwitcherModel', () => {
  it('creates Skedular Teams switcher destinations from environment configuration', () => {
    const logger = { info: vi.fn(), warn: vi.fn() };

    const model = createTeamsAppSwitcherModel({
      logger,
      env: {
        NEXT_PUBLIC_SKEDULAR_APP_URL: 'https://app.skedular.test/',
        NEXT_PUBLIC_SITE_URL: 'https://teams.skedular.test/',
        NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL: 'https://spaces.skedular.test/',
      } as NodeJS.ProcessEnv,
    });

    expect(model.currentAppId).toBe('webapp-teams');
    expect(model.destinations.find((destination) => destination.appId === 'webapp-teams')).toMatchObject({ availability: 'current' });
    expect(model.availableDestinationCount).toBe(2);
    expect(logger.info).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'web_app_switcher_configuration', currentAppId: 'webapp-teams' }),
      'Web app switcher configuration resolved',
    );
  });
});
