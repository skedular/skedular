import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import MarketplaceProductBookingPaymentPanel from './marketplace-product-booking-payment-panel';

describe('MarketplaceProductBookingPaymentPanel', () => {
  it('keeps a pending credit purchase payable from the marketplace page', () => {
    render(
      <MarketplaceProductBookingPaymentPanel
        checkoutUrl="https://checkout.stripe.test/session"
        ctaLabel="Pay now"
        entityLabel="credit purchase"
        invoiceUrl={null}
        isPaymentRequired
        paymentExpiry="2030-01-01T00:00:00Z"
        paymentMethodType="CARD"
        paymentStatusLabel="Pending"
        paymentStatusType="PENDING"
      />,
    );

    expect(screen.getByText('Time left to pay:', { exact: false })).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Pay now' })).toHaveAttribute('href', 'https://checkout.stripe.test/session');
  });
});
