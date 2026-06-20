import { readFileSync } from 'node:fs';
import { describe, expect, it } from 'vitest';

const readEditor = (path: string) => readFileSync(new URL(path, import.meta.url), 'utf8');

describe('booking edit autosave', () => {
  it('autosaves private booking edit groups without the manual update action', () => {
    const source = readEditor('./editPrivateBooking/edit-private-booking.tsx');

    expect(source).toContain('debouncedBookingDetailUpdate');
    expect(source).not.toContain('onSubmit={handleBookingDetailUpdateClick}');
    expect(source).not.toContain('primaryAction="Update Booking"');
  });

  it('keeps marketplace booking customer details read-only', () => {
    const source = readEditor('./editMarketplaceBooking/edit-marketplace-booking.tsx');

    expect(source).toContain('Customer Details');
    expect(source).toContain('read-only');
    expect(source).not.toContain('debouncedBookingDetailUpdate');
    expect(source).not.toContain('updateMarketplaceBooking');
    expect(source).not.toContain('type="submit"');
  });

  it('autosaves recurring booking edit groups without the manual update action', () => {
    const source = readEditor('./editPrivateRecurringBooking/edit-private-recurring-booking.tsx');

    expect(source).toContain('debouncedBookingDetailUpdate');
    expect(source).not.toContain('onSubmit={handleSubmit}');
    expect(source).not.toContain('primaryAction="Update recurring booking"');
  });

  it('shows failed-state feedback for booking edits', () => {
    const source = readEditor('./editPrivateBooking/edit-private-booking.tsx');
    const marketplaceSource = readEditor('./editMarketplaceBooking/edit-marketplace-booking.tsx');
    const recurringSource = readEditor('./editPrivateRecurringBooking/edit-private-recurring-booking.tsx');

    expect(source).toContain('errorNotificationOptions');
    expect(marketplaceSource).toContain('errorNotificationOptions');
    expect(recurringSource).toContain('errorNotificationOptions');
  });
});
