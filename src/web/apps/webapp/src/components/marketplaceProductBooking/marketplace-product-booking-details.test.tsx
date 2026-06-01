import { describe, expect, it } from 'vitest';
import { canRequestMarketplaceBookingCancellation, shouldEnterRefundLifecycle } from './marketplace-self-service-eligibility';

describe('MarketplaceProductBookingDetails action eligibility', () => {
  it('exposes cancellation only for future non-cancelled bookings', () => {
    const now = new Date('2026-02-01T00:00:00.000Z');

    expect(canRequestMarketplaceBookingCancellation({ bookingStartsAt: '2026-02-02T00:00:00.000Z', isCancelled: false, now })).toBe(true);
    expect(canRequestMarketplaceBookingCancellation({ bookingStartsAt: '2026-02-01T00:00:00.000Z', isCancelled: false, now })).toBe(false);
  });

  it('keeps refund handling behind confirmed payment and accepted cancellation', () => {
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: true, isCancellationAccepted: true })).toBe(true);
    expect(shouldEnterRefundLifecycle({ hasConfirmedPayment: false, isCancellationAccepted: true })).toBe(false);
  });
});
