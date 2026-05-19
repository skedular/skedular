import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from './page';

vi.mock('@/rootPages/page', () => ({
  default: () => <div>Private organisation root</div>,
}));

describe('Teams root page foundation', () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it('keeps the root URL focused on private organisation entry', () => {
    render(<RootPage />);

    expect(screen.getByText('Private organisation root')).toBeInTheDocument();
    expect(screen.getByText('Private organisation root').parentElement).toHaveAttribute('data-product-app', 'webapp-teams');
    expect(screen.getByText('Private organisation root').parentElement).toHaveAttribute('data-review-scope', 'private-organisation-entry');
  });
});
