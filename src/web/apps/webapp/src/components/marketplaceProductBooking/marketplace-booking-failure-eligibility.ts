/**
 * Pure helpers for determining how to present a marketplace booking failure
 * in customer-facing booking details and history.
 */

export type FailureCategoryType = 'AvailabilityConflict' | 'PaymentFailed' | 'PaymentExpired';

export type FailureCustomerActionType = 'Rebook' | 'ReviewSubscription' | 'None';

export interface MarketplaceBookingFailureSummary {
  category: { type: FailureCategoryType };
  customerAction: { type: FailureCustomerActionType };
  finalizedAt: string;
  resourceReleaseStatus?: { type: 'UNKNOWN' | 'PENDING' | 'RELEASED' | '%future added value'; name: string };
  accountingCleanupStatus?: { type: 'UNKNOWN' | 'NOT_REQUIRED' | 'PENDING' | 'TRANSITION_REQUIRED' | '%future added value'; name: string };
}

export function getFormFailureToastMessage(categoryType: string): string {
  return categoryType === 'AvailabilityConflict'
    ? 'That time is no longer available. Please choose another time and submit a new booking.'
    : 'We could not complete that booking. Please start a new booking.';
}

/** Returns true when the failure represents an availability conflict. */
export function isAvailabilityConflictFailure(failure: MarketplaceBookingFailureSummary): boolean {
  return failure.category.type === 'AvailabilityConflict';
}

/** Returns true when the failure represents a payment problem (failed or expired). */
export function isPaymentFailure(failure: MarketplaceBookingFailureSummary): boolean {
  return failure.category.type === 'PaymentFailed' || failure.category.type === 'PaymentExpired';
}

/** Returns true when the customer has a recommended rebook action. */
export function hasRebookAction(failure: MarketplaceBookingFailureSummary): boolean {
  return failure.customerAction.type === 'Rebook';
}

/** Returns the category-specific copy headline for the failure card. */
export function getFailureHeadline(failure: MarketplaceBookingFailureSummary): string {
  if (failure.category.type === 'AvailabilityConflict') {
    return 'This booking could not be confirmed';
  }
  if (failure.category.type === 'PaymentFailed') {
    return 'Payment was not completed';
  }
  if (failure.category.type === 'PaymentExpired') {
    return 'Payment time expired';
  }
  return 'Booking outcome';
}

/** Returns customer-safe cleanup copy without exposing provider implementation details. */
export function getFailureCleanupMessage(failure: Pick<MarketplaceBookingFailureSummary, 'resourceReleaseStatus' | 'accountingCleanupStatus'>): string {
  if (failure.resourceReleaseStatus?.type !== 'RELEASED') {
    return 'We are releasing the reserved capacity. Check back shortly for the final status.';
  }

  if (failure.accountingCleanupStatus?.type === 'TRANSITION_REQUIRED') {
    return 'The reserved capacity has been released. A related accounting update needs follow-up, but it does not affect availability.';
  }

  if (failure.accountingCleanupStatus?.type === 'PENDING') {
    return 'The reserved capacity has been released. We are completing the related accounting update separately.';
  }

  return 'The reserved capacity has been released.';
}
