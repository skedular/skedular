import { describe, expect, it } from 'vitest';
import { shouldShowCustomerPurchaseHubSignInPrompt, toCustomerPurchaseHubCounts } from './customer-purchase-hub';

describe('GuestStoreFrontSubscriptions hub rules', () => {
  it('keeps active and historical subscription counts available for cross-organization customer context', () => {
    expect(toCustomerPurchaseHubCounts({ activeCount: 2, historicCount: 1 })).toEqual({ activeCount: 2, historicCount: 1, totalCount: 3 });
  });

  it('treats negative empty-state counts as zero', () => {
    expect(toCustomerPurchaseHubCounts({ activeCount: -1, historicCount: -2 })).toEqual({ activeCount: 0, historicCount: 0, totalCount: 0 });
  });

  it('shows the unauthenticated prompt only before sign-in', () => {
    expect(shouldShowCustomerPurchaseHubSignInPrompt(false)).toBe(true);
    expect(shouldShowCustomerPurchaseHubSignInPrompt(true)).toBe(false);
  });
});
