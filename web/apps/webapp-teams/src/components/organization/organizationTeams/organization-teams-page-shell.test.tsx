import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import OrganizationTeamsPageShell from './organization-teams-page-shell';

describe('OrganizationTeamsPageShell', () => {
  it('renders the page header, toolbar, and empty state when there are no teams', () => {
    render(
      <OrganizationTeamsPageShell actions={<button type="button">Add Team</button>} toolbar={<div>Filters</div>} isEmpty>
        <div>Ignored content</div>
      </OrganizationTeamsPageShell>,
    );

    expect(screen.getByText('Teams')).toBeInTheDocument();
    expect(screen.getByText('Create teams, manage members, and choose the main location for each team.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Add Team' })).toBeInTheDocument();
    expect(screen.getByText('Filters')).toBeInTheDocument();
    expect(screen.getByText('Create your first team to get started.')).toBeInTheDocument();
  });

  it('renders the provided content when teams exist', () => {
    render(
      <OrganizationTeamsPageShell isEmpty={false}>
        <div>Ops</div>
        <div>Sales</div>
      </OrganizationTeamsPageShell>,
    );

    expect(screen.getByText('Ops')).toBeInTheDocument();
    expect(screen.getByText('Sales')).toBeInTheDocument();
    expect(screen.queryByText('Create your first team to get started.')).not.toBeInTheDocument();
  });
});
