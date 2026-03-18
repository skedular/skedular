import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { getOrganizationBaseLink } from '@/components/links';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { pageOrganizationSubscriptions_rootQuery } from '@/queries/__generated__/pageOrganizationSubscriptions_rootQuery.graphql';
import type { pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_confirmRecurringBookingPaymentMutation.graphql';
import type { pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation.graphql';
import type { pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation } from '@/queries/__generated__/pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Grid from '@mui/material/Grid';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationSubscriptions_rootQuery($organizationCustomDomain: String!) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {
      canViewBookings
      canModifyPaymentMethod
    }
    marketplaceBookingSubscriptions(
      first: 50
      where: { organizationCustomDomains: [$organizationCustomDomain], status: ACTIVE }
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
  const [commitMakeRecurringBookingPaymentNotRequired] =
    useMutation<pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation>(graphql`
      mutation pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation($input: MakeRecurringBookingPaymentNotRequiredInput!)
      @raw_response_type {
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

  const handleBackClick = () => {
    router.push(getOrganizationBaseLink(integratedPlatrform, organizationCustomDomain));
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
            render: <NotificationContent content={`Failed to confirm payment for ${cycleLabel}. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to reject payment for ${cycleLabel}. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to mark payment as not required for ${cycleLabel}. Error: ${joinErrors(errors)}.`} />,
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
      <Box sx={{ p: 2 }}>
        <StackColumn spacing={1}>
          <LeadIconTypography label="Marketplace subscriptions" />
          <BodyIconTypography label="Review customer subscriptions and confirm payment on a specific recurring period when the customer has paid outside of hosted checkout." />
        </StackColumn>

        {!rootData.organizationBookingPermissions.canModifyPaymentMethod && (
          <Card sx={{ mt: 2 }}>
            <CardContent>
              <SmallIconTypography label="You do not have permission to confirm subscription payments for this organization." />
            </CardContent>
          </Card>
        )}

        {rootData.organizationBookingPermissions.canModifyPaymentMethod && (
          <Grid container spacing={2} sx={{ mt: 1 }}>
            {subscriptions.length > 0 ? (
              subscriptions.map((subscription) => {
                const sortedRecurringBookings = [...subscription.recurringBookings].sort(
                  (left, right) => new Date(left.startDate).getTime() - new Date(right.startDate).getTime(),
                );

                return (
                  <Grid key={subscription.id} size={{ xs: 12 }}>
                    <Card>
                      <CardContent>
                        <StackRow sx={{ alignItems: 'flex-start', flexWrap: 'wrap', gap: 1 }}>
                          <StackColumn spacing={0.4}>
                            <SubtitleIconTypography label={subscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription'} />
                            <SmallIconTypography
                              label={
                                subscription.involvedCustomers.length > 0
                                  ? `Customer: ${getCustomerDisplayName(subscription.involvedCustomers[0])}`
                                  : 'Customer unavailable'
                              }
                              sx={{ opacity: 0.82 }}
                            />
                            <SmallIconTypography
                              label={`Started ${new Date(subscription.startedAt).toLocaleDateString()}${subscription.nextRenewalAt ? ` • Next renewal ${new Date(subscription.nextRenewalAt).toLocaleDateString()}` : ''}`}
                              sx={{ opacity: 0.72 }}
                            />
                          </StackColumn>
                          <PushToRight />
                          <Chip
                            label={subscription.marketplaceBooking.paymentStatus.name}
                            color={subscription.marketplaceBooking.paymentStatus.type === 'CONFIRMED' ? 'success' : 'warning'}
                            variant="outlined"
                          />
                        </StackRow>

                        <StackColumn spacing={1} sx={{ mt: 2 }}>
                          {sortedRecurringBookings.length > 0 ? (
                            sortedRecurringBookings.map((recurringBooking) => {
                              const cycleLabel = `${new Date(recurringBooking.startDate).toLocaleDateString()} - ${
                                recurringBooking.endDate ? new Date(recurringBooking.endDate).toLocaleDateString() : 'Open ended'
                              }`;
                              const cycleMarketplaceBooking = recurringBooking.marketplaceBooking;

                              return (
                                <Box
                                  key={recurringBooking.id}
                                  sx={{
                                    border: 1,
                                    borderColor: (theme) => theme.palette.divider,
                                    borderRadius: 2,
                                    p: 1.5,
                                  }}
                                >
                                  <StackRow sx={{ alignItems: 'center', flexWrap: 'wrap', gap: 1 }}>
                                    <StackColumn spacing={0.3}>
                                      <BodyIconTypography label={cycleLabel} />
                                      <SmallIconTypography
                                        label={`Payment: ${cycleMarketplaceBooking?.paymentStatus.name ?? 'Not set'} • Method: ${cycleMarketplaceBooking?.paymentMethod.name ?? 'Not set'} • Quantity: ${cycleMarketplaceBooking?.quantity ?? subscription.marketplaceBooking.quantity}`}
                                        sx={{ opacity: 0.78 }}
                                      />
                                    </StackColumn>
                                    <PushToRight />
                                    {cycleMarketplaceBooking?.paymentStatus.type === 'PENDING' && (
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
                                    )}
                                  </StackRow>
                                </Box>
                              );
                            })
                          ) : (
                            <SmallIconTypography label="No recurring periods generated yet." sx={{ opacity: 0.72 }} />
                          )}
                        </StackColumn>
                      </CardContent>
                    </Card>
                  </Grid>
                );
              })
            ) : (
              <Grid size={{ xs: 12 }}>
                <Card>
                  <CardContent>
                    <SmallIconTypography label="No active subscriptions found for this organization." sx={{ opacity: 0.78 }} />
                  </CardContent>
                </Card>
              </Grid>
            )}
          </Grid>
        )}
      </Box>
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
