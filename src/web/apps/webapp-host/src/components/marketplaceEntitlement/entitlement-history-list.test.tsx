import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import EntitlementHistoryList from './entitlement-history-list';

describe('EntitlementHistoryList', () => {
  it('shows expiry and refund status', () => {
    render(<EntitlementHistoryList items={[{ id: '1', status: 'EXPIRED', availableQuantity: 0, expiresAt: '2026-12-31T00:00:00Z', refundStatus: 'PENDING' }]} />);
    expect(screen.getByText(/0 credits available · EXPIRED/)).toBeInTheDocument();
    expect(screen.getByText(/Refund: PENDING/)).toBeInTheDocument();
  });

  it('shows an empty state', () => {
    render(<EntitlementHistoryList items={[]} />);
    expect(screen.getByText('No credit entitlements found.')).toBeInTheDocument();
  });

  it('shows linked booking history', () => {
    render(<EntitlementHistoryList items={[{ id: '1', status: 'ACTIVE', availableQuantity: 1, expiresAt: '2026-12-31T00:00:00Z', bookingIds: ['booking-1'] }]} />);
    expect(screen.getByText('Linked bookings: booking-1')).toBeInTheDocument();
  });

  it('shows payment and renewal state', () => {
    render(
      <EntitlementHistoryList
        items={[
          {
            id: '1',
            status: 'ACTIVE',
            availableQuantity: 1,
            expiresAt: '2026-12-31T00:00:00Z',
            paymentStatus: 'PENDING',
            renewalStatus: 'PENDING',
            nextRenewalAt: '2027-01-01T00:00:00Z',
            paymentAction: 'CONFIRM',
          },
        ]}
      />,
    );
    expect(screen.getByText('Payment: PENDING')).toBeInTheDocument();
    expect(screen.getByText(/Renewal: PENDING/)).toBeInTheDocument();
    expect(screen.getByText('Payment action: CONFIRM')).toBeInTheDocument();
  });

  it('shows zero balance, expired, and non-refundable states', () => {
    render(
      <EntitlementHistoryList
        items={[
          { id: 'zero', status: 'ACTIVE', availableQuantity: 0, expiresAt: '2026-12-31T00:00:00Z' },
          { id: 'expired', status: 'EXPIRED', availableQuantity: 0, expiresAt: '2025-12-31T00:00:00Z', refundStatus: 'NOT_ELIGIBLE' },
        ]}
      />,
    );
    expect(screen.getByText(/0 credits available · ACTIVE/)).toBeInTheDocument();
    expect(screen.getByText(/0 credits available · EXPIRED/)).toBeInTheDocument();
    expect(screen.getByText(/Refund: NOT_ELIGIBLE/)).toBeInTheDocument();
  });
});
