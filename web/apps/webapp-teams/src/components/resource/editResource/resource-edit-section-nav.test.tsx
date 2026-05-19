import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import ResourceEditSectionNav from './resource-edit-section-nav';

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
    useIntegratedPlatrform: () => ({ integratedPlatrform: 'web' }),
  };
});

vi.mock('@/components/links', () => ({
  getOrganizationLocationResourceSetupBaseLink: () => '/organizations/acme/locations/location-1/resources/resource-1?section=setup',
  getOrganizationLocationResourceOpeningHoursBaseLink: () => '/organizations/acme/locations/location-1/resources/resource-1?section=opening-hours',
}));

describe('ResourceEditSectionNav', () => {
  it('renders route-backed section links on desktop', () => {
    mockMatchMedia(false);

    render(<ResourceEditSectionNav activeSection="opening-hours" organizationCustomDomain="acme" locationId="location-1" resourceId="resource-1" stickyTop={64} />);

    const setupTab = screen.getByRole('link', { name: 'Resource Setup' });
    const openingHoursTab = screen.getByRole('link', { name: 'Opening Hours' });

    expect(setupTab).toHaveAttribute('href', '/organizations/acme/locations/location-1/resources/resource-1?section=setup');
    expect(openingHoursTab).toHaveAttribute('href', '/organizations/acme/locations/location-1/resources/resource-1?section=opening-hours');
    expect(openingHoursTab.className).toContain('MuiButton-contained');
    expect(setupTab.className).toContain('MuiButton-text');
  });

  it('collapses into a menu on narrower screens', () => {
    mockMatchMedia(true);

    render(<ResourceEditSectionNav activeSection="opening-hours" organizationCustomDomain="acme" locationId="location-1" resourceId="resource-1" stickyTop={64} />);

    fireEvent.click(screen.getByRole('button', { name: 'Section: Opening Hours' }));

    const setupMenuItem = screen.getByRole('menuitem', { name: 'Resource Setup' });
    const openingHoursMenuItem = screen.getByRole('menuitem', { name: 'Opening Hours' });

    expect(setupMenuItem).toHaveAttribute('href', '/organizations/acme/locations/location-1/resources/resource-1?section=setup');
    expect(openingHoursMenuItem).toHaveAttribute('href', '/organizations/acme/locations/location-1/resources/resource-1?section=opening-hours');
    expect(openingHoursMenuItem.className).toContain('Mui-selected');
  });
});
