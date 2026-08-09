import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import EditMarketplaceBooking from './edit-marketplace-booking';

const backMock = vi.fn();
const commitModifyMarketplaceBooking = vi.fn();
const useMutationMock = vi.fn();

const booking = {
  id: 'booking-1',
  entityFrameworkVersion: 4,
  from: '2026-08-10T09:00:00.000Z',
  until: '2026-08-10T10:00:00.000Z',
  involvedCustomers: [{ id: 'customer-1', name: 'Sam Carter', givenName: 'Sam', middleName: null, familyName: 'Carter' }],
  involvedLocations: [{ name: 'Entire venue' }],
  bookingResources: [],
};

vi.mock('next/navigation', () => ({ useRouter: () => ({ back: backMock, push: vi.fn() }) }));
vi.mock('react-toastify', () => ({ toast: vi.fn() }));
vi.mock('@skedular/shared', () => ({
  PaletteModeContext: createContext('light'),
  getCustomerFullName: (customer: { name: string }) => customer.name,
  getOpeningHoursFromDateTime: () => undefined,
  getRelayErrorMessage: () => 'Request failed',
  isMidnight: () => false,
  toOpeningHoursFromTime: () => undefined,
  toShortDate: () => 'Aug 10',
  toShortTime: () => '9:00 AM',
}));
vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useFragment: () => ({ organizationBookingPermissions: { canModifyPaymentMethod: false }, marketplaceBookingSubscriptions: { edges: [] }, paymentStatuses: [] }),
  useRefetchableFragment: () => [{ booking }],
  useMutation: (...args: unknown[]) => useMutationMock(...args),
}));
vi.mock('@skedular/ui', () => ({
  MarketplaceBookingModificationForm: (props: {
    canSelectResources: boolean;
    resources: Array<{ id: string }>;
    onSubmit: (values: { from: string; until: string; reason: string }) => void;
  }) => (
    <div>
      <span>{props.canSelectResources ? 'Resource picker' : 'Whole-place schedule only'}</span>
      <span>{props.resources.length} resources</span>
      <button onClick={() => props.onSubmit({ from: '2026-08-11T09:00:00.000Z', until: '2026-08-11T10:00:00.000Z', reason: 'Venue maintenance' })}>Update booking</button>
    </div>
  ),
}));

describe('EditMarketplaceBooking (Host)', () => {
  beforeEach(() => {
    backMock.mockReset();
    commitModifyMarketplaceBooking.mockReset();
    useMutationMock.mockReset();
    useMutationMock.mockReturnValue([commitModifyMarketplaceBooking, false]);
  });

  it('limits Host operators to a whole-place date and time change', () => {
    render(<EditMarketplaceBooking rootDataRelay={{} as never} rootDataBookingRelay={{} as never} page />);

    expect(screen.getByText('Whole-place schedule only')).toBeInTheDocument();
    expect(screen.getByText('0 resources')).toBeInTheDocument();
  });

  it('submits the operator reason without a resource selection and returns after success', async () => {
    const user = userEvent.setup();
    commitModifyMarketplaceBooking.mockImplementation(({ onCompleted }) =>
      onCompleted({ modifyMarketplaceBooking: { booking: { id: 'booking-1' }, modification: { id: 'modification-1' } } }),
    );
    render(<EditMarketplaceBooking rootDataRelay={{} as never} rootDataBookingRelay={{} as never} page />);

    await user.click(screen.getByRole('button', { name: 'Update booking' }));

    expect(commitModifyMarketplaceBooking).toHaveBeenCalledWith(
      expect.objectContaining({ variables: { input: expect.objectContaining({ reason: 'Venue maintenance', actorKind: 'ORGANIZATION_OPERATOR' }) } }),
    );
    expect(commitModifyMarketplaceBooking.mock.calls[0]?.[0].variables.input).not.toHaveProperty('resourceIds');
    expect(backMock).toHaveBeenCalledOnce();
  });
});
