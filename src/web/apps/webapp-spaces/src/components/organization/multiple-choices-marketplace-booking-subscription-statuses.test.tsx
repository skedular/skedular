import { render, screen } from '@testing-library/react';
import { Form } from 'react-final-form';
import type { ReactNode } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MultipleChoicesMarketplaceBookingSubscriptionStatuses from './multiple-choices-marketplace-booking-subscription-statuses';

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

const subscriptionStatusOptions = [
  { type: 'ACTIVE', name: 'Active' },
  { type: 'CANCELLED', name: 'Cancelled' },
  { type: 'EXPIRED', name: 'Expired' },
  { type: 'RENEWAL_FAILED', name: 'Renewal failed' },
  { type: 'PAUSED', name: 'Paused' },
];

describe('MultipleChoicesMarketplaceBookingSubscriptionStatuses', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    useFragmentMock.mockImplementation(() => ({
      marketplaceBookingSubscriptionStatuses: subscriptionStatusOptions,
    }));
  });

  it('renders all 5 subscription status options', () => {
    render(<Form onSubmit={() => {}} render={() => <MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={{} as never} name="statuses" />} />);

    expect(screen.getByTestId('option-ACTIVE')).toBeInTheDocument();
    expect(screen.getByTestId('option-CANCELLED')).toBeInTheDocument();
    expect(screen.getByTestId('option-EXPIRED')).toBeInTheDocument();
    expect(screen.getByTestId('option-RENEWAL_FAILED')).toBeInTheDocument();
    expect(screen.getByTestId('option-PAUSED')).toBeInTheDocument();
  });

  it('renders as a single select with an all option', () => {
    render(<Form onSubmit={() => {}} render={() => <MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={{} as never} name="statuses" />} />);

    expect(screen.getByTestId('select')).not.toHaveAttribute('multiple');
    expect(screen.getByText('All statuses')).toBeInTheDocument();
  });

  it('displays the correct display names for all status options', () => {
    render(<Form onSubmit={() => {}} render={() => <MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={{} as never} name="statuses" />} />);

    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Cancelled')).toBeInTheDocument();
    expect(screen.getByText('Expired')).toBeInTheDocument();
    expect(screen.getByText('Renewal failed')).toBeInTheDocument();
    expect(screen.getByText('Paused')).toBeInTheDocument();
  });
});
