import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import CoWorkingSubdomain from './co-working-subdomain';

describe('CoWorkingSubdomain', () => {
  it('marks the co-working customer-facing subdomain entry point', () => {
    render(
      <CoWorkingSubdomain>
        <div>Co-working storefront</div>
      </CoWorkingSubdomain>,
    );

    expect(screen.getByText('Co-working storefront').parentElement).toHaveAttribute('data-customer-facing-entry', 'co-working-subdomain');
  });

  it('keeps owner-specific storefront content wrapped without aggregate discovery content', () => {
    render(
      <CoWorkingSubdomain>
        <div>Owner storefront products</div>
      </CoWorkingSubdomain>,
    );

    expect(screen.getByText('Owner storefront products')).toBeInTheDocument();
    expect(screen.queryByText(/aggregate marketplace/i)).not.toBeInTheDocument();
  });
});
