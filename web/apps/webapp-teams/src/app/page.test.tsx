import { render, screen } from '@testing-library/react';
import { useKnownParams } from '@skedular/shared';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from './page';

vi.mock('@skedular/shared', async () => {
  const actual = await vi.importActual<typeof import('@skedular/shared')>('@skedular/shared');

  return {
    ...actual,
    useKnownParams: vi.fn(() => ({ isCustomDomain: false })),
  };
});

vi.mock('@/rootPages/page', () => ({
  default: () => <div>Public discovery root</div>,
}));

vi.mock('@/rootPages/marketplace/page', () => ({
  default: () => <div>Customer-facing subdomain</div>,
}));

describe('WebApp root page foundation', () => {
  beforeEach(() => {
    sessionStorage.clear();
    vi.mocked(useKnownParams).mockReturnValue({ isCustomDomain: false });
  });

  it('keeps the root URL focused on public marketplace discovery', () => {
    render(<RootPage />);

    expect(screen.getByText('Public discovery root')).toBeInTheDocument();
    expect(screen.getByText('Public discovery root').parentElement).toHaveAttribute('data-product-app', 'webapp');
    expect(screen.getByText('Public discovery root').parentElement).toHaveAttribute('data-review-scope', 'public-discovery');
  });

  it('keeps custom domains on the customer-facing co-working storefront until private subdomain detection is wired', () => {
    vi.mocked(useKnownParams).mockReturnValue({ isCustomDomain: true });

    render(<RootPage />);

    expect(screen.getByText('Customer-facing subdomain')).toBeInTheDocument();
    expect(screen.getByText('Customer-facing subdomain').parentElement).toHaveAttribute('data-customer-facing-entry', 'co-working-subdomain');
    expect(screen.getByText('Customer-facing subdomain').parentElement?.parentElement).toHaveAttribute('data-review-scope', 'co-working-subdomain');
  });
});
