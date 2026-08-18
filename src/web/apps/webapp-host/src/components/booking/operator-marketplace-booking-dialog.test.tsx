import { fireEvent, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
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

describe('OperatorMarketplaceBookingDialog (Host)', () => {
  beforeEach(() => commit.mockReset());

  it('requires the operator to choose the booking fields', async () => {
    const user = userEvent.setup();
    render(<OperatorMarketplaceBookingDialog {...props} />);
    expect(screen.getByRole('button', { name: 'Create booking' })).toBeDisabled();
    await user.click(screen.getByLabelText('Product'));
    await user.click(screen.getByRole('option', { name: 'Desk' }));
    await user.click(screen.getByLabelText('Pricing'));
    await user.click(screen.getByRole('option', { name: /Credits/ }));
    expect(screen.getByLabelText('Booking credits')).toBeInTheDocument();
  });

  it('submits the selected customer entitlement to the marketplace mutation', async () => {
    const user = userEvent.setup();
    commit.mockImplementation((options: { onCompleted?: (response: unknown) => void } | undefined) =>
      options?.onCompleted?.({ addMarketplaceBooking: { booking: { id: 'booking-1' } } }),
    );
    render(<OperatorMarketplaceBookingDialog {...props} />);
    await user.click(screen.getByLabelText('Product'));
    await user.click(screen.getByRole('option', { name: 'Desk' }));
    await user.click(screen.getByLabelText('Pricing'));
    await user.click(screen.getByRole('option', { name: /Credits/ }));
    await user.click(screen.getByLabelText('Booking credits'));
    await user.click(screen.getByRole('option', { name: /2 credits/ }));
    fireEvent.change(screen.getByLabelText('From'), { target: { value: '2030-01-01T09:00' } });
    fireEvent.change(screen.getByLabelText('Until'), { target: { value: '2030-01-01T10:00' } });
    await user.click(screen.getByRole('button', { name: 'Create booking' }));
    expect(commit).toHaveBeenCalledWith(
      expect.objectContaining({
        variables: { input: expect.objectContaining({ customerIds: ['customer-1'], entitlementId: 'entitlement-1', productVersionId: 'version-1', pricingId: 'pricing-1' }) },
      }),
    );
    expect(props.onCompleted).toHaveBeenCalledOnce();
  });
});
