'use client';

import Chip from '@mui/material/Chip';

const labels: Record<string, string> = {
  Requested: 'Requested',
  UnderReview: 'Under review',
  Approved: 'Approved',
  ProviderPending: 'Processing',
  Processing: 'Processing',
  Completed: 'Completed',
  Failed: 'Failed',
  Rejected: 'Rejected',
  Cancelled: 'Canceled',
  ReconciliationRequired: 'Reconciliation required',
};

export function RefundStatusBadge({ status }: { status: string | null | undefined }) {
  const normalized = status ?? 'Requested';
  const label = labels[normalized] ?? normalized;
  const color: 'success' | 'error' | 'warning' | 'info' =
    normalized === 'Completed'
      ? 'success'
      : normalized === 'Failed' || normalized === 'Rejected' || normalized === 'Cancelled'
        ? 'error'
        : normalized === 'ReconciliationRequired'
          ? 'warning'
          : 'info';

  return <Chip size="small" color={color} label={label} />;
}
