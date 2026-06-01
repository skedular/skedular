import { describe, expect, it } from 'vitest';
import {
  canSelectOrganisationTypeInApp,
  getCustomerEntryTypes,
  getProductAppDefinition,
  hasMarketplaceConcepts,
  hasPrivateOrganisationConcepts,
  isProductAppId,
} from '../app-products';

describe('app-products', () => {
  it('uses canonical Skedular product names', () => {
    expect(getProductAppDefinition('webapp').name).toBe('Skedular');
    expect(getProductAppDefinition('webapp-teams').name).toBe('Skedular Teams');
    expect(getProductAppDefinition('webapp-spaces').name).toBe('Skedular Spaces');
  });

  it('keeps Teams scoped to private organizations without marketplace concepts', () => {
    expect(getProductAppDefinition('webapp-teams').allowedOrganisationTypes).toEqual(['private']);
    expect(canSelectOrganisationTypeInApp('webapp-teams', 'private')).toBe(true);
    expect(canSelectOrganisationTypeInApp('webapp-teams', 'marketplace')).toBe(false);
    expect(hasMarketplaceConcepts('webapp-teams')).toBe(false);
  });

  it('keeps Spaces scoped to marketplace organizations without private organization workflows', () => {
    expect(getProductAppDefinition('webapp-spaces').allowedOrganisationTypes).toEqual(['marketplace']);
    expect(canSelectOrganisationTypeInApp('webapp-spaces', 'marketplace')).toBe(true);
    expect(canSelectOrganisationTypeInApp('webapp-spaces', 'private')).toBe(false);
    expect(hasPrivateOrganisationConcepts('webapp-spaces')).toBe(false);
  });

  it('keeps WebApp customer-facing entry points available', () => {
    expect(getCustomerEntryTypes('webapp')).toEqual(['root', 'marketplace-subdomain', 'private-organisation-subdomain']);
    expect(isProductAppId('webapp')).toBe(true);
    expect(isProductAppId('unknown')).toBe(false);
  });
});
