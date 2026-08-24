import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationLocationResourceManagementList from './organization-location-resource-management-list';

vi.mock('@/components/resourceType', () => ({
  ResourceType: ({ resourceType }: { resourceType: { name?: string | null } }) => <span>{resourceType.name}</span>,
}));

vi.mock('@/components/customTag', () => ({
  CustomTags: ({ customTags }: { customTags: { name?: string | null }[] }) => <div>{customTags.map((item) => item.name).join(', ') || 'N/A'}</div>,
}));

vi.mock('@/components/zone', () => ({
  Zones: ({ zones }: { zones: { name?: string | null }[] }) => <div>{zones.map((item) => item.name).join(', ') || 'N/A'}</div>,
}));

describe('OrganizationLocationResourceManagementList', () => {
  it('renders a compact resource row without an inline accordion', () => {
    const onOpenResource = vi.fn();
    render(
      <OrganizationLocationResourceManagementList
        items={[
          {
            id: 'resource-1',
            resourceName: 'Desk A1',
            resourceType: { id: 'desk', name: 'Desk', color: null },
            customTags: [{ id: 'quiet', name: 'Quiet', color: null }],
            zones: [{ id: 'north', name: 'North Wing', color: null }],
            isActive: true,
            isPreferred: false,
            capacity: 1,
          },
        ]}
        selectedIds={[]}
        onToggleSelected={vi.fn()}
        onOpenResource={onOpenResource}
        onOpenMoreActions={vi.fn()}
        onDeactivateSelected={vi.fn()}
        onActivateSelected={vi.fn()}
        onDeleteSelected={vi.fn()}
      />,
    );

    expect(screen.getByText('Desk A1')).toBeInTheDocument();
    expect(screen.getAllByText('1 person')).not.toHaveLength(0);
    expect(screen.getAllByText('North Wing')).not.toHaveLength(0);

    fireEvent.click(screen.getByText('Desk A1'));
    expect(onOpenResource).toHaveBeenCalledWith('resource-1');
    expect(screen.queryByRole('button', { name: 'View' })).not.toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Details' })).not.toBeInTheDocument();
  });

  it('shows bulk actions when resources are selected', () => {
    const onDeactivateSelected = vi.fn();

    render(
      <OrganizationLocationResourceManagementList
        items={[
          {
            id: 'resource-1',
            resourceName: 'Desk A1',
            resourceType: { id: 'desk', name: 'Desk', color: null },
            customTags: [],
            zones: [],
            isActive: true,
            isPreferred: false,
            capacity: 1,
          },
        ]}
        selectedIds={['resource-1']}
        onToggleSelected={vi.fn()}
        onOpenResource={vi.fn()}
        onOpenMoreActions={vi.fn()}
        onDeactivateSelected={onDeactivateSelected}
        onActivateSelected={vi.fn()}
        onDeleteSelected={vi.fn()}
      />,
    );

    expect(screen.getByText('1 resource selected')).toBeInTheDocument();

    fireEvent.click(screen.getByRole('button', { name: 'Deactivate' }));

    expect(onDeactivateSelected).toHaveBeenCalledWith(['resource-1']);
  });
});
