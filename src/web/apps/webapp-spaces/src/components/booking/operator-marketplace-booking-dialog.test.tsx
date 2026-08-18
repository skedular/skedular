import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OperatorMarketplaceBookingDialog from './operator-marketplace-booking-dialog';

const commit = vi.fn();
vi.mock('react-relay', () => ({ graphql: (value: TemplateStringsArray) => value.join(''), useMutation: () => [commit, false] }));
const props = {
  open: true,
  organizationCustomDomain: 'org.example',
  customerId: 'customer-1',
  products: [{ id: 'product-1', latestProductVersionId: 'version-1', title: 'Desk', pricingOptions: [{ id: 'pricing-1', title: 'Credits', fulfillmentType: 'ENTITLEMENT' }] }],
  entitlements: [{ id: 'entitlement-1', pricingId: 'pricing-1', availableQuantity: 2, expiresAt: '2030-01-01T00:00:00.000Z' }],
  onClose: vi.fn(),
  onCompleted: vi.fn(),
} as const;

describe('OperatorMarketplaceBookingDialog (Spaces)', () => {
  beforeEach(() => commit.mockReset());
  it('renders the operator entitlement workflow and disables incomplete submission', () => {
    render(<OperatorMarketplaceBookingDialog {...props} />);
    expect(screen.getByText('Make marketplace booking for customer')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Create booking' })).toBeDisabled();
    expect(screen.getByLabelText('Booking credits')).toBeInTheDocument();
  });
});
