import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from './page';

vi.mock('@/rootPages/page', () => ({
  default: () => <div>Spaces root</div>,
}));

describe('WebApp root page foundation', () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it('keeps the root URL focused on the Spaces app entry', () => {
    render(<RootPage />);

    expect(screen.getByText('Spaces root')).toBeInTheDocument();
    expect(screen.getByText('Spaces root').closest('[data-product-app]')).toHaveAttribute('data-product-app', 'webapp-spaces');
    expect(screen.getByText('Spaces root').closest('[data-review-scope]')).toHaveAttribute('data-review-scope', 'spaces-entry');
  });

  it('does not switch the Spaces root into a customer-facing storefront', () => {
    render(<RootPage />);

    expect(screen.queryByText('Customer-facing subdomain')).not.toBeInTheDocument();
  });
});
