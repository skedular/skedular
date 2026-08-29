'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TableContainer from '@mui/material/TableContainer';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import { BodyIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import type { ReactNode } from 'react';
import { useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { PurchaseDetailNavigation } from './purchase-detail-navigation';

export type PurchaseDetailValue = { label: string; value: ReactNode };
export type PurchaseDetailBooking = { id: string; title: string; meta: string; href: string };
export type PurchaseDetailAction = { label: string; onClick: () => void; tone?: 'default' | 'destructive'; disabled?: boolean };
export type PurchaseDetailHistoryEvent = {
  title: string;
  meta: string;
  details?: string;
};

const formatHistoryValue = (value: string | null | undefined) =>
  value
    ? value
        .toLowerCase()
        .replaceAll('_', ' ')
        .replace(/(^| )\S/g, (character) => character.toUpperCase())
    : null;

export const formatPurchaseHistoryEventDetails = (event: {
  type?: string | null;
  previousPaymentStatus?: string | null;
  paymentStatus?: string | null;
  previousRefundStatus?: string | null;
  refundStatus?: string | null;
  creditQuantity?: number | null;
  remainingCreditQuantity?: number | null;
  amount?: number | null;
  currency?: string | null;
  cancellationRequestedAt?: string | null;
  cancellationEffectiveAt?: string | null;
  reason?: string | null;
}) => {
  const details: string[] = [];
  const previousPayment = formatHistoryValue(event.previousPaymentStatus);
  const payment = formatHistoryValue(event.paymentStatus);
  const isCancellationEvent = event.type === 'CANCELLATION_SCHEDULED' || event.type === 'CANCELLATION_COMPLETED';
  if (payment && !isCancellationEvent) details.push(`Payment: ${previousPayment ? `${previousPayment} → ` : ''}${payment}`);

  const previousRefund = formatHistoryValue(event.previousRefundStatus);
  const refund = formatHistoryValue(event.refundStatus);
  if (refund && !isCancellationEvent) details.push(`Refund: ${previousRefund ? `${previousRefund} → ` : ''}${refund}`);
  if (event.creditQuantity !== null && event.creditQuantity !== undefined) details.push(`Credits: ${event.creditQuantity}`);
  if (event.remainingCreditQuantity !== null && event.remainingCreditQuantity !== undefined) details.push(`Remaining: ${event.remainingCreditQuantity}`);
  if (event.amount !== null && event.amount !== undefined) details.push(`Amount: ${event.currency ? `${event.currency} ` : ''}${event.amount}`);
  if (event.cancellationRequestedAt) details.push(`Requested: ${new Date(event.cancellationRequestedAt).toLocaleString()}`);
  if (event.cancellationEffectiveAt) details.push(`Cancellation effective: ${new Date(event.cancellationEffectiveAt).toLocaleString()}`);
  if (event.reason) details.push(`Reason: ${event.reason}`);
  return details.join(' · ');
};

type Props = {
  title: string;
  purchaseType: string;
  customer: string;
  customerAvatar?: ReactNode;
  status: string;
  statusColor?: 'default' | 'success' | 'warning' | 'error' | 'primary';
  headline: string;
  summary: PurchaseDetailValue[];
  payment: ReactNode;
  refund?: ReactNode;
  actions?: PurchaseDetailAction[];
  linkedBookings?: PurchaseDetailBooking[];
  history?: PurchaseDetailHistoryEvent[];
};

const cardSx = { borderRadius: 4, border: 1, borderColor: 'divider', boxShadow: 'none' };

export const PurchaseDetailPage = ({
  title,
  purchaseType,
  customer,
  customerAvatar,
  status,
  statusColor = 'default',
  headline,
  summary,
  payment,
  refund,
  actions,
  linkedBookings,
  history = [],
}: Props) => (
  <PurchaseDetailPageContent
    title={title}
    purchaseType={purchaseType}
    customer={customer}
    status={status}
    statusColor={statusColor}
    headline={headline}
    customerAvatar={customerAvatar}
    summary={summary}
    payment={payment}
    refund={refund}
    actions={actions}
    linkedBookings={linkedBookings}
    history={history}
  />
);

const PurchaseDetailPageContent = ({
  title,
  purchaseType,
  customer,
  customerAvatar,
  status,
  statusColor = 'default',
  headline,
  summary,
  payment,
  refund,
  actions,
  linkedBookings,
  history = [],
}: Props) => {
  const searchParams = useSearchParams();
  const router = useRouter();
  const [actionsAnchor, setActionsAnchor] = useState<null | HTMLElement>(null);
  const [linkedBookingMenu, setLinkedBookingMenu] = useState<{ anchor: HTMLElement; bookingId: string } | null>(null);
  const activeSection =
    searchParams.get('tab') === 'bookings' ? 'bookings' : searchParams.get('tab') === 'billing' ? 'billing' : searchParams.get('tab') === 'refunds' ? 'refunds' : 'overview';
  const linkedBookingMenuItem = linkedBookings?.find((booking) => booking.id === linkedBookingMenu?.bookingId);

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: { xs: 1, sm: 2 } }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, pb: 4 }} spacing={2}>
        <Card sx={cardSx}>
          <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
            <StackColumn spacing={0.5} sx={{ mb: 1 }}>
              <SubtitleIconTypography label={title} fontWeight={700} sx={{ fontSize: '1.05rem' }} />
            </StackColumn>
            <Box sx={{ pb: 2 }}>
              <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', gap: 2, flexWrap: 'wrap' }}>
                <StackColumn spacing={0.5}>
                  <LeadIconTypography label={headline} />
                </StackColumn>
                <StackRow sx={{ gap: 1, alignItems: 'center' }}>
                  {actions?.length ? (
                    <Button size="small" variant="outlined" onClick={(event) => setActionsAnchor(event.currentTarget)} aria-haspopup="menu" aria-expanded={Boolean(actionsAnchor)}>
                      Actions
                    </Button>
                  ) : null}
                </StackRow>
              </StackRow>
              <Divider sx={{ my: 2 }} />
              <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr 1fr', md: 'repeat(4, 1fr)' }, gap: 2 }}>
                {[
                  { label: 'Type', value: purchaseType },
                  {
                    label: 'Customer',
                    value: (
                      <StackRow spacing={0.75} sx={{ alignItems: 'center' }}>
                        {customerAvatar}
                        <BodyIconTypography label={customer} />
                      </StackRow>
                    ),
                  },
                  { label: 'Status', value: <Chip label={status} color={statusColor} variant="outlined" size="small" /> },
                  ...summary,
                ].map((item) => (
                  <StackColumn key={item.label} spacing={0.35}>
                    <SmallIconTypography label={item.label} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                    <BodyIconTypography label={item.value} />
                  </StackColumn>
                ))}
              </Box>
            </Box>
            <PurchaseDetailNavigation hasLinkedBookings={!!linkedBookings} />
          </CardContent>
        </Card>
        {actions?.length ? (
          <Menu anchorEl={actionsAnchor} open={Boolean(actionsAnchor)} onClose={() => setActionsAnchor(null)}>
            {actions.map((action, index) => {
              const paymentAction = action.label.toLowerCase().includes('payment');
              const previousPaymentAction = index > 0 && actions[index - 1].label.toLowerCase().includes('payment');
              return (
                <Box key={action.label}>
                  {index > 0 && paymentAction !== previousPaymentAction ? <Divider /> : null}
                  <Button
                    fullWidth
                    disabled={action.disabled}
                    onClick={() => {
                      setActionsAnchor(null);
                      action.onClick();
                    }}
                    color={action.tone === 'destructive' ? 'error' : 'inherit'}
                    sx={{ justifyContent: 'flex-start', px: 2, py: 1, textTransform: 'none' }}
                  >
                    {action.label}
                  </Button>
                </Box>
              );
            })}
          </Menu>
        ) : null}
        <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', xl: 'minmax(0, 1.2fr) 360px' }, gap: 2, alignItems: 'start' }}>
          <StackColumn spacing={2} sx={{ gridColumn: { xl: '1 / -1' } }}>
            {activeSection === 'overview' ? (
              <Card id="purchase-section-overview" sx={cardSx}>
                <CardContent sx={{ p: { xs: 1, sm: 2 } }}>
                  <SubtitleIconTypography label="History" />
                  {history.length ? (
                    <TableContainer component={Box} sx={{ overflowX: 'auto' }}>
                      <Table size="small" aria-label="Purchase history" sx={{ mt: 1, minWidth: 520 }}>
                        <TableHead>
                          <TableRow
                            sx={{
                              '& th': { fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.04em', color: 'text.primary', borderBottom: 1, borderColor: 'divider' },
                            }}
                          >
                            <TableCell>Date</TableCell>
                            <TableCell>Event</TableCell>
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {history.map((event) => (
                            <TableRow key={`${event.title}-${event.meta}`} hover sx={{ '& td': { py: 1.25, borderBottom: 1, borderColor: 'divider' } }}>
                              <TableCell>{event.meta}</TableCell>
                              <TableCell>
                                <StackColumn spacing={0.25}>
                                  <BodyIconTypography label={event.title} />
                                  {event.details ? <SmallIconTypography label={event.details} sx={{ opacity: 0.72 }} /> : null}
                                </StackColumn>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  ) : (
                    <BodyIconTypography label="Activity will appear here as this purchase changes." sx={{ opacity: 0.7, mt: 2 }} />
                  )}
                </CardContent>
              </Card>
            ) : null}
            {activeSection === 'billing' ? (
              <Card id="purchase-section-billing" sx={cardSx}>
                <CardContent sx={{ p: { xs: 1, sm: 2 } }}>{payment}</CardContent>
              </Card>
            ) : null}
            {activeSection === 'refunds' ? (
              <Card id="purchase-section-refunds" sx={cardSx}>
                <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
                  <SubtitleIconTypography label="Refunds" />
                  {refund ?? <BodyIconTypography label="No refund has been requested." sx={{ opacity: 0.7, mt: 2 }} />}
                </CardContent>
              </Card>
            ) : null}
            {activeSection === 'bookings' && linkedBookings ? (
              <Card id="purchase-section-bookings" sx={cardSx}>
                <CardContent sx={{ p: { xs: 2, sm: 3 } }}>
                  <SubtitleIconTypography label="Linked bookings" />
                  {linkedBookings.length ? (
                    <TableContainer component={Box} sx={{ overflowX: 'auto' }}>
                      <Table size="small" sx={{ mt: 1, minWidth: 560 }} aria-label="Linked bookings">
                        <TableHead>
                          <TableRow
                            sx={{
                              '& th': { fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.04em', color: 'text.primary', borderBottom: 1, borderColor: 'divider' },
                            }}
                          >
                            <TableCell>Booking</TableCell>
                            <TableCell>Details</TableCell>
                            <TableCell align="right" />
                          </TableRow>
                        </TableHead>
                        <TableBody>
                          {linkedBookings.map((booking) => (
                            <TableRow
                              key={booking.id}
                              hover
                              tabIndex={0}
                              onClick={() => router.push(booking.href)}
                              onKeyDown={(event) => {
                                if (event.key === 'Enter' || event.key === ' ') {
                                  event.preventDefault();
                                  router.push(booking.href);
                                }
                              }}
                              sx={{ cursor: 'pointer', '& td': { py: 1.25, borderBottom: 1, borderColor: 'divider' } }}
                            >
                              <TableCell>{booking.title}</TableCell>
                              <TableCell>{booking.meta}</TableCell>
                              <TableCell align="right" onClick={(event) => event.stopPropagation()}>
                                <IconButton
                                  size="small"
                                  aria-label={`Actions for ${booking.title}`}
                                  onClick={(event) => setLinkedBookingMenu({ anchor: event.currentTarget, bookingId: booking.id })}
                                >
                                  <MoreVertIcon fontSize="small" />
                                </IconButton>
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </TableContainer>
                  ) : (
                    <BodyIconTypography label="No linked bookings yet." sx={{ opacity: 0.7, mt: 2 }} />
                  )}
                  <Menu anchorEl={linkedBookingMenu?.anchor} open={Boolean(linkedBookingMenu)} onClose={() => setLinkedBookingMenu(null)}>
                    <MenuItem
                      onClick={() => {
                        if (linkedBookingMenuItem) router.push(linkedBookingMenuItem.href);
                        setLinkedBookingMenu(null);
                      }}
                    >
                      View
                    </MenuItem>
                  </Menu>
                </CardContent>
              </Card>
            ) : null}
          </StackColumn>
          <StackColumn spacing={2}></StackColumn>
        </Box>
      </StackColumn>
    </Box>
  );
};
