'use client';

import Stack from '@mui/material/Stack';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';
import TextField from '@mui/material/TextField';
import { SmallIconTypography, SubtitleIconTypography } from '@skedular/ui';
import { useMemo, useState } from 'react';

export type RefundQueueItem = {
  id: string;
  bookingReference: string;
  customerName?: string | null;
  amount?: string | null;
  paymentMethod?: string | null;
  status: string;
  requestedAt: string;
};

export type ExternalRefundQueueItem = {
  provider: string;
  externalRefundId: string;
  amount?: string | null;
  currency?: string | null;
  status: string;
  lastSeenAt: string;
  resolutionReason?: string | null;
};

export type ExternalRefundQueuePageInfo = {
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  startCursor?: string | null;
  endCursor?: string | null;
};
export type RefundStatusOption = { type: string; name: string };
export type RefundQueuePageInfo = ExternalRefundQueuePageInfo;

export function RefundQueue({
  refunds,
  statusOptions = [],
  refundPageInfo,
  onRefundPageChange,
  externalRefunds = [],
  onRetry,
  onResolve,
  onResolveExternal,
  externalProvider,
  externalStatus,
  externalPageInfo,
  onExternalFilterChange,
  onExternalPageChange,
}: {
  refunds: readonly RefundQueueItem[];
  statusOptions?: readonly RefundStatusOption[];
  refundPageInfo?: RefundQueuePageInfo;
  onRefundPageChange?: (direction: 'next' | 'previous') => void;
  externalRefunds?: readonly ExternalRefundQueueItem[];
  onRetry?: (refundId: string) => Promise<void> | void;
  onResolve?: (refundId: string, reason: string) => Promise<void> | void;
  onResolveExternal?: (provider: string, externalRefundId: string, status: string, reason: string) => Promise<void> | void;
  externalProvider?: string | null;
  externalStatus?: string | null;
  externalPageInfo?: ExternalRefundQueuePageInfo;
  onExternalFilterChange?: (provider: string | null, status: string | null) => void;
  onExternalPageChange?: (direction: 'next' | 'previous') => void;
}) {
  const [status, setStatus] = useState('All');
  const [now] = useState(() => Date.now());
  const [resolutionReasons, setResolutionReasons] = useState<Record<string, string>>({});
  const [externalResolutionStatuses, setExternalResolutionStatuses] = useState<Record<string, string>>({});
  const filtered = useMemo(() => (status === 'All' ? refunds : refunds.filter((refund) => refund.status === status)), [refunds, status]);
  const daysPending = (requestedAt: string) => Math.max(0, Math.floor((now - new Date(requestedAt).getTime()) / 86_400_000));

  return (
    <Stack spacing={2}>
      <Card variant="outlined" sx={{ borderRadius: 3 }}>
        <CardContent>
          <Stack spacing={2}>
            <SubtitleIconTypography label="Refund operations" />
            <SmallIconTypography label="Review active refund work and provider reconciliation records." sx={{ opacity: 0.72 }} />
            <Select size="small" value={status} onChange={(event) => setStatus(event.target.value)} sx={{ alignSelf: 'flex-start', minWidth: 220 }}>
              <MenuItem value="All">All statuses</MenuItem>
              {statusOptions.map((option) => (
                <MenuItem key={option.type} value={option.type}>
                  {option.name}
                </MenuItem>
              ))}
            </Select>
            <List>
              {filtered.map((refund) => (
                <ListItem key={refund.id} divider>
                  <ListItemText
                    primary={`${refund.bookingReference} · ${refund.status}`}
                    secondary={`${refund.customerName ?? 'Unknown customer'} · ${refund.amount ?? 'Amount unavailable'} · ${refund.paymentMethod ?? 'Payment method unavailable'} · ${daysPending(refund.requestedAt)} days pending`}
                  />
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                    {refund.status === 'Failed' && onRetry && (
                      <Button size="small" onClick={() => onRetry(refund.id)}>
                        Retry
                      </Button>
                    )}
                    {refund.status === 'ReconciliationRequired' && onResolve && (
                      <>
                        <TextField
                          size="small"
                          label="Resolution reason"
                          value={resolutionReasons[refund.id] ?? ''}
                          onChange={(event) =>
                            setResolutionReasons((current) => ({
                              ...current,
                              [refund.id]: event.target.value,
                            }))
                          }
                        />
                        <Button
                          size="small"
                          variant="contained"
                          disabled={!resolutionReasons[refund.id]?.trim()}
                          onClick={() => onResolve(refund.id, resolutionReasons[refund.id]?.trim() ?? '')}
                        >
                          Resolve
                        </Button>
                      </>
                    )}
                  </Stack>
                </ListItem>
              ))}
              {externalRefunds.map((reconciliation) => {
                const key = `${reconciliation.provider}:${reconciliation.externalRefundId}`;
                const reason = resolutionReasons[key] ?? '';
                return (
                  <ListItem key={key} divider>
                    <ListItemText
                      primary={`${reconciliation.provider} · ${reconciliation.externalRefundId} · ${reconciliation.status}`}
                      secondary={`${reconciliation.amount ?? 'Amount unavailable'} ${reconciliation.currency ?? ''} · Last seen ${new Date(reconciliation.lastSeenAt).toLocaleString()}${
                        reconciliation.resolutionReason ? ` · ${reconciliation.resolutionReason}` : ''
                      }`}
                    />
                    {onResolveExternal && (
                      <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                        <Select
                          size="small"
                          value={externalResolutionStatuses[key] ?? 'Resolved'}
                          onChange={(event) =>
                            setExternalResolutionStatuses((current) => ({
                              ...current,
                              [key]: event.target.value,
                            }))
                          }
                        >
                          <MenuItem value="Resolved">Resolved</MenuItem>
                          <MenuItem value="Rejected">Rejected</MenuItem>
                        </Select>
                        <TextField
                          size="small"
                          label="Resolution reason"
                          value={reason}
                          onChange={(event) =>
                            setResolutionReasons((current) => ({
                              ...current,
                              [key]: event.target.value,
                            }))
                          }
                        />
                        <Button
                          size="small"
                          variant="contained"
                          disabled={!reason.trim()}
                          onClick={() => onResolveExternal(reconciliation.provider, reconciliation.externalRefundId, externalResolutionStatuses[key] ?? 'Resolved', reason.trim())}
                        >
                          Resolve
                        </Button>
                      </Stack>
                    )}
                  </ListItem>
                );
              })}
            </List>
            {refundPageInfo && onRefundPageChange ? (
              <Stack direction="row" spacing={1}>
                <Button size="small" disabled={!refundPageInfo.hasPreviousPage} onClick={() => onRefundPageChange('previous')}>
                  Previous page
                </Button>
                <Button size="small" disabled={!refundPageInfo.hasNextPage} onClick={() => onRefundPageChange('next')}>
                  Next page
                </Button>
              </Stack>
            ) : null}
            {externalPageInfo && (onExternalFilterChange || onExternalPageChange) ? (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                {onExternalFilterChange ? (
                  <>
                    <Select
                      size="small"
                      aria-label="External reconciliation provider"
                      value={externalProvider ?? 'All'}
                      onChange={(event) => onExternalFilterChange(event.target.value === 'All' ? null : event.target.value, externalStatus ?? null)}
                    >
                      <MenuItem value="All">All providers</MenuItem>
                      <MenuItem value="STRIPE">Stripe refunds</MenuItem>
                      <MenuItem value="STRIPE_PAYOUT">Stripe payouts</MenuItem>
                      <MenuItem value="XERO">Xero</MenuItem>
                    </Select>
                    <Select
                      size="small"
                      aria-label="External reconciliation status"
                      value={externalStatus ?? 'All'}
                      onChange={(event) => onExternalFilterChange(externalProvider ?? null, event.target.value === 'All' ? null : event.target.value)}
                    >
                      <MenuItem value="All">All reconciliation statuses</MenuItem>
                      <MenuItem value="Open">Open</MenuItem>
                      <MenuItem value="Resolved">Resolved</MenuItem>
                      <MenuItem value="Rejected">Rejected</MenuItem>
                    </Select>
                  </>
                ) : null}
                {onExternalPageChange ? (
                  <>
                    <Button size="small" disabled={!externalPageInfo.hasPreviousPage} onClick={() => onExternalPageChange('previous')}>
                      Previous page
                    </Button>
                    <Button size="small" disabled={!externalPageInfo.hasNextPage} onClick={() => onExternalPageChange('next')}>
                      Next page
                    </Button>
                  </>
                ) : null}
              </Stack>
            ) : null}
          </Stack>
        </CardContent>
      </Card>
    </Stack>
  );
}
