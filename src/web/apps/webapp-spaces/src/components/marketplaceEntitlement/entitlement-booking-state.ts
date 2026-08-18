export type EntitlementBookingState = 'eligible' | 'unavailable' | 'expired' | 'zero-balance';

export const getEntitlementBookingState = (input: { availableQuantity: number; expiresAt: string; now?: Date; hasMatchingResource: boolean }): EntitlementBookingState => {
  if (new Date(input.expiresAt).getTime() <= (input.now ?? new Date()).getTime()) return 'expired';
  if (input.availableQuantity <= 0) return 'zero-balance';
  if (!input.hasMatchingResource) return 'unavailable';
  return 'eligible';
};

export const getEntitlementBookingStateMessage = (state: EntitlementBookingState): string =>
  ({
    eligible: 'Use booking credit',
    unavailable: 'No matching resource is available',
    expired: 'This booking credit has expired',
    'zero-balance': 'No booking credits remain',
  })[state];
