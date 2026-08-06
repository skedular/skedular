import {
  buildMarketplacePurchaseQueryVariables,
  formatMarketplacePurchaseDisplay,
  formatMarketplacePurchaseInactiveEvidence,
  toMarketplacePurchaseOrder,
  updateMarketplacePurchaseSearchParams,
} from '@skedular/shared';
import { describe, expect, it } from 'vitest';

describe('Spaces Marketplace purchases page query behavior', () => {
  it('loads newest activity first and resets the cursor when filters change', () => {
    expect(buildMarketplacePurchaseQueryVariables({ sourceType: 'BOOKING', lifecycleState: 'DELETED' })).toMatchObject({
      purchaseFirst: 50,
      purchaseSourceTypes: ['BOOKING'],
      purchaseLifecycleStates: ['DELETED'],
      purchaseOrderBy: [{ field: 'ACTIVITY_AT', direction: 'DESCENDING' }],
    });
  });

  it('preserves unrelated URL filters while changing purchase sort', () => {
    expect(updateMarketplacePurchaseSearchParams('statuses=CONFIRMED', { sourceType: 'SUBSCRIPTION', lifecycleState: 'ACTIVE', sort: 'PURCHASED_DESC' })).toBe(
      'statuses=CONFIRMED&purchaseSourceType=SUBSCRIPTION&purchaseLifecycleState=ACTIVE&purchaseSort=PURCHASED_DESC',
    );
  });

  it('maps booking ordering consistently for list and grid data', () => {
    expect(toMarketplacePurchaseOrder('BOOKING_FROM_ASC')).toEqual({ field: 'BOOKING_FROM', direction: 'ASCENDING' });
  });

  it('keeps deletion evidence separate from refund evidence', () => {
    expect(
      formatMarketplacePurchaseInactiveEvidence({ isDeleted: true, deletedByCustomerId: 'operator-1', cancellationReason: 'Customer request', refundStatus: 'Processing' }),
    ).toEqual({
      lifecycle: 'Deleted',
      actor: 'operator-1',
      reason: 'Customer request',
      refund: 'Processing',
    });
  });

  it('uses one display model for list and grid source data', () => {
    expect(
      formatMarketplacePurchaseDisplay({
        sourceTypeName: 'Hourly booking',
        lifecycleStateName: 'Active',
        productTitle: 'Desk',
        productVersionId: 'product-1',
        customerId: 'customer-1',
        paymentStatus: 'Confirmed',
      }),
    ).toEqual({
      source: 'Hourly booking · Active',
      product: 'Desk',
      customer: 'customer-1',
      payment: 'Confirmed',
    });
  });
});
