import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { ArrowLeftIcon, LocationIcon, PaymentStatusIcon, QuantityIcon, ResourceIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink, getMarketplaceProductLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { convertCalendarDayToStartOfDay, getCustomerFullName } from '@/libs/utils';
import type { marketplaceProductSubscriptionDetails_relatedBookingsQuery } from '@/queries/__generated__/marketplaceProductSubscriptionDetails_relatedBookingsQuery.graphql';
import type { marketplaceProductSubscriptionDetails_rootQuery } from '@/queries/__generated__/marketplaceProductSubscriptionDetails_rootQuery.graphql';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, ReactNode, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useLazyLoadQuery, usePreloadedQuery, useQueryLoader, useSubscription } from 'react-relay';
import MarketplaceProductBookingDetailsHero from '../marketplaceProductBooking/marketplace-product-booking-details-hero';
import MarketplaceProductBookingPaymentPanel from '../marketplaceProductBooking/marketplace-product-booking-payment-panel';

const RootQuery = graphql`
  query marketplaceProductSubscriptionDetails_rootQuery($subscriptionId: String!) {
    marketplaceBookingSubscription(id: $subscriptionId) {
      id
      startedAt
      nextRenewalAt
      autoRenew
      cancelAtPeriodEnd
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
      recurringBookings {
        id
        startDate
        endDate
        marketplaceBooking {
          id
          quantity
          invoiceUrl
          isPaymentRequired
          paymentExpiry
          productVersion {
            id
            listingMetadata {
              title
              subTitle
              about
              includedFeatures
            }
            featureImages {
              original {
                url
              }
            }
            organization {
              customerFacingTermsAndConditionsUrl
            }
          }
          bookingCheckoutSession {
            checkoutUrl
          }
          paymentMethod {
            type
            name
          }
          paymentStatus {
            type
            name
          }
        }
      }
      arrearsInvoices {
        invoiceNumber
        invoiceUrl
        billingPeriodStartInclusive
        billingPeriodEndExclusive
      }
    }
  }
`;

const RelatedBookingsQuery = graphql`
  query marketplaceProductSubscriptionDetails_relatedBookingsQuery(
    $organizationCustomDomain: String!
    $recurringBookingIds: [String!]
    $relatedBookingsFirst: Int!
    $today: DateTime!
  ) {
    bookings(
      first: $relatedBookingsFirst
      where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, channel: MARKETPLACE, recurringBookingIds: $recurringBookingIds, fromGte: $today }
      orderBy: [{ field: FROM, direction: ASCENDING }]
    ) {
      totalCount
      edges {
        node {
          id
          recurringBooking {
            id
          }
          from
          until
          involvedLocations {
            name
          }
          bookingResources {
            resource {
              id
              name
            }
          }
          marketplaceBooking {
            quantity
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  }
`;

const SubscriptionUpdates = graphql`
  subscription marketplaceProductSubscriptionDetails_subscription_Subscription($subscriptionId: String!) {
    marketplaceBookingSubscription(id: $subscriptionId) {
      id
      startedAt
      nextRenewalAt
      autoRenew
      cancelAtPeriodEnd
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
      recurringBookings {
        id
        startDate
        endDate
        marketplaceBooking {
          id
          quantity
          invoiceUrl
          isPaymentRequired
          paymentExpiry
          productVersion {
            id
            listingMetadata {
              title
              subTitle
              about
              includedFeatures
            }
            featureImages {
              original {
                url
              }
            }
            organization {
              customerFacingTermsAndConditionsUrl
            }
          }
          bookingCheckoutSession {
            checkoutUrl
          }
          paymentMethod {
            type
            name
          }
          paymentStatus {
            type
            name
          }
        }
      }
      arrearsInvoices {
        invoiceNumber
        invoiceUrl
        billingPeriodStartInclusive
        billingPeriodEndExclusive
      }
    }
  }
`;

const MarketplaceProductSubscriptionDetails = ({
  queryReference,
}: {
  queryReference: PreloadedQuery<marketplaceProductSubscriptionDetails_rootQuery, Record<string, unknown>>;
}) => {
  const rootData = usePreloadedQuery<marketplaceProductSubscriptionDetails_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const [relatedBookingsFirst, setRelatedBookingsFirst] = useState(8);
  const subscription = rootData.marketplaceBookingSubscription;
  const today = useMemo(() => convertCalendarDayToStartOfDay(dayjs()).toISOString(), []);

  useSubscription({
    variables: { subscriptionId: subscription?.id ?? '' },
    subscription: SubscriptionUpdates,
  });

  const currentCycle = useMemo(
    () => [...(subscription?.recurringBookings ?? [])].sort((left, right) => new Date(right.startDate).getTime() - new Date(left.startDate).getTime())[0] ?? null,
    [subscription?.recurringBookings],
  );
  const recurringBookingIds = useMemo(() => subscription?.recurringBookings.map((item) => item.id) ?? [], [subscription?.recurringBookings]);
  const relatedBookingsData = useLazyLoadQuery<marketplaceProductSubscriptionDetails_relatedBookingsQuery>(
    RelatedBookingsQuery,
    {
      organizationCustomDomain,
      recurringBookingIds,
      relatedBookingsFirst,
      today,
    },
    {
      fetchPolicy: 'store-and-network',
    },
  );
  const relatedBookings = useMemo(
    () => relatedBookingsData.bookings?.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item) ?? [],
    [relatedBookingsData.bookings?.edges],
  );
  const currentMarketplaceBooking = currentCycle?.marketplaceBooking ?? null;
  const productVersion = currentMarketplaceBooking?.productVersion ?? null;

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
      }}
    >
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        {productVersion ? (
          <MarketplaceProductBookingDetailsHero
            about={productVersion.listingMetadata.about}
            imageUrl={productVersion.featureImages[0]?.original?.url}
            includedFeatures={productVersion.listingMetadata.includedFeatures}
            subTitle={productVersion.listingMetadata.subTitle}
            title={productVersion.listingMetadata.title}
          />
        ) : null}

        {!subscription ? (
          <Alert severity="info" sx={{ mt: 2, borderRadius: 3 }}>
            We couldn&apos;t find this subscription anymore.
          </Alert>
        ) : !currentCycle || !currentMarketplaceBooking ? (
          <Box
            sx={{
              mt: 1,
              display: 'grid',
              gap: { xs: 3, lg: 4 },
              gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.15fr) 380px' },
              alignItems: 'start',
            }}
          >
            <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
              <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
                <CaptionIconTypography label="Subscription details" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                <LeadIconTypography label="Preparing your current subscription period" sx={{ mt: 1 }} />
                <BodyIconTypography
                  label="Your subscription was created successfully. We’re still preparing the current cycle details and payment state for this period."
                  sx={{ mt: 1, opacity: 0.82 }}
                />

                <StackRow sx={{ mt: 2, rowGap: 1 }}>
                  <Chip label={subscription.status.name} color={subscription.status.type === 'ACTIVE' ? 'success' : 'default'} />
                </StackRow>

                <StackColumn spacing={2} sx={{ mt: 3 }}>
                  <DetailsRow label="Started" value={toStoredDate(subscription.startedAt)} />
                  <DetailsRow label="Next renewal" value={subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : 'Not scheduled'} />
                  <DetailsRow label="Booked for" value={subscription.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ') || 'Not available'} />
                  <DetailsRow label="Renewal" value={subscription.autoRenew ? 'Auto-renew on' : 'Ends after this period'} />
                </StackColumn>
              </CardContent>
            </Card>

            <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
              <CardContent sx={{ p: 2.5 }}>
                <CaptionIconTypography label="Payment progress" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
                <LeadIconTypography label="Setting up payment" sx={{ mt: 1 }} />
                <BodyIconTypography label="This page updates automatically as soon as the current subscription cycle and payment link are ready." sx={{ mt: 1, opacity: 0.82 }} />
              </CardContent>
            </Card>
          </Box>
        ) : (
          <Box
            sx={{
              mt: 1,
              display: 'grid',
              gap: { xs: 3, lg: 4 },
              gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.15fr) 380px' },
              alignItems: 'start',
            }}
          >
            <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
              <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
                <CaptionIconTypography label="Subscription details" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                <LeadIconTypography label="Review this subscription period and payment status" sx={{ mt: 1 }} />
                <BodyIconTypography
                  label="This page stays in sync while the current cycle payment is being prepared, and it remains the place to return to after payment."
                  sx={{ mt: 1, opacity: 0.82 }}
                />

                <StackRow sx={{ mt: 2, rowGap: 1 }}>
                  <Chip label={subscription.status.name} color={subscription.status.type === 'ACTIVE' ? 'success' : 'default'} />
                  <Chip label={currentMarketplaceBooking.paymentMethod.name} variant="outlined" />
                </StackRow>

                <StackColumn spacing={2} sx={{ mt: 3 }}>
                  <DetailsRow label="Current period" value={`${toStoredDate(currentCycle.startDate)} - ${toStoredDate(currentCycle.endDate)}`} />
                  <DetailsRow label="Started" value={toStoredDate(subscription.startedAt)} />
                  <DetailsRow label="Next renewal" value={subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : 'Not scheduled'} />
                  <DetailsRow label="Quantity" value={`${currentMarketplaceBooking.quantity}`} />
                  <DetailsRow label="Booked for" value={subscription.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ') || 'Not available'} />
                  <DetailsRow label="Renewal" value={subscription.autoRenew ? 'Auto-renew on' : 'Ends after this period'} />
                  {productVersion ? (
                    <DetailsRow
                      label="Product"
                      value={
                        <Link
                          component={NextLink}
                          href={getMarketplaceProductLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, productVersion.id)}
                          underline="none"
                          color="inherit"
                          sx={{
                            display: 'inline-flex',
                            width: 'fit-content',
                            px: 1.25,
                            py: 0.75,
                            borderRadius: 2,
                            bgcolor: (theme) => theme.palette.action.hover,
                            border: 1,
                            borderColor: (theme) => theme.palette.divider,
                            transition: 'background-color 120ms ease, border-color 120ms ease, transform 120ms ease',
                            '&:hover': {
                              bgcolor: (theme) => theme.palette.action.selected,
                              borderColor: (theme) => theme.palette.primary.main,
                              transform: 'translateY(-1px)',
                            },
                          }}
                        >
                          <SubtitleIconTypography label={productVersion.listingMetadata.title ?? 'View product'} />
                        </Link>
                      }
                    />
                  ) : null}
                  {productVersion?.organization.customerFacingTermsAndConditionsUrl ? (
                    <DetailsRow
                      label="Terms and conditions"
                      value={
                        <Link href={productVersion.organization.customerFacingTermsAndConditionsUrl} target="_blank" rel="noreferrer" underline="hover">
                          Review pricing terms and conditions
                        </Link>
                      }
                    />
                  ) : null}
                </StackColumn>

                <Box sx={{ mt: 4 }}>
                  <CaptionIconTypography label="Included periods" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                  <LeadIconTypography label="Recurring periods in this subscription" sx={{ mt: 0.75 }} />
                  <Box
                    sx={{
                      mt: 2,
                      display: 'grid',
                      gap: 1.25,
                      gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' },
                    }}
                  >
                    {[...subscription.recurringBookings]
                      .sort((left, right) => new Date(right.startDate).getTime() - new Date(left.startDate).getTime())
                      .map((recurringBooking) => {
                        const isCurrentCycle = recurringBooking.id === currentCycle.id;

                        return (
                          <Card key={recurringBooking.id} sx={{ borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
                            <CardContent sx={{ p: 2 }}>
                              <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                                <Box>
                                  <SmallIconTypography
                                    label={isCurrentCycle ? 'Current period' : 'Subscription period'}
                                    sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }}
                                  />
                                  <SubtitleIconTypography label={`${toStoredDate(recurringBooking.startDate)} - ${toStoredDate(recurringBooking.endDate)}`} sx={{ mt: 0.35 }} />
                                </Box>
                                <Chip
                                  size="small"
                                  icon={<PaymentStatusIcon />}
                                  label={isCurrentCycle ? currentMarketplaceBooking.paymentStatus.name : (recurringBooking.marketplaceBooking?.paymentStatus.name ?? 'Preparing')}
                                  color={isCurrentCycle && currentMarketplaceBooking.paymentStatus.type === 'CONFIRMED' ? 'success' : 'default'}
                                  variant={isCurrentCycle && currentMarketplaceBooking.paymentStatus.type === 'CONFIRMED' ? 'filled' : 'outlined'}
                                />
                              </StackRow>
                              {recurringBooking.marketplaceBooking?.invoiceUrl ? (
                                <Link
                                  href={recurringBooking.marketplaceBooking.invoiceUrl}
                                  target="_blank"
                                  rel="noreferrer"
                                  underline="hover"
                                  sx={{ mt: 1.25, display: 'inline-flex' }}
                                >
                                  Download invoice
                                </Link>
                              ) : null}
                            </CardContent>
                          </Card>
                        );
                      })}
                  </Box>
                </Box>

                <Box sx={{ mt: 4 }}>
                  <CaptionIconTypography label="Related bookings" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                  <LeadIconTypography label="Booking instances created from this subscription" sx={{ mt: 0.75 }} />
                  {relatedBookings.length > 0 ? (
                    <>
                      <Box
                        sx={{
                          mt: 2,
                          display: 'grid',
                          gap: 1.25,
                          gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' },
                        }}
                      >
                        {relatedBookings.map((booking) => {
                          const bookingLink = getMarketplaceBookingDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, booking.id);
                          const locationLabel = booking.involvedLocations[0]?.name ?? 'Location to be confirmed';
                          const resourcesLabel = booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
                          const isConfirmed = booking.marketplaceBooking?.paymentStatus.type === 'CONFIRMED';
                          const isTodayBooking = dayjs.utc(booking.from).isSame(dayjs.utc(today), 'day');

                          return (
                            <Link
                              key={booking.id}
                              component={NextLink}
                              href={bookingLink}
                              underline="none"
                              color="inherit"
                              sx={{
                                display: 'block',
                                borderRadius: 3,
                                border: 1,
                                borderColor: (theme) => (isTodayBooking ? theme.palette.primary.main : theme.palette.divider),
                                bgcolor: (theme) => (isTodayBooking ? theme.palette.action.selected : theme.palette.background.paper),
                                transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                                '&:hover': {
                                  transform: 'translateY(-2px)',
                                  boxShadow: (theme) => theme.shadows[3],
                                  borderColor: (theme) => theme.palette.primary.main,
                                },
                              }}
                            >
                              <Box sx={{ p: 2 }}>
                                <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                                  <Box>
                                    <SmallIconTypography label={toStoredDate(booking.from)} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                                    <SubtitleIconTypography label={toStoredTimeRange(booking.from, booking.until)} sx={{ mt: 0.35 }} />
                                  </Box>
                                  <StackColumn spacing={0.75} sx={{ alignItems: 'flex-end' }}>
                                    {isTodayBooking ? <Chip size="small" label="Today" color="primary" /> : null}
                                    <Chip
                                      size="small"
                                      icon={<PaymentStatusIcon />}
                                      label={booking.marketplaceBooking?.paymentStatus.name ?? currentMarketplaceBooking.paymentStatus.name}
                                      color={isConfirmed ? 'success' : 'default'}
                                      variant={isConfirmed ? 'filled' : 'outlined'}
                                    />
                                  </StackColumn>
                                </StackRow>

                                <StackColumn spacing={1} sx={{ mt: 2 }}>
                                  <StackRow sx={{ flexWrap: 'nowrap' }}>
                                    <LocationIcon fontSize="small" />
                                    <BodyIconTypography label={locationLabel} sx={{ opacity: 0.88 }} />
                                  </StackRow>
                                  <StackRow sx={{ flexWrap: 'nowrap' }}>
                                    <QuantityIcon fontSize="small" />
                                    <BodyIconTypography label={`Quantity ${booking.marketplaceBooking?.quantity ?? currentMarketplaceBooking.quantity}`} sx={{ opacity: 0.88 }} />
                                  </StackRow>
                                  <StackRow sx={{ flexWrap: 'nowrap' }}>
                                    <ResourceIcon fontSize="small" />
                                    <BodyIconTypography label={resourcesLabel} sx={{ opacity: 0.88 }} />
                                  </StackRow>
                                </StackColumn>
                              </Box>
                            </Link>
                          );
                        })}
                      </Box>

                      {relatedBookingsData.bookings.totalCount > relatedBookings.length ? (
                        <Button variant="text" onClick={() => setRelatedBookingsFirst((current) => current + 8)} sx={{ mt: 1.5, textTransform: 'none', px: 0 }}>
                          Show more bookings
                        </Button>
                      ) : null}
                    </>
                  ) : (
                    <Card sx={{ mt: 2, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
                      <CardContent sx={{ p: 2.5 }}>
                        <BodyIconTypography label="No booking instances have been created for this subscription yet." sx={{ opacity: 0.8 }} />
                      </CardContent>
                    </Card>
                  )}
                </Box>
              </CardContent>
            </Card>

            <MarketplaceProductBookingPaymentPanel
              checkoutUrl={currentMarketplaceBooking.bookingCheckoutSession?.checkoutUrl ?? null}
              ctaLabel="Pay for plan"
              entityLabel="subscription"
              invoices={subscription.arrearsInvoices ?? []}
              invoiceUrl={currentMarketplaceBooking.invoiceUrl ?? null}
              isPaymentRequired={currentMarketplaceBooking.isPaymentRequired}
              pendingStatusMessage="Payment status: Pending. The first invoice for this subscription period is ready here now and can be paid using the selected payment method."
              paymentExpiry={currentMarketplaceBooking.paymentExpiry}
              paymentMethodType={currentMarketplaceBooking.paymentMethod.type}
              paymentStatusLabel={currentMarketplaceBooking.paymentStatus.name}
              paymentStatusType={currentMarketplaceBooking.paymentStatus.type}
            />
          </Box>
        )}
      </Container>
    </Box>
  );
};

const DetailsRow = ({ label, value }: { label: string; value: ReactNode }) => (
  <StackColumn spacing={0.35}>
    <SmallIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
    {typeof value === 'string' ? <SubtitleIconTypography label={value} /> : value}
  </StackColumn>
);

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('dddd, Do MMM YYYY') : '');
const toStoredTime = (date?: string | null) => (date ? dayjs.utc(date).format('hh:mm a') : '');
const toStoredTimeRange = (from?: string | null, until?: string | null) => `${toStoredTime(from)} - ${toStoredTime(until)}`;

const MemoMarketplaceProductSubscriptionDetails = memo(MarketplaceProductSubscriptionDetails);

const MarketplaceProductSubscriptionDetailsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductSubscriptionDetails_rootQuery>(RootQuery);
  const { subscriptionId } = useKnownParams();

  if (!subscriptionId) {
    throw new Error('subscriptionId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        subscriptionId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, subscriptionId]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMarketplaceProductSubscriptionDetails queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductSubscriptionDetailsWithRelay);
