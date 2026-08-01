import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { RefundQueue } from './RefundQueue';

const refunds = [
  {
    id: '1',
    bookingReference: 'booking-1',
    status: 'Failed',
    requestedAt: new Date().toISOString(),
    amount: '10 USD',
  },
  {
    id: '2',
    bookingReference: 'booking-2',
    status: 'ReconciliationRequired',
    requestedAt: new Date().toISOString(),
    amount: '20 USD',
  },
] as const;
const statusOptions = [
  { type: 'Failed', name: 'Failed' },
  { type: 'ReconciliationRequired', name: 'Reconciliation required' },
] as const;

describe('RefundQueue', () => {
  it('filters refunds by status', () => {
    render(<RefundQueue refunds={refunds} statusOptions={statusOptions} />);
    fireEvent.mouseDown(screen.getByRole('combobox'));
    fireEvent.click(screen.getByRole('option', { name: 'Failed' }));
    expect(screen.getByText(/booking-1/)).toBeInTheDocument();
    expect(screen.queryByText(/booking-2/)).not.toBeInTheDocument();
  });

  it('calls retry and resolve callbacks with the selected values', () => {
    const onRetry = vi.fn();
    const onResolve = vi.fn();
    render(<RefundQueue refunds={refunds} statusOptions={statusOptions} onRetry={onRetry} onResolve={onResolve} />);
    fireEvent.click(screen.getByRole('button', { name: 'Retry' }));
    fireEvent.change(screen.getByLabelText('Resolution reason'), {
      target: { value: 'Provider confirmed' },
    });
    fireEvent.click(screen.getByRole('button', { name: 'Resolve' }));
    expect(onRetry).toHaveBeenCalledWith('1');
    expect(onResolve).toHaveBeenCalledWith('2', 'Provider confirmed');
  });

  it('calls the external reconciliation callback with provider resolution details', () => {
    const onResolveExternal = vi.fn();
    render(
      <RefundQueue
        refunds={[]}
        externalRefunds={[
          {
            provider: 'STRIPE',
            externalRefundId: 'po_1',
            status: 'Open',
            lastSeenAt: new Date().toISOString(),
          },
        ]}
        onResolveExternal={onResolveExternal}
      />,
    );

    fireEvent.change(screen.getByLabelText('Resolution reason'), {
      target: { value: 'Matched manually' },
    });
    fireEvent.click(screen.getByRole('button', { name: 'Resolve' }));

    expect(onResolveExternal).toHaveBeenCalledWith('STRIPE', 'po_1', 'Resolved', 'Matched manually');
  });

  it('exposes external reconciliation filters and pagination', () => {
    const onExternalFilterChange = vi.fn();
    const onExternalPageChange = vi.fn();
    render(
      <RefundQueue
        refunds={[]}
        externalRefunds={[]}
        externalProvider={null}
        externalStatus="Open"
        externalPageInfo={{ hasNextPage: true, hasPreviousPage: false }}
        onExternalFilterChange={onExternalFilterChange}
        onExternalPageChange={onExternalPageChange}
      />,
    );

    fireEvent.mouseDown(screen.getByLabelText('External reconciliation provider'));
    fireEvent.click(screen.getByRole('option', { name: 'Xero' }));
    fireEvent.mouseDown(screen.getByLabelText('External reconciliation status'));
    fireEvent.click(screen.getByRole('option', { name: 'Resolved' }));
    fireEvent.click(screen.getByRole('button', { name: 'Next page' }));

    expect(onExternalFilterChange).toHaveBeenNthCalledWith(1, 'XERO', 'Open');
    expect(onExternalFilterChange).toHaveBeenNthCalledWith(2, null, 'Resolved');
    expect(onExternalPageChange).toHaveBeenCalledWith('next');
    expect(screen.getByRole('button', { name: 'Previous page' })).toBeDisabled();
  });
});
