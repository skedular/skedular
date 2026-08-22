import { render, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import type { marketplaceLocations_locations_query$key } from '@/queries/__generated__/marketplaceLocations_locations_query.graphql';

vi.mock('@/libs/logging', () => ({ default: { info: vi.fn(), warn: vi.fn(), error: vi.fn() } }));
vi.mock('@/libs/logging/aggregate-marketplace-telemetry', () => ({ logAggregateMarketplaceDiscoveryCompleted: vi.fn() }));
vi.mock('@/styles/leaflet/leaflet.css', () => ({}));
vi.mock('react-leaflet-cluster/dist/assets/MarkerCluster.css', () => ({}));
vi.mock('react-leaflet-cluster/dist/assets/MarkerCluster.Default.css', () => ({}));

const mockMapInstance = {
  getBounds: vi.fn(() => ({ getSouthWest: vi.fn(() => ({ lat: 0, lng: 0 })), getNorthEast: vi.fn(() => ({ lat: 0, lng: 0 })) })),
  getZoom: vi.fn(() => 10),
  setView: vi.fn(),
};

vi.mock('leaflet', () => {
  class MockIcon {
    static Default = { mergeOptions: vi.fn() };
  }
  return {
    default: { divIcon: vi.fn(), icon: vi.fn(), Icon: MockIcon },
    Icon: MockIcon,
  };
});

vi.mock('react-leaflet', () => ({
  useMap: vi.fn(() => mockMapInstance),
  useMapEvents: vi.fn(() => null),
  MapContainer: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  Marker: () => null,
  TileLayer: () => null,
  Popup: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

vi.mock('react-leaflet-cluster', () => ({ default: ({ children }: { children: React.ReactNode }) => <div>{children}</div> }));

vi.mock('@skedular/ui', () => ({
  defaultPadding: 2,
  GridContainer: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  StackColumn: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

vi.mock('@mui/material', () => ({
  useMediaQuery: vi.fn((query) => {
    // MUI's breakpoints generate media query strings like '(max-width: 960px)' or '(min-width: 1200px)'
    if (typeof query !== 'string') return false;

    // Parse the query string to determine the result
    if (query.includes('max-width')) {
      // down() breakpoints - returns true if screen width is <= breakpoint value
      if (query.includes('960px')) return true; // md
      if (query.includes('1200px')) return false; // lg
      if (query.includes('1536px')) return false; // xl
      return false;
    }

    if (query.includes('min-width')) {
      // up() breakpoints - returns true if screen width is >= breakpoint value
      if (query.includes('960px')) return false; // md
      if (query.includes('1200px')) return true; // lg
      if (query.includes('1536px')) return true; // xl
      return false;
    }

    return false;
  }),
  useTheme: vi.fn(() => ({
    mixins: { toolbar: { minHeight: 56 } },
    breakpoints: {
      down: (size: string) => {
        const widths: Record<string, string> = { xs: '0px', sm: '600px', md: '960px', lg: '1200px', xl: '1536px' };
        return `(max-width: ${widths[size]})`;
      },
      up: (size: string) => {
        const widths: Record<string, string> = { xs: '0px', sm: '600px', md: '960px', lg: '1200px', xl: '1536px' };
        return `(min-width: ${widths[size]})`;
      },
    },
  })),
}));
vi.mock('@mui/material/Box', () => ({ default: ({ children }: { children: React.ReactNode }) => <div>{children}</div> }));
vi.mock('@mui/material/Grid', () => ({ default: ({ children }: { children: React.ReactNode }) => <div>{children}</div> }));
vi.mock('@mui/material/Pagination', () => ({ default: () => null }));

vi.mock('next/navigation', () => ({
  useSearchParams: vi.fn(() => ({ get: vi.fn(() => null) })),
  useRouter: vi.fn(() => ({ push: vi.fn() })),
  usePathname: vi.fn(() => '/'),
}));

vi.mock('react-relay', () => ({
  graphql: vi.fn(),
  useFragment: vi.fn(() => ({ locations: [], totalCount: 0 })),
  useRefetchableFragment: vi.fn(() => [{ marketplaceLocations: { edges: [], totalCount: 0 } }, vi.fn()]),
}));

vi.mock('react-leaflet', () => ({
  useMap: vi.fn(() => mockMapInstance),
  useMapEvents: vi.fn(() => null),
  MapContainer: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  Marker: () => null,
  TileLayer: () => null,
  Popup: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

vi.mock('./marketplace-location-card', () => ({ default: () => null }));

describe('MarketplaceLocations — geolocation via API route', () => {
  beforeEach(() => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue({ city: 'Auckland', region: 'Auckland', country: 'NZ', lat: '-36.8485', lng: '174.7633' }),
      }),
    );

    Object.defineProperty(globalThis.navigator, 'geolocation', {
      value: undefined,
      configurable: true,
      writable: true,
    });
    // Ensure the property doesn't exist so 'geolocation' in navigator is false
    try {
      delete (globalThis.navigator as unknown as Record<string, unknown>).geolocation;
    } catch {
      /* ignore */
    }
  });

  it('calls fetch("/api/geolocation") when navigator.geolocation is unavailable', async () => {
    const { default: MarketplaceLocations } = await import('./marketplace-locations');

    render(<MarketplaceLocations rootDataLocationsRelay={{} as marketplaceLocations_locations_query$key} onReloadRequired={vi.fn()} />);

    await waitFor(
      () => {
        expect(vi.mocked(fetch)).toHaveBeenCalledWith('/api/geolocation');
      },
      { timeout: 5000 },
    );
  });
});
