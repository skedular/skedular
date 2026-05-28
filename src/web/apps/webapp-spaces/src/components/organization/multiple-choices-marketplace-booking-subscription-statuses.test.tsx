import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MultipleChoicesMarketplaceBookingSubscriptionStatuses from './multiple-choices-marketplace-booking-subscription-statuses';

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
    render(<MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={{} as never} name="statuses" />);

    expect(screen.getByTestId('option-ACTIVE')).toBeInTheDocument();
    expect(screen.getByTestId('option-CANCELLED')).toBeInTheDocument();
    expect(screen.getByTestId('option-EXPIRED')).toBeInTheDocument();
    expect(screen.getByTestId('option-RENEWAL_FAILED')).toBeInTheDocument();
    expect(screen.getByTestId('option-PAUSED')).toBeInTheDocument();
  });

  it('renders with multiple selection enabled', () => {
    render(<MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={{} as never} name="statuses" />);

    expect(screen.getByTestId('autocomplete')).toHaveAttribute('data-multiple', 'true');
  });

  it('displays the correct display names for all status options', () => {
    render(<MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={{} as never} name="statuses" />);

    expect(screen.getByText('Active')).toBeInTheDocument();
    expect(screen.getByText('Cancelled')).toBeInTheDocument();
    expect(screen.getByText('Expired')).toBeInTheDocument();
    expect(screen.getByText('Renewal failed')).toBeInTheDocument();
    expect(screen.getByText('Paused')).toBeInTheDocument();
  });
});
