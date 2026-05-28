import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import PrivateOrganizationSubdomain from './private-organization-subdomain';

describe('PrivateOrganizationSubdomain', () => {
  it('renders the private organisation customer-facing shell', () => {
    render(<PrivateOrganizationSubdomain />);

    expect(screen.getAllByText('Private organisation')).toHaveLength(2);
    expect(screen.getByText('Customer-facing private organisation access.')).toBeInTheDocument();
    expect(screen.getByText('Customer-facing private organisation access.').closest('[data-customer-facing-entry]')).toHaveAttribute(
      'data-customer-facing-entry',
      'private-organisation-subdomain',
    );
  });
});
