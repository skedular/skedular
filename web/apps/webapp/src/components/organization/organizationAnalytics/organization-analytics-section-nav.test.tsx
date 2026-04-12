import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationAnalyticsSectionNav from './organization-analytics-section-nav';

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

vi.mock(import('@/libs/providers'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatrform: () => ({ integratedPlatrform: 'web' }),
  };
});

vi.mock('@/components/links', () => ({
  getOrganizationAnalyticsBaseLink: () => '/organizations/acme/analytics?section=organization',
  getOrganizationLocationsAnalyticsLocationsBaseLink: () => '/organizations/acme/analytics?section=locations',
}));

describe('OrganizationAnalyticsSectionNav', () => {
  it('renders route-backed section links', () => {
    mockMatchMedia(false);

    render(<OrganizationAnalyticsSectionNav activeSection="organization" organizationCustomDomain="acme" stickyTop={64} />);

    const organizationTab = screen.getByRole('link', { name: 'Organization' });
    const locationsTab = screen.getByRole('link', { name: 'Locations' });

    expect(organizationTab).toHaveAttribute('href', '/organizations/acme/analytics?section=organization');
    expect(locationsTab).toHaveAttribute('href', '/organizations/acme/analytics?section=locations');
    expect(organizationTab.className).toContain('MuiButton-contained');
    expect(locationsTab.className).toContain('MuiButton-text');
  });

  it('collapses sections into a menu on narrower screens', () => {
    mockMatchMedia(true);

    render(<OrganizationAnalyticsSectionNav activeSection="locations" organizationCustomDomain="acme" stickyTop={64} />);

    fireEvent.click(screen.getByRole('button', { name: 'Section: Locations' }));

    const organizationMenuItem = screen.getByRole('menuitem', { name: 'Organization' });
    const locationsMenuItem = screen.getByRole('menuitem', { name: 'Locations' });

    expect(organizationMenuItem).toHaveAttribute('href', '/organizations/acme/analytics?section=organization');
    expect(locationsMenuItem).toHaveAttribute('href', '/organizations/acme/analytics?section=locations');
    expect(locationsMenuItem.className).toContain('Mui-selected');
  });
});
