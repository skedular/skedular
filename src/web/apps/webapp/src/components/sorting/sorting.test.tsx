import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import SortingDirection from './sorting';

vi.mock('@skedular/ui', () => ({
  BodyIconTypography: ({ children }: { children: React.ReactNode }) => <span>{children}</span>,
}));
vi.mock('@/components/icons', () => ({
  AscDirectionIcon: () => null,
  DescDirectionIcon: () => null,
}));

describe('SortingDirection', () => {
  it('renders without error', () => {
    const { container } = render(
      <SortingDirection options={[{ id: 'name', label: 'Name' }]} defaultOption="name" defaultSortingDirectionValue="Ascending" onValueChange={vi.fn()} />,
    );
    expect(container).toBeTruthy();
  });
});
