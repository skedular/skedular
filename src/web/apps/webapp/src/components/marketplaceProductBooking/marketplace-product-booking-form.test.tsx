import { describe, expect, it } from 'vitest';
import { getFormFailureToastMessage } from './marketplace-booking-failure-eligibility';

describe('marketplace product booking form failure toast', () => {
  it('shows availability-specific message for AvailabilityConflict', () => {
    const message = getFormFailureToastMessage('AvailabilityConflict');
    expect(message).toContain('no longer available');
    expect(message).toContain('new booking');
  });

  it('shows generic message for PaymentFailed', () => {
    const message = getFormFailureToastMessage('PaymentFailed');
    expect(message).toContain('could not complete');
    expect(message).toContain('new booking');
  });

  it('shows generic message for PaymentExpired', () => {
    const message = getFormFailureToastMessage('PaymentExpired');
    expect(message).toContain('could not complete');
    expect(message).toContain('new booking');
  });

  it('availability message is distinct from payment message', () => {
    const availabilityMessage = getFormFailureToastMessage('AvailabilityConflict');
    const paymentMessage = getFormFailureToastMessage('PaymentFailed');
    expect(availabilityMessage).not.toBe(paymentMessage);
  });
});
