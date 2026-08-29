'use client';

import InvoiceDownloadLinks from '@/components/booking/invoice-download-links';
import { getOrganizationBookingBaseLink, getOrganizationRefundBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import {
  SupportedMarketplaceBookingSubscriptionCancellationMode,
  SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  toSupportedMarketplaceBookingSubscriptionCancellationModeDetails,
} from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import { toMarketplaceBookingSubscriptionLifecycleDisplay } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-lifecycle';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { buildMarketplaceBookingInstancesQueryVariables, getRelayErrorMessage, RelayError, toRootError, useIntegratedPlatform, useKnownParams } from '@skedular/shared';

import { CustomerAvatar } from '@/components/avatars';
import { formatPurchaseHistoryEventDetails, PurchaseDetailPage, type PurchaseDetailAction } from '@/components/purchaseDetail/purchase-detail-page';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationSubscriptionDetail_confirmRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_confirmRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptionDetail_rejectRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_rejectRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptionDetail_rootQuery } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_rootQuery.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Link from '@mui/material/Link';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TextField from '@mui/material/TextField';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';

import { BodyIconTypography, DefaultDialogTitle, SmallIconTypography, StackColumn, StackRow, TwoButtonsDialogActions } from '@skedular/ui';
import dayjs from 'dayjs';
import { memo, useCallback, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

const surfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const RootQuery = graphql`
  query pageOrganizationSubscriptionDetail_rootQuery($organizationCustomDomain: String!, $subscriptionId: String!, $bookingAfter: String, $linkedBookingAfter: String) {
    marketplaceBookingSubscriptionCancellationModes {
      type
      name
    }
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {
      canViewBookings
      canModifyPaymentMethod
    }
    marketplaceBookingSubscription(id: $subscriptionId) {
      id
      history(first: 100) {
        edges {
          node {
            id
            type
            name
            occurredAt
            previousPaymentStatus
            paymentStatus
            previousRefundStatus
            refundStatus
            refundId
            creditQuantity
            remainingCreditQuantity
            amount
            currency
            cancellationRequestedAt
            cancellationEffectiveAt
            reason
          }
        }
      }
      cancellationPolicyOverridden
      cancellationOverrideReason
      startedAt
      nextRenewalAt
      cancelledAt
      autoRenew
      cancelAtPeriodEnd
      refund {
        id
        currency {
          type
          name
        }
        status {
          type
          name
        }
        requestedAt
        lastProcessedAt
        refundAmount
        refundPercentage
        currencyToDisplay
        reason
        lastError
        externalRefundNumber
        requestedByCustomerName
        canProcessInXero
        xeroProcessingBlockedReason
        events {
          id
          eventType {
            type
            name
          }
          occurredAt
          refundAmount
          currencyToDisplay
          reason
          lastError
          externalRefundNumber
          actorName
        }
      }
      status {
        type
        name
      }
      involvedCustomers {
        id
        name
        givenName
        middleName
        familyName
        photoUrl
      }
      marketplaceBooking {
        invoiceUrl
        totalAmountToDisplay
        quantity
        paymentStatus {
          type
          name
        }
        paymentMethod {
          type
          name
        }
        productVersion {
          listingMetadata {
            title
          }
        }
      }
      recurringBookings {
        id
        startDate
        endDate
        marketplaceBooking {
          id
          quantity
          invoiceUrl
          totalAmountToDisplay
          paymentStatus {
            type
            name
          }
          paymentMethod {
            type
            name
          }
        }
      }
      bookingInstances(after: $bookingAfter, first: 50) {
        totalCount
        pageInfo {
          hasNextPage
          hasPreviousPage
          startCursor
          endCursor
        }
        edges {
          cursor
          node {
            id
            startDate
            endDate
            marketplaceBooking {
              id
              invoiceUrl
              paymentStatus {
                type
                name
              }
            }
          }
        }
      }
      linkedBookings(after: $linkedBookingAfter, first: 50) {
        totalCount
        pageInfo {
          hasNextPage
          hasPreviousPage
          startCursor
          endCursor
        }
        edges {
          cursor
          node {
            id
            from
            until
            bookingResources {
              resource {
                id
                name
              }
            }
            involvedLocations {
              uniqueId
              name
            }
            marketplaceBooking {
              paymentStatus {
                type
                name
              }
            }
          }
        }
      }
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSubscriptionDetail_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onBookingAfterChange: (cursor: string | undefined) => void;
  onLinkedBookingAfterChange: (cursor: string | undefined) => void;
};

const getCustomerDisplayName = (customer: { name?: string | null; givenName?: string | null; middleName?: string | null; familyName?: string | null }) => {
  const structuredName = [customer.givenName, customer.middleName, customer.familyName].filter(Boolean).join(' ').trim();
  return structuredName || customer.name || 'Customer';
};

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('D MMM YYYY') : 'Not scheduled');

const formatBookingDateTime = (from: string, until: string) => {
  const start = dayjs.utc(from);
  const end = dayjs.utc(until);
  return start.hour() === 0 && start.minute() === 0 && end.hour() === 0 && end.minute() === 0
    ? `${start.format('D MMM YYYY')}, All day`
    : `${start.format('D MMM YYYY, HH:mm')}–${end.format('HH:mm')}`;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationSubscriptionDetail_rootQuery>(RootQuery, queryReference);
  const { integratedPlatform } = useIntegratedPlatform();
  const [commitDeleteMarketplaceBookingSubscription] = useMutation<pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation>(graphql`
    mutation pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
      deleteMarketplaceBookingSubscription(input: $input) {
        cancellationError {
          code
          message
        }
        marketplaceBookingSubscription {
          id
          cancelAtPeriodEnd
          nextRenewalAt
          status {
            type
            name
          }
        }
      }
    }
  `);
  const [commitConfirmRecurringBookingPayment] = useMutation<pageOrganizationSubscriptionDetail_confirmRecurringBookingPaymentMutation>(graphql`
    mutation pageOrganizationSubscriptionDetail_confirmRecurringBookingPaymentMutation($input: ConfirmRecurringBookingPaymentInput!) @raw_response_type {
      confirmRecurringBookingPayment(input: $input) {
        recurringBooking {
          id
          marketplaceBooking {
            id
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);
  const [commitRejectRecurringBookingPayment] = useMutation<pageOrganizationSubscriptionDetail_rejectRecurringBookingPaymentMutation>(graphql`
    mutation pageOrganizationSubscriptionDetail_rejectRecurringBookingPaymentMutation($input: RejectRecurringBookingPaymentInput!) @raw_response_type {
      rejectRecurringBookingPayment(input: $input) {
        recurringBooking {
          id
          marketplaceBooking {
            id
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);
  const [commitMakeRecurringBookingPaymentNotRequired] = useMutation<pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation>(graphql`
    mutation pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation($input: MakeRecurringBookingPaymentNotRequiredInput!) @raw_response_type {
      makeRecurringBookingPaymentNotRequired(input: $input) {
        recurringBooking {
          id
          marketplaceBooking {
            id
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const subscription = rootData.marketplaceBookingSubscription;
  const immediateCancellationMode = useMemo((): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => {
    const mode = rootData.marketplaceBookingSubscriptionCancellationModes.find((item) => item.type === 'IMMEDIATE');
    return mode ? toSupportedMarketplaceBookingSubscriptionCancellationModeDetails(mode.type, mode.name) : null;
  }, [rootData.marketplaceBookingSubscriptionCancellationModes]);
  const atPeriodEndCancellationMode = useMemo((): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => {
    const mode = rootData.marketplaceBookingSubscriptionCancellationModes.find((item) => item.type === 'AT_PERIOD_END');
    return mode ? toSupportedMarketplaceBookingSubscriptionCancellationModeDetails(mode.type, mode.name) : null;
  }, [rootData.marketplaceBookingSubscriptionCancellationModes]);
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState<{
    mode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails;
    productTitle: string;
  } | null>(null);
  const [cancellationOverrideReason, setCancellationOverrideReason] = useState('');

  const handleDeleteMarketplaceBookingSubscriptionClick = (
    subscriptionId: string,
    productTitle: string,
    cancellationModeType: SupportedMarketplaceBookingSubscriptionCancellationMode,
    overrideReason?: string,
  ) => {
    commitDeleteMarketplaceBookingSubscription({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: subscriptionId,
          cancellationMode: cancellationModeType,
          cancellationOverrideReason: overrideReason,
        },
      },
      onCompleted: (data, errors) => {
        const cancellationError = data.deleteMarketplaceBookingSubscription.cancellationError;
        if (cancellationError) {
          toast(<NotificationContent content={`We couldn't update ${productTitle}. ${cancellationError.message}`} />, errorNotificationOptions);
          return;
        }
        if (errors && errors.length > 0) {
          toast(<NotificationContent content={`We couldn't update ${productTitle}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        toast(<NotificationContent content={`We couldn't update ${productTitle}. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleConfirmRecurringBookingPaymentClick = (recurringBookingId: string, cycleLabel: string) => {
    commitConfirmRecurringBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBookingId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast(<NotificationContent content={`We couldn't confirm payment for ${cycleLabel}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        toast(<NotificationContent content={`We couldn't confirm payment for ${cycleLabel}. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleRejectRecurringBookingPaymentClick = (recurringBookingId: string, cycleLabel: string) => {
    commitRejectRecurringBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBookingId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast(<NotificationContent content={`We couldn't reject payment for ${cycleLabel}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        toast(<NotificationContent content={`We couldn't reject payment for ${cycleLabel}. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  const handleMakeRecurringBookingPaymentNotRequiredClick = (recurringBookingId: string, cycleLabel: string) => {
    commitMakeRecurringBookingPaymentNotRequired({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBookingId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast(<NotificationContent content={`We couldn't update payment settings for ${cycleLabel}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        toast(<NotificationContent content={`We couldn't update payment settings for ${cycleLabel}. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  if (!rootData.organizationBookingPermissions.canModifyPaymentMethod) {
    return (
      <RootShell>
        <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
          <Box sx={{ ...surfaceSx, width: '100%', maxWidth: 1200, px: 3, py: 4 }}>
            <BodyIconTypography label="You do not have permission to manage subscription payments for this organization." />
          </Box>
        </Box>
      </RootShell>
    );
  }

  if (!subscription) {
    return (
      <RootShell>
        <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
          <Box sx={{ ...surfaceSx, width: '100%', maxWidth: 1200, px: 3, py: 4 }}>
            <BodyIconTypography label="We could not find this subscription anymore." />
          </Box>
        </Box>
      </RootShell>
    );
  }

  const sortedRecurringBookings = [...subscription.recurringBookings].sort((left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime());
  const lifecycleDisplay = subscription
    ? toMarketplaceBookingSubscriptionLifecycleDisplay({
        autoRenew: subscription.autoRenew,
        cancelAtPeriodEnd: subscription.cancelAtPeriodEnd,
        isCancelled: subscription.status.type === 'CANCELLED',
        fallbackActiveLabel: subscription.status.name,
      })
    : null;
  const productTitle = subscription?.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription';
  const customerLabel =
    subscription?.involvedCustomers.length && subscription.involvedCustomers.length > 0 ? getCustomerDisplayName(subscription.involvedCustomers[0]) : 'Customer unavailable';
  const pendingCycle = sortedRecurringBookings.find((cycle) => cycle.marketplaceBooking?.paymentStatus.type === 'PENDING') ?? sortedRecurringBookings[0];
  const billingPeriods = [...sortedRecurringBookings].reverse();
  const retainedBillingPeriod = billingPeriods.find((period) => period.marketplaceBooking?.totalAmountToDisplay || period.marketplaceBooking?.invoiceUrl);
  const rootSubscriptionAmount = subscription?.marketplaceBooking.totalAmountToDisplay;
  const retainedPeriodAmount = retainedBillingPeriod?.marketplaceBooking?.totalAmountToDisplay;
  const subscriptionAmount =
    rootSubscriptionAmount && rootSubscriptionAmount !== 'N/A'
      ? rootSubscriptionAmount
      : retainedPeriodAmount && retainedPeriodAmount !== 'N/A'
        ? retainedPeriodAmount
        : 'Not available';
  const subscriptionInvoiceUrl = subscription?.marketplaceBooking.invoiceUrl ?? retainedBillingPeriod?.marketplaceBooking?.invoiceUrl;
  const purchaseActions: PurchaseDetailAction[] =
    subscription?.status.type === 'ACTIVE'
      ? [
          {
            label: 'Cancel now',
            tone: 'destructive',
            onClick: () => {
              if (immediateCancellationMode) {
                setCancellationOverrideReason('');
                setPendingCancellationConfirmation({ mode: immediateCancellationMode, productTitle });
              }
            },
          },
          ...(subscription.autoRenew
            ? [
                {
                  label: 'Cancel at period end',
                  tone: 'destructive' as const,
                  onClick: () =>
                    atPeriodEndCancellationMode ? handleDeleteMarketplaceBookingSubscriptionClick(subscription.id, productTitle, atPeriodEndCancellationMode.type) : undefined,
                },
              ]
            : []),
          ...(pendingCycle
            ? [
                {
                  label: 'Confirm payment',
                  disabled: subscription.marketplaceBooking.paymentStatus.type !== 'PENDING' && pendingCycle.marketplaceBooking?.paymentStatus.type !== 'PENDING',
                  onClick: () =>
                    handleConfirmRecurringBookingPaymentClick(
                      pendingCycle.id,
                      `${toStoredDate(pendingCycle.startDate)} - ${pendingCycle.endDate ? toStoredDate(pendingCycle.endDate) : 'Open ended'}`,
                    ),
                },
                {
                  label: 'Reject payment',
                  tone: 'destructive' as const,
                  disabled: subscription.marketplaceBooking.paymentStatus.type !== 'PENDING' && pendingCycle.marketplaceBooking?.paymentStatus.type !== 'PENDING',
                  onClick: () =>
                    handleRejectRecurringBookingPaymentClick(
                      pendingCycle.id,
                      `${toStoredDate(pendingCycle.startDate)} - ${pendingCycle.endDate ? toStoredDate(pendingCycle.endDate) : 'Open ended'}`,
                    ),
                },
                {
                  label: 'Payment not required',
                  disabled: subscription.marketplaceBooking.paymentStatus.type !== 'PENDING' && pendingCycle.marketplaceBooking?.paymentStatus.type !== 'PENDING',
                  onClick: () =>
                    handleMakeRecurringBookingPaymentNotRequiredClick(
                      pendingCycle.id,
                      `${toStoredDate(pendingCycle.startDate)} - ${pendingCycle.endDate ? toStoredDate(pendingCycle.endDate) : 'Open ended'}`,
                    ),
                },
              ]
            : []),
        ]
      : [];

  if (subscription) {
    return (
      <RootShell>
        <PurchaseDetailPage
          title="Purchase details"
          purchaseType="Subscription"
          customer={customerLabel}
          customerAvatar={<CustomerAvatar name={subscription.involvedCustomers[0]} photo={{ url: subscription.involvedCustomers[0].photoUrl }} size="small" />}
          headline={productTitle}
          status={lifecycleDisplay?.statusLabel ?? subscription.status.name}
          statusColor={lifecycleDisplay?.statusColor ?? 'default'}
          summary={[
            { label: 'Started', value: toStoredDate(subscription.startedAt) },
            { label: 'Renewal', value: lifecycleDisplay?.renewalLabel ?? 'Not scheduled' },
            { label: 'Next renewal', value: subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : 'Not scheduled' },
            { label: 'Payment', value: subscription.marketplaceBooking.paymentStatus.name },
            { label: 'Method', value: subscription.marketplaceBooking.paymentMethod.name ?? 'Not set' },
            { label: 'Amount', value: subscriptionAmount },
            { label: 'Quantity', value: `${subscription.marketplaceBooking.quantity}` },
            {
              label: 'Invoice',
              value: subscriptionInvoiceUrl ? (
                <InvoiceDownloadLinks invoices={[]} legacyInvoiceUrl={subscriptionInvoiceUrl} linkLabel="View invoice" size="small" />
              ) : (
                'Not available'
              ),
            },
          ]}
          payment={
            billingPeriods.length ? (
              <TableContainer component={Box} sx={{ overflowX: 'auto' }}>
                <Table size="small" sx={{ minWidth: 700 }} aria-label="Subscription billing periods">
                  <TableHead>
                    <TableRow
                      sx={{ '& th': { fontWeight: 700, textTransform: 'uppercase', letterSpacing: '0.04em', color: 'text.primary', borderBottom: 1, borderColor: 'divider' } }}
                    >
                      <TableCell>Period</TableCell>
                      <TableCell>Payment status</TableCell>
                      <TableCell align="right">Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {billingPeriods.map((cycle) => (
                      <TableRow key={cycle.id} hover sx={{ '& td': { py: 1.25, borderBottom: 1, borderColor: 'divider' } }}>
                        <TableCell>{`${toStoredDate(cycle.startDate)} – ${cycle.endDate ? toStoredDate(cycle.endDate) : 'Open ended'}`}</TableCell>
                        <TableCell>{cycle.marketplaceBooking?.paymentStatus.name ?? 'Not set'}</TableCell>
                        <TableCell align="right">
                          {cycle.marketplaceBooking?.paymentStatus.type === 'PENDING' ? (
                            <StackRow sx={{ justifyContent: 'flex-end', gap: 1, flexWrap: 'wrap' }}>
                              <Button
                                size="small"
                                variant="contained"
                                onClick={() =>
                                  handleConfirmRecurringBookingPaymentClick(
                                    cycle.id,
                                    `${toStoredDate(cycle.startDate)} - ${cycle.endDate ? toStoredDate(cycle.endDate) : 'Open ended'}`,
                                  )
                                }
                              >
                                Confirm
                              </Button>
                              <Button
                                size="small"
                                variant="outlined"
                                onClick={() =>
                                  handleRejectRecurringBookingPaymentClick(
                                    cycle.id,
                                    `${toStoredDate(cycle.startDate)} - ${cycle.endDate ? toStoredDate(cycle.endDate) : 'Open ended'}`,
                                  )
                                }
                              >
                                Reject
                              </Button>
                              <Button
                                size="small"
                                variant="text"
                                onClick={() =>
                                  handleMakeRecurringBookingPaymentNotRequiredClick(
                                    cycle.id,
                                    `${toStoredDate(cycle.startDate)} - ${cycle.endDate ? toStoredDate(cycle.endDate) : 'Open ended'}`,
                                  )
                                }
                              >
                                Not required
                              </Button>
                            </StackRow>
                          ) : (
                            '—'
                          )}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            ) : (
              <BodyIconTypography label="No billing periods have been created yet." />
            )
          }
          refund={
            subscription.refund ? (
              <StackColumn spacing={1} sx={{ mt: 2 }}>
                <BodyIconTypography label={`Status: ${subscription.refund.status.name}`} />
                <BodyIconTypography label={`Amount: ${subscription.refund.currencyToDisplay}`} />
                <SmallIconTypography label={subscription.refund.reason ?? 'No reason provided'} />
                <Link href={getOrganizationRefundBaseLink(integratedPlatform, organizationCustomDomain, subscription.refund.id)} sx={{ alignSelf: 'flex-start' }}>
                  View refund details
                </Link>
              </StackColumn>
            ) : undefined
          }
          actions={purchaseActions}
          linkedBookings={subscription.linkedBookings.edges.map(({ node }) => ({
            id: node.id,
            title: formatBookingDateTime(node.from, node.until),
            meta: `${node.involvedLocations.map((location) => location.name).join(', ') || 'Location pending'} · ${node.bookingResources.map((item) => item.resource.name).join(', ') || 'Resources pending'}`,
            href: getOrganizationBookingBaseLink(integratedPlatform, organizationCustomDomain, node.id),
          }))}
          history={subscription.history.edges.map(({ node }) => ({
            title: node.name,
            meta: new Date(node.occurredAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' }),
            details: formatPurchaseHistoryEventDetails(node),
          }))}
        />
        <Dialog open={pendingCancellationConfirmation !== null} onClose={() => setPendingCancellationConfirmation(null)}>
          <DefaultDialogTitle title="Cancel subscription now" />
          <DialogContent>
            <DialogContentText>Cancel this subscription now? Future billing will stop immediately.</DialogContentText>
            <TextField
              label="Cancellation reason"
              value={cancellationOverrideReason}
              onChange={(event) => setCancellationOverrideReason(event.target.value)}
              required
              multiline
              minRows={2}
              fullWidth
              helperText="Required when the cancellation policy would block this cancellation."
              sx={{ mt: 2 }}
            />
            <TwoButtonsDialogActions
              onPrimaryClicked={() => {
                if (!pendingCancellationConfirmation || cancellationOverrideReason.trim() === '') return;
                handleDeleteMarketplaceBookingSubscriptionClick(
                  subscription.id,
                  pendingCancellationConfirmation.productTitle,
                  pendingCancellationConfirmation.mode.type,
                  cancellationOverrideReason.trim(),
                );
                setPendingCancellationConfirmation(null);
              }}
              onSecondaryClicked={() => setPendingCancellationConfirmation(null)}
              primaryLabel="Cancel now"
              secondaryLabel="Keep subscription"
            />
          </DialogContent>
        </Dialog>
      </RootShell>
    );
  }
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationSubscriptionDetail_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [bookingAfter, setBookingAfter] = useState<string | undefined>();
  const bookingAfterHistory = useRef<Array<string | undefined>>([]);
  const [linkedBookingAfter, setLinkedBookingAfter] = useState<string | undefined>();
  const linkedBookingAfterHistory = useRef<Array<string | undefined>>([]);
  const handleBookingAfterChange = useCallback(
    (cursor: string | undefined) => {
      if (cursor === undefined) {
        setBookingAfter(bookingAfterHistory.current.pop());
      } else {
        bookingAfterHistory.current.push(bookingAfter);
        setBookingAfter(cursor);
      }
    },
    [bookingAfter],
  );
  const handleLinkedBookingAfterChange = useCallback(
    (cursor: string | undefined) => {
      if (cursor === undefined) {
        setLinkedBookingAfter(linkedBookingAfterHistory.current.pop());
      } else {
        linkedBookingAfterHistory.current.push(linkedBookingAfter);
        setLinkedBookingAfter(cursor);
      }
    },
    [linkedBookingAfter],
  );
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, subscriptionId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!subscriptionId) {
    throw new Error('subscriptionId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        subscriptionId,
        ...buildMarketplaceBookingInstancesQueryVariables(bookingAfter),
        linkedBookingAfter,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, subscriptionId, triggerReloadId, bookingAfter, linkedBookingAfter]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        onBookingAfterChange={handleBookingAfterChange}
        onLinkedBookingAfterChange={handleLinkedBookingAfterChange}
      />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
