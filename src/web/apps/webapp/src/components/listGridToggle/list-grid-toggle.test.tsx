import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import ListGridToggle from './list-grid-toggle';

vi.mock('@/components/icons', () => ({
  GridViewIcon: () => null,
  ListViewIcon: () => null,
}));

describe('ListGridToggle', () => {
  it('renders without error', () => {
    const { container } = render(<ListGridToggle defaultValue="list" onChange={vi.fn()} />);
    expect(container).toBeTruthy();
  });
});
