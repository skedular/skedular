import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import AppShellLayout from '../app-shell-layout';
import OrganisationEmptyState from '../organisation-empty-state';

describe('AppShellLayout', () => {
  it('renders the shared shell and review checkpoint', () => {
    render(
      <AppShellLayout appName="WebApp Spaces" title="Spaces foundation" description="Operator workflows move here." reviewNote="Ready for review.">
        <OrganisationEmptyState title="No co-working organisations available" description="Create or join one first." actionLabel="Create co-working organisation" />
      </AppShellLayout>,
    );

    expect(screen.getByText('WebApp Spaces')).toBeInTheDocument();
    expect(screen.getByText('Spaces foundation')).toBeInTheDocument();
    expect(screen.getByText('Review checkpoint')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Create co-working organisation' })).toBeInTheDocument();
  });
});
