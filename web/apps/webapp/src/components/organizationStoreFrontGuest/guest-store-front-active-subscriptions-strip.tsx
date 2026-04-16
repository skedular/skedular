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
        height: '100%',
        borderRadius: 4,
        overflow: 'hidden',
        border: 1,
        borderColor: (theme) => alpha(theme.palette.success.main, 0.18),
        background: (theme) =>
          `linear-gradient(135deg, ${alpha(theme.palette.success.light, 0.1)} 0%, ${alpha(theme.palette.background.paper, 1)} 58%, ${alpha(theme.palette.info.light, 0.08)} 100%)`,
      }}
    >
      <CardContent sx={{ p: { xs: 2, md: 2.5 } }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', gap: 1.5 }}>
          <Box sx={{ minWidth: 0 }}>
            <CaptionIconTypography label="Your plans" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <StackRow sx={{ mt: 0.5, alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
              <LeadIconTypography label="Subscriptions" />
              <Chip
                size="small"
                color={subscriptions.length > 0 ? 'success' : 'default'}
                variant={subscriptions.length > 0 ? 'filled' : 'outlined'}
                label={subscriptions.length > 0 ? `${subscriptions.length} active` : 'No active plans'}
              />
            </StackRow>
            <BodyIconTypography
              label={subscriptions.length > 0 ? 'Open a plan to check renewal, billing, and cancellation.' : 'Any active plan you purchase here will appear in this summary.'}
              sx={{ mt: 0.75, opacity: 0.78 }}
            />
          </Box>

          <Button
            component={NextLink}
            href={getMarketplaceSubscriptionsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain)}
            variant="text"
            endIcon={<ChevronRightIcon fontSize="small" />}
            sx={{ textTransform: 'none', whiteSpace: 'nowrap', px: 0, minWidth: 'auto', alignSelf: 'flex-start' }}
          >
            All subscriptions
          </Button>
        </StackRow>

        {subscriptions.length > 0 ? (
          <StackColumn
            sx={{
              mt: 1.75,
              gap: 1,
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
                    bgcolor: (theme) => alpha(theme.palette.background.paper, 0.86),
                    backdropFilter: 'blur(10px)',
                    transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                    '&:hover': {
                      transform: 'translateY(-2px)',
                      boxShadow: (theme) => theme.shadows[3],
                      borderColor: (theme) => theme.palette.success.main,
                    },
                  }}
                >
                  <Box sx={{ p: 1.5 }}>
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

                    <StackColumn spacing={0.8} sx={{ mt: 1.25 }}>
                      <StackRow sx={{ flexWrap: 'nowrap', alignItems: 'center' }}>
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
                  </Box>
                </Link>
              );
            })}
          </StackColumn>
        ) : (
          <Box
            sx={{
              mt: 1.75,
              borderRadius: 3,
              border: 1,
              borderStyle: 'dashed',
              borderColor: (theme) => alpha(theme.palette.success.main, 0.24),
              bgcolor: (theme) => alpha(theme.palette.background.paper, 0.64),
              px: 1.5,
              py: 1.25,
            }}
          >
            <SmallIconTypography label="No active plans yet." sx={{ opacity: 0.72 }} />
          </Box>
        )}
      </CardContent>
    </Card>
  );
};

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('ddd, Do MMM') : '');

export default memo(GuestStoreFrontActiveSubscriptionsStrip);
