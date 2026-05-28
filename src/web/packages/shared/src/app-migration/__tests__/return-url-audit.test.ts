import { describe, expect, it } from 'vitest';
import { auditBackendReturnUrlUsage } from '../return-url-audit';

describe('auditBackendReturnUrlUsage', () => {
  it('marks routes without backend-originated references as not applicable', () => {
    expect(
      auditBackendReturnUrlUsage({
        route: '/organization-selection',
        action: 'keep',
        riskTypes: [],
        referencesFound: [],
      }),
    ).toEqual({ state: 'not applicable', blockers: [] });
  });

  it('blocks delete or redirect when backend-originated references still target the old route', () => {
    const result = auditBackendReturnUrlUsage({
      route: '/organizations/example/products',
      action: 'delete',
      riskTypes: ['payment', 'notification'],
      referencesFound: ['payment success return URL', 'notification deep link'],
    });

    expect(result.state).toBe('blocked');
    expect(result.blockers).toContain('backend-originated return URL references must be replaced before delete or redirect');
    expect(result.blockers).toContain('app-specific target URL strategy is required for risky route changes');
  });

  it('passes a risky transition when an app-specific target URL strategy is recorded and no old references remain', () => {
    expect(
      auditBackendReturnUrlUsage({
        route: '/organizations/example/products',
        action: 'transition',
        riskTypes: ['payment'],
        referencesFound: [],
        appSpecificTargetUrl: 'WEBAPP_SPACES_BASE_URL',
      }),
    ).toEqual({ state: 'pass', blockers: [] });
  });
});
