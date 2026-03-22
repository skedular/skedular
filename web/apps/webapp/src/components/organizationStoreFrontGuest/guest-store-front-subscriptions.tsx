import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { ArrowLeftIcon, PaymentStatusIcon, QuantityIcon } from '@/components/icons';
import { getMarketplaceSubscriptionDetailsLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import type { guestStoreFrontSubscriptions_rootQuery } from '@/queries/__generated__/guestStoreFrontSubscriptions_rootQuery.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<guestStoreFrontSubscriptions_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query guestStoreFrontSubscriptions_rootQuery($organizationCustomDomain: String!) {
    organizationPublic(customDomain: $organizationCustomDomain) {
      name
      marketplaceListingMetadata {
        title
        subTitle
      }
    }
    marketplaceBookingSubscriptions(
      first: 24
      where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, status: ACTIVE }
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
          marketplaceBooking {
            quantity
            paymentStatus {
              type
              name
            }
            paymentMethod {
              name
            }
            productVersion {
              listingMetadata {
                title
                subTitle
              }
            }
          }
          recurringBookings {
            id
            startDate
            endDate
          }
        }
      }
    }
  }
`;

const GuestStoreFrontSubscriptions = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<guestStoreFrontSubscriptions_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const subscriptions = useMemo(
    () => rootData.marketplaceBookingSubscriptions.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item),
    [rootData.marketplaceBookingSubscriptions.edges],
  );

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background: 'radial-gradient(circle at top left, rgba(46, 125, 50, 0.12), transparent 24%), radial-gradient(circle at top right, rgba(23, 93, 175, 0.1), transparent 20%)',
      }}
    >
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        <Card
          sx={{
            borderRadius: 4,
            overflow: 'hidden',
            border: 1,
            borderColor: (theme) => alpha(theme.palette.success.main, 0.18),
            background: (theme) =>
              `linear-gradient(135deg, ${alpha(theme.palette.success.light, 0.12)} 0%, ${alpha(theme.palette.background.paper, 1)} 42%, ${alpha(theme.palette.info.light, 0.08)} 100%)`,
          }}
        >
          <CardContent sx={{ p: { xs: 2.5, md: 3.5 } }}>
            <CaptionIconTypography label="Marketplace subscriptions" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <LeadIconTypography label={`Your active plans at ${rootData.organizationPublic?.name ?? 'this store'}`} sx={{ mt: 0.75 }} />
            <BodyIconTypography
              label="Open any subscription to review its current cycle, recurring periods, payment progress, and the plan you purchased from this storefront."
              sx={{ mt: 0.9, opacity: 0.82, maxWidth: 760 }}
            />

            <StackRow sx={{ mt: 2 }}>
              <Chip label={`${rootData.marketplaceBookingSubscriptions.totalCount} active`} color="success" variant="outlined" />
              {rootData.organizationPublic?.marketplaceListingMetadata.title ? (
                <Chip label={rootData.organizationPublic.marketplaceListingMetadata.title} variant="filled" />
              ) : null}
            </StackRow>
          </CardContent>
        </Card>

        <Box sx={{ mt: 4 }}>
          <CaptionIconTypography label="Current plans" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
          <LeadIconTypography label="Active subscriptions" sx={{ mt: 0.5 }} />

          {subscriptions.length > 0 ? (
            <Box
              sx={{
                mt: 2,
                display: 'grid',
                gap: 1.5,
                gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(3, minmax(0, 1fr))' },
              }}
            >
              {subscriptions.map((subscription) => {
                const subscriptionLink = getMarketplaceSubscriptionDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, subscription.id);
                const latestRecurringBooking = [...subscription.recurringBookings].sort(
                  (left, right) => new Date(right.startDate).getTime() - new Date(left.startDate).getTime(),
                )[0];
                const isConfirmed = subscription.marketplaceBooking.paymentStatus.type === 'CONFIRMED';

                return (
                  <Link
                    key={subscription.id}
                    component={NextLink}
                    href={subscriptionLink}
                    underline="none"
                    color="inherit"
                    sx={{
                      display: 'block',
                      borderRadius: 3,
                      border: 1,
                      borderColor: (theme) => alpha(theme.palette.divider, 0.9),
                      bgcolor: (theme) => alpha(theme.palette.background.paper, 0.86),
                      backdropFilter: 'blur(10px)',
                      transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                      '&:hover': {
                        transform: 'translateY(-2px)',
                        boxShadow: (theme) => theme.shadows[4],
                        borderColor: (theme) => theme.palette.success.main,
                      },
                    }}
                  >
                    <Box sx={{ p: 2.25 }}>
                      <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                        <Box>
                          <SmallIconTypography label="Subscription" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                          <SubtitleIconTypography label={subscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription'} sx={{ mt: 0.4 }} />
                        </Box>
                        <Chip
                          size="small"
                          icon={<PaymentStatusIcon />}
                          label={subscription.marketplaceBooking.paymentStatus.name}
                          color={isConfirmed ? 'success' : 'default'}
                          variant={isConfirmed ? 'filled' : 'outlined'}
                        />
                      </StackRow>

                      <StackColumn spacing={1.1} sx={{ mt: 2 }}>
                        <StackRow sx={{ flexWrap: 'nowrap' }}>
                          <QuantityIcon fontSize="small" />
                          <BodyIconTypography label={`Quantity ${subscription.marketplaceBooking.quantity}`} sx={{ opacity: 0.88 }} />
                        </StackRow>
                        <DetailsRow
                          label="Current period"
                          value={latestRecurringBooking ? `${toStoredDate(latestRecurringBooking.startDate)} - ${toStoredDate(latestRecurringBooking.endDate)}` : 'Preparing cycle'}
                        />
                        <DetailsRow label="Started" value={toStoredDate(subscription.startedAt)} />
                        <DetailsRow
                          label="Next renewal"
                          value={subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : subscription.autoRenew ? 'Not scheduled yet' : 'Ends after this period'}
                        />
                        <DetailsRow label="Payment method" value={subscription.marketplaceBooking.paymentMethod.name} />
                      </StackColumn>

                      <StackRow sx={{ mt: 2, justifyContent: 'space-between', flexWrap: 'nowrap' }}>
                        <BodyIconTypography label="Open subscription" sx={{ color: 'primary.main', fontWeight: 600 }} />
                        <ChevronRightIcon fontSize="small" />
                      </StackRow>
                    </Box>
                  </Link>
                );
              })}
            </Box>
          ) : (
            <Card sx={{ mt: 2, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
              <CardContent sx={{ p: 2.5 }}>
                <BodyIconTypography label="No active subscriptions to show yet." sx={{ opacity: 0.8 }} />
              </CardContent>
            </Card>
          )}
        </Box>
      </Container>
    </Box>
  );
};

const DetailsRow = ({ label, value }: { label: string; value: string }) => (
  <StackColumn spacing={0.35}>
    <SmallIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
    <BodyIconTypography label={value} sx={{ opacity: 0.88 }} />
  </StackColumn>
);

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('dddd, Do MMM YYYY') : '');

const MemoGuestStoreFrontSubscriptions = memo(GuestStoreFrontSubscriptions);

const GuestStoreFrontSubscriptionsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<guestStoreFrontSubscriptions_rootQuery>(RootQuery);
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
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoGuestStoreFrontSubscriptions queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(GuestStoreFrontSubscriptionsWithRelay);
