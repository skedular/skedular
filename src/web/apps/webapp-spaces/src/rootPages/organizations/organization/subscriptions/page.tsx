import { DayPicker } from '@/components/datePickers';
import { getOrganizationPurchaseDetailLink, getOrganizationRefundBaseLink, getOrganizationSubscriptionBaseLink } from '@/components/links';
import { ListGridToggle } from '@/components/listGridToggle';
import { Loading } from '@/components/loading';
import {
  isSupportedMarketplaceBookingPaymentStatusForFilter,
  SupportedMarketplaceBookingPaymentStatusForFilter,
} from '@/components/marketplaceProductSubscription/marketplace-booking-payment-status';
import {
  SupportedMarketplaceBookingSubscriptionCancellationMode,
  SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  toSupportedMarketplaceBookingSubscriptionCancellationModeDetails,
} from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import { toMarketplaceBookingSubscriptionLifecycleDisplay } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-lifecycle';
import {
  isSupportedMarketplaceBookingSubscriptionStatusForFilter,
  SupportedMarketplaceBookingSubscriptionStatusForFilter,
} from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-status';
import SubscriptionCancellationSection from '@/components/marketplaceProductSubscription/subscription-cancellation-section';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { DefaultSelect } from '@/components/styled';
import {
  buildMarketplacePurchaseQueryVariables,
  formatMarketplacePurchaseDisplay,
  formatMarketplacePurchaseInactiveEvidence,
  getRelayErrorMessage,
  RelayError,
  toRootError,
  updateMarketplacePurchaseSearchParams,
  useIntegratedPlatform,
  useKnownParams,
} from '@skedular/shared';

import { RootShell } from '@/components/rootShell';
import type { pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation.graphql';
import type { pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation.graphql';
import type { pageOrganizationSubscriptions_rootQuery } from '@/queries/__generated__/pageOrganizationSubscriptions_rootQuery.graphql';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import LinearProgress from '@mui/material/LinearProgress';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import TextField from '@mui/material/TextField';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';

import {
  BodyIconTypography,
  CollectionToolbar,
  DefaultDialogTitle,
  defaultGridStyle,
  defaultPadding,
  LeadIconTypography,
  PageHeaderPanel,
  PushToRight,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';
import dayjs from 'dayjs';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useCallback, useEffect, useMemo, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form, FormSpy } from 'react-final-form';
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

const subscriptionCardSx: SxProps<Theme> = {
  ...surfaceSx,
  height: '100%',
};

const clickablePanelSx: SxProps<Theme> = {
  cursor: 'pointer',
  borderRadius: 2,
  transition: 'transform 120ms ease, background-color 120ms ease',
  '&:hover': {
    backgroundColor: (theme) => theme.palette.action.hover,
    transform: 'translateY(-1px)',
  },
};

const RootQuery = graphql`
  query pageOrganizationSubscriptions_rootQuery(
    $organizationCustomDomain: String!
    $statuses: [MarketplaceBookingSubscriptionStatus!]
    $paymentStatuses: [PaymentStatus!]
    $purchaseAfter: String
    $purchaseFirst: Int
    $purchaseSourceTypes: [MarketplacePurchaseSourceType!]
    $purchaseLifecycleStates: [MarketplacePurchaseLifecycleState!]
    $purchasePaymentStatuses: [PaymentStatus!]
    $purchaseActivityFrom: DateTime
    $purchaseActivityUntil: DateTime
    $purchaseOrderBy: [MarketplacePurchaseHistoryOrderInput!]
  ) {
    ...multipleChoicesMarketplaceBookingSubscriptionStatuses_query
    ...multipleChoicesMarketplaceBookingPaymentStatuses_query
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
    marketplaceBookingSubscriptions(
      first: 50
      where: { organizationCustomDomain: $organizationCustomDomain, statuses: $statuses, paymentStatuses: $paymentStatuses }
      orderBy: [{ field: NEXT_RENEWAL_AT, direction: ASCENDING }]
    ) {
      totalCount
      edges {
        node {
          id
          cancellationPolicyOverridden
          cancellationOverrideReason
          startedAt
          nextRenewalAt
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
          }
          marketplaceBooking {
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
        }
      }
    }
    marketplacePurchases(
      after: $purchaseAfter
      first: $purchaseFirst
      where: {
        organizationCustomDomain: $organizationCustomDomain
        sourceTypes: $purchaseSourceTypes
        lifecycleStates: $purchaseLifecycleStates
        paymentStatuses: $purchasePaymentStatuses
        activityFrom: $purchaseActivityFrom
        activityUntil: $purchaseActivityUntil
      }
      orderBy: $purchaseOrderBy
    ) {
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
          sourceId
          sourceType
          sourceTypeName
          lifecycleState
          lifecycleStateName
          renewalState
          renewalStateName
          purchasedAt
          activityAt
          bookingFrom
          bookingUntil
          paymentStatus
          paymentMethod
          productVersionId
          productTitle
          entitlementStatus
          creditQuantity
          grantedQuantity
          availableQuantity
          totalAmount
          currency
          customerId
          deletedByCustomerId
          cancellationReason
          refundId
          bookingId
          refund {
            id
            status {
              name
            }
            requestedAt
            lastProcessedAt
            refundAmount
            events {
              id
              occurredAt
              eventType {
                name
              }
            }
          }
          isDeleted
        }
      }
    }
  }
`;

type PendingCancellationConfirmation = {
  subscriptionId: string;
  productTitle: string;
  mode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails;
} | null;

type SubscriptionViewMode = 'card' | 'list';

type SubscriptionListRow = {
  id: string;
  productTitle: string;
  customerLabel: string;
  startedAtLabel: string;
  nextRenewalLabel: string;
  currentCycleLabel: string;
  renewalLabel: string;
  paymentStatusLabel: string;
  paymentMethodLabel: string;
  quantityLabel: string;
  statusLabel: string;
  statusColor: 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
  hasPendingPayment: boolean;
  refundId: string | null;
};

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSubscriptions_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onFiltersChange: (statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[], paymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[]) => void;
  isLoading: boolean;
  initialFormValues: {
    statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[];
    paymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[];
  };
  onPurchaseAfterChange: (cursor: string | undefined) => void;
  purchaseSourceType: string;
  purchaseLifecycleState: string;
  purchasePaymentStatus: string;
  purchaseActivityFrom: string;
  purchaseActivityUntil: string;
  onPurchaseFiltersChange: (sourceType: string, lifecycleState: string, paymentStatus: string, activityFrom: string, activityUntil: string) => void;
  purchaseSort: string;
  onPurchaseSortChange: (sort: string) => void;
};

const getCustomerDisplayName = (customer: { name?: string | null; givenName?: string | null; middleName?: string | null; familyName?: string | null }) => {
  const structuredName = [customer.givenName, customer.middleName, customer.familyName].filter(Boolean).join(' ').trim();
  return structuredName || customer.name || 'Customer';
};

const RootPage = ({
  queryReference,
  onReloadRequired,
  organizationCustomDomain,
  onFiltersChange,
  isLoading,
  initialFormValues,
  onPurchaseAfterChange,
  purchaseSourceType,
  purchaseLifecycleState,
  purchasePaymentStatus,
  purchaseActivityFrom,
  purchaseActivityUntil,
  onPurchaseFiltersChange,
  purchaseSort,
  onPurchaseSortChange,
}: Props) => {
  const prevFiltersRef = useRef({
    statuses: initialFormValues.statuses,
    paymentStatuses: initialFormValues.paymentStatuses,
  });
  const rootData = usePreloadedQuery<pageOrganizationSubscriptions_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState<PendingCancellationConfirmation>(null);
  const [cancellationOverrideReason, setCancellationOverrideReason] = useState('');
  const [viewMode, setViewMode] = useState<SubscriptionViewMode>('list');
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<HTMLElement | null>(null);
  const [selectedSubscriptionId, setSelectedSubscriptionId] = useState<string | null>(null);
  const [commitDeleteMarketplaceBookingSubscription, isDeleteMarketplaceBookingSubscriptionInFlight] =
    useMutation<pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation>(graphql`
      mutation pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
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
  const [commitConfirmRecurringBookingPayment] = useMutation<pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation>(graphql`
    mutation pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation($input: ConfirmRecurringBookingPaymentInput!) @raw_response_type {
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
  const [commitRejectRecurringBookingPayment] = useMutation<pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation>(graphql`
    mutation pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation($input: RejectRecurringBookingPaymentInput!) @raw_response_type {
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
  const [commitMakeRecurringBookingPaymentNotRequired] = useMutation<pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation>(graphql`
    mutation pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation($input: MakeRecurringBookingPaymentNotRequiredInput!) @raw_response_type {
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
  const [commitConfirmEntitlementPurchase] = useMutation<pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation>(graphql`
    mutation pageOrganizationSubscriptions_confirmEntitlementPurchaseMutation($input: ConfirmEntitlementPurchaseInput!) {
      confirmEntitlementPurchase(input: $input) {
        error
        purchase {
          id
          paymentStatus
          lifecycleState
        }
      }
    }
  `);
  const [commitRejectEntitlementPurchase] = useMutation<pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation>(graphql`
    mutation pageOrganizationSubscriptions_rejectEntitlementPurchaseMutation($input: RejectEntitlementPurchaseInput!) {
      rejectEntitlementPurchase(input: $input) {
        error
        purchase {
          id
          paymentStatus
          lifecycleState
        }
      }
    }
  `);
  const [commitMakeEntitlementPurchasePaymentNotRequired] = useMutation<pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation>(graphql`
    mutation pageOrganizationSubscriptions_makeEntitlementPurchasePaymentNotRequiredMutation($input: MakeEntitlementPurchasePaymentNotRequiredInput!) {
      makeEntitlementPurchasePaymentNotRequired(input: $input) {
        error
        purchase {
          id
          paymentStatus
          lifecycleState
        }
      }
    }
  `);

  const handleEntitlementPaymentAction = (sourceId: string, action: 'confirm' | 'reject' | 'not-required') => {
    if (action === 'reject' && !window.confirm('Reject this bank-transfer payment? The entitlement will not be granted.')) return;
    if (action === 'not-required' && !window.confirm('Mark this purchase as payment not required? This grants the entitlement without collecting payment.')) return;
    const input = { clientMutationId: uuid(), purchaseId: sourceId };
    const onCompleted = (response: { error?: string | null }) => {
      if (response.error) {
        toast.error(<NotificationContent content={response.error} />);
      } else {
        onReloadRequired();
      }
    };
    const onError = (error: Error) => {
      toast.error(<NotificationContent content={error.message} />, errorNotificationOptions);
    };
    if (action === 'confirm') {
      commitConfirmEntitlementPurchase({ variables: { input }, onCompleted: (response) => onCompleted(response.confirmEntitlementPurchase), onError });
    } else if (action === 'reject') {
      commitRejectEntitlementPurchase({ variables: { input }, onCompleted: (response) => onCompleted(response.rejectEntitlementPurchase), onError });
    } else {
      commitMakeEntitlementPurchasePaymentNotRequired({
        variables: { input },
        onCompleted: (response) => onCompleted(response.makeEntitlementPurchasePaymentNotRequired),
        onError,
      });
    }
  };

  const subscriptions = useMemo(
    () => rootData.marketplaceBookingSubscriptions.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item),
    [rootData.marketplaceBookingSubscriptions.edges],
  );
  const immediateCancellationMode = useMemo((): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => {
    const mode = rootData.marketplaceBookingSubscriptionCancellationModes.find((item) => item.type === 'IMMEDIATE');

    return mode ? toSupportedMarketplaceBookingSubscriptionCancellationModeDetails(mode.type, mode.name) : null;
  }, [rootData.marketplaceBookingSubscriptionCancellationModes]);
  const atPeriodEndCancellationMode = useMemo((): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => {
    const mode = rootData.marketplaceBookingSubscriptionCancellationModes.find((item) => item.type === 'AT_PERIOD_END');

    return mode ? toSupportedMarketplaceBookingSubscriptionCancellationModeDetails(mode.type, mode.name) : null;
  }, [rootData.marketplaceBookingSubscriptionCancellationModes]);
  const filteredSubscriptions = subscriptions;
  const selectedSubscription = useMemo(() => filteredSubscriptions.find((item) => item.id === selectedSubscriptionId) ?? null, [filteredSubscriptions, selectedSubscriptionId]);
  const subscriptionListRows = useMemo<SubscriptionListRow[]>(
    () =>
      filteredSubscriptions.map((subscription) => {
        const sortedRecurringBookings = [...subscription.recurringBookings].sort((left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime());
        const currentCycle = sortedRecurringBookings[sortedRecurringBookings.length - 1] ?? null;
        const lifecycleDisplay = toMarketplaceBookingSubscriptionLifecycleDisplay({
          autoRenew: subscription.autoRenew,
          cancelAtPeriodEnd: subscription.cancelAtPeriodEnd,
          isCancelled: subscription.status.type === 'CANCELLED',
          fallbackActiveLabel: subscription.status.name,
        });

        return {
          id: subscription.id,
          productTitle: subscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription',
          customerLabel: subscription.involvedCustomers.length > 0 ? getCustomerDisplayName(subscription.involvedCustomers[0]) : 'Customer unavailable',
          startedAtLabel: new Date(subscription.startedAt).toLocaleDateString(),
          nextRenewalLabel: subscription.nextRenewalAt ? new Date(subscription.nextRenewalAt).toLocaleDateString() : 'Not scheduled',
          currentCycleLabel: currentCycle
            ? `${new Date(currentCycle.startDate).toLocaleDateString()} - ${currentCycle.endDate ? new Date(currentCycle.endDate).toLocaleDateString() : 'Open ended'}`
            : 'No billing period yet',
          renewalLabel: lifecycleDisplay.renewalLabel,
          paymentStatusLabel: subscription.status.type === 'CANCELLED' ? lifecycleDisplay.statusLabel : subscription.marketplaceBooking.paymentStatus.name,
          paymentMethodLabel: subscription.marketplaceBooking.paymentMethod.name ?? 'Not set',
          quantityLabel: `${subscription.marketplaceBooking.quantity}`,
          statusLabel: lifecycleDisplay.statusLabel,
          statusColor: lifecycleDisplay.statusColor,
          hasPendingPayment: sortedRecurringBookings.some((item) => item.marketplaceBooking?.paymentStatus.type === 'PENDING'),
          refundId: subscription.refund?.id ?? null,
        };
      }),
    [filteredSubscriptions],
  );

  const handleOpenSubscriptionClick = useCallback(
    (subscriptionId: string) => {
      router.push(getOrganizationSubscriptionBaseLink(integratedPlatform, organizationCustomDomain, subscriptionId));
    },
    [integratedPlatform, organizationCustomDomain, router],
  );

  const handleOpenMoreActionsClick = useCallback((subscriptionId: string, event: React.MouseEvent<HTMLElement>) => {
    setSelectedSubscriptionId(subscriptionId);
    setMoreActionsAnchorEl(event.currentTarget);
  }, []);

  const handleCloseMoreActions = () => {
    setMoreActionsAnchorEl(null);
    setSelectedSubscriptionId(null);
  };

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
  const handleRequestImmediateCancellationClick = (
    subscriptionId: string,
    productTitle: string,
    cancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  ) => {
    setPendingCancellationConfirmation({
      subscriptionId,
      productTitle,
      mode: cancellationMode,
    });
    setCancellationOverrideReason('');
  };
  const handleCancelImmediateCancellationClick = () => {
    setPendingCancellationConfirmation(null);
  };
  const handleConfirmImmediateCancellationClick = () => {
    if (!pendingCancellationConfirmation) {
      return;
    }

    handleDeleteMarketplaceBookingSubscriptionClick(
      pendingCancellationConfirmation.subscriptionId,
      pendingCancellationConfirmation.productTitle,
      pendingCancellationConfirmation.mode.type,
      cancellationOverrideReason.trim(),
    );
    setPendingCancellationConfirmation(null);
  };

  const handleQuickConfirmPaymentClick = (recurringBookingId: string, cycleLabel: string) => {
    handleCloseMoreActions();
    handleConfirmRecurringBookingPaymentClick(recurringBookingId, cycleLabel);
  };

  const handleQuickRejectPaymentClick = (recurringBookingId: string, cycleLabel: string) => {
    handleCloseMoreActions();
    handleRejectRecurringBookingPaymentClick(recurringBookingId, cycleLabel);
  };

  const handleQuickMarkPaymentNotRequiredClick = (recurringBookingId: string, cycleLabel: string) => {
    handleCloseMoreActions();
    handleMakeRecurringBookingPaymentNotRequiredClick(recurringBookingId, cycleLabel);
  };
  const subscriptionListColumns = useMemo<GridColDef<SubscriptionListRow>[]>(
    () => [
      {
        field: 'productTitle',
        headerName: 'Subscription',
        flex: 1.2,
        minWidth: 220,
        renderCell: (params) => (
          <StackColumn spacing={0.35} sx={{ py: 1 }}>
            <SubtitleIconTypography label={params.row.productTitle} />
            <SmallIconTypography label={params.row.customerLabel} sx={{ opacity: 0.8 }} />
          </StackColumn>
        ),
      },
      {
        field: 'statusLabel',
        headerName: 'Status',
        minWidth: 160,
        renderCell: (params) => <Chip size="small" label={params.row.statusLabel} color={params.row.statusColor} variant="outlined" sx={{ mt: 1 }} />,
      },
      {
        field: 'paymentStatusLabel',
        headerName: 'Payment',
        minWidth: 150,
      },
      {
        field: 'currentCycleLabel',
        headerName: 'Current period',
        flex: 1,
        minWidth: 220,
      },
      {
        field: 'nextRenewalLabel',
        headerName: 'Next renewal',
        minWidth: 150,
      },
      {
        field: 'paymentMethodLabel',
        headerName: 'Method',
        minWidth: 150,
      },
      {
        field: 'quantityLabel',
        headerName: 'Qty',
        minWidth: 90,
      },
      {
        field: 'refundId',
        headerName: 'Refund',
        minWidth: 130,
        sortable: false,
        renderCell: (params) =>
          params.row.refundId ? (
            <Button
              size="small"
              variant="text"
              onClick={(event) => {
                event.stopPropagation();
                router.push(getOrganizationRefundBaseLink(integratedPlatform, organizationCustomDomain, params.row.refundId!));
              }}
            >
              View refund
            </Button>
          ) : null,
      },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        disableColumnMenu: true,
        minWidth: 170,
        renderCell: (params) => (
          <StackRow sx={{ py: 0.5, alignItems: 'center', gap: 0.25 }}>
            <Button
              variant="text"
              size="small"
              sx={{ textTransform: 'none' }}
              onClick={(event) => {
                event.stopPropagation();
                handleOpenSubscriptionClick(params.row.id);
              }}
            >
              Open
            </Button>
            <IconButton
              size="small"
              onClick={(event) => {
                event.stopPropagation();
                handleOpenMoreActionsClick(params.row.id, event);
              }}
              aria-label="Open subscription actions"
            >
              <MoreVertIcon fontSize="small" />
            </IconButton>
          </StackRow>
        ),
      },
    ],
    [handleOpenMoreActionsClick, handleOpenSubscriptionClick, integratedPlatform, organizationCustomDomain, router],
  );

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

  const pageToolbar = (
    <CollectionToolbar
      filters={
        <Form onSubmit={() => {}} initialValues={initialFormValues} subscription={{}}>
          {() => (
            <>
              <FormSpy
                subscription={{ values: true }}
                onChange={({ values }) => {
                  const newStatuses = (((values ?? {}).statuses as string[]) ?? []).filter(isSupportedMarketplaceBookingSubscriptionStatusForFilter);
                  const newPaymentStatuses = (((values ?? {}).paymentStatuses as string[]) ?? []).filter(isSupportedMarketplaceBookingPaymentStatusForFilter);
                  const prev = prevFiltersRef.current;
                  if (
                    newStatuses.length !== prev.statuses.length ||
                    newStatuses.some((s, i) => s !== prev.statuses[i]) ||
                    newPaymentStatuses.length !== prev.paymentStatuses.length ||
                    newPaymentStatuses.some((s, i) => s !== prev.paymentStatuses[i])
                  ) {
                    prevFiltersRef.current = {
                      statuses: newStatuses,
                      paymentStatuses: newPaymentStatuses,
                    };
                    onFiltersChange(newStatuses, newPaymentStatuses);
                  }
                }}
              />
              <Box
                sx={{
                  display: 'grid',
                  gridTemplateColumns: {
                    xs: 'minmax(0, 1fr)',
                    sm: 'repeat(2, minmax(0, 1fr))',
                    lg: 'repeat(3, minmax(0, 1fr))',
                  },
                  gap: 1,
                  width: '100%',
                  '& .MuiInputBase-root': { width: '100%' },
                }}
              >
                <DefaultSelect
                  displayEmpty
                  size="small"
                  value={purchasePaymentStatus}
                  onChange={(event) =>
                    onPurchaseFiltersChange(purchaseSourceType, purchaseLifecycleState, event.target.value as string, purchaseActivityFrom, purchaseActivityUntil)
                  }
                  sx={{ minWidth: 260 }}
                  renderValue={() => (
                    <StackRow>
                      <LeadIconTypography label="Payment status" />
                      <Divider orientation="vertical" flexItem />
                      <PushToRight />
                      <SmallIconTypography label={purchasePaymentStatus ? purchasePaymentStatus.replace('_', ' ') : 'All payments'} />
                    </StackRow>
                  )}
                  aria-label="Payment status"
                >
                  <MenuItem value="">All payments</MenuItem>
                  <MenuItem value="CONFIRMED">Confirmed</MenuItem>
                  <MenuItem value="PENDING">Pending</MenuItem>
                  <MenuItem value="REJECTED">Rejected</MenuItem>
                  <MenuItem value="EXPIRED">Expired</MenuItem>
                  <MenuItem value="NOT_SET">Not set</MenuItem>
                </DefaultSelect>
                <DefaultSelect
                  displayEmpty
                  size="small"
                  value={purchaseSourceType}
                  onChange={(event) =>
                    onPurchaseFiltersChange(event.target.value as string, purchaseLifecycleState, purchasePaymentStatus, purchaseActivityFrom, purchaseActivityUntil)
                  }
                  sx={{ minWidth: 230 }}
                  renderValue={() => (
                    <StackRow>
                      <LeadIconTypography label="Purchase type" />
                      <Divider orientation="vertical" flexItem />
                      <PushToRight />
                      <SmallIconTypography label={purchaseSourceType === 'BOOKING' ? 'One-time booking' : purchaseSourceType === 'SUBSCRIPTION' ? 'Subscription' : 'All types'} />
                    </StackRow>
                  )}
                  aria-label="Purchase type"
                >
                  <MenuItem value="">All types</MenuItem>
                  <MenuItem value="BOOKING">One-time booking</MenuItem>
                  <MenuItem value="SUBSCRIPTION">Subscription</MenuItem>
                  <MenuItem value="ENTITLEMENT">Credit entitlement</MenuItem>
                </DefaultSelect>
                <DefaultSelect
                  displayEmpty
                  size="small"
                  value={purchaseLifecycleState}
                  onChange={(event) =>
                    onPurchaseFiltersChange(purchaseSourceType, event.target.value as string, purchasePaymentStatus, purchaseActivityFrom, purchaseActivityUntil)
                  }
                  sx={{ minWidth: 220 }}
                  renderValue={() => (
                    <StackRow>
                      <LeadIconTypography label="Lifecycle" />
                      <Divider orientation="vertical" flexItem />
                      <PushToRight />
                      <SmallIconTypography label={purchaseLifecycleState ? purchaseLifecycleState.replaceAll('_', ' ') : 'All states'} />
                    </StackRow>
                  )}
                  aria-label="Purchase lifecycle"
                >
                  <MenuItem value="">All lifecycle states</MenuItem>
                  <MenuItem value="ACTIVE">Active</MenuItem>
                  <MenuItem value="CANCELLED">Canceled</MenuItem>
                  <MenuItem value="DELETED">Deleted</MenuItem>
                  <MenuItem value="EXPIRED">Expired</MenuItem>
                  <MenuItem value="PAYMENT_FAILED">Payment failed</MenuItem>
                  <MenuItem value="PENDING">Pending</MenuItem>
                </DefaultSelect>
                <DayPicker
                  label="Activity from"
                  allowEmpty
                  value={purchaseActivityFrom ? dayjs(purchaseActivityFrom) : null}
                  onDateChanged={(date) =>
                    onPurchaseFiltersChange(purchaseSourceType, purchaseLifecycleState, purchasePaymentStatus, date.format('YYYY-MM-DD'), purchaseActivityUntil)
                  }
                />
                <DayPicker
                  label="Activity to"
                  allowEmpty
                  value={purchaseActivityUntil ? dayjs(purchaseActivityUntil) : null}
                  onDateChanged={(date) =>
                    onPurchaseFiltersChange(purchaseSourceType, purchaseLifecycleState, purchasePaymentStatus, purchaseActivityFrom, date.format('YYYY-MM-DD'))
                  }
                />
                <StackRow sx={{ alignItems: 'center', gap: 1, minWidth: 0 }}>
                  <DefaultSelect
                    displayEmpty
                    size="small"
                    value={purchaseSort}
                    onChange={(event) => onPurchaseSortChange(event.target.value as string)}
                    sx={{ flex: 1, minWidth: 0 }}
                    renderValue={() => (
                      <StackRow>
                        <LeadIconTypography label="Sort" />
                        <Divider orientation="vertical" flexItem />
                        <PushToRight />
                        <SmallIconTypography
                          label={
                            purchaseSort === 'ACTIVITY_ASC'
                              ? 'Oldest activity'
                              : purchaseSort === 'PURCHASED_DESC'
                                ? 'Newest purchase'
                                : purchaseSort === 'BOOKING_FROM_ASC'
                                  ? 'Booking start'
                                  : purchaseSort === 'BOOKING_UNTIL_ASC'
                                    ? 'Booking end'
                                    : 'Newest activity'
                          }
                        />
                      </StackRow>
                    )}
                    aria-label="Sort purchases"
                  >
                    <MenuItem value="ACTIVITY_DESC">Newest activity</MenuItem>
                    <MenuItem value="ACTIVITY_ASC">Oldest activity</MenuItem>
                    <MenuItem value="PURCHASED_DESC">Newest purchase</MenuItem>
                    <MenuItem value="BOOKING_FROM_ASC">Booking start</MenuItem>
                    <MenuItem value="BOOKING_UNTIL_ASC">Booking end</MenuItem>
                  </DefaultSelect>
                  <ListGridToggle defaultValue={viewMode === 'list' ? 'list' : 'grid'} onChange={(view) => setViewMode(view === 'list' ? 'list' : 'card')} />
                </StackRow>
              </Box>
            </>
          )}
        </Form>
      }
      actions={null}
    />
  );

  return (
    <RootShell>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
        <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pb: defaultPadding }} spacing={2}>
          <PageHeaderPanel
            title="Marketplace purchases"
            description="Review retained marketplace purchases, including subscriptions and one-time bookings, manage payments and refunds, and inspect their history."
          />

          {pageToolbar}

          {rootData.marketplacePurchases.totalCount > 0 && (
            <Box sx={viewMode === 'card' ? {} : { ...surfaceSx, p: 2 }}>
              <SubtitleIconTypography label={`All marketplace purchases (${rootData.marketplacePurchases.totalCount})`} sx={viewMode === 'card' ? { px: 0 } : undefined} />
              <StackColumn
                spacing={1}
                sx={{
                  mt: 1,
                  display: viewMode === 'card' ? 'grid' : 'flex',
                  gridTemplateColumns: viewMode === 'card' ? 'repeat(auto-fit, minmax(320px, 1fr))' : undefined,
                  gridAutoRows: viewMode === 'card' ? '1fr' : undefined,
                  gap: viewMode === 'card' ? 2 : 1,
                }}
              >
                {viewMode === 'list' ? (
                  <Box
                    sx={{
                      display: { xs: 'none', md: 'grid' },
                      gridTemplateColumns: 'minmax(260px, 1.2fr) minmax(260px, 1.3fr) minmax(160px, 0.6fr)',
                      gap: 2,
                      px: 1.5,
                      py: 1,
                      color: 'text.secondary',
                      borderBottom: 1,
                      borderColor: 'divider',
                    }}
                  >
                    <SmallIconTypography label="Purchase" />
                    <SmallIconTypography label="Activity and payment" />
                    <SmallIconTypography label="Amount" />
                  </Box>
                ) : null}
                {rootData.marketplacePurchases.edges.map(({ node }) => (
                  <Box
                    key={node.id}
                    onClick={() => {
                      const destination = getOrganizationPurchaseDetailLink(integratedPlatform, organizationCustomDomain, node);

                      if (destination) {
                        router.push(destination);
                      }
                    }}
                    sx={{
                      cursor: 'pointer',
                      display: viewMode === 'list' ? { xs: 'flex', md: 'grid' } : 'flex',
                      gridTemplateColumns:
                        viewMode === 'list'
                          ? {
                              md: 'minmax(260px, 1.2fr) minmax(260px, 1.3fr) minmax(160px, 0.6fr)',
                            }
                          : undefined,
                      justifyContent: 'space-between',
                      gap: 2,
                      alignItems: 'center',
                      p: viewMode === 'card' ? 2 : 1.25,
                      minHeight: viewMode === 'card' ? 220 : undefined,
                      border: 1,
                      borderColor: 'divider',
                      borderRadius: 2,
                      backgroundColor: viewMode === 'card' ? 'background.paper' : 'transparent',
                      '&:hover': {
                        backgroundColor: 'action.hover',
                        borderColor: 'primary.main',
                      },
                    }}
                  >
                    <StackColumn spacing={0.75} sx={{ minWidth: 0 }}>
                      <StackRow
                        sx={{
                          alignItems: 'center',
                          flexWrap: 'wrap',
                          gap: 0.75,
                        }}
                      >
                        <BodyIconTypography label={formatMarketplacePurchaseDisplay(node).product} />
                        <Chip size="small" label={node.sourceTypeName} variant="outlined" />
                        <Chip size="small" label={node.lifecycleStateName} color={node.isDeleted ? 'error' : 'default'} variant="outlined" />
                      </StackRow>
                      <SmallIconTypography label={`Activity ${new Date(node.activityAt).toLocaleString()}`} sx={{ opacity: 0.72 }} />
                      <SmallIconTypography
                        label={
                          node.bookingFrom
                            ? `Booking ${new Date(node.bookingFrom).toLocaleString()}–${node.bookingUntil ? new Date(node.bookingUntil).toLocaleString() : 'Open ended'}`
                            : node.renewalStateName
                        }
                        sx={{ opacity: 0.62 }}
                      />
                      {node.sourceType === 'ENTITLEMENT' ? (
                        <SmallIconTypography
                          label={`${node.availableQuantity} of ${node.grantedQuantity} credits available · ${node.entitlementStatus ?? node.lifecycleStateName}`}
                          sx={{ opacity: 0.72 }}
                        />
                      ) : null}
                      {viewMode === 'card' ? (
                        <SmallIconTypography
                          label={node.sourceType === 'ENTITLEMENT' ? 'Credit entitlement' : `Open ${node.sourceType === 'SUBSCRIPTION' ? 'subscription' : 'booking'} details`}
                          sx={{ mt: 'auto', color: 'primary.main' }}
                        />
                      ) : null}
                    </StackColumn>
                    <StackColumn spacing={0.75} sx={{ minWidth: 0 }}>
                      <BodyIconTypography label={`Payment: ${node.paymentStatus}`} />
                      <SmallIconTypography label={node.renewalStateName} sx={{ opacity: 0.72 }} />
                      {formatMarketplacePurchaseInactiveEvidence(node).lifecycle ? (
                        <SmallIconTypography
                          label={`${formatMarketplacePurchaseInactiveEvidence(node).lifecycle}${formatMarketplacePurchaseInactiveEvidence(node).actor ? ` by ${formatMarketplacePurchaseInactiveEvidence(node).actor}` : ''}${formatMarketplacePurchaseInactiveEvidence(node).reason ? ` · Reason: ${formatMarketplacePurchaseInactiveEvidence(node).reason}` : ''}`}
                          sx={{ opacity: 0.72 }}
                        />
                      ) : null}
                      {node.refund ? <SmallIconTypography label={`Refund: ${node.refund.status.name}`} sx={{ opacity: 0.72 }} /> : null}
                      {node.refundId ? (
                        <Button
                          size="small"
                          onClick={(event) => {
                            event.stopPropagation();
                            router.push(getOrganizationRefundBaseLink(integratedPlatform, organizationCustomDomain, node.refundId!));
                          }}
                        >
                          View refund
                        </Button>
                      ) : null}
                      {node.sourceType === 'ENTITLEMENT' && node.paymentMethod === 'BANK_TRANSFER' && node.paymentStatus === 'PENDING' ? (
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }} onClick={(event) => event.stopPropagation()}>
                          <Button size="small" onClick={() => handleEntitlementPaymentAction(node.sourceId, 'confirm')}>
                            Confirm
                          </Button>
                          <Button size="small" color="error" onClick={() => handleEntitlementPaymentAction(node.sourceId, 'reject')}>
                            Reject
                          </Button>
                          <Button size="small" onClick={() => handleEntitlementPaymentAction(node.sourceId, 'not-required')}>
                            Waive payment
                          </Button>
                        </Box>
                      ) : null}
                    </StackColumn>
                    <StackColumn
                      spacing={0.5}
                      sx={{
                        alignItems: viewMode === 'list' ? { xs: 'flex-end', md: 'flex-start' } : 'flex-end',
                        justifyContent: 'space-between',
                        height: '100%',
                      }}
                    >
                      <SmallIconTypography label={`${node.totalAmount ?? 'Amount unavailable'} ${node.currency ?? ''}`} sx={{ opacity: 0.78 }} />
                      {viewMode === 'list' ? (
                        <SmallIconTypography
                          label={node.sourceType === 'ENTITLEMENT' ? 'Credit entitlement' : `Open ${node.sourceType === 'SUBSCRIPTION' ? 'subscription' : 'booking'}`}
                          sx={{ color: 'primary.main' }}
                        />
                      ) : null}
                    </StackColumn>
                  </Box>
                ))}
              </StackColumn>
              <StackRow sx={{ mt: 2, justifyContent: 'flex-end' }}>
                <Button disabled={!rootData.marketplacePurchases.pageInfo.hasPreviousPage} onClick={() => onPurchaseAfterChange(undefined)}>
                  Previous
                </Button>
                <Button
                  disabled={!rootData.marketplacePurchases.pageInfo.hasNextPage}
                  onClick={() => onPurchaseAfterChange(rootData.marketplacePurchases.pageInfo.endCursor ?? undefined)}
                >
                  Next
                </Button>
              </StackRow>
            </Box>
          )}

          {rootData.marketplacePurchases.totalCount === 0 && !isLoading ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 5, textAlign: 'center' }}>
              <SubtitleIconTypography label="No purchases found" sx={{ justifyContent: 'center' }} />
              <SmallIconTypography label="Try changing the purchase type, lifecycle, payment status, or activity date filters." sx={{ mt: 1, opacity: 0.72 }} />
            </Box>
          ) : null}

          {isLoading && <LinearProgress />}

          {false &&
            (!rootData.organizationBookingPermissions.canModifyPaymentMethod ? (
              <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
                <SmallIconTypography label="You do not have permission to manage subscription payments for this organization." />
              </Box>
            ) : subscriptions.length === 0 ? (
              <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
                <SmallIconTypography label="This organization does not have any marketplace purchases yet." sx={{ opacity: 0.78 }} />
              </Box>
            ) : filteredSubscriptions.length === 0 ? (
              <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
                <SmallIconTypography label="No marketplace purchases match the current filters." sx={{ opacity: 0.78 }} />
              </Box>
            ) : viewMode === 'list' ? (
              <Box sx={{ ...surfaceSx, p: 1.5 }}>
                <DataGrid
                  rows={subscriptionListRows}
                  columns={subscriptionListColumns}
                  disableRowSelectionOnClick
                  ignoreDiacritics
                  hideFooter
                  onRowClick={(params) => handleOpenSubscriptionClick(params.row.id)}
                  getRowHeight={() => 'auto'}
                  rowSpacingType="margin"
                  getRowSpacing={() => ({ top: 4, bottom: 4 })}
                  sx={{
                    ...defaultGridStyle,
                    '& .MuiDataGrid-row': {
                      cursor: 'pointer',
                    },
                  }}
                  localeText={{ noRowsLabel: 'No subscriptions found' }}
                  initialState={{
                    sorting: {
                      sortModel: [{ field: 'nextRenewalLabel', sort: 'asc' }],
                    },
                  }}
                />
              </Box>
            ) : (
              <Grid container spacing={2}>
                {filteredSubscriptions.map((subscription) => {
                  const sortedRecurringBookings = [...subscription.recurringBookings].sort(
                    (left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime(),
                  );
                  const lifecycleDisplay = toMarketplaceBookingSubscriptionLifecycleDisplay({
                    autoRenew: subscription.autoRenew,
                    cancelAtPeriodEnd: subscription.cancelAtPeriodEnd,
                    isCancelled: subscription.status.type === 'CANCELLED',
                    fallbackActiveLabel: subscription.status.name,
                  });
                  const productTitle = subscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription';
                  const customerLabel = subscription.involvedCustomers.length > 0 ? getCustomerDisplayName(subscription.involvedCustomers[0]) : 'Customer unavailable';

                  return (
                    <Grid key={subscription.id} size={{ xs: 12, lg: 6 }}>
                      <Card sx={subscriptionCardSx}>
                        <CardContent sx={{ p: 2, height: '100%' }}>
                          <StackColumn spacing={2} sx={{ height: '100%' }}>
                            <Box onClick={() => handleOpenSubscriptionClick(subscription.id)} sx={{ ...clickablePanelSx, p: 1 }}>
                              <StackRow
                                sx={{
                                  alignItems: 'flex-start',
                                  flexWrap: 'wrap',
                                  gap: 1,
                                }}
                              >
                                <StackColumn spacing={0.5} sx={{ minWidth: 0 }}>
                                  <SubtitleIconTypography label={productTitle} />
                                  <SmallIconTypography label={customerLabel} sx={{ opacity: 0.82 }} />
                                  <SmallIconTypography
                                    label={`Started ${new Date(subscription.startedAt).toLocaleDateString()}${subscription.nextRenewalAt ? ` • Next renewal ${new Date(subscription.nextRenewalAt).toLocaleDateString()}` : ''}`}
                                    sx={{ opacity: 0.72 }}
                                  />
                                </StackColumn>
                                <PushToRight />
                                <Chip label={lifecycleDisplay.statusLabel} color={lifecycleDisplay.statusColor} variant="outlined" />
                              </StackRow>

                              <Divider sx={{ my: 1.5 }} />

                              <StackColumn spacing={0.75}>
                                <BodyIconTypography label={`Renewal: ${lifecycleDisplay.renewalLabel}`} />
                                <SmallIconTypography
                                  label={`Current payment: ${subscription.status.type === 'CANCELLED' ? lifecycleDisplay.statusLabel : subscription.marketplaceBooking.paymentStatus.name} • Payment method: ${subscription.marketplaceBooking.paymentMethod.name ?? 'Not set'} • Quantity: ${subscription.marketplaceBooking.quantity}`}
                                  sx={{ opacity: 0.78 }}
                                />
                                {subscription.cancellationPolicyOverridden ? (
                                  <SmallIconTypography label={`Cancellation reason: ${subscription.cancellationOverrideReason ?? 'Policy overridden'}`} sx={{ opacity: 0.78 }} />
                                ) : null}
                                <SmallIconTypography label="Open details and manage this subscription" sx={{ opacity: 0.72 }} />
                              </StackColumn>
                            </Box>

                            {subscription.status.type === 'ACTIVE' ? (
                              <StackColumn spacing={2}>
                                <Divider />
                                <SubscriptionCancellationSection
                                  cancelAtPeriodEnd={subscription.cancelAtPeriodEnd}
                                  hasConfirmedPayment={subscription.marketplaceBooking.paymentStatus.type === 'CONFIRMED'}
                                  isInFlight={isDeleteMarketplaceBookingSubscriptionInFlight}
                                  immediateCancellationMode={immediateCancellationMode}
                                  atPeriodEndCancellationMode={subscription.autoRenew ? atPeriodEndCancellationMode : null}
                                  onImmediateCancellationClick={() =>
                                    immediateCancellationMode ? handleRequestImmediateCancellationClick(subscription.id, productTitle, immediateCancellationMode) : undefined
                                  }
                                  onAtPeriodEndCancellationClick={() =>
                                    atPeriodEndCancellationMode
                                      ? handleDeleteMarketplaceBookingSubscriptionClick(subscription.id, productTitle, atPeriodEndCancellationMode.type)
                                      : undefined
                                  }
                                />
                              </StackColumn>
                            ) : null}

                            {subscription.refund ? (
                              <StackColumn spacing={1}>
                                <MarketplaceRefundAdminPanel entityLabel={`${productTitle} for ${customerLabel}`} refund={subscription.refund} />
                                <Button
                                  size="small"
                                  variant="text"
                                  onClick={() => router.push(getOrganizationRefundBaseLink(integratedPlatform, organizationCustomDomain, subscription.refund!.id))}
                                  sx={{ alignSelf: 'flex-start' }}
                                >
                                  View refund details
                                </Button>
                              </StackColumn>
                            ) : null}

                            <Divider />

                            <StackColumn spacing={1}>
                              <BodyIconTypography label="Billing periods" />
                              {sortedRecurringBookings.length > 0 ? (
                                <StackColumn spacing={0}>
                                  {sortedRecurringBookings.map((recurringBooking, recurringBookingIndex) => {
                                    const cycleLabel = `${new Date(recurringBooking.startDate).toLocaleDateString()} - ${
                                      recurringBooking.endDate ? new Date(recurringBooking.endDate).toLocaleDateString() : 'Open ended'
                                    }`;
                                    const cycleMarketplaceBooking = recurringBooking.marketplaceBooking;

                                    return (
                                      <StackColumn key={recurringBooking.id} spacing={0}>
                                        <Box sx={{ py: 1.25 }}>
                                          <StackColumn spacing={1}>
                                            <StackColumn spacing={0.35}>
                                              <BodyIconTypography label={cycleLabel} />
                                              <SmallIconTypography
                                                label={`Payment: ${cycleMarketplaceBooking?.paymentStatus.name ?? 'Not set'} • Payment method: ${cycleMarketplaceBooking?.paymentMethod.name ?? 'Not set'} • Quantity: ${cycleMarketplaceBooking?.quantity ?? subscription.marketplaceBooking.quantity}`}
                                                sx={{ opacity: 0.78 }}
                                              />
                                            </StackColumn>

                                            <StackRow
                                              sx={{
                                                alignItems: 'center',
                                                flexWrap: 'wrap',
                                                gap: 1,
                                              }}
                                            >
                                              {cycleMarketplaceBooking?.invoiceUrl ? (
                                                <Button
                                                  variant="text"
                                                  size="small"
                                                  href={cycleMarketplaceBooking.invoiceUrl}
                                                  target="_blank"
                                                  rel="noreferrer"
                                                  sx={{
                                                    textTransform: 'none',
                                                  }}
                                                >
                                                  View invoice
                                                </Button>
                                              ) : null}

                                              {cycleMarketplaceBooking?.paymentStatus.type === 'PENDING' ? (
                                                <StackRow
                                                  sx={{
                                                    flexWrap: 'wrap',
                                                    gap: 1,
                                                  }}
                                                >
                                                  <Button
                                                    variant="contained"
                                                    size="small"
                                                    sx={{
                                                      textTransform: 'none',
                                                      color: 'white',
                                                    }}
                                                    onClick={() => handleConfirmRecurringBookingPaymentClick(recurringBooking.id, cycleLabel)}
                                                  >
                                                    Confirm Payment
                                                  </Button>
                                                  <Button
                                                    variant="outlined"
                                                    color="error"
                                                    size="small"
                                                    sx={{
                                                      textTransform: 'none',
                                                    }}
                                                    onClick={() => handleRejectRecurringBookingPaymentClick(recurringBooking.id, cycleLabel)}
                                                  >
                                                    Reject Payment
                                                  </Button>
                                                  <Button
                                                    variant="text"
                                                    size="small"
                                                    sx={{
                                                      textTransform: 'none',
                                                    }}
                                                    onClick={() => handleMakeRecurringBookingPaymentNotRequiredClick(recurringBooking.id, cycleLabel)}
                                                  >
                                                    Payment Not Required
                                                  </Button>
                                                </StackRow>
                                              ) : null}
                                            </StackRow>
                                          </StackColumn>
                                        </Box>
                                        {recurringBookingIndex < sortedRecurringBookings.length - 1 ? <Divider /> : null}
                                      </StackColumn>
                                    );
                                  })}
                                </StackColumn>
                              ) : (
                                <Box sx={{ py: 1.25 }}>
                                  <SmallIconTypography label="No billing periods have been created for this subscription yet." sx={{ opacity: 0.72 }} />
                                </Box>
                              )}
                            </StackColumn>
                          </StackColumn>
                        </CardContent>
                      </Card>
                    </Grid>
                  );
                })}
              </Grid>
            ))}
        </StackColumn>
      </Box>

      <Menu anchorEl={moreActionsAnchorEl} open={!!moreActionsAnchorEl} onClose={handleCloseMoreActions}>
        {selectedSubscription ? (
          <>
            <MenuItem
              onClick={() => {
                handleCloseMoreActions();
                handleOpenSubscriptionClick(selectedSubscription.id);
              }}
            >
              Open details
            </MenuItem>
            {[...selectedSubscription.recurringBookings]
              .sort((left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime())
              .filter((item) => item.marketplaceBooking?.paymentStatus.type === 'PENDING')
              .slice(0, 1)
              .map((recurringBooking) => {
                const cycleLabel = `${new Date(recurringBooking.startDate).toLocaleDateString()} - ${
                  recurringBooking.endDate ? new Date(recurringBooking.endDate).toLocaleDateString() : 'Open ended'
                }`;

                return [
                  <MenuItem key={`${recurringBooking.id}-confirm`} onClick={() => handleQuickConfirmPaymentClick(recurringBooking.id, cycleLabel)}>
                    Confirm payment
                  </MenuItem>,
                  <MenuItem key={`${recurringBooking.id}-reject`} onClick={() => handleQuickRejectPaymentClick(recurringBooking.id, cycleLabel)}>
                    Reject payment
                  </MenuItem>,
                  <MenuItem key={`${recurringBooking.id}-not-required`} onClick={() => handleQuickMarkPaymentNotRequiredClick(recurringBooking.id, cycleLabel)}>
                    Payment not required
                  </MenuItem>,
                ];
              })}
            {selectedSubscription.status.type === 'ACTIVE' && immediateCancellationMode ? (
              <MenuItem
                onClick={() => {
                  handleCloseMoreActions();
                  handleRequestImmediateCancellationClick(
                    selectedSubscription.id,
                    selectedSubscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription',
                    immediateCancellationMode,
                  );
                }}
              >
                Cancel subscription now
              </MenuItem>
            ) : null}
            {selectedSubscription.status.type === 'ACTIVE' && selectedSubscription.autoRenew && atPeriodEndCancellationMode ? (
              <MenuItem
                onClick={() => {
                  handleCloseMoreActions();
                  handleDeleteMarketplaceBookingSubscriptionClick(
                    selectedSubscription.id,
                    selectedSubscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription',
                    atPeriodEndCancellationMode.type,
                  );
                }}
              >
                Cancel at period end
              </MenuItem>
            ) : null}
          </>
        ) : null}
      </Menu>

      <Dialog open={!!pendingCancellationConfirmation} onClose={handleCancelImmediateCancellationClick}>
        <DefaultDialogTitle title="Cancel subscription now" />
        <DialogContent sx={{ mt: 2 }}>
          <DialogContentText>
            {`Cancel ${pendingCancellationConfirmation?.productTitle ?? 'this subscription'} now? Future billing will stop immediately. Previous invoices will stay on record.`}
          </DialogContentText>
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
            onPrimaryClicked={handleConfirmImmediateCancellationClick}
            onSecondaryClicked={handleCancelImmediateCancellationClick}
            primaryLabel="Cancel now"
            secondaryLabel="Keep subscription"
          />
        </DialogContent>
      </Dialog>
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationSubscriptions_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [isPending, startTransition] = useTransition();
  const { organizationCustomDomain } = useKnownParams();
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  const [selectedStatuses, setSelectedStatuses] = useState<SupportedMarketplaceBookingSubscriptionStatusForFilter[]>(
    () => searchParams.get('statuses')?.split(',').filter(isSupportedMarketplaceBookingSubscriptionStatusForFilter) ?? [],
  );
  const [selectedPaymentStatuses, setSelectedPaymentStatuses] = useState<SupportedMarketplaceBookingPaymentStatusForFilter[]>(
    () => searchParams.get('paymentStatuses')?.split(',').filter(isSupportedMarketplaceBookingPaymentStatusForFilter) ?? [],
  );
  const [purchaseAfter, setPurchaseAfter] = useState<string | undefined>();
  const purchaseAfterHistory = useRef<Array<string | undefined>>([]);
  const handlePurchaseAfterChange = useCallback(
    (cursor: string | undefined) => {
      if (cursor === undefined) {
        setPurchaseAfter(purchaseAfterHistory.current.pop());
      } else {
        purchaseAfterHistory.current.push(purchaseAfter);
        setPurchaseAfter(cursor);
      }
    },
    [purchaseAfter],
  );
  const [purchaseSourceType, setPurchaseSourceType] = useState(() => searchParams.get('purchaseSourceType') ?? '');
  const [purchaseLifecycleState, setPurchaseLifecycleState] = useState(() => searchParams.get('purchaseLifecycleState') ?? '');
  const [purchasePaymentStatus, setPurchasePaymentStatus] = useState(() => searchParams.get('purchasePaymentStatus') ?? '');
  const [purchaseActivityFrom, setPurchaseActivityFrom] = useState(() => searchParams.get('purchaseActivityFrom') ?? '');
  const [purchaseActivityUntil, setPurchaseActivityUntil] = useState(() => searchParams.get('purchaseActivityUntil') ?? '');
  const [purchaseSort, setPurchaseSort] = useState(() => searchParams.get('purchaseSort') ?? 'ACTIVITY_DESC');
  const initialFormValues = useMemo(
    () => ({
      statuses: selectedStatuses,
      paymentStatuses: selectedPaymentStatuses,
    }),
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [],
  );

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        statuses: selectedStatuses.length > 0 ? selectedStatuses : undefined,
        paymentStatuses: selectedPaymentStatuses.length > 0 ? selectedPaymentStatuses : undefined,
        ...buildMarketplacePurchaseQueryVariables({
          after: purchaseAfter,
          sourceType: purchaseSourceType,
          lifecycleState: purchaseLifecycleState,
          paymentStatus: purchasePaymentStatus,
          activityFrom: purchaseActivityFrom,
          activityUntil: purchaseActivityUntil,
          sort: purchaseSort,
        }),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [
    loadQuery,
    triggerReloadId,
    organizationCustomDomain,
    selectedStatuses,
    selectedPaymentStatuses,
    purchaseAfter,
    purchaseSourceType,
    purchaseLifecycleState,
    purchasePaymentStatus,
    purchaseActivityFrom,
    purchaseActivityUntil,
    purchaseSort,
  ]);

  const handlePurchaseFiltersChange = useCallback(
    (sourceType: string, lifecycleState: string, paymentStatus: string, activityFrom: string, activityUntil: string) => {
      startTransition(() => {
        setPurchaseSourceType(sourceType);
        setPurchaseLifecycleState(lifecycleState);
        setPurchasePaymentStatus(paymentStatus);
        setPurchaseActivityFrom(activityFrom);
        setPurchaseActivityUntil(activityUntil);
        setPurchaseAfter(undefined);
        const qs = updateMarketplacePurchaseSearchParams(searchParams.toString(), {
          sourceType,
          lifecycleState,
          paymentStatus,
          activityFrom,
          activityUntil,
        });
        router.replace(qs ? `?${qs}` : pathname);
      });
    },
    [pathname, router, searchParams],
  );
  const handlePurchaseSortChange = useCallback(
    (sort: string) => {
      setPurchaseSort(sort);
      setPurchaseAfter(undefined);
      const qs = updateMarketplacePurchaseSearchParams(searchParams.toString(), {
        sourceType: purchaseSourceType,
        lifecycleState: purchaseLifecycleState,
        paymentStatus: purchasePaymentStatus,
        activityFrom: purchaseActivityFrom,
        activityUntil: purchaseActivityUntil,
        sort,
      });
      router.replace(`?${qs}`);
    },
    [purchaseActivityFrom, purchaseActivityUntil, purchaseLifecycleState, purchasePaymentStatus, purchaseSourceType, router, searchParams],
  );

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  const handleFiltersChange = useCallback(
    (statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[], paymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[]) => {
      startTransition(() => {
        setSelectedStatuses(statuses);
        setSelectedPaymentStatuses(paymentStatuses);
        const params = new URLSearchParams(searchParams.toString());
        if (statuses.length > 0) {
          params.set('statuses', statuses.join(','));
        } else {
          params.delete('statuses');
        }
        if (paymentStatuses.length > 0) {
          params.set('paymentStatuses', paymentStatuses.join(','));
        } else {
          params.delete('paymentStatuses');
        }
        const qs = params.toString();
        router.replace(qs ? `?${qs}` : pathname);
      });
    },
    [router, searchParams, pathname],
  );

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        onFiltersChange={handleFiltersChange}
        isLoading={isPending}
        initialFormValues={initialFormValues}
        onPurchaseAfterChange={handlePurchaseAfterChange}
        purchaseSourceType={purchaseSourceType}
        purchaseLifecycleState={purchaseLifecycleState}
        purchasePaymentStatus={purchasePaymentStatus}
        purchaseActivityFrom={purchaseActivityFrom}
        purchaseActivityUntil={purchaseActivityUntil}
        onPurchaseFiltersChange={handlePurchaseFiltersChange}
        purchaseSort={purchaseSort}
        onPurchaseSortChange={handlePurchaseSortChange}
      />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
