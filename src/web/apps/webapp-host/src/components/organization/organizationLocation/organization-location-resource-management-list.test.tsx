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

vi.mock('@/components/productTag', () => ({
  ProductTags: ({ productTags }: { productTags: { name?: string | null }[] }) => <div>{productTags.map((item) => item.name).join(', ') || 'N/A'}</div>,
}));

describe('OrganizationLocationResourceManagementList', () => {
  it('renders a compact resource row and expands details on demand', () => {
    render(
      <OrganizationLocationResourceManagementList
        items={[
          {
            id: 'resource-1',
            resourceName: 'Desk A1',
            resourceType: { id: 'desk', name: 'Desk', color: null },
            customTags: [{ id: 'quiet', name: 'Quiet', color: null }],
            zones: [{ id: 'north', name: 'North Wing', color: null }],
            productTags: [{ id: 'day', name: 'Day Pass', color: null }],
            isActive: true,
            isPreferred: false,
            capacity: 1,
          },
        ]}
        selectedIds={[]}
        onToggleSelected={vi.fn()}
        onOpenResource={vi.fn()}
        onOpenMoreActions={vi.fn()}
        onDeactivateSelected={vi.fn()}
        onActivateSelected={vi.fn()}
        onDeleteSelected={vi.fn()}
      />,
    );

    expect(screen.getByText('Desk A1')).toBeInTheDocument();
    expect(screen.getByText('Capacity 1')).toBeInTheDocument();
    expect(screen.getByText('North Wing')).toBeInTheDocument();
    expect(screen.getByText('Quiet')).toBeInTheDocument();
    expect(screen.getByText('Day Pass')).toBeInTheDocument();

    fireEvent.click(screen.getByRole('button', { name: 'Details' }));

    expect(screen.getByText('Hide details')).toBeInTheDocument();
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
            productTags: [],
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
