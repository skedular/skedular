import { describe, expect, it } from 'vitest';
import { validateRouteRetirementRecord, type RouteRetirementRecord } from '../route-retirement';

const validRecord: RouteRetirementRecord = {
  route: '/organizations/[organizationCustomDomain]/teams',
  current_owner: 'WebApp',
  target_owner: 'WebApp Teams',
  action: 'keep',
  backend_originated_return_url_audit: 'blocked',
  configuration_source: 'unknown',
  manual_review_path: 'http://localhost:15000/organizations/example/teams',
  notes: 'Keep until return URL usage is checked.',
};

describe('validateRouteRetirementRecord', () => {
  it('accepts a kept route with blocked return URL audit', () => {
    expect(validateRouteRetirementRecord(validRecord)).toEqual({ valid: true, errors: [] });
  });

  it('blocks route deletion before backend-originated return URL audit passes', () => {
    const result = validateRouteRetirementRecord({
      ...validRecord,
      action: 'delete',
    });

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('delete or redirect requires a passed backend-originated return URL audit');
  });
});
