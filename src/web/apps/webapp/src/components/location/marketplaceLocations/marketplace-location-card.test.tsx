import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MarketplaceLocationCard from './marketplace-location-card';

const pushMock = vi.fn();
const useFragmentMock = vi.fn();

const queryFragmentData = {
  me: {
    favouriteLocations: [],
  },
};

const locationFragmentData = {
  id: 'location-1',
  name: 'Harbour Workspace',
  extraMetadata: {
    areaRange: {
      fromInSqm: 180,
      toInSqm: 180,
    },
    peopleCapacity: {
      from: 20,
      to: 20,
    },
  },
  physicalAddress: {
    multilinesFormattedAddress: '10 Main St\nAuckland',
  },
  featureImages: [],
  organization: {
    type: {
      type: 'MARKETPLACE',
      name: 'Marketplace',
    },
    spacesPublicBookingAvailability: null,
  },
};

const partialLocationFragmentData = {
  ...locationFragmentData,
  extraMetadata: null,
  physicalAddress: null,
  featureImages: [],
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

vi.mock('@workos-inc/authkit-nextjs/components', () => ({
  useAuth: () => ({ user: null, loading: false }),
}));

vi.mock('@/components/links', () => ({
  getMarketplaceLocationLink: () => '/marketplace/locations/location-1',
  getSignInLink: () => '/signin',
}));

vi.mock('@/components/icons', () => ({
  AreaIcon: () => <span>area-icon</span>,
  CloseIcon: () => <span>close-icon</span>,
  FavouriteIcon: () => <span>fav-icon</span>,
  LocationIcon: () => <span>location-icon</span>,
  NotFavouriteIcon: () => <span>not-fav-icon</span>,
  PersonIcon: () => <span>person-icon</span>,
  ShareIcon: () => <span>share-icon</span>,
}));

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useMutation: () => [vi.fn()],
}));

describe('MarketplaceLocationCard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    pushMock.mockReset();
    useFragmentMock.mockImplementation((query: string) => {
      if (query.includes('fragment marketplaceLocationCard_query')) {
        return queryFragmentData;
      }

      return locationFragmentData;
    });
  });

  it('renders the compact marketplace location card with the fallback icon when no feature image exists', () => {
    render(<MarketplaceLocationCard rootDataRelay={{} as never} locationDetailsRelay={{} as never} onReloadRequired={vi.fn()} />);

    expect(screen.getByText('Harbour Workspace')).toBeInTheDocument();
    expect(screen.getByText('10 Main St, Auckland')).toBeInTheDocument();
    expect(screen.getByText('20 People')).toBeInTheDocument();
    expect(screen.getByText('180 m2')).toBeInTheDocument();
    expect(screen.getByText('location-icon')).toBeInTheDocument();
  });

  it('uses the aggregate marketplace location link as the purchase entry point', () => {
    render(<MarketplaceLocationCard rootDataRelay={{} as never} locationDetailsRelay={{} as never} onReloadRequired={vi.fn()} />);

    expect(screen.getByRole('link')).toHaveAttribute('href', '/marketplace/locations/location-1');
  });

  it('handles partial customer-facing location data without placeholder noise', () => {
    useFragmentMock.mockImplementation((query: string) => {
      if (query.includes('fragment marketplaceLocationCard_query')) {
        return queryFragmentData;
      }

      return partialLocationFragmentData;
    });

    render(<MarketplaceLocationCard rootDataRelay={{} as never} locationDetailsRelay={{} as never} onReloadRequired={vi.fn()} />);

    expect(screen.getByText('Harbour Workspace')).toBeInTheDocument();
    expect(screen.getByText('location-icon')).toBeInTheDocument();
    expect(screen.queryByText(/assigned later/i)).not.toBeInTheDocument();
    expect(screen.queryByText(/not available/i)).not.toBeInTheDocument();
  });
});
