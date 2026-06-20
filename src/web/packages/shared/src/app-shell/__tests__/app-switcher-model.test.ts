import { describe, expect, it } from 'vitest';
import { createAppSwitcherModel, getAvailableAppSwitcherDestinations } from '../app-switcher-model';

describe('app-switcher-model', () => {
  it('orders destinations consistently and marks valid non-current URLs available', () => {
    const model = createAppSwitcherModel({
      currentAppId: 'webapp',
      destinations: {
        webapp: 'https://app.skedular.test/',
        'webapp-teams': 'https://teams.skedular.test/',
        'webapp-spaces': 'https://spaces.skedular.test/',
        'webapp-host': 'https://host.skedular.test/',
      },
    });

    expect(model.destinations.map((destination) => destination.displayName)).toEqual(['Skedular', 'Skedular Teams', 'Skedular Spaces', 'Skedular Host']);
    expect(model.destinations.map((destination) => destination.availability)).toEqual(['current', 'available', 'available', 'available']);
    expect(model.availableDestinationCount).toBe(3);
    expect(model.hasSwitchTargets).toBe(true);
  });

  it('keeps the current app as context without making it an active destination', () => {
    const model = createAppSwitcherModel({
      currentAppId: 'webapp-teams',
      destinations: {
        webapp: 'https://app.skedular.test/',
        'webapp-teams': 'https://teams.skedular.test/',
        'webapp-spaces': 'https://spaces.skedular.test/',
        'webapp-host': 'https://host.skedular.test/',
      },
    });

    expect(model.destinations.find((destination) => destination.appId === 'webapp-teams')).toMatchObject({
      displayName: 'Skedular Teams',
      isCurrent: true,
      availability: 'current',
    });
    expect(getAvailableAppSwitcherDestinations(model).map((destination) => destination.appId)).toEqual(['webapp', 'webapp-spaces', 'webapp-host']);
  });

  it('marks missing, malformed, and unsupported URLs unavailable without blocking valid destinations', () => {
    const model = createAppSwitcherModel({
      currentAppId: 'webapp-spaces',
      destinations: {
        webapp: '',
        'webapp-teams': 'mailto:teams@example.test',
        'webapp-spaces': 'not-a-url',
      },
    });

    expect(model.destinations.map((destination) => [destination.appId, destination.availability])).toEqual([
      ['webapp', 'missing-url'],
      ['webapp-teams', 'invalid-url'],
      ['webapp-spaces', 'current'],
      ['webapp-host', 'missing-url'],
    ]);
    expect(model.hasSwitchTargets).toBe(false);
  });
});
