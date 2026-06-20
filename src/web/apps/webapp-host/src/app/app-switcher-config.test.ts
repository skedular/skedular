import { describe, expect, it } from 'vitest';
import { createHostAppSwitcherModel } from './app-switcher-config';

describe('createHostAppSwitcherModel', () => {
  it('creates the Host app switcher destinations', () => {
    const model = createHostAppSwitcherModel({
      logConfiguration: false,
      env: {
        NEXT_PUBLIC_SKEDULAR_APP_URL: 'https://app.skedular.test/',
        NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL: 'https://teams.skedular.test/',
        NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL: 'https://spaces.skedular.test/',
        NEXT_PUBLIC_SITE_URL: 'https://host.skedular.test/',
      },
    });

    expect(model.currentAppId).toBe('webapp-host');
    expect(model.destinations.map(({ appId, availability }) => [appId, availability])).toEqual([
      ['webapp', 'available'],
      ['webapp-teams', 'available'],
      ['webapp-spaces', 'available'],
      ['webapp-host', 'current'],
    ]);
  });
});
