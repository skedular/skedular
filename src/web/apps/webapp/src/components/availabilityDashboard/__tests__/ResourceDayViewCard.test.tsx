import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import ResourceDayViewCard from '../ResourceDayViewCard';

const useFragmentMock = vi.fn();

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
}));

vi.mock('@/hooks/use-known-params', () => ({
  default: () => ({ organizationCustomDomain: 'test-org' }),
}));

vi.mock('next/link', () => ({
  default: ({ children, href }: { children: React.ReactNode; href: string }) => <a href={href}>{children}</a>,
}));

const makeResourceDayView = (overrides = {}) => ({
  resourceId: 'resource-1',
  resourceName: 'Meeting Room Alpha',
  resourceType: 'RESOURCE_ROOM',
  locationId: 'location-1',
  locationName: 'HQ Building',
  floorId: 'floor-1',
  floorName: 'Ground Floor',
  zoneId: 'zone-1',
  zoneName: 'North Wing',
  date: '2026-01-15',
  status: 'AVAILABLE',
  openingFrom: '08:00',
  openingUntil: '18:00',
  totalOpeningMinutes: 600,
  bookedMinutes: 0,
  bookingWindows: [],
  ...overrides,
});

describe('ResourceDayViewCard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    useFragmentMock.mockReturnValue(makeResourceDayView());
  });

  it('renders the resource name', () => {
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('Meeting Room Alpha')).toBeInTheDocument();
  });

  it('renders the location name', () => {
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText(/HQ Building/)).toBeInTheDocument();
  });

  it('renders status badge for AVAILABLE status', () => {
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('Available')).toBeInTheDocument();
  });

  it('renders status badge for FULLY_BOOKED status', () => {
    useFragmentMock.mockReturnValue(makeResourceDayView({ status: 'FULLY_BOOKED', bookedMinutes: 600 }));
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('Fully Booked')).toBeInTheDocument();
  });

  it('renders status badge for PARTIALLY_BOOKED status', () => {
    useFragmentMock.mockReturnValue(makeResourceDayView({ status: 'PARTIALLY_BOOKED', bookedMinutes: 120 }));
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('Partially Booked')).toBeInTheDocument();
  });

  it('renders status badge for BLOCKED status', () => {
    useFragmentMock.mockReturnValue(makeResourceDayView({ status: 'BLOCKED' }));
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('Blocked')).toBeInTheDocument();
  });

  it('renders booking windows when present', () => {
    useFragmentMock.mockReturnValue(
      makeResourceDayView({
        status: 'PARTIALLY_BOOKED',
        bookedMinutes: 60,
        bookingWindows: [
          {
            bookingId: 'bk-1',
            from: '2026-01-15T09:00:00Z',
            until: '2026-01-15T10:00:00Z',
            isRecurring: false,
            isCheckedIn: false,
            bookedByName: 'Jane Doe',
            notes: null,
          },
        ],
      }),
    );
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('Jane Doe')).toBeInTheDocument();
  });

  it('does not render bookedByName when null (restricted visibility)', () => {
    useFragmentMock.mockReturnValue(
      makeResourceDayView({
        status: 'FULLY_BOOKED',
        bookedMinutes: 600,
        bookingWindows: [
          {
            bookingId: 'bk-2',
            from: '2026-01-15T09:00:00Z',
            until: '2026-01-15T18:00:00Z',
            isRecurring: false,
            isCheckedIn: false,
            bookedByName: null,
            notes: null,
          },
        ],
      }),
    );
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.queryByText(/jane/i)).not.toBeInTheDocument();
  });

  it('renders booked minutes summary', () => {
    useFragmentMock.mockReturnValue(makeResourceDayView({ bookedMinutes: 120, totalOpeningMinutes: 600 }));
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByText('120 / 600 mins booked')).toBeInTheDocument();
  });

  it('has proper aria-label on the card', () => {
    render(<ResourceDayViewCard resourceDayViewRef={{} as never} />);
    expect(screen.getByLabelText('Resource: Meeting Room Alpha')).toBeInTheDocument();
  });
});
