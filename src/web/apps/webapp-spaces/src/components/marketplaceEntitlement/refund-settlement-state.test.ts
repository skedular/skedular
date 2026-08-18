import { describe, expect, it } from 'vitest';
import { getRefundSettlementMessage, getRefundSettlementState } from './refund-settlement-state';

describe('refund settlement state', () => {
  it.each([
    ['automatic', { canView: true, eligible: true }],
    ['manual', { canView: true, eligible: true, paymentRefundStatus: 'pending_manual' }],
    ['completed', { canView: true, eligible: true, refundStatus: 'COMPLETED' }],
    ['failed', { canView: true, eligible: true, refundStatus: 'FAILED' }],
    ['unauthorized', { canView: false, eligible: true }],
    ['ineligible', { canView: true, eligible: false }],
  ])('returns %s', (expected, input) => expect(getRefundSettlementState(input)).toBe(expected));
  it('provides manual settlement copy', () => expect(getRefundSettlementMessage('manual')).toContain('manual'));
});
