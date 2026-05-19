import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import OrganizationProductsPageShell from './organization-products-page-shell';

describe('OrganizationProductsPageShell', () => {
  it('renders the page header and empty state when there are no products', () => {
    render(<OrganizationProductsPageShell actions={<button type="button">Add Product</button>} isEmpty />);

    expect(screen.getByText('Products')).toBeInTheDocument();
    expect(screen.getByText('Create and manage the bookable offers customers can purchase.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Add Product' })).toBeInTheDocument();
    expect(screen.getByText('No products yet')).toBeInTheDocument();
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
    expect(screen.queryByText('No products yet')).not.toBeInTheDocument();
  });
});
