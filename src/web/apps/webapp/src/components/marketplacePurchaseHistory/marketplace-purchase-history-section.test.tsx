import { render, screen } from '@testing-library/react';
import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';
import { MarketplacePurchaseHistoryEventList } from './marketplace-purchase-history-event-list';

describe('MarketplacePurchaseHistoryEventList', () => {
  it('renders backend-provided events in the order received', () => {
    render(
      <MarketplacePurchaseHistoryEventList
        events={[
          {
            id: 'newest',
            type: 'PAYMENT_STATE_CHANGED',
            name: 'Payment state changed',
            occurredAt: '2026-08-29T12:00:00Z',
            cancellationRequestedAt: null,
            cancellationEffectiveAt: null,
            paymentStatus: 'CONFIRMED',
            refundStatus: null,
            creditQuantity: null,
            remainingCreditQuantity: null,
            reason: null,
          },
          {
            id: 'older',
            type: 'PURCHASE_CREATED',
            name: 'Purchase created',
            occurredAt: '2026-08-28T12:00:00Z',
            cancellationRequestedAt: null,
            cancellationEffectiveAt: null,
            paymentStatus: 'PENDING',
            refundStatus: null,
            creditQuantity: 10,
            remainingCreditQuantity: 10,
            reason: null,
          },
        ]}
      />,
    );

    const events = screen.getAllByText(/Payment state changed|Purchase created/);
    expect(events[0]).toHaveTextContent('Payment state changed');
    expect(events[1]).toHaveTextContent('Purchase created');
    expect(screen.getByText('Credits: 10')).toBeInTheDocument();
  });

  it('renders an explicit empty state when the backend has no events', () => {
    render(<MarketplacePurchaseHistoryEventList events={[]} />);

    expect(screen.getByText('No purchase history is available yet.')).toBeInTheDocument();
  });

  it('renders backend-provided cancellation dates without deriving them from the purchase', () => {
    render(
      <MarketplacePurchaseHistoryEventList
        events={[
          {
            id: 'scheduled-cancellation',
            type: 'CANCELLATION_SCHEDULED',
            name: 'Cancellation scheduled',
            occurredAt: '2026-08-29T12:00:00Z',
            cancellationRequestedAt: '2026-08-29T12:00:00Z',
            cancellationEffectiveAt: '2026-09-05T12:00:00Z',
            paymentStatus: null,
            refundStatus: null,
            creditQuantity: null,
            remainingCreditQuantity: null,
            reason: 'Customer requested cancellation at period end',
          },
        ]}
      />,
    );

    expect(
      screen.getByText(`Cancellation requested: ${new Date('2026-08-29T12:00:00Z').toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' })}`),
    ).toBeInTheDocument();
    expect(
      screen.getByText(`Cancellation effective: ${new Date('2026-09-05T12:00:00Z').toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' })}`),
    ).toBeInTheDocument();
    expect(screen.getByText('Customer requested cancellation at period end')).toBeInTheDocument();
  });

  it('keeps loading, error, refresh, and deep-link behavior at the detail-page query boundary', () => {
    const subscriptionSource = readFileSync(resolve(process.cwd(), 'src/components/marketplaceProductSubscription/marketplace-product-subscription-details.tsx'), 'utf8');
    const entitlementSource = readFileSync(resolve(process.cwd(), 'src/components/marketplaceEntitlement/entitlement-purchase-details.tsx'), 'utf8');

    for (const source of [subscriptionSource, entitlementSource]) {
      expect(source).toContain('history(first: 100)');
      expect(source).toContain('<Loading />');
    }

    expect(subscriptionSource).toContain('ErrorBoundary');
    expect(subscriptionSource).toContain('usePreloadedQuery');
    expect(entitlementSource).toContain('useLazyLoadQuery');

    expect(subscriptionSource).toContain('$subscriptionId: String!');
    expect(entitlementSource).toContain('$purchaseId: String!');
  });
});
