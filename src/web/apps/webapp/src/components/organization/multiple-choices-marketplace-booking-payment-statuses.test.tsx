import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MultipleChoicesMarketplaceBookingPaymentStatuses from './multiple-choices-marketplace-booking-payment-statuses';

const useFragmentMock = vi.fn();

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
}));

vi.mock('mui-rff', () => ({
  Autocomplete: ({ options, multiple }: { options: { type: string; name: string }[]; multiple?: boolean }) => (
    <div data-testid="autocomplete" data-multiple={multiple ? 'true' : 'false'}>
      {options.map((option) => (
        <span key={option.type} data-testid={`option-${option.type}`}>
          {option.name}
        </span>
      ))}
    </div>
  ),
}));

vi.mock('@skedular/ui', () => ({
  BodyIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
}));

vi.mock('@mui/material/useAutocomplete', () => ({
  createFilterOptions: () => (options: unknown[]) => options,
}));

const paymentStatusOptions = [
  { type: 'NOT_SET', name: 'Not set' },
  { type: 'PENDING', name: 'Pending' },
  { type: 'REJECTED', name: 'Rejected' },
  { type: 'CONFIRMED', name: 'Confirmed' },
  { type: 'EXPIRED', name: 'Expired' },
  { type: 'NO_PAYMENT_REQUIRED', name: 'No payment required' },
];

describe('MultipleChoicesMarketplaceBookingPaymentStatuses', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    useFragmentMock.mockImplementation(() => ({
      marketplaceBookingPaymentStatuses: paymentStatusOptions,
    }));
  });

  it('renders all 6 payment status options', () => {
    render(<MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={{} as never} name="paymentStatuses" />);

    expect(screen.getByTestId('option-NOT_SET')).toBeInTheDocument();
    expect(screen.getByTestId('option-PENDING')).toBeInTheDocument();
    expect(screen.getByTestId('option-REJECTED')).toBeInTheDocument();
    expect(screen.getByTestId('option-CONFIRMED')).toBeInTheDocument();
    expect(screen.getByTestId('option-EXPIRED')).toBeInTheDocument();
    expect(screen.getByTestId('option-NO_PAYMENT_REQUIRED')).toBeInTheDocument();
  });

  it('renders with multiple selection enabled', () => {
    render(<MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={{} as never} name="paymentStatuses" />);

    expect(screen.getByTestId('autocomplete')).toHaveAttribute('data-multiple', 'true');
  });

  it('displays British-spelled display names for all payment status options', () => {
    render(<MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={{} as never} name="paymentStatuses" />);

    expect(screen.getByText('Not set')).toBeInTheDocument();
    expect(screen.getByText('Pending')).toBeInTheDocument();
    expect(screen.getByText('Rejected')).toBeInTheDocument();
    expect(screen.getByText('Confirmed')).toBeInTheDocument();
    expect(screen.getByText('Expired')).toBeInTheDocument();
    expect(screen.getByText('No payment required')).toBeInTheDocument();
  });
});
