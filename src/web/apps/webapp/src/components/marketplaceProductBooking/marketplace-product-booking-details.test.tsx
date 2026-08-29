import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';
import { getFailureHeadline, hasRebookAction, isAvailabilityConflictFailure, type MarketplaceBookingFailureSummary } from './marketplace-booking-failure-eligibility';
import { canRequestMarketplaceBookingCancellation, canRequestMarketplaceBookingModification, shouldEnterRefundLifecycle } from './marketplace-self-service-eligibility';

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

  it('exposes modification only for a future confirmed or no-payment-required booking', () => {
    const now = new Date('2026-02-01T00:00:00.000Z');

    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-02-02T00:00:00.000Z', isCancelled: false, paymentStatusType: 'CONFIRMED', now })).toBe(true);
    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-02-02T00:00:00.000Z', isCancelled: false, paymentStatusType: 'NO_PAYMENT_REQUIRED', now })).toBe(true);
    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-02-02T00:00:00.000Z', isCancelled: false, paymentStatusType: 'PENDING', now })).toBe(false);
    expect(canRequestMarketplaceBookingModification({ bookingStartsAt: '2026-02-01T00:00:00.000Z', isCancelled: false, paymentStatusType: 'CONFIRMED', now })).toBe(false);
  });
});
describe('MarketplaceProductBookingDetails failure presentation', () => {
  it('shows availability-specific headline for availability conflict', () => {
    const failure = {
      category: { type: 'AvailabilityConflict' },
      customerAction: { type: 'Rebook' },
      finalizedAt: '2026-07-22T10:00:00Z',
    } satisfies MarketplaceBookingFailureSummary;
    const headline = getFailureHeadline(failure);
    expect(headline).toBe('This booking could not be confirmed');
  });

  it('exposes rebook action for availability conflict failure', () => {
    const failure = {
      category: { type: 'AvailabilityConflict' },
      customerAction: { type: 'Rebook' },
      finalizedAt: '2026-07-22T10:00:00Z',
    } satisfies MarketplaceBookingFailureSummary;
    const result = hasRebookAction(failure);
    expect(result).toBe(true);
  });

  it('distinguishes availability conflict from payment failures', () => {
    const availabilityFailure = {
      category: { type: 'AvailabilityConflict' },
      customerAction: { type: 'Rebook' },
      finalizedAt: '2026-07-22T10:00:00Z',
    } satisfies MarketplaceBookingFailureSummary;
    const paymentFailure = {
      category: { type: 'PaymentFailed' },
      customerAction: { type: 'None' },
      finalizedAt: '2026-07-22T10:00:00Z',
    } satisfies MarketplaceBookingFailureSummary;

    expect(isAvailabilityConflictFailure(availabilityFailure)).toBe(true);
    expect(isAvailabilityConflictFailure(paymentFailure)).toBe(false);
  });
});

describe('MarketplaceProductBookingDetails history boundary', () => {
  it('does not request or render marketplace purchase lifecycle history for one-time bookings', () => {
    const source = readFileSync(resolve(process.cwd(), 'src/components/marketplaceProductBooking/marketplace-product-booking-details.tsx'), 'utf8');

    expect(source).not.toContain('marketplacePurchaseHistory');
    expect(source).not.toContain('MarketplacePurchaseHistoryEventList');
    expect(source).toContain('RefundHistoryTimeline');
  });
});
