export type SupportedMarketplaceBookingSubscriptionStatusForFilter = 'ACTIVE' | 'CANCELLED' | 'EXPIRED' | 'RENEWAL_FAILED' | 'PAUSED';

export type MarketplaceBookingSubscriptionStatusForFilterDetails = {
  type: SupportedMarketplaceBookingSubscriptionStatusForFilter;
  name: string;
};

export const isSupportedMarketplaceBookingSubscriptionStatusForFilter = (type: string): type is SupportedMarketplaceBookingSubscriptionStatusForFilter =>
  type === 'ACTIVE' || type === 'CANCELLED' || type === 'EXPIRED' || type === 'RENEWAL_FAILED' || type === 'PAUSED';

export const toMarketplaceBookingSubscriptionStatusForFilterDetails = (type: string, name: string): MarketplaceBookingSubscriptionStatusForFilterDetails | null =>
  isSupportedMarketplaceBookingSubscriptionStatusForFilter(type) ? { type, name } : null;
