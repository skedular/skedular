import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationLocationSectionNav from './organization-location-section-nav';

const mockMatchMedia = (matches: boolean) => {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: vi.fn().mockImplementation((query: string) => ({
      matches,
      media: query,
      onchange: null,
      addListener: vi.fn(),
      removeListener: vi.fn(),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    })),
  });
};

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
  getOrganizationBookingsBaseLink: () => '/organizations/acme?locationId=location-1',
  getOrganizationLocationSetupBaseLink: () => '/organizations/acme/locations/location-1?section=setup',
  getOrganizationLocationPhysicalAddressSetupBaseLink: () => '/organizations/acme/locations/location-1?section=physical-address-setup',
  getOrganizationLocationOpeningHoursBaseLink: () => '/organizations/acme/locations/location-1?section=opening-hours',
  getOrganizationLocationRestrictedInformationBaseLink: () => '/organizations/acme/locations/location-1?section=restricted-information',
  getOrganizationLocationManageLocationBaseLink: () => '/organizations/acme/locations/location-1?section=manage-location',
}));

describe('OrganizationLocationSectionNav', () => {
  it('renders route-backed section links, highlights the active section, and includes the bookings action', () => {
    mockMatchMedia(false);

    render(<OrganizationLocationSectionNav activeSection="opening-hours" organizationCustomDomain="acme" locationId="location-1" stickyTop={64} />);

    const openingHoursTab = screen.getByRole('link', { name: 'Opening Hours' });
    const setupTab = screen.getByRole('link', { name: 'Location Setup' });
    const bookingsLink = screen.getByRole('link', { name: 'View location bookings' });

    expect(openingHoursTab).toHaveAttribute('href', '/organizations/acme/locations/location-1?section=opening-hours');
    expect(setupTab).toHaveAttribute('href', '/organizations/acme/locations/location-1?section=setup');
    expect(bookingsLink).toHaveAttribute('href', '/organizations/acme?locationId=location-1');
    expect(openingHoursTab.className).toContain('MuiButton-contained');
    expect(setupTab.className).toContain('MuiButton-text');
  });

  it('collapses sections into a menu on narrower screens', () => {
    mockMatchMedia(true);

    render(<OrganizationLocationSectionNav activeSection="opening-hours" organizationCustomDomain="acme" locationId="location-1" stickyTop={64} />);

    const sectionButton = screen.getByRole('button', { name: 'Section: Opening Hours' });
    const bookingsLink = screen.getByRole('link', { name: 'View location bookings' });

    expect(bookingsLink).toHaveAttribute('href', '/organizations/acme?locationId=location-1');

    fireEvent.click(sectionButton);

    const setupMenuItem = screen.getByRole('menuitem', { name: 'Location Setup' });
    const openingHoursMenuItem = screen.getByRole('menuitem', { name: 'Opening Hours' });

    expect(setupMenuItem).toHaveAttribute('href', '/organizations/acme/locations/location-1?section=setup');
    expect(openingHoursMenuItem).toHaveAttribute('href', '/organizations/acme/locations/location-1?section=opening-hours');
    expect(openingHoursMenuItem.className).toContain('Mui-selected');
  });
});
