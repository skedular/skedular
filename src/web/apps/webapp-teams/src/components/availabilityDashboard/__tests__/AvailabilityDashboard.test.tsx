import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import AvailabilityDashboard from '../AvailabilityDashboard';

const useFragmentMock = vi.fn();
const useSubscriptionMock = vi.fn();

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useSubscription: (...args: unknown[]) => useSubscriptionMock(...args),
}));

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: vi.fn() }),
  useSearchParams: () => new URLSearchParams(),
  useParams: () => ({ organizationCustomDomain: 'test-org' }),
}));

vi.mock('@skedular/shared', () => ({
  useKnownParams: () => ({ organizationCustomDomain: 'test-org' }),
  startOfDay: (d: Date) => d,
  toShortDateWithoutWeekDay: (d: Date) => (d ? d.toString().slice(0, 10) : '2026-01-15'),
  endOfDay: (d: Date) => d,
  defaultPadding: 2,
}));

// Stub heavy sub-components to keep the test focused on AvailabilityDashboard
vi.mock('../AvailabilityFilterBar', () => ({
  default: () => <div role="search" />,
}));

vi.mock('../ResourceDayViewList', () => ({
  default: () => <div data-testid="resource-list" />,
}));

vi.mock('next/link', () => ({
  default: ({ children, href }: { children: React.ReactNode; href: string }) => <a href={href}>{children}</a>,
}));

const defaultData = {
  subscriptionKey: 'sk:org-1:loc-1:2026-01-15',
  items: [
    {
      resourceId: 'resource-1',
      resourceName: 'Desk A1',
      resourceType: 'RESOURCE_DESK',
      locationId: 'location-1',
      locationName: 'HQ',
      floorId: null,
      floorName: null,
      zoneId: null,
      zoneName: null,
      date: '2026-01-15',
      status: 'AVAILABLE',
      openingFrom: '08:00',
      openingUntil: '18:00',
      totalOpeningMinutes: 600,
      bookedMinutes: 0,
      bookingWindows: [],
    },
  ],
};

const defaultFilters = {
  date: '2026-01-15',
  locationIds: [],
  statuses: [],
};

describe('AvailabilityDashboard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    useSubscriptionMock.mockReset();

    useFragmentMock.mockImplementation((query: string) => {
      if (typeof query === 'string' && query.trimStart().startsWith('fragment ResourceDayViewCard_resourceDayView')) {
        return defaultData.items[0];
      }
      return defaultData;
    });
  });

  it('renders the page header', () => {
    render(
      <AvailabilityDashboard
        dataRef={{} as never}
        locationsRef={{} as never}
        statusesRef={[] as never}
        filters={defaultFilters}
        onFiltersChange={vi.fn()}
        organizationCustomDomain="test-org"
      />,
    );
    expect(screen.getByText('Availability Dashboard')).toBeInTheDocument();
  });

  it('renders the filter bar', () => {
    render(
      <AvailabilityDashboard
        dataRef={{} as never}
        locationsRef={{} as never}
        statusesRef={[] as never}
        filters={defaultFilters}
        onFiltersChange={vi.fn()}
        organizationCustomDomain="test-org"
      />,
    );
    expect(screen.getByRole('search')).toBeInTheDocument();
  });

  it('does not show reconnection alert when subscription is healthy', () => {
    render(
      <AvailabilityDashboard
        dataRef={{} as never}
        locationsRef={{} as never}
        statusesRef={[] as never}
        filters={defaultFilters}
        onFiltersChange={vi.fn()}
        organizationCustomDomain="test-org"
      />,
    );
    expect(screen.queryByText(/live updates paused/i)).not.toBeInTheDocument();
  });

  it('initialises subscription with the subscription key from query result', () => {
    render(
      <AvailabilityDashboard
        dataRef={{} as never}
        locationsRef={{} as never}
        statusesRef={[] as never}
        filters={defaultFilters}
        onFiltersChange={vi.fn()}
        organizationCustomDomain="test-org"
      />,
    );
    expect(useSubscriptionMock).toHaveBeenCalled();
    const subscriptionConfig = useSubscriptionMock.mock.calls[0][0];
    expect(subscriptionConfig.variables.subscriptionKey).toBe('sk:org-1:loc-1:2026-01-15');
  });
});
