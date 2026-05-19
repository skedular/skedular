import { describe, expect, it } from 'vitest';
import { createAppSelectionLogEvent, logAppSelection, type AppSelectionLogEvent } from '../app-selection-logger';

describe('app-selection-logger', () => {
  it('creates structured app selection diagnostics', () => {
    expect(
      createAppSelectionLogEvent({
        appId: 'webapp-spaces',
        reason: 'organisation-filter',
        organisationType: 'marketplace',
        organisationCount: 2,
        correlationId: 'correlation-id',
      }),
    ).toEqual({
      event: 'web_app_selection',
      appId: 'webapp-spaces',
      reason: 'organisation-filter',
      organisationType: 'marketplace',
      organisationCount: 2,
      correlationId: 'correlation-id',
    });
  });

  it('emits the structured event through the provided logger', () => {
    const messages: Array<{ event: AppSelectionLogEvent; message: string }> = [];

    const event = logAppSelection(
      {
        info: (loggedEvent, message) => messages.push({ event: loggedEvent, message }),
      },
      {
        appId: 'webapp-teams',
        reason: 'direct-url',
      },
    );

    expect(event).toEqual({ event: 'web_app_selection', appId: 'webapp-teams', reason: 'direct-url' });
    expect(messages).toEqual([{ event, message: 'Web app selection resolved' }]);
  });
});
