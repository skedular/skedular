import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import OrganizationLocationsPageShell from './organization-locations-page-shell';

describe('OrganizationLocationsPageShell', () => {
  it('renders the page header, toolbar, and empty state when there are no locations', () => {
    render(
      <OrganizationLocationsPageShell actions={<button type="button">Add Location</button>} toolbar={<div>Filters</div>} isEmpty>
        <div>Ignored content</div>
      </OrganizationLocationsPageShell>,
    );

    expect(screen.getByText('Locations')).toBeInTheDocument();
    expect(screen.getByText('Manage bookable spaces, availability context, and contact details for each location.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Add Location' })).toBeInTheDocument();
    expect(screen.getByText('Filters')).toBeInTheDocument();
    expect(screen.getByText('No locations yet')).toBeInTheDocument();
  });

  it('renders the provided content when locations exist', () => {
    render(
      <OrganizationLocationsPageShell isEmpty={false}>
        <div>HQ</div>
        <div>Annex</div>
      </OrganizationLocationsPageShell>,
    );

    expect(screen.getByText('HQ')).toBeInTheDocument();
    expect(screen.getByText('Annex')).toBeInTheDocument();
    expect(screen.queryByText('No locations yet')).not.toBeInTheDocument();
  });
});
