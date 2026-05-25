import { getOrganizationBaseLink, getOrganizationSubscriptionBaseLink } from '@/components/links';
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
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesMarketplaceBookingPaymentStatuses, MultipleChoicesMarketplaceBookingSubscriptionStatuses } from '@/components/organization';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_rootQuery } from '@/queries/__generated__/pageOrganizationSubscriptions_rootQuery.graphql';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import { Breadcrumbs } from '@mui/material';
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
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { getRelayErrorMessage, useIntegratedPlatrform, useKnownParams } from '@skedular/shared';
import {
  BodyIconTypography,
  CollectionToolbar,
  DefaultDialogTitle,
  defaultGridStyle,
  defaultPadding,
  GridContainer,
  PageHeaderPanel,
  PushToRight,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';
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
  query pageOrganizationSubscriptions_rootQuery($organizationCustomDomain: String!, $statuses: [MarketplaceBookingSubscriptionStatus!], $paymentStatuses: [PaymentStatus!]) {
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
};

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSubscriptions_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onFiltersChange: (statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[], paymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[]) => void;
  isLoading: boolean;
  initialFormValues: { statuses: SupportedMarketplaceBookingSubscriptionStatusForFilter[]; paymentStatuses: SupportedMarketplaceBookingPaymentStatusForFilter[] };
};

const getCustomerDisplayName = (customer: { name?: string | null; givenName?: string | null; middleName?: string | null; familyName?: string | null }) => {
  const structuredName = [customer.givenName, customer.middleName, customer.familyName].filter(Boolean).join(' ').trim();
  return structuredName || customer.name || 'Customer';
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain, onFiltersChange, isLoading, initialFormValues }: Props) => {
  const prevFiltersRef = useRef({ statuses: initialFormValues.statuses, paymentStatuses: initialFormValues.paymentStatuses });
  const rootData = usePreloadedQuery<pageOrganizationSubscriptions_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState<PendingCancellationConfirmation>(null);
  const [viewMode, setViewMode] = useState<SubscriptionViewMode>('list');
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<HTMLElement | null>(null);
  const [selectedSubscriptionId, setSelectedSubscriptionId] = useState<string | null>(null);
  const [commitDeleteMarketplaceBookingSubscription, isDeleteMarketplaceBookingSubscriptionInFlight] =
    useMutation<pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation>(graphql`
      mutation pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
        deleteMarketplaceBookingSubscription(input: $input) {
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
          paymentStatusLabel: subscription.marketplaceBooking.paymentStatus.name,
          paymentMethodLabel: subscription.marketplaceBooking.paymentMethod.name ?? 'Not set',
          quantityLabel: `${subscription.marketplaceBooking.quantity}`,
          statusLabel: lifecycleDisplay.statusLabel,
          statusColor: lifecycleDisplay.statusColor,
          hasPendingPayment: sortedRecurringBookings.some((item) => item.marketplaceBooking?.paymentStatus.type === 'PENDING'),
        };
      }),
    [filteredSubscriptions],
  );
  const handleBackClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  const handleOpenSubscriptionClick = useCallback(
    (subscriptionId: string) => {
      router.push(getOrganizationSubscriptionBaseLink(integratedPlatrform, organizationCustomDomain, subscriptionId));
    },
    [integratedPlatrform, organizationCustomDomain, router],
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
  ) => {
    const toastId = toast(<NotificationContent content={`${cancellationModeType === 'AT_PERIOD_END' ? 'Updating' : 'Cancelling'} ${productTitle}...`} />, infoNotificationOptions);

    commitDeleteMarketplaceBookingSubscription({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: subscriptionId,
          cancellationMode: cancellationModeType,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update ${productTitle}. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: (
            <NotificationContent
              content={
                cancellationModeType === 'AT_PERIOD_END'
                  ? `${productTitle} will end at the close of the current billing period. No new charges will be created after that.`
                  : `${productTitle} has been cancelled. No new charges will be created, and past invoices will stay on record.`
              }
            />
          ),
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update ${productTitle}. ${error.message}`} />,
        });
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
    [handleOpenMoreActionsClick, handleOpenSubscriptionClick],
  );

  const handleConfirmRecurringBookingPaymentClick = (recurringBookingId: string, cycleLabel: string) => {
    const toastId = toast(<NotificationContent content={`Confirming payment for ${cycleLabel}...`} />, infoNotificationOptions);

    commitConfirmRecurringBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBookingId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't confirm payment for ${cycleLabel}. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment has been confirmed for ${cycleLabel}.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't confirm payment for ${cycleLabel}. ${error.message}`} />,
        });
      },
    });
  };

  const handleRejectRecurringBookingPaymentClick = (recurringBookingId: string, cycleLabel: string) => {
    const toastId = toast(<NotificationContent content={`Rejecting payment for ${cycleLabel}...`} />, infoNotificationOptions);

    commitRejectRecurringBookingPayment({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBookingId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't reject payment for ${cycleLabel}. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment has been rejected for ${cycleLabel}.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't reject payment for ${cycleLabel}. ${error.message}`} />,
        });
      },
    });
  };

  const handleMakeRecurringBookingPaymentNotRequiredClick = (recurringBookingId: string, cycleLabel: string) => {
    const toastId = toast(<NotificationContent content={`Updating payment settings for ${cycleLabel}...`} />, infoNotificationOptions);

    commitMakeRecurringBookingPaymentNotRequired({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: recurringBookingId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't update payment settings for ${cycleLabel}. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment is no longer required for ${cycleLabel}.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't update payment settings for ${cycleLabel}. ${error.message}`} />,
        });
      },
    });
  };

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Subscriptions" />
          <BodyIconTypography label={rootData.organization?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );
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
                    prevFiltersRef.current = { statuses: newStatuses, paymentStatuses: newPaymentStatuses };
                    onFiltersChange(newStatuses, newPaymentStatuses);
                  }
                }}
              />
              <GridContainer spacing={1} sx={{ alignItems: 'center' }}>
                <Box sx={{ width: 'min(100%, 300px)' }}>
                  <MultipleChoicesMarketplaceBookingSubscriptionStatuses rootDataRelay={rootData} name="statuses" />
                </Box>
                <Box sx={{ width: 'min(100%, 300px)' }}>
                  <MultipleChoicesMarketplaceBookingPaymentStatuses rootDataRelay={rootData} name="paymentStatuses" />
                </Box>
              </GridContainer>
            </>
          )}
        </Form>
      }
      actions={<ListGridToggle defaultValue={viewMode === 'list' ? 'list' : 'grid'} onChange={(view) => setViewMode(view === 'list' ? 'list' : 'card')} />}
    />
  );

  return (
    <RootShell hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
        <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pb: defaultPadding }} spacing={2}>
          <PageHeaderPanel
            title="Marketplace subscriptions"
            description="Review customer subscriptions, update recurring payments, manage refunds, and stop future billing now or at the end of the current period."
          />

          {pageToolbar}

          {isLoading && <LinearProgress />}

          {!rootData.organizationBookingPermissions.canModifyPaymentMethod ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <SmallIconTypography label="You do not have permission to manage subscription payments for this organization." />
            </Box>
          ) : subscriptions.length === 0 ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <SmallIconTypography label="This organization does not have any subscriptions yet." sx={{ opacity: 0.78 }} />
            </Box>
          ) : filteredSubscriptions.length === 0 ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <SmallIconTypography label="No subscriptions match current filters." sx={{ opacity: 0.78 }} />
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
                const sortedRecurringBookings = [...subscription.recurringBookings].sort((left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime());
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
                            <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'wrap', gap: 1 }}>
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
                                label={`Current payment: ${subscription.marketplaceBooking.paymentStatus.name} • Payment method: ${subscription.marketplaceBooking.paymentMethod.name ?? 'Not set'} • Quantity: ${subscription.marketplaceBooking.quantity}`}
                                sx={{ opacity: 0.78 }}
                              />
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

                          {subscription.refund ? <MarketplaceRefundAdminPanel entityLabel={`${productTitle} for ${customerLabel}`} refund={subscription.refund} /> : null}

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

                                          <StackRow sx={{ alignItems: 'center', flexWrap: 'wrap', gap: 1 }}>
                                            {cycleMarketplaceBooking?.invoiceUrl ? (
                                              <Button
                                                variant="text"
                                                size="small"
                                                href={cycleMarketplaceBooking.invoiceUrl}
                                                target="_blank"
                                                rel="noreferrer"
                                                sx={{ textTransform: 'none' }}
                                              >
                                                Download invoice
                                              </Button>
                                            ) : null}

                                            {cycleMarketplaceBooking?.paymentStatus.type === 'PENDING' ? (
                                              <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
                                                <Button
                                                  variant="contained"
                                                  size="small"
                                                  sx={{ textTransform: 'none', color: 'white' }}
                                                  onClick={() => handleConfirmRecurringBookingPaymentClick(recurringBooking.id, cycleLabel)}
                                                >
                                                  Confirm Payment
                                                </Button>
                                                <Button
                                                  variant="outlined"
                                                  color="error"
                                                  size="small"
                                                  sx={{ textTransform: 'none' }}
                                                  onClick={() => handleRejectRecurringBookingPaymentClick(recurringBooking.id, cycleLabel)}
                                                >
                                                  Reject Payment
                                                </Button>
                                                <Button
                                                  variant="text"
                                                  size="small"
                                                  sx={{ textTransform: 'none' }}
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
          )}
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
  const initialFormValues = useMemo(
    () => ({ statuses: selectedStatuses, paymentStatuses: selectedPaymentStatuses }),
    // eslint-disable-next-line react-hooks/exhaustive-deps
    [],
  );

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        statuses: selectedStatuses.length > 0 ? selectedStatuses : undefined,
        paymentStatuses: selectedPaymentStatuses.length > 0 ? selectedPaymentStatuses : undefined,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain, selectedStatuses, selectedPaymentStatuses]);

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
      />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
