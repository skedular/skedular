import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import RootPage from './page';

vi.mock('@/rootPages/page', () => ({
  default: () => <div>Private organization root</div>,
}));

describe('Teams root page foundation', () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it('keeps the root URL focused on private organization entry', () => {
    render(<RootPage />);

    expect(screen.getByText('Private organization root')).toBeInTheDocument();
    expect(screen.getByText('Private organization root').closest('[data-product-app]')).toHaveAttribute('data-product-app', 'webapp-teams');
    expect(screen.getByText('Private organization root').closest('[data-review-scope]')).toHaveAttribute('data-review-scope', 'private-organisation-entry');
  });
});
