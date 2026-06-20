import type { SpacesSubscriptionState } from './spaces-subscription-context';

export type SpacesSubscriptionPresentation = {
  readonly showBanner: boolean;
  readonly severity: 'error' | 'info' | 'warning';
  readonly bannerMessage: string | null;
  readonly blocksProduct: boolean;
};

export const getSpacesSubscriptionPresentation = (subscription: SpacesSubscriptionState | null | undefined): SpacesSubscriptionPresentation => {
  const status = subscription?.subscriptionStatus;

  if (subscription && (status === 'TRIAL_ACTIVE' || status === 'TRIAL_EXPIRING')) {
    const days = subscription.remainingTrialDays;
    return {
      showBanner: true,
      severity: status === 'TRIAL_EXPIRING' ? 'warning' : 'info',
      bannerMessage: `Your 14-day Spaces trial has ${days} ${days === 1 ? 'day' : 'days'} remaining.`,
      blocksProduct: false,
    };
  }

  if (status === 'TRIAL_EXPIRED') {
    return {
      showBanner: true,
      severity: 'error',
      bannerMessage: 'Your 14-day Spaces trial has ended. Upgrade to continue using Spaces and accepting bookings.',
      blocksProduct: true,
    };
  }

  if (status === 'PAID_INACTIVE' || status === 'MISSING_STATE' || subscription?.canUseProduct === false) {
    return {
      showBanner: true,
      severity: 'error',
      bannerMessage: 'Spaces is currently unavailable. Review your subscription or contact support to restore access.',
      blocksProduct: true,
    };
  }

  return { showBanner: false, severity: 'info', bannerMessage: null, blocksProduct: false };
};
