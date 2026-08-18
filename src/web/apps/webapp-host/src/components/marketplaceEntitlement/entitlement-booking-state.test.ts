import { describe, expect, it } from 'vitest';
import { getEntitlementBookingState, getEntitlementBookingStateMessage } from './entitlement-booking-state';

const base = { expiresAt: '2026-09-01T00:00:00Z', now: new Date('2026-08-10T00:00:00Z') };
describe('entitlement booking state', () => {
  it.each([
    ['eligible', { availableQuantity: 1, hasMatchingResource: true }],
    ['unavailable', { availableQuantity: 1, hasMatchingResource: false }],
    ['zero-balance', { availableQuantity: 0, hasMatchingResource: true }],
    ['expired', { availableQuantity: 1, hasMatchingResource: true, expiresAt: '2026-08-09T00:00:00Z' }],
  ])('returns %s', (expected, input) => expect(getEntitlementBookingState({ ...base, ...input })).toBe(expected));
  it('provides customer-facing copy for every state', () => {
    expect(getEntitlementBookingStateMessage('eligible')).toBe('Use booking credit');
    expect(getEntitlementBookingStateMessage('expired')).toContain('expired');
  });
});
