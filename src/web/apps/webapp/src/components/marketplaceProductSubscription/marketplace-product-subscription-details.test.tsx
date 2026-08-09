import { describe, expect, it } from 'vitest';
import {
  canRequestMarketplaceBookingModification,
  canRequestMarketplaceSubscriptionCancellation,
  shouldEnterRefundLifecycle,
} from '../marketplaceProductBooking/marketplace-self-service-eligibility';
import { getSubscriptionOccurrenceModificationLabel } from './subscription-occurrence-display';

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

  it('marks only an individually changed occurrence in the subscription history', () => {
    expect(getSubscriptionOccurrenceModificationLabel(true)).toBe('Individually updated');
    expect(getSubscriptionOccurrenceModificationLabel(false)).toBeNull();
  });

  it('keeps the modification entry point limited to eligible future confirmed occurrences', () => {
    const now = new Date('2026-08-09T00:00:00.000Z');

    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-08-10T09:00:00.000Z', isCancelled: false, paymentStatusType: 'CONFIRMED', now })).toBe(true);
    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-08-08T09:00:00.000Z', isCancelled: false, paymentStatusType: 'CONFIRMED', now })).toBe(false);
    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-08-10T09:00:00.000Z', isCancelled: true, paymentStatusType: 'CONFIRMED', now })).toBe(false);
  });
});
