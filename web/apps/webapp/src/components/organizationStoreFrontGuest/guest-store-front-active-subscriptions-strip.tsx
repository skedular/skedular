import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { PaymentStatusIcon, QuantityIcon } from '@/components/icons';
import { getMarketplaceSubscriptionDetailsLink, getMarketplaceSubscriptionsLink } from '@/components/links';
import { toMarketplaceBookingSubscriptionLifecycleDisplay } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-lifecycle';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import type { guestStoreFrontActiveSubscriptionsStrip_query$key } from '@/queries/__generated__/guestStoreFrontActiveSubscriptionsStrip_query.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontActiveSubscriptionsStrip_query$key;
};

const GuestStoreFrontActiveSubscriptionsStrip = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment guestStoreFrontActiveSubscriptionsStrip_query on Query
      @argumentDefinitions(includeActiveSubscriptions: { type: "Boolean!", defaultValue: false }, organizationCustomDomain: { type: "String!" }) {
        marketplaceBookingSubscriptions(
          first: 3
          where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, status: ACTIVE }
          orderBy: [{ field: NEXT_RENEWAL_AT, direction: ASCENDING }]
        ) @include(if: $includeActiveSubscriptions) {
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
    `,
    rootDataRelay,
  );
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const subscriptions = useMemo(
    () => rootData.marketplaceBookingSubscriptions?.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item) ?? [],
    [rootData.marketplaceBookingSubscriptions?.edges],
  );

  if (!rootData.marketplaceBookingSubscriptions) {
    return null;
  }

  return (
    <Card
      sx={{
        mt: 2,
        borderRadius: 4,
        overflow: 'hidden',
        border: 1,
        borderColor: (theme) => alpha(theme.palette.success.main, 0.18),
        background: (theme) =>
          `linear-gradient(135deg, ${alpha(theme.palette.success.light, 0.12)} 0%, ${alpha(theme.palette.background.paper, 1)} 46%, ${alpha(theme.palette.info.light, 0.08)} 100%)`,
      }}
    >
      <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ justifyContent: 'space-between', alignItems: { xs: 'flex-start', md: 'center' } }}>
          <Box sx={{ maxWidth: 720 }}>
            <CaptionIconTypography label="Your plans" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <LeadIconTypography label="Active subscriptions" sx={{ mt: 0.75 }} />
            <BodyIconTypography
              label={
                subscriptions.length > 0
                  ? 'Open a plan to review billing, renewal, and cancellation. Stopping a plan ends future billing, but issued invoices stay on record.'
                  : 'Any active plan you purchase here will appear in this section so you can reopen its billing, renewal, and cancellation details.'
              }
              sx={{ mt: 0.75, opacity: 0.82 }}
            />
          </Box>

          <Button
            component={NextLink}
            href={getMarketplaceSubscriptionsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain)}
            variant="text"
            sx={{ textTransform: 'none', whiteSpace: 'nowrap' }}
          >
            View all subscriptions
          </Button>
        </Stack>

        {subscriptions.length > 0 ? (
          <Box
            sx={{
              mt: 2.5,
              display: 'grid',
              gap: 1.5,
              gridTemplateColumns: { xs: '1fr', md: 'repeat(3, minmax(0, 1fr))' },
            }}
          >
            {subscriptions.map((subscription) => {
              const subscriptionLink = getMarketplaceSubscriptionDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, subscription.id);
              const latestRecurringBooking = [...subscription.recurringBookings].sort((left, right) => new Date(right.startDate).getTime() - new Date(left.startDate).getTime())[0];
              const productTitle = subscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription';
              const paymentStatusType = subscription.marketplaceBooking.paymentStatus.type;
              const isConfirmed = paymentStatusType === 'CONFIRMED';
              const isPending = paymentStatusType === 'PENDING';
              const lifecycleDisplay = toMarketplaceBookingSubscriptionLifecycleDisplay({
                autoRenew: subscription.autoRenew,
                cancelAtPeriodEnd: subscription.cancelAtPeriodEnd,
              });

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
                    bgcolor: (theme) => alpha(theme.palette.background.paper, 0.82),
                    backdropFilter: 'blur(10px)',
                    transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                    '&:hover': {
                      transform: 'translateY(-2px)',
                      boxShadow: (theme) => theme.shadows[4],
                      borderColor: (theme) => theme.palette.success.main,
                    },
                  }}
                >
                  <Box sx={{ p: 2 }}>
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                      <Box>
                        <SmallIconTypography label="Active plan" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                        <SubtitleIconTypography label={productTitle} sx={{ mt: 0.4 }} />
                      </Box>
                      <Chip
                        size="small"
                        icon={<PaymentStatusIcon />}
                        label={subscription.marketplaceBooking.paymentStatus.name}
                        color={isConfirmed ? 'success' : isPending ? 'warning' : 'default'}
                        variant={isConfirmed || isPending ? 'filled' : 'outlined'}
                      />
                    </StackRow>

                    <StackColumn spacing={1.1} sx={{ mt: 2 }}>
                      <StackRow sx={{ flexWrap: 'nowrap' }}>
                        <QuantityIcon fontSize="small" />
                        <BodyIconTypography label={`Quantity ${subscription.marketplaceBooking.quantity}`} sx={{ opacity: 0.88 }} />
                      </StackRow>
                      <StackColumn spacing={0.35}>
                        <SmallIconTypography label="Current period" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                        <BodyIconTypography
                          label={latestRecurringBooking ? `${toStoredDate(latestRecurringBooking.startDate)} - ${toStoredDate(latestRecurringBooking.endDate)}` : 'Preparing cycle'}
                          sx={{ opacity: 0.88 }}
                        />
                      </StackColumn>
                      <StackColumn spacing={0.35}>
                        <SmallIconTypography label="Next renewal" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                        <BodyIconTypography
                          label={subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : lifecycleDisplay.nextRenewalFallbackLabel}
                          sx={{ opacity: 0.88 }}
                        />
                      </StackColumn>
                      <StackColumn spacing={0.35}>
                        <SmallIconTypography label="Cancellation" sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                        <BodyIconTypography label={lifecycleDisplay.renewalLabel} sx={{ opacity: 0.88 }} />
                      </StackColumn>
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
        ) : null}
      </CardContent>
    </Card>
  );
};

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('ddd, Do MMM') : '');

export default memo(GuestStoreFrontActiveSubscriptionsStrip);
