import { describe, expect, it } from 'vitest';
import { validateTransitionPathRecord, type TransitionPathRecord } from '../transition-path';

const validTransition: TransitionPathRecord = {
  source_route: '/organizations/add-private',
  target_route: 'https://teams.skedular.app/organizations/add-private',
  action: 'transition',
  owner: 'WebApp Teams',
  removal_condition: 'App-specific URL strategy has been verified.',
  backend_return_url_checked: false,
};

describe('validateTransitionPathRecord', () => {
  it('accepts a documented transition path before backend return URL checks pass', () => {
    expect(validateTransitionPathRecord(validTransition)).toEqual({ valid: true, errors: [] });
  });

  it('blocks redirect paths until backend return URL checks pass', () => {
    const result = validateTransitionPathRecord({
      ...validTransition,
      action: 'redirect',
      backend_return_url_checked: false,
    });

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('backend_return_url_checked must be true before redirecting a route');
  });
});
