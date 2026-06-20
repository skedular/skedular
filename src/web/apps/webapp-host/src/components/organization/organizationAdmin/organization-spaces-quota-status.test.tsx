import { SpacesSubscriptionProvider, type SpacesSubscriptionState } from '@/components/rootShell/spaces-subscription-context';
import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { SpacesQuotaStatus } from './organization-spaces-quota-status';

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  usePreloadedQuery: () => ({ bookingSpacesQuotaStatus: null }),
  useQueryLoader: () => [null, vi.fn()],
}));

const createSubscription = (subscriptionStatus: string, overrides: Partial<SpacesSubscriptionState> = {}): SpacesSubscriptionState => ({
  subscriptionStatus,
  accessReason: 'ALLOWED_TRIAL',
  trialStartedAt: '2026-06-01T00:00:00Z',
  trialEndsAt: '2026-06-15T00:00:00Z',
  remainingTrialDays: 3,
  canUseProduct: true,
  canAcceptBookings: true,
  canProtectExistingCommitments: true,
  upgradeRequired: false,
  isComplimentaryBridge: false,
  nextBillingAt: null,
  ...overrides,
});

const renderStatus = (subscription: SpacesSubscriptionState | null) =>
  render(
    <SpacesSubscriptionProvider value={subscription}>
      <SpacesQuotaStatus organizationId="acme" />
    </SpacesSubscriptionProvider>,
  );

describe('SpacesQuotaStatus', () => {
  it('shows remaining days and the retained monthly quota during the trial', () => {
    renderStatus(createSubscription('TRIAL_EXPIRING'));

    expect(screen.getByText('14-day free trial')).toBeInTheDocument();
    expect(screen.getByText(/3 days remaining/)).toBeInTheDocument();
    expect(screen.getByText(/100 booking instances per month/)).toBeInTheDocument();
  });

  it('shows preserved-data upgrade guidance after expiry', () => {
    renderStatus(createSubscription('TRIAL_EXPIRED', { remainingTrialDays: 0, canUseProduct: false }));

    expect(screen.getByText('14-day trial expired')).toBeInTheDocument();
    expect(screen.getByText(/data and configuration are preserved/)).toBeInTheDocument();
  });

  it.each([null, createSubscription('MISSING_STATE', { canUseProduct: false }), createSubscription('PAID_INACTIVE', { canUseProduct: false })])(
    'fails closed when subscription state is unavailable',
    (subscription) => {
      renderStatus(subscription);

      expect(screen.getByText('Spaces subscription unavailable')).toBeInTheDocument();
      expect(screen.getByText(/contact support/)).toBeInTheDocument();
    },
  );
});
