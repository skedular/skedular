import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { PageHeaderPanel, PageSectionCard } from '@skedular/ui';

describe('page layout primitives', () => {
  it('renders the page header panel title, description, actions, and supporting content', () => {
    render(
      <PageHeaderPanel title="Products" description="Manage bookable offers." actions={<button type="button">Add Product</button>}>
        Supporting copy
      </PageHeaderPanel>,
    );

    expect(screen.getByText('Products')).toBeInTheDocument();
    expect(screen.getByText('Manage bookable offers.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Add Product' })).toBeInTheDocument();
    expect(screen.getByText('Supporting copy')).toBeInTheDocument();
  });

  it('renders a section card with title, description, and child content', () => {
    render(
      <PageSectionCard title="Catalog" description="Review active products.">
        <div>Card content</div>
      </PageSectionCard>,
    );

    expect(screen.getByText('Catalog')).toBeInTheDocument();
    expect(screen.getByText('Review active products.')).toBeInTheDocument();
    expect(screen.getByText('Card content')).toBeInTheDocument();
  });
});
