import {
  BodyIconTypography,
  DefaultDialogTitle,
  PushToRight,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@/components/commons';
import { getOrganizationBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import {
  SupportedMarketplaceBookingSubscriptionCancellationMode,
  SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  toSupportedMarketplaceBookingSubscriptionCancellationModeDetails,
} from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import { toMarketplaceBookingSubscriptionLifecycleDisplay } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-lifecycle';
import SubscriptionCancellationSection from '@/components/marketplaceProductSubscription/subscription-cancellation-section';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { defaultPadding } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_rootQuery } from '@/queries/__generated__/pageOrganizationSubscriptions_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Grid from '@mui/material/Grid';
import type { SxProps, Theme } from '@mui/system';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { PageHeaderPanel } from '@skedular/ui';

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

const innerPanelSx: SxProps<Theme> = {
  borderRadius: 3,
  px: 1.5,
  py: 1.25,
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.03)' : theme.palette.action.hover),
};

const RootQuery = graphql`
  query pageOrganizationSubscriptions_rootQuery($organizationCustomDomain: String!) {
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
    marketplaceBookingSubscriptions(first: 50, where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: [{ field: NEXT_RENEWAL_AT, direction: ASCENDING }]) {
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

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSubscriptions_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const getCustomerDisplayName = (customer: { name?: string | null; givenName?: string | null; middleName?: string | null; familyName?: string | null }) => {
  const structuredName = [customer.givenName, customer.middleName, customer.familyName].filter(Boolean).join(' ').trim();
  return structuredName || customer.name || 'Customer';
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationSubscriptions_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState<PendingCancellationConfirmation>(null);
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

  const handleBackClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationCustomDomain));
  };

  const handleDeleteMarketplaceBookingSubscriptionClick = (
    subscriptionId: string,
    productTitle: string,
    cancellationModeType: SupportedMarketplaceBookingSubscriptionCancellationMode,
    cancellationModeName: string,
  ) => {
    const toastId = toast(
      <NotificationContent content={`${cancellationModeType === 'AT_PERIOD_END' ? 'Scheduling' : 'Applying'} '${cancellationModeName.toLowerCase()}' for ${productTitle}...`} />,
      infoNotificationOptions,
    );

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
            render: <NotificationContent content={`Failed to update ${productTitle}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: (
            <NotificationContent
              content={
                cancellationModeType === 'AT_PERIOD_END'
                  ? `${productTitle} will end at the end of the current period. Future billing stops, but issued invoices stay on record.`
                  : `${productTitle} cancelled. Future billing stops, but issued invoices stay on record.`
              }
            />
          ),
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update ${productTitle}. Error: ${error.message}.`} />,
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
      pendingCancellationConfirmation.mode.name,
    );
    setPendingCancellationConfirmation(null);
  };

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
            render: <NotificationContent content={`Failed to confirm payment for ${cycleLabel}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment confirmed for ${cycleLabel}.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to confirm payment for ${cycleLabel}. Error: ${error.message}.`} />,
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
            render: <NotificationContent content={`Failed to reject payment for ${cycleLabel}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment rejected for ${cycleLabel}.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject payment for ${cycleLabel}. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleMakeRecurringBookingPaymentNotRequiredClick = (recurringBookingId: string, cycleLabel: string) => {
    const toastId = toast(<NotificationContent content={`Marking payment as not required for ${cycleLabel}...`} />, infoNotificationOptions);

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
            render: <NotificationContent content={`Failed to mark payment as not required for ${cycleLabel}. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment marked as not required for ${cycleLabel}.`} />,
        });

        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to mark payment as not required for ${cycleLabel}. Error: ${error.message}.`} />,
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

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
        <StackColumn sx={{ width: '100%', maxWidth: 1120, mx: 'auto', pb: defaultPadding }} spacing={2}>
          <PageHeaderPanel
            title="Marketplace subscriptions"
            description="Review customer subscriptions, confirm recurring payments, manage refunds, and stop future billing now or at period end."
          />

          {!rootData.organizationBookingPermissions.canModifyPaymentMethod ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <SmallIconTypography label="You do not have permission to confirm subscription payments for this organization." />
            </Box>
          ) : subscriptions.length === 0 ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <SmallIconTypography label="No subscriptions found for this organization." sx={{ opacity: 0.78 }} />
            </Box>
          ) : (
            <Grid container spacing={2}>
              {subscriptions.map((subscription) => {
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

                          <Box sx={innerPanelSx}>
                            <StackColumn spacing={0.75}>
                              <BodyIconTypography label={`Renewal: ${lifecycleDisplay.renewalLabel}`} />
                              <SmallIconTypography
                                label={`Current payment: ${subscription.marketplaceBooking.paymentStatus.name} • Method: ${subscription.marketplaceBooking.paymentMethod.name ?? 'Not set'} • Quantity: ${subscription.marketplaceBooking.quantity}`}
                                sx={{ opacity: 0.78 }}
                              />
                            </StackColumn>
                          </Box>

                          {subscription.status.type === 'ACTIVE' ? (
                            <Box sx={innerPanelSx}>
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
                                    ? handleDeleteMarketplaceBookingSubscriptionClick(
                                        subscription.id,
                                        productTitle,
                                        atPeriodEndCancellationMode.type,
                                        atPeriodEndCancellationMode.name,
                                      )
                                    : undefined
                                }
                              />
                            </Box>
                          ) : null}

                          {subscription.refund ? <MarketplaceRefundAdminPanel entityLabel={`${productTitle} for ${customerLabel}`} refund={subscription.refund} /> : null}

                          <StackColumn spacing={1}>
                            <BodyIconTypography label="Billing periods" />
                            {sortedRecurringBookings.length > 0 ? (
                              sortedRecurringBookings.map((recurringBooking) => {
                                const cycleLabel = `${new Date(recurringBooking.startDate).toLocaleDateString()} - ${
                                  recurringBooking.endDate ? new Date(recurringBooking.endDate).toLocaleDateString() : 'Open ended'
                                }`;
                                const cycleMarketplaceBooking = recurringBooking.marketplaceBooking;

                                return (
                                  <Box key={recurringBooking.id} sx={innerPanelSx}>
                                    <StackColumn spacing={1}>
                                      <StackColumn spacing={0.35}>
                                        <BodyIconTypography label={cycleLabel} />
                                        <SmallIconTypography
                                          label={`Payment: ${cycleMarketplaceBooking?.paymentStatus.name ?? 'Not set'} • Method: ${cycleMarketplaceBooking?.paymentMethod.name ?? 'Not set'} • Quantity: ${cycleMarketplaceBooking?.quantity ?? subscription.marketplaceBooking.quantity}`}
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
                                            Download Invoice
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
                                );
                              })
                            ) : (
                              <Box sx={innerPanelSx}>
                                <SmallIconTypography label="No recurring periods generated yet." sx={{ opacity: 0.72 }} />
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

      <Dialog open={!!pendingCancellationConfirmation} onClose={handleCancelImmediateCancellationClick}>
        <DefaultDialogTitle title="Cancel Subscription Immediately" />
        <DialogContent sx={{ mt: 2 }}>
          <DialogContentText>
            {`Cancel ${pendingCancellationConfirmation?.productTitle ?? 'this subscription'} now? Future billing stops immediately. Issued invoices stay on record.`}
          </DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmImmediateCancellationClick}
            onSecondaryClicked={handleCancelImmediateCancellationClick}
            primaryLabel={pendingCancellationConfirmation?.mode.name ?? 'Immediate'}
            secondaryLabel="Keep Subscription"
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
  const [, startTransition] = useTransition();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
