import { describe, expect, it } from 'vitest';
import { buildMarketplacePurchaseQueryVariables, toMarketplacePurchaseOrder, updateMarketplacePurchaseSearchParams } from '../marketplace-purchase-history';

describe('marketplace purchase history query state', () => {
  it('defaults unknown sorts to newest activity', () => {
    expect(toMarketplacePurchaseOrder('unsupported')).toEqual({ field: 'ACTIVITY_AT', direction: 'DESCENDING' });
  });

  it('builds source and lifecycle filters with a resettable cursor', () => {
    expect(buildMarketplacePurchaseQueryVariables({ after: undefined, sourceType: 'BOOKING', lifecycleState: 'DELETED', sort: 'PURCHASED_DESC' })).toEqual({
      purchaseAfter: undefined,
      purchaseFirst: 50,
      purchaseSourceTypes: ['BOOKING'],
      purchaseLifecycleStates: ['DELETED'],
      purchaseOrderBy: [{ field: 'PURCHASED_AT', direction: 'DESCENDING' }],
    });
  });

  it('includes deleted retained purchases in the default view', () => {
    expect(buildMarketplacePurchaseQueryVariables({})).toMatchObject({
      purchaseLifecycleStates: ['ACTIVE', 'CANCELLED', 'DELETED', 'EXPIRED', 'PAYMENT_FAILED', 'PENDING'],
    });
  });

  it('maps ascending booking sorts deterministically', () => {
    expect(toMarketplacePurchaseOrder('BOOKING_UNTIL_ASC')).toEqual({ field: 'BOOKING_UNTIL', direction: 'ASCENDING' });
  });

  it('applies and clears URL-backed filters without losing unrelated parameters', () => {
    expect(
      updateMarketplacePurchaseSearchParams('statuses=CONFIRMED&purchaseSourceType=SUBSCRIPTION', { sourceType: 'BOOKING', lifecycleState: 'DELETED', sort: 'ACTIVITY_DESC' }),
    ).toBe('statuses=CONFIRMED&purchaseSourceType=BOOKING&purchaseLifecycleState=DELETED&purchaseSort=ACTIVITY_DESC');
    expect(updateMarketplacePurchaseSearchParams('statuses=CONFIRMED&purchaseSourceType=BOOKING&purchaseLifecycleState=DELETED', {})).toBe('statuses=CONFIRMED');
  });
});
