import { describe, expect, it } from 'vitest';
import { canRequestMarketplaceBookingCancellation, canRequestMarketplaceSubscriptionCancellation, shouldEnterRefundLifecycle } from './marketplace-self-service-eligibility';

describe('marketplace self-service eligibility', () => {
  it('allows booking cancellation only before the booking starts and before it is already cancelled', () => {
    const now = new Date('2026-01-15T12:00:00.000Z');

    expect(canRequestMarketplaceBookingCancellation({ bookingStartsAt: '2026-01-16T12:00:00.000Z', isCancelled: false, now })).toBe(true);
    expect(canRequestMarketplaceBookingCancellation({ bookingStartsAt: '2026-01-14T12:00:00.000Z', isCancelled: false, now })).toBe(false);
    expect(canRequestMarketplaceBookingCancellation({ bookingStartsAt: '2026-01-16T12:00:00.000Z', isCancelled: true, now })).toBe(false);
  });

  it('enters refund handling only after accepted cancellation with confirmed payment', () => {
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: true, isCancellationAccepted: true })).toBe(true);
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: false, isCancellationAccepted: true })).toBe(false);
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: true, isCancellationAccepted: false })).toBe(false);
  });

  it('allows subscription cancellation only for active subscriptions with an available cancellation mode', () => {
    expect(canRequestMarketplaceSubscriptionCancellation({ isActive: true, cancellationModeAvailable: true })).toBe(true);
    expect(canRequestMarketplaceSubscriptionCancellation({ isActive: true, cancellationModeAvailable: false })).toBe(false);
    expect(canRequestMarketplaceSubscriptionCancellation({ isActive: false, cancellationModeAvailable: true })).toBe(false);
  });
});
