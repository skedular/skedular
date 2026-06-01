import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import AppShellLayout from '../app-shell-layout';
import OrganisationEmptyState from '../organisation-empty-state';

describe('AppShellLayout', () => {
  it('renders the shared shell and review checkpoint', () => {
    render(
      <AppShellLayout appName="Skedular Spaces" title="Spaces foundation" description="Operator workflows move here." reviewNote="Ready for review.">
        <OrganisationEmptyState title="No co-working organizations available" description="Create or join one first." actionLabel="Create co-working organization" />
      </AppShellLayout>,
    );

    expect(screen.getByText('Skedular Spaces')).toBeInTheDocument();
    expect(screen.getByText('Spaces foundation')).toBeInTheDocument();
    expect(screen.getByText('Review checkpoint')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Create co-working organization' })).toBeInTheDocument();
  });

  it('does not reserve header space for product switching', () => {
    render(
      <AppShellLayout appName="Skedular" title="Skedular foundation" description="Customer workflows.">
        <div>Content</div>
      </AppShellLayout>,
    );

    expect(screen.queryByRole('button', { name: 'Switch app' })).not.toBeInTheDocument();
  });
});
