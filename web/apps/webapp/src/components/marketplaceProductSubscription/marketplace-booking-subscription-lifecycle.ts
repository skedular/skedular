export type MarketplaceBookingSubscriptionLifecycleDisplay = {
  statusLabel: string;
  statusColor: 'default' | 'success' | 'warning';
  nextRenewalFallbackLabel: string;
  renewalLabel: string;
};

type Input = {
  autoRenew: boolean;
  cancelAtPeriodEnd: boolean;
  isCancelled?: boolean;
  fallbackActiveLabel?: string;
};

export const toMarketplaceBookingSubscriptionLifecycleDisplay = ({
  autoRenew,
  cancelAtPeriodEnd,
  isCancelled = false,
  fallbackActiveLabel = 'Active',
}: Input): MarketplaceBookingSubscriptionLifecycleDisplay => {
  if (isCancelled) {
    return {
      statusLabel: 'Cancelled',
      statusColor: 'default',
      nextRenewalFallbackLabel: 'No further renewals',
      renewalLabel: 'Cancelled immediately',
    };
  }

  if (cancelAtPeriodEnd) {
    return {
      statusLabel: 'Ends at period end',
      statusColor: 'warning',
      nextRenewalFallbackLabel: 'Ends after this period',
      renewalLabel: 'Scheduled to stop at period end',
    };
  }

  return {
    statusLabel: fallbackActiveLabel,
    statusColor: autoRenew ? 'success' : 'default',
    nextRenewalFallbackLabel: autoRenew ? 'Not scheduled yet' : 'Ends after this period',
    renewalLabel: autoRenew ? 'Active' : 'Ends after this period',
  };
};
