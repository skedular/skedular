import { describe, expect, it } from 'vitest';
import { validateRouteRetirementRegister, type RouteRetirementRecord } from '../route-retirement';

const keptRoute: RouteRetirementRecord = {
  route: '/organizations/[organizationCustomDomain]/products/**',
  current_owner: 'WebApp',
  target_owner: 'WebApp Spaces',
  action: 'keep',
  backend_originated_return_url_audit: 'blocked',
  configuration_source: 'Unknown notification/payment/deep-link usage',
  removal_condition: 'User approves the Spaces product route and backend return URL audit passes.',
  manual_review_path: 'http://localhost:15000/organizations/example/products and http://localhost:15004/products',
  notes: 'Dual-run first.',
};

describe('validateRouteRetirementRegister', () => {
  it('accepts kept risky routes while the backend-originated return URL audit is blocked', () => {
    const result = validateRouteRetirementRegister([keptRoute]);

    expect(result).toEqual({ valid: true, errors: [], routeCount: 1 });
  });

  it('rejects duplicate route entries', () => {
    const result = validateRouteRetirementRegister([keptRoute, keptRoute]);

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('route /organizations/[organizationCustomDomain]/products/** is duplicated');
  });

  it('rejects route deletion without a passed audit', () => {
    const result = validateRouteRetirementRegister([
      {
        ...keptRoute,
        action: 'delete',
      },
    ]);

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('record 1 (/organizations/[organizationCustomDomain]/products/**): delete or redirect requires a passed backend-originated return URL audit');
  });
});
