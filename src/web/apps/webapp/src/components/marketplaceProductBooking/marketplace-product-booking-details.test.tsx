import { describe, expect, it } from 'vitest';
import { getFailureHeadline, hasRebookAction, isAvailabilityConflictFailure } from './marketplace-booking-failure-eligibility';
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
describe('MarketplaceProductBookingDetails failure presentation', () => {
  it('shows availability-specific headline for availability conflict', () => {
    const headline = getFailureHeadline({ category: { type: 'AvailabilityConflict' }, customerAction: { type: 'Rebook' }, finalizedAt: '2026-07-22T10:00:00Z' });
    expect(headline).toBe('This booking could not be confirmed');
  });

  it('exposes rebook action for availability conflict failure', () => {
    const result = hasRebookAction({ category: { type: 'AvailabilityConflict' }, customerAction: { type: 'Rebook' }, finalizedAt: '2026-07-22T10:00:00Z' });
    expect(result).toBe(true);
  });

  it('distinguishes availability conflict from payment failures', () => {
    const availabilityFailure = { category: { type: 'AvailabilityConflict' }, customerAction: { type: 'Rebook' }, finalizedAt: '2026-07-22T10:00:00Z' };
    const paymentFailure = { category: { type: 'PaymentFailed' }, customerAction: { type: 'None' }, finalizedAt: '2026-07-22T10:00:00Z' };

    expect(isAvailabilityConflictFailure(availabilityFailure)).toBe(true);
    expect(isAvailabilityConflictFailure(paymentFailure)).toBe(false);
  });
});
