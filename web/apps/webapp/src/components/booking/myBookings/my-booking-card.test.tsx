import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MyBookingCard from './my-booking-card';

const pushMock = vi.fn();
const useFragmentMock = vi.fn();

const bookingFragmentData = {
  id: 'booking-1',
  from: '2026-04-12T09:00:00.000Z',
  until: '2026-04-12T11:00:00.000Z',
  notes: 'Bring laptop',
  channel: { channel: 'MARKETPLACE' },
  involvedCustomers: [],
  involvedLocations: [{ uniqueId: 'location-1', name: 'Level 2 Hot Desk' }],
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
    isPaymentRequired: true,
    paymentStatus: { type: 'PAID', name: 'Paid' },
    invoiceUrl: 'https://example.com/invoice.pdf',
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
  },
  MoreActionsMenuOptionType: {
    EditBooking: 'EditBooking',
    DeleteBooking: 'DeleteBooking',
  },
}));

vi.mock('@/components/icons', () => ({
  CalendarIcon: () => <span>calendar-icon</span>,
  EllipseMenuIcon: () => <span>menu</span>,
  NotesIcon: () => <span>notes-icon</span>,
  PaymentStatusIcon: () => <span>payment-icon</span>,
  PdfIcon: () => <span>pdf-icon</span>,
  TeamIcon: () => <span>team-icon</span>,
}));

vi.mock('@/components/resource/resources', () => ({
  default: ({ resources }: { resources: Array<{ name: string }> }) => <div>{resources.map((item) => item.name).join(', ')}</div>,
}));

vi.mock('@/components/customTag', () => ({
  CustomTags: ({ customTags }: { customTags: Array<{ name: string }> }) => <div>{customTags.map((item) => item.name).join(', ')}</div>,
}));

vi.mock('@/components/zone', () => ({
  Zones: ({ zones }: { zones: Array<{ name: string }> }) => <div>{zones.map((item) => item.name).join(', ')}</div>,
}));

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useMutation: () => [vi.fn()],
}));

describe('MyBookingCard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    pushMock.mockReset();
    useFragmentMock.mockImplementation(() => bookingFragmentData);
  });

  it('renders the compact booking card layout', () => {
    render(
      <MyBookingCard
        bookingDetailsRelay={{} as never}
        organizationCustomDomain="acme"
        connectionIds={[]}
        otherTeammates={[
          {
            id: 'customer-2',
            name: 'Alex',
            photoUrl: null,
          },
        ]}
      />,
    );

    expect(screen.getByText('Level 2 Hot Desk')).toBeInTheDocument();
    expect(screen.getByText('Operations')).toBeInTheDocument();
    expect(screen.getByText('Paid')).toBeInTheDocument();
    expect(screen.getByText('Booking details')).toBeInTheDocument();
    expect(screen.getByText('Desk A1')).toBeInTheDocument();
    expect(screen.getByText('Monitor')).toBeInTheDocument();
    expect(screen.getByText('North Wing')).toBeInTheDocument();
    expect(screen.getByText('Bring laptop')).toBeInTheDocument();
    expect(screen.queryByText('Open booking')).not.toBeInTheDocument();
    expect(screen.queryByText('Marketplace booking')).not.toBeInTheDocument();
  });
});
