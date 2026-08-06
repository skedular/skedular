import { buildMarketplaceBookingInstancesQueryVariables } from '@skedular/shared';
import { describe, expect, it } from 'vitest';

describe('Spaces subscription detail booking instances', () => {
  it('uses a bounded connection and forwards the cursor', () => {
    expect(buildMarketplaceBookingInstancesQueryVariables('next-cursor')).toEqual({ bookingAfter: 'next-cursor', bookingFirst: 50 });
    expect(buildMarketplaceBookingInstancesQueryVariables(undefined)).toEqual({ bookingAfter: undefined, bookingFirst: 50 });
  });
});
