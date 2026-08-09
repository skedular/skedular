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
  involvedLocations: [{ uniqueId: 'location-1', name: 'Main office' }],
  bookingResources: [{ resource: { id: 'resource-1', name: 'Desk one' } }],
  marketplaceBookingResourceSelection: {
    canSelectResources: true,
    maximumResourceCount: 2,
    availableResourceIds: ['resource-1', 'resource-2'],
    eligibleLocations: [{ uniqueId: 'location-1', name: 'Main office' }],
    eligibleResources: [{ resource: { id: 'resource-1', name: 'Desk one' } }, { resource: { id: 'resource-2', name: 'Desk two' } }],
  },
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
    maximumResourceCount: number;
    resources: Array<{ id: string }>;
    onSubmit: (values: { from: string; until: string; reason: string; resourceIds: string[] }) => void;
  }) => (
    <div>
      <span>{props.canSelectResources ? `Resource picker (${props.maximumResourceCount})` : 'No resource picker'}</span>
      <span>{props.resources.map((resource) => resource.id).join(',')}</span>
      <button
        onClick={() => props.onSubmit({ from: '2026-08-11T09:00:00.000Z', until: '2026-08-11T10:00:00.000Z', reason: 'Customer asked to move', resourceIds: ['resource-2'] })}
      >
        Update booking
      </button>
    </div>
  ),
}));

describe('EditMarketplaceBooking (Spaces)', () => {
  beforeEach(() => {
    backMock.mockReset();
    commitModifyMarketplaceBooking.mockReset();
    useMutationMock.mockReset();
    useMutationMock.mockReturnValue([commitModifyMarketplaceBooking, false]);
  });

  it('provides an authorized operator with eligible replacement resources up to the purchased limit', () => {
    render(<EditMarketplaceBooking rootDataRelay={{} as never} rootDataBookingRelay={{} as never} page />);

    expect(screen.getByText('Resource picker (2)')).toBeInTheDocument();
    expect(screen.getByText('resource-1,resource-2')).toBeInTheDocument();
  });

  it('submits the required reason and replacement resources, then returns after the booking succeeds', async () => {
    const user = userEvent.setup();
    commitModifyMarketplaceBooking.mockImplementation(({ onCompleted }) =>
      onCompleted({ modifyMarketplaceBooking: { booking: { id: 'booking-1' }, modification: { id: 'modification-1' } } }),
    );
    render(<EditMarketplaceBooking rootDataRelay={{} as never} rootDataBookingRelay={{} as never} page />);

    await user.click(screen.getByRole('button', { name: 'Update booking' }));

    expect(commitModifyMarketplaceBooking).toHaveBeenCalledWith(
      expect.objectContaining({
        variables: {
          input: expect.objectContaining({ bookingId: 'booking-1', actorKind: 'ORGANIZATION_OPERATOR', reason: 'Customer asked to move', resourceIds: ['resource-2'] }),
        },
      }),
    );
    expect(backMock).toHaveBeenCalledOnce();
  });

  it('does not require a notification payload before reflecting a successful booking update', async () => {
    const user = userEvent.setup();
    commitModifyMarketplaceBooking.mockImplementation(({ onCompleted }) =>
      onCompleted({ modifyMarketplaceBooking: { booking: { id: 'booking-1' }, modification: { id: 'modification-1' } } }),
    );
    render(<EditMarketplaceBooking rootDataRelay={{} as never} rootDataBookingRelay={{} as never} page />);

    await user.click(screen.getByRole('button', { name: 'Update booking' }));

    expect(backMock).toHaveBeenCalledOnce();
  });
});
