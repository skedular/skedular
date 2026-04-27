import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationAdminSectionNav from './organization-admin-section-nav';

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
  getOrganizationAdminSetupBaseLink: () => '/organizations/acme/admin?section=setup',
  getOrganizationAdminPhysicalAddressBaseLink: () => '/organizations/acme/admin?section=physical-address-setup',
  getOrganizationAdminBillingAndPaymentBaseLink: () => '/organizations/acme/admin?section=billing-payment-setup',
  getOrganizationAdminSsoSettingsBaseLink: () => '/organizations/acme/admin?section=sso-setup',
  getOrganizationAdminTaxDetailsBaseLink: () => '/organizations/acme/admin?section=tax-details-setup',
  getOrganizationAdminZonesBaseLink: () => '/organizations/acme/admin?section=zones-setup',
  getOrganizationAdminCustomTagsBaseLink: () => '/organizations/acme/admin?section=tags-setup',
  getOrganizationAdminSubscriptionsBaseLink: () => '/organizations/acme/admin?section=subscriptions',
  getOrganizationAdminManageOrganizationBaseLink: () => '/organizations/acme/admin?section=manage-organization',
}));

describe('OrganizationAdminSectionNav', () => {
  it('renders route-backed section links and highlights the active section', () => {
    mockMatchMedia(false);

    render(<OrganizationAdminSectionNav activeSection="subscriptions" organizationCustomDomain="acme" stickyTop={64} />);

    const subscriptionsTab = screen.getByRole('link', { name: 'Subscriptions' });
    const setupTab = screen.getByRole('link', { name: 'Setup' });

    expect(subscriptionsTab).toHaveAttribute('href', '/organizations/acme/admin?section=subscriptions');
    expect(setupTab).toHaveAttribute('href', '/organizations/acme/admin?section=setup');
    expect(subscriptionsTab.className).toContain('MuiButton-contained');
    expect(setupTab.className).toContain('MuiButton-text');
  });

  it('collapses sections into a menu on narrower screens', () => {
    mockMatchMedia(true);

    render(<OrganizationAdminSectionNav activeSection="subscriptions" organizationCustomDomain="acme" stickyTop={64} />);

    fireEvent.click(screen.getByRole('button', { name: 'Section: Subscriptions' }));

    const setupMenuItem = screen.getByRole('menuitem', { name: 'Setup' });
    const subscriptionsMenuItem = screen.getByRole('menuitem', { name: 'Subscriptions' });

    expect(setupMenuItem).toHaveAttribute('href', '/organizations/acme/admin?section=setup');
    expect(subscriptionsMenuItem).toHaveAttribute('href', '/organizations/acme/admin?section=subscriptions');
    expect(subscriptionsMenuItem.className).toContain('Mui-selected');
  });
});
