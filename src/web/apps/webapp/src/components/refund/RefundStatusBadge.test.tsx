import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { RefundStatusBadge } from './RefundStatusBadge';

describe('RefundStatusBadge', () => {
  it.each([
    ['Requested', 'Requested'],
    ['UnderReview', 'Under review'],
    ['Approved', 'Approved'],
    ['ProviderPending', 'Processing'],
    ['Processing', 'Processing'],
    ['Completed', 'Completed'],
    ['Failed', 'Failed'],
    ['Rejected', 'Rejected'],
    ['Cancelled', 'Canceled'],
    ['ReconciliationRequired', 'Reconciliation required'],
  ])('renders the American-English label for %s', (status, label) => {
    render(<RefundStatusBadge status={status} />);

    expect(screen.getByText(label)).toBeInTheDocument();
  });

  it('does not represent a provider-pending refund as completed', () => {
    render(<RefundStatusBadge status="ProviderPending" />);

    expect(screen.getByText('Processing')).toBeInTheDocument();
    expect(screen.queryByText(/^Completed$/)).not.toBeInTheDocument();
  });
});
