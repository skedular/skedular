import { render, screen } from '@testing-library/react';
import { createElement } from 'react';
import { describe, expect, it, vi } from 'vitest';
import PrivateOrganizationSubdomain from './private-organization-subdomain';

vi.mock('next/image', () => ({
  default: ({ alt, src }: { alt: string; src: string }) => createElement('img', { alt, src }),
}));

describe('PrivateOrganizationSubdomain', () => {
  it('renders the private organization customer-facing shell', () => {
    render(<PrivateOrganizationSubdomain />);

    expect(screen.getAllByText('Private organization')).toHaveLength(2);
    expect(screen.getByText('Customer-facing private organization access.')).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Switch app' })).not.toBeInTheDocument();
    expect(screen.getByText('Customer-facing private organization access.').closest('[data-customer-facing-entry]')).toHaveAttribute(
      'data-customer-facing-entry',
      'private-organisation-subdomain',
    );
  });
});
