export type MarketplacePaymentStatusType = 'CONFIRMED' | 'PENDING' | 'REJECTED' | 'EXPIRED' | 'NOT_SET' | string;

export const canRequestMarketplaceBookingCancellation = ({ bookingStartsAt, isCancelled, now }: { bookingStartsAt?: string | null; isCancelled: boolean; now: Date }) => {
  if (!bookingStartsAt || isCancelled) {
    return false;
  }

  return new Date(bookingStartsAt).getTime() > now.getTime();
};

export const shouldEnterRefundLifecycle = ({ hasConfirmedPayment, isCancellationAccepted }: { hasConfirmedPayment: boolean; isCancellationAccepted: boolean }) =>
  hasConfirmedPayment && isCancellationAccepted;

export const canRequestMarketplaceSubscriptionCancellation = ({ isActive, cancellationModeAvailable }: { isActive: boolean; cancellationModeAvailable: boolean }) =>
  isActive && cancellationModeAvailable;
