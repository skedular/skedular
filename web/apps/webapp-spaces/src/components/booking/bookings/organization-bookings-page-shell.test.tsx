import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import OrganizationBookingsPageShell from './organization-bookings-page-shell';

describe('OrganizationBookingsPageShell', () => {
  it('renders the home collection shell', () => {
    render(
      <OrganizationBookingsPageShell isEmpty={false}>
        <div>cards</div>
      </OrganizationBookingsPageShell>,
    );

    expect(screen.getByText('Home')).toBeInTheDocument();
    expect(screen.getByText('Review and manage bookings across the organization for the selected week.')).toBeInTheDocument();
    expect(screen.getByText('cards')).toBeInTheDocument();
  });

  it('renders the empty state when there are no bookings', () => {
    render(<OrganizationBookingsPageShell isEmpty>ignored</OrganizationBookingsPageShell>);

    expect(screen.getByText('No bookings match the current filters.')).toBeInTheDocument();
  });
});
