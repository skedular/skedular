import { render, screen } from '@testing-library/react';
import dayjs from 'dayjs';
import { describe, expect, it, vi } from 'vitest';
import MyBookings from './my-bookings';

const refetchMock = vi.fn();
const useFragmentMock = vi.fn();
const useRefetchableFragmentMock = vi.fn();

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useRefetchableFragment: (...args: unknown[]) => useRefetchableFragmentMock(...args),
}));

vi.mock('./my-booking-card', () => ({
  default: ({ bookingDetailsRelay }: { bookingDetailsRelay: { id: string } }) => <div data-testid="booking-card">{bookingDetailsRelay.id}</div>,
}));

vi.mock('./my-bookings-page-shell', () => ({
  default: ({ children }: React.PropsWithChildren) => <div>{children}</div>,
}));

const makeBooking = (id: string) => ({
  id,
  from: '2026-04-12T09:00:00.000Z',
  until: '2026-04-12T11:00:00.000Z',
  notes: null,
  channel: { channel: 'PRIVATE' },
  involvedCustomers: [{ id: 'user-1', name: 'Alice', givenName: 'Alice', middleName: null, familyName: 'Smith', photoUrl: null }],
  involvedLocations: [{ uniqueId: 'loc-1', name: 'Office' }],
  involvedTeams: [],
  bookingResources: [],
});

describe('MyBookings', () => {
  it('renders bookings returned by the server-side private channel filter', () => {
    useFragmentMock.mockReturnValue({ me: { id: 'user-1' } });
    useRefetchableFragmentMock.mockReturnValue([
      {
        bookings: {
          __id: 'connection-1',
          totalCount: 2,
          edges: [{ node: makeBooking('private-booking-1') }],
        },
      },
      refetchMock,
    ]);

    render(
      <MyBookings
        rootDataRelay={{} as never}
        rootDataBookingRelay={{} as never}
        organizationCustomDomain="acme"
        from={dayjs('2026-04-12')}
        to={dayjs('2026-04-12')}
        locationIds={[]}
        teamIds={[]}
      />,
    );

    expect(screen.getByText('private-booking-1')).toBeInTheDocument();
  });
});
