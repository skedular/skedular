import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import EntitlementBalanceCard from './entitlement-balance-card';

describe('EntitlementBalanceCard', () => {
  it('shows the entitlement restrictions and refund amount', () => {
    render(
      <EntitlementBalanceCard
        availableQuantity={3}
        grantedQuantity={5}
        expiresAt="2026-12-31T00:00:00Z"
        currency="NZD"
        refundAmount={24.5}
        restrictions={{
          availableDays: ['MONDAY', 'FRIDAY'],
          minDurationMinutes: 30,
          maxDurationMinutes: 120,
          numberOfResourcesToBook: 2,
        }}
      />,
    );

    expect(screen.getByText('Refund amount: 24.5 NZD')).toBeInTheDocument();
    expect(screen.getByText('Available days: MONDAY, FRIDAY')).toBeInTheDocument();
    expect(screen.getByText('Minimum booking duration: 30 minutes')).toBeInTheDocument();
    expect(screen.getByText('Maximum booking duration: 120 minutes')).toBeInTheDocument();
    expect(screen.getByText('Resources per booking: 2')).toBeInTheDocument();
  });
});
