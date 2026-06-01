import { describe, expect, it } from 'vitest';
import { createAppSwitcherModel } from '../app-switcher-model';
import {
  createAppSwitcherConfigurationLogEvent,
  createAppSwitcherSelectionLogEvent,
  logAppSwitcherConfiguration,
  logAppSwitcherSelection,
  type AppSwitcherLogEvent,
} from '../app-switcher-logger';

describe('app-switcher-logger', () => {
  it('creates configuration diagnostics without URL payloads', () => {
    const model = createAppSwitcherModel({
      currentAppId: 'webapp',
      destinations: {
        webapp: 'https://app.skedular.test/secret-path',
        'webapp-spaces': '',
        'webapp-teams': 'not-a-url',
      },
    });

    expect(createAppSwitcherConfigurationLogEvent(model, 'correlation-id')).toEqual({
      event: 'web_app_switcher_configuration',
      currentAppId: 'webapp',
      availableDestinationCount: 0,
      invalidDestinationAppIds: ['webapp-teams'],
      missingDestinationAppIds: ['webapp-spaces'],
      correlationId: 'correlation-id',
    });
  });

  it('logs invalid configuration as a warning when the logger supports warnings', () => {
    const messages: Array<{ event: AppSwitcherLogEvent; message: string; level: 'info' | 'warn' }> = [];
    const model = createAppSwitcherModel({
      currentAppId: 'webapp',
      destinations: {
        'webapp-teams': 'not-a-url',
      },
    });

    const event = logAppSwitcherConfiguration(
      {
        info: (loggedEvent, message) => messages.push({ event: loggedEvent, message, level: 'info' }),
        warn: (loggedEvent, message) => messages.push({ event: loggedEvent, message, level: 'warn' }),
      },
      model,
    );

    expect(event.invalidDestinationAppIds).toEqual(['webapp-teams']);
    expect(messages).toEqual([{ event, message: 'Web app switcher configuration has invalid destinations', level: 'warn' }]);
  });

  it('creates and logs selection diagnostics', () => {
    const messages: Array<{ event: AppSwitcherLogEvent; message: string }> = [];

    expect(createAppSwitcherSelectionLogEvent('webapp', 'webapp-spaces')).toEqual({
      event: 'web_app_switcher_selection',
      currentAppId: 'webapp',
      destinationAppId: 'webapp-spaces',
    });

    const event = logAppSwitcherSelection(
      {
        info: (loggedEvent, message) => messages.push({ event: loggedEvent, message }),
      },
      'webapp',
      'webapp-teams',
    );

    expect(messages).toEqual([{ event, message: 'Web app switcher destination selected' }]);
  });
});
