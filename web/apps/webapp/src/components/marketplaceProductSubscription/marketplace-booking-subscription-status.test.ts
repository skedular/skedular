import { describe, expect, it } from 'vitest';
import {
    isSupportedMarketplaceBookingSubscriptionStatusForFilter,
    toMarketplaceBookingSubscriptionStatusForFilterDetails,
    type SupportedMarketplaceBookingSubscriptionStatusForFilter,
} from './marketplace-booking-subscription-status';

describe('isSupportedMarketplaceBookingSubscriptionStatusForFilter', () => {
  it('returns true for all supported status values', () => {
    const supported: string[] = ['ACTIVE', 'CANCELLED', 'EXPIRED', 'RENEWAL_FAILED', 'PAUSED'];

    supported.forEach((value) => {
      expect(isSupportedMarketplaceBookingSubscriptionStatusForFilter(value)).toBe(true);
    });
  });

  it('returns false for unknown or invalid values', () => {
    expect(isSupportedMarketplaceBookingSubscriptionStatusForFilter('UNKNOWN')).toBe(false);
    expect(isSupportedMarketplaceBookingSubscriptionStatusForFilter('')).toBe(false);
    expect(isSupportedMarketplaceBookingSubscriptionStatusForFilter('active')).toBe(false);
    expect(isSupportedMarketplaceBookingSubscriptionStatusForFilter('RECORD_NEVER_CREATED')).toBe(false);
  });

  it('acts as a type guard so filtered arrays have the correct type', () => {
    const raw = ['ACTIVE', 'UNKNOWN', 'CANCELLED', ''];
    const result: SupportedMarketplaceBookingSubscriptionStatusForFilter[] = raw.filter(isSupportedMarketplaceBookingSubscriptionStatusForFilter);

    expect(result).toEqual(['ACTIVE', 'CANCELLED']);
  });
});

describe('toMarketplaceBookingSubscriptionStatusForFilterDetails', () => {
  it('returns a details object for a supported status', () => {
    const result = toMarketplaceBookingSubscriptionStatusForFilterDetails('ACTIVE', 'Active');

    expect(result).toEqual({ type: 'ACTIVE', name: 'Active' });
  });

  it('returns null for an unsupported status value', () => {
    const result = toMarketplaceBookingSubscriptionStatusForFilterDetails('UNKNOWN', 'Unknown');

    expect(result).toBeNull();
  });
});

describe('URL param parsing for subscription status filter', () => {
  it('parses a comma-separated statuses URL param into a valid filter array', () => {
    const raw = 'ACTIVE,CANCELLED';
    const parsed = raw.split(',').filter(isSupportedMarketplaceBookingSubscriptionStatusForFilter);

    expect(parsed).toEqual(['ACTIVE', 'CANCELLED']);
  });

  it('filters out unrecognised values from a URL param', () => {
    const raw = 'ACTIVE,UNKNOWN,PAUSED';
    const parsed = raw.split(',').filter(isSupportedMarketplaceBookingSubscriptionStatusForFilter);

    expect(parsed).toEqual(['ACTIVE', 'PAUSED']);
  });

  it('produces an empty array when the URL param contains only unrecognised values', () => {
    const raw = 'FOO,BAR';
    const parsed = raw.split(',').filter(isSupportedMarketplaceBookingSubscriptionStatusForFilter);

    expect(parsed).toEqual([]);
  });

  it('serialises a filter array back to a comma-joined URL param string', () => {
    const statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[] = ['ACTIVE', 'EXPIRED'];
    const serialised = statuses.join(',');

    expect(serialised).toBe('ACTIVE,EXPIRED');
  });

  it('removes the param when the filter array is empty', () => {
    const params = new URLSearchParams('statuses=ACTIVE&paymentStatuses=PENDING');
    const statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[] = [];

    if (statuses.length > 0) {
      params.set('statuses', statuses.join(','));
    } else {
      params.delete('statuses');
    }

    expect(params.has('statuses')).toBe(false);
    expect(params.get('paymentStatuses')).toBe('PENDING');
  });
});
