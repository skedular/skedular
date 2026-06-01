import useKnownParams from '@/hooks/use-known-params';
import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from './page';

vi.mock('@skedular/shared', async () => {
  const actual = await vi.importActual<typeof import('@skedular/shared')>('@skedular/shared');

  return {
    ...actual,
  };
});

vi.mock('@/hooks/use-known-params', () => ({
  default: vi.fn(() => ({ isCustomDomain: false })),
}));

vi.mock('@/rootPages/page', () => ({
  default: () => <div>Public discovery root</div>,
}));

vi.mock('@/rootPages/marketplace/page', () => ({
  default: () => <div>Customer-facing subdomain</div>,
}));

const knownParams = {
  bookingId: '',
  customerId: '',
  floorPlanId: '',
  isCustomDomain: false,
  locationId: '',
  organizationBankAccountId: '',
  organizationCustomDomain: '',
  organizationStripeConnectAccountId: '',
  productId: '',
  resourceId: '',
  subscriptionId: '',
  teamId: '',
};

describe('WebApp root page foundation', () => {
  beforeEach(() => {
    sessionStorage.clear();
    window.history.pushState({}, '', '/');
    vi.mocked(useKnownParams).mockReturnValue(knownParams);
  });

  it('keeps the root URL focused on public marketplace discovery', () => {
    render(<RootPage />);

    expect(screen.getByText('Public discovery root')).toBeInTheDocument();
    expect(screen.queryByText(/admin/i)).not.toBeInTheDocument();
    expect(screen.getByText('Public discovery root').closest('[data-product-app]')).toHaveAttribute('data-product-app', 'webapp');
    expect(screen.getByText('Public discovery root').closest('[data-review-scope]')).toHaveAttribute('data-review-scope', 'public-discovery');
  });

  it('keeps custom domains on the customer-facing co-working storefront until private subdomain detection is wired', () => {
    vi.mocked(useKnownParams).mockReturnValue({ ...knownParams, isCustomDomain: true });

    render(<RootPage />);

    expect(screen.getByText('Customer-facing subdomain')).toBeInTheDocument();
    expect(screen.getByText('Customer-facing subdomain').parentElement).toHaveAttribute('data-customer-facing-entry', 'co-working-subdomain');
    expect(screen.getByText('Customer-facing subdomain').closest('[data-review-scope]')).toHaveAttribute('data-review-scope', 'co-working-subdomain');
  });

  it('does not redirect the root webapp URL during feature entry resolution', () => {
    sessionStorage.setItem('postSignOutReturnTo', '/marketplace/bookings');
    window.history.pushState({}, '', '/current-path');

    render(<RootPage />);

    expect(window.location.pathname).toBe('/current-path');
  });
});
