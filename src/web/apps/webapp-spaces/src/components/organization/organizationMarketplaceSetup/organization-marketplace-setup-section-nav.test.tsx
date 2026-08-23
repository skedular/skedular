import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationMarketplaceSetupSectionNav from './organization-marketplace-setup-section-nav';

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
  getOrganizationMarketplaceSetupMarketplaceListingBaseLink: () => '/organizations/acme/admin?tab=profile&section=marketplace-listing',
  getOrganizationMarketplaceSetupBillingCycleBaseLink: () => '/organizations/acme/admin?tab=profile&section=billing-cadence',
  getOrganizationMarketplaceSetupXeroBaseLink: () => '/organizations/acme/integrations?tab=xero-setup',
  getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink: () => '/organizations/acme/integrations?tab=stripe-connect-accounts-setup',
  getOrganizationMarketplaceSetupBankAccountsBaseLink: () => '/organizations/acme/admin?tab=bank-accounts',
  getOrganizationMarketplaceSetupProductTagsBaseLink: () => '/organizations/acme/admin?section=product-tags-setup',
}));

describe('OrganizationMarketplaceSetupSectionNav', () => {
  it('renders route-backed section links and highlights the active section', () => {
    mockMatchMedia(false);

    render(<OrganizationMarketplaceSetupSectionNav activeSection="xero-setup" organizationCustomDomain="acme" stickyTop={64} />);

    const xeroTab = screen.getByRole('link', { name: 'Xero' });
    const listingTab = screen.getByRole('link', { name: 'Listing' });

    expect(xeroTab).toHaveAttribute('href', '/organizations/acme/integrations?tab=xero-setup');
    expect(listingTab).toHaveAttribute('href', '/organizations/acme/admin?tab=profile&section=marketplace-listing');
    expect(xeroTab.className).toContain('MuiButton-contained');
    expect(listingTab.className).toContain('MuiButton-text');
  });

  it('collapses sections into a menu on narrower screens', () => {
    mockMatchMedia(true);

    render(<OrganizationMarketplaceSetupSectionNav activeSection="xero-setup" organizationCustomDomain="acme" stickyTop={64} />);

    fireEvent.click(screen.getByRole('button', { name: 'Section: Xero' }));

    const listingMenuItem = screen.getByRole('menuitem', { name: 'Listing' });
    const xeroMenuItem = screen.getByRole('menuitem', { name: 'Xero' });

    expect(listingMenuItem).toHaveAttribute('href', '/organizations/acme/admin?tab=profile&section=marketplace-listing');
    expect(xeroMenuItem).toHaveAttribute('href', '/organizations/acme/integrations?tab=xero-setup');
    expect(xeroMenuItem.className).toContain('Mui-selected');
  });
});
