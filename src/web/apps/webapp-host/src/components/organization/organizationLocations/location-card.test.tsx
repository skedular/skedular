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
  products: [{ id: 'product-1' }, { id: 'product-2' }],
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

vi.mock(import('@skedular/shared'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatform: () => ({ integratedPlatform: 'web' }),
  };
});

vi.mock('@/components/links', () => ({
  getOrganizationBookingsBaseLink: () => '/bookings',
  getOrganizationLocationBaseLink: () => '/locations/location-1',
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
  LocationIcon: () => <span>location-icon</span>,
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

  it('routes editing through location settings without exposing booking resources', () => {
    render(<LocationCard rootDataRelay={{} as never} locationDetailsRelay={{} as never} connectionIds={[]} />);

    expect(screen.getByText('Listing setup')).toBeInTheDocument();
    expect(screen.getByText('2 pricing profiles')).toBeInTheDocument();
    expect(screen.getByText('Pricing and booking settings are edited inside the location settings page.')).toBeInTheDocument();
    expect(screen.getByText('Address')).toBeInTheDocument();
    expect(screen.queryByText('Shared With')).not.toBeInTheDocument();
    expect(screen.queryByRole('link', { name: 'Manage location' })).not.toBeInTheDocument();
  });

  it('does not expose floor-plan controls', () => {
    useFragmentMock.mockImplementation((fragment: string) => {
      if (fragment.includes('fragment locationCard_query')) {
        return rootFragmentData;
      }

      if (fragment.includes('fragment locationCard_LocationDetails')) {
        return {
          ...locationFragmentData,
        };
      }

      return undefined;
    });

    render(<LocationCard rootDataRelay={{} as never} locationDetailsRelay={{} as never} connectionIds={[]} />);

    expect(screen.queryByRole('button', { name: 'View floor plan' })).not.toBeInTheDocument();
  });
});
