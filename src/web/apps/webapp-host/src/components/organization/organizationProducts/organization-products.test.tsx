import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import OrganizationProductsPageShell from './organization-products-page-shell';

describe('OrganizationProductsPageShell', () => {
  it('renders the page header and empty state when there are no products', () => {
    render(<OrganizationProductsPageShell isEmpty />);

    expect(screen.getByText('Products')).toBeInTheDocument();
    expect(screen.getByText('Set pricing and policies for each place. A private draft is created automatically when you add a location.')).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /product/i })).not.toBeInTheDocument();
    expect(screen.getByText(/No products yet/)).toBeInTheDocument();
  });

  it('renders product content instead of the empty state when products exist', () => {
    render(
      <OrganizationProductsPageShell isEmpty={false}>
        <div>Hot Desk</div>
        <div>Meeting Room</div>
      </OrganizationProductsPageShell>,
    );

    expect(screen.getByText('Hot Desk')).toBeInTheDocument();
    expect(screen.getByText('Meeting Room')).toBeInTheDocument();
    expect(screen.queryByText(/No products yet/)).not.toBeInTheDocument();
  });
});
