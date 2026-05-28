export type SupportedMarketplaceBookingPaymentStatusForFilter = 'NOT_SET' | 'PENDING' | 'REJECTED' | 'CONFIRMED' | 'EXPIRED' | 'NO_PAYMENT_REQUIRED';

export type MarketplaceBookingPaymentStatusForFilterDetails = {
  type: SupportedMarketplaceBookingPaymentStatusForFilter;
  name: string;
};

export const isSupportedMarketplaceBookingPaymentStatusForFilter = (type: string): type is SupportedMarketplaceBookingPaymentStatusForFilter =>
  type === 'NOT_SET' || type === 'PENDING' || type === 'REJECTED' || type === 'CONFIRMED' || type === 'EXPIRED' || type === 'NO_PAYMENT_REQUIRED';

export const toMarketplaceBookingPaymentStatusForFilterDetails = (type: string, name: string): MarketplaceBookingPaymentStatusForFilterDetails | null =>
  isSupportedMarketplaceBookingPaymentStatusForFilter(type) ? { type, name } : null;
