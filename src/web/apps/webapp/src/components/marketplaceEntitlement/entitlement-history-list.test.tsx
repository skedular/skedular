import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import EntitlementHistoryList from './entitlement-history-list';

describe('EntitlementHistoryList', () => {
  it('shows balance, expiry, and refund status', () => {
    render(<EntitlementHistoryList items={[{ id: '1', status: 'ACTIVE', availableQuantity: 2, expiresAt: '2026-12-31T00:00:00Z', refundStatus: 'COMPLETED' }]} />);
    expect(screen.getByText(/2 credits available · ACTIVE/)).toBeInTheDocument();
    expect(screen.getByText(/Refund: COMPLETED/)).toBeInTheDocument();
  });

  it('shows an empty state', () => {
    render(<EntitlementHistoryList items={[]} />);
    expect(screen.getByText('No credit entitlements found.')).toBeInTheDocument();
  });

  it('shows linked booking history', () => {
    render(<EntitlementHistoryList items={[{ id: '1', status: 'ACTIVE', availableQuantity: 1, expiresAt: '2026-12-31T00:00:00Z', bookingIds: ['booking-1'] }]} />);
    expect(screen.getByText('Linked bookings: booking-1')).toBeInTheDocument();
  });
});
