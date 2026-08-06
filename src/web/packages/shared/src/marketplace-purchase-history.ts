export type MarketplacePurchaseSort = 'ACTIVITY_DESC' | 'PURCHASED_DESC' | 'BOOKING_FROM_ASC' | 'BOOKING_UNTIL_ASC';
export type MarketplacePurchaseLifecycleState = 'ACTIVE' | 'CANCELLED' | 'DELETED' | 'EXPIRED' | 'PAYMENT_FAILED' | 'PENDING' | '%future added value';
export type MarketplacePurchaseSourceType = 'BOOKING' | 'SUBSCRIPTION' | '%future added value';
export type MarketplacePurchasePaymentStatus = 'CONFIRMED' | 'PENDING' | 'REJECTED' | 'EXPIRED' | 'NOT_SET' | '%future added value';
export type MarketplacePurchaseOrder = MarketplacePurchaseQueryVariables['purchaseOrderBy'][number];

export type MarketplacePurchaseQueryVariables = {
  purchaseAfter?: string;
  purchaseFirst: number;
  purchaseSourceTypes?: MarketplacePurchaseSourceType[];
  purchaseLifecycleStates?: MarketplacePurchaseLifecycleState[];
  purchasePaymentStatuses?: MarketplacePurchasePaymentStatus[];
  purchaseActivityFrom?: string;
  purchaseActivityUntil?: string;
  purchaseOrderBy: [{ field: 'ACTIVITY_AT' | 'PURCHASED_AT' | 'BOOKING_FROM' | 'BOOKING_UNTIL'; direction: 'ASCENDING' | 'DESCENDING' }];
};

export const toMarketplacePurchaseOrder = (sort: string): MarketplacePurchaseOrder => {
  const normalized = (['ACTIVITY_DESC', 'PURCHASED_DESC', 'BOOKING_FROM_ASC', 'BOOKING_UNTIL_ASC'] as const).includes(sort as MarketplacePurchaseSort)
    ? (sort as MarketplacePurchaseSort)
    : 'ACTIVITY_DESC';

  return {
    field:
      normalized === 'PURCHASED_DESC'
        ? 'PURCHASED_AT'
        : normalized === 'BOOKING_FROM_ASC'
          ? 'BOOKING_FROM'
          : normalized === 'BOOKING_UNTIL_ASC'
            ? 'BOOKING_UNTIL'
            : ('ACTIVITY_AT' as const),
    direction: normalized.endsWith('_ASC') ? ('ASCENDING' as const) : ('DESCENDING' as const),
  };
};

export const buildMarketplacePurchaseQueryVariables = (input: {
  after?: string;
  sourceType?: string;
  lifecycleState?: string;
  paymentStatus?: string;
  activityFrom?: string;
  activityUntil?: string;
  sort?: string;
}): MarketplacePurchaseQueryVariables => ({
  purchaseAfter: input.after,
  purchaseFirst: 50,
  purchaseSourceTypes: input.sourceType ? [input.sourceType as MarketplacePurchaseSourceType] : undefined,
  purchaseLifecycleStates: input.lifecycleState ? [input.lifecycleState as MarketplacePurchaseLifecycleState] : undefined,
  purchasePaymentStatuses: input.paymentStatus ? [input.paymentStatus as MarketplacePurchasePaymentStatus] : undefined,
  purchaseActivityFrom: input.activityFrom ? `${input.activityFrom}T00:00:00.000Z` : undefined,
  purchaseActivityUntil: input.activityUntil ? `${input.activityUntil}T23:59:59.999Z` : undefined,
  purchaseOrderBy: [toMarketplacePurchaseOrder(input.sort ?? 'ACTIVITY_DESC')],
});

export const updateMarketplacePurchaseSearchParams = (
  current: string,
  input: { sourceType?: string; lifecycleState?: string; paymentStatus?: string; activityFrom?: string; activityUntil?: string; sort?: string },
) => {
  const params = new URLSearchParams(current);
  if (input.sourceType) params.set('purchaseSourceType', input.sourceType);
  else params.delete('purchaseSourceType');
  if (input.lifecycleState) params.set('purchaseLifecycleState', input.lifecycleState);
  else params.delete('purchaseLifecycleState');
  if (input.paymentStatus) params.set('purchasePaymentStatus', input.paymentStatus);
  else params.delete('purchasePaymentStatus');
  if (input.activityFrom) params.set('purchaseActivityFrom', input.activityFrom);
  else params.delete('purchaseActivityFrom');
  if (input.activityUntil) params.set('purchaseActivityUntil', input.activityUntil);
  else params.delete('purchaseActivityUntil');
  if (input.sort) params.set('purchaseSort', input.sort);
  else params.delete('purchaseSort');
  return params.toString();
};

export const buildMarketplaceBookingInstancesQueryVariables = (after?: string) => ({
  bookingAfter: after,
  bookingFirst: 50,
});

export const formatMarketplacePurchaseInactiveEvidence = (input: {
  isDeleted: boolean;
  deletedByCustomerId?: string | null;
  cancellationReason?: string | null;
  refundStatus?: string | null;
}) => ({
  lifecycle: input.isDeleted ? 'Deleted' : input.cancellationReason ? 'Lifecycle update' : null,
  actor: input.isDeleted ? (input.deletedByCustomerId ?? null) : null,
  reason: input.cancellationReason ?? null,
  refund: input.refundStatus ?? null,
});

export const formatMarketplacePurchaseDisplay = (input: {
  sourceTypeName: string;
  lifecycleStateName: string;
  productTitle?: string | null;
  productVersionId?: string | null;
  customerId?: string | null;
  paymentStatus: string;
}) => ({
  source: `${input.sourceTypeName} · ${input.lifecycleStateName}`,
  product: input.productTitle ?? input.productVersionId ?? 'Product unavailable',
  customer: input.customerId ?? 'Unavailable',
  payment: input.paymentStatus,
});
