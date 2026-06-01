import { logUnsupportedWebappPathHandled } from '@/libs/logging/aggregate-marketplace-telemetry';
import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import MarketplaceUnsupportedPath from './marketplace-unsupported-path';

vi.mock('@/libs/logging', () => ({ default: { info: vi.fn(), warn: vi.fn(), error: vi.fn() } }));
vi.mock('@/libs/logging/aggregate-marketplace-telemetry', () => ({
  logUnsupportedWebappPathHandled: vi.fn(),
}));

describe('MarketplaceUnsupportedPath', () => {
  it('renders customer-safe copy without private administration controls', () => {
    render(<MarketplaceUnsupportedPath pathCategory="admin" ownerClassification="webapp-teams" />);

    expect(screen.getByRole('status')).toHaveTextContent('This page is not available here');
    expect(screen.getByText(/customer marketplace stays on this webapp/i)).toBeInTheDocument();
    expect(screen.queryByRole('button')).not.toBeInTheDocument();
    expect(screen.queryByText(/admin controls/i)).not.toBeInTheDocument();
  });

  it('logs unsupported path handling without redirecting', () => {
    window.history.pushState({}, '', '/marketplace/organizations/example-owner/products/product-1');

    render(<MarketplaceUnsupportedPath pathCategory="owner-specific" ownerClassification="webapp" />);

    expect(logUnsupportedWebappPathHandled).toHaveBeenCalledWith(expect.objectContaining({ pathCategory: 'owner-specific', ownerClassification: 'webapp' }));
    expect(window.location.pathname).toBe('/marketplace/organizations/example-owner/products/product-1');
  });

  it('handles old admin paths in place without exposing relocation controls', () => {
    window.history.pushState({}, '', '/organizations/acme/bookings');

    render(<MarketplaceUnsupportedPath pathCategory="old-admin-path" ownerClassification="webapp-teams" />);

    expect(screen.getByRole('status')).toHaveTextContent('This page is not available here');
    expect(screen.queryByText(/go to teams/i)).not.toBeInTheDocument();
    expect(window.location.pathname).toBe('/organizations/acme/bookings');
  });

  it('handles unsupported marketplace paths in place', () => {
    window.history.pushState({}, '', '/marketplace/unsupported/resource-admin');

    render(<MarketplaceUnsupportedPath pathCategory="unsupported-marketplace" ownerClassification="webapp" />);

    expect(logUnsupportedWebappPathHandled).toHaveBeenCalledWith(expect.objectContaining({ pathCategory: 'unsupported-marketplace' }));
    expect(window.location.pathname).toBe('/marketplace/unsupported/resource-admin');
  });
});
