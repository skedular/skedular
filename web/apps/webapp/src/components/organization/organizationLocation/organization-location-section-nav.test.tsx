import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationLocationSectionNav from './organization-location-section-nav';

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
  getOrganizationBookingsBaseLink: () => '/organizations/acme/bookings?locationId=location-1',
  getOrganizationLocationSetupBaseLink: () => '/organizations/acme/locations/location-1?section=setup',
  getOrganizationLocationPhysicalAddressSetupBaseLink: () => '/organizations/acme/locations/location-1?section=physical-address-setup',
  getOrganizationLocationOpeningHoursBaseLink: () => '/organizations/acme/locations/location-1?section=opening-hours',
  getOrganizationLocationFloorPlansBaseLink: () => '/organizations/acme/locations/location-1?section=floor-plans',
  getOrganizationLocationManageResourcesBaseLink: () => '/organizations/acme/locations/location-1?section=manage-resources',
  getOrganizationLocationManageLocationBaseLink: () => '/organizations/acme/locations/location-1?section=manage-location',
}));

describe('OrganizationLocationSectionNav', () => {
  it('renders route-backed section links, highlights the active section, and includes the bookings action', () => {
    render(<OrganizationLocationSectionNav activeSection="manage-resources" organizationCustomDomain="acme" locationId="location-1" stickyTop={64} />);

    const resourcesTab = screen.getByRole('link', { name: 'Resources' });
    const setupTab = screen.getByRole('link', { name: 'Location Setup' });
    const bookingsLink = screen.getByRole('link', { name: 'View location bookings' });

    expect(resourcesTab).toHaveAttribute('href', '/organizations/acme/locations/location-1?section=manage-resources');
    expect(setupTab).toHaveAttribute('href', '/organizations/acme/locations/location-1?section=setup');
    expect(bookingsLink).toHaveAttribute('href', '/organizations/acme/bookings?locationId=location-1');
    expect(resourcesTab.className).toContain('MuiButton-contained');
    expect(setupTab.className).toContain('MuiButton-text');
  });
});
