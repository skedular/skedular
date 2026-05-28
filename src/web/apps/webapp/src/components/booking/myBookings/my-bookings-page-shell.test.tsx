import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import MyBookingsPageShell from './my-bookings-page-shell';

describe('MyBookingsPageShell', () => {
  it('renders the modern bookings collection shell', () => {
    render(
      <MyBookingsPageShell isEmpty={false}>
        <div>cards</div>
      </MyBookingsPageShell>,
    );

    expect(screen.getByText('My Bookings')).toBeInTheDocument();
    expect(screen.getByText('Review the bookings that matter to you for the selected week.')).toBeInTheDocument();
    expect(screen.getByText('cards')).toBeInTheDocument();
  });

  it('renders the empty state when there are no bookings', () => {
    render(<MyBookingsPageShell isEmpty>empty</MyBookingsPageShell>);

    expect(screen.getByText('No bookings match the current filters.')).toBeInTheDocument();
  });
});
