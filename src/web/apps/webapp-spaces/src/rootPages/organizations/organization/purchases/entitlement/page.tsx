'use client';

import InvoiceDownloadLinks from '@/components/booking/invoice-download-links';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { NotificationContent } from '@/components/notification';
import { RootShell } from '@/components/rootShell';
import { CustomerAvatar } from '@/components/avatars';
import { formatPurchaseHistoryEventDetails, PurchaseDetailPage, type PurchaseDetailAction } from '@/components/purchaseDetail/purchase-detail-page';
import type { pageOrganizationEntitlementPurchaseDetail_rootQuery } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_rootQuery.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation.graphql';
import type { pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation.graphql';
import { BodyIconTypography, PageHeaderPanel, StackColumn } from '@skedular/ui';
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

const RootQuery = graphql`
  query pageOrganizationEntitlementPurchaseDetail_rootQuery($purchaseId: String!, $linkedBookingsAfter: String) {
    entitlementPurchase(purchaseId: $purchaseId) {
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
        status
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
          status
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
          status
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

const Detail = ({ queryReference, organizationCustomDomain }: Props) => {
  const data = usePreloadedQuery<pageOrganizationEntitlementPurchaseDetail_rootQuery>(RootQuery, queryReference);
  const [commitConfirmPayment, isConfirmingPayment] = useMutation<pageOrganizationEntitlementPurchaseDetail_confirmEntitlementPurchaseMutation>(ConfirmPaymentMutation);
  const [commitRejectPayment, isRejectingPayment] = useMutation<pageOrganizationEntitlementPurchaseDetail_rejectEntitlementPurchaseMutation>(RejectPaymentMutation);
  const [commitMakePaymentNotRequired, isMakingPaymentNotRequired] =
    useMutation<pageOrganizationEntitlementPurchaseDetail_makeEntitlementPurchasePaymentNotRequiredMutation>(MakePaymentNotRequiredMutation);
  const purchase = data.entitlementPurchase;
  const linkedBookings = data.entitlementPurchase?.linkedBookings;

  if (!purchase)
    return (
      <RootShell>
        <PageHeaderPanel title="Credit entitlement purchase" description="This entitlement purchase could not be found." />
      </RootShell>
    );

  const customerLabel = purchase.customerName || 'Customer unavailable';
  const usedCredits = linkedBookings?.totalCount ?? 0;
  const freeCredits = Math.max(purchase.creditQuantity - usedCredits, 0);
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

  return (
    <RootShell>
      <PurchaseDetailPage
        title="Purchase details"
        purchaseType="Credit entitlement"
        customer={customerLabel}
        customerAvatar={<CustomerAvatar name={{ name: customerLabel }} size="small" />}
        headline={`${purchase.creditQuantity} credits`}
        status={purchase.lifecycleState}
        statusColor={purchase.lifecycleState === 'ACTIVE' ? 'success' : 'warning'}
        summary={[
          { label: 'Payment', value: purchase.paymentStatus },
          { label: 'Method', value: purchase.paymentMethod },
          { label: 'Credits left', value: `${freeCredits} of ${purchase.creditQuantity}` },
          { label: 'Validity', value: `${purchase.validityDays} days` },
        ]}
        payment={
          <StackColumn spacing={1} sx={{ mt: 2 }}>
            <BodyIconTypography label={`Amount: ${purchase.amount} ${purchase.currency}`} />
            {purchase.invoiceUrl ? (
              <InvoiceDownloadLinks invoices={[]} legacyInvoiceUrl={purchase.invoiceUrl} linkLabel="View invoice" emptyLabel="Invoice not available" size="body" />
            ) : null}
          </StackColumn>
        }
        actions={
          [
            ...(purchase.paymentMethod === 'BANK_TRANSFER' && purchase.paymentStatus === 'PENDING'
              ? [
                  { label: 'Confirm payment', onClick: confirmPayment, disabled: isPaymentActionInFlight },
                  { label: 'Reject payment', tone: 'destructive' as const, onClick: rejectPayment, disabled: isPaymentActionInFlight },
                  { label: 'Payment not required', onClick: makePaymentNotRequired, disabled: isPaymentActionInFlight },
                ]
              : []),
          ] satisfies PurchaseDetailAction[]
        }
        linkedBookings={(linkedBookings?.edges ?? []).map(({ node }) => ({
          id: node.id,
          title: formatBookingDateTime(node.from, node.until),
          meta: `${node.involvedLocations.map((location) => location.name).join(', ') || 'Location pending'} · ${node.bookingResources.map((item) => item.resource.name).join(', ') || 'Resources pending'}`,
          href: getOrganizationBookingBaseLink(undefined, organizationCustomDomain, node.id),
        }))}
        history={purchase.history.edges.map(({ node }) => ({
          title: node.name,
          meta: new Date(node.occurredAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' }),
          details: formatPurchaseHistoryEventDetails(node),
        }))}
      />
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
