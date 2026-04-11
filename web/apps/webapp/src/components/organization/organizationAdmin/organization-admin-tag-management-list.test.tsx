import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationAdminTagManagementList from './organization-admin-tag-management-list';

describe('OrganizationAdminTagManagementList', () => {
  it('renders compact rows and forwards selection and more actions', () => {
    const onToggleSelected = vi.fn();
    const onOpenMoreActions = vi.fn();

    render(
      <OrganizationAdminTagManagementList
        items={[
          {
            id: 'zone-1',
            name: 'North Wing',
            description: 'Near the reception desk',
          },
        ]}
        emptyTitle="No zones found"
        emptyDescription="Add a zone."
        selectedIds={[]}
        onToggleSelected={onToggleSelected}
        onOpenMoreActions={onOpenMoreActions}
        renderPrimary={(item) => <span>{item.name}</span>}
      />,
    );

    expect(screen.getByText('North Wing')).toBeInTheDocument();
    expect(screen.getByText('Near the reception desk')).toBeInTheDocument();

    fireEvent.click(screen.getByRole('checkbox', { name: 'Select North Wing' }));
    expect(onToggleSelected).toHaveBeenCalledWith('zone-1');

    fireEvent.click(screen.getByRole('button', { name: 'More actions for North Wing' }));
    expect(onOpenMoreActions).toHaveBeenCalledTimes(1);
    expect(onOpenMoreActions.mock.calls[0][0]).toBe('zone-1');
  });
});
