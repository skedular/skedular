import { describe, expect, it } from 'vitest';
import {
  isSupportedMarketplaceBookingPaymentStatusForFilter,
  toMarketplaceBookingPaymentStatusForFilterDetails,
  type SupportedMarketplaceBookingPaymentStatusForFilter,
} from './marketplace-booking-payment-status';

describe('isSupportedMarketplaceBookingPaymentStatusForFilter', () => {
  it('returns true for all supported payment status values', () => {
    const supported: string[] = ['NOT_SET', 'PENDING', 'REJECTED', 'CONFIRMED', 'EXPIRED', 'NO_PAYMENT_REQUIRED'];

    supported.forEach((value) => {
      expect(isSupportedMarketplaceBookingPaymentStatusForFilter(value)).toBe(true);
    });
  });

  it('returns false for unknown or invalid values', () => {
    expect(isSupportedMarketplaceBookingPaymentStatusForFilter('UNKNOWN')).toBe(false);
    expect(isSupportedMarketplaceBookingPaymentStatusForFilter('')).toBe(false);
    expect(isSupportedMarketplaceBookingPaymentStatusForFilter('pending')).toBe(false);
    expect(isSupportedMarketplaceBookingPaymentStatusForFilter('RECORD_NEVER_CREATED')).toBe(false);
  });

  it('acts as a type guard so filtered arrays have the correct type', () => {
    const raw = ['NOT_SET', 'UNKNOWN', 'CONFIRMED', ''];
    const result: SupportedMarketplaceBookingPaymentStatusForFilter[] = raw.filter(isSupportedMarketplaceBookingPaymentStatusForFilter);

    expect(result).toEqual(['NOT_SET', 'CONFIRMED']);
  });
});

describe('toMarketplaceBookingPaymentStatusForFilterDetails', () => {
  it('returns a details object for a supported payment status', () => {
    const result = toMarketplaceBookingPaymentStatusForFilterDetails('PENDING', 'Pending');

    expect(result).toEqual({ type: 'PENDING', name: 'Pending' });
  });

  it('returns null for an unsupported payment status value', () => {
    const result = toMarketplaceBookingPaymentStatusForFilterDetails('RECORD_NEVER_CREATED', 'Record never created');

    expect(result).toBeNull();
  });
});

describe('URL param parsing for payment status filter', () => {
  it('parses a comma-separated paymentStatuses URL param into a valid filter array', () => {
    const raw = 'PENDING,CONFIRMED';
    const parsed = raw.split(',').filter(isSupportedMarketplaceBookingPaymentStatusForFilter);

    expect(parsed).toEqual(['PENDING', 'CONFIRMED']);
  });

  it('filters out unrecognised values from a URL param', () => {
    const raw = 'PENDING,RECORD_NEVER_CREATED,EXPIRED';
    const parsed = raw.split(',').filter(isSupportedMarketplaceBookingPaymentStatusForFilter);

    expect(parsed).toEqual(['PENDING', 'EXPIRED']);
  });

  it('serialises a payment filter array back to a comma-joined URL param string', () => {
    const statuses: SupportedMarketplaceBookingPaymentStatusForFilter[] = ['PENDING', 'NO_PAYMENT_REQUIRED'];
    const serialised = statuses.join(',');

    expect(serialised).toBe('PENDING,NO_PAYMENT_REQUIRED');
  });

  it('removes the paymentStatuses param when the filter array is empty', () => {
    const params = new URLSearchParams('statuses=ACTIVE&paymentStatuses=PENDING');
    const paymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[] = [];

    if (paymentStatuses.length > 0) {
      params.set('paymentStatuses', paymentStatuses.join(','));
    } else {
      params.delete('paymentStatuses');
    }

    expect(params.has('paymentStatuses')).toBe(false);
    expect(params.get('statuses')).toBe('ACTIVE');
  });
});
