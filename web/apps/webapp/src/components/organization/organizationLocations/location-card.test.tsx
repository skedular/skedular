import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import LocationCard from './location-card';

const pushMock = vi.fn();
const useFragmentMock = vi.fn();

const rootFragmentData = {
  me: {
    preferredLocations: [],
  },
};

const locationFragmentData = {
  id: 'location-1',
  name: 'HQ',
  customTags: [],
  zones: [],
  resources: { totalCount: 4 },
  physicalAddress: { multilinesFormattedAddress: '10 Main St\nMelbourne VIC 3000', latitude: -37.8, longitude: 144.9 },
  featureImages: [],
  canModify: true,
  canDelete: true,
  organization: { customDomain: 'acme' },
  extraMetadata: { contactDetails: { contactPeople: [], contactEmails: [], contactPhones: [] } },
  uniqueClaimCode: 'HQ01',
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
  getOrganizationBookingsBaseLink: () => '/bookings',
  getOrganizationLocationFloorPlansLink: () => '/floor-plans',
  getOrganizationLocationSetupBaseLink: () => '/locations/setup',
}));

vi.mock('@/components/booking/addBooking', () => ({
  NewBookingButton: ({ label }: { label?: string }) => <button type="button">{label ?? 'Add Booking'}</button>,
}));

vi.mock('@/components/moreActionsMenu', () => ({
  MoreActionsMenu: () => null,
  moreActionsMenuAllOptions: {
    EditLocation: [{ id: 'EditLocation', label: 'Edit Location' }],
    DeleteLocation: [{ id: 'DeleteLocation', label: 'Delete Location' }],
    ViewLocationBookings: [{ id: 'ViewLocationBookings', label: 'View Location Bookings' }],
    SetAsPreferredLocation: [{ id: 'SetAsPreferredLocation', label: 'Set As Preferred Location' }],
    RemoveAsPreferredLocation: [{ id: 'RemoveAsPreferredLocation', label: 'Remove As Preferred Location' }],
  },
  MoreActionsMenuOptionType: {
    EditLocation: 'EditLocation',
    DeleteLocation: 'DeleteLocation',
    ViewLocationBookings: 'ViewLocationBookings',
    SetAsPreferredLocation: 'SetAsPreferredLocation',
    RemoveAsPreferredLocation: 'RemoveAsPreferredLocation',
  },
}));

vi.mock('@/components/icons', () => ({
  EllipseMenuIcon: () => <span>menu</span>,
  FloorPlanIcon: () => <span>floor-plan</span>,
  LocationIcon: () => <span>location-icon</span>,
  ResourceIcon: () => <span>resource-icon</span>,
}));

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useMutation: () => [vi.fn()],
}));

describe('LocationCard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    pushMock.mockReset();
    useFragmentMock.mockImplementation((fragment: string) => {
      if (fragment.includes('fragment locationCard_query')) {
        return rootFragmentData;
      }

      if (fragment.includes('fragment locationCard_LocationDetails')) {
        return locationFragmentData;
      }

      return undefined;
    });
  });

  it('does not render the shared-with section and still shows core booking details', () => {
    render(
      <LocationCard
        rootDataRelay={{} as never}
        locationDetailsRelay={{} as never}
        onReloadRequired={vi.fn()}
        organizationCustomDomain="acme"
        connectionIds={[]}
        availableResourcesCount={2}
        availablePercentage={50}
        defaultDate={{} as never}
      />,
    );

    expect(screen.getByText('Availability')).toBeInTheDocument();
    expect(screen.getByText('2 resources available today')).toBeInTheDocument();
    expect(screen.getByText('Address')).toBeInTheDocument();
    expect(screen.queryByText('Shared With')).not.toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Book Now' })).toBeInTheDocument();
  });
});
