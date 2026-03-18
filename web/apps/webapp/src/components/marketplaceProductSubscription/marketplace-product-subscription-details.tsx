import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { ArrowLeftIcon } from '@/components/icons';
import { getMarketplaceProductLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { getCustomerFullName } from '@/libs/utils';
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
import { memo, ReactNode, useEffect, useMemo } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useSubscription } from 'react-relay';
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
  const subscription = rootData.marketplaceBookingSubscription;

  useSubscription({
    variables: { subscriptionId: subscription?.id ?? '' },
    subscription: SubscriptionUpdates,
  });

  const currentCycle = useMemo(
    () => [...(subscription?.recurringBookings ?? [])].sort((left, right) => new Date(right.startDate).getTime() - new Date(left.startDate).getTime())[0] ?? null,
    [subscription?.recurringBookings],
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
              </CardContent>
            </Card>

            <MarketplaceProductBookingPaymentPanel
              checkoutUrl={currentMarketplaceBooking.bookingCheckoutSession?.checkoutUrl ?? null}
              ctaLabel="Pay for plan"
              entityLabel="subscription"
              invoiceUrl={currentMarketplaceBooking.invoiceUrl ?? null}
              isPaymentRequired={currentMarketplaceBooking.isPaymentRequired}
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
