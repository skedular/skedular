import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import BookingCard from './booking-card';

const pushMock = vi.fn();
const useFragmentMock = vi.fn();

const queryFragmentData = {
  me: {
    id: 'customer-1',
    name: 'Sam Carter',
    givenName: 'Sam',
    middleName: null,
    familyName: 'Carter',
    photoUrl: null,
  },
  organizationBookingPermissions: {
    canModifyPaymentMethod: true,
  },
  paymentStatuses: [
    { type: 'CONFIRMED', name: 'Confirmed' },
    { type: 'REJECTED', name: 'Rejected' },
    { type: 'NO_PAYMENT_REQUIRED', name: 'No payment required' },
  ],
};

const bookingFragmentData = {
  id: 'booking-1',
  from: '2026-04-12T09:00:00.000Z',
  until: '2026-04-12T11:00:00.000Z',
  notes: 'Needs projector access',
  category: { category: 'PRIVATE', name: 'Private' },
  channel: { channel: 'MARKETPLACE', name: 'Marketplace' },
  involvedCustomers: [
    { id: 'customer-1', givenName: 'Sam', middleName: null, familyName: 'Carter', name: 'Sam Carter', photoUrl: null },
    { id: 'customer-2', givenName: 'Alex', middleName: null, familyName: 'Ng', name: 'Alex Ng', photoUrl: null },
  ],
  involvedOrganizations: [{ id: 'organization-1' }],
  involvedLocations: [{ uniqueId: 'location-1', name: 'HQ Level 3' }],
  involvedTeams: [{ id: 'team-1', name: 'Operations' }],
  bookingResources: [
    {
      resource: {
        id: 'resource-1',
        name: 'Desk A1',
        color: '#123456',
        customTags: [{ id: 'tag-1', name: 'Monitor', color: '#ff00ff' }],
        zones: [{ id: 'zone-1', name: 'North Wing', color: '#00ffaa' }],
      },
    },
  ],
  marketplaceBooking: {
    id: 'marketplace-booking-1',
    isPaymentRequired: true,
    paymentStatus: { type: 'CONFIRMED', name: 'Confirmed' },
    invoiceUrl: 'https://example.com/invoice.pdf',
    refund: null,
  },
};

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

vi.mock('next/link', () => ({
  default: ({ children, href, ...props }: React.AnchorHTMLAttributes<HTMLAnchorElement>) => (
    <a href={typeof href === 'string' ? href : '#'} {...props}>
      {children}
    </a>
  ),
}));

vi.mock(import('@/libs/providers'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatrform: () => ({ integratedPlatrform: 'web' }),
  };
});

vi.mock('@/components/links', () => ({
  getOrganizationBookingBaseLink: () => '/bookings/booking-1',
}));

vi.mock('@/components/moreActionsMenu', () => ({
  MoreActionsMenu: () => null,
  moreActionsMenuAllOptions: {
    EditBooking: [{ id: 'EditBooking', label: 'Edit Booking' }],
    DeleteBooking: [{ id: 'DeleteBooking', label: 'Delete Booking' }],
    ConfirmBookingPayment: [{ id: 'ConfirmBookingPayment', label: 'Confirm Booking Payment' }],
    RejectBookingPayment: [{ id: 'RejectBookingPayment', label: 'Reject Booking Payment' }],
    MakeBookingPaymentNotRequired: [{ id: 'MakeBookingPaymentNotRequired', label: 'Make Booking Payment Not Required' }],
  },
  MoreActionsMenuOptionType: {
    EditBooking: 'EditBooking',
    DeleteBooking: 'DeleteBooking',
    ConfirmBookingPayment: 'ConfirmBookingPayment',
    RejectBookingPayment: 'RejectBookingPayment',
    MakeBookingPaymentNotRequired: 'MakeBookingPaymentNotRequired',
  },
}));

vi.mock('@/components/icons', () => ({
  CalendarIcon: () => <span>calendar-icon</span>,
  EllipseMenuIcon: () => <span>menu</span>,
  JoinIcon: () => <span>join-icon</span>,
  NotesIcon: () => <span>notes-icon</span>,
  PaymentStatusIcon: () => <span>payment-icon</span>,
  PdfIcon: () => <span>pdf-icon</span>,
  TeamIcon: () => <span>team-icon</span>,
}));

vi.mock('@/components/resource', () => ({
  Resources: ({ resources }: { resources: Array<{ name: string }> }) => <div>{resources.map((item) => item.name).join(', ')}</div>,
}));

vi.mock('@/components/customTag', () => ({
  CustomTags: ({ customTags }: { customTags: Array<{ name: string }> }) => <div>{customTags.map((item) => item.name).join(', ')}</div>,
}));

vi.mock('@/components/zone', () => ({
  Zones: ({ zones }: { zones: Array<{ name: string }> }) => <div>{zones.map((item) => item.name).join(', ')}</div>,
}));

vi.mock('@/components/marketplaceRefund/marketplace-refund-admin-panel', () => ({
  default: () => <div>Refund panel</div>,
}));

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useMutation: () => [vi.fn()],
}));

describe('BookingCard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    pushMock.mockReset();
    useFragmentMock.mockImplementation((query: string) => {
      if (query.includes('fragment bookingCard_query')) {
        return queryFragmentData;
      }

      return bookingFragmentData;
    });
  });

  it('renders the compact organization booking card layout', () => {
    render(<BookingCard rootDataRelay={{} as never} bookingDetailsRelay={{} as never} organizationCustomDomain="acme" connectionIds={[]} canJoinBooking />);

    expect(screen.getByText('HQ Level 3')).toBeInTheDocument();
    expect(screen.getByText('Operations')).toBeInTheDocument();
    expect(screen.getByText('Confirmed')).toBeInTheDocument();
    expect(screen.queryByText('People')).not.toBeInTheDocument();
    expect(screen.getByText('Booking details')).toBeInTheDocument();
    expect(screen.getByText('Sam Carter, Alex Ng')).toBeInTheDocument();
    expect(screen.getByText('Desk A1')).toBeInTheDocument();
    expect(screen.getByText('Monitor')).toBeInTheDocument();
    expect(screen.getByText('North Wing')).toBeInTheDocument();
    expect(screen.getByText('Needs projector access')).toBeInTheDocument();
    expect(screen.getByText('View Invoice')).toBeInTheDocument();
    expect(screen.queryByText('Open booking')).not.toBeInTheDocument();
  });
});
