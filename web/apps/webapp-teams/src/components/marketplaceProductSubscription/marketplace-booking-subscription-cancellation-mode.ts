export type SupportedMarketplaceBookingSubscriptionCancellationMode = 'IMMEDIATE' | 'AT_PERIOD_END';

export type SupportedMarketplaceBookingSubscriptionCancellationModeDetails = {
  type: SupportedMarketplaceBookingSubscriptionCancellationMode;
  name: string;
};

export const isSupportedMarketplaceBookingSubscriptionCancellationMode = (type: string): type is SupportedMarketplaceBookingSubscriptionCancellationMode =>
  type === 'IMMEDIATE' || type === 'AT_PERIOD_END';

export const toSupportedMarketplaceBookingSubscriptionCancellationModeDetails = (
  type: string,
  name: string,
): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => (isSupportedMarketplaceBookingSubscriptionCancellationMode(type) ? { type, name } : null);
