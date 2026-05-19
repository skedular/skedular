import { describe, expect, it } from 'vitest';
import { resolveCustomerFacingEntryPoint } from './customer-facing-subdomain-resolver';

describe('resolveCustomerFacingEntryPoint', () => {
  it('resolves the root host to public discovery', () => {
    expect(resolveCustomerFacingEntryPoint({ isCustomDomain: false })).toBe('public-discovery');
  });

  it('keeps custom domains on the existing co-working storefront by default', () => {
    expect(resolveCustomerFacingEntryPoint({ isCustomDomain: true })).toBe('co-working-subdomain');
    expect(resolveCustomerFacingEntryPoint({ isCustomDomain: true, organizationType: 'marketplace' })).toBe('co-working-subdomain');
  });

  it('resolves private organisation custom domains separately when the type is known', () => {
    expect(resolveCustomerFacingEntryPoint({ isCustomDomain: true, organizationType: 'private' })).toBe('private-organisation-subdomain');
  });
});
