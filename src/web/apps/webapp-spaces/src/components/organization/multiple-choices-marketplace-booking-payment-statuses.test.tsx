import { render, screen } from '@testing-library/react';
import { Form } from 'react-final-form';
import type { ReactNode } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MultipleChoicesMarketplaceBookingPaymentStatuses from './multiple-choices-marketplace-booking-payment-statuses';

const useFragmentMock = vi.fn();

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
}));

vi.mock('@/components/styled', () => ({
  DefaultSelect: ({ children, value, onChange }: { children: ReactNode; value?: string; onChange?: (event: React.ChangeEvent<HTMLSelectElement>) => void }) => (
    <select data-testid="select" value={value ?? ''} onChange={onChange}>
      {children}
    </select>
  ),
}));
vi.mock('@mui/material/MenuItem', () => ({
  default: ({ children, value }: { children: ReactNode; value?: string }) => (
    <option data-testid={value ? `option-${value}` : undefined} value={value}>
      {children}
    </option>
  ),
}));

vi.mock('@skedular/ui', () => ({
  BodyIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  LeadIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  SmallIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  StackRow: ({ children }: { children: ReactNode }) => <span>{children}</span>,
  PushToRight: () => null,
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
    render(<Form onSubmit={() => {}} render={() => <MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={{} as never} name="paymentStatuses" />} />);

    expect(screen.getByTestId('option-NOT_SET')).toBeInTheDocument();
    expect(screen.getByTestId('option-PENDING')).toBeInTheDocument();
    expect(screen.getByTestId('option-REJECTED')).toBeInTheDocument();
    expect(screen.getByTestId('option-CONFIRMED')).toBeInTheDocument();
    expect(screen.getByTestId('option-EXPIRED')).toBeInTheDocument();
    expect(screen.getByTestId('option-NO_PAYMENT_REQUIRED')).toBeInTheDocument();
  });

  it('renders as a single select with an all option', () => {
    render(<Form onSubmit={() => {}} render={() => <MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={{} as never} name="paymentStatuses" />} />);

    expect(screen.getByTestId('select')).not.toHaveAttribute('multiple');
    expect(screen.getByText('All payments')).toBeInTheDocument();
  });

  it('displays British-spelled display names for all payment status options', () => {
    render(<Form onSubmit={() => {}} render={() => <MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={{} as never} name="paymentStatuses" />} />);

    expect(screen.getByText('Not set')).toBeInTheDocument();
    expect(screen.getByText('Pending')).toBeInTheDocument();
    expect(screen.getByText('Rejected')).toBeInTheDocument();
    expect(screen.getByText('Confirmed')).toBeInTheDocument();
    expect(screen.getByText('Expired')).toBeInTheDocument();
    expect(screen.getByText('No payment required')).toBeInTheDocument();
  });
});
