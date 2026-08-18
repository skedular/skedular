export type RefundSettlementState = 'automatic' | 'manual' | 'completed' | 'failed' | 'unauthorized' | 'ineligible';

export const getRefundSettlementState = (input: {
  refundStatus?: string | null;
  paymentRefundStatus?: string | null;
  canView: boolean;
  eligible: boolean;
}): RefundSettlementState => {
  if (!input.canView) return 'unauthorized';
  if (!input.eligible) return 'ineligible';
  if (input.refundStatus === 'FAILED') return 'failed';
  if (input.refundStatus === 'COMPLETED' || input.paymentRefundStatus === 'succeeded') return 'completed';
  return input.paymentRefundStatus === 'manual' || input.paymentRefundStatus === 'pending_manual' ? 'manual' : 'automatic';
};

export const getRefundSettlementMessage = (state: RefundSettlementState): string =>
  ({
    automatic: 'Refund processing started automatically.',
    manual: 'Refund requires manual settlement.',
    completed: 'Refund completed.',
    failed: 'Refund processing failed and requires review.',
    unauthorized: 'You are not authorized to view refund details.',
    ineligible: 'This entitlement is not eligible for a refund.',
  })[state];
