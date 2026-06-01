import { describe, expect, it } from 'vitest';
import { shouldShowCustomerPurchaseHubSignInPrompt, toCustomerPurchaseHubCounts } from './customer-purchase-hub';

describe('GuestStoreFrontBookings hub rules', () => {
  it('keeps active and historical booking counts available for cross-organization customer context', () => {
    expect(toCustomerPurchaseHubCounts({ activeCount: 3, historicCount: 4 })).toEqual({ activeCount: 3, historicCount: 4, totalCount: 7 });
  });

  it('shows the unauthenticated prompt only before sign-in', () => {
    expect(shouldShowCustomerPurchaseHubSignInPrompt(false)).toBe(true);
    expect(shouldShowCustomerPurchaseHubSignInPrompt(true)).toBe(false);
  });
});
