import { describe, expect, it } from 'vitest';
import { canRequestMarketplaceSubscriptionCancellation, shouldEnterRefundLifecycle } from '../marketplaceProductBooking/marketplace-self-service-eligibility';

describe('MarketplaceProductSubscriptionDetails action eligibility', () => {
  it('exposes cancellation only for active subscriptions with an available cancellation mode', () => {
    expect(canRequestMarketplaceSubscriptionCancellation({ isActive: true, cancellationModeAvailable: true })).toBe(true);
    expect(canRequestMarketplaceSubscriptionCancellation({ isActive: true, cancellationModeAvailable: false })).toBe(false);
    expect(canRequestMarketplaceSubscriptionCancellation({ isActive: false, cancellationModeAvailable: true })).toBe(false);
  });

  it('keeps refund handling behind confirmed payment and accepted cancellation', () => {
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: true, isCancellationAccepted: true })).toBe(true);
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: true, isCancellationAccepted: false })).toBe(false);
  });
});
