import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import ManagementPageShell from '../management-page-shell';

describe('shared management shell consumption', () => {
  it('renders a configured management page shell for app-owned journeys', () => {
    render(
      <ManagementPageShell
        title="Products"
        description="Create and manage products."
        actions={<button type="button">Add product</button>}
        isEmpty
        emptyMessage="No products yet"
      />,
    );

    expect(screen.getByText('Products')).toBeInTheDocument();
    expect(screen.getByText('Create and manage products.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Add product' })).toBeInTheDocument();
    expect(screen.getByText('No products yet')).toBeInTheDocument();
  });

  it('renders grid content when requested', () => {
    render(
      <ManagementPageShell title="Products" description="Create and manage products." isEmpty={false} emptyMessage="No products yet" contentMode="grid">
        <div>Hot Desk</div>
      </ManagementPageShell>,
    );

    expect(screen.getByText('Hot Desk')).toBeInTheDocument();
    expect(screen.queryByText('No products yet')).not.toBeInTheDocument();
  });
});
