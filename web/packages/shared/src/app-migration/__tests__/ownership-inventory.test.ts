import { describe, expect, it } from 'vitest';
import { validateOwnershipInventoryItem, type OwnershipInventoryItem } from '../ownership-inventory';

const validItem: OwnershipInventoryItem = {
  item_path: 'web/apps/webapp/src/app/organizations/[organizationCustomDomain]/teams/page.tsx',
  item_type: 'route',
  current_owner: 'webapp',
  target_owner: 'WebApp Teams',
  reason: 'Private organisation team management belongs in Teams.',
  backend_return_url_risk: 'unknown',
  relay_impact: 'yes',
  tests_required: 'webapp-teams lint/test/build and Relay check',
};

describe('validateOwnershipInventoryItem', () => {
  it('accepts a complete ownership inventory item', () => {
    expect(validateOwnershipInventoryItem(validItem)).toEqual({ valid: true, errors: [] });
  });

  it('requires a transition condition for transition ownership', () => {
    const result = validateOwnershipInventoryItem({
      ...validItem,
      target_owner: 'transition',
    });

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('transition_condition is required for transition ownership');
  });
});
