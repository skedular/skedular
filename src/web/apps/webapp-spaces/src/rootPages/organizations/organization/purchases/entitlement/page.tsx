'use client';

import InvoiceDownloadLinks from '@/components/booking/invoice-download-links';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { NotificationContent } from '@/components/notification';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationEntitlementPurchaseDetail_rootQuery } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_rootQuery.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Link from '@mui/material/Link';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import { BodyIconTypography, DefaultDialogTitle, PageHeaderPanel, StackColumn, StackRow, SubtitleIconTypography, TwoButtonsDialogActions } from '@skedular/ui';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import { useParams } from 'next/navigation';
import { useEffect, useRef, useState } from 'react';
import { graphql, type PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

dayjs.extend(utc);

const formatBookingDateTime = (from: string, until: string) => {
  const start = dayjs.utc(from);
  const end = dayjs.utc(until);
  return start.hour() === 0 && start.minute() === 0 && end.hour() === 0 && end.minute() === 0
    ? `${start.format('D MMM YYYY')}, All day`
    : `${start.format('D MMM YYYY, HH:mm')}–${end.format('HH:mm')}`;
};

const surfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const RootQuery = graphql`
  query pageOrganizationEntitlementPurchaseDetail_rootQuery($purchaseId: String!, $linkedBookingsAfter: String) {
    entitlementPurchase(purchaseId: $purchaseId) {
      id
      paymentStatus
      lifecycleState
      paymentMethod
      serviceStartAt
      amount
      currency
      pricingId
      creditQuantity
      validityDays
      customerId
      customerName
      organizationId
      entitlementId
      entitlement {
        id
        autoRenew
        cancelAtPeriodEnd
        status
        nextRenewalAt
        renewalFailureReason
      }
      invoiceNumber
      invoiceUrl
      linkedBookings(after: $linkedBookingsAfter, first: 10) {
        totalCount
        pageInfo {
          hasNextPage
          hasPreviousPage
          endCursor
        }
        edges {
          node {
            id
            from
            until
            involvedCustomers {
              id
              name
              givenName
              middleName
              familyName
            }
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
          }
        }
      }
    }
  }
`;

const RenewalPolicyMutation = graphql`
  mutation pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation($input: SetEntitlementRenewalPolicyInput!) {
    setEntitlementRenewalPolicy(input: $input) {
      entitlement {
        id
        autoRenew
        cancelAtPeriodEnd
        status
        nextRenewalAt
        renewalFailureReason
      }
      error
    }
  }
`;

const ConfirmPaymentMutation = graphql`
  mutation pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation($input: ConfirmEntitlementPurchaseInput!) {
    confirmEntitlementPurchase(input: $input) {
      error
      purchase {
        id
        paymentStatus
        lifecycleState
        entitlement {
          id
          autoRenew
          cancelAtPeriodEnd
          status
          nextRenewalAt
          renewalFailureReason
        }
      }
    }
  }
`;

const RejectPaymentMutation = graphql`
  mutation pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation($input: RejectEntitlementPurchaseInput!) {
    rejectEntitlementPurchase(input: $input) {
      error
      purchase {
        id
        paymentStatus
        lifecycleState
      }
    }
  }
`;

const MakePaymentNotRequiredMutation = graphql`
  mutation pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation($input: MakeEntitlementPurchasePaymentNotRequiredInput!) {
    makeEntitlementPurchasePaymentNotRequired(input: $input) {
      error
      purchase {
        id
        paymentStatus
        lifecycleState
        entitlement {
          id
          autoRenew
          cancelAtPeriodEnd
          status
          nextRenewalAt
          renewalFailureReason
        }
      }
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationEntitlementPurchaseDetail_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
  onLinkedBookingAfterChange: (cursor: string | undefined) => void;
};

const SummaryRow = ({ label, value }: { label: string; value: string }) => (
  <StackColumn spacing={0.35}>
    <BodyIconTypography
      label={label}
      sx={{
        opacity: 0.62,
        textTransform: 'uppercase',
        fontSize: '0.78rem',
        letterSpacing: '0.06em',
      }}
    />
    <BodyIconTypography label={value} sx={{ opacity: 0.88 }} />
  </StackColumn>
);

const Detail = ({ queryReference, organizationCustomDomain, onLinkedBookingAfterChange }: Props) => {
  const data = usePreloadedQuery<pageOrganizationEntitlementPurchaseDetail_rootQuery>(RootQuery, queryReference);
  const [commitRenewalPolicy, isInFlight] = useMutation<pageOrganizationEntitlementPurchaseDetail_setRenewalPolicyMutation>(RenewalPolicyMutation);
  const [commitConfirmPayment, isConfirmingPayment] = useMutation<pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation>(ConfirmPaymentMutation);
  const [commitRejectPayment, isRejectingPayment] = useMutation<pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation>(RejectPaymentMutation);
  const [commitMakePaymentNotRequired, isMakingPaymentNotRequired] =
    useMutation<pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation>(MakePaymentNotRequiredMutation);
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const purchase = data.entitlementPurchase;
  const linkedBookings = data.entitlementPurchase?.linkedBookings;

  if (!purchase)
    return (
      <RootShell>
        <PageHeaderPanel title="Credit entitlement purchase" description="This entitlement purchase could not be found." />
      </RootShell>
    );

  const customerLabel = purchase.customerName || 'Customer unavailable';
  const entitlement = purchase.entitlement;
  const usedCredits = linkedBookings?.totalCount ?? 0;
  const freeCredits = Math.max(purchase.creditQuantity - usedCredits, 0);
  const isBankTransferPaymentPending = purchase.paymentMethod === 'BANK_TRANSFER' && purchase.paymentStatus === 'PENDING';
  const isPaymentActionInFlight = isConfirmingPayment || isRejectingPayment || isMakingPaymentNotRequired;

  const handlePaymentActionError = (error: string | null | undefined) => {
    if (error) toast.error(<NotificationContent content={error} />);
  };

  const confirmPayment = () => {
    commitConfirmPayment({
      variables: { input: { clientMutationId: uuid(), purchaseId: purchase.id } },
      onCompleted: (response) => {
        handlePaymentActionError(response.confirmEntitlementPurchase.error);
      },
      onError: (error) => {
        toast.error(<NotificationContent content={error.message} />);
      },
    });
  };

  const rejectPayment = () => {
    if (!window.confirm('Reject this bank-transfer payment? The entitlement will not be granted.')) return;
    commitRejectPayment({
      variables: { input: { clientMutationId: uuid(), purchaseId: purchase.id } },
      onCompleted: (response) => {
        handlePaymentActionError(response.rejectEntitlementPurchase.error);
      },
      onError: (error) => {
        toast.error(<NotificationContent content={error.message} />);
      },
    });
  };

  const makePaymentNotRequired = () => {
    if (!window.confirm('Mark this purchase as payment not required? This grants the entitlement without collecting payment.')) return;
    commitMakePaymentNotRequired({
      variables: { input: { clientMutationId: uuid(), purchaseId: purchase.id } },
      onCompleted: (response) => {
        handlePaymentActionError(response.makeEntitlementPurchasePaymentNotRequired.error);
      },
      onError: (error) => {
        toast.error(<NotificationContent content={error.message} />);
      },
    });
  };

  const cancel = () => {
    if (!entitlement?.id) return;
    commitRenewalPolicy({
      variables: {
        input: {
          clientMutationId: uuid(),
          entitlementId: entitlement.id,
          autoRenew: false,
          cancelAtPeriodEnd: true,
        },
      },
      onCompleted: (response) => {
        if (response.setEntitlementRenewalPolicy.error) {
          toast.error(<NotificationContent content={response.setEntitlementRenewalPolicy.error} />);
        } else {
          setShowCancelDialog(false);
        }
      },
      onError: (error) => {
        toast.error(<NotificationContent content={error.message} />);
      },
    });
  };

  const updateRenewalPolicy = (autoRenew: boolean, cancelAtPeriodEnd: boolean) => {
    if (!entitlement?.id) return;
    commitRenewalPolicy({
      variables: {
        input: {
          clientMutationId: uuid(),
          entitlementId: entitlement.id,
          autoRenew,
          cancelAtPeriodEnd,
        },
      },
      onCompleted: (response) => {
        if (response.setEntitlementRenewalPolicy.error) toast.error(<NotificationContent content={response.setEntitlementRenewalPolicy.error} />);
      },
      onError: (error) => {
        toast.error(<NotificationContent content={error.message} />);
      },
    });
  };

  return (
    <RootShell>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
        <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pb: 4 }} spacing={2}>
          <PageHeaderPanel title="Credit entitlement purchase" description="Review payment, entitlement usage, linked bookings, and renewal settings." />
          <Box
            sx={{
              display: 'grid',
              gap: 2,
              gridTemplateColumns: { xs: '1fr', xl: 'minmax(0, 1.1fr) 380px' },
              alignItems: 'start',
            }}
          >
            <StackColumn spacing={2}>
              <Card sx={surfaceSx}>
                <CardContent sx={{ p: 2.5 }}>
                  <StackRow
                    sx={{
                      alignItems: 'flex-start',
                      justifyContent: 'space-between',
                      gap: 1,
                      flexWrap: 'wrap',
                    }}
                  >
                    <StackColumn spacing={0.5}>
                      <SubtitleIconTypography label={`${purchase.creditQuantity} credit(s)`} />
                      <BodyIconTypography label={customerLabel} sx={{ opacity: 0.84 }} />
                      <BodyIconTypography
                        label={`Credits valid ${dayjs.utc(purchase.serviceStartAt).format('D MMM YYYY')} – ${dayjs
                          .utc(purchase.serviceStartAt)
                          .add(purchase.validityDays, 'day')
                          .format('D MMM YYYY')} (${purchase.validityDays} day(s))`}
                        sx={{ opacity: 0.74 }}
                      />
                    </StackColumn>
                    <Chip label={purchase.lifecycleState} variant="outlined" />
                  </StackRow>
                  <Divider sx={{ my: 2 }} />
                  <StackColumn spacing={0.8}>
                    <BodyIconTypography label={`Payment: ${purchase.paymentStatus} • Method: ${purchase.paymentMethod}`} />
                    <BodyIconTypography label={`Amount: ${purchase.amount} ${purchase.currency} • Valid for ${purchase.validityDays} day(s)`} sx={{ opacity: 0.78 }} />
                  </StackColumn>
                  {entitlement ? (
                    <Box sx={{ mt: 2, p: 2, borderRadius: 3, border: 1, borderColor: 'divider' }}>
                      <StackRow sx={{ alignItems: 'center', justifyContent: 'space-between', gap: 1 }}>
                        <SubtitleIconTypography label="Renewal" />
                        {entitlement.status === 'CANCELLED' ? <Chip label="Cancelled" color="default" size="small" variant="outlined" /> : null}
                      </StackRow>
                      <BodyIconTypography
                        label={
                          entitlement.status === 'CANCELLED'
                            ? 'This entitlement has been cancelled.'
                            : entitlement.cancelAtPeriodEnd
                              ? 'Renewal is scheduled to stop at the end of this credit period.'
                              : entitlement.autoRenew
                                ? `Auto-renew is enabled${entitlement.nextRenewalAt ? `; next renewal ${dayjs.utc(entitlement.nextRenewalAt).format('D MMM YYYY')}.` : '.'}`
                                : 'Auto-renew is disabled for this entitlement.'
                        }
                        sx={{ mt: 0.75, opacity: 0.78 }}
                      />
                      {entitlement.renewalFailureReason ? <BodyIconTypography label={entitlement.renewalFailureReason} sx={{ mt: 0.75, color: 'error.main' }} /> : null}
                      {entitlement.status === 'ACTIVE' ? (
                        <StackRow sx={{ mt: 1.5, gap: 1, flexWrap: 'wrap' }}>
                          <Button
                            variant="contained"
                            size="small"
                            sx={{ textTransform: 'none' }}
                            onClick={() => updateRenewalPolicy(!entitlement.autoRenew, false)}
                            disabled={isInFlight}
                          >
                            {entitlement.autoRenew ? 'Disable auto-renew' : 'Enable auto-renew'}
                          </Button>
                          {entitlement.autoRenew ? (
                            <Button
                              variant="outlined"
                              size="small"
                              sx={{ textTransform: 'none' }}
                              onClick={() => (entitlement.cancelAtPeriodEnd ? updateRenewalPolicy(true, false) : setShowCancelDialog(true))}
                              disabled={isInFlight}
                            >
                              {entitlement.cancelAtPeriodEnd ? 'Keep renewing' : 'Cancel at period end'}
                            </Button>
                          ) : null}
                        </StackRow>
                      ) : null}
                    </Box>
                  ) : null}
                </CardContent>
              </Card>
              <Card sx={surfaceSx}>
                <CardContent sx={{ p: 2.5 }}>
                  <SubtitleIconTypography label="Linked bookings" />
                  <BodyIconTypography label="Bookings that consumed credits from this entitlement." sx={{ mt: 0.75, opacity: 0.78 }} />
                  <StackColumn spacing={1} sx={{ mt: 2 }}>
                    {linkedBookings?.edges.length ? (
                      linkedBookings.edges.map(({ node: booking }) => (
                        <StackRow
                          key={booking.id}
                          sx={{
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            gap: 1,
                          }}
                        >
                          <StackColumn spacing={0.25}>
                            <BodyIconTypography label={formatBookingDateTime(booking.from, booking.until)} />
                            <BodyIconTypography label={booking.involvedLocations.map((location) => location.name).join(', ') || 'Location pending'} sx={{ opacity: 0.72 }} />
                            <BodyIconTypography
                              label={`Resources: ${booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Resources pending'}`}
                              sx={{ opacity: 0.72 }}
                            />
                          </StackColumn>
                          <Button
                            component={Link}
                            href={getOrganizationBookingBaseLink(undefined, organizationCustomDomain, booking.id)}
                            size="small"
                            sx={{ textTransform: 'none' }}
                          >
                            View
                          </Button>
                        </StackRow>
                      ))
                    ) : (
                      <BodyIconTypography label="No linked bookings yet." sx={{ opacity: 0.72 }} />
                    )}
                  </StackColumn>
                  {linkedBookings?.totalCount && linkedBookings.totalCount > 10 ? (
                    <StackRow sx={{ justifyContent: 'flex-end', gap: 1, mt: 2 }}>
                      <Button size="small" disabled={!linkedBookings.pageInfo.hasPreviousPage} onClick={() => onLinkedBookingAfterChange(undefined)} sx={{ textTransform: 'none' }}>
                        Previous
                      </Button>
                      <Button
                        size="small"
                        disabled={!linkedBookings.pageInfo.hasNextPage}
                        onClick={() => onLinkedBookingAfterChange(linkedBookings.pageInfo.endCursor ?? undefined)}
                        sx={{ textTransform: 'none' }}
                      >
                        Next
                      </Button>
                    </StackRow>
                  ) : null}
                </CardContent>
              </Card>
            </StackColumn>
            <Card sx={surfaceSx}>
              <CardContent sx={{ p: 2.5 }}>
                <SubtitleIconTypography label="Entitlement summary" />
                <StackColumn spacing={1.25} sx={{ mt: 2 }}>
                  <SummaryRow label="Payment status" value={purchase.paymentStatus} />
                  <SummaryRow label="Payment method" value={purchase.paymentMethod} />
                  {isBankTransferPaymentPending ? (
                    <StackRow sx={{ flexWrap: 'wrap', gap: 1, pt: 1 }}>
                      <Button size="small" variant="contained" disabled={isPaymentActionInFlight} onClick={confirmPayment}>
                        Confirm payment
                      </Button>
                      <Button size="small" color="error" variant="outlined" disabled={isPaymentActionInFlight} onClick={rejectPayment}>
                        Reject payment
                      </Button>
                      <Button size="small" variant="outlined" disabled={isPaymentActionInFlight} onClick={makePaymentNotRequired}>
                        Payment not required
                      </Button>
                    </StackRow>
                  ) : null}
                  <SummaryRow label="Credits used" value={`${usedCredits}`} />
                  <SummaryRow label="Credits free" value={`${freeCredits}`} />
                  <SummaryRow label="Credits total" value={`${purchase.creditQuantity}`} />
                  <SummaryRow label="Validity" value={`${purchase.validityDays} day(s)`} />
                  {purchase.invoiceNumber || purchase.invoiceUrl ? (
                    <InvoiceDownloadLinks invoices={[]} legacyInvoiceUrl={purchase.invoiceUrl} linkLabel="View invoice" emptyLabel="Invoice not available yet" size="body" />
                  ) : null}
                </StackColumn>
              </CardContent>
            </Card>
          </Box>
        </StackColumn>
      </Box>
      <Dialog open={showCancelDialog} onClose={() => setShowCancelDialog(false)}>
        <DefaultDialogTitle title="Cancel entitlement renewal" />
        <DialogContent>
          <DialogContentText>Cancel future renewal for this entitlement? Existing credits and linked bookings remain available according to their current terms.</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={cancel}
            onSecondaryClicked={() => setShowCancelDialog(false)}
            primaryLabel="Cancel renewal"
            secondaryLabel="Keep entitlement"
          />
        </DialogContent>
      </Dialog>
    </RootShell>
  );
};

const Page = () => {
  const params = useParams<{
    organizationCustomDomain: string;
    purchaseId: string;
  }>();
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationEntitlementPurchaseDetail_rootQuery>(RootQuery);
  const [linkedBookingAfter, setLinkedBookingAfter] = useState<string>();
  const linkedBookingAfterHistory = useRef<Array<string | undefined>>([]);
  const handleLinkedBookingAfterChange = (cursor: string | undefined) => {
    if (cursor === undefined) {
      setLinkedBookingAfter(linkedBookingAfterHistory.current.pop());
    } else {
      linkedBookingAfterHistory.current.push(linkedBookingAfter);
      setLinkedBookingAfter(cursor);
    }
  };
  useEffect(() => {
    if (params.purchaseId && params.organizationCustomDomain)
      loadQuery(
        {
          purchaseId: params.purchaseId,
          linkedBookingsAfter: linkedBookingAfter,
        },
        { fetchPolicy: 'store-and-network' },
      );
  }, [loadQuery, params.organizationCustomDomain, params.purchaseId, linkedBookingAfter]);
  return queryReference ? (
    <Detail queryReference={queryReference} organizationCustomDomain={params.organizationCustomDomain} onLinkedBookingAfterChange={handleLinkedBookingAfterChange} />
  ) : (
    <Loading />
  );
};

export default Page;
