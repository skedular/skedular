import { fireEvent, render, screen } from '@testing-library/react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import GuestStoreFrontLocationsStrip from './guest-store-front-locations-strip';

const pushMock = vi.fn();
const useFragmentMock = vi.fn();

const allDayOpeningHours = {
  closed: false,
  openAllDay: true,
  from: null,
  until: null,
};

const createRootData = (floorPlanCount: number) => ({
  marketplaceLocations: {
    totalCount: 1,
    edges: [
      {
        node: {
          id: 'location-1',
          name: 'Harbour Workspace',
          timezone: 'Pacific/Auckland',
          floorPlanCount,
          physicalAddress: {
            formattedAddress: '10 Main St, Auckland',
          },
          openingHours: {
            weekOpeningHours: {
              monday: allDayOpeningHours,
              tuesday: allDayOpeningHours,
              wednesday: allDayOpeningHours,
              thursday: allDayOpeningHours,
              friday: allDayOpeningHours,
              saturday: allDayOpeningHours,
              sunday: allDayOpeningHours,
            },
          },
        },
      },
    ],
  },
});

const theme = createTheme();

const renderComponent = () =>
  render(
    <ThemeProvider theme={theme}>
      <GuestStoreFrontLocationsStrip rootDataRelay={{} as never} />
    </ThemeProvider>,
  );

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

vi.mock(import('@skedular/shared'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatform: () => ({ integratedPlatform: 'web' }),
  };
});

vi.mock('@/components/links', () => ({
  getMarketplaceLocationFloorPlansLink: () => '/marketplace/locations/location-1/floorPlans',
  getMarketplaceLocationLink: () => '/marketplace/locations/location-1',
}));

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
}));

describe('GuestStoreFrontLocationsStrip', () => {
  beforeEach(() => {
    pushMock.mockReset();
    useFragmentMock.mockReset();
  });

  it('hides the view floor plan action when a location has no floor plans', () => {
    useFragmentMock.mockReturnValue(createRootData(0));

    renderComponent();

    expect(screen.getByText('Harbour Workspace')).toBeInTheDocument();
    fireEvent.click(screen.getByRole('button', { name: /Harbour Workspace/ }));

    expect(screen.queryByRole('button', { name: 'Floor plan' })).not.toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Details' })).toBeInTheDocument();
  });

  it('shows the view floor plan action when a location has at least one floor plan', () => {
    useFragmentMock.mockReturnValue(createRootData(1));

    renderComponent();

    fireEvent.click(screen.getByRole('button', { name: /Harbour Workspace/ }));

    expect(screen.getByRole('button', { name: 'Floor plan' })).toBeInTheDocument();
  });
});
