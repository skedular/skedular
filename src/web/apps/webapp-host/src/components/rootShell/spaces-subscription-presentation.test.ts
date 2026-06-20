import { describe, expect, it } from 'vitest';
import type { SpacesSubscriptionState } from './spaces-subscription-context';
import { getSpacesSubscriptionPresentation } from './spaces-subscription-presentation';

const createSubscription = (subscriptionStatus: string, overrides: Partial<SpacesSubscriptionState> = {}): SpacesSubscriptionState => ({
  subscriptionStatus,
  accessReason: 'ALLOWED_TRIAL',
  trialStartedAt: '2026-06-01T00:00:00Z',
  trialEndsAt: '2026-06-15T00:00:00Z',
  remainingTrialDays: 8,
  canUseProduct: true,
  canAcceptBookings: true,
  canProtectExistingCommitments: true,
  upgradeRequired: false,
  isComplimentaryBridge: false,
  nextBillingAt: null,
  ...overrides,
});

describe('getSpacesSubscriptionPresentation', () => {
  it.each([
    ['TRIAL_ACTIVE', 8, 'info', false],
    ['TRIAL_EXPIRING', 3, 'warning', false],
    ['TRIAL_EXPIRED', 0, 'error', true],
    ['PAID_INACTIVE', 0, 'error', true],
    ['MISSING_STATE', 0, 'error', true],
  ] as const)('maps %s to its banner and blocked state', (status, remainingDays, severity, blocksProduct) => {
    const result = getSpacesSubscriptionPresentation(createSubscription(status, { remainingTrialDays: remainingDays, canUseProduct: !blocksProduct }));

    expect(result).toMatchObject({ showBanner: true, severity, blocksProduct });
  });

  it.each(['PAID_ACTIVE', 'COMPLIMENTARY_BRIDGE', 'LEGACY_ACTIVE'])('does not show a trial banner for %s', (status) => {
    expect(getSpacesSubscriptionPresentation(createSubscription(status))).toEqual({
      showBanner: false,
      severity: 'info',
      bannerMessage: null,
      blocksProduct: false,
    });
  });

  it('uses singular day copy at one remaining day', () => {
    const result = getSpacesSubscriptionPresentation(createSubscription('TRIAL_EXPIRING', { remainingTrialDays: 1 }));

    expect(result.bannerMessage).toContain('1 day remaining');
  });
});
