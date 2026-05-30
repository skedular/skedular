'use client';

import { getOrganizationSubscriptionsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import {
  SupportedMarketplaceBookingSubscriptionCancellationMode,
  SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  toSupportedMarketplaceBookingSubscriptionCancellationModeDetails,
} from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import { toMarketplaceBookingSubscriptionLifecycleDisplay } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-lifecycle';
import SubscriptionCancellationSection from '@/components/marketplaceProductSubscription/subscription-cancellation-section';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import useKnownParams from '@/hooks/use-known-params';
import type { pageOrganizationSubscriptionDetail_confirmRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_confirmRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptionDetail_rejectRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_rejectRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptionDetail_rootQuery } from '@/queries/__generated__/pageOrganizationSubscriptionDetail_rootQuery.graphql';
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
import { getRelayErrorMessage, useIntegratedPlatrform } from '@skedular/shared';
import { BodyIconTypography, DefaultDialogTitle, defaultPadding, PageHeaderPanel, StackColumn, StackRow, SubtitleIconTypography, TwoButtonsDialogActions } from '@skedular/ui';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
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
  query pageOrganizationSubscriptionDetail_rootQuery($organizationCustomDomain: String!, $subscriptionId: String!) {
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
`;

type PendingCancellationConfirmation = {
  subscriptionId: string;
  productTitle: string;
  mode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails;
} | null;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationSubscriptionDetail_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const getCustomerDisplayName = (customer: { name?: string | null; givenName?: string | null; middleName?: string | null; familyName?: string | null }) => {
  const structuredName = [customer.givenName, customer.middleName, customer.familyName].filter(Boolean).join(' ').trim();
  return structuredName || customer.name || 'Customer';
};

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('D MMM YYYY') : 'Not scheduled');

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationSubscriptionDetail_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState<PendingCancellationConfirmation>(null);
  const [commitDeleteMarketplaceBookingSubscription, isDeleteMarketplaceBookingSubscriptionInFlight] =
    useMutation<pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation>(graphql`
      mutation pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
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

  const handleDeleteMarketplaceBookingSubscriptionClick = (
    subscriptionId: string,
    productTitle: string,
    cancellationModeType: SupportedMarketplaceBookingSubscriptionCancellationMode,
  ) => {
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

  const sortedRecurringBookings = [...(subscription?.recurringBookings ?? [])].sort((left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime());
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

  return (
    <RootShell>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
        <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pb: defaultPadding }} spacing={2}>
          <PageHeaderPanel
            title={subscription ? productTitle : 'Subscription'}
            description="Review the full subscription record, manage billing periods, process refunds, and update future billing for this customer."
          />

          {!subscription ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <BodyIconTypography label="We could not find this subscription anymore." />
            </Box>
          ) : (
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
                    <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'wrap', gap: 1 }}>
                      <StackColumn spacing={0.5} sx={{ minWidth: 0 }}>
                        <SubtitleIconTypography label={productTitle} />
                        <BodyIconTypography label={customerLabel} sx={{ opacity: 0.84 }} />
                        <BodyIconTypography
                          label={`Started ${toStoredDate(subscription.startedAt)}${subscription.nextRenewalAt ? ` • Next renewal ${toStoredDate(subscription.nextRenewalAt)}` : ''}`}
                          sx={{ opacity: 0.72 }}
                        />
                      </StackColumn>
                      <Chip label={lifecycleDisplay?.statusLabel ?? subscription.status.name} color={lifecycleDisplay?.statusColor ?? 'default'} variant="outlined" />
                    </StackRow>

                    <Divider sx={{ my: 2 }} />

                    <StackColumn spacing={0.75}>
                      <BodyIconTypography label={`Renewal: ${lifecycleDisplay?.renewalLabel ?? 'Not scheduled'}`} />
                      <BodyIconTypography
                        label={`Current payment: ${subscription.marketplaceBooking.paymentStatus.name} • Payment method: ${subscription.marketplaceBooking.paymentMethod.name ?? 'Not set'} • Quantity: ${subscription.marketplaceBooking.quantity}`}
                        sx={{ opacity: 0.78 }}
                      />
                    </StackColumn>

                    {subscription.status.type === 'ACTIVE' ? (
                      <StackColumn spacing={2} sx={{ mt: 2 }}>
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
                      <StackColumn spacing={2} sx={{ mt: 2 }}>
                        <Divider />
                        <MarketplaceRefundAdminPanel entityLabel={`${productTitle} for ${customerLabel}`} refund={subscription.refund} />
                      </StackColumn>
                    ) : null}
                  </CardContent>
                </Card>

                <Card sx={surfaceSx}>
                  <CardContent sx={{ p: 2.5 }}>
                    <SubtitleIconTypography label="Billing periods" />
                    <BodyIconTypography
                      label="Confirm or reject the payment state for each recurring billing period. Keep these actions in this detail page so the overview stays compact."
                      sx={{ mt: 0.75, opacity: 0.78 }}
                    />

                    <StackColumn spacing={1.25} sx={{ mt: 2 }}>
                      {sortedRecurringBookings.length > 0 ? (
                        sortedRecurringBookings.map((recurringBooking) => {
                          const cycleLabel = `${toStoredDate(recurringBooking.startDate)} - ${recurringBooking.endDate ? toStoredDate(recurringBooking.endDate) : 'Open ended'}`;
                          const cycleMarketplaceBooking = recurringBooking.marketplaceBooking;

                          return (
                            <StackColumn key={recurringBooking.id} spacing={1.25}>
                              <Divider />
                              <StackColumn spacing={1}>
                                <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', gap: 1, flexWrap: 'wrap' }}>
                                  <StackColumn spacing={0.35}>
                                    <BodyIconTypography label={cycleLabel} />
                                    <BodyIconTypography
                                      label={`Payment: ${cycleMarketplaceBooking?.paymentStatus.name ?? 'Not set'} • Payment method: ${cycleMarketplaceBooking?.paymentMethod.name ?? 'Not set'} • Quantity: ${cycleMarketplaceBooking?.quantity ?? subscription.marketplaceBooking.quantity}`}
                                      sx={{ opacity: 0.78 }}
                                    />
                                  </StackColumn>
                                  {cycleMarketplaceBooking?.invoiceUrl ? (
                                    <Button variant="text" size="small" href={cycleMarketplaceBooking.invoiceUrl} target="_blank" rel="noreferrer" sx={{ textTransform: 'none' }}>
                                      Download invoice
                                    </Button>
                                  ) : null}
                                </StackRow>

                                {cycleMarketplaceBooking?.paymentStatus.type === 'PENDING' ? (
                                  <StackRow sx={{ flexWrap: 'wrap', gap: 1 }}>
                                    <Button
                                      variant="contained"
                                      size="small"
                                      sx={{ textTransform: 'none', color: 'white' }}
                                      onClick={() => handleConfirmRecurringBookingPaymentClick(recurringBooking.id, cycleLabel)}
                                    >
                                      Confirm payment
                                    </Button>
                                    <Button
                                      variant="outlined"
                                      color="error"
                                      size="small"
                                      sx={{ textTransform: 'none' }}
                                      onClick={() => handleRejectRecurringBookingPaymentClick(recurringBooking.id, cycleLabel)}
                                    >
                                      Reject payment
                                    </Button>
                                    <Button
                                      variant="text"
                                      size="small"
                                      sx={{ textTransform: 'none' }}
                                      onClick={() => handleMakeRecurringBookingPaymentNotRequiredClick(recurringBooking.id, cycleLabel)}
                                    >
                                      Payment not required
                                    </Button>
                                  </StackRow>
                                ) : null}
                              </StackColumn>
                            </StackColumn>
                          );
                        })
                      ) : (
                        <>
                          <Divider />
                          <BodyIconTypography label="No billing periods have been created for this subscription yet." sx={{ opacity: 0.72 }} />
                        </>
                      )}
                    </StackColumn>
                  </CardContent>
                </Card>
              </StackColumn>

              <Card sx={surfaceSx}>
                <CardContent sx={{ p: 2.5 }}>
                  <SubtitleIconTypography label="Subscription summary" />
                  <StackColumn spacing={1.25} sx={{ mt: 2 }}>
                    <SummaryRow label="Customer" value={customerLabel} />
                    <SummaryRow label="Started" value={toStoredDate(subscription.startedAt)} />
                    <SummaryRow label="Next renewal" value={subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : 'Not scheduled'} />
                    <SummaryRow label="Status" value={lifecycleDisplay?.statusLabel ?? subscription.status.name} />
                    <SummaryRow label="Renewal" value={lifecycleDisplay?.renewalLabel ?? 'Not scheduled'} />
                    <SummaryRow label="Payment" value={subscription.marketplaceBooking.paymentStatus.name} />
                    <SummaryRow label="Payment method" value={subscription.marketplaceBooking.paymentMethod.name ?? 'Not set'} />
                    <SummaryRow label="Quantity" value={`${subscription.marketplaceBooking.quantity}`} />
                  </StackColumn>

                  <Divider sx={{ my: 2 }} />

                  <BodyIconTypography
                    label="Use this page for detailed payment and refund decisions. The subscriptions overview stays focused on scanning status and opening the right record quickly."
                    sx={{ opacity: 0.8 }}
                  />

                  <Box sx={{ mt: 2 }}>
                    <Link component={NextLink} href={getOrganizationSubscriptionsBaseLink(integratedPlatrform, organizationCustomDomain)} underline="hover">
                      Back to all subscriptions
                    </Link>
                  </Box>
                </CardContent>
              </Card>
            </Box>
          )}
        </StackColumn>
      </Box>

      <Dialog open={!!pendingCancellationConfirmation} onClose={() => setPendingCancellationConfirmation(null)}>
        <DefaultDialogTitle title="Cancel subscription now" />
        <DialogContent sx={{ mt: 2 }}>
          <DialogContentText>
            {`Cancel ${pendingCancellationConfirmation?.productTitle ?? 'this subscription'} now? Future billing will stop immediately. Previous invoices will stay on record.`}
          </DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={() => {
              if (!pendingCancellationConfirmation) {
                return;
              }

              handleDeleteMarketplaceBookingSubscriptionClick(
                pendingCancellationConfirmation.subscriptionId,
                pendingCancellationConfirmation.productTitle,
                pendingCancellationConfirmation.mode.type,
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
};

const SummaryRow = ({ label, value }: { label: string; value: string }) => (
  <StackColumn spacing={0.35}>
    <BodyIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase', fontSize: '0.78rem', letterSpacing: '0.06em' }} />
    <BodyIconTypography label={value} sx={{ opacity: 0.88 }} />
  </StackColumn>
);

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationSubscriptionDetail_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
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
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, subscriptionId, triggerReloadId]);

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
