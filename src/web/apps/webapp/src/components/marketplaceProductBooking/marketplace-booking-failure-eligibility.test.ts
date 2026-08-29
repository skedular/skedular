import { describe, expect, it } from 'vitest';
import {
  getFailureHeadline,
  getFailureCleanupMessage,
  hasRebookAction,
  isAvailabilityConflictFailure,
  isPaymentFailure,
  type FailureCategoryType,
  type FailureCustomerActionType,
  type MarketplaceBookingFailureSummary,
} from './marketplace-booking-failure-eligibility';

const makeFailure = (categoryType: FailureCategoryType, customerActionType: FailureCustomerActionType = 'None'): MarketplaceBookingFailureSummary => ({
  category: { type: categoryType },
  customerAction: { type: customerActionType },
  finalizedAt: '2026-07-22T10:00:00Z',
});

describe('marketplace booking failure eligibility', () => {
  describe('isAvailabilityConflictFailure', () => {
    it('returns true for AvailabilityConflict', () => {
      expect(isAvailabilityConflictFailure(makeFailure('AvailabilityConflict'))).toBe(true);
    });

    it('returns false for PaymentFailed', () => {
      expect(isAvailabilityConflictFailure(makeFailure('PaymentFailed'))).toBe(false);
    });

    it('returns false for PaymentExpired', () => {
      expect(isAvailabilityConflictFailure(makeFailure('PaymentExpired'))).toBe(false);
    });
  });

  describe('isPaymentFailure', () => {
    it('returns true for PaymentFailed', () => {
      expect(isPaymentFailure(makeFailure('PaymentFailed'))).toBe(true);
    });

    it('returns true for PaymentExpired', () => {
      expect(isPaymentFailure(makeFailure('PaymentExpired'))).toBe(true);
    });

    it('returns false for AvailabilityConflict', () => {
      expect(isPaymentFailure(makeFailure('AvailabilityConflict'))).toBe(false);
    });
  });

  describe('hasRebookAction', () => {
    it('returns true when customer action is Rebook', () => {
      expect(hasRebookAction(makeFailure('AvailabilityConflict', 'Rebook'))).toBe(true);
    });

    it('returns false when customer action is None', () => {
      expect(hasRebookAction(makeFailure('AvailabilityConflict', 'None'))).toBe(false);
    });

    it('returns false when customer action is ReviewSubscription', () => {
      expect(hasRebookAction(makeFailure('AvailabilityConflict', 'ReviewSubscription'))).toBe(false);
    });
  });

  describe('getFailureHeadline', () => {
    it('returns availability-specific copy for AvailabilityConflict', () => {
      const headline = getFailureHeadline(makeFailure('AvailabilityConflict'));
      expect(headline).toBe('This booking could not be confirmed');
    });

    it('returns payment-failed copy for PaymentFailed', () => {
      const headline = getFailureHeadline(makeFailure('PaymentFailed'));
      expect(headline).toBe('Payment was not completed');
    });

    it('returns payment-expired copy for PaymentExpired', () => {
      const headline = getFailureHeadline(makeFailure('PaymentExpired'));
      expect(headline).toBe('Payment time expired');
    });
  });

  describe('getFailureCleanupMessage', () => {
    it('does not claim the capacity is released before the local commit', () => {
      expect(
        getFailureCleanupMessage({
          ...makeFailure('PaymentFailed'),
          resourceReleaseStatus: { type: 'PENDING', name: 'Pending' },
        }),
      ).toBe('We are releasing the reserved capacity. Check back shortly for the final status.');
    });

    it('separates committed capacity release from pending accounting work', () => {
      expect(
        getFailureCleanupMessage({
          ...makeFailure('PaymentFailed'),
          resourceReleaseStatus: { type: 'RELEASED', name: 'Released' },
          accountingCleanupStatus: { type: 'PENDING', name: 'Pending' },
        }),
      ).toBe('The reserved capacity has been released. We are completing the related accounting update separately.');
    });
  });
});
