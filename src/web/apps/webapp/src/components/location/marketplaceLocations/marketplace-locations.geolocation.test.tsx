import { render, waitFor } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';

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
  useMediaQuery: vi.fn(() => false),
  useTheme: vi.fn(() => ({ mixins: { toolbar: { minHeight: 56 } }, breakpoints: { down: vi.fn() } })),
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

    render(<MarketplaceLocations rootDataRelay={{} as never} rootDataLocationsRelay={{} as never} onReloadRequired={vi.fn()} />);

    await waitFor(
      () => {
        expect(vi.mocked(fetch)).toHaveBeenCalledWith('/api/geolocation');
      },
      { timeout: 3000 },
    );
  });
});
